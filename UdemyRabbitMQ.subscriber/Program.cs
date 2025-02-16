using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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

        //Senaryoya göre kuyruk burada oluşturulabilir

        //await channel.QueueDeclareAsync(
        //    queue: "task_queue", // Kuyruk adı
        //    durable: true, // Kuyruğun kalıcı olup olmayacağı
        //    exclusive: false, // Sadece bu bağlantıdan erişilebilir olup olmayacağı
        //    autoDelete: false, // Son tüketici bağlandığında kuyruğun silinip silinmeyeceği
        //    arguments: null // Kuyruk argümanları
        //); 

        var consumer = new AsyncEventingBasicConsumer(channel);

        await channel.BasicConsumeAsync(
            "task_queue", // Kuyruk adı
            true, // Mesajın işlendiğini doğrulamak için false
            consumer // Tüketici
        );

        consumer.ReceivedAsync += (object sender, BasicDeliverEventArgs eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);
            return Task.CompletedTask;
        };

        Console.ReadLine();
    }
}
