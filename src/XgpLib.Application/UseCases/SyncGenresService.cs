namespace XgpLib.Application.UseCases;

public class SyncGenresService : ISyncGenresService
{
    private readonly IGenreRepository _genreRepository;
    private readonly IGenreService _genreService;
    public SyncGenresService(IGenreRepository genreRepository, IGenreService genreService)
    {
        _genreRepository = genreRepository;
        _genreService = genreService;
    }
    public async Task SyncAsync(CancellationToken cancellationToken = default)
    {
        var genres = await _genreService.GetAllGenresAsync(cancellationToken);
        var existingGenres = await _genreRepository.GetAllAsync(cancellationToken);
        foreach (var genre in genres)
        {
            if (!existingGenres.Any(g => g.Slug == genre.Slug))
            {
                await _genreRepository.AddAsync(genre, cancellationToken);
            }
        }
    }
}
