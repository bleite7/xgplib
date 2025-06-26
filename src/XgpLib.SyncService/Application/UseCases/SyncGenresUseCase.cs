namespace XgpLib.SyncService.Application.UseCases;

public class SyncGenresUseCase(
    IIgdbService igdbService,
    ILogger<SyncGenresUseCase> logger)
{
    private readonly IIgdbService _igdbService = igdbService;
    private readonly ILogger<SyncGenresUseCase> _logger = logger;

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var genresFromApi = await _igdbService.FetchGenresAsync(cancellationToken);
        if (genresFromApi is null || !genresFromApi.Any())
        {
            _logger.LogWarning("No genres found in the API response. Skipping synchronization.");
            return;
        }

        var genres = genresFromApi.Select(genreDto => new Genre
        {
            Id = genreDto.Id,
            Name = genreDto.Name,
            Slug = genreDto.Slug,
        });
        foreach (var genre in genres)
        {
            _logger.LogInformation("Genre synchronized: {GenreId} - {GenreName} ({GenreSlug})", genre.Id, genre.Name, genre.Slug);
        }
    }
}
