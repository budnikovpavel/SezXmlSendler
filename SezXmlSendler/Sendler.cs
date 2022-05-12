using RabbitMQ.Client;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SezXmlSendler.Model;
using SezXmlSendler.Model.Abstract;
using SezXmlSendler.Model.Interfaces;
using RabbitMQ.Client.Events;
using System.Linq;

namespace SezXmlSendler
{
    public partial class Sendler<TBaseMessageObject> : ISendler
        where TBaseMessageObject : BaseMessageObject
    {
        private readonly Func<DataRow, string, DataTable> _filledTable;
        private readonly Action<ISendler, int> _processingPercent;

        public delegate DataTable LoadTableAction<T>(out T key);

        public delegate void SerializeObjectEventHandler(object sender, string serialize);

        public delegate void LogEventHandler(object sender, string log);

        public event SerializeObjectEventHandler OnSerializeObject;
        public event ErrorEventHandler OnError;
        public event LogEventHandler OnSended;
        public event LogEventHandler OnLog;
        public event LogEventHandler OnReceive;
        public LoadTableAction<string> OnLoadTable;

        private int? _processingPercentValue;
        public DateTime TimeRunning { get; set; }

        public bool Cancel { get; set; } = true;


        public bool IsRunning => !Cancel;


        public async Task RunningAsync(bool needSend)
        {
            Cancel = false;
            var key = string.Empty;
            DataTable sourceTbl = null;
            try
            {

                if (OnLoadTable != null) sourceTbl = OnLoadTable.Invoke(out key);
                if (sourceTbl != null)
                    showLog($"Загрузили таблицу с данными: найдено строк {sourceTbl.Rows.Count}");

            }
            catch (Exception err)
            {
                if (OnError != null) OnError.Invoke(this, new ErrorEventArgs(err));
                Name = _stockName;
            }


            if (sourceTbl != null)
            {
                var i = 1;
                foreach (DataRow item in sourceTbl.Rows)
                {
                    
                    var keyValue = item.Table.Columns.Contains(key) ? $" - Ключ {item[key]}" : "";
                    showLog($"{keyValue} обрабатываем строку {i} из {sourceTbl.Rows.Count} ");
                    if (_processingPercent != null)
                    {
                        _processingPercentValue = (int)Math.Round(i / (float)sourceTbl.Rows.Count * 100);
                        Name = ToString();
                        _processingPercent(this, (int)_processingPercentValue);
                    }
                   
                    var mess = Activator.CreateInstance(typeof(TBaseMessageObject));
                    //showLog($"Инициализировался объект {mess}");
                    if ((mess as IFillOnRow) != null)
                    {
                        (mess as IFillOnRow).FillOnRow(item);
                        //showLog($"Заполнили объект {mess} данными из строки");
                    }
                    else
                    {
                        if ((mess as IFillOnTable) != null)
                        {
                            try
                            {

                                if (_filledTable != null)
                                {
                                    var fillTbl = _filledTable(item, key);
                                    ((mess as IFillOnTable)).FillOnTable(fillTbl);
                                }
                            }
                            catch (Exception err)
                            {
                                if (OnError != null)
                                    OnError(this, new ErrorEventArgs(new Exception($" Ошибка загрузки данных из таблицы : {err.Message}")));
                                Name = _stockName;
                            }
                            showLog($"Заполнили объект {mess} данными из таблицы");
                        }
                    }

                    try
                    {
                        var str = SerializeObject(mess);

                        //showLog($"Объект {mess} сериализован");
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
                        Name = _stockName;

                    }

                    i += 1;
                    if (Cancel)
                    {
                        Name = _stockName;
                        _processingPercentValue = null;
                        showLog($"Задача {_stockName} отменена!");
                        return;
                    }
                }
            }
            _processingPercentValue = null;
            Cancel = true;
            showLog($"Задача {_stockName} выполнена успешно");
           
        }

        private void showLog(string messageLog)
        {
            OnLog?.Invoke(this, messageLog);
        }

        private void receiveMessage(string receiveMessage)
        {
            OnReceive?.Invoke(this, receiveMessage);
        }

        public string RoutingKey { get; set; }
        public string QueueName { get; set; }
        private string _stockName;
        public string Name { get; set; }
        public Sendler(string name, string routingKey,
            LoadTableAction<string> loadTable = default,
            Func<DataRow, string, DataTable> filledTable = default,
            Action<ISendler, int> processing = default)
        {
            _stockName = name;
            Name = name;
            RoutingKey = routingKey;
            OnLoadTable = loadTable;
            _filledTable = filledTable;
            _processingPercent = processing;
        }

        public Sendler(string name, string queueName)
        {
            _stockName = name;
            Name = name;
            QueueName = queueName;
        }

        public override string ToString()
        {
            
            var time = TimeRunning != null ? $"время запуска {TimeRunning.TimeOfDay}" : "";
            var processing = _processingPercentValue != null ? $" - {_processingPercentValue}%" : time;
            var result = $"{_stockName} {processing}";
            
            return result;

        }

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

        public async Task ReceiveAsync()
        {
            Cancel = false;
            var factory = new ConnectionFactory()
            {
                HostName = RabbitMQConnectionParameters.HostName,
                Port = RabbitMQConnectionParameters.Port,
                VirtualHost = "/",
                UserName = RabbitMQConnectionParameters.User,
                Password = RabbitMQConnectionParameters.Password
            };
            try
            {
                //showLog($"{DateTime.Now} - factory created");
                using (var connection = factory.CreateConnection())
                {
                    //showLog($"{DateTime.Now} - factory.CreateConnection() - ok");
                    using (var channel = connection.CreateModel())
                    {
                        showLog($"{DateTime.Now} - connection.CreateModel() - ok");
                        channel.QueueDeclare(QueueName, true, false, false, null);
                        var consumer = new EventingBasicConsumer(channel);
                        showLog($"{DateTime.Now} - Очередь {QueueName} задекларирована!");
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            showLog($"{DateTime.Now} - Очередь {QueueName} получено сообщение: {ea.Body}!");
                            receiveMessage(Encoding.UTF8.GetString(body.ToArray()));
                            Cancel = true;
                        };

                        channel.BasicConsume(QueueName, true, consumer);
                    }
                }
            }
            catch(Exception err)
            {
                if (OnError != null)
                    OnError(this, new ErrorEventArgs(new Exception($" Ошибка: {err.Message}")));
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
