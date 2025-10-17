using System.Text.Json;

namespace XgpLib.SyncService.Application.UseCases;

/// <summary>
/// Use case to synchronize games from IGDB API to the local database.
/// </summary>
/// <param name="logger">Logger for the use case</param>
/// <param name="igdbService">Service to interact with IGDB API</param>
/// <param name="gameRepository">Repository to manage game data</param>
public class SyncGamesUseCase(
    ILogger<SyncGamesUseCase> logger,
    IIgdbService igdbService,
    IGameRepository gameRepository)
{
    private readonly ILogger<SyncGamesUseCase> _logger = logger;
    private readonly IIgdbService _igdbService = igdbService;
    private readonly IGameRepository _gameRepository = gameRepository;

    /// <summary>
    /// Sync games from IGDB API to the local database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching games from IGDB API for platform(s): {PlatformIds}", "[3]");

        var gamesFromApi = await _igdbService.FetchGamesByPlatformAsync([3], cancellationToken);
        if (gamesFromApi is null || !gamesFromApi.Any())
        {
            _logger.LogWarning("No games found in the API response");
            return;
        }

        _logger.LogInformation("Fetched {Count} games from IGDB API", gamesFromApi.Count());

        var games = gamesFromApi.Select(gameDto => new Game
        {
            Id = gameDto.Id,
            Name = gameDto.Name,
            Genres = gameDto.Genres,
            Data = JsonSerializer.Serialize(gameDto),
        });

        try
        {
            await _gameRepository.AddOrUpdateRangeAsync(games, cancellationToken);
            _logger.LogInformation("Successfully synchronized {Count} games to the database", gamesFromApi.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to synchronize games to the database: {Message}", ex.Message);
            throw;
        }
    }
}
