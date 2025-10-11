namespace XgpLib.SyncService.WorkerServices.Workers;

public class IgdbGenresSyncWorker(
    IServiceProvider serviceProvider,
    ILogger<IgdbGenresSyncWorker> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<IgdbGenresSyncWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var startTime = DateTimeOffset.UtcNow;
            _logger.LogInformation("Starting genres synchronization at {Time}", startTime);

            using (var scope = _serviceProvider.CreateScope())
            {
                var syncGenresUseCase = scope.ServiceProvider.GetRequiredService<SyncGenresUseCase>();
                try
                {
                    await syncGenresUseCase.ExecuteAsync(stoppingToken);
                    _logger.LogInformation("Genres synchronization finished successfully at {Time}", DateTimeOffset.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while synchronizing genres: {Message}", ex.Message);
                }
            }
            var elapsed = DateTimeOffset.UtcNow - startTime;
            _logger.LogInformation("Genres synchronization completed at {Time} (Elapsed: {Elapsed})", DateTimeOffset.UtcNow, elapsed);
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
