namespace Hangfire.Community.Outbox.Xaf.BackgroundJobs;

using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Hangfire.Community.Outbox.Xaf.Entities;
using Hangfire.Community.Outbox.Xaf.Extensions;
using Microsoft.Extensions.Logging;

public class OutboxJobsCleanupJob
{
    protected INonSecuredObjectSpaceFactory ObjectSpaceFactory { get; }

    private readonly ILogger<OutboxJobsCleanupJob> _logger;

    private readonly HangfireOutboxOptions _outboxOptions;

    public OutboxJobsCleanupJob(INonSecuredObjectSpaceFactory objectSpaceFactory, ILogger<OutboxJobsCleanupJob> logger, HangfireOutboxOptions outboxOptions)
    {
        ObjectSpaceFactory = objectSpaceFactory;
        _logger = logger;
        _outboxOptions = outboxOptions;
    }

    public async Task CleanOldJobs(CancellationToken ct)
    {
        _logger.LogDebug("Cleaning old outbox jobs");

        var threshold = DateTimeOffset.UtcNow.Subtract(_outboxOptions.CleanupOlderThan);

        var os = ObjectSpaceFactory.CreateNonSecuredObjectSpace<OutboxJob>();
        var query = os.GetObjectsQuery<OutboxJob>().Where(x => x.Processed && x.CreatedOn <= threshold);

        if (!_outboxOptions.CleanupJobsWithExceptions)
        {
            query = query.Where(x => x.Exception == null);
        }

        var toDelete = await query.ToArrayAsync(ct);
        if (toDelete.Length > 0)
        {
            os.Delete(toDelete);
            os.CommitChanges();

            var deletedCount = toDelete.Length;
            _logger.LogDebug("Deleted {deletedCount} outbox jobs older than {threshold}", deletedCount, threshold);
        }
    }
}