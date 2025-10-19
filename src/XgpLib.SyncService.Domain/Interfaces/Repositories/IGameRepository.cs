namespace XgpLib.SyncService.Domain.Interfaces.Repositories;

/// <summary>
/// 
/// </summary>
public interface IGameRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="games"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddOrUpdateRangeAsync(IEnumerable<Game> games, CancellationToken cancellationToken);
}
