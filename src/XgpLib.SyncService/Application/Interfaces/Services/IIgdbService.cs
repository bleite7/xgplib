namespace XgpLib.SyncService.Application.Interfaces.Services;

public interface IIgdbService
{
    Task<IEnumerable<IgdbGenre>> FetchGenresAsync(CancellationToken cancellationToken);
}
