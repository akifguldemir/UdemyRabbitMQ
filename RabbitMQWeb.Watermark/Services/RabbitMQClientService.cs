using RabbitMQ.Client;

namespace RabbitMQWeb.Watermark.Services
{
    public class RabbitMQClientService
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public static string ExchangeName = "ImageDirectExchange";
        public static string RoutingWatermark = "watermark-route-image";
        public static string QueueName = "watermark-image";

        private readonly ILogger<RabbitMQClientService> _logger;

        public RabbitMQClientService(ConnectionFactory connectionFactory, IConnection connection, IChannel channel, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _connection = connection;
            _channel = channel;
            _logger = logger;
        }
    }
}
