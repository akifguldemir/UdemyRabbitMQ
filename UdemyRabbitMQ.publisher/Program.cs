using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

public enum LogNames
{
    Critical=1,
    Error=2,
    Warning=3,
    Info4
}

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

        //var factory = new ConnectionFactory
        //{
        //    HostName = "localhost"
        //};

        //await using var connection = await factory.CreateConnectionAsync();
        //await using var channel = await connection.CreateChannelAsync();

        //await channel.ExchangeDeclareAsync("logs-fanout",
        //    durable: false, // fiziksel olarak kaydolur
        //    type: ExchangeType.Fanout
        //    );

        //Enumerable.Range(1, 50).ToList().ForEach(async x =>
        //{
        //    string message = $"log {x}";
        //    var body = Encoding.UTF8.GetBytes(message);

        //    await channel.BasicPublishAsync(
        //        exchange: "logs-fanout",
        //        routingKey: "", // filtreleme olmadığı için route key yok
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


        //Direct Exchange.
        //mesajı direk ilgili kuyruğa gönderir. rastgele log seviyeleri sahip loglar var diyelim.
        //Kuyruk oluşturacaz ve logu gerekn kuyruğa gönderecez

        //var factory = new ConnectionFactory
        //{
        //    HostName = "localhost"
        //};

        //await using var connection = await factory.CreateConnectionAsync();
        //await using var channel = await connection.CreateChannelAsync();

        //// **Exchange Tanımlama**
        //await channel.ExchangeDeclareAsync("logs-direct",
        //    durable: false,
        //    type: ExchangeType.Direct
        //);

        //// **Kuyrukları Tanımlama ve Exchange'e Bağlama**
        //foreach (var logName in Enum.GetNames(typeof(LogNames)))
        //{
        //    var queueName = $"direct-queue-{logName}";
        //    var routeKey = $"route-{logName}";

        //    await channel.QueueDeclareAsync(queueName,
        //        durable: false,
        //        exclusive: false,
        //        autoDelete: false,
        //        arguments: null
        //    );

        //    await channel.QueueBindAsync(queueName, "logs-direct", routeKey);
        //}

        //// **50 Adet Rastgele Mesaj Gönderme**
        //for (int i = 1; i <= 50; i++)
        //{
        //    // Rastgele bir log seviyesi seç
        //    LogNames logName = (LogNames)new Random().Next(1, 5); // 1-4 arası değerler

        //    // Route key'yi log seviyesine göre ayarla
        //    var routeKey = $"route-{logName}";

        //    string message = $"log-type:{logName}";
        //    var body = Encoding.UTF8.GetBytes(message);

        //    await channel.BasicPublishAsync(
        //        exchange: "logs-direct",
        //        routingKey: routeKey,
        //        mandatory: false,
        //        basicProperties: new BasicProperties
        //        {
        //            DeliveryMode = (DeliveryModes)2 // Kalıcı mesaj
        //        },
        //        body: body
        //    );

        //    Console.WriteLine($"Mesaj gönderildi: {message}, Route Key: {routeKey}");
        //}


        //Topic Exchange.. *.log, *.error, *.info şeklinde filtreleme yapar

        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        // **Exchange Tanımlama**
        await channel.ExchangeDeclareAsync("logs-topic",
            durable: false,
            type: ExchangeType.Topic
        );

        Random rnd = new Random();

        // **50 Adet Rastgele Mesaj Gönderme**
        for (int i = 1; i <= 50; i++)
        {

            // Rastgele bir log seviyesi seç
            LogNames logName = (LogNames)new Random().Next(1, 5); // 1-4 arası değerler

            LogNames log1 = (LogNames)rnd.Next(1, 5);
            LogNames log2 = (LogNames)rnd.Next(1, 5);
            LogNames log3 = (LogNames)rnd.Next(1, 5);

            // Route key'yi log seviyesine göre ayarla
            var routeKey = $"{log1}.{log2}.{log3}";
            string message = $"log-type:{log1}{log2}{log3}";

            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: "logs-topic",
                routingKey: routeKey,
                mandatory: false,
                basicProperties: new BasicProperties
                {
                    DeliveryMode = (DeliveryModes)2 // Kalıcı mesaj
                },
                body: body
            );

            Console.WriteLine($"Mesaj gönderildi: {message}, Route Key: {routeKey}");
        }


    }
}
