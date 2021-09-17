using System;
using System.Data;
using System.Windows.Forms;
using ERP_DAL;
using System.Collections.Generic;
using KernelUI;
using System.Threading.Tasks;
using SezXmlSendler.Model;
using SezXmlSendler.Model.Interfaces;

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
            var s = new Sendler<MessageProdInTask>("", "");
            var str = s.SerializeObject(new MessageProdInTask());

            if (Environment.MachineName != "COMP")
                if (!FormConnection.OpenConnection())
                {
                    Close();
                    return;
                }

            SetConnectionsParams(true);

            var sen = new Sendler<MessageObject>("Выгрузка заказов", "mfrorders")
            {
                TimeRunning = new DateTime(2021, 1, 1, hour: 19, minute: 0, second: 0)
            };
            sen.OnLoadTable += LoadPlan;
            sen.OnSerializeObject += Sen_OnSerializeObject;
            sen.OnSended += Sen_OnSerializeObject;
            sen.OnError += Sen_OnError;
            sen.OnLog += Sen_OnLog;
            _taskList.Add(sen);



            var sen2 = new Sendler<MessageProdInTask>("Структура продукта в заказе", "mfrcontents")
            {
                TimeRunning = new DateTime(2021, 1, 1, hour: 19, minute: 0, second: 0)
            };
            sen2.OnLoadTable += LoadPlan;
            sen2.OnSerializeObject += Sen_OnSerializeObject;
            sen2.OnSended += Sen_OnSerializeObject;
            sen2.OnError += Sen_OnError;
            sen2.OnLog += Sen_OnLog;
            sen2.OnFilledTable += Sen2_OnFilledTable; 
            _taskList.Add(sen2);


            numericUpDownIntervalRunning.Maximum = int.MaxValue;
            foreach (var item in _taskList)
            {
                checkedListBoxTasks.Items.Add(item);
                checkedListBoxTasks.SetItemChecked(checkedListBoxTasks.Items.Count - 1, true);
            }
        }

        private DataTable Sen2_OnFilledTable(object sender)
        {
            return DAL.RabbitSendlerData.GetProdInTask((sender as DataRow)?["ID_TASK"].ToString());
        }

        private void Sen_OnLog(object sender, string log)
        {
            tbLog.Invoke((MethodInvoker)(() =>
                tbLog.Text += $@"{DateTime.Now} - отправитель {((ISendler)sender).RoutingKey}: {log}" + Environment.NewLine
            ));
        }

        private void Sen_OnError(object sender, System.IO.ErrorEventArgs e) =>
            tbLog.Invoke((MethodInvoker)(() => tbLog.Text += e.GetException().Message + Environment.NewLine));


        private void Sen_OnSerializeObject(object sender, string serialize) =>
            tbDataInfo.Invoke((MethodInvoker)(() => tbDataInfo.Text = serialize + Environment.NewLine));


        private DataTable LoadPlan(object sender) =>
            DAL.RabbitSendlerData.LoadOneTask();

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
                numericUpDownIntervalRunning.Value = 0;
                tbRoutingKey.Text = sen.RoutingKey;
            }

        }

        private void buttonRunSelectedTask_Click(object sender, EventArgs e)
        {
            if (!(checkedListBoxTasks.SelectedItem is ISendler sen)) return;
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
    }
}
