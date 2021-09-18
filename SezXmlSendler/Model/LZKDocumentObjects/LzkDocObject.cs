using System.Data;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model.LZKDocumentObjects
{
    public class LzkDocObject : ISerializable
    {
        public LzkDocObject()
        {
        }

        [XmlElement(ElementName = "Документ", IsNullable = true)]
        public LzkDocumentObject Document { get; set; }
        public LzkDocObject(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
            Document = new LzkDocumentObject(sourceRow);
        }
    }
}