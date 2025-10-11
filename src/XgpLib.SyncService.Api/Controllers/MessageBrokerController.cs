using Microsoft.AspNetCore.Mvc;
using XgpLib.SyncService.Application.DTOs;
using XgpLib.SyncService.Application.UseCases;

namespace XgpLib.SyncService.Api.Controllers;

/// <summary>
/// Controller for handling message broker operations
/// </summary>
/// <param name="logger">Logger instance</param>
/// <param name="publishMessageUseCase">Use case for publishing messages</param>
[ApiController]
[Route("api/[controller]")]
public class MessageBrokerController(
    ILogger<MessageBrokerController> logger,
    PublishMessageUseCase publishMessageUseCase) : ControllerBase
{
    private readonly ILogger<MessageBrokerController> _logger = logger;
    private readonly PublishMessageUseCase _publishMessageUseCase = publishMessageUseCase;

    /// <summary>
    /// Publishes a message to RabbitMQ
    /// </summary>
    /// <param name="request">The message publishing request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Publishing result</returns>
    [HttpPost("publish")]
    public async Task<ActionResult<PublishMessageResponse>> PublishMessage(
        [FromBody] PublishMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _publishMessageUseCase.ExecuteAsync(request, cancellationToken);

            return result.Success
                ? Ok(result)
                : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while publishing message to topic {Topic}", request.Topic);
            return StatusCode(500, new PublishMessageResponse(false, ex.Message));
        }
    }
}
