using System.Data;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model.LZKDocumentObjects
{
    public class LzkTMCObject:ISerializable
    {
        [XmlElement(ElementName = "Стр", IsNullable = true)]
        public LzkStrObject LzkStr { get; set; }
        LzkTMCObject()
        {
        }

        public LzkTMCObject(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
            LzkStr = new LzkStrObject(sourceRow);
        }
    }
}