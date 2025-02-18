using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program
{
    static async Task Main()
    {

        ////No Exchange
        //var factory = new ConnectionFactory
        //{
        //    HostName = "localhost"
        //};

        //await using var connection = await factory.CreateConnectionAsync();
        //await using var channel = await connection.CreateChannelAsync();

        ////Senaryoya göre kuyruk burada oluşturulabilir

        ////await channel.QueueDeclareAsync(
        ////    queue: "task_queue", // Kuyruk adı
        ////    durable: true, // Kuyruğun kalıcı olup olmayacağı
        ////    exclusive: false, // Sadece bu bağlantıdan erişilebilir olup olmayacağı
        ////    autoDelete: false, // Son tüketici bağlandığında kuyruğun silinip silinmeyeceği
        ////    arguments: null // Kuyruk argümanları
        ////); 

        //await channel.BasicQosAsync(
        //    0,
        //    6, //Gödnerilecek sayı
        //    false // her birine 6 tane gönderir. true 6 yıl böler
        //    );
        //var consumer = new AsyncEventingBasicConsumer(channel);

        //await channel.BasicConsumeAsync(
        //    "task_queue", // Kuyruk adı
        //    false, // Mesajın işlendiğini doğrulamak için false..BasicAckAsync ile bilgi gelene kadar mesajı silmez
        //    consumer // Tüketici
        //);

        //consumer.ReceivedAsync += (object sender, BasicDeliverEventArgs eventArgs) =>
        //{
        //    var body = eventArgs.Body.ToArray();
        //    var message = Encoding.UTF8.GetString(body);

        //    Thread.Sleep(1500);
        //    Console.WriteLine(" [x] Received {0}", message);
        //    channel.BasicAckAsync(eventArgs.DeliveryTag, false);

        //    return Task.CompletedTask;
        //};

        //Console.ReadLine();

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        //Fanout
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        var queueDeclareOk = await channel.QueueDeclareAsync();
        var randomQueueName = queueDeclareOk.QueueName;

        await channel.QueueBindAsync(randomQueueName, "logs-fanout", "", null); // uygulama çalışınca otomatik kuyruk oluşur, kapanınca silinir.
        //Eğer kuyruk kalsın isersek declare edebiliriz.

        await channel.BasicQosAsync(
            0,
            1, //Gödnerilecek sayı
            false // her birine 6 tane gönderir. true 6 yıl böler
            );
        var consumer = new AsyncEventingBasicConsumer(channel);

        await channel.QueueDeclareAsync(randomQueueName, true, false, false);//hazır mesaj alırken gelen mesajlar kaybolmasın diye kuyruk olusturduk. server down olursa kuyruktaki mesajları alabiliriz.

        await channel.BasicConsumeAsync(
            randomQueueName, // Kuyruk adı
            false, // Mesajın işlendiğini doğrulamak için false..BasicAckAsync ile bilgi gelene kadar mesajı silmez
            consumer // Tüketici
        );

        Console.WriteLine("Loglar dinleniyor...");

        consumer.ReceivedAsync += (object sender, BasicDeliverEventArgs eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Thread.Sleep(1500);
            Console.WriteLine(" [x] Received {0}", message);
            channel.BasicAckAsync(eventArgs.DeliveryTag, false);

            return Task.CompletedTask;
        };

        Console.ReadLine();
    }
}
