using System;
using System.Data;
using System.Diagnostics;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model
{
    [Serializable]
    [XmlType("Сообщение")]
    public class MessageObject: BaseMessageObject, IFillOnRow
    {
        [XmlElement(ElementName = "Событие", IsNullable = true)]
        public EventObject Event { get; set; }

        public MessageObject() { }

        public void FillOnRow(DataRow sourceRow)
        {
            Debug.WriteLine("Загружаем из строки");
            this.GetBindingAttributeValues(sourceRow);
            Event = new EventObject(sourceRow);
        }
    }
}
