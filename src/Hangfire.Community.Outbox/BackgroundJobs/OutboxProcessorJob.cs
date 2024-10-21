using Hangfire.Community.Outbox.Services;

namespace Hangfire.Community.Outbox.BackgroundJobs;

public class OutboxProcessorJob
{
    private readonly IOutboxProcessor _outboxProcessor;

    public OutboxProcessorJob(IOutboxProcessor outboxProcessor)
    {
        _outboxProcessor = outboxProcessor;
    }
    
    [DisableConcurrentExecution(120)]
    public async Task Process(CancellationToken ct)
    {
        await _outboxProcessor.Process(ct);
    }
}