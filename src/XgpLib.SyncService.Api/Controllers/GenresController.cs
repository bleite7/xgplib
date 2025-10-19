using Microsoft.AspNetCore.Mvc;
using XgpLib.SyncService.Application.Abstractions.Messaging;
using XgpLib.SyncService.Application.Genres.Queries.GetGenreById;

namespace XgpLib.SyncService.Api.Controllers;

/// <summary>
/// Controller for managing genre-related operations.
/// </summary>
/// <param name="logger">Logger instance</param>
/// <param name="getGenreByIdCommandHandler">Handler for retrieving genre by ID</param>
[ApiController]
[Route("api/[controller]")]
public class GenresController(
    ILogger<GenresController> logger,
    IQueryHandler<GetGenreByIdQuery, GenreResponse> getGenreByIdCommandHandler) : ControllerBase
{
    /// <summary>
    /// Retrieves a genre by its unique identifier.
    /// </summary>
    /// <param name="genreId">The unique identifier of the genre (must be positive)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The genre details if found</returns>
    /// <response code="200">Returns the genre details</response>
    /// <response code="400">If the genre ID is invalid</response>
    /// <response code="404">If the genre is not found</response>
    /// <response code="500">An error occurred while processing request</response>
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
