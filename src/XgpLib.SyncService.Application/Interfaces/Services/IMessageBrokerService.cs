namespace XgpLib.SyncService.Infrastructure.Interfaces.Services;

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
    /// <param name="messageHandler">The function to handle received messages</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task ReceiveMessagesAsync(string topic, Func<string, Task> messageHandler, CancellationToken cancellationToken);
}
