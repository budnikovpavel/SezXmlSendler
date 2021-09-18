using System;
using System.Data;
using System.Windows.Forms;
using ERP_DAL;
using System.Collections.Generic;
using KernelUI;
using System.Threading.Tasks;
using SezXmlSendler.Model;
using SezXmlSendler.Model.Abstract;
using SezXmlSendler.Model.Interfaces;
using SezXmlSendler.Model.LZKDocumentObjects;
using SezXmlSendler.Model.OrdersObjects;
using SezXmlSendler.Model.OrderSpecificationsObjects;

namespace SezXmlSendler
{
    public partial class FormMain : Form
    {
        readonly List<ISendler> _taskList = new();
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

            dateTimeBegin.Value = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
            dateTimeEnd.Value = DateTime.Now;

            SetConnectionsParams(true);

            var sen = new Sendler<OrderMessageObject>("Выгрузка заказов", "mfrorders",
                LoadPlan,
                null, OnShowPercent)
            {
                TimeRunning = new DateTime(2021, 1, 1, hour: 19, minute: 0, second: 0)
            };

            sen.OnSerializeObject += Sen_OnSerializeObject;
            sen.OnSended += Sen_OnSerializeObject;
            sen.OnError += Sen_OnError;
            sen.OnLog += Sen_OnLog;
            _taskList.Add(sen);



            var sen2 = new Sendler<MessageProdInTask>("Структура продукта в заказе", "mfrcontents",
                LoadPlan,
                Sen2_OnFilledTable, OnShowPercent)
            {
                TimeRunning = new DateTime(2021, 1, 1, hour: 19, minute: 0, second: 0)
            };

            sen2.OnSerializeObject += Sen_OnSerializeObject;
            sen2.OnSended += Sen_OnSerializeObject;
            sen2.OnError += Sen_OnError;
            sen2.OnLog += Sen_OnLog;

            _taskList.Add(sen2);

            var sen3 = new Sendler<LzkMessageObject>("Выгрузка выдачи материалов по ЛЗК", "mfrcontents",
                (LoadLzkDocuments),
                (row, keyField) => DAL.RabbitSendlerData.GetLZKDocumentSpecification(row[keyField].ToString())
                , OnShowPercent)
            {
                TimeRunning = new DateTime(2021, 1, 1, hour: 19, minute: 0, second: 0)
            };

            sen3.OnSerializeObject += Sen_OnSerializeObject;
            sen3.OnSended += Sen_OnSerializeObject;
            sen3.OnError += Sen_OnError;
            sen3.OnLog += Sen_OnLog;

            _taskList.Add(sen3);

            foreach (var item in _taskList)
            {
                checkedListBoxTasks.Items.Add(item);
                checkedListBoxTasks.SetItemChecked(checkedListBoxTasks.Items.Count - 1, true);
            }
        }

        private void OnShowPercent(ISendler arg1, int arg2)=>
            checkedListBoxTasks.Refresh();


        private DataTable LoadLzkDocuments(out string keyField)=>
           DAL.RabbitSendlerData.GetLZKDocuments(new SEZ.DatePeriod(dateTimeBegin.Value, dateTimeEnd.Value), out keyField);

        private DataTable Sen2_OnFilledTable(DataRow row, string keyField) =>
            DAL.RabbitSendlerData.GetProdInTask(row[keyField].ToString());


        private void Sen_OnLog(object sender, string log) =>
            tbLog.Invoke((MethodInvoker)(() =>
                tbLog.Text += $@"{DateTime.Now} - отправитель {((ISendler)sender).RoutingKey}: {log}" + Environment.NewLine
            ));


        private void Sen_OnError(object sender, System.IO.ErrorEventArgs e) =>
            tbLog.Invoke((MethodInvoker)(() => tbLog.Text += e.GetException().Message + Environment.NewLine));


        private void Sen_OnSerializeObject(object sender, string serialize) =>
            tbDataInfo.Invoke((MethodInvoker)(() => tbDataInfo.Text = serialize + Environment.NewLine));


        private DataTable LoadPlan(out string keyField)
        {
            keyField = "ID_TASK";
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

            RabbitMQConnectionParameters.User = tbLogin.Text;
            RabbitMQConnectionParameters.Password = tbPassword.Text;
            RabbitMQConnectionParameters.HostName = tbHost.Text;
            RabbitMQConnectionParameters.Port = Convert.ToInt32(numPort.Value);
            RabbitMQConnectionParameters.ExchangeName = tbExchangeName.Text;

        }
        private static void SaveConnectionParams()
        {
            Properties.Settings.Default.User = RabbitMQConnectionParameters.User;
            Properties.Settings.Default.Password = RabbitMQConnectionParameters.Password;
            Properties.Settings.Default.HostName = RabbitMQConnectionParameters.HostName;
            Properties.Settings.Default.Port = RabbitMQConnectionParameters.Port;
            Properties.Settings.Default.ExchangeName = RabbitMQConnectionParameters.ExchangeName;
        }


        private void CheckedListBoxTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedListBoxTasks.SelectedItem is ISendler sen)
            {
                tbRoutingKey.Text = sen.RoutingKey;
            }

        }

        private void buttonRunSelectedTask_Click(object sender, EventArgs e)
        {
            if (!(checkedListBoxTasks.SelectedItem is ISendler sen)) return;
            if (sen.IsRunning)
            {
                MessageBox.Show("Задача уже запущена");
                return;
            }

            if (MessageBox.Show($"Запустить {sen.Name} ?") != DialogResult.OK) return;

            Task.Run(() => { sen.RunningAsync(checkBoxNeedSend.Checked).Wait(-1); });

        }

        private void buttonTestConnection_Click(object sender, EventArgs e)
        {
            SetConnectionsParams(false);
            var sendler = new Sendler<BaseMessageObject>("testconnection", tbRoutingKey.Text);
            try
            {
                sendler.Send("проверка связи", sendler.RoutingKey);
                MessageBox.Show("сообщение отправлено");
                SaveConnectionParams();
            }
            catch (Exception err)
            {
                MessageBox.Show(
                    $@"Host:{RabbitMQConnectionParameters.HostName}
Port: {RabbitMQConnectionParameters.Port}
User: {RabbitMQConnectionParameters.User}
Password: {RabbitMQConnectionParameters.Password}
{err.Message} {(err.InnerException != null ? err.InnerException.Message : string.Empty)}");

            }

        }


        private async void timer1_Tick(object sender, EventArgs e)
        {
            var time = DateTime.Now;
            if (checkedListBoxTasks.CheckedItems.Count > 0)
                foreach (var item in checkedListBoxTasks.CheckedItems)
                {
                    var sen = (item as ISendler);
                    if (sen == null) return;
                    if (sen.TimeRunning.TimeOfDay == time.TimeOfDay)
                    {
                        await sen.RunningAsync(checkBoxNeedSend.Checked);
                    }
                }

        }

        private void buttonClearInfo_Click(object sender, EventArgs e)
        {
            tbDataInfo.Clear();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            tbLog.Clear();
        }

        private void btnCancelTask_Click(object sender, EventArgs e)
        {
            if (checkedListBoxTasks.SelectedItem is ISendler sen)
            {
                if (MessageBox.Show($"Хотите отменить {sen.Name}? ", "Отменить задачу", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    sen.Cancel = true;
            }
        }
    }
}
