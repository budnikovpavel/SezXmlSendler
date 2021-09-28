using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Interfaces;
using SezXmlSendler.Model.OrderSpecificationsObjects;

namespace SezXmlSendler.Model
{
    public class ImportedSostavIzdNode : ISerializable
    {
        [XmlAttribute(AttributeName = "Нпп"), Binding(FieldName = "USEL_NPP")]
        public string Npp { get; set; }
        [XmlAttribute(AttributeName = "LevelNumber"), Binding(FieldName = "LEVELNUMBER")]
        public string LevelNumber { get; set; }
        [XmlAttribute(AttributeName = "ParentId"), Binding(FieldName = "WAY1")]
        public string ParentId { get; set; }
        [XmlAttribute(AttributeName = "ChildId"), Binding(FieldName = "WAY")]
        public string ChildId { get; set; }
        [XmlAttribute(AttributeName = "ItemId"), Binding(FieldName = "ITEMID")]
        public string ItemId { get; set; }
        [XmlAttribute(AttributeName = "Наименование"), Binding(FieldName = "NDSE")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "НомерЧертежа"), Binding(FieldName = "ODSE")]
        public string ODSE { get; set; }
        [XmlAttribute(AttributeName = "МАРКА"), Binding(FieldName = "MARK")]
        public string Mark { get; set; }
        [XmlAttribute(AttributeName = "РАЗМЕР"), Binding(FieldName = "MSIZE")]
        public string MSize { get; set; }
        [XmlAttribute(AttributeName = "ГОСТ"), Binding(FieldName = "GOST")]
        public string Gost { get; set; }
        [XmlAttribute(AttributeName = "КЛАСС"), Binding(FieldName = "NCLM")]
        public string Nclass { get; set; }
        [XmlAttribute(AttributeName = "ПОДКЛАСС"), Binding(FieldName = "NSBCLASS")]
        public string NSubclass { get; set; }

        [XmlAttribute(AttributeName = "ПозицияВЧертеже"), Binding(FieldName = "NPOS")]
        public string PozVChert { get; set; }
        [XmlAttribute(AttributeName = "ЕдИзм"), Binding(FieldName = "KEI")]
        public string EdIzm { get; set; }
        [XmlAttribute(AttributeName = "QBrutto"), Binding(FieldName = "QBRUTTO")]
        public string QBrutto { get; set; }
        [XmlAttribute(AttributeName = "QNetto"), Binding(FieldName = "QNETTO")]
        public string QNetto { get; set; }
        [XmlAttribute(AttributeName = "ChildTypeId"), Binding(FieldName = "NRS")]
        public string ChildTypeId { get; set; }

        [XmlAttribute(AttributeName = "ChildTypeName"), Binding(FieldName = "NNRS")]
        public string ChildTypeName { get; set; }
        [XmlAttribute(AttributeName = "ЕдИзмЦены"), Binding(FieldName = "KEI_PRICE")]
        public string EdIzmPrice { get; set; }
        [XmlAttribute(AttributeName = "ЦенаМатериала"), Binding(FieldName = "PRICE")]
        public string ЦенаМатериала { get; set; }

        [XmlAttribute(AttributeName = "ДатаОбновления"), Binding(FieldName = "SYSDATE")]
        public string DateObn { get; set; }
        [XmlElement(ElementName = "Маршрут", IsNullable = true)]
        public List<ImportedSostavIzdTechMarshrut> TechMarshruts { get; set; }

        public ImportedSostavIzdNode() { TechMarshruts = new List<ImportedSostavIzdTechMarshrut>(); }
        public ImportedSostavIzdNode(DataRow sourceRow) : this()
        {
            this.GetBindingAttributeValues(sourceRow);
        }
    }
}