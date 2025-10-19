using Microsoft.AspNetCore.Mvc;

namespace XgpLib.SyncService.Api.Controllers;

/// <summary>
/// Controller for handling message broker operations (publish and receive messages).
/// </summary>
/// <param name="logger"></param>
/// <param name="publishMessageUseCase"></param>
/// <param name="receiveMessagesUseCase"></param>
[ApiController]
[Route("api/[controller]")]
public class MessageBrokerController(
    ILogger<MessageBrokerController> logger,
    PublishMessageUseCase publishMessageUseCase,
    ReceiveMessagesUseCase receiveMessagesUseCase) : ControllerBase
{
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
            var result = await publishMessageUseCase.ExecuteAsync(request, cancellationToken);

            return result.Success
                ? Ok(result)
                : BadRequest(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while publishing message to topic {Topic}", request.Topic);
            return StatusCode(500, new PublishMessageResponse(false, ex.Message));
        }
    }

    /// <summary>
    /// Receives messages from RabbitMQ
    /// </summary>
    /// <param name="topic">The topic to receive messages from</param>
    /// <param name="maxMessages">Maximum number of messages to receive (default: 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Received messages</returns>
    [HttpGet("receive/{topic}")]
    public async Task<ActionResult<ReceiveMessagesResponse>> ReceiveMessages(
        [FromRoute] string topic,
        [FromQuery] int maxMessages = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new ReceiveMessagesRequest(topic, maxMessages);
            var result = await receiveMessagesUseCase.ExecuteAsync(request, cancellationToken);

            return result.Success
                ? Ok(result)
                : BadRequest(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while receiving messages from topic {Topic}", topic);
            return StatusCode(500, new ReceiveMessagesResponse(false, [], ex.Message));
        }
    }
}
