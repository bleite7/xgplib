namespace XgpLib.SyncService.Infrastructure.Interfaces.Services;

/// <summary>
/// 
/// </summary>
public interface ITokenManagerService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GetValidTokenAsync(CancellationToken cancellationToken);
}
