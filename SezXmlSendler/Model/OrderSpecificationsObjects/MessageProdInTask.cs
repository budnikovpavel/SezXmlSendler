using System;
using System.Data;
using System.Linq;
using System.Xml.Serialization;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model.Abstract;
using SezXmlSendler.Model.Interfaces;

namespace SezXmlSendler.Model.OrderSpecificationsObjects
{
    [Serializable]
    [XmlType("Сообщение")]
    public class MessageProdInTask : BaseMessageObject, IFillOnTable
    {
        [XmlElement(ElementName = "Событие", IsNullable = true)]
        public EventObjectProdInTask Event { get; set; }

        public MessageProdInTask(DataRow sourceRow)
        {
            this.GetBindingAttributeValues(sourceRow);
            Event = new EventObjectProdInTask(sourceRow);
        }
        public MessageProdInTask() { }
        public void FillOnTable(DataTable tbl)
        {
            
            var g = new Guid();

            var idTask = g.ToString();
            var uzelId = g.ToString();
            var marshrutPoint = g.ToString();
            var idTp = g.ToString();
            var oper = g.ToString();
            
            foreach (DataRow item in tbl.Rows)
            {
                if (idTask != item["ID_TASK"].ToString())
                {
                    idTask = item["ID_TASK"].ToString();
                    this.GetBindingAttributeValues(item);
                    Event = new EventObjectProdInTask(item);
                }
                if (uzelId != item["ITEMID"].ToString()) // узел
                {
                    uzelId = item["ITEMID"].ToString();
                    marshrutPoint = g.ToString();
                    idTp = g.ToString();
                    oper = g.ToString();
                   
                    var node = new ImportedSostavIzdNode(item);
                    if (node.LevelNumber == "0") node.ParentId = string.Empty;
                    Event.Product.SostavIzd.Nodes.Add(node);
                }

                if (idTp != item["ID_TP"].ToString()) // маршрут
                {
                    marshrutPoint = g.ToString();
                    oper = g.ToString();
                    idTp = item["ID_TP"].ToString();
                    Event.Product.SostavIzd.Nodes.Last()
                        .TechMarshruts.Add(new ImportedSostavIzdTechMarshrut(item));
                }
                if (marshrutPoint != item["ORDER_DEP"].ToString())
                {
                    marshrutPoint = item["ORDER_DEP"].ToString();
                    oper = g.ToString();

                    Event.Product.SostavIzd.Nodes.Last().TechMarshruts.Last()
                        .Marshrut.Add(new ImportedSostavIzdMarshrutPoint(item));
                };
                if (oper != item["ORDER_NO"].ToString())
                {
                    oper = item["ORDER_NO"].ToString();

                    Event.Product.SostavIzd.Nodes.Last().TechMarshruts.Last()
                        .Marshrut.Last()
                        .Operations.Add(new ImportedSostavIzdMarshrutOperation(item));
                }
            }
        }
    }
}