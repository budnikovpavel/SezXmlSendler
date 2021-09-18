using System.Data;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model.LZKDocumentObjects
{
    public class LzkDataObject : ISerializable
    {

        [XmlElement(ElementName = "Док", IsNullable = true)]
        public LzkDocObject Doc { get; set; }
        public LzkDataObject()
        {
        }

        public LzkDataObject(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
            Doc = new LzkDocObject(sourceRow);
        }
    }
}