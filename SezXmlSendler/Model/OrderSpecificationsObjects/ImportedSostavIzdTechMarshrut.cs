using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model.OrderSpecificationsObjects
{
    public class ImportedSostavIzdTechMarshrut : ISerializable
    {
        [XmlAttribute(AttributeName = "id_tp"), Binding(FieldName = "ID_TP")]
        public string IdTp { get; set; }
        [XmlElement(ElementName = "ТочкаМаршрута")]
        public List<ImportedSostavIzdMarshrutPoint> Marshrut { get; set; }

        public ImportedSostavIzdTechMarshrut() { Marshrut = new List<ImportedSostavIzdMarshrutPoint>(); }

        public ImportedSostavIzdTechMarshrut(DataRow sourceRow) : this()
        {
            this.GetBindingAttributeValues(sourceRow);
        }
    }
}