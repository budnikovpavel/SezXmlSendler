using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model.OrderSpecificationsObjects
{
    [Serializable]
    public class ImportedSostavIzd : ISerializable
    {
        [XmlElement(ElementName = "Узел", IsNullable = true)]
        public List<ImportedSostavIzdNode> Nodes { get; set; }
        public ImportedSostavIzd()
        {
            Nodes = new List<ImportedSostavIzdNode>();
        }
    }
}