using System.Xml.Serialization;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model.OrderSpecificationsObjects
{
    public class ImportedProduct : ISerializable
    {
        [XmlAttribute(AttributeName = "ВидОбъекта")]
        public string ObjectVid { get; set; }
        [XmlAttribute(AttributeName = "ТипОбъекта")]
        public string ObjectType { get; set; }
        [XmlAttribute(AttributeName = "ИдентификаторОбъекта")]
        public string NP { get; set; }
        [XmlElement(ElementName = "Свойства", IsNullable = true)]
        public ImportedProductProperties ProductsOptions { get; set; }
    }
}