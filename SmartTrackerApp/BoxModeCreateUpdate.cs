using System;
using System.Linq;
using System.Windows.Forms;
using DBClass;
using DataClass;
using ErrorMessage;
using SDK_SC_RfidReader;
using SDK_SC_RFID_Devices;
using System.Collections;
using System.Threading;
using System.Text.RegularExpressions;
using smartTracker.Properties;


namespace smartTracker
{
    public partial class BoxModeCreateUpdate : Form
    {

        public readonly Hashtable Op = new Hashtable
        {
            {0, " = "},
            {1, " <= "},
            {2, " < "},
            {3, " > "},
            {4, " >= "},
            {5, " <> " },
            {6, " AND "},
            {7, " OR "}, 
        };

        MainDBClass db = new MainDBClass();
        BoxTagInfo _bti;

        LiveDataForm ldf;

        dtColumnInfo[] _colInfo;

        rfidPluggedInfo[] _arrayOfPluggedDevice;
        int selectedDevice = 0;
        //General device object
        private RFID_Device _device;

        ArrayList _listTagId = new ArrayList();
    

        public BoxModeCreateUpdate(BoxTagInfo bti, LiveDataForm ldf)
        {
            InitializeComponent();
            _bti = bti;
            this.ldf = ldf;
            db.OpenDB();
        }

        private void BoxModeCreateUpdate_Load(object sender, EventArgs e)
        {
            if (_bti != null)
                UpdateInfo();

            for (int i = 0; i < Op.Count; i++)
            {
                listBoxOperator.Items.Add(Op[i].ToString());
            }

            comboBoxReader.SelectedIndex = 0;
            UpdateColumn();
        }

        private void UpdateInfo()
        {
            txtBoxTagUid.Text = _bti.TagBox;
            txtBoxRef.Text = _bti.BoxRef;
            txtBoxDesc.Text = _bti.BoxDesc;
            txtCriteria.Text =  _bti.Criteria.Replace("&quote", "'");
        }

        private void UpdateColumn()
        {
            _colInfo = db.GetdtColumnInfo();
            if (_colInfo != null)
            {
                listBoxParam.Items.Clear();

                for (int i = 0; i < _colInfo.Length; i++)
                {     
                    string colStr = "[" +  _colInfo[i].colName + "]";
                    listBoxParam.Items.Add(colStr);                   
                }
            }
        }


        private bool ValidUid(string uid)
        {
            if (uid.Length != 10) return false;
            Regex myRegex = new Regex(@"[0-7]{10}");
            //([\w]+) ==> caractère alphanumérique apparaissant une fois ou plus 
            //return myRegex.IsMatch(uid); // retourne true ou false selon la vérification
            return true;
        }
                 


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                if (!ValidUid(txtBoxTagUid.Text))
                {
                    MessageBox.Show(ResStrings.BoxModeCreateUpdate_button1_Click_TagUID_not_Valid_, ResStrings.BoxModeCreateUpdate_button1_Click_Box_Mode_Info, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _bti = new BoxTagInfo();
                _bti.TagBox = txtBoxTagUid.Text;
                _bti.BoxRef = txtBoxRef.Text;
                _bti.BoxDesc = txtBoxDesc.Text;
                _bti.Criteria = txtCriteria.Text.Replace("'","&quote");

                db.DeleteGroup(_bti.TagBox);
                db.AddGroup(_bti.TagBox, _bti.BoxRef, _bti.BoxDesc, _bti.Criteria);
                Close();
            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }

        }

        private void BoxModeCreateUpdate_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.CloseDB();

            if (_device == null) return;
            if (_device.DeviceStatus == DeviceStatus.DS_NotReady) 
                _device.StopScan();
            if (_device.ConnectionStatus == ConnectionStatus.CS_Connected)
                _device.ReleaseDevice();


        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<LiveDataForm>().Any())
            {
                MessageBox.Show(ResStrings.StrClick_Live_Data_will_be_closed_to_use_device, ResStrings.BoxModeCreateUpdate_button1_Click_Box_Mode_Info, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Invoke((MethodInvoker)delegate { toolStrip.Text = ResStrings.str_Search_connected_device; });
                ldf.Close();
                Thread.Sleep(1000);
            }

            Invoke((MethodInvoker)delegate { toolStrip.Text = ResStrings.str_Search_connected_device; });
            FindDevice();
        }

