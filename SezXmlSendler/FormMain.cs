using System;
using System.Data;
using System.Windows.Forms;
using ERP_DAL;
using System.Collections.Generic;
using KernelUI;
using System.Text;
using System.Linq;
using SezXmlSendler.Extantions;
using SezXmlSendler.Model;
using System.Threading.Tasks;

namespace SezXmlSendler
{
    public partial class FormMain : Form
    {
        readonly List<Sendler> _taskList = new List<Sendler>();
        public FormMain()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (Environment.MachineName != "COMP")
                if (!FormConnection.OpenConnection())
                {
                    Close();
                    return;
                }

            SetConnectionsParams(true);

            var sen = new Sendler(typeof(MessageObject),"Выгрузка заказов", "mfrorders")
            {
                TimeRunning = new DateTime(2021, 1, 1, hour: 19, minute: 0, second: 0)
            };
            sen.OnLoadTable += LoadPlan;
            sen.OnSerializeObject += Sen_OnSerializeObject;
            sen.OnSended += Sen_OnSerializeObject;
            sen.OnError += Sen_OnError;            
            _taskList.Add(sen);

            sen = new Sendler("Выгрузка заводских номеров по двигателям", "productmark");
            //sen.OnLoadTable += DAL.RabbitSendlerData.GetZavNumbersList;
            
            _taskList.Add(sen);

            sen = new Sendler("Выгрузка готовой продукции", "products");

            //sen.OnLoadTable += new F0nsi37().LoadForImportToMoscow;
            _taskList.Add(sen);

            sen = new Sendler("Структура продукта в заказе", "mfrcontents")
            {
                TimeRunning = new DateTime(2021, 1, 1, hour: 19, minute: 0, second: 0)
            };
            sen.OnRun += RunMfrcontents;
            _taskList.Add(sen);


            numericUpDownIntervalRunning.Maximum = int.MaxValue;
            foreach (var item in _taskList)
            {
                checkedListBoxTasks.Items.Add(item);
                checkedListBoxTasks.SetItemChecked(checkedListBoxTasks.Items.Count - 1, true);
            }
        }

        private void Sen_OnError(object sender, System.IO.ErrorEventArgs e)
        {
            tbLog.Invoke((MethodInvoker)(()=>{ tbLog.Text += e.GetException().Message; }));
        }

        private void Sen_OnSerializeObject(object sender, string serialize)
        {
            tbLog.Invoke((MethodInvoker)(() => { tbLog.Text += serialize; }));
        }

        private DataTable LoadPlan(object sender)
        {
            return DAL.RabbitSendlerData.LoadOneTask();
        }

        private void SetConnectionsParams(bool fromSettings)
        {
            if (fromSettings)
            {
                tbLogin.Text = Properties.Settings.Default.User;
                tbPassword.Text = Properties.Settings.Default.Password;
                tbHost.Text = Properties.Settings.Default.HostName;
                numPort.Value = Properties.Settings.Default.Port;

                tbExchangeName.Text = Properties.Settings.Default.ExchangeName;
            }

            Sendler.User = tbLogin.Text;
            Sendler.Password = tbPassword.Text;
            Sendler.HostName = tbHost.Text;
            Sendler.Port = Convert.ToInt32(numPort.Value);
            Sendler.ExchangeName = tbExchangeName.Text;

        }
        private static void SaveConnectionParams()
        {
            Properties.Settings.Default.User = Sendler.User;
            Properties.Settings.Default.Password = Sendler.Password;
            Properties.Settings.Default.HostName = Sendler.HostName;
            Properties.Settings.Default.Port = Sendler.Port;
            Properties.Settings.Default.ExchangeName = Sendler.ExchangeName;
        }


