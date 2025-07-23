namespace XgpLib.SyncService.Workers;

public class IgdbSyncWorker(
    IServiceProvider serviceProvider,
    ILogger<IgdbSyncWorker> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<IgdbSyncWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var startTime = DateTimeOffset.UtcNow;
            _logger.LogInformation("Starting synchronization at {Time}", startTime);

            using (var scope = _serviceProvider.CreateScope())
            {
                var syncGamesUseCase = scope.ServiceProvider.GetRequiredService<SyncGamesUseCase>();
                var syncGenresUseCase = scope.ServiceProvider.GetRequiredService<SyncGenresUseCase>();
                try
                {
                    var gamesTask = syncGamesUseCase.ExecuteAsync(cancellationToken);
                    var genresTask = syncGenresUseCase.ExecuteAsync(cancellationToken);
                    await Task.WhenAll(
                        gamesTask,
                        genresTask);

                    _logger.LogInformation("Synchronization finished successfully at {Time}", DateTimeOffset.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while synchronizing data: {Message}", ex.Message);
                }
            }
            var elapsed = DateTimeOffset.UtcNow - startTime;
            _logger.LogInformation("Synchronization completed at {Time} (Elapsed: {Elapsed})", DateTimeOffset.UtcNow, elapsed);
            await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
        }
    }
}
