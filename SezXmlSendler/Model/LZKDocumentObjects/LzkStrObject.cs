using System.Data;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model.LZKDocumentObjects
{
    public class LzkStrObject:ISerializable
    {
        [XmlAttribute(AttributeName = "Нпп"), Binding(FieldName = "")]
        public virtual string Npp { get; set; }

        [XmlAttribute(AttributeName = "ГруппаТМЦНаим1"), Binding(FieldName = "GR1")]
        public virtual string GroupTMCName1 { get; set; }

        [XmlAttribute(AttributeName = "ГруппаТМЦНаим2"), Binding(FieldName = "KNCLM")]
        public virtual string GroupTMCName2 { get; set; }

        [XmlAttribute(AttributeName = "ГруппаТМЦНаим3"), Binding(FieldName = "KNSBCLASS")]
        public virtual string GroupTMCName3 { get; set; }

        [XmlAttribute(AttributeName = "ЕдКрНаим"), Binding(FieldName = "SNEI")]
        public virtual string SNEI { get; set; }

        [XmlAttribute(AttributeName = "ТМЦКод"), Binding(FieldName = "KM")]
        public virtual string MaterialCode { get; set; }

        [XmlAttribute(AttributeName = "ТМЦНаим"), Binding(FieldName = "NM")]
        public virtual string MaterialName { get; set; }

        [XmlAttribute(AttributeName = "МаркаГОСТ"), Binding(FieldName = "GOST_MARK")]
        public virtual string GostMark { get; set; }

        [XmlAttribute(AttributeName = "Марка"), Binding(FieldName = "MARK")]
        public virtual string Mark { get; set; }

        [XmlAttribute(AttributeName = "Сортамент"), Binding(FieldName = "MSIZE")]
        public virtual string MSize { get; set; }

        [XmlAttribute(AttributeName = "СортаментГОСТ"), Binding(FieldName = "GOST")]
        public virtual string Gost { get; set; }

        [XmlAttribute(AttributeName = "ВидТМЦ"), Binding(FieldName = "PR")]
        public virtual string Pr { get; set; }

        [XmlAttribute(AttributeName = "Затребовано"), Binding(FieldName = "POTR")]
        public virtual string Required { get; set; }

        [XmlAttribute(AttributeName = "Отпущено"), Binding(FieldName = "QUANTITY")]
        public virtual string Quantity { get; set; }

        [XmlAttribute(AttributeName = "Сумма"), Binding(FieldName = "SUMMA")]
        public virtual string Summ { get; set; }

        [XmlAttribute(AttributeName = "Заказ"), Binding(FieldName = "ZAK")]
        public virtual string Zak { get; set; }

        [XmlAttribute(AttributeName = "КонстрКод"), Binding(FieldName = "KM_OLD")]
        public virtual string CodeOld { get; set; }

        [XmlAttribute(AttributeName = "КонстрЕдКрНаим"), Binding(FieldName = "KEI_OLD")]
        public virtual string KeiOld { get; set; }

        [XmlAttribute(AttributeName = "ПоНорме"), Binding(FieldName = "QUANTITY_OLD")]
        public virtual string QuantityOld { get; set; }

        public LzkStrObject()
        {
        }

        public LzkStrObject(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
        }
    }
}