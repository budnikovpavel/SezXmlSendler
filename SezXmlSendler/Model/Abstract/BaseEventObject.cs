using System;
using System.Data;
using System.Xml.Serialization;
using ERP_DAL;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model.Abstract
{
    /// <summary>
    /// Класс базового события для отправки сообщения
    /// </summary>
    public class BaseEventObject: ISerializable
    {
        [XmlAttribute(AttributeName = "ТипСобытия"), Binding(StaticValue = "РучнаяВыгрузка")]
        public string EventType { get; set; }
        [XmlAttribute(AttributeName = "ДатаВремяСобытия")]
        public string EventDate { get; set; }
        [XmlAttribute(AttributeName = "Пользователь")]
        public string User { get; set; }

        public BaseEventObject()
        {
        }

        public BaseEventObject(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
            EventDate = DateTime.Now.ToString();
            User = DAL.User;
        }
    }
}