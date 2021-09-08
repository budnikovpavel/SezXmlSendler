using ERP_DAL;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SezXmlSendler
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
    public interface ISerializable
    {

    }
    [Serializable]
    [XmlType("Сообщение")]
    public class MessageObject : ISerializable
    {
        [XmlAttribute(AttributeName = "Источник"), Binding(StaticValue = "AIS.SEZ")]
        public string Source { get; set; }

        [XmlAttribute(AttributeName = "Организация"), Binding(StaticValue = "ООО Русэлпром СЭЗ")]
        public string Company { get; set; }

        [XmlAttribute(AttributeName = "Агент"), Binding(StaticValue = "aissez2rmq")]
        public string Agent { get; set; }

        [XmlAttribute(AttributeName = "ВерсияАгента"), Binding(StaticValue = "1")]
        public string AgentVersion { get; set; }
        [XmlElement(ElementName = "Событие", IsNullable = true)]
        public EventObject Event { get; set; }

        public MessageObject() { }
        public MessageObject(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
            this.Event = new EventObject(sourceRow);
        }
    }

    public class EventObject : ISerializable
    {
        [XmlAttribute(AttributeName = "ТипСобытия"), Binding(StaticValue = "РучнаяВыгрузка")]
        public string EventType { get; set; }
        [XmlAttribute(AttributeName = "ДатаВремяСобытия")]
        public string EventDate { get; set; }
        [XmlAttribute(AttributeName = "Пользователь")]
        public string User { get; set; }
        [XmlElement(ElementName = "Объект", IsNullable = true)]
        public ImportedOrder Object { get; set; }

        public EventObject(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
            EventDate = DateTime.Now.ToString();
            User = DAL.User;
            Object = new ImportedOrder(sourceRow);
        }
        public EventObject() { }
    }

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

    public class OrderSpecification : ISerializable
    {
        [XmlElement(ElementName = "Строка", IsNullable = true)]
        public OrderSpecificationString OSS { get; set; }
        public OrderSpecification(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
            OSS = new OrderSpecificationString(sourceRow);
        }
        public OrderSpecification() { }
    }
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




    /****************выгрузка продукции*********************/
    [Serializable]
    [XmlType("Сообщение")]
    public class MessageObjectProducts : ISerializable
    {
        [XmlAttribute(AttributeName = "Источник")]
        public string Source { get; set; }

        [XmlAttribute(AttributeName = "Организация")]
        public string Company { get; set; }

        [XmlAttribute(AttributeName = "Агент")]
        public string Agent { get; set; }

        [XmlAttribute(AttributeName = "ВерсияАгента")]
        public string AgentVersion { get; set; }
        [XmlElement(ElementName = "Событие", IsNullable = true)]
        public EventObjectProducts Event { get; set; }
    }

    public class EventObjectProducts : ISerializable
    {
        [XmlAttribute(AttributeName = "ТипСобытия")]
        public string EventType { get; set; }
        [XmlAttribute(AttributeName = "ДатаВремяСобытия")]
        public string EventDate { get; set; }
        [XmlAttribute(AttributeName = "Пользователь")]
        public string User { get; set; }
        [XmlElement(ElementName = "ОсновнойКонтекст", IsNullable = true)]
        public ImportedProduct Product { get; set; }
    }

    public class ImportedProduct : ISerializable
    {
        [XmlAttribute(AttributeName = "ВидОбъекта")]
        public string ObjectVid { get; set; }
        [XmlAttribute(AttributeName = "ТипОбъекта")]
        public string ObjectType { get; set; }
        [XmlAttribute(AttributeName = "ИдентификаторОбъекта")]
        public string NP { get; set; }
        [XmlElement(ElementName = "Свойства", IsNullable = true)]
        public ImportedProductProperties ProductsOptions { get; set; }
    }
    [Serializable]
    public class ImportedProductProperties : ISerializable
    {
        [XmlAttribute(AttributeName = "Наименование")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "Печатноенаименование")]
        public string TypePrint { get; set; }
        [XmlAttribute(AttributeName = "ЕИ")]
        public string KEI { get; set; }

        [XmlElement(ElementName = "Характеристики", IsNullable = true)]
        public ProductOptionsSettings ProductsSettings { get; set; }

    }

    public class ProductOptionsSettings : ISerializable
    {
        [XmlElement(ElementName = "Параметр")]
        public List<PropductOptionsProperty> Props { get; set; }

    }
    public class PropductOptionsProperty : ISerializable
    {
        [XmlAttribute(AttributeName = "Название")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "Значение")]
        public string Value { get; set; }
    }

    /********* структура продукта в заказе *********/
    [Serializable]
    [XmlType("Сообщение")]
    public class MessagetProdinTask : ISerializable
    {
        [XmlAttribute(AttributeName = "Источник"), Binding(StaticValue = "AIS.SEZ")]
        public string Source { get; set; }

        [XmlAttribute(AttributeName = "Организация"), Binding(StaticValue = "ООО Русэлпром СЭЗ")]
        public string Company { get; set; }

        [XmlAttribute(AttributeName = "Агент"), Binding(StaticValue = "aissez2rmq")]
        public string Agent { get; set; }

        [XmlAttribute(AttributeName = "ВерсияАгента"), Binding(StaticValue = "1")]
        public string AgentVersion { get; set; }
        [XmlElement(ElementName = "Событие", IsNullable = true)]
        public EventObjectProdinTask Event { get; set; }
        public MessagetProdinTask(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
            Event = new EventObjectProdinTask(sourceRow);
        }
        public MessagetProdinTask() { }
    }

    public class EventObjectProdinTask : ISerializable
    {
        [XmlAttribute(AttributeName = "ТипСобытия"), Binding(StaticValue = "РучнаяВыгрузка")]
        public string EventType { get; set; }
        [XmlAttribute(AttributeName = "ДатаВремяСобытия")]
        public string EventDate { get; set; }
        [XmlAttribute(AttributeName = "Пользователь")]
        public string User { get; set; }
        [XmlElement(ElementName = "Объект", IsNullable = true)]
        public ImportedProdInTask Product { get; set; }
        public EventObjectProdinTask(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
            EventDate = DateTime.Now.ToString();
            User = DAL.User;
            Product = new ImportedProdInTask(sourceRow);
        }
        public EventObjectProdinTask() { }
    }

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
        [XmlAttribute(AttributeName = "КЛАСС"), Binding(FieldName = "NCLS")]
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
