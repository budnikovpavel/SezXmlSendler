using System.Data;
using System.Xml.Serialization;
using SezXmlSendler.Model.Abstract;

namespace SezXmlSendler.Model.OrdersObjects
{
    public class OrderEventObject : BaseEventObject
    {
        
        [XmlElement(ElementName = "Объект", IsNullable = true)]
        public OrderObject Object { get; set; }

        public OrderEventObject(DataRow sourceRow):base(sourceRow)
        {
            Object = new OrderObject(sourceRow);
        }

        public OrderEventObject()
        {
        }
    }
}