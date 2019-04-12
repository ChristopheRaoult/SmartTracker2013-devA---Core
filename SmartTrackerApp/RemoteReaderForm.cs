using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using DataClass;
using DBClass;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class RemoteReaderForm : Form
    {
        DeviceInfo[] _dev;
        DeviceInfo[] _networkDevice;
        string _ip;
        int _port = 0;
        readonly string _serverIp;
        readonly int _serverPort;

        DataTable _dt;
       
        MainDBClass _db;
       

        public RemoteReaderForm(TcpIP_class.TcpIpServer tcpIpServer)
        {
            _serverIp = tcpIpServer.ServerIP;
            _serverPort = tcpIpServer.Port;
            InitializeComponent();    
        }

        private void RemoteReaderForm_Load(object sender, EventArgs e)
        {       

            _dt = new DataTable();
            _dt.Columns.Add(ResStrings.str_Serial_RFID, typeof(string));
            _dt.Columns.Add(ResStrings.str_Device_Name, typeof(string));
            _dt.Columns.Add(ResStrings.str_Device_Type, typeof(string));
            _dt.Columns.Add(ResStrings.str_Server_IP, typeof(string));
            _dt.Columns.Add(ResStrings.str_Server_Port, typeof(string));

            UpdateGrid();
            
        }

        private void UpdateGrid()
        {
            _dt.Rows.Clear();
            
            _db = new MainDBClass();
           
            _db.OpenDB();
            DeviceInfo[] tmpDevice = _db.RecoverDevice(false);
            _db.CloseDB();
            if (tmpDevice == null)
            {
                dataGridViewRemote.DataSource = null;
                dataGridViewRemote.Refresh();
                return;
            }

            _networkDevice = new DeviceInfo[tmpDevice.Length];
            tmpDevice.CopyTo(_networkDevice, 0);
            foreach (DeviceInfo di in _networkDevice)
            {
                _dt.Rows.Add(di.SerialRFID,di.DeviceName,di.deviceType,di.IP_Server,di.Port_Server);
            }
            dataGridViewRemote.DataSource = null;
            dataGridViewRemote.DataSource = _dt.DefaultView;
            dataGridViewRemote.Refresh();

        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {

            Search();
        }

        private void Search()
        {
            toolStripStatusInfo.Text = ResStrings.str_Status;


            if (textBoxIPAddress.Text.Contains("."))
                _ip = textBoxIPAddress.Text;
            else
            {
                _ip = System.Net.Dns.GetHostEntry(textBoxIPAddress.Text).AddressList[0].ToString();
            }

            if (TcpIP_class.tcpUtils.CheckIp(_ip))
            {

                _port = 0;
                int.TryParse(textBoxPort.Text, out _port);

                if (TcpIP_class.tcpUtils.PingAddress(_ip, 1000))
                {
                    TcpIP_class.TcpIpClient tcp = new TcpIP_class.TcpIpClient();
                    if (tcp.pingServer(_ip, _port) == TcpIP_class.TcpIpClient.RetCode.RC_Succeed)
                    {
                        toolStripStatusInfo.Text = ResStrings.str_Ping_Server_OK;

                        tcp.getDevice(_ip, _port, out _dev);
                        UpdateListReader();

                    }
                    else
                        toolStripStatusInfo.Text = string.Format(ResStrings.str_Unable_to_connect_to_server, _ip, _port);
                }
                else
                   toolStripStatusInfo.Text = string.Format(ResStrings.str_Unable_to_connect_to_machine, _ip);
            }
            else
                toolStripStatusInfo.Text = string.Format(ResStrings.str_Enter_a_valid_IP_or_DNS_Not_Resolve_adress, textBoxIPAddress.Text);
        }

        private void UpdateListReader()
        {
            listBoxReader.Items.Clear();

            for (int loop = 0; loop < _dev.Length ; loop++)
            {
                string str = string.Format("{0} : {1} ({2})", _dev[loop].SerialRFID, _dev[loop].DeviceName, _dev[loop].deviceType);
                listBoxReader.Items.Add(str);
            }
        }

        private void listBoxReader_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxReader.SelectedIndex >= 0)
            {
                toolStripButtonACreateNew.Enabled = true;
            }
            else
            {
                toolStripButtonACreateNew.Enabled = false;
            }
        }

        private void toolStripButtonACreateNew_Click(object sender, EventArgs e)
        {
            toolStripStatusInfo.Text = ResStrings.str_Status;

            if (textBoxIPAddress.Text.Contains("."))
                _ip = textBoxIPAddress.Text;
            else
            {
                _ip = System.Net.Dns.GetHostEntry(textBoxIPAddress.Text).AddressList[0].ToString();
            }

            if (TcpIP_class.tcpUtils.CheckIp(_ip))
            {

                _port = 0;
                int.TryParse(textBoxPort.Text, out _port);

                if (TcpIP_class.tcpUtils.PingAddress(_ip, 1000))
                {
                    TcpIP_class.TcpIpClient tcp = new TcpIP_class.TcpIpClient();
                    if (tcp.pingServer(_ip, _port) == TcpIP_class.TcpIpClient.RetCode.RC_Succeed)
                    {
                        _dev[listBoxReader.SelectedIndex].IP_Server = _serverIp;
                        _dev[listBoxReader.SelectedIndex].Port_Server = _serverPort;

                        //if (tcp.enableRemoteAccess(IP, port, dev[listBoxReader.SelectedIndex]) == TcpIP_class.TcpIpClient.RetCode.RC_Succeed)
                        //{

                        MainDBClass db = new MainDBClass();

                        if (db.OpenDB())
                        {
                            _dev[listBoxReader.SelectedIndex].IP_Server = _ip;
                            _dev[listBoxReader.SelectedIndex].Port_Server = _port;
                            if (db.StoreDevice(_dev[listBoxReader.SelectedIndex], true))
                            {

                                MessageBox.Show(ResStrings.str_DATA_SAVED, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                                MessageBox.Show(ResStrings.str_Error_while_Saving_Data, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            db.CloseDB();
                        }
                      // }
                        //else
                        //{
                        //    toolStripStatusInfo.Text = "Status : Error while set Remote On server";
                        //}                   

                    }
                    else
                        toolStripStatusInfo.Text = string.Format(ResStrings.Rstr_Unable_to_connect_to_server__0__on_port__1, _ip, _port);
                }
                else
                    toolStripStatusInfo.Text = string.Format(ResStrings.str_Unable_to_connect_to_machine__0, _ip);
            }           
             else
                toolStripStatusInfo.Text = string.Format(ResStrings.str_Status___Enter_a_valid_IP_or_DNS_Not_Resolve_adress__0_, textBoxIPAddress.Text);

            UpdateGrid();
        }

        private void dataGridViewRemote_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewRemote.SelectedRows.Count > 0)
                toolStripButtonDelete.Enabled = true;
            else
                toolStripButtonDelete.Enabled = false;
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewRemote.SelectedRows.Count > 0)
            {
               // string ip = _networkDevice[dataGridViewRemote.SelectedRows[0].Index].IP_Server;
               // int _port = _networkDevice[dataGridViewRemote.SelectedRows[0].Index].Port_Server;               
                            
                MainDBClass db = new MainDBClass();                            
                if (db.OpenDB())
                {
                    db.DeleteDevice(_networkDevice[dataGridViewRemote.SelectedRows[0].Index]);
                    db.CloseDB();     
                }  
            }

            UpdateGrid();    
        }

        private void button1_Click(object sender, EventArgs e)
        {
           toolStripStatusInfo.Text = ResStrings.str_search_Online_reader___Please_wait_;        
           button1.Enabled = false;
           listBoxPC.Items.Clear();
           toolStripProgressBar1.MarqueeAnimationSpeed = 100;
           backgroundWorker1.RunWorkerAsync();         
              
         
        }

        private void listBoxPC_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string ipSelected = (string)listBoxPC.SelectedItem;

            textBoxIPAddress.Text = ipSelected;
            textBoxPort.Text = textBoxportsearch.Text;
            Search();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            int portsearch;
            int.TryParse(textBoxportsearch.Text, out portsearch);
            ArrayList tmpPc = TcpIP_class.tcpUtils.findOnlineMachine(portsearch);
            foreach (string ip in tmpPc)
            {
                string ip1 = ip;
                Invoke((MethodInvoker)delegate { listBoxPC.Items.Add(ip1); });
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            toolStripProgressBar1.MarqueeAnimationSpeed = 0;
            toolStripStatusInfo.Text = ResStrings.str_Status;
            button1.Enabled = true;
        }

        private void toolStripButtonQuit_Click(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate { Close(); });
        }

        private void textBoxIPAddress_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Return)
            {
                Search();
            }
        }

    }
}
