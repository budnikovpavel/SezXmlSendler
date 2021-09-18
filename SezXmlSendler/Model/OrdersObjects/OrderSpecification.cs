using System.Data;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model.OrdersObjects
{
    public class OrderSpecification : ISerializable
    {
        [XmlElement(ElementName = "Строка", IsNullable = true)]
        public OrderSpecificationString OSS { get; set; }
        public OrderSpecification(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
            OSS = new OrderSpecificationString(sourceRow);
        }
        public OrderSpecification() { }
    }
}