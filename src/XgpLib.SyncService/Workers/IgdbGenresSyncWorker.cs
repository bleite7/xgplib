namespace XgpLib.SyncService.Workers;

public class IgdbGenresSyncWorker(
    IServiceProvider serviceProvider,
    ILogger<IgdbGenresSyncWorker> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<IgdbGenresSyncWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var startTime = DateTimeOffset.UtcNow;
            _logger.LogInformation("Starting genres synchronization at {Time}", startTime);

            // Using the service provider to create a scope
            using (var scope = _serviceProvider.CreateScope())
            {
                var syncGenresUseCase = scope.ServiceProvider.GetRequiredService<SyncGenresUseCase>();
                try
                {
                    await syncGenresUseCase.ExecuteAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while synchronizing genres: {Message}", ex.Message);
                }
            }
            _logger.LogInformation("Genres synchronization completed at {Time} in {Elapsed}", DateTimeOffset.UtcNow, (DateTimeOffset.UtcNow - startTime));
            await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
        }
    }
}
