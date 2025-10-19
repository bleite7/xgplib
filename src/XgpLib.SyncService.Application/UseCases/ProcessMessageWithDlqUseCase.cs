using System.Text.Json;

namespace XgpLib.SyncService.Application.UseCases;

/// <summary>
/// Use case for processing messages with Dead Letter Queue support
/// </summary>
/// <typeparam name="TMessage">The type of message to deserialize</typeparam>
/// <param name="rejectMessageToDlqUseCase">Use case for rejecting messages to DLQ</param>
/// <param name="logger">Logger instance</param>
public class ProcessMessageWithDlqUseCase<TMessage>(
    RejectMessageToDlqUseCase rejectMessageToDlqUseCase,
    ILogger<ProcessMessageWithDlqUseCase<TMessage>> logger) where TMessage : class
{
    private readonly RejectMessageToDlqUseCase _rejectMessageToDlqUseCase = rejectMessageToDlqUseCase;
    private readonly ILogger<ProcessMessageWithDlqUseCase<TMessage>> _logger = logger;

    /// <summary>
    /// Attempts to deserialize and process a message, sending to DLQ on failure
    /// </summary>
    /// <param name="queueName">The queue name</param>
    /// <param name="rawMessage">The raw message string</param>
    /// <param name="processAction">The action to process the deserialized message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if processed successfully, false otherwise</returns>
    public async Task<bool> ExecuteAsync(
        string queueName,
        string rawMessage,
        Func<TMessage, CancellationToken, Task> processAction,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Attempt to deserialize the message
            JsonSerializerOptions jsonSerializerOptions = new()
            {
                PropertyNameCaseInsensitive = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            var message = JsonSerializer.Deserialize<TMessage>(
                rawMessage,
                options);

            if (message == null)
            {
                _logger.LogError("Failed to deserialize message from queue {Queue}. Message is null after deserialization.", queueName);

                await _rejectMessageToDlqUseCase.ExecuteAsync(
                    new RejectMessageToDlqRequest(queueName, rawMessage, "Deserialization returned null"),
                    cancellationToken);

                return false;
            }

            _logger.LogDebug("Successfully deserialized message from queue {Queue}", queueName);

            await processAction(message, cancellationToken);

            _logger.LogInformation("Successfully processed message from queue {Queue}", queueName);
            return true;
        }
        catch (JsonException jsonEx)
        {
            _logger.LogError(jsonEx, "Invalid JSON format in message from queue {Queue}. Sending to DLQ.", queueName);

            await _rejectMessageToDlqUseCase.ExecuteAsync(
                new RejectMessageToDlqRequest(queueName, rawMessage, $"JSON deserialization error: {jsonEx.Message}"),
                cancellationToken);

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message from queue {Queue}. Sending to DLQ.", queueName);

            await _rejectMessageToDlqUseCase.ExecuteAsync(
                new RejectMessageToDlqRequest(queueName, rawMessage, $"Processing error: {ex.Message}"),
                cancellationToken);

            return false;
        }
    }
}
