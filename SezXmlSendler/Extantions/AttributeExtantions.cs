using SezXmlSendler.Model;
using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace SezXmlSendler.Extantions
{
    public static class AttributeExtantions
    {
       
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
