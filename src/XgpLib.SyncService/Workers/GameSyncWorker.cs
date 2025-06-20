namespace XgpLib.SyncService.Workers;

public class GameSyncWorker(ILogger<GameSyncWorker> logger) : BackgroundService
{
    private readonly ILogger<GameSyncWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("{WorkerName} running at: {Time}", nameof(GameSyncWorker), DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
