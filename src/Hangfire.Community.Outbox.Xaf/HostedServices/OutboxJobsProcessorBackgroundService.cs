namespace Hangfire.Community.Outbox.Xaf.HostedServices;

using Extensions;
using Microsoft.Extensions.Hosting;
using Services;

public class OutboxJobsProcessorBackgroundService: BackgroundService
{
    private readonly IOutboxProcessor _outboxProcessor;
    private readonly HangfireOutboxOptions _outboxOptions;
    private readonly IHostApplicationLifetime _lifetime;

    public OutboxJobsProcessorBackgroundService(IOutboxProcessor outboxProcessor, HangfireOutboxOptions outboxOptions, IHostApplicationLifetime lifetime)
    {
        _outboxProcessor = outboxProcessor;
        _outboxOptions = outboxOptions;
        _lifetime = lifetime;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await WaitForAppStartup(_lifetime, stoppingToken))
        {
            return;
        }

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

    /// <summary>
    /// See https://andrewlock.net/finding-the-urls-of-an-aspnetcore-app-from-a-hosted-service-in-dotnet-6/
    /// Without this, the job runs, although XAF/XPO is not ready yet
    /// </summary>
    /// <param name="lifetime"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    private static async Task<bool> WaitForAppStartup(IHostApplicationLifetime lifetime, CancellationToken stoppingToken)
    {
        var startedSource = new TaskCompletionSource();
        var cancelledSource = new TaskCompletionSource();

        await using var reg1 = lifetime.ApplicationStarted.Register(() => startedSource.SetResult());
        await using var reg2 = stoppingToken.Register(() => cancelledSource.SetResult());

        var completedTask = await Task.WhenAny(
            startedSource.Task,
            cancelledSource.Task).ConfigureAwait(false);

        // If the completed tasks was the "app started" task, return true, otherwise false
        return completedTask == startedSource.Task;
    }
}