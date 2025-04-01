namespace Hangfire.Community.Outbox.Xaf.Extensions;

using BackgroundJobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public static class IHostExtensions
{
    public static void UseHangfireOutbox(this IApplicationBuilder appBuilder)
    {
        var recurringJobManager = appBuilder.ApplicationServices.GetRequiredService<IRecurringJobManager>();
        var options = appBuilder.ApplicationServices.GetRequiredService<HangfireOutboxOptions>();
        
        ConfigureCleanupJob(options, recurringJobManager);

        ConfigureOutboxProcessorJob(options, recurringJobManager);
    }

    private static void ConfigureCleanupJob(HangfireOutboxOptions options, IRecurringJobManager recurringJobManager)
    {
        if (options.CleanupJobEnabled)
        {
            recurringJobManager.AddOrUpdate<OutboxJobsCleanupJob>(nameof(OutboxJobsCleanupJob), x => x.CleanOldJobs(CancellationToken.None), options.CleanupJobCron);
        }
        else
        {
            recurringJobManager.RemoveIfExists(nameof(OutboxJobsCleanupJob));
        }
    }
    
    private static void ConfigureOutboxProcessorJob(HangfireOutboxOptions options, IRecurringJobManager recurringJobManager)
    {
        if (options.OutboxProcessor == HangfireOutboxOptions.OutboxProcessorType.HangfireRecurringJob)
        {
            recurringJobManager.AddOrUpdate<OutboxProcessorJob>(nameof(OutboxProcessorJob), x => x.Process(CancellationToken.None), "*/10 * * * * *");
        }
        else
        {
            recurringJobManager.RemoveIfExists(nameof(OutboxProcessorJob));
        }
    }
}