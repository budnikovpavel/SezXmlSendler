using System;
using System.Xml.Serialization;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model
{
    [Serializable]
    [XmlType("Сообщение")]
    public class MessageObjectProducts : ISerializable
    {
        [XmlAttribute(AttributeName = "Источник")]
        public string Source { get; set; }

        [XmlAttribute(AttributeName = "Организация")]
        public string Company { get; set; }

        [XmlAttribute(AttributeName = "Агент")]
        public string Agent { get; set; }

        [XmlAttribute(AttributeName = "ВерсияАгента")]
        public string AgentVersion { get; set; }
        [XmlElement(ElementName = "Событие", IsNullable = true)]
        public EventObjectProducts Event { get; set; }
    }
}