using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitMQWeb.Watermark.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitMQClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitMQClientService)
        {
            _rabbitMQClientService = rabbitMQClientService;
        }

        public async Task PublishProductImageCreatedEvent(ProductImageCreatedEvent productImageCreatedEvent)
        {
            var channel = await _rabbitMQClientService.Connect();
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(productImageCreatedEvent));

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange: RabbitMQClientService.ExchangeName,
                routingKey: RabbitMQClientService.RoutingWatermark,
                mandatory: false,
                basicProperties: properties,
                body: body
            );
        }
    }
}
