namespace XgpLib.SyncService.Domain.Interfaces.Repositories;

/// <summary>
/// 
/// </summary>
public interface IGenreRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="genres"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddOrUpdateRangeAsync(IEnumerable<Genre> genres, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="genreId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Genre> GetGenreById(long genreId, CancellationToken cancellationToken);
}
