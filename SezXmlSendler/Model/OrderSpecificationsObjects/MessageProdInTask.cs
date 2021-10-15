using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
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
            var way = g.ToString();
            var oper = g.ToString();
            int i = 0;
            foreach (DataRow item in tbl.Rows)
            {
                i += 1;
                var isDetect = false;
                if (idTask != item["ID_TASK"].ToString())
                {
                    idTask = item["ID_TASK"].ToString();
                    this.GetBindingAttributeValues(item);
                    Event = new EventObjectProdInTask(item);
                    isDetect = true;
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
                    isDetect = true;
                }
               
                if (idTp != item["ID_TP"].ToString()) // маршрут
                {
                    marshrutPoint = g.ToString();
                    oper = g.ToString();
                    idTp = item["ID_TP"].ToString();
                    Event.Product.SostavIzd.Nodes.Last()
                        .TechMarshruts.Add(new ImportedSostavIzdTechMarshrut(item));
                    isDetect = true;
                } 
                
                if (marshrutPoint != item["ORDER_DEP"].ToString())
                {
                    marshrutPoint = item["ORDER_DEP"].ToString();
                    oper = g.ToString();

                    Event.Product.SostavIzd.Nodes.Last().TechMarshruts.Last()
                        .Marshrut.Add(new ImportedSostavIzdMarshrutPoint(item));
                    isDetect = true;
                };
                if (oper != item["ORDER_NO"].ToString())
                {
                    oper = item["ORDER_NO"].ToString();

                    Event.Product.SostavIzd.Nodes.Last().TechMarshruts.Last()
                        .Marshrut.Last()
                        .Operations.Add(new ImportedSostavIzdMarshrutOperation(item));
                    isDetect = true;
                }
                if (!isDetect) { // частные случаи 
                    if (way != item["WAY"].ToString()) // добавляем узел с проверкой по way 
                    {
                        isDetect = true;
                        
                        var node = new ImportedSostavIzdNode(item);
                        if (node.LevelNumber == "0") node.ParentId = string.Empty;
                        Event.Product.SostavIzd.Nodes.Add(node);
                        Event.Product.SostavIzd.Nodes.Last()
                            .TechMarshruts.Add(new ImportedSostavIzdTechMarshrut(item));
                        Event.Product.SostavIzd.Nodes.Last().TechMarshruts.Last()
                           .Marshrut.Add(new ImportedSostavIzdMarshrutPoint(item));
                        Event.Product.SostavIzd.Nodes.Last()
                           .TechMarshruts.Last()
                           .Marshrut.Last()
                           .Operations.Add(new ImportedSostavIzdMarshrutOperation(item));
                    } else {
                        //строка является дублем , поэтому суммируем брутто и нетто
                        var brutto = Convert.ToDecimal(Event.Product.SostavIzd.Nodes.Last()?.QBrutto??"0");
                        var netto = Convert.ToDecimal(Event.Product.SostavIzd.Nodes.Last()?.QNetto??"0");

                        brutto += Convert.ToDecimal(item["QBRUTTO"].ToString());
                        netto += Convert.ToDecimal(item["QNETTO"].ToString());

                        Event.Product.SostavIzd.Nodes.Last().QBrutto = brutto.ToString();
                        Event.Product.SostavIzd.Nodes.Last().QNetto = netto.ToString();

                    }
                }
                way = item["WAY"].ToString();
            }
        }
    }
}