using Ardalis.Result;
using System.Text.Json;
using XgpLib.SyncService.Application.Abstractions.Messaging;

namespace XgpLib.SyncService.Application.Games.Commands.SyncGames;

/// <summary>
/// 
/// </summary>
/// <param name="logger"></param>
/// <param name="igdbService"></param>
/// <param name="gameRepository"></param>
public class SyncGamesCommandHandler(
    ILogger<SyncGamesCommandHandler> logger,
    IIgdbService igdbService,
    IGameRepository gameRepository) : ICommandHandler<SyncGamesCommand>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result> HandleAsync(SyncGamesCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching games from IGDB API for platform(s): {PlatformIds}", command.PlatformsIds);

        var gamesFromApi = await igdbService.FetchGamesByPlatformsAsync(command.PlatformsIds, cancellationToken);
        if (gamesFromApi is null || !gamesFromApi.Any())
        {
            logger.LogWarning("No games found in the API response");
            return Result.Success();
        }

        logger.LogInformation("Fetched {Count} games from IGDB API", gamesFromApi.Count());

        var games = gamesFromApi.Select(gameDto => new Game
        {
            Id = gameDto.Id,
            Name = gameDto.Name,
            Genres = gameDto.Genres,
            Data = JsonSerializer.Serialize(gameDto),
        });

        try
        {
            await gameRepository.AddOrUpdateRangeAsync(games, cancellationToken);
            logger.LogInformation("Successfully synchronized {Count} games to the database", gamesFromApi.Count());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to synchronize games to the database: {Message}", ex.Message);
            throw;
        }

        return Result.Success();
    }
}
