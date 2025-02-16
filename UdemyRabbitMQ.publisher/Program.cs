using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

class Program
{
    static async Task Main()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "task_queue", // Kuyruk adı
            durable: true, // Kuyruğun kalıcı olup olmayacağı
            exclusive: false, // Sadece bu bağlantıdan erişilebilir olup olmayacağı
            autoDelete: false, // Son tüketici bağlandığında kuyruğun silinip silinmeyeceği
            arguments: null // Kuyruk argümanları
        );

        string message = "Hello World!";
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: "task_queue",
            mandatory: false,
            basicProperties: new BasicProperties
            {
                DeliveryMode = (DeliveryModes)2 // 2 = Kalıcı mesaj
            },
            body: body
        );

        Console.WriteLine(" [x] Sent {0}", message);
    }
}
