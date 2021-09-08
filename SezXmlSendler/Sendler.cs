using RabbitMQ.Client;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SezXmlSendler
{
    public class Sendler
    {
        public Type SerializeObjectType { get; protected set; }

        public delegate DataTable LoadTableEventHandler(object sender);
        
        public delegate string GetXmlStringEventHandler(object sender, DataRow row);
        public delegate void SendEventHandler(object sender, string send, bool needSend);
        public delegate void SerializeObjectEventHandler(object sender, string serialize);
        public delegate void LogEventHandler(object sender, string log);
        public event LoadTableEventHandler OnLoadTable;
        public event SerializeObjectEventHandler OnSerializeObject;
        public event ErrorEventHandler OnError;
        public event SendEventHandler OnRun;
        public event LogEventHandler OnSended;
        public DateTime TimeRunning { get; set; }

        public string Name { get; set; }

        public async Task RunningAsync(bool needSend)
        {
            if (OnRun != null)
                 await Task.Run(() => { OnRun(this, RoutingKey, needSend); });
            else
            {
                 await Task.Run(() =>
                {
                    DataTable sourceTbl = null;
                    try
                    {
                        sourceTbl = OnLoadTable?.Invoke(this);
                    }
                    catch (Exception err) { OnError?.Invoke(this, new ErrorEventArgs(err)); }
                    if (sourceTbl != null)
                        foreach (DataRow item in sourceTbl.Rows)
                        {
                            var mess = new MessageObject(item);
                            try
                            {
                                var str = SerializeObject(typeof(MessageObject), mess);
                                OnSerializeObject?.Invoke(this, str);
                                if (needSend)
                                {
                                    Send(str, RoutingKey);
                                    OnSended?.Invoke(this, "пакет отправлен");
                                }
                            }
                            catch (Exception err)
                            {
                                OnError?.Invoke(this, new ErrorEventArgs(new Exception($" Ошибка: {err.Message}")));
                            }
                        }
                });
            }
        }

        public static string HostName { get; set; }
        public static int Port { get; set; }
        public static string User { get; set; }
        public static string Password { get; set; }
        public static string ExchangeName { get; set; }
        public string RoutingKey { get; set; }

        public DateTime RunTime { get; set; }
        public Sendler(string name, string routingKey)
        {
            Name = name;
            RoutingKey = routingKey;
        }
        public Sendler(Type serializatonObjectType, string name, string routingKey) : this(name, routingKey)
        {
            SerializeObjectType = serializatonObjectType;
        }
        public override string ToString()
        {
            return Name;
        }


        public static string SerializeObject(Type typeObject, object obj)
        {
            string utf8;
            var serializer = new XmlSerializer(typeObject);
            using (StringWriter writer = new Utf8StringWriter())
            {
                serializer.Serialize(writer, obj);
                utf8 = writer.ToString();
            }
            return utf8;
        }


        public static void Send(string message, string routingKey)
        {
            var factory = new ConnectionFactory()
            {
                HostName = Sendler.HostName,
                Port = Sendler.Port,
                VirtualHost = "/",
                UserName = Sendler.User,
                Password = Sendler.Password
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(
                        exchange: Sendler.ExchangeName,
                        routingKey: routingKey,
                        basicProperties: null,
                        body: body
                      );
                }
            }
        }
    }

}
