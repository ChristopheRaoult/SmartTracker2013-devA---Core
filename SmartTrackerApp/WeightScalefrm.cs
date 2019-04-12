using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;
using System.Management;
using System.Xml;
using System.IO;
using System.Reflection;


using DBClass;
using DataClass;
using ErrorMessage;
using SDK_SC_RfidReader;
using SDK_SC_RFID_Devices;
using BrightIdeasSoftware;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class WeightScalefrm : Form
    {
        
        MainDBClass _db;
        dtColumnInfo[] _columnInfoFull = null;
        public SartoriusScale scale = null;

        DataTable _tbHistory;

        private RFID_Device _device = null;
        rfidPluggedInfo[] _arrayOfPluggedDevice = null;
        bool _bProcess = false;
        int _nIndexColWeight = -1;
        int _selectedDevice = 0;
        string _columnWeightName = string.Empty;
        
        private bool _bAutomatic = false;
        private bool _bRunning = false;
        private string _lastTag = string.Empty;

        public WeightScalefrm()
        {
            InitializeComponent();
            CreateChart();
            //DoChartDemo();
        }

     

        //Function to discover device
        private void FindDevice()
        {
            _arrayOfPluggedDevice = null;
            RFID_Device tmp = new RFID_Device();
            _arrayOfPluggedDevice = tmp.getRFIDpluggedDevice(true);
            tmp.ReleaseDevice();
            comboBoxDevice.Items.Clear();
            if (_arrayOfPluggedDevice != null)
            {
                foreach (rfidPluggedInfo dev in _arrayOfPluggedDevice)
                {
                    
                        DeviceInfo di = _db.RecoverDevice(dev.SerialRFID);
                        if (di != null)
                            comboBoxDevice.Items.Add(string.Format("{0} ({1}:{2})", di.DeviceName, dev.SerialRFID,dev.portCom));
                        else
                            comboBoxDevice.Items.Add(dev.SerialRFID);                                   
                }
                if (comboBoxDevice.Items.Count > 0)
                {
                    comboBoxDevice.SelectedIndex = 0;
                    buttonCreate.Enabled = true;
                }
                else
                    buttonCreate.Enabled = false;
            }
            else
            {
                Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.WeightScalefrm_FindDevice_Info___No_Device_Detected___Quit_Live_Data; });
                buttonCreate.Enabled = false;
            }
        }

        private void WeightScalefrm_Load(object sender, EventArgs e)
        {
            _nIndexColWeight = -1;
            _db = new MainDBClass();
            _db.OpenDB();
            _columnInfoFull = _db.GetdtColumnInfo();             
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (!_bAutomatic)
            {
                timerscan.Interval = 100;
                timerscan.Enabled = false;
                timerscan.Stop();           
                buttonScan.Enabled = false;
                StartScan();
                chkbContinu.Enabled = true;
            }
            else
            {
                if (_bRunning)
                {
                    timerscan.Interval = 100;
                    timerscan.Enabled = false;
                    timerscan.Stop();
                    _bRunning = false;
                    buttonScan.Text = ResStrings.str_Start;
                    chkbContinu.Enabled = true;
                }
                else
                {
                    timerscan.Interval = 100;
                    timerscan.Enabled = true;
                    timerscan.Start();
                    _bRunning = true;
                    buttonScan.Text = ResStrings.str_Stop;
                    chkbContinu.Enabled = false;
                }
                           
            }
        }

        private void StartScan()
        {
            for (int i = 0; i < _columnInfoFull.Length; i++)
            {
                if (_columnInfoFull[i].colIndex == 0) labelTagID.Text = string.Format("{0} : ", _columnInfoFull[i].colName);
                else if (_columnInfoFull[i].colIndex == 1) labelLotID.Text = string.Format("{0} : ", _columnInfoFull[i].colName);
                else if (_columnInfoFull[i].colName.ToUpper().Contains("WEIGHT"))
                {
                    labelOldWeight.Text = string.Format(ResStrings.WeightScalefrm_StartScan_Previous, _columnInfoFull[i].colName);
                    break;
                }
            }
            labelWeight.Text = string.Empty;
            _bProcess = false;
            ScanDevice();
            Application.DoEvents();
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            // release previous object if not connected
            if (_device != null)
            {
                if (_device.ConnectionStatus != ConnectionStatus.CS_Connected)
                    _device.ReleaseDevice();
            }
            //Create a new object 
            _device = new RFID_Device();
            toolStripStatusLabelInfo.Text = ResStrings.str_In_Connection;
            buttonCreate.Enabled = false;
            //subscribe the event 
            _device.NotifyRFIDEvent += new NotifyHandlerRFIDDelegate(rfidDev_NotifyRFIDEvent);
            //Create a smartboard device
            //As the function search on all the serial port of the PC, this connection can
            //take some time and is under a thread pool to avoid freeze of the GUI
            ThreadPool.QueueUserWorkItem(
                delegate
                {             
                  //  Use create with portcom parameter for speed connection (doesn't search again the reader at is is previouly done;
                  _device.Create_NoFP_Device(_arrayOfPluggedDevice[_selectedDevice].SerialRFID, _arrayOfPluggedDevice[_selectedDevice].portCom);
                });          
        }

        private void buttonDispose_Click(object sender, EventArgs e)
        {
            if ((_device != null) && (_device.ConnectionStatus == ConnectionStatus.CS_Connected))
                _device.ReleaseDevice();
            if (scale != null) scale.ClosePort();
            buttonCreate.Enabled = true;
        }

        // Function to get rfid event
        private void rfidDev_NotifyRFIDEvent(object sender, rfidReaderArgs args)
        {
            switch (args.RN_Value)
            {
                // Event when failed to connect          
                case rfidReaderArgs.ReaderNotify.RN_FailedToConnect:
                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Failed_to_Connect; });
                    Invoke((MethodInvoker)delegate { buttonCreate.Enabled = true; });
                    Invoke((MethodInvoker)delegate { buttonDispose.Enabled = false; });

                    break;
                // Event when release the object
                case rfidReaderArgs.ReaderNotify.RN_Disconnected:
                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Device_Disconnected; });
                    Invoke((MethodInvoker)delegate { buttonCreate.Enabled = true; });
                    Invoke((MethodInvoker)delegate { buttonDispose.Enabled = false; });
                    Invoke((MethodInvoker)delegate { buttonScan.Enabled = false; });

                    break;

                //Event when device is connected
                case rfidReaderArgs.ReaderNotify.RN_Connected:
                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Device_Connected___Search_Scale; });
                    Invoke((MethodInvoker)delegate { buttonCreate.Enabled = false; });
                    Invoke((MethodInvoker)delegate { buttonDispose.Enabled = true; });
                    Invoke((MethodInvoker)delegate { buttonScan.Enabled = true; });
                    Invoke((MethodInvoker)delegate { buttonFindScale.Enabled = true; });
                    Invoke((MethodInvoker)delegate { FindAndConnectScale(args.SerialNumber,_device.get_RFID_Device.StrCom); });

                    break;

                // Event when scan started
                case rfidReaderArgs.ReaderNotify.RN_ScanStarted:


                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Scan_Started; });
                     SetLabelText(labelTagID, _columnInfoFull[0].colName + " : ");
                     SetLabelText(labelLotID, _columnInfoFull[1].colName + " : ");                
                     SetLabelText(labelOldWeight, string.Format("{0}{1} : ", ResStrings.str_Previous, _columnInfoFull[_nIndexColWeight].colName));               

                    break;

                //event when fail to start scan
                case rfidReaderArgs.ReaderNotify.RN_ReaderFailToStartScan:
                    Invoke((MethodInvoker)delegate { buttonScan.Enabled = true; });
                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Failed_to_start_scan; });
                    break;

                //event when a new tag is identify
                case rfidReaderArgs.ReaderNotify.RN_TagAdded:

                    break;

                // Event when scan completed
                case rfidReaderArgs.ReaderNotify.RN_ScanCompleted:

                    Invoke((MethodInvoker)delegate { 
                        buttonScan.Enabled = true;
                        buttonUpdate.Enabled = true;
                    });
                    if (_device.currentInventory.nbTagAll == 0)
                    {
                       _lastTag = string.Empty;
                       Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_No_Tag_Found; });
                       if (_bAutomatic)
                       {
                           Invoke((MethodInvoker)delegate
                           {
                               timerscan.Interval = 1000;
                               timerscan.Enabled = true;
                               timerscan.Start();
                           });
                       }
                       else
                       {
                           Invoke((MethodInvoker)delegate
                           {
                               timerscan.Interval = 1000;
                               timerscan.Enabled = false;
                               timerscan.Stop();
                           });
                       }
                    }
                    else if (_device.currentInventory.nbTagAll > 1)
                    {
                        Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_More_than_one_tag; });
                        if (!_bAutomatic) MessageBox.Show(ResStrings.str_More_than_one_tag_scanned, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _lastTag = string.Empty;
                        if (_bAutomatic)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                timerscan.Interval = 1000;
                                timerscan.Enabled = true;
                                timerscan.Start();
                            });
                        }
                        else
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                timerscan.Interval = 1000;
                                timerscan.Enabled = false;
                                timerscan.Stop();
                            });
                        }
                    }
                    else
                    {                       
                        _bProcess = true;
                        Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str__Scan_Completed___Tag_Scanned___ + _device.currentInventory.listTagAll[0]; });                     
                        Invoke((MethodInvoker)delegate { ProcessTag(); });
                    }
                    break;

                //error when error during scan
                case rfidReaderArgs.ReaderNotify.RN_ReaderScanTimeout:
                case rfidReaderArgs.ReaderNotify.RN_ErrorDuringScan:
                    Invoke((MethodInvoker)delegate { buttonScan.Enabled = true; });
                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str__Scan_has_error; });
                    break;
                case rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost:
                    Invoke((MethodInvoker)delegate { buttonScan.Enabled = true; });
                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str__Scan_cancel_by_host; });
                    break;
            }
            Application.DoEvents();
        }
        private void ScanDevice()
        {
            if (_device == null) return;
            if ((_device.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                (_device.DeviceStatus == DeviceStatus.DS_Ready))
            {
                //Request a scan
                //Scan status will be notified by event
                _device.ScanDevice();
            }
            else
                MessageBox.Show(ResStrings.str_device_not_ready, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void WeightScalefrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((_device != null) && (_device.ConnectionStatus == ConnectionStatus.CS_Connected))
                _device.ReleaseDevice();
            if (scale != null) scale.ClosePort();
            _db.CloseDB();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Search_connected_device; });
            FindDevice();
        }
        private void ProcessTag()
        {
            /*if (!lastTag.Equals(device.currentInventory.listTagAll[0].ToString()))
            {
                lastTag = device.currentInventory.listTagAll[0].ToString();
                labelWeight.Text = "Wait Stability";
                Application.DoEvents();
                scale.getWeight(); 
            }
            else
            {*/
                if (_bAutomatic)
                {
                    if (!_lastTag.Equals(_device.currentInventory.listTagAll[0].ToString()))
                    {
                        _lastTag = _device.currentInventory.listTagAll[0].ToString();
                        labelWeight.Text = ResStrings.WeightScalefrm_ProcessTag_Wait_Stability;
                        Application.DoEvents();
                        scale.GetWeight();
                    }
                    else
                    Invoke((MethodInvoker)delegate
                    {
                        timerscan.Interval = 1000;
                        timerscan.Enabled = true;
                        timerscan.Start();
                    });
                }
                else
                {
                    _lastTag = _device.currentInventory.listTagAll[0].ToString();
                    labelWeight.Text = ResStrings.WeightScalefrm_ProcessTag_Wait_Stability;
                    Application.DoEvents();
                    scale.GetWeight();

                    Invoke((MethodInvoker)delegate
                    {
                        timerscan.Interval = 1000;
                        timerscan.Enabled = false;
                        timerscan.Stop();
                    });
                }
            //}           
        }

        static private bool GetComScale(string serialNumber,string comPortRfid, out string comScale)
        {
            comScale = null;
            string serialFtdi = null;
             Hashtable listPortScale = null;
            string pathXml = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "deviceInfo.xml";
            if (File.Exists(pathXml))
            {

              
                XmlDocument pDoc = new XmlDocument();
                pDoc.Load(pathXml);

                XmlElement root = pDoc.DocumentElement;

                if (root != null)
                {
                    XmlNode device = root.SelectSingleNode("Device[@serialNumber='" + serialNumber + "']");
                    if (device != null)
                    {
                        XmlNode scaleUsbSerial = device.SelectSingleNode("ScaleUsbSerial");

                        /* ManagementObjectSearcher MOSearcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSSerial_PortName");
                    foreach (ManagementObject MOject in MOSearcher.Get())
                    {
                        if (MOject["InstanceName"].ToString().Contains(ScaleUsbSerial.InnerText + "A"))
                        {
                            comScale = MOject["PortName"].ToString().ToUpper();
                            break;
                        }                       
                    }*/

                        ManagementObjectSearcher moSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");
                        foreach (var o in moSearcher.Get())
                        {
                            var mosObject = (ManagementObject) o;
                            string portName = string.Empty;
                            
                                if (mosObject["Name"] != null)
                                    portName = mosObject["Name"].ToString().ToUpper();
                            
                           
                            if (portName.Contains("(COM")) // msObject["Name"] looks like "Usb Serial Device (COM1)"
                            {
                                int startIndex = portName.IndexOf("(COM", StringComparison.OrdinalIgnoreCase) + 1;
                                int endIndex = portName.IndexOf(")", startIndex, StringComparison.Ordinal);

                                if (scaleUsbSerial != null && mosObject["DeviceID"].ToString().Contains(scaleUsbSerial.InnerText + "A"))
                                {
                                    // "DeviceID" contains e.g "FTU7PL54" (badge reader usb serial)
                                    comScale = portName.Substring(startIndex, endIndex - startIndex); // substring to only get e.g "COM1"
                                    break;
                                }
                            }
                        }
                    }
                    else //not in device file try to add IT
                    {
                   
                        listPortScale = new Hashtable();
                        string instancePort = null;
                        string portScale = null;
                        ManagementObjectSearcher moSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");
                        foreach (ManagementObject mOject in moSearcher.Get())
                        {
                            /*if (MOject["InstanceName"].ToString().Contains("FTDIBUS\\VID_0403+PID_6010"))
                        {                            
                            instancePort = MOject["InstanceName"].ToString().Substring(26, 8);
                            if (MOject["InstanceName"].ToString().Contains(instancePort+"A"))
                                portScale = MOject["PortName"].ToString().ToUpper();
                            if (MOject["InstanceName"].ToString().Contains(instancePort + "B"))
                            {
                                if (MOject["PortName"].ToString().ToUpper() == comPortRFID)
                                {
                                    break;
                                }
                                else
                                    portScale = null;
                            }
                        }*/
                            // Search serial usd 2 port FTDI and update deviceinfo.xml
                   
                     
                       
                            string instanceName = mOject["DeviceID"].ToString().ToUpper();
                            if (mOject["DeviceID"].ToString().Contains("FTDIBUS\\VID_0403+PID_6010"))
                            {
                                string strDeviceID = "VID_0403+PID_6010+";                         
                           
                                int start = instanceName.IndexOf(strDeviceID, StringComparison.OrdinalIgnoreCase) + strDeviceID.Length;
                                instancePort = instanceName.Substring(start, 8);

                                string portName = string.Empty;
                                
                                    if (mOject["Name"] != null)
                                        portName = mOject["Name"].ToString().ToUpper();
                                
                                int startIndex = portName.IndexOf("(COM", StringComparison.OrdinalIgnoreCase) + 1;
                                int endIndex = portName.IndexOf(")", startIndex, StringComparison.Ordinal);
                                if (instanceName.Contains(instancePort + "A")) // "DeviceID" contains e.g "FTU7PL54" (badge reader usb serial)
                                    //comScale = portName.Substring(startIndex, endIndex - startIndex); // substring to only get e.g "COM1"
                                    listPortScale.Add(instancePort, portName.Substring(startIndex, endIndex - startIndex));

                                if (instanceName.Contains(instancePort + "B"))
                                {
                                    if (portName.Substring(startIndex, endIndex - startIndex).ToUpper() == comPortRfid)
                                    {
                                        serialFtdi = instancePort;                                   
                                    }   
                                }                          
                            }
                        }

                        if (serialFtdi != null)
                        {
                            if (listPortScale.Contains(serialFtdi))
                                portScale = listPortScale[instancePort].ToString() ;
                        }

                    
                        if (!string.IsNullOrEmpty(portScale))
                        {                       

                            XmlDocument pSaveDoc = new XmlDocument();
                            pSaveDoc.Load(pathXml);
                            XmlElement rootSave = pSaveDoc.DocumentElement;
                            XmlElement deviceSave = pSaveDoc.CreateElement("Device");
                            deviceSave.SetAttribute("serialNumber", serialNumber);
                            XmlElement serialName = pSaveDoc.CreateElement("SerialName");
                            serialName.InnerText = "To Set";
                            XmlElement deviceType = pSaveDoc.CreateElement("DeviceType");
                            deviceType.InnerText = "DT_STR";
                            XmlElement scaleUsbSerial = pSaveDoc.CreateElement("ScaleUsbSerial");
                            scaleUsbSerial.InnerText = instancePort;
                            deviceSave.AppendChild(serialName);
                            deviceSave.AppendChild(deviceType);
                            deviceSave.AppendChild(scaleUsbSerial);
                            if (rootSave != null) rootSave.AppendChild(deviceSave);
                            pSaveDoc.Save(pathXml);

                            comScale = portScale;
                        }
                        else
                        {
                            MessageBox.Show(ResStrings.WeightScalefrm_GetComScale, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(comScale))
                return false;
            return true;
        }

        private void FindAndConnectScale(string serialRfid, string comPortRfid)
        {
                      
            Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.WeightScalefrm_FindAndConnectScale_Info___Search_scale; });

           
            //ERROR WINDOWS 8 - No More A and B in name
            //Set com scale to serial rfid com + 1
            //Impose user to check that port com is succeeding value and RFID in first
            try
            {
                string comValue = comPortRfid.Substring(3);
                int portCom = int.Parse(comValue) + 1;
                string comScale = "COM" + portCom;

                List<string> ports = new List<string>(System.IO.Ports.SerialPort.GetPortNames());
             
                if (ports.Contains(comScale))
                {
                    MessageBox.Show("RFID on Port :" + comPortRfid + "\r\nSearch Scale on Port :" + comScale);
                    scale = new SartoriusScale(comScale);
                    scale.NotifyWeightEvent += new NotifyHandlerWeightScaleDelegate(scale_NotifyWeightEvent);
                    Thread.Sleep(100);
                    if (scale.IsConnected)
                    {
                        scale.GetWeight();
                        Thread.Sleep(3000);
                        if (scale.LastScaledWeight != null)
                        {
                            Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.WeightScalefrm_FindAndConnectScale_Info___Scale_found_on_port + comScale; });
                            Invoke((MethodInvoker)delegate { buttonScan.Enabled = true; });
                        }
                        else
                        {
                            Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.WeightScalefrm_FindAndConnectScale_Unable_to_connect_scale_on_port + comScale; });
                        }
                    }
                    else
                    {
                        Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.WeightScalefrm_FindAndConnectScale_Unable_to_connect_scale_on_port + comScale; });
                    }
                }
                else
                {
                    Invoke((MethodInvoker)delegate { buttonScan.Enabled = false;});
                    MessageBox.Show("Port " +  comScale + " not exists or already in used on this PC \r\nPlease check scale serial port Setting");
                    SetLabelText(labelWeight, ResStrings.WeightScalefrm_FindAndConnectScale_No_Scale_serialPort_Found);
                }            
            }
            catch (Exception exp)
            {
                
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            
        }
        private string UpdateWeight()
        {
            if (scale.LastScaledWeight != null)
            {
                string unit;
                if (!scale.LastScaledWeight.BValueStable)
                {
                    labelWeight.ForeColor = Color.Red;
                    unit = string.Empty;
                }
                else
                {
                    labelWeight.ForeColor = Color.Green;
                    unit = scale.LastScaledWeight.StrUnit.ToString();
                }

                if (scale.LastScaledWeight.WeightValue > 0)
                {
                    return string.Format(ResStrings.WeightScalefrm_UpdateWeight_Weight_____0___1_, scale.LastScaledWeight.WeightValue.ToString("0.000"), unit);
                }
                return string.Format(ResStrings.WeightScalefrm_UpdateWeight_Weight____0___1_, scale.LastScaledWeight.WeightValue.ToString("0.000"), unit);
            }

            return "";
        }

        private delegate void SetLabelTextDelegate(Label label, string data);
        private void SetLabelText(Label label, string data)
        {
            // label.Text = number.ToString();
            // Do NOT do this, as we are on a different thread.

            // Check if we need to call BeginInvoke.
            if (label.InvokeRequired)
            {
                // Pass the same function to BeginInvoke,
                // but the call would come on the correct
                // thread and InvokeRequired will be false.
                BeginInvoke(new SetLabelTextDelegate(SetLabelText),
                                                 new object[] { label, data });

                return;
            }

            label.Text = data;
        }

        private void scale_NotifyWeightEvent(Object sender, string deviceSerial, string dataReceived)
        {
            SetLabelText(labelWeight, UpdateWeight());
            Application.DoEvents();
            if (_bProcess)
            {
                if (_bAutomatic)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        updateWeight();
                        if (_bAutomatic)
                        {
                            timerscan.Interval = 5000;
                            timerscan.Enabled = true;
                            timerscan.Start();
                        }
                        else
                        {
                            timerscan.Interval = 5000;
                            timerscan.Enabled = false;
                            timerscan.Stop();
                        }
                    });
                }
                else
                    Invoke((MethodInvoker)delegate { updateWeight(); });
            }
            Application.DoEvents();
        }

        private void RefreshInfo()
        {
            string[] productInfo = _db.RecoverProductInfo(_device.currentInventory.listTagAll[0].ToString());

            if (productInfo != null)
            {
                SetLabelText(labelTagID, _columnInfoFull[0].colName + " : " + _device.currentInventory.listTagAll[0]);
                SetLabelText(labelLotID, _columnInfoFull[1].colName + " : " + productInfo[1]);
                if (productInfo[_nIndexColWeight].Equals(" "))
                    SetLabelText(labelOldWeight, string.Format(ResStrings.WeightScalefrm_refreshInfo_Previous__0____0_0_Cts, _columnInfoFull[_nIndexColWeight].colName));
                else
                    SetLabelText(labelOldWeight, string.Format(ResStrings.WeightScalefrm_refreshInfo_Previous__0_____1__Cts, _columnInfoFull[_nIndexColWeight].colName, productInfo[_nIndexColWeight]));
            }
            else
            {
                SetLabelText(labelTagID, _columnInfoFull[0].colName + " : " + _device.currentInventory.listTagAll[0]);
                SetLabelText(labelLotID, _columnInfoFull[1].colName + " : " + ResStrings.str_Unreferenced);
                SetLabelText(labelOldWeight, string.Format(ResStrings.WeightScalefrm_refreshInfo_Previous__0____0_0_Cts, _columnInfoFull[_nIndexColWeight].colName));
            }

            UpdateHistoryTable(_device.currentInventory.listTagAll[0].ToString());
            DoChart();
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

           /* if (!DataClass.deviceUtils.IsUserAdministrator())
            {
                MessageBox.Show("Weight Scale feature require Administrator Right \r\n Run smartTracker as Administrator", Properties.ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                return;
            }*/

            _columnWeightName = _db.GetColumnNameForWeight();
            if (string.IsNullOrEmpty(_columnWeightName))
            {
                MessageBox.Show(ResStrings.WeightScalefrm_noColumn, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                return;
            }

            for (int i = 0; i < _columnInfoFull.Length; i++)
            {
                if (_columnInfoFull[i].colIndex == 0) labelTagID.Text = string.Format("{0} : ", _columnInfoFull[i].colName);
                else if (_columnInfoFull[i].colIndex == 1) labelLotID.Text = string.Format("{0} : ", _columnInfoFull[i].colName);
                else if (_columnInfoFull[i].colName.Equals(_columnWeightName))
                {
                    labelOldWeight.Text = string.Format(ResStrings.WeightScalefrm_timer1_Tick_Previous__0____, _columnInfoFull[i].colName); 
                    _nIndexColWeight = _columnInfoFull[i].colIndex;
                    break;
                }
            }
            InitHistoryTable();
            Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.WeightScalefrm_timer1_Tick_Info___Search_connected_device; });
            FindDevice();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            buttonUpdate.Enabled = false;
            updateWeight();
        }

        private void updateWeight()
        {
           
            ProductClassTemplate pc = new ProductClassTemplate();

            string[] productInfo = _db.RecoverProductInfo(_device.currentInventory.listTagAll[0].ToString());

            if (productInfo != null)
            {
                pc.tagUID = _device.currentInventory.listTagAll[0].ToString();
                pc.reference = productInfo[1];
                for (int i = 0; i < productInfo.Length - 2; i++)
                {
                    if (i == (_nIndexColWeight - 2))
                    {
                        if (scale.LastScaledWeight != null)
                        {
                            pc.productInfo[i] = scale.LastScaledWeight.WeightValue.ToString(CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            pc.productInfo[i] = " ";
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(productInfo[i + 2]))
                            pc.productInfo[i] = productInfo[i + 2];
                        else
                            pc.productInfo[i] = " ";
                    }
                }

                if (_db.StoreProduct(pc))
                    RefreshInfo();
                else
                    MessageBox.Show(ResStrings.WeightScalefrm_updateWeight_error, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                MessageBox.Show(ResStrings.WeightScalefrm_updateWeight_product, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }            
        }

        private void InitHistoryTable()
        {
            _tbHistory = new DataTable();
            _tbHistory.Columns.Add(ResStrings.str_Event_Date, typeof(string));
            _tbHistory.Columns.Add(_columnInfoFull[0].colName, typeof(string));
            _tbHistory.Columns.Add(_columnInfoFull[1].colName, typeof(string));
            _tbHistory.Columns.Add(_columnInfoFull[_nIndexColWeight].colName, typeof(string));

            Invoke((MethodInvoker)delegate
            {
                //dataGridViewHistory.DataSource = null;
                //dataGridViewHistory.DataSource = tbHistory.DefaultView;

                _tbHistory.DefaultView.RowFilter = null;
                dataListViewHistory.DataSource = null;
                dataListViewHistory.DataSource = _tbHistory.DefaultView;

                for (int i = 0; i < dataListViewHistory.Columns.Count; i++)
                {
                    OLVColumn ol = dataListViewHistory.GetColumn(i);
                    //ol.FillsFreeSpace = true;
                    if (i == 0)
                        ol.Text = ResStrings.str_Event_Date;
                    else if (i == 3)
                        ol.Text = _columnInfoFull[_nIndexColWeight].colName;
                    else
                        ol.Text = _columnInfoFull[i-1].colName;

                    ol.HeaderFont = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
                    ol.HeaderForeColor = Color.AliceBlue;
                    ol.IsTileViewColumn = true;
                    ol.UseInitialLetterForGroup = false;

                    ol.MinimumWidth = 20 + ol.Text.Length * 10;
                    ol.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                    if (ol.Index == dataListViewHistory.Columns.Count - 1)
                        ol.FillsFreeSpace = true;

                }      
            });

        }

        private void UpdateHistoryTable(string tagUid)
        {
            try
            {
                ArrayList prodHistory = _db.RecoverProductHistory(tagUid);

                InitHistoryTable();
                if (prodHistory != null)
                {
                  
                    foreach (string[] ph in prodHistory)
                    {
                        DataRow rowToadd = _tbHistory.NewRow();

                        rowToadd[0] = ph[0];
                        rowToadd[1] = ph[2];
                        rowToadd[2] = ph[3];
                        rowToadd[3] = ph[_nIndexColWeight + 2];
                        _tbHistory.Rows.Add(rowToadd);
                       
                    }
                }
                
                Invoke((MethodInvoker)delegate
                {
                    //dataGridViewHistory.DataSource = null;
                    //dataGridViewHistory.DataSource = tbHistory.DefaultView;

                    _tbHistory.DefaultView.RowFilter = null;
                    dataListViewHistory.DataSource = null;
                    dataListViewHistory.DataSource = _tbHistory.DefaultView;

                    for (int i = 0; i < dataListViewHistory.Columns.Count; i++)
                    {
                        OLVColumn ol = dataListViewHistory.GetColumn(i);
                        //ol.FillsFreeSpace = true;
                        ol.HeaderFont = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
                        ol.HeaderForeColor = Color.AliceBlue;
                        ol.IsTileViewColumn = true;
                        ol.UseInitialLetterForGroup = false;
                        ol.MinimumWidth = 20 + ol.Text.Length * 10;
                        ol.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                        if (ol.Index == dataListViewHistory.Columns.Count - 1)
                            ol.FillsFreeSpace = true;

                    }      
                });
            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }
        }
       
        private void chkbContinu_Click(object sender, EventArgs e)
        {
            if (chkbContinu.Checked)
            {
                _bAutomatic = true;
                buttonScan.Text = ResStrings.str_Start;
                buttonUpdate.Visible = false;
            }
            else
            {
                _bAutomatic = false;
                buttonScan.Text = ResStrings.str_Scan;
                buttonUpdate.Visible = true;
            }
        }

        private void timerscan_Tick(object sender, EventArgs e)
        {
            timerscan.Stop();
            timer1.Enabled = false;
            StartScan();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            labelWeight.Text = ResStrings.str_Search_Scale;
            Application.DoEvents();
            if (scale != null)
            {
                scale.ClosePort();
                scale = null;
            }
            if (_device != null)
                  FindAndConnectScale(_device.SerialNumberRFID,_device.get_RFID_Device.StrCom);
        }

        // Bug VS 2012 chart 3.5 - needto create out designer
        ChartArea chartArea1 = new ChartArea();
        Legend legend1 = new Legend();
        private Chart _chart;
        private void CreateChart()
        {
            _chart = new Chart();
            _chart.Anchor = (AnchorStyles.Top | AnchorStyles.Left)  | AnchorStyles.Right;
            chartArea1.Name = "ChartArea1";
            _chart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            _chart.Legends.Add(legend1);
            _chart.Location = new Point(1109, 114);
            _chart.Name = "chartTemp";
            _chart.Size = new Size(454, 209);
            _chart.TabIndex = 40;
            _chart.Text = "chart1";
            _chart.CustomizeLegend += new EventHandler<CustomizeLegendEventArgs>(chartTemp_CustomizeLegend);

            _chart.Dock = DockStyle.Fill;
            panelChart.BackColor = Color.White;
            _chart.BackColor = Color.White;
            panelChart.Controls.Add(_chart);
        }
        private void chartTemp_CustomizeLegend(object sender, CustomizeLegendEventArgs e)
        {
            e.LegendItems.Clear();
        }
      private void DoChart()
        {
            _chart.ChartAreas.Clear();
             _chart.Series.Clear();
             _chart.Titles.Clear();

             ChartArea3DStyle chartArea3DStyle = new ChartArea3DStyle();
             chartArea3DStyle.Enable3D = true;
             chartArea3DStyle.LightStyle = LightStyle.Simplistic;
             chartArea3DStyle.Rotation = 30;
             chartArea3DStyle.Inclination = 45;
             chartArea3DStyle.PointDepth = 100;
             chartArea3DStyle.Perspective = 30;
             chartArea3DStyle.WallWidth = 10;
             chartArea3DStyle.IsRightAngleAxes = true;

            

             _chart.BackColor = Color.WhiteSmoke;
             _chart.BackGradientStyle = GradientStyle.None;
             _chart.BorderlineColor = Color.FromArgb(64, 0, 64);
             _chart.BorderlineDashStyle = ChartDashStyle.Solid;
             _chart.BorderlineWidth = 2;
             _chart.AntiAliasing = AntiAliasingStyles.All;
             _chart.ChartAreas.Add("Main");
             _chart.ChartAreas["Main"].Area3DStyle = chartArea3DStyle;
             _chart.ChartAreas["Main"].AxisX.Interval = 1;
             _chart.ChartAreas["Main"].AxisX.LabelStyle.Angle = -45;
             _chart.ChartAreas["Main"].AxisX.LabelStyle.Font = new Font("Verdana", 10.0f, FontStyle.Bold);

             _chart.ChartAreas["Main"].AxisY.LabelStyle.Font = new Font("Verdana", 10.0f, FontStyle.Bold);

             _chart.ChartAreas["Main"].BackColor = Color.WhiteSmoke;
             _chart.ChartAreas["Main"].BackGradientStyle = GradientStyle.VerticalCenter;
             _chart.ChartAreas["Main"].BackSecondaryColor = Color.Purple;
             _chart.ChartAreas["Main"].BorderColor = Color.FromArgb(64, 0, 64);
             _chart.ChartAreas["Main"].BorderDashStyle = ChartDashStyle.Solid;
             _chart.ChartAreas["Main"].BorderWidth = 2;
             _chart.ChartAreas["Main"].ShadowOffset = 3;
             _chart.ChartAreas["Main"].ShadowColor = Color.FromArgb(128, 0, 0);

             _chart.ChartAreas["Main"].AxisX.TitleForeColor = Color.Blue;
             _chart.ChartAreas["Main"].AxisY.TitleForeColor = Color.Blue;
             _chart.ChartAreas["Main"].AxisY.Title = "Weight";
             _chart.ChartAreas["Main"].AxisY.TextOrientation = TextOrientation.Rotated270;
             _chart.ChartAreas["Main"].AxisY.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);

             _chart.Titles.Add(ResStrings.WeightScalefrm_doChart_Weight_evolution_versus_Time);
             _chart.ChartAreas["Main"].AxisX.Title = "Time";
             _chart.ChartAreas["Main"].AxisX.TextOrientation = TextOrientation.Horizontal;
             _chart.ChartAreas["Main"].AxisX.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);

             _chart.Titles[0].Font = new Font("Verdana", 16.0f, FontStyle.Underline);
             _chart.Titles[0].ForeColor = Color.Blue;

             _chart.Series.Add("Serie");
             _chart.Series["Serie"].ChartType = SeriesChartType.Line;
            
             _chart.Series["Serie"].YValueType = ChartValueType.Double;
             _chart.Series["Serie"].IsValueShownAsLabel = false;
             _chart.Series["Serie"].ToolTip = "#VALX : #VALY";
             _chart.Series["Serie"].BackGradientStyle = GradientStyle.VerticalCenter;
             _chart.Series["Serie"].ShadowOffset = 5;
             _chart.Series["Serie"].BackSecondaryColor = Color.White;
             _chart.Series["Serie"].ShadowColor = Color.FromArgb(128, 0, 0);

            
             Font ft = new Font("Verdana", 12.0f, FontStyle.Bold);
             _chart.Series["Serie"].Font = ft;

             int nIndex = 1;
             //foreach (DataRow dr in tbHistory.Rows)
             if (_tbHistory.Rows.Count > 0)
             for (int loop =  _tbHistory.Rows.Count-1 ; loop >= 0 ; loop--)
             {
                 DataRow dr = _tbHistory.Rows[loop];
                 string value = dr[_columnInfoFull[_nIndexColWeight].colName].ToString();
                 double carat;
                 if (double.TryParse(value, NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture, out carat))
                 _chart.Series["Serie"].Points.AddXY(" ", carat);
                 nIndex++;
             }

            /* _chart.ChartAreas.Clear();
             _chart.Series.Clear();
             _chart.Titles.Clear();

             _chart.BackColor = Color.White;
             _chart.BorderlineWidth = 2;
        
             _chart.ChartAreas.Add("Main");
        
             _chart.ChartAreas["Main"].AxisX.Interval = 1;
             _chart.ChartAreas["Main"].AxisX.LabelStyle.Angle = -45;
             _chart.ChartAreas["Main"].AxisX.LabelStyle.Font = new Font("Verdana", 10.0f, FontStyle.Bold);

             _chart.ChartAreas["Main"].AxisY.LabelStyle.Font = new Font("Verdana", 10.0f, FontStyle.Bold);

             _chart.ChartAreas["Main"].AxisX.TitleForeColor = Color.Blue;
             _chart.ChartAreas["Main"].AxisY.TitleForeColor = Color.Blue;
             _chart.ChartAreas["Main"].AxisY.Title = "Weight";
             _chart.ChartAreas["Main"].AxisY.TextOrientation = TextOrientation.Rotated270;
             _chart.ChartAreas["Main"].AxisY.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);

             _chart.Titles.Add(ResStrings.WeightScalefrm_doChart_Weight_evolution_versus_Time);
             _chart.ChartAreas["Main"].AxisX.Title = "Time";
             _chart.ChartAreas["Main"].AxisX.TextOrientation = TextOrientation.Horizontal;
             _chart.ChartAreas["Main"].AxisX.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);

             _chart.Titles[0].Font = new Font("Verdana", 16.0f, FontStyle.Underline);
             _chart.Titles[0].ForeColor = Color.Blue;

             _chart.Series.Add("Serie");
             _chart.Series["Serie"].ChartType = SeriesChartType.Line;
             _chart.Series["Serie"].BorderWidth = 3;
             _chart.Series["Serie"].YValueType = ChartValueType.Double;
             _chart.Series["Serie"].IsValueShownAsLabel = false;
             _chart.Series["Serie"].ToolTip = "#VALX : #VALY";
             _chart.Series["Serie"].BackGradientStyle = GradientStyle.VerticalCenter;
         


             Font ft = new Font("Verdana", 12.0f, FontStyle.Bold);
             _chart.Series["Serie"].Font = ft;

             int nIndex = 1;
             //foreach (DataRow dr in tbHistory.Rows)
             if (_tbHistory.Rows.Count > 0)
             for (int loop = _tbHistory.Rows.Count - 1; loop >= 0; loop--)
             {
                 DataRow dr = _tbHistory.Rows[loop];
                 string value = dr[_columnInfoFull[_nIndexColWeight].colName].ToString();
                 double carat;
                 string tt = string.Empty;
                 MessageBox.Show(value);
                 if (double.TryParse(value, NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture, out carat))
                 {
                     tt += carat.ToString() + "\r\n";
                     _chart.Series["Serie"].Points.AddXY(" ", carat);
                 }
                 MessageBox.Show(tt);
                 nIndex++;
             }*/
            _chart.Invalidate();
          

        }
      private void DoChartDemo()
      {
          _chart.ChartAreas.Clear();
          _chart.Series.Clear();
          _chart.Titles.Clear();

         /*/ ChartArea3DStyle chartArea3DStyle = new ChartArea3DStyle();
          chartArea3DStyle.Enable3D = true;
          chartArea3DStyle.LightStyle = LightStyle.Simplistic;
          chartArea3DStyle.Rotation = 30;
          chartArea3DStyle.Inclination = 45;
          chartArea3DStyle.PointDepth = 100;
          chartArea3DStyle.Perspective = 30;
          chartArea3DStyle.WallWidth = 10;
          chartArea3DStyle.IsRightAngleAxes = true;*/



          _chart.BackColor = Color.White;
         /* _chart.BackGradientStyle = GradientStyle.None;
          _chart.BorderlineColor = Color.FromArgb(64, 0, 64);
          _chart.BorderlineDashStyle = ChartDashStyle.Solid;*/
          _chart.BorderlineWidth = 2;
          //_chart.AntiAliasing = AntiAliasingStyles.All;
          _chart.ChartAreas.Add("Main");
         // _chart.ChartAreas["Main"].Area3DStyle = chartArea3DStyle;
          _chart.ChartAreas["Main"].AxisX.Interval = 1;
          _chart.ChartAreas["Main"].AxisX.LabelStyle.Angle = -45;
          _chart.ChartAreas["Main"].AxisX.LabelStyle.Font = new Font("Verdana", 10.0f, FontStyle.Bold);

          _chart.ChartAreas["Main"].AxisY.LabelStyle.Font = new Font("Verdana", 10.0f, FontStyle.Bold);

          /*_chart.ChartAreas["Main"].BackColor = Color.WhiteSmoke;
          _chart.ChartAreas["Main"].BackGradientStyle = GradientStyle.VerticalCenter;
          _chart.ChartAreas["Main"].BackSecondaryColor = Color.Purple;
          _chart.ChartAreas["Main"].BorderColor = Color.FromArgb(64, 0, 64);
          _chart.ChartAreas["Main"].BorderDashStyle = ChartDashStyle.Solid;
          _chart.ChartAreas["Main"].BorderWidth = 2;
          _chart.ChartAreas["Main"].ShadowOffset = 3;
          _chart.ChartAreas["Main"].ShadowColor = Color.FromArgb(128, 0, 0);*/

          _chart.ChartAreas["Main"].AxisX.TitleForeColor = Color.Blue;
          _chart.ChartAreas["Main"].AxisY.TitleForeColor = Color.Blue;
          _chart.ChartAreas["Main"].AxisY.Title = "Weight";
          _chart.ChartAreas["Main"].AxisY.TextOrientation = TextOrientation.Rotated270;
          _chart.ChartAreas["Main"].AxisY.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);

          _chart.Titles.Add(ResStrings.WeightScalefrm_doChart_Weight_evolution_versus_Time);
          _chart.ChartAreas["Main"].AxisX.Title = "Time";
          _chart.ChartAreas["Main"].AxisX.TextOrientation = TextOrientation.Horizontal;
          _chart.ChartAreas["Main"].AxisX.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);

          _chart.Titles[0].Font = new Font("Verdana", 16.0f, FontStyle.Underline);
          _chart.Titles[0].ForeColor = Color.Blue;

          _chart.Series.Add("Serie");
          _chart.Series["Serie"].ChartType = SeriesChartType.Line;
          _chart.Series["Serie"].BorderWidth = 3;
          _chart.Series["Serie"].YValueType = ChartValueType.Double;
          _chart.Series["Serie"].IsValueShownAsLabel = false;
          _chart.Series["Serie"].ToolTip = "#VALX : #VALY";
          _chart.Series["Serie"].BackGradientStyle = GradientStyle.VerticalCenter;
         // _chart.Series["Serie"].ShadowOffset = 5;
         // _chart.Series["Serie"].BackSecondaryColor = Color.White;
         // _chart.Series["Serie"].ShadowColor = Color.FromArgb(128, 0, 0);


          Font ft = new Font("Verdana", 12.0f, FontStyle.Bold);
          _chart.Series["Serie"].Font = ft;

          int nIndex = 1;
          Random r = new Random();
          
            for (int loop = 10; loop >= 0; loop--)
            {
                 
                _chart.Series["Serie"].Points.AddXY(loop,r.Next());
                nIndex++;
            }
      }

        private void comboBoxDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedDevice = comboBoxDevice.SelectedIndex;
        }
       
    }
}
