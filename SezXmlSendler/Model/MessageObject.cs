using System;
using System.Data;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model
{
    [Serializable]
    [XmlType("Сообщение")]
    public class MessageObject : ISerializable
    {
        [XmlAttribute(AttributeName = "Источник"), Binding(StaticValue = "AIS.SEZ")]
        public string Source { get; set; }

        [XmlAttribute(AttributeName = "Организация"), Binding(StaticValue = "ООО Русэлпром СЭЗ")]
        public string Company { get; set; }

        [XmlAttribute(AttributeName = "Агент"), Binding(StaticValue = "aissez2rmq")]
        public string Agent { get; set; }

        [XmlAttribute(AttributeName = "ВерсияАгента"), Binding(StaticValue = "1")]
        public string AgentVersion { get; set; }
        [XmlElement(ElementName = "Событие", IsNullable = true)]
        public EventObject Event { get; set; }

        public MessageObject() { }
        public MessageObject(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
            this.Event = new EventObject(sourceRow);
        }
    }
}
