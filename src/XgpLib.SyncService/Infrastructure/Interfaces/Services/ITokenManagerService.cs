namespace XgpLib.SyncService.Infrastructure.Interfaces.Services;

public interface ITokenManagerService
{
    Task<string> GetValidTokenAsync(CancellationToken cancellationToken);
}
