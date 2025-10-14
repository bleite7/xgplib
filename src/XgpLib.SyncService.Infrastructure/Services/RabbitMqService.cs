using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using XgpLib.SyncService.Infrastructure.Configuration;

namespace XgpLib.SyncService.Infrastructure.Services;

public class RabbitMqService : IMessageBrokerService, IDisposable
{
    private readonly ILogger<RabbitMqService> _logger;
    private readonly RabbitMqConfiguration _configuration;
    private readonly Lazy<Task<IConnection>> _connectionLazy;
    private bool _disposed;

    public RabbitMqService(
        ILogger<RabbitMqService> logger,
        IOptions<RabbitMqConfiguration> configuration)
    {
        _logger = logger;
        _configuration = configuration.Value;
        _connectionLazy = new Lazy<Task<IConnection>>(CreateConnectionAsync);
    }

    public async Task PublishMessageAsync(string topic, string message, CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = await _connectionLazy.Value;
            using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: topic,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

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

            _logger.LogInformation("Message {Message} published to topic {Topic}", message, topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message {Message} to topic {Topic}", message, topic);
            throw;
        }
    }

    public async Task ReceiveMessagesAsync(string topic, Func<string, Task> messageHandler, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("ReceiveMessagesAsync is not implemented yet.");
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
            _logger.LogInformation("RabbitMQ connection established to {HostName}:{Port}",
                _configuration.HostName, _configuration.Port);

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
        }
    }

    #endregion IDisposable
}