        private void CheckedListBoxTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedListBoxTasks.SelectedItem is Sendler sen)
            {
                numericUpDownIntervalRunning.Value = 0;
                tbRoutingKey.Text = sen.RoutingKey;
            }

        }

        private void buttonRunSelectedTask_Click(object sender, EventArgs e)
        {
            if (!(checkedListBoxTasks.SelectedItem is Sendler sen)) return;
            if (MessageBox.Show($"Запустить {sen.Name} ?") != DialogResult.OK) return;
            sen.RunningAsync(checkBoxNeedSend.Checked).Wait();
        }

        private void buttonTestConnection_Click(object sender, EventArgs e)
        {
            SetConnectionsParams(false);
            var sendler = new Sendler("testconnection", tbRoutingKey.Text);
            try
            {
                Sendler.Send("проверка связи", sendler.RoutingKey);
                MessageBox.Show("сообщение отправлено");
                SaveConnectionParams();
            }
            catch (Exception err)
            {
                MessageBox.Show($"Host:{Sendler.HostName}     Port: {Sendler.Port}     User: {Sendler.User}     Password: {Sendler.Password}\n" +
                    $"{err.Message} {(err.InnerException != null ? err.InnerException.Message : string.Empty)}");

            }

        }

        private void buttonClearLog_Click(object sender, EventArgs e)
        {
            tbLog.Clear();
        }

       
        private void RunMfrcontents(object sender, string routingKey, bool needSending)
        {
            // var tblTasks = DAL.RabbitSendlerData.GetTasks();
            var i = 0;
            //foreach (DataRow task in tblTasks.Rows)
            {
                try
                {
                    //var tbl = DAL.RabbitSendlerData.GetProdInTask(task["ID_TASK"].ToString());
                    var tbl = DAL.RabbitSendlerData.GetProdInTask("64534");
                   
                    var sb = new StringBuilder();
                    MessagetProdinTask mess = null;

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

                            mess = new MessagetProdinTask(item);
                            
                        }
                        if (uzelId != item["ITEMID"].ToString()) // узел
                        {
                            uzelId = item["ITEMID"].ToString();
                            marshrutPoint = g.ToString();
                            idTp = g.ToString();
                            oper = g.ToString();
                            var node = new ImportedSostavIzdNode(item);
                          
                            if (node.LevelNumber == "0") node.ParentId = string.Empty;
                            mess?.Event.Product.SostavIzd.Nodes.Add(node);
                        }
                        
                        if (idTp != item["ID_TP"].ToString()) // маршрут
                        {
                            marshrutPoint = g.ToString();
                            oper = g.ToString();
                            idTp = item["ID_TP"].ToString();
                          
                            mess?.Event.Product.SostavIzd.Nodes.Last()
                                .TechMarshruts.Add(new ImportedSostavIzdTechMarshrut(item));
                        }
                        if (marshrutPoint != item["ORDER_DEP"].ToString())
                        {
                            marshrutPoint = item["ORDER_DEP"].ToString();
                            oper = g.ToString();

                            mess?.Event.Product.SostavIzd.Nodes.Last().TechMarshruts.Last()
                                .Marshrut.Add(new ImportedSostavIzdMarshrutPoint(item));
                        };
                        if (oper != item["ORDER_NO"].ToString())
                        {
                            oper = item["ORDER_NO"].ToString();
                            
                            mess?.Event.Product.SostavIzd.Nodes.Last().TechMarshruts.Last()
                                .Marshrut.Last()
                                .Operations.Add(new ImportedSostavIzdMarshrutOperation(item));
                        }
                    }
                    if (mess != null)
                    {
                        var str = Sendler.SerializeObject(mess.GetType(), mess);
                        sb.AppendLine(str);
                        tbLog.Text = sb.ToString();
                        if (needSending)
                        {
                            Sendler.Send(str, routingKey);
                            i += 1;
                            tbLog.Text += $"отправлено {i} пакетов";
                        }
                    }
                }
                catch(Exception err)
                {
                    tbLog.Text += $" Ошибка: {err.Message}";
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var time = DateTime.Now;
            if(checkedListBoxTasks.CheckedItems.Count>0)
                foreach (var item in checkedListBoxTasks.CheckedItems)
                {
                    var sen = (item as Sendler);
                    if (sen == null) return;
                    if (sen.TimeRunning.TimeOfDay == time.TimeOfDay)
                    {
                        sen.RunningAsync(checkBoxNeedSend.Checked);
                    }
                }
           
        }

     
    }
}
