namespace XgpLib.SyncService.Workers;

public class GenreSyncWorker(ILogger<GenreSyncWorker> logger) : BackgroundService
{
    private readonly ILogger<GenreSyncWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("{WorkerName} running at: {Time}", nameof(GenreSyncWorker), DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
