using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WindowsReceiver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = "192.168.112.78",//RabbitMQConnectionParameters.HostName,
                    Port = 5672,//RabbitMQConnectionParameters.Port,
                    VirtualHost = "/",
                    UserName = "sez_ais",//RabbitMQConnectionParameters.User,
                    Password = "ais_sez_7"//RabbitMQConnectionParameters.Password
                };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        tbLog.Text += $"CreateModel" + Environment.NewLine;
                        channel.QueueDeclare(tbQueue.Text, true, false, false, null);
                        var consumer = new EventingBasicConsumer(channel);

                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            string message = Encoding.UTF8.GetString(body.ToArray());

                            tbLog.Text += $"получено сообщение: {message}";
                        };

                        channel.BasicConsume(tbQueue.Text, true, consumer);
                    }
                }
            }
            catch (Exception err) { tbLog.Text += $"ошибка: {err.Message}"; }

        }

       

        public class PropertyValue
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
