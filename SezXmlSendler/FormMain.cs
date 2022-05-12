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
using SezXmlSendler.Model.XMLParser;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
                LoadLzkDocuments,
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

            var sen4 = new Sendler<LzkMessageObject>(jsonDe.SendlerParam[3].Name, jsonDe.SendlerParam[3].QueueName);

            sen4.OnReceive += Sen_OnReceive;
            sen4.OnError += Sen_OnError;
            sen4.OnLog += Sen_OnLog;

            _taskList.Add(sen4);

            foreach (var item in _taskList)
            {
                checkedListBoxTasks.Items.Add(item);
                checkedListBoxTasks.SetItemChecked(checkedListBoxTasks.Items.Count - 1, true);
            }
           // DAL.F0nsi09.Load();
            timer1.Enabled = true;
        }

        private void OnRecXML(object sender, EventArgs e)
        {
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<Сообщение Источник=""&quot;1С8.2#resurs_ts_b&quot;"" Агент=""Инфосервис_1C2RMQ"" ВерсияАгента=""1.0.0.0"">
	<Событие ТипСобытия=""Выгрузка"" ДатаВремяСобытия=""28.04.2022 14:56:39"" Пользователь=""СамсоновА"">
		<ОсновнойКонтекст ВидОбъекта=""Справочник"" ТипОбъекта=""ДоговорыКонтрагентов"" ИдентификаторОбъекта=""55b571a6-548a-11ea-8099-00155d70294f"">
			<Реквизиты ПометкаУдаления=""Нет"" ЭтоГруппа=""Да"" Владелец=""ТермоМарк"" ВладелецКод=""000003662"" ВладелецНаименование=""СИТИЛИНК ООО"" Родитель="""" ВладелецИНН=""7718979307"" РодительКод="""" РодительНаименование="""" Наименование=""2018"" Код=""БП-007881"" ВалютаВзаиморасчетов=""руб."" Комментарий="""" Организация="""" ВидДоговора="""" УчетАгентскогоНДС="""" ВидАгентскогоДоговора="""" Дата=""22.04.2020"" Номер=""J0016158"" СрокДействия="""" Валютный="""" ОплатаВВалюте="""" ГосударственныйКонтракт="""">
				<ДополнительныеРеквизиты КоличествоСтрок=""0""/>
			</Реквизиты>
		</ОсновнойКонтекст>
	</Событие>
</Сообщение>";
            var parser = new XMLParser();
            parser.Parse(xml);
            var dict = parser.ObjValues;
            SaveToDB(dict);
        }
        private void Sen_OnReceive(object sender, string receiveMessage)
        {

            if (!(sender is ISendler sen)) return;
            {
                var parser = new XMLParser();
                parser.Parse(receiveMessage);
                var dict = parser.ObjValues;
                SaveToDB(dict);
            }
        }

        private void SaveToDB(Dictionary<string, List<string>> dict)
        {
            try
            {
                if (dict.ContainsKey("Сообщение.Событие.ОсновнойКонтекст.Реквизиты.@ВладелецИНН"))
                {
                    var inn = dict["Сообщение.Событие.ОсновнойКонтекст.Реквизиты.@ВладелецИНН"].FirstOrDefault();
                    var nameFirm = dict["Сообщение.Событие.ОсновнойКонтекст.Реквизиты.@ВладелецНаименование"].FirstOrDefault();
                    var nn = -1;
                    var dbRows = DAL.F0nsi09.GetRowByInn(inn);

                    if (dbRows.Rows.Count > 0)
                    {
                        nn = Convert.ToInt32(dbRows.Rows[0]["NN"]);
                        dbRows.Rows[0]["NAME_FIRM"] = nameFirm;
                        Logged($"Обновление контрагента с ИНН = {inn}");
                    }
                    else
                    {
                        var newRow = DAL.F0nsi09.InitNewDataRow();
                        newRow.NAME_FIRM = nameFirm;
                        newRow.INN = inn;
                        DAL.F0nsi09.Table.AddF0NSI09Row(newRow);
                        nn = newRow.NN;
                        Logged($"Найден новый контрагент ИНН = {inn} ид строки {nn}");
                    }

                    var contract = DAL.FContract.InitNewDataRow();
                    contract.NAME_FIRM = nameFirm;
                    contract.NN_CUST = nn;
                    contract.NN_FIRM = nn;
                    contract.NAME_CURRENCY = dict["Сообщение.Событие.ОсновнойКонтекст.Реквизиты.@ВалютаВзаиморасчетов"].FirstOrDefault();
                    DateTime.TryParse(dict["Сообщение.Событие.ОсновнойКонтекст.Реквизиты.@Дата"].FirstOrDefault(), out var date);
                    contract.DATE_CONTRACT = date;
                    contract.DEPART = "6100";
                    contract.BIC = "1";
                    contract.NCONTRACT = string.IsNullOrEmpty(dict["Сообщение.Событие.ОсновнойКонтекст.Реквизиты.@Номер"].FirstOrDefault())
                        ?"пусто"
                        : dict["Сообщение.Событие.ОсновнойКонтекст.Реквизиты.@Номер"].FirstOrDefault();
                    Logged($"NC = {contract.NC} NAME_FIRM = {contract.NAME_FIRM} NN_CUST = {contract.NN_CUST} NAME_CURRENCY = {contract.NAME_CURRENCY} NCONTRACT = {contract.NCONTRACT}");
                    DAL.FContract.Table.AddFCONTRACTRow(contract);
                    DAL.F0nsi09.Update();
                    DAL.FContract.Update();
                    Logged($"{dict["Сообщение.Событие.ОсновнойКонтекст.Реквизиты.@ВладелецИНН"].FirstOrDefault()} - сохранен");
                }
            }
            catch (Exception ee) { Logged($"ОШИБКА - {ee.Message}"); }
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
            var logText = $@"{DateTime.Now} - отправитель {((ISendler)sender).RoutingKey?? ((ISendler)sender).QueueName}: {log}" + Environment.NewLine;
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
            if (dateTimeBegin.Value == null) dateTimeBegin.Value = DateTime.Now;
            if (dateTimeEnd.Value == null) dateTimeEnd.Value = DateTime.Now;
            return DAL.RabbitSendlerData.LoadOneTask(new SEZ.DatePeriod(dateTimeBegin.Value, dateTimeEnd.Value));
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
            
            var time =  DateTime.Now;
            //this.Text = $"Клиент Rabbit {Math.Round( time.TimeOfDay.TotalSeconds)}";
            if (Math.Round(time.TimeOfDay.TotalSeconds) == 0)
            {
                dateTimeBegin.Value = time.Date;
                dateTimeEnd.Value = time.Date.AddDays(1).AddSeconds(-1);
            }
            foreach (var item in checkedListBoxTasks.CheckedItems)
            {
                var sen = (item as ISendler);
                if (sen == null) return;
                
                if (sen.QueueName != string.Empty)
                {
                    tbLog.Text += $"{DateTime.Now} - запуск {sen.Name}";
                    await Task.Run(() => { sen.ReceiveAsync().Wait(-1); });
                }
                //continue;
                /*if(sen.QueueName == string.Empty && Math.Abs(Math.Round((time.TimeOfDay - sen.TimeRunning.TimeOfDay).TotalSeconds)) < 2)
                {
                    tbLog.Text += $"{DateTime.Now} - запуск {sen.Name}";
                    if (!sen.IsRunning)
                        await Task.Run(() => { sen.RunningAsync(checkBoxNeedSend.Checked).Wait(-1); });
                    
                }*/
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = "192.168.112.78",//RabbitMQConnectionParameters.HostName,
                    Port = 5672,//RabbitMQConnectionParameters.Port,
                    VirtualHost = "/",
                    UserName = "sez_ais",//RabbitMQConnectionParameters.User,
                    Password = "ais_sez_7"//RabbitMQConnectionParameters.Password
                };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        tbLog.Text += $"CreateModel" + Environment.NewLine;
                        channel.QueueDeclare(tbQueue.Text, true, false, false, null);
                        var consumer = new EventingBasicConsumer(channel);

                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            string message = Encoding.UTF8.GetString(body.ToArray());

                            tbLog.Text += $"получено сообщение: {message}";
                        };

                        channel.BasicConsume(tbQueue.Text, true, consumer);
                    }
                }
            }
            catch (Exception err) { tbLog.Text += $"ошибка: {err.Message}"; }

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
        }
    }
}
