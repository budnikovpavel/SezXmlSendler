using SezXmlSendler.Model;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Extantions
{
    public static class AttributeExtantions
    {
        /// <summary>
        /// Метод расширения для привязки значений из аттрибутов и полей из DataRow
        /// </summary>
        /// <param name="attributeOwner">Сериализирумый класс с задаными аттрибутами</param>
        /// <param name="sourceRow">строка из БД</param>
        /// <remarks>
        /// По факту производит заполнение данными из DataRow в соответствуюшие свойства объекта 
        /// Пример: у объекта есть свойство OrderNumber
        ///       [XmlAttribute(AttributeName = "НомерЗаказа"), Binding(FieldName = "ZAK")]
        ///       public string OrderNumber { get; set; }
        /// если в sourceRow будет поле ZAK, то OrderNumber примет значение из этой записи
        /// </remarks>
        public static void GetBindingAttributeValues(this ISerializable attributeOwner, DataRow sourceRow = null)
        {
            Type type = attributeOwner.GetType();

            MemberInfo[] propInfo = type.GetProperties();
            if (propInfo != null && propInfo.Length > 0)
            {
                foreach (PropertyInfo item in propInfo.Where(x => x.GetCustomAttributes(typeof(BindingAttribute), false).Any()))
                {
                    var attr = item.GetCustomAttribute(typeof(BindingAttribute), false);
                    var fieldName = ((BindingAttribute)attr).FieldName;
                    var staticValue = ((BindingAttribute)attr).StaticValue;
                    if (fieldName != string.Empty && sourceRow != null && sourceRow.Table.Columns.Contains(fieldName))
                        item.SetValue(attributeOwner, sourceRow[fieldName].ToString());
                    else
                        item.SetValue(attributeOwner, staticValue);
                }
            }
        }

    }
}
