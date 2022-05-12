using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace SezXmlSendler.Model.XMLParser
{
    public partial class XMLParser
    {
        readonly List<PropertyValue> NodesInfo = new List<PropertyValue>();
        public readonly Dictionary<string, List<string>> ObjValues = new Dictionary<string, List<string>>();
        string currentNode = string.Empty;

        public void Parse(string xmlString)
        {
            NodesInfo.Clear();
            ObjValues.Clear();
            XmlReader reader = XmlReader.Create(new StringReader(xmlString));

            while (reader.Read())
            {
                GetNodeInfo(reader.Name, reader.Value, reader.NodeType);
                if (reader.AttributeCount > 0)
                {
                    for (int i = 0; i < reader.AttributeCount; i++)
                    {
                        reader.MoveToAttribute(i);
                        GetNodeInfo(reader.Name, reader.Value, reader.NodeType);
                    }
                }
            }
        }
        void GetNodeInfo(string key, string value, XmlNodeType nodeType)
        {
            switch (nodeType)
            {
                case XmlNodeType.None:
                    break;
                case XmlNodeType.Element:
                    currentNode = currentNode == string.Empty ? key
                        : currentNode.EndsWith($".{key}") ? currentNode : $"{currentNode}.{key}";

                    if (!ObjValues.ContainsKey(currentNode))
                    {
                        ObjValues.Add(currentNode, new List<string>());
                    }

                    NodesInfo.Add(new PropertyValue { TypeNode = nodeType, Name = currentNode, Value = string.Empty });
                    break;
                case XmlNodeType.Attribute:
                    var attrName = $"{currentNode}.@{key}";

                    if (!ObjValues.ContainsKey(attrName))
                    {
                        ObjValues.Add(attrName, new List<string>());
                    }

                    NodesInfo.Add(new PropertyValue { TypeNode = nodeType, Name = attrName, Value = value });
                    ObjValues[NodesInfo.LastOrDefault().Name].Add(value);
                    break;
                case XmlNodeType.Text:
                    break;
                case XmlNodeType.CDATA:
                    break;
                case XmlNodeType.EntityReference:
                    break;
                case XmlNodeType.Entity:
                    break;
                case XmlNodeType.ProcessingInstruction:
                    break;
                case XmlNodeType.Comment:
                    break;
                case XmlNodeType.Document:
                    break;
                case XmlNodeType.DocumentType:
                    break;
                case XmlNodeType.DocumentFragment:
                    break;
                case XmlNodeType.Notation:
                    break;
                case XmlNodeType.Whitespace:
                    break;
                case XmlNodeType.SignificantWhitespace:
                    break;
                case XmlNodeType.EndElement:
                    currentNode = currentNode.Contains('.') ? currentNode.Remove(currentNode.LastIndexOf('.')) : string.Empty;
                    break;
                case XmlNodeType.EndEntity:
                    break;
                case XmlNodeType.XmlDeclaration:
                    break;
                default:
                    break;
            }
        }
    }
}
