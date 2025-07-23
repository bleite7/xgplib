namespace XgpLib.SyncService.Domain.Interfaces.Repositories;

public interface IGameRepository
{
    Task AddOrUpdateRangeAsync(IEnumerable<Game> games, CancellationToken cancellationToken);
}
