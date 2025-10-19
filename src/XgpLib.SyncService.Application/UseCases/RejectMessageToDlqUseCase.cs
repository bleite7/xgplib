namespace XgpLib.SyncService.Application.UseCases;

/// <summary>
/// Use case for rejecting messages and sending them to the Dead Letter Queue
/// </summary>
/// <param name="messageBrokerService">The message broker service</param>
/// <param name="logger">Logger instance</param>
public class RejectMessageToDlqUseCase(
    IMessageBrokerService messageBrokerService,
    ILogger<RejectMessageToDlqUseCase> logger)
{
    private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
    private readonly ILogger<RejectMessageToDlqUseCase> _logger = logger;

    /// <summary>
    /// Executes the use case to reject a message to the Dead Letter Queue
    /// </summary>
    /// <param name="request">The reject message request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response containing the result of the rejection</returns>
    public async Task<RejectMessageToDlqResponse> ExecuteAsync(
        RejectMessageToDlqRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Queue))
            {
                _logger.LogWarning("Attempted to reject message with null or empty queue name");
                return new RejectMessageToDlqResponse(
                    false,
                    string.Empty,
                    "Queue name cannot be null or empty");
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning("Attempted to reject null or empty message from queue {Queue}", request.Queue);
                return new RejectMessageToDlqResponse(
                    false,
                    $"{request.Queue}.dlq",
                    "Message cannot be null or empty");
            }

            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                _logger.LogWarning("Attempted to reject message without reason from queue {Queue}", request.Queue);
                return new RejectMessageToDlqResponse(
                    false,
                    $"{request.Queue}.dlq",
                    "Rejection reason cannot be null or empty");
            }

            var dlqName = $"{request.Queue}.dlq";

            _logger.LogInformation(
                "Rejecting message to DLQ {DlqName}. Reason: {Reason}",
                dlqName,
                request.Reason);

            await _messageBrokerService.RejectMessageToDlqAsync(
                request.Queue,
                request.Message,
                request.Reason,
                cancellationToken);

            _logger.LogInformation(
                "Message successfully rejected to DLQ {DlqName}",
                dlqName);

            return new RejectMessageToDlqResponse(true, dlqName);
        }
        catch (Exception ex)
        {
            var dlqName = $"{request.Queue}.dlq";
            _logger.LogError(
                ex,
                "Failed to reject message to DLQ {DlqName}. Reason: {Reason}",
                dlqName,
                request.Reason);

            return new RejectMessageToDlqResponse(
                false,
                dlqName,
                ex.Message);
        }
    }
}

