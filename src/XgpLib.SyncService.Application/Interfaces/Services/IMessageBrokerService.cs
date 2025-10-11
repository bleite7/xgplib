namespace XgpLib.SyncService.Infrastructure.Interfaces.Services;

public interface IMessageBrokerService
{
    Task PublishMessageAsync(string topic, string message, CancellationToken cancellationToken);
    Task ReceiveMessagesAsync(string topic, Func<string, Task> messageHandler, CancellationToken cancellationToken);
}
