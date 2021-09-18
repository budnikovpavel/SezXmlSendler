using System;
using System.Data;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Abstract;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model.OrdersObjects
{
    [Serializable]
    [XmlType("Сообщение")]
    public class OrderMessageObject: BaseMessageObject, IFillOnRow
    {
        [XmlElement(ElementName = "Событие", IsNullable = true)]
        public OrderEventObject Event { get; set; }

       // public OrderMessageObject() { }

        public void FillOnRow(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
            Event = new OrderEventObject(sourceRow);
        }
    }
}
