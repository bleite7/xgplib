namespace XgpLib.SyncService.Domain.Interfaces.Repositories;

public interface IGenreRepository
{
    Task AddOrUpdateRangeAsync(IEnumerable<Genre> genres, CancellationToken cancellationToken);
    Task<IEnumerable<Genre>> GetAllAsync(CancellationToken cancellationToken);
}
