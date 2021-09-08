using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace SezXmlReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var chanel = connection.CreateModel();
            chanel.QueueDeclare("hello", false, false, false, null);
            Console.WriteLine("Первый, первый, я второй, прием");
            var consumer = new EventingBasicConsumer(chanel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                string message = Encoding.UTF8.GetString(body.ToArray());
                Console.WriteLine($"получено сообщение: {message}");
            };
            chanel.BasicConsume("hello", true, consumer);
            Console.ReadLine();
        }
    }
}
