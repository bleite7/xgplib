namespace XgpLib.SyncService.Application.UseCases;

public class SyncGamesUseCase(
    IIgdbService igdbService,
    ILogger<SyncGamesUseCase> logger)
{
    private readonly IIgdbService _igdbService = igdbService;
    private readonly ILogger<SyncGamesUseCase> _logger = logger;

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var gamesFromApi = await _igdbService.FetchGamesByPlatformAsync([7, 11], cancellationToken);
        if (gamesFromApi is null || !gamesFromApi.Any())
        {
            _logger.LogWarning("No games found in the API response. Skipping synchronization.");
            return;
        }

        var games = gamesFromApi.Select(gameDto => new Game
        {
            Id = gameDto.Id,
            Name = gameDto.Name,
        });
        foreach (var game in games)
        {
            _logger.LogInformation("Game synchronized: {GameId} - {GameName}", game.Id, game.Name);
        }
    }
}
