namespace XgpLib.SyncService.Services;

public interface ISyncGenresService
{
    public Task SyncAsync(CancellationToken cancellationToken = default);
}
