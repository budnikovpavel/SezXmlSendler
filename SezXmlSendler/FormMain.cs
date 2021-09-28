using System;
using System.Data;
using System.Windows.Forms;
using ERP_DAL;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KernelUI;
using System.Threading.Tasks;
using SezXmlSendler.Model.Abstract;
using SezXmlSendler.Model.Interfaces;
using SezXmlSendler.Model.LZKDocumentObjects;
using SezXmlSendler.Model.OrdersObjects;
using SezXmlSendler.Model.OrderSpecificationsObjects;
using System.Text;
using Newtonsoft.Json;
using SezXmlSendler.Model;

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


            using var reader = new StreamReader(SendlersCongif.PathConfig, Encoding.UTF8);
            var text = reader.ReadToEnd();
            var jsonDe = JsonConvert.DeserializeObject<SendlersCongif>(text);


            if (Environment.MachineName != "COMP")
                if (!FormConnection.OpenConnection())
                {
                    Close();
                    return;
                }

            dateTimeBegin.Value = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
            dateTimeEnd.Value = DateTime.Now;

            SetConnectionsParams(true);

            var sen = new Sendler<OrderMessageObject>(jsonDe.SendlerParam[0].Name, jsonDe.SendlerParam[0].RoutingKey,
                LoadPlan,
                null, OnShowPercent)
            {
                TimeRunning = Convert.ToDateTime(jsonDe.SendlerParam[0].Time)
            };

            sen.OnSerializeObject += Sen_OnSerializeObject;
            sen.OnSended += Sen_OnLog;
            sen.OnError += Sen_OnError;
            sen.OnLog += Sen_OnLog;
            _taskList.Add(sen);



            var sen2 = new Sendler<MessageProdInTask>(jsonDe.SendlerParam[1].Name, jsonDe.SendlerParam[1].RoutingKey,
                LoadPlan,
                Sen2_OnFilledTable, OnShowPercent)
            {
                TimeRunning = Convert.ToDateTime(jsonDe.SendlerParam[1].Time)
            };

            sen2.OnSerializeObject += Sen_OnSerializeObject;
            sen2.OnSended += Sen_OnLog;
            sen2.OnError += Sen_OnError;
            sen2.OnLog += Sen_OnLog;

            _taskList.Add(sen2);

            var sen3 = new Sendler<LzkMessageObject>(jsonDe.SendlerParam[2].Name, jsonDe.SendlerParam[2].RoutingKey,
                (LoadLzkDocuments),
                (row, keyField) => DAL.RabbitSendlerData.GetLZKDocumentSpecification(row[keyField].ToString())
                , OnShowPercent)
            {
                TimeRunning = Convert.ToDateTime(jsonDe.SendlerParam[2].Time)
            };

            sen3.OnSerializeObject += Sen_OnSerializeObject;
            sen3.OnSended += Sen_OnLog;
            sen3.OnError += Sen_OnError;
            sen3.OnLog += Sen_OnLog;

            _taskList.Add(sen3);

            foreach (var item in _taskList)
            {
                checkedListBoxTasks.Items.Add(item);
                checkedListBoxTasks.SetItemChecked(checkedListBoxTasks.Items.Count - 1, true);
            }
        }

        private void OnShowPercent(ISendler arg1, int arg2) =>
            checkedListBoxTasks.Refresh();


        private DataTable LoadLzkDocuments(out string keyField) =>
           DAL.RabbitSendlerData.GetLZKDocuments(new SEZ.DatePeriod(dateTimeBegin.Value, dateTimeEnd.Value), out keyField);

        private DataTable Sen2_OnFilledTable(DataRow row, string keyField) =>
            DAL.RabbitSendlerData.GetProdInTask(row[keyField].ToString());

        private void WriteToLogFile(string logText)
        {
            // запись в файл
            using (FileStream fstream = new FileStream(Path.Combine(Environment.CurrentDirectory, "RabbitLog.log"), FileMode.Append))
            {
                // преобразуем строку в байты
                byte[] array = System.Text.Encoding.Default.GetBytes(logText);
                // запись массива байтов в файл
                fstream.Write(array, 0, array.Length);
            }
        }

        private void Logged(string log)
        {

            tbLog.Invoke((MethodInvoker)(delegate
            {
                var textLog = tbLog.Text + log + Environment.NewLine;
                if (textLog.Length < 10000)
                    tbLog.Text = textLog;
                else
                {
                    WriteToLogFile(textLog);
                    tbLog.Text = log;
                }
            }));
        }

        private void Sen_OnLog(object sender, string log)
        {
            var logText = $@"{DateTime.Now} - отправитель {((ISendler)sender).RoutingKey}: {log}" + Environment.NewLine;
            Logged(logText);
            if (!((ISendler)sender).IsRunning)
                WriteToLogFile(logText);
        }


        private void Sen_OnError(object sender, System.IO.ErrorEventArgs e) =>
            tbLog.Invoke((MethodInvoker)(() =>
            tbLog.Text += e.GetException().Message + Environment.NewLine));


        private void Sen_OnSerializeObject(object sender, string serialize)
        {
            tbDataInfo.Invoke((MethodInvoker)(() => tbDataInfo.Text = serialize + Environment.NewLine));
            if(checkBoxLogXML.Checked)
                WriteToLogFile(serialize + Environment.NewLine);
        }


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
                MessageBox.Show($"Задача {sen.Name} уже запущена");
                return;
            }

            if (MessageBox.Show($"Запустить {sen.Name} ?") != DialogResult.OK) return;

            Task.Run(() => { sen.RunningAsync(checkBoxNeedSend.Checked).Wait(-1); });

        }

        private void buttonTestConnection_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
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
            });
        }


        private async void timer1_Tick(object sender, EventArgs e)
        {
            var time = DateTime.Now;
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

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_taskList.Any(sen => sen.IsRunning))
                e.Cancel = MessageBox.Show("Есть незавершенные задачи, Вы действительно хотите закрыть приложение?",
                     "Внимание", MessageBoxButtons.YesNo) != DialogResult.Yes;
        }
    }
}
