using Ardalis.Result;

namespace XgpLib.SyncService.Application.Genres.Queries.GetGenreById;

/// <summary>
///
/// </summary>
/// <param name="GenreId"></param>
public sealed record GetGenreByIdQuery(long GenreId) : IQuery<Result<GenreResponse>>;
