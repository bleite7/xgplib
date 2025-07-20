namespace XgpLib.SyncService.Workers;

public class IgdbGamesSyncWorker(
    IServiceProvider serviceProvider,
    ILogger<IgdbGamesSyncWorker> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<IgdbGamesSyncWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var startTime = DateTimeOffset.Now;
            _logger.LogInformation("Starting games synchronization at {Time}", startTime);

            using (var scope = _serviceProvider.CreateScope())
            {
                var syncGamesUseCase = scope.ServiceProvider.GetRequiredService<SyncGamesUseCase>();
                try
                {
                    await syncGamesUseCase.ExecuteAsync(cancellationToken);
                    _logger.LogInformation("Games synchronization finished successfully at {Time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while synchronizing games: {Message}", ex.Message);
                }
            }
            var elapsed = DateTimeOffset.Now - startTime;
            _logger.LogInformation("Games synchronization completed at {Time} (Elapsed: {Elapsed})", DateTimeOffset.Now, elapsed);
            await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
        }

    }
}
