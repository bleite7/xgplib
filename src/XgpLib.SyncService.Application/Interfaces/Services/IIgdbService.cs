namespace XgpLib.SyncService.Application.Interfaces.Services;

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
    /// Fetch games by platform IDs from IGDB API
    /// </summary>
    /// <param name="platformIds">List of platform IDs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of games</returns>
    Task<IEnumerable<IgdbGame>> FetchGamesByPlatformAsync(IEnumerable<int> platformIds, CancellationToken cancellationToken);
}