        //Function to discover device
        private void FindDevice()
        {
            _arrayOfPluggedDevice = null;
            RFID_Device tmp = new RFID_Device();
            _arrayOfPluggedDevice = tmp.getRFIDpluggedDevice(true);
            tmp.ReleaseDevice();
            comboBoxReader.Items.Clear();
            if (_arrayOfPluggedDevice != null)
            {
                foreach (rfidPluggedInfo dev in _arrayOfPluggedDevice)
                {
                    if ((dev.deviceType == DeviceType.DT_SBR) || (dev.deviceType == DeviceType.DT_STR))
                    {
                        DeviceInfo di = db.RecoverDevice(dev.SerialRFID);
                        if (di != null)
                            comboBoxReader.Items.Add(di.DeviceName + " (" + dev.SerialRFID + ")");
                        else
                            comboBoxReader.Items.Add(dev.SerialRFID);
                    }
                }
                if (comboBoxReader.Items.Count > 0)
                {
                    comboBoxReader.SelectedIndex = 0;                  
                }
               
            }
            else
            {
                Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Quit_Live_Data; });
               
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ScanDevice();
        }

        private void comboBoxReader_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_arrayOfPluggedDevice == null) return;
            // release previous object if not connected
            if (_device != null)
            {
                if (_device.ConnectionStatus != ConnectionStatus.CS_Connected)
                    _device.ReleaseDevice();
            }
            //Create a new object 
            _device = new RFID_Device();
            toolStripStatusLabelInfo.Text = ResStrings.str_In_Connection_;          
            //subscribe the event 
            _device.NotifyRFIDEvent += new NotifyHandlerRFIDDelegate(rfidDev_NotifyRFIDEvent);
            //Create a smartboard device
            //As the function search on all the serial port of the PC, this connection can
            //take some time and is under a thread pool to avoid freeze of the GUI          

            switch (_arrayOfPluggedDevice[selectedDevice].deviceType)
            {
                case DeviceType.DT_SBR:
                case DeviceType.DT_STR:
                    //  Use create with portcom parameter for speed connection (doesn't search again the reader at is is previouly done;
                    _device.Create_NoFP_Device(_arrayOfPluggedDevice[selectedDevice].SerialRFID, _arrayOfPluggedDevice[selectedDevice].portCom);

                    //device.Create_NoFP_Device(arrayOfPluggedDevice[selectedDevice].SerialRFID);
                    break;

                default:
                    MessageBox.Show(ResStrings.str_OnlySBR_STR, ResStrings.BoxModeCreateUpdate_button1_Click_Box_Mode_Info, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }

                    
        }

        private void rfidDev_NotifyRFIDEvent(object sender, rfidReaderArgs args)
        {
            switch (args.RN_Value)
            {
                // Event when failed to connect          
                case rfidReaderArgs.ReaderNotify.RN_FailedToConnect:
                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Failed_to_Connect; });
                    Invoke((MethodInvoker)delegate { butScan.Enabled = false; });

                    break;
                // Event when release the object
                case rfidReaderArgs.ReaderNotify.RN_Disconnected:
                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Device_Disconnected; });
                    Invoke((MethodInvoker)delegate { butScan.Enabled = false; });

                    break;

                //Event when device is connected
                case rfidReaderArgs.ReaderNotify.RN_Connected:
                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Device_Connected; });
                    Invoke((MethodInvoker)delegate { butScan.Enabled = true; });

                    break;

                // Event when scan started
                case rfidReaderArgs.ReaderNotify.RN_ScanStarted:


                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Scan_Started; });
                    Invoke((MethodInvoker)delegate { butScan.Enabled = false; });
                    break;

                //event when fail to start scan
                case rfidReaderArgs.ReaderNotify.RN_ReaderFailToStartScan:

                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Failed_to_start_scan; });
                    break;

                //event when a new tag is identify
                case rfidReaderArgs.ReaderNotify.RN_TagAdded:

                    break;

                // Event when scan completed
                case rfidReaderArgs.ReaderNotify.RN_ScanCompleted:

                    if (_device.currentInventory.nbTagAll == 0)
                    {
                        Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_No_Tag_Found; });
                    }
                    else if (_device.currentInventory.nbTagAll > 1)
                    {
                        Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_More_than_one_tag_scanned; });
                    }
                    else
                    {
                        Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str__Scan_Completed___Tag_Scanned___ + _device.currentInventory.listTagAll[0]; });
                        Invoke((MethodInvoker)delegate { txtBoxTagUid.Text = _device.currentInventory.listTagAll[0].ToString(); });
                     
                    }

                    Invoke((MethodInvoker)delegate { butScan.Enabled = true; });
                    break;

                //error when error during scan
                case rfidReaderArgs.ReaderNotify.RN_ReaderScanTimeout:
                case rfidReaderArgs.ReaderNotify.RN_ErrorDuringScan:

                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str__Scan_has_error; });
                    Invoke((MethodInvoker)delegate { butScan.Enabled = true; });
                    break;
                case rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost:

                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str__Scan_cancel_by_host; });
                    Invoke((MethodInvoker)delegate { butScan.Enabled = true; });
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
                MessageBox.Show(ResStrings.StrDeviceNotReadyOrNotConnected, ResStrings.BoxModeCreateUpdate_button1_Click_Box_Mode_Info, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void listBoxParam_Click(object sender, EventArgs e)
        {
            if (listBoxParam.SelectedIndex > -1)
            {   
                txtCriteria.Text += "[" + _colInfo[listBoxParam.SelectedIndex].colName + "]";
                txtCriteria.SelectionLength = 0;
                txtCriteria.SelectionStart =  txtCriteria.TextLength;               
                txtCriteria.Focus();
            }
        }

        private void listBoxOperator_Click(object sender, EventArgs e)
        {
            if (listBoxOperator.SelectedIndex > -1)
            {
                if (listBoxOperator.SelectedIndex < 6)
                {
                    txtCriteria.Text += Op[listBoxOperator.SelectedIndex] + ResStrings.BoxModeCreateUpdate_listBoxOperator_Click__Enter_value_here_;
                    txtCriteria.SelectionStart = txtCriteria.Text.IndexOf(ResStrings.BoxModeCreateUpdate_listBoxOperator_Click_Enter, StringComparison.Ordinal);
                    txtCriteria.SelectionLength = 16;

                }
                else
                {
                    txtCriteria.Text += Op[listBoxOperator.SelectedIndex];
                    txtCriteria.SelectionLength = 0;
                    txtCriteria.SelectionStart = txtCriteria.TextLength;

                }

                txtCriteria.Focus();
                
            }
           
        }
    }
}
