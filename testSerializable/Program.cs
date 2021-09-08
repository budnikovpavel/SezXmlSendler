using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Ruselprom.SerializeXml
{
    class Program
    {
       
        static void Main(string[] args)
        {

            var testObject = new RootClass()
            {
                TestClassProperty = new TestClass()
                {
                    StrColumn = "Test col\"um'n",
                    DateColumn = DateTime.Now,
                    IntColumn = 100,
                    DblColumn = 10.050123,
                    BoolColumn = false
                }
            };
            Console.WriteLine(SerializeObject(testObject));
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }

        public static string SerializeObject(RootClass testObject)
        {
            string utf8;
            var serializer = new XmlSerializer(typeof(RootClass));
            using (StringWriter writer = new Utf8StringWriter())
            {
                serializer.Serialize(writer, testObject);
                utf8 = writer.ToString();
            }
            return utf8;
        }

    }

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }

    [Serializable]
    [XmlType("Root")]
    public class RootClass
    {
        [XmlElement(ElementName = "ТестовоеСобытие", IsNullable = true)]
        public TestClass TestClassProperty { get; set; }
    }

    public class TestClass
    {
        [XmlAttribute(AttributeName = "ПолеСтроковое")]
        public string StrColumn { get; set; }

        // nullable тип нельзя сериализовать в атрибут, только в [XmlElement(IsNullable = true)],
        // поэтому делаем строковое расчётное поле в параллель, его и сериализуем в атрибут xml-а
        [XmlIgnore]
        public DateTime? DateColumn { get; set; }
        [XmlAttribute(AttributeName = "ПолеДата")]
        public string DateColumnString
        {
            get
            {
                return DateColumn.HasValue ? DateColumn.Value.ToString("yyyy/MM/dd HH:mm:ss.fff") : string.Empty;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    DateColumn = DateTime.ParseExact(
                      value, "yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None
                    );
                }
                else
                {
                    DateColumn = null;
                }
            }
        }

        [XmlAttribute(AttributeName = "ПолеЦелое")]
        public int IntColumn { get; set; }

        [XmlAttribute(AttributeName = "ПолеВещественное")]
        public double DblColumn { get; set; }

        [XmlAttribute(AttributeName = "ПолеЛогическое")]
        public Boolean BoolColumn { get; set; }
    }

}