using MediatR;
using System.Text.Json;
using XgpLib.SyncService.Application.Commands;

namespace XgpLib.SyncService.Application.Handlers;

public class SyncGamesCommandHandler(
    ILogger<SyncGamesCommandHandler> logger,
    IIgdbService igdbService,
    IGameRepository gameRepository) : IRequestHandler<SyncGamesCommand, Unit>
{
    private readonly ILogger<SyncGamesCommandHandler> _logger = logger;
    private readonly IIgdbService _igdbService = igdbService;
    private readonly IGameRepository _gameRepository = gameRepository;

    public async Task<Unit> Handle(SyncGamesCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching games from IGDB API for platform(s): {PlatformIds}", "[3]");

        var gamesFromApi = await _igdbService.FetchGamesByPlatformAsync([3], cancellationToken);
        if (gamesFromApi is null || !gamesFromApi.Any())
        {
            _logger.LogWarning("No games found in the API response");
            return Unit.Value;
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

        return Unit.Value;
    }
}
