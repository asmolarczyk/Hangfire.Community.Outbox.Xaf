namespace Hangfire.Community.Outbox.Xaf.Services;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using Entities;
using Extensions;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class OutboxProcessor: IOutboxProcessor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly HangfireOutboxOptions _options;

    public OutboxProcessor(IServiceScopeFactory serviceScopeFactory, ILogger<OutboxProcessor> logger, HangfireOutboxOptions options)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _options = options;
    }
    
    public async Task Process(CancellationToken ct)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var osFactory = scope.ServiceProvider.GetRequiredService<INonSecuredObjectSpaceFactory>();
        using var os = osFactory.CreateNonSecuredObjectSpace<OutboxJob>();
        
        var toProcess = await os.GetObjectsQuery<OutboxJob>()
            .Where(x => !x.Processed && x.Exception == null)
            .OrderBy(x => x.CreatedOn)
            .Take(_options.OutboxProcessorBatchSize)
            .ToArrayAsync(ct);

        if (toProcess.Length == 0)
        {
            return;
        }

        var typesInfo = scope.ServiceProvider.GetRequiredService<ITypesInfo>();
        var jobTypesInfo = typesInfo.FindTypeInfo(typeof(OutboxJob));
        var processedMemberInfo = jobTypesInfo.FindMember(nameof(OutboxJob.Processed));
        var jobIdMemberInfo = jobTypesInfo.FindMember(nameof(OutboxJob.HangfireJobId));
        var exceptionMemberInfo = jobTypesInfo.FindMember(nameof(OutboxJob.Exception));

        var backgroundJobClient = scope.ServiceProvider.GetRequiredService<IBackgroundJobClient>();

        _logger.LogDebug("Processing {nbJobs} outbox jobs", toProcess.Length);
        
        foreach (var outboxMessage in toProcess)
        {
            if (ct.IsCancellationRequested)
            {
                _logger.LogDebug("Cancellation requested");
                return;
            }
            
            try
            {
                _logger.LogDebug("Processing outbox job {id}", outboxMessage.Oid);
                
                var jobType = outboxMessage.GetJobType();
                var job = new Job(jobType, outboxMessage.GetMethod(), outboxMessage.GetArguments().ToArray(), outboxMessage.Queue);

                string jobId = null;
                
                if (outboxMessage.EnqueueAt.HasValue)
                {
                    //schedule for specified date
                    jobId = backgroundJobClient.Create(job, new ScheduledState(outboxMessage.EnqueueAt.Value.DateTime));
                }
                else if (outboxMessage.Delay.HasValue)
                {
                    //schedule for specified delay
                    jobId = backgroundJobClient.Create(job, new ScheduledState(outboxMessage.Delay.Value));
                }
                else
                {
                    //enqueue now
                    jobId = backgroundJobClient.Create(job, new EnqueuedState(outboxMessage.Queue));
                }

                outboxMessage.Processed = true;
                outboxMessage.HangfireJobId = jobId;
                os.SetModified(outboxMessage, processedMemberInfo);
                os.SetModified(outboxMessage, jobIdMemberInfo);

                _logger.LogDebug("Successfully processed outbox job {id}", outboxMessage.Oid);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to process outbox job {id}", outboxMessage.Oid);
                outboxMessage.Exception = e.ToString();
                os.SetModified(outboxMessage, exceptionMemberInfo);
            }
        }

        _logger.LogDebug("Persisting outbox job changes");
        os.CommitChanges();
    }
}

public interface IOutboxProcessor
{
    Task Process(CancellationToken ct);
}