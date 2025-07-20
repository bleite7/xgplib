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
        var gamesFromApi = await _igdbService.FetchGamesByPlatformAsync([3], cancellationToken);
        if (gamesFromApi is null || !gamesFromApi.Any())
        {
            _logger.LogWarning("No games found in the API response. Skipping synchronization.");
            return;
        }

        var games = gamesFromApi.Select(gameDto => new Game
        {
            Id = gameDto.Id,
            Name = gameDto.Name,
            Data = JsonSerializer.Serialize(gameDto),
        });

        await _gameRepository.AddOrUpdateRangeAsync(games, cancellationToken);
    }
}
