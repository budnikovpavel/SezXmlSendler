using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SezXmlSendler.Model
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BindingAttribute: Attribute
    {
        public static readonly BindingAttribute Default = new BindingAttribute(string.Empty, string.Empty);
        public virtual string FieldName { get; set; }
        public virtual string StaticValue { get; set; }
        public BindingAttribute() : this(string.Empty, string.Empty) { }
        public BindingAttribute(string fieldName, string staticValue)
        {
            FieldName = string.Empty;
            StaticValue = string.Empty;
            if (!string.IsNullOrEmpty(fieldName))
            { FieldName = fieldName; }
            else { StaticValue = staticValue; }
        }
    }
}
