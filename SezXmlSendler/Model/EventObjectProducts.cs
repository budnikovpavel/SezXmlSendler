using System.Xml.Serialization;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model
{
    public class EventObjectProducts : ISerializable
    {
        [XmlAttribute(AttributeName = "ТипСобытия")]
        public string EventType { get; set; }
        [XmlAttribute(AttributeName = "ДатаВремяСобытия")]
        public string EventDate { get; set; }
        [XmlAttribute(AttributeName = "Пользователь")]
        public string User { get; set; }
        [XmlElement(ElementName = "ОсновнойКонтекст", IsNullable = true)]
        public ImportedProduct Product { get; set; }
    }
}