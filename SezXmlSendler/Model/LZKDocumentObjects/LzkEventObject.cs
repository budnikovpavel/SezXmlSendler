using System.Data;
using System.Xml.Serialization;
using SezXmlSendler.Model.Abstract;

namespace SezXmlSendler.Model.LZKDocumentObjects
{
    public class LzkEventObject : BaseEventObject
    {

        [XmlElement(ElementName = "Данные", IsNullable = true)]
        public LzkDataObject Data { get; set; }

        public LzkEventObject(DataRow sourceRow) : base(sourceRow)
        {
            Data = new LzkDataObject(sourceRow);
        }

        public LzkEventObject()
        {
        }
    }
}