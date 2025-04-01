namespace Hangfire.Community.Outbox.Xaf.BackgroundJobs;

using Services;

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