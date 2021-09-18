using System;
using System.Xml.Serialization;
using SezXmlSendler.Model.Abstract;
using SezXmlSendler.Model.OrderSpecificationsObjects;

namespace SezXmlSendler.Model.OrdersObjects
{
    [Serializable]
    public class MessageObjectProducts : BaseMessageObject
    {
        [XmlElement(ElementName = "Событие", IsNullable = true)]
        public EventObjectProducts Event { get; set; }
    }
}