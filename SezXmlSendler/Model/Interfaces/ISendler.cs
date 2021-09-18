using System;
using System.Threading.Tasks;

namespace SezXmlSendler.Model.Interfaces
{
    /// <summary>
    /// Интерфейс класса для формирования отправки сообщений RabbitMQ
    /// </summary>
    public interface ISendler
    {
        /// <summary>
        /// Наименование задачи
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Код задачи
        /// </summary>
        string RoutingKey { get; set; }
        DateTime TimeRunning { get; set; }

        bool Cancel { get; set; }
        bool IsRunning { get; }
        public Task RunningAsync(bool needSend);
    }
    
}