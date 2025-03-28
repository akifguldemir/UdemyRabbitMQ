using RabbitMQ.Client;
using Microsoft.Extensions.Logging;

namespace RabbitMQWeb.Watermark.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection? _connection;
        private IChannel? _channel;

        public static string ExchangeName = "ImageDirectExchange";
        public static string RoutingWatermark = "watermark-route-image";
        public static string QueueName = "watermark-image";

        private readonly ILogger<RabbitMQClientService> _logger;

        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<IChannel> Connect()
        {
            if (_connection is not { IsOpen: true })
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
            }

            if (_channel is { IsOpen: true })
            {
                return _channel;
            }

            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(
                ExchangeName,
                type: "direct",
                durable: true,
                autoDelete: false
            );

            await _channel.QueueDeclareAsync(
                QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            await _channel.QueueBindAsync(
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: RoutingWatermark
            );

            _logger.LogInformation("RabbitMQ bağlantısı kuruldu ve kanal açıldı.");

            return _channel;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _channel = default;
            _connection?.Dispose();
            _connection = default;

            _logger.LogInformation($"Rbmq ile bağlantı koptu..");
        }
    }
}
 