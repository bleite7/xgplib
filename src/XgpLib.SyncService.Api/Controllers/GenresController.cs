using Microsoft.AspNetCore.Mvc;
using XgpLib.SyncService.Application.Abstractions.Messaging;
using XgpLib.SyncService.Application.Genres.Queries.GetGenreById;

namespace XgpLib.SyncService.Api.Controllers;

/// <summary>
/// 
/// </summary>
/// <param name="logger"></param>
/// <param name="getGenreByIdCommandHandler"></param>
[ApiController]
[Route("api/[controller]")]
public class GenresController(
    ILogger<GenresController> logger,
    IQueryHandler<GetGenreByIdQuery, GenreResponse> getGenreByIdCommandHandler) : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="genreId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{genreId}")]
    public async Task<IActionResult> GetGenreById([FromRoute] long genreId, CancellationToken cancellationToken)
    {
        if (genreId <= 0)
        {
            return Problem(
                title: "Invalid Genre ID",
                detail: "The provided genre ID must be a positive integer.",
                statusCode: StatusCodes.Status400BadRequest,
                instance: Request.Path
            );
        }

        try
        {
            var query = new GetGenreByIdQuery(genreId);
            var result = await getGenreByIdCommandHandler.HandleAsync(query, cancellationToken);

            return result is null ? NotFound() : Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving genre with ID {GenreId}", genreId);
            return Problem(
                title: "Internal Server Error",
                detail: $"An error occurred while processing your request: {ex.Message}",
                statusCode: StatusCodes.Status500InternalServerError,
                instance: Request.Path
            );
        }
    }
}
