namespace XgpLib.SyncService.Application.Interfaces.Services;

public interface IIgdbService
{
    Task<IEnumerable<IgdbGenre>> FetchGenresAsync(CancellationToken cancellationToken);
    Task<IEnumerable<IgdbGame>> FetchGamesByPlatformAsync(IEnumerable<int> platformIds, CancellationToken cancellationToken);
}
