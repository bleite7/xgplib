using XgpLib.SyncService.Infrastructure.Interfaces.Services;

namespace XgpLib.SyncService.Application.UseCases;


public class GetMessageUseCase(
    ILogger<GetMessageUseCase> logger,
    IMessageBrokerService messageBrokerService)
{
    private readonly ILogger<GetMessageUseCase> _logger = logger;
    private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;

    /// <summary>
    /// Execute the use case to publish a message
    /// </summary>
    /// <param name="request">The message publishing request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Publishing result</returns>
    public async Task<PublishMessageResponse> ExecuteAsync(
        PublishMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Topic))
            {
                return new PublishMessageResponse(false, "Topic cannot be null or empty");
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return new PublishMessageResponse(false, "Message cannot be null or empty");
            }

            _logger.LogInformation("Publishing message to topic {Topic}", request.Topic);

            await _messageBrokerService.PublishMessageAsync(
                request.Topic,
                request.Message,
                cancellationToken);

            _logger.LogInformation("Message successfully published to topic {Topic}", request.Topic);

            return new PublishMessageResponse(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to topic {Topic}", request.Topic);
            return new PublishMessageResponse(false, ex.Message);
        }
    }
}
