using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model
{
    public class ImportedSostavIzdMarshrutPoint : ISerializable
    {
        [XmlAttribute(AttributeName = "Нпп"), Binding(FieldName = "ORDER_DEP")]
        public string Npp { get; set; }
        [XmlAttribute(AttributeName = "Подразделение"), Binding(FieldName = "DEPART")]
        public string Podrazd { get; set; }
        [XmlAttribute(AttributeName = "ДатаОт"), Binding(FieldName = "LFINISH_DAT")]
        public string DataOt { get; set; }
        [XmlAttribute(AttributeName = "ДатаДо"), Binding(FieldName = "LSTART_DAT")]
        public string DataDo { get; set; }
        [XmlElement(ElementName = "Операция")]
        public List<ImportedSostavIzdMarshrutOperation> Operations { get; set; }

        public ImportedSostavIzdMarshrutPoint() { Operations = new List<ImportedSostavIzdMarshrutOperation>(); }
        public ImportedSostavIzdMarshrutPoint(DataRow sourceRow) : this()
        {
            this.GetBindingAttributeValues(sourceRow);
        }
    }
}