using System;

namespace SezXmlSendler.Model
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class BindingAttribute: Attribute
    {
        public static readonly BindingAttribute Default = new BindingAttribute(string.Empty, string.Empty);
        public string FieldName { get; set; }
        public string StaticValue { get; set; }
        public BindingAttribute() : this(string.Empty, string.Empty) { }

        public BindingAttribute(string fieldName, string staticValue)
        {
            FieldName = string.Empty;
            StaticValue = string.Empty;
            if (!string.IsNullOrEmpty(fieldName))
                FieldName = fieldName;
            else StaticValue = staticValue;
        }
    }
}
