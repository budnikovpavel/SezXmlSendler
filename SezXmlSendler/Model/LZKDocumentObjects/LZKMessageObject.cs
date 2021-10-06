using System;
using System.Data;
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

        public void FillOnTable(DataTable tbl)
        {
            this.GetBindingAttributeValues();
            foreach (DataRow dataRow in tbl.Rows)
            {
                if (Event == null)
                    Event = new LzkEventObject(dataRow);
                else Event.Data.Doc.Document.LzkTMC.Add(new LzkTMCObject(dataRow));
            }
        }
    }
}