using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

class Program
{
    static async Task Main()
    {

        //// No Exchange, No Queue, No Binding
        //var factory = new ConnectionFactory
        //{
        //    HostName = "localhost"
        //};

        //await using var connection = await factory.CreateConnectionAsync();
        //await using var channel = await connection.CreateChannelAsync();

        //await channel.QueueDeclareAsync(
        //    queue: "task_queue", // Kuyruk adı
        //    durable: true, // Kuyruğun kalıcı olup olmayacağı
        //    exclusive: false, // Sadece bu bağlantıdan erişilebilir olup olmayacağı
        //    autoDelete: false, // Son tüketici bağlandığında kuyruğun silinip silinmeyeceği
        //    arguments: null // Kuyruk argümanları
        //);

        //Enumerable.Range(1, 50).ToList().ForEach(async x =>
        //{
        //    string message = $"Message {x}";
        //    var body = Encoding.UTF8.GetBytes(message);

        //    await channel.BasicPublishAsync(
        //        exchange: "",
        //        routingKey: "task_queue",
        //        mandatory: false,
        //        basicProperties: new BasicProperties
        //        {
        //            DeliveryMode = (DeliveryModes)2 // 2 = Kalıcı mesaj
        //        },
        //        body: body
        //    );

        //    Console.WriteLine($"Mesaj gönderildi {message}");
        //});

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        //Fanout Exchange.
        //Filtreleme yapmaksızın tüm kuyruklara gönderir..hava durumu senaryosu

        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync("logs-fanout",
            durable: false, // fiziksel olarak kaydolur
            type: ExchangeType.Fanout
            );

        Enumerable.Range(1, 50).ToList().ForEach(async x =>
        {
            string message = $"log {x}";
            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: "logs-fanout",
                routingKey: "", // filtreleme olmadığı için route key yok
                mandatory: false,
                basicProperties: new BasicProperties
                {
                    DeliveryMode = (DeliveryModes)2 // 2 = Kalıcı mesaj
                },
                body: body
            );

            Console.WriteLine($"Mesaj gönderildi {message}");
        });




    }
}
