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
        public string Name { get; set; }
        /// <summary>
        /// Код задачи
        /// </summary>
        public string RoutingKey { get; set; }
        public DateTime TimeRunning { get; set; }
        public Task RunningAsync(bool needSend);
    }
    
}