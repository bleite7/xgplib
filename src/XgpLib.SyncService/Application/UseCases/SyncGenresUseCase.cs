using System.Text.Json;

namespace XgpLib.SyncService.Application.UseCases;

public class SyncGenresUseCase(
    IIgdbService igdbService,
    IGenreRepository genreRepository,
    ILogger<SyncGenresUseCase> logger)
{
    private readonly IIgdbService _igdbService = igdbService;
    private readonly IGenreRepository _genreRepository = genreRepository;
    private readonly ILogger<SyncGenresUseCase> _logger = logger;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
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
            Data = JsonSerializer.Serialize(genreDto),
        });

        await _genreRepository.AddOrUpdateRangeAsync(genres, cancellationToken);
    }
}
