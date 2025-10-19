using Ardalis.Result;
using System.Text.Json;
using XgpLib.SyncService.Application.Abstractions.Data;
using XgpLib.SyncService.Application.Abstractions.Messaging;

namespace XgpLib.SyncService.Application.Genres.Commands.SyncGenres;

/// <summary>
/// 
/// </summary>
/// <param name="logger"></param>
/// <param name="igdbService"></param>
/// <param name="genreRepository"></param>
/// <param name="unitOfWork"></param>
public sealed class SyncGenresCommandHandler(
    ILogger<SyncGenresCommandHandler> logger,
    IIgdbService igdbService,
    IGenreRepository genreRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<SyncGenresCommand>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result> HandleAsync(SyncGenresCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching genres from IGDB API");

        var genresFromApi = await igdbService.FetchGenresAsync(cancellationToken);
        if (genresFromApi is null || !genresFromApi.Any())
        {
            logger.LogWarning("No genres found in the API response");
            return Result.Success();
        }

        logger.LogInformation("Fetched {Count} genres from IGDB API", genresFromApi.Count());

        var genres = genresFromApi.Select(genreDto => new Genre
        {
            Id = genreDto.Id,
            Name = genreDto.Name,
            Slug = genreDto.Slug,
            Data = JsonSerializer.Serialize(genreDto),
        });

        try
        {
            await genreRepository.AddOrUpdateRangeAsync(genres, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Successfully synchronized {Count} genres to the database", genresFromApi.Count());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to synchronize genres to the database: {Message}", ex.Message);
            throw;
        }

        return Result.Success();
    }
}
