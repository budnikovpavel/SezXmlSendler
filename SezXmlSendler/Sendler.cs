using RabbitMQ.Client;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SezXmlSendler.Model;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler
{
    public partial class Sendler<TBaseMessageObject> : ISendler
        where TBaseMessageObject : BaseMessageObject
    {
        private readonly Func<DataRow, DataTable> _filledTable;

        public delegate DataTable LoadTableEventHandler(object sender);

        public delegate void SerializeObjectEventHandler(object sender, string serialize);

        public delegate void LogEventHandler(object sender, string log);

        public event LoadTableEventHandler OnLoadTable;
        public event LoadTableEventHandler OnFilledTable;
        public event SerializeObjectEventHandler OnSerializeObject;
        public event ErrorEventHandler OnError;
        public event LogEventHandler OnSended;
        public event LogEventHandler OnLog;
        public DateTime TimeRunning { get; set; }

        public async Task RunningAsync(bool needSend)
        {

            DataTable sourceTbl = null;
            try
            {

                if (OnLoadTable != null) sourceTbl = OnLoadTable.Invoke(this);
                if (sourceTbl != null)
                    showLog($"Загрузили таблицу с данными: найдено строк {sourceTbl.Rows.Count}");

            }
            catch (Exception err)
            {
                if (OnError != null) OnError.Invoke(this, new ErrorEventArgs(err));
            }


            if (sourceTbl != null)
            {
                var i = 1;
                foreach (DataRow item in sourceTbl.Rows)
                {

                    var idTask = item.Table.Columns.Contains("ID_TASK") ? $" - Заказ {item["ID_TASK"]}" : "";
                    showLog($"{idTask} обрабатываем строку {i} из {sourceTbl.Rows.Count} ");

                    var mess = Activator.CreateInstance(typeof(TBaseMessageObject));
                    showLog($"Инициализировался объект {mess}");
                    if ((mess as IFillOnRow) != null)
                    {
                        (mess as IFillOnRow).FillOnRow(item);
                        showLog($"Заполнили объект {mess} данными из строки");
                    }
                    else
                    {
                        if ((mess as IFillOnTable) != null)
                        {
                            try
                            {

                                if (OnFilledTable != null)
                                {
                                    var fillTbl = OnFilledTable(item);
                                    ((mess as IFillOnTable)).FillOnTable(fillTbl);
                                }
                            }
                            catch (Exception err)
                            {
                                if (OnError != null)
                                    OnError(this, new ErrorEventArgs(new Exception($" Ошибка загрузки данных из таблицы : {err.Message}")));
                            }
                            showLog($"Заполнили объект {mess} данными из таблицы");
                        }
                    }

                    try
                    {
                        var str = SerializeObject(mess);

                        showLog($"Объект {mess} сериализован");
                        OnSerializeObject?.Invoke(this, str);

                        if (needSend)
                        {
                            Send(str, RoutingKey);
                            OnSended?.Invoke(this, "пакет отправлен");
                        }
                    }
                    catch (Exception err)
                    {
                        if (OnError != null)
                            OnError(this, new ErrorEventArgs(new Exception($" Ошибка: {err.Message}")));
                        if (OnLog != null) OnLog.Invoke(this, $"Ошибка : {err.Message}");

                    }

                    i += 1;
                }
            }

            showLog($"Операция прошла успешно");
        }

        private void showLog(string messageLog)
        {
            OnLog?.Invoke(this, messageLog);
        }

        public string RoutingKey { get; set; }

        public string Name { get; set; }
        public Sendler(string name, string routingKey, Func<DataRow, DataTable> filledTable = default)
        {
            Name = name;
            RoutingKey = routingKey;
            _filledTable = filledTable;
        }

        public override string ToString() => Name;

        public string SerializeObject(object obj)
        {
            string utf8;
            var serializer = new XmlSerializer(obj.GetType());
            using (StringWriter writer = new Utf8StringWriter())
            {
                serializer.Serialize(writer, obj);
                utf8 = writer.ToString();
            }

            return utf8;
        }


        public void Send(string message, string routingKey)
        {
            var factory = new ConnectionFactory()
            {
                HostName = RabbitMQConnectionParameters.HostName,
                Port = RabbitMQConnectionParameters.Port,
                VirtualHost = "/",
                UserName = RabbitMQConnectionParameters.User,
                Password = RabbitMQConnectionParameters.Password
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(
                        exchange: RabbitMQConnectionParameters.ExchangeName,
                        routingKey: routingKey,
                        basicProperties: null,
                        body: body
                    );
                }
            }
        }
    }

    public class RabbitMQConnectionParameters
    {
        public static string HostName { get; set; }
        public static int Port { get; set; }
        public static string User { get; set; }
        public static string Password { get; set; }
        public static string ExchangeName { get; set; }
    }


}
