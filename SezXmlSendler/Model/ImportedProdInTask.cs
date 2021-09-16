using System.Data;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model
{
    public class ImportedProdInTask : ISerializable
    {
        [XmlAttribute(AttributeName = "Тип"), Binding(StaticValue = "ПродуктЗаказа")]
        public string Tip { get; set; }
        [XmlAttribute(AttributeName = "Заказ"), Binding(FieldName = "ID_TASK")]
        public string Zakaz { get; set; }
        [XmlAttribute(AttributeName = "НомерЗаказа"), Binding(FieldName = "ZAK")]
        public string NomerZakaza { get; set; }
        [XmlAttribute(AttributeName = "Код"), Binding(FieldName = "NP")]
        public string Kod { get; set; }
        [XmlAttribute(AttributeName = "Наименование"), Binding(FieldName = "TYPE")]
        public string Name { get; set; }
        [XmlElement(ElementName = "СоставИзделия", IsNullable = true)]
        public ImportedSostavIzd SostavIzd { get; set; }
        public ImportedProdInTask(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
            SostavIzd = new ImportedSostavIzd();
        }
        public ImportedProdInTask() { }
    }
}