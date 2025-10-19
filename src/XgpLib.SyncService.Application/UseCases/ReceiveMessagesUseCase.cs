using XgpLib.SyncService.Application.Abstractions.Services;

namespace XgpLib.SyncService.Application.UseCases;

/// <summary>
/// Use case for receiving messages from a message broker topic
/// </summary>
/// <param name="messageBrokerService">The message broker service</param>
/// <param name="logger">Logger instance</param>
public class ReceiveMessagesUseCase(
    IMessageBrokerService messageBrokerService,
    ILogger<ReceiveMessagesUseCase> logger)
{
    private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
    private readonly ILogger<ReceiveMessagesUseCase> _logger = logger;

    /// <summary>
    /// Executes the use case to receive messages from a topic
    /// </summary>
    /// <param name="request">The receive messages request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response containing received messages</returns>
    public async Task<ReceiveMessagesResponse> ExecuteAsync(
        ReceiveMessagesRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Receiving up to {MaxMessages} messages from topic {Topic}",
                request.MaxMessages,
                request.Topic);

            var messages = await _messageBrokerService.ReceiveMessagesAsync(
                request.Topic,
                request.MaxMessages,
                cancellationToken);

            _logger.LogInformation(
                "Successfully received {Count} messages from topic {Topic}",
                messages.Count,
                request.Topic);

            return new ReceiveMessagesResponse(true, messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error receiving messages from topic {Topic}",
                request.Topic);

            return new ReceiveMessagesResponse(false, [], ex.Message);
        }
    }
}
