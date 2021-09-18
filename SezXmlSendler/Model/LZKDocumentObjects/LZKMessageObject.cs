using System;
using System.Data;
using System.Windows.Forms;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Abstract;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model.LZKDocumentObjects
{
    [Serializable]
    [XmlType("Сообщение")]
    public class LzkMessageObject : BaseMessageObject, IFillOnTable
    {
        [XmlElement(ElementName = "Событие", IsNullable = true)]
        public LzkEventObject Event { get; set; }

        public LzkMessageObject() { }
       
        public void FillOnTable(DataTable tbl)
        {
            foreach (DataRow dataRow in tbl.Rows)
            {
                this.GetBindingAttributeValues(dataRow);
                Event = new LzkEventObject(dataRow);
            }
        }
    }
}