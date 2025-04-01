namespace Hangfire.Community.Outbox.Xaf.HostedServices;

using Extensions;
using Microsoft.Extensions.Hosting;
using Services;

public class OutboxJobsProcessorBackgroundService: BackgroundService
{
    private readonly IOutboxProcessor _outboxProcessor;
    private readonly HangfireOutboxOptions _outboxOptions;

    public OutboxJobsProcessorBackgroundService(IOutboxProcessor outboxProcessor, HangfireOutboxOptions outboxOptions)
    {
        _outboxProcessor = outboxProcessor;
        _outboxOptions = outboxOptions;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessOutboxMessages(stoppingToken);    
            await Task.Delay(_outboxOptions.OutboxProcessorFrequency, stoppingToken);
        }
    }

    private async Task ProcessOutboxMessages(CancellationToken stoppingToken)
    {
        await _outboxProcessor.Process(stoppingToken);
    }
}