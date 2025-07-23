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
        _logger.LogInformation("Fetching genres from IGDB API");

        var genresFromApi = await _igdbService.FetchGenresAsync(cancellationToken);
        if (genresFromApi is null || !genresFromApi.Any())
        {
            _logger.LogWarning("No genres found in the API response");
            return;
        }

        _logger.LogInformation("Fetched {Count} genres from IGDB API", genresFromApi.Count());

        var genres = genresFromApi.Select(genreDto => new Genre
        {
            Id = genreDto.Id,
            Name = genreDto.Name,
            Slug = genreDto.Slug,
            Data = JsonSerializer.Serialize(genreDto),
        });

        try
        {
            await _genreRepository.AddOrUpdateRangeAsync(genres, cancellationToken);
            _logger.LogInformation("Successfully synchronized {Count} genres to the database", genresFromApi.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to synchronize genres to the database: {Message}", ex.Message);
            throw;
        }
    }
}
