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
            _logger.LogInformation("Worker rodando em: {time}", DateTimeOffset.Now);

            // Usando o service provider para criar um escopo
            using (var scope = _serviceProvider.CreateScope())
            {
                var syncGenresUseCase = scope.ServiceProvider.GetRequiredService<SyncGenresUseCase>();
                try
                {
                    await syncGenresUseCase.ExecuteAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ocorreu um erro cataclísmico ao sincronizar os gêneros.");
                }
            }
            await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
        }

    }
}
