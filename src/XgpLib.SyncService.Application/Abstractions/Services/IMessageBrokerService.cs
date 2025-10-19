namespace XgpLib.SyncService.Application.Abstractions.Services;

/// <summary>
/// Interface for message broker service
/// </summary>
public interface IMessageBrokerService
{
    /// <summary>
    /// Publish a message to a specified topic
    /// </summary>
    /// <param name="topic">The topic to publish the message to</param>
    /// <param name="message">The message to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task PublishMessageAsync(string topic, string message, CancellationToken cancellationToken);

    /// <summary>
    /// Receive messages from a specified topic
    /// </summary>
    /// <param name="topic">The topic to receive messages from</param>
    /// <param name="maxMessages">Maximum number of messages to receive</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of received messages</returns>
    Task<List<string>> ReceiveMessagesAsync(string topic, int maxMessages, CancellationToken cancellationToken);

    /// <summary>
    /// Rejects a message and sends it to the dead letter queue
    /// </summary>
    /// <param name="queue">The queue name</param>
    /// <param name="message">The message to reject</param>
    /// <param name="reason">Reason for rejection</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task RejectMessageToDlqAsync(string queue, string message, string reason, CancellationToken cancellationToken);
}
