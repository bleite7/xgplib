using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.ComponentModel.Design;
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

    private static short MIN_BOUNDARY = short.MinValue;
    private static short MAX_BOUNDARY = short.MaxValue;

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

        var arguments = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", "" },
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
    /// <param name="topic">The queue to publish the message to</param>
    /// <param name="message">The message to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public async Task PublishMessageAsync(
        string topic,
        string message,
        CancellationToken cancellationToken)
    {
        try
        {
            var connection = await _connectionLazy.Value;
            using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await DeclareQueueWithDlqAsync(channel, topic, cancellationToken);

            var body = Encoding.UTF8.GetBytes(message);
            var properties = new BasicProperties
            {
                Persistent = true,
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: topic,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Message {Message} published to queue {Queue}", message, topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message {Message} to queue {Queue}", message, topic);
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
        short maxMessages,
        CancellationToken cancellationToken)
    {
        var messages = new List<string>();
        try
        {
            var connection = await _connectionLazy.Value;
            using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            if (MIN_BOUNDARY > maxMessages)
                maxMessages = MIN_BOUNDARY;
            else if (maxMessages > MAX_BOUNDARY)
                maxMessages = MAX_BOUNDARY;

            _logger.LogInformation("Attempting to receive up to {MaxMessages} messages from queue {Queue}", maxMessages, topic);

            // Receive messages
            for (short i = 0; i < maxMessages; i++)
            {
                var result = await channel.BasicGetAsync(topic, autoAck: false, cancellationToken);

                if (result == null)
                {
                    _logger.LogInformation("No more messages available in queue {Queue}", topic);
                    break;
                }

                var messageBody = Encoding.UTF8.GetString(result.Body.ToArray());
                messages.Add(messageBody);

                // Acknowledge the message
                await channel.BasicAckAsync(
                    deliveryTag: result.DeliveryTag,
                    multiple: false,
                    cancellationToken: cancellationToken);

                _logger.LogDebug("Received and acknowledged message from queue {Queue}: {Message}", topic, messageBody);
            }

            _logger.LogInformation("Received {Count} messages from queue {Queue}", messages.Count, topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to receive messages from queue {Queue}", topic);
            throw;
        }

        return messages;
    }

    /// <summary>
    /// Rejects a message and sends it to the dead letter queue
    /// </summary>
    /// <param name="queue">The queue name</param>
    /// <param name="message">The message to reject</param>
    /// <param name="reason">Reason for rejection</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public async Task RejectMessageToDlqAsync(
        string queue,
        string message,
        string reason,
        CancellationToken cancellationToken)
    {
        try
        {
            var dlqName = $"{queue}.dlq";
            var connection = await _connectionLazy.Value;
            using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            // Ensure DLQ exists
            await channel.QueueDeclareAsync(
                queue: dlqName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            var body = Encoding.UTF8.GetBytes(message);
            var properties = new BasicProperties
            {
                Persistent = true,
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                Headers = new Dictionary<string, object?>
                {
                    { "x-original-queue", queue },
                    { "x-rejection-reason", reason },
                    { "x-rejected-at", DateTimeOffset.UtcNow.ToString("O") }
                }
            };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: dlqName,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogWarning(
                "Message rejected to DLQ {DlqName}. Reason: {Reason}. Message: {Message}",
                dlqName,
                reason,
                message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to reject message to DLQ for queue {Queue}. Message: {Message}",
                queue,
                message);
            throw;
        }
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

    /// <summary>
    /// Dispose the RabbitMqService and its resources.
    /// </summary>
    public void Dispose()
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
