
using Microsoft.EntityFrameworkCore.InMemory.ValueGeneration.Internal;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQWeb.Watermark.Services;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace RabbitMQWeb.Watermark.BackgroundServices
{
    public class ImageWatermarkProcessBackgroundService : BackgroundService
    {
        private readonly RabbitMQClientService _rabbitMQClientService;
        private readonly ILogger<ImageWatermarkProcessBackgroundService> _logger;
        private IChannel _channel;

        public ImageWatermarkProcessBackgroundService(RabbitMQClientService rabbitMQClientService, ILogger<ImageWatermarkProcessBackgroundService> logger)
        {
            _rabbitMQClientService = rabbitMQClientService;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQClientService.Connect().Result;

            _channel.BasicQosAsync(0, 1, false);

            return base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            await _channel.BasicConsumeAsync(RabbitMQClientService.QueueName, false, consumer);

            consumer.ReceivedAsync += Consumer_ReceivedAsync;   

        }

        private async Task<Task> Consumer_ReceivedAsync(object sender, BasicDeliverEventArgs @event)
        {

            try
            {
                var imageCreatedEvent = JsonSerializer.Deserialize<ProductImageCreatedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", imageCreatedEvent.ImageName);

                using var igm = Image.FromFile(path);

                using var graphic = Graphics.FromImage(igm);

                var font = new Font(FontFamily.GenericSansSerif, 20, FontStyle.Bold, GraphicsUnit.Pixel);

                var textSize = graphic.MeasureString("RabbitMQ", font);

                var color = Color.FromArgb(100, 255, 0, 0);

                var brush = new SolidBrush(color);

                var position = new PointF(igm.Width - ((int)textSize.Width + 30), igm.Height - textSize.Height);

                graphic.DrawString("RabbitMQ", font, brush, position);

                igm.Save("wwwroot/Images/watermarks/" + imageCreatedEvent.ImageName);

                igm.Dispose();
                graphic.Dispose();

                _channel.BasicAckAsync(@event.DeliveryTag, false);

            }
            catch (Exception ex)
            {

                throw;
            }

            return Task.CompletedTask;


        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
