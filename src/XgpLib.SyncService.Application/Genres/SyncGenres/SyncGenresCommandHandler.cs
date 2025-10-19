using Ardalis.Result;
using System.Text.Json;
using XgpLib.SyncService.Application.Abstractions.Messaging;

namespace XgpLib.SyncService.Application.Genres.SyncGenres;

/// <summary>
/// 
/// </summary>
/// <param name="logger"></param>
/// <param name="igdbService"></param>
/// <param name="genreRepository"></param>
public class SyncGenresCommandHandler(
    ILogger<SyncGenresCommandHandler> logger,
    IIgdbService igdbService,
    IGenreRepository genreRepository) : ICommandHandler<SyncGenresCommand>
{
    private readonly ILogger<SyncGenresCommandHandler> _logger = logger;
    private readonly IIgdbService _igdbService = igdbService;
    private readonly IGenreRepository _genreRepository = genreRepository;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result> HandleAsync(SyncGenresCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching genres from IGDB API");

        var genresFromApi = await _igdbService.FetchGenresAsync(cancellationToken);
        if (genresFromApi is null || !genresFromApi.Any())
        {
            _logger.LogWarning("No genres found in the API response");
            return Result.Success();
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

        return Result.Success();
    }
}
