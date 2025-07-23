namespace XgpLib.SyncService.Infrastructure.Data.Repositories;

public class GenreRepository(XgpLibDbContext context) : IGenreRepository
{
    private readonly XgpLibDbContext _context = context;

    public async Task AddOrUpdateRangeAsync(
        IEnumerable<Genre> genres,
        CancellationToken cancellationToken)
    {
        foreach (var genre in genres)
        {
            var existingGenre = await _context.Genres.FindAsync([genre.Id], cancellationToken);
            if (existingGenre == null)
            {
                _context.Genres.Add(genre);
            }
            else
            {
                _context.Entry(existingGenre).CurrentValues.SetValues(genre);
            }
        }
        await _context.SaveChangesAsync(cancellationToken);
    }
}
