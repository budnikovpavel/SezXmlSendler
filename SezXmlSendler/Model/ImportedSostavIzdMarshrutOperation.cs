using System.Data;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model
{
    public class ImportedSostavIzdMarshrutOperation : ISerializable
    {

        [XmlAttribute(AttributeName = "Нпп"), Binding(FieldName = "ORDER_NO")]
        public string Npp { get; set; }
        [XmlAttribute(AttributeName = "ВидОперации"), Binding(FieldName = "NAME_TO")]
        public string VidOperation { get; set; }
        [XmlAttribute(AttributeName = "Оборудование"), Binding(FieldName = "PASSPORT")]
        public string Oborud { get; set; }
        [XmlAttribute(AttributeName = "КодПрофессии"), Binding(FieldName = "KPRF")]
        public string KodProfessii { get; set; }
        [XmlAttribute(AttributeName = "Разряд"), Binding(FieldName = "CATEGORY")]
        public string Razrad { get; set; }
        [XmlAttribute(AttributeName = "ЕдИзм"), Binding(FieldName = "KEI1")]
        public string EdIzm { get; set; }
        [XmlAttribute(AttributeName = "Партия"), Binding(FieldName = "OB_PART")]
        public string PArtiya { get; set; }
        [XmlAttribute(AttributeName = "Длительность"), Binding(FieldName = "TO_TIME")]
        public string Dlitelnost { get; set; }
        [XmlAttribute(AttributeName = "Работников"), Binding(FieldName = "KOL_OP")]
        public string Rabotnikov { get; set; }
        [XmlAttribute(AttributeName = "Трудоемкость"), Binding(FieldName = "TRUD")]
        public string Trudoemkost { get; set; }

        public ImportedSostavIzdMarshrutOperation() { }
        public ImportedSostavIzdMarshrutOperation(DataRow sourceRow) { this.GetBindingAttributeValues(sourceRow); }
    }
}