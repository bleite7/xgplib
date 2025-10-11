namespace XgpLib.SyncService.Infrastructure.Data.Repositories;

public class GameRepository(XgpLibDbContext context) : IGameRepository
{
    private readonly XgpLibDbContext _context = context;

    public async Task AddOrUpdateRangeAsync(
        IEnumerable<Game> games,
        CancellationToken cancellationToken = default)
    {
        foreach (var game in games)
        {
            var existingGame = await _context.Games.FindAsync([game.Id], cancellationToken);
            if (existingGame == null)
            {
                _context.Games.Add(game);
            }
            else
            {
                _context.Entry(existingGame).CurrentValues.SetValues(game);
            }
        }
        await _context.SaveChangesAsync(cancellationToken);
    }
}
