namespace XgpLib.SyncService.Application.Genres.Queries.GetGenreById;

/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="Slug"></param>
public sealed record GenreResponse(long Id, string Name, string Slug);
