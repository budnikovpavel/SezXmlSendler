using System.Collections.Generic;
using System.Xml.Serialization;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model.OrderSpecificationsObjects
{
    public class ProductOptionsSettings : ISerializable
    {
        [XmlElement(ElementName = "Параметр")]
        public List<ProductOptionsProperty> Props { get; set; }

    }
}