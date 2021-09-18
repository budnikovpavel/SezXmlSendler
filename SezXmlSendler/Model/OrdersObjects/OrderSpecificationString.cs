using System.Data;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model.OrdersObjects
{
    public class OrderSpecificationString : ISerializable
    {
        [XmlAttribute(AttributeName = "Нпп"), Binding(FieldName = "")]
        public string Npp { get; set; }
        [XmlAttribute(AttributeName = "Тип"), Binding(StaticValue = "Электрические машины")]
        public string OssType { get; set; }
        [XmlAttribute(AttributeName = "Код"), Binding(FieldName = "NP")]
        public string OssCode { get; set; }
        [XmlAttribute(AttributeName = "Наименование"), Binding(FieldName = "TYPE")]
        public string OssName { get; set; }
        [XmlAttribute(AttributeName = "Примечание"), Binding(FieldName = "NOTE")]
        public string OssNote { get; set; }
        [XmlAttribute(AttributeName = "ЕдИзм"), Binding(FieldName = "KEI")]
        public string OssSNEI { get; set; }
        [XmlAttribute(AttributeName = "Количество"), Binding(FieldName = "QUANTITY")]
        public string OssQuantity { get; set; }
        [XmlAttribute(AttributeName = "ДатаПлан"), Binding(FieldName = "DATEP2")]
        public string OssDatePlan { get; set; }
        [XmlAttribute(AttributeName = "НомерЧертежа"), Binding(FieldName = "ODSE")]
        public string OssOdse { get; set; }
        public OrderSpecificationString(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
        }
        public OrderSpecificationString() { }
    }
}