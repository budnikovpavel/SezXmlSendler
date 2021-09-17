using System;
using System.Xml.Serialization;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model
{
    [Serializable]
    public class MessageObjectProducts : BaseMessageObject
    {
        [XmlElement(ElementName = "Событие", IsNullable = true)]
        public EventObjectProducts Event { get; set; }
    }
}