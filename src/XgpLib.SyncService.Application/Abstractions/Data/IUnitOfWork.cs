namespace XgpLib.SyncService.Application.Abstractions.Data;

/// <summary>
/// 
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
