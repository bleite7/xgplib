using System.Text.Json;

namespace XgpLib.SyncService.Application.UseCases;

public class SyncGamesUseCase(
    IIgdbService igdbService,
    IGameRepository gameRepository,
    ILogger<SyncGamesUseCase> logger)
{
    private readonly IIgdbService _igdbService = igdbService;
    private readonly IGameRepository _gameRepository = gameRepository;
    private readonly ILogger<SyncGamesUseCase> _logger = logger;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching games from IGDB API for platform(s): {PlatformIds}", "[3]");

        var gamesFromApi = await _igdbService.FetchGamesByPlatformAsync([3], cancellationToken);
        if (gamesFromApi is null || !gamesFromApi.Any())
        {
            _logger.LogWarning("No games found in the API response. Skipping synchronization.");
            return;
        }

        _logger.LogInformation("Fetched {Count} games from IGDB API.", gamesFromApi.Count());

        var games = gamesFromApi.Select(gameDto => new Game
        {
            Id = gameDto.Id,
            Name = gameDto.Name,
            Data = JsonSerializer.Serialize(gameDto),
        });

        try
        {
            await _gameRepository.AddOrUpdateRangeAsync(games, cancellationToken);
            _logger.LogInformation("Successfully synchronized {Count} games to the database.", gamesFromApi.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to synchronize games to the database: {Message}", ex.Message);
            throw;
        }
    }
}
