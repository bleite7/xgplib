namespace XgpLib.SyncService.Infrastructure.Data.Repositories;

/// <summary>
/// 
/// </summary>
/// <param name="context"></param>
public class GameRepository(XgpLibDbContext context) : IGameRepository
{
    private readonly XgpLibDbContext _context = context;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="games"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AddOrUpdateRangeAsync(
        IEnumerable<Game> games,
        CancellationToken cancellationToken)
    {
        foreach (var game in games)
        {
            var existingGame = await _context.Games.FindAsync([game.Id], cancellationToken);
            if (existingGame == null)
                _context.Games.Add(game);
            else
                _context.Entry(existingGame).CurrentValues.SetValues(game);
        }
    }
}
