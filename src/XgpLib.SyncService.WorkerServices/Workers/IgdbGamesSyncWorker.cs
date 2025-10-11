namespace XgpLib.SyncService.WorkerServices.Workers;

public class IgdbGamesSyncWorker(
    IServiceProvider serviceProvider,
    ILogger<IgdbGamesSyncWorker> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<IgdbGamesSyncWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var startTime = DateTimeOffset.UtcNow;
            _logger.LogInformation("Starting games synchronization at {Time}", startTime);

            using (var scope = _serviceProvider.CreateScope())
            {
                var syncGamesUseCase = scope.ServiceProvider.GetRequiredService<SyncGamesUseCase>();
                try
                {
                    await syncGamesUseCase.ExecuteAsync(stoppingToken);
                    _logger.LogInformation("Games synchronization finished successfully at {Time}", DateTimeOffset.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while synchronizing games: {Message}", ex.Message);
                }
            }
            var elapsed = DateTimeOffset.UtcNow - startTime;
            _logger.LogInformation("Games synchronization completed at {Time} (Elapsed: {Elapsed})", DateTimeOffset.UtcNow, elapsed);
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
