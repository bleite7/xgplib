using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using XgpLib.SyncService.Application.Abstractions.Services;
using XgpLib.SyncService.Infrastructure.Configuration;

namespace XgpLib.SyncService.Infrastructure.Services;

/// <summary>
/// RabbitMQ implementation of IMessageBrokerService.
/// </summary>
public class RabbitMqService : IMessageBrokerService, IDisposable
{
    private readonly ILogger<RabbitMqService> _logger;
    private readonly RabbitMqConfiguration _configuration;
    private readonly Lazy<Task<IConnection>> _connectionLazy;
    private bool _disposed;

    /// <summary>
    /// Constructor for RabbitMqService.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    public RabbitMqService(
        ILogger<RabbitMqService> logger,
        IOptions<RabbitMqConfiguration> configuration)
    {
        _logger = logger;
        _configuration = configuration.Value;
        _connectionLazy = new Lazy<Task<IConnection>>(CreateConnectionAsync);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="queueName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task DeclareQueueWithDlqAsync(
        IChannel channel,
        string queueName,
        CancellationToken cancellationToken)
    {
        var dlqName = $"{queueName}.dlq";

        await channel.QueueDeclareAsync(
            queue: dlqName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var arguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" }, // exchange padrão
            { "x-dead-letter-routing-key", dlqName }
        };

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Publish a message to a specified topic
    /// </summary>
    /// <param name="queue">The queue to publish the message to</param>
    /// <param name="message">The message to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public async Task PublishMessageAsync(
        string queue,
        string message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = await _connectionLazy.Value;
            using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await DeclareQueueWithDlqAsync(channel, queue, cancellationToken);

            var body = Encoding.UTF8.GetBytes(message);
            var properties = new BasicProperties
            {
                Persistent = true,
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: queue,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Message {Message} published to queue {Queue}", message, queue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message {Message} to queue {Queue}", message, queue);
            throw;
        }
    }

    /// <summary>
    /// Receive messages from a specified topic
    /// </summary>
    /// <param name="topic">The topic to receive messages from</param>
    /// <param name="maxMessages">Maximum number of messages to receive</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of received messages</returns>
    public async Task<List<string>> ReceiveMessagesAsync(
        string topic,
        int maxMessages,
        CancellationToken cancellationToken = default)
    {
        var messages = new List<string>();
        try
        {
            var connection = await _connectionLazy.Value;
            using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            // Declare the queue (ensure it exists)
            await channel.QueueDeclareAsync(
                queue: topic,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            // Set prefetch count to limit the number of messages
            await channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: (ushort)maxMessages,
                global: false,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Attempting to receive up to {MaxMessages} messages from topic {Topic}", maxMessages, topic);

            // Receive messages
            for (int i = 0; i < maxMessages; i++)
            {
                var result = await channel.BasicGetAsync(topic, autoAck: false, cancellationToken);

                if (result == null)
                {
                    _logger.LogInformation("No more messages available in topic {Topic}", topic);
                    break;
                }

                var messageBody = Encoding.UTF8.GetString(result.Body.ToArray());
                messages.Add(messageBody);

                // Acknowledge the message
                await channel.BasicAckAsync(
                    deliveryTag: result.DeliveryTag,
                    multiple: false,
                    cancellationToken: cancellationToken);

                _logger.LogDebug("Received and acknowledged message from topic {Topic}: {Message}", topic, messageBody);
            }

            _logger.LogInformation("Successfully received {Count} messages from topic {Topic}", messages.Count, topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to receive messages from topic {Topic}", topic);
            throw;
        }

        return messages;
    }

    #region Private Methods

    private async Task<IConnection> CreateConnectionAsync()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration.HostName,
                Port = _configuration.Port,
                UserName = _configuration.UserName,
                Password = _configuration.Password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            var connection = await factory.CreateConnectionAsync();
            _logger.LogInformation("RabbitMQ connection established to {HostName}:{Port}", _configuration.HostName, _configuration.Port);

            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create RabbitMQ connection");
            throw;
        }
    }

    #endregion Private Methods

    #region IDisposable

    void IDisposable.Dispose()
    {
        if (_disposed) return;
        try
        {
            if (_connectionLazy.IsValueCreated
                && _connectionLazy.Value.IsCompletedSuccessfully)
            {
                _connectionLazy.Value.Result.Dispose();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error disposing RabbitMQ connection");
        }
        finally
        {
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    #endregion IDisposable
}
