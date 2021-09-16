using System.Data;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model
{
    public class ImportedOrder : ISerializable
    {
        [XmlAttribute(AttributeName = "Тип"), Binding(StaticValue = "Электрические машины")]
        public string ObjectType { get; set; }
        [XmlAttribute(AttributeName = "Код"), Binding(FieldName = "ID_TASK")]
        public string Code { get; set; }
        [XmlAttribute(AttributeName = "НомерЗаказа"), Binding(FieldName = "ZAK")]
        public string OrderNumber { get; set; }
        [XmlAttribute(AttributeName = "ДатаСоздания"), Binding(FieldName = "DATEP2")]
        public string DateCreated { get; set; }
        [XmlAttribute(AttributeName = "Заказчик"), Binding(FieldName = "NAME")]
        public string Customer { get; set; }
        [XmlAttribute(AttributeName = "Поставщик"), Binding(StaticValue = "ООО Русэлпром. Электрические машины")]
        public string Supplier { get; set; }
        [XmlAttribute(AttributeName = "Производитель"), Binding(FieldName = "SNAME")]
        public string Manufacturer { get; set; }
        [XmlAttribute(AttributeName = "ДоговорПоставки"), Binding(FieldName = "CONTRACT_DATA")]
        public string DeliveryContract { get; set; }

        [XmlAttribute(AttributeName = "ДоговорПроизводство"), Binding(FieldName = "CONTRACT_DAT")]
        public string DeliveryManufacture { get; set; }
        [XmlElement(ElementName = "Спецификация", IsNullable = true)]
        public OrderSpecification Specification { get; set; }

        public ImportedOrder(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
            Specification = new OrderSpecification(sourceRow);
        }
        public ImportedOrder() { }
    }
}