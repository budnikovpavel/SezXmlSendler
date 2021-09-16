using System;
using System.Data;
using System.Xml.Serialization;
using ERP_DAL;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model
{
    public class EventObject : ISerializable
    {
        [XmlAttribute(AttributeName = "ТипСобытия"), Binding(StaticValue = "РучнаяВыгрузка")]
        public string EventType { get; set; }
        [XmlAttribute(AttributeName = "ДатаВремяСобытия")]
        public string EventDate { get; set; }
        [XmlAttribute(AttributeName = "Пользователь")]
        public string User { get; set; }
        [XmlElement(ElementName = "Объект", IsNullable = true)]
        public ImportedOrder Object { get; set; }

        public EventObject(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
            EventDate = DateTime.Now.ToString();
            User = DAL.User;
            Object = new ImportedOrder(sourceRow);
        }
        public EventObject() { }
    }
}