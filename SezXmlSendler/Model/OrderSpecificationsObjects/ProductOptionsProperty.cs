using System.Xml.Serialization;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model.OrderSpecificationsObjects
{
    public class ProductOptionsProperty : ISerializable
    {
        [XmlAttribute(AttributeName = "Название")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "Значение")]
        public string Value { get; set; }
    }
}