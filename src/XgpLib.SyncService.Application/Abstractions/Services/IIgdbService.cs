namespace XgpLib.SyncService.Application.Abstractions.Services;

/// <summary>
/// Interface for IGDB service to fetch data from IGDB API
/// </summary>
public interface IIgdbService
{
    /// <summary>
    /// Fetch genres from IGDB API
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of genres</returns>
    Task<IEnumerable<IgdbGenre>> FetchGenresAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Fetch games by platforms IDs from IGDB API
    /// </summary>
    /// <param name="platformsIds">List of platform IDs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of games</returns>
    Task<IEnumerable<IgdbGame>> FetchGamesByPlatformsAsync(int[] platformsIds, CancellationToken cancellationToken);
}
