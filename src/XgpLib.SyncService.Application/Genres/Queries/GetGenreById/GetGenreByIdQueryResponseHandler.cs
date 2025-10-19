using Ardalis.Result;
using XgpLib.SyncService.Application.Abstractions.Messaging;

namespace XgpLib.SyncService.Application.Genres.Queries.GetGenreById;

/// <summary>
/// 
/// </summary>
/// <param name="genreRepository"></param>
public sealed class GetGenreByIdQueryResponseHandler(
    IGenreRepository genreRepository) : IQueryHandler<GetGenreByIdQuery, GenreResponse>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<GenreResponse> HandleAsync(GetGenreByIdQuery query, CancellationToken cancellationToken)
    {
        var genre = await genreRepository.GetGenreById(query.GenreId, cancellationToken);
        if (genre is null)
        {
            return Result<GenreResponse>.NotFound();
        }
        var response = new GenreResponse(genre.Id, genre.Name, genre.Slug);
        return Result<GenreResponse>.Success(response);
    }
}
