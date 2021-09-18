using System.Data;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model.LZKDocumentObjects
{
    public class LzkDocumentObject:ISerializable
    {
        [XmlElement(ElementName = "ТМЦ", IsNullable = true)]
        public LzkTMCObject LzkTMC { get; set; }
        
        [XmlAttribute(AttributeName = "ВидДокументаВыгрузки"), Binding(StaticValue = "ЛЗК")]
        public string DocumentVid { get; set; }

        [XmlAttribute(AttributeName = "СинхроКод"), Binding(FieldName = "")]
        public string SinhrCode { get; set; }

        [XmlAttribute(AttributeName = "ДатаДок"), Binding(FieldName = "DATE_OUT")]
        public string DocumentDate { get; set; }
        [XmlAttribute(AttributeName = "ВремяДок"), Binding(FieldName = "TIME_DOC")]
        public string DocumentTime { get; set; }
        [XmlAttribute(AttributeName = "НомерДок"), Binding(FieldName = "NDOC")]
        public string DocumentNumber { get; set; }
        [XmlAttribute(AttributeName = "Статус"), Binding(FieldName = "STATUS")]
        public string Status { get; set; }
        [XmlAttribute(AttributeName = "Подразделение"), Binding(FieldName = "DEPARTG")]
        public string DepartG { get; set; }
        [XmlAttribute(AttributeName = "МестоХранениеОтгрузки"), Binding(FieldName = "DEPARTS")]
        public string DepartS { get; set; }
        [XmlAttribute(AttributeName = "ФирмаПолнНаименование"), Binding(FieldName = "SKLAD")]
        public string Sklad { get; set; }

        public LzkDocumentObject()
        {
        }

        public LzkDocumentObject(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
            LzkTMC = new LzkTMCObject(sourceRow);
        }
    }
}