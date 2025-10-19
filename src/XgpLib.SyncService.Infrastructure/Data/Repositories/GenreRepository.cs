namespace XgpLib.SyncService.Infrastructure.Data.Repositories;

/// <summary>
/// 
/// </summary>
/// <param name="context"></param>
public class GenreRepository(XgpLibDbContext context) : IGenreRepository
{
    private readonly XgpLibDbContext _context = context;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="genres"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AddOrUpdateRangeAsync(
        IEnumerable<Genre> genres,
        CancellationToken cancellationToken = default)
    {
        foreach (var genre in genres)
        {
            var existingGenre = await _context.Genres.FindAsync([genre.Id], cancellationToken);
            if (existingGenre == null)
                _context.Genres.Add(genre);
            else
                _context.Entry(existingGenre).CurrentValues.SetValues(genre);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="genreId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Genre?> GetGenreById(
        long genreId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Genres.FindAsync([genreId], cancellationToken);
    }
}
