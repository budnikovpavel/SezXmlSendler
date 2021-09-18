using System;
using System.Xml.Serialization;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model.OrderSpecificationsObjects
{
    [Serializable]
    public class ImportedProductProperties : ISerializable
    {
        [XmlAttribute(AttributeName = "Наименование")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "Печатноенаименование")]
        public string TypePrint { get; set; }
        [XmlAttribute(AttributeName = "ЕИ")]
        public string KEI { get; set; }

        [XmlElement(ElementName = "Характеристики", IsNullable = true)]
        public ProductOptionsSettings ProductsSettings { get; set; }

    }
}