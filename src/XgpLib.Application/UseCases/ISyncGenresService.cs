namespace XgpLib.Application.UseCases;

public interface ISyncGenresService
{
    public Task SyncAsync(CancellationToken cancellationToken = default);
}
