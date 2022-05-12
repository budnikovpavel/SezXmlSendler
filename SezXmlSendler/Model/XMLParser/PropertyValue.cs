using System.Xml;

namespace SezXmlSendler.Model.XMLParser
{
    public partial class XMLParser
    {
        public class PropertyValue
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public XmlNodeType TypeNode { get; set; }
        }
    }
}
