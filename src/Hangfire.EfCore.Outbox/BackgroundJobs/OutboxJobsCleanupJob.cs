using Hangfire.EfCore.Outbox.Entities;
using Hangfire.EfCore.Outbox.Extensions;
using Hangfire.EfCore.Outbox.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hangfire.EfCore.Outbox.BackgroundJobs;

public class OutboxJobsCleanupJob
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OutboxJobsCleanupJob> _logger;
    private readonly HangfireOutboxOptions _outboxOptions;

    public OutboxJobsCleanupJob(IServiceScopeFactory serviceScopeFactory, ILogger<OutboxJobsCleanupJob> logger, HangfireOutboxOptions outboxOptions)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _outboxOptions = outboxOptions;
    }

    public async Task CleanOldJobs(CancellationToken ct)
    {
        _logger.LogDebug("Cleaning old outbox jobs");
        
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContextAccessor = scope.ServiceProvider.GetRequiredService<IDbContextAccessor>();
        await using var dbContext = dbContextAccessor.GetDbContext();

        var threshold = DateTime.UtcNow.Subtract(_outboxOptions.CleanupOlderThan);
        
        var query = dbContext.Set<OutboxJob>().Where(x => x.Processed && x.CreatedOn <= threshold);

        if (!_outboxOptions.CleanupJobsWithExceptions)
        {
            query = query.Where(x => x.Exception == null);
        }

#if NET8_0
        var deletedCount = await query.ExecuteDeleteAsync(ct);
#else
        var toDelete = await query.ToArrayAsync(ct);
        var deletedCount = toDelete.Length;
        
        dbContext.Set<OutboxJob>().RemoveRange(toDelete);

        await dbContext.SaveChangesAsync(ct);
#endif
        _logger.LogDebug("Deleted {deletedCount} outbox jobs older than {threshold}", deletedCount, threshold);
    }
}