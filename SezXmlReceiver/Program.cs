using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;

namespace SezXmlReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "192.168.112.78",//RabbitMQConnectionParameters.HostName,
                Port = 5672,//RabbitMQConnectionParameters.Port,
                VirtualHost = "/",
                UserName = "sez_ais",//RabbitMQConnectionParameters.User,
                Password = "ais_sez_7"//RabbitMQConnectionParameters.Password
            };
            try
            {
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare("aissez.1c8b.ts.agents", false, false, false, null);
                        var consumer = new EventingBasicConsumer(channel);

                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            string message = Encoding.UTF8.GetString(body.ToArray());
                            Console.WriteLine($"получено сообщение: {message}");
                        };
                    }
                }
            }
            catch (Exception e){ Console.WriteLine(e.Message); }

            Console.WriteLine("нажми любую клавишу");
        }
/*


                    var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var chanel = connection.CreateModel();
            chanel.QueueDeclare("aissez.1c8b.ts.agents", false, false, false, null);
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
        }*/
    }

}
