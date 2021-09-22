using System.Data;
using System.Xml.Serialization;
using NUnit.Framework;
using SezXmlSendler;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model;
using SezXmlSendler.Model.Abstract;
using SezXmlSendler.Model.Interfaces;
using SezXmlSendler.Model.LZKDocumentObjects;
using SezXmlSendler.Model.OrdersObjects;
using SezXmlSendler.Model.OrderSpecificationsObjects;

namespace SezXmlSendlerTests
{
    [TestFixture]
    public class SezXmlSendlerTest
    {
        private Sendler<BaseMessageObject> _sendler;
        [SetUp]
        public void SetUp()
        {
            
            _sendler = new Sendler<BaseMessageObject>("", "")
            {
                OnLoadTable = null,
                TimeRunning = default,
                Cancel = false
            };
        }

        [Test]
        public void SerializeMessageProdInTaskTest()
        {
            var result = _sendler.SerializeObject(new MessageProdInTask());

            Assert.IsFalse(string.IsNullOrEmpty(result), $"Объект {nameof(MessageProdInTask)} не может быть сериализован");
        }

        [Test]
        public void SerializeMessageObjectProductsTest()
        {
            var result = _sendler.SerializeObject(new MessageObjectProducts());

            Assert.IsFalse(string.IsNullOrEmpty(result), $"Объект {nameof(MessageObjectProducts)} не может быть сериализован");
        }

        [Test]
        public void SerializeLzkMessageObjectTest()
        {
            var result = _sendler.SerializeObject(new LzkMessageObject());

            Assert.IsFalse(string.IsNullOrEmpty(result),
                $"Объект {nameof(LzkMessageObject)} не может быть сериализован");
        }


        [Test]
        public void GetBindingAttributeValuesTest()
        {
            var tbl = new DataTable();
            tbl.Columns.Add(new DataColumn() { ColumnName = "Field1", Caption = "Поле 1"});
            tbl.Columns.Add(new DataColumn() { ColumnName = "Field2", Caption = "Поле 2" });
            tbl.Columns.Add(new DataColumn() { ColumnName = "Field3", Caption = "Поле 3" });

            tbl.Rows.Add(tbl.NewRow());
            tbl.Rows[0][0] = "Value1";
            tbl.Rows[0][1] = "Value2";
            tbl.Rows[0][2] = "Value3";


            var result = new TestClass();
            result.GetBindingAttributeValues(tbl.Rows[0]);

            Assert.AreEqual(result.F1, tbl.Rows[0][0]);
            Assert.AreEqual(result.F2, tbl.Rows[0][1]);
            Assert.AreNotEqual(result.F3, tbl.Rows[0][2]);
            Assert.AreEqual(result.F3, "Static3");
        }
       
        class TestClass : ISerializable
        {
            [XmlAttribute(AttributeName = "поле1"), Binding(FieldName = "Field1")]
            public string F1 { get; set; }
            [XmlAttribute(AttributeName = "поле2"), Binding(FieldName = "Field2")]
            public string F2 { get; set; }

            [XmlAttribute(AttributeName = "поле3"), Binding(StaticValue = "Static3")]
            public string F3 { get; set; }
        }
    }
}
