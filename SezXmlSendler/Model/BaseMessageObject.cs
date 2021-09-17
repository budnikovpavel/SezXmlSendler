using System;
using System.Xml.Serialization;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model
{
    [Serializable]
    [XmlType("Базовое сообщение")]
    public class BaseMessageObject : ISerializable
    {
        [XmlAttribute(AttributeName = "Источник"), Binding(StaticValue = "AIS.SEZ")]
        public virtual string Source { get; set; }

        [XmlAttribute(AttributeName = "Организация"), Binding(StaticValue = "ООО Русэлпром СЭЗ")]
        public virtual string Company { get; set; }

        [XmlAttribute(AttributeName = "Агент"), Binding(StaticValue = "aissez2rmq")]
        public virtual string Agent { get; set; }

        [XmlAttribute(AttributeName = "ВерсияАгента"), Binding(StaticValue = "1")]
        public virtual string AgentVersion { get; set; }

    }
    
}