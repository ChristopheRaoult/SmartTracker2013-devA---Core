using System;
using System.Windows.Forms;
using System.Collections;
using DBClass;
using DataClass;
using SDK_SC_RFID_Devices;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class LocalDeviceForm : Form
    {
        readonly MainDBClass _db = new MainDBClass();
   
        DeviceInfo[] _deviceArray = null;
        DeviceInfo _devLoaded;
        bool _bSaved = true;
        bool _bComeFromListBox = false;

        rfidPluggedInfo[] _arrayOfPluggedDevice = null;
        string[] _fpDevArray = null;

        public readonly Hashtable ListReader = new Hashtable
        {
            {DeviceType.DT_DSB,0 },
            {DeviceType.DT_JSC,1 },
            {DeviceType.DT_SAS,2 },
            {DeviceType.DT_MSR,3 },
            {DeviceType.DT_SBR,4 },
            {DeviceType.DT_SMC,5 },
            {DeviceType.DT_SFR,6 },
            {DeviceType.DT_STR,7 },
            {DeviceType.DT_SBF,8 },
           
        };

        public LocalDeviceForm()
        {
            InitializeComponent();
        }


        private void LocalDeviceForm_Load(object sender, EventArgs e)
        {   
            _db.OpenDB();
            UpdateListBoxDevice();
            FindDevice();
        }

        private void LocalDeviceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _db.CloseDB();
        }


        private void FindDevice()
        {
            _arrayOfPluggedDevice = null;
            RFID_Device tmp = new RFID_Device();
            _arrayOfPluggedDevice = tmp.getRFIDpluggedDevice(true);
            comboBoxSerialRFID.Items.Clear();
            if (_arrayOfPluggedDevice != null)
            {
                foreach (rfidPluggedInfo dev in _arrayOfPluggedDevice)
                {
                    comboBoxSerialRFID.Items.Add(dev.SerialRFID);
                   
                }
            }
            else
            {
                MessageBox.Show("No device found");
            }

            _fpDevArray = tmp.getFingerprintPluggedGUID();
            tmp.ReleaseDevice();
            comboBoxSerialFPMaster.Items.Clear();
            comboBoxSerialFPSlave.Items.Clear();
            if (_fpDevArray != null)
            {
                foreach (string fpDev in _fpDevArray)
                {
                    comboBoxSerialFPMaster.Items.Add(fpDev);
                    comboBoxSerialFPSlave.Items.Add(fpDev);
                }
            }           

        }

        private void UpdateListBoxDevice()
        {
            listBoxLocalDevice.Items.Clear();
            DeviceInfo[] tmpDeviceArray = _db.RecoverDevice(true);
            if (tmpDeviceArray != null)
            {
                _deviceArray = new DeviceInfo[tmpDeviceArray.Length];
                tmpDeviceArray.CopyTo(_deviceArray, 0);

                foreach (DeviceInfo dv in _deviceArray)
                {
                    string strDevice = string.Format("{0} ({1} - {2})", dv.DeviceName, dv.SerialRFID, getStringType(dv.deviceType));
                    listBoxLocalDevice.Items.Add(strDevice);
                }
            }
        }

        private string getStringType(DeviceType devType)
        {
            string strType = ResStrings.str_Unknown_Type;
            switch (devType)
            {
                case DeviceType.DT_DSB: strType = ResStrings.str_Diamond_Smart_Box; break;
                case DeviceType.DT_JSC: strType = ResStrings.str_Jewelry_Smart_Cabinet; break;
                case DeviceType.DT_SAS: strType = ResStrings.str_Diamond_SAS; break;
                case DeviceType.DT_MSR: strType = ResStrings.str_Smart_Drawer; break;
                case DeviceType.DT_SBR: strType = ResStrings.str_Smart_Board; break;
                case DeviceType.DT_SMC: strType = ResStrings.str_Medical_Cabinet_; break;
                case DeviceType.DT_SFR: strType = ResStrings.str_Smart_Fridge; break;
                case DeviceType.DT_STR: strType = ResStrings.str_Smart_Station; break;
                case DeviceType.DT_SBF: strType = ResStrings.str_Smart_Blood_Fridge; break;
            }
            return strType;
        }

        private void toolStripButtonApply_Click(object sender, EventArgs e)
        {

            //Asset combobox value fridge is only set for fridge
            if ((_devLoaded.deviceType == DeviceType.DT_SBF) || (_devLoaded.deviceType == DeviceType.DT_SFR))
                _devLoaded.fridgeType = (FridgeType)comboBoxFridgeType.SelectedIndex;
            else
                _devLoaded.fridgeType = 0;

            if (_db.StoreDevice(_devLoaded,false))
            {               
                Reset();            
                UpdateListBoxDevice();
                _bSaved = true;
                MessageBox.Show(ResStrings.str_DATA_SAVED, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Reset()
        {
            _bSaved = true;
            _bComeFromListBox = false;
            listBoxLocalDevice.SelectedIndex = -1;
            listBoxLocalDevice.Enabled = true;
            groupBoxLocalUserCtrl.Enabled = false;
            _devLoaded = new DeviceInfo();
            textBoxDeviceName.Text = null;
            comboBoxSerialRFID.Text = null;
            comboBoxSerialFPMaster.Text = null;
            comboBoxSerialFPSlave.Text = null;
            textBoxMasterBadgeReader.Text = null;
            textBoxSlaveBadgeReader.Text = null;
            textBoxTempReader.Text = null;
            checkBoxEnabledDevice.Checked = true;
            _devLoaded.enabled = Convert.ToInt32(checkBoxEnabledDevice.Checked);
            _devLoaded.accessReaderType = AccessBagerReaderType.RT_HF;
            radioHF.Checked = true;
            radioLF.Checked = false;
            listBoxReader.SelectedIndex = -1;
           
            toolStripButtonDelete.Enabled = false;
            toolStripButtonApply.Enabled = false;
            textBoxDeviceName.ReadOnly = false;
            comboBoxSerialRFID.Enabled = true;
            comboBoxSerialFPMaster.Enabled = true;
            comboBoxSerialFPSlave.Enabled = true;
            textBoxMasterBadgeReader.ReadOnly = true;
            textBoxSlaveBadgeReader.ReadOnly = true;
            textBoxTempReader.ReadOnly = true;
            groupBox1.Enabled = false;

            radioLF.Checked = false;
            radioHF.Checked = true;

            comboBoxFridgeType.Enabled = false;
            comboBoxFridgeType.SelectedIndex = 0;

        }         

        private void listBoxLocalDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxLocalDevice.SelectedIndex >= 0)
            {
                _bComeFromListBox = true;
                groupBoxLocalUserCtrl.Enabled = true;
                toolStripButtonDelete.Enabled = true;
                int rowSelected = listBoxLocalDevice.SelectedIndex;
                _devLoaded = new DeviceInfo();
                _devLoaded = _deviceArray[rowSelected];

                comboBoxSerialRFID.Enabled = false;
                comboBoxSerialRFID.Text = _devLoaded.SerialRFID;
                textBoxDeviceName.Text = _devLoaded.DeviceName;
                comboBoxSerialFPMaster.Text = _devLoaded.SerialFPMaster;
                comboBoxSerialFPSlave.Text = _devLoaded.SerialFPSlave;              

                textBoxMasterBadgeReader.Text = _devLoaded.comMasterReader;
                textBoxSlaveBadgeReader.Text = _devLoaded.comSlaveReader;
                textBoxTempReader.Text = _devLoaded.comTempReader;

                if (_devLoaded.accessReaderType == AccessBagerReaderType.RT_LF)
                {
                    radioLF.Checked = true;
                    radioHF.Checked = false;
                }
                else
                {
                    radioLF.Checked = false;
                    radioHF.Checked = true;
                }                

                checkBoxEnabledDevice.Checked = Convert.ToBoolean(_devLoaded.enabled);   
            
                if (_devLoaded.deviceType != DeviceType.DT_UNKNOWN)
                listBoxReader.SelectedIndex = (int)ListReader[_devLoaded.deviceType];

                comboBoxFridgeType.SelectedIndex = (int) _devLoaded.fridgeType;
            }
        }

       

        private void toolStripButtonReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void toolStripButtonCreateNew_Click(object sender, EventArgs e)
        {
            Reset();
            groupBoxLocalUserCtrl.Enabled = true;
            checkBoxEnabledDevice.Checked = true;
            listBoxLocalDevice.SelectedIndex = -1;
            listBoxLocalDevice.Enabled = false;

        }

        private void textBoxSerialRFID_TextChanged(object sender, EventArgs e)
        {

            if (comboBoxSerialRFID.Text.Length != 8) return;

            bool bDevInBase = false;
            if (_deviceArray != null)
            {
                foreach (DeviceInfo dev in _deviceArray)
                {
                    if (dev.SerialRFID == comboBoxSerialRFID.Text)
                    {
                        bDevInBase = true;
                        break;
                    }
                }
            }

            if (!bDevInBase)
            {

                _devLoaded.SerialRFID = comboBoxSerialRFID.Text;
                try
                {
                    // Ping ping = new Ping();
                   // PingReply reply = ping.Send("ns1.infomaniak.ch", 500);
                   // if (reply.Status == IPStatus.Success)
                   // {
                    DeviceInfo findDevice;
                    if (deviceUtils.getDeviceInfo(_devLoaded.SerialRFID, out findDevice))                       
                        {
                            switch (findDevice.deviceType)
                            {
                                case DeviceType.DT_SBR:
                                case DeviceType.DT_STR:

                                    _devLoaded.SerialFPMaster = string.Empty;
                                    comboBoxSerialFPMaster.SelectedIndex = -1;

                                    _devLoaded.SerialFPSlave = string.Empty;
                                    comboBoxSerialFPSlave.SelectedIndex = -1;

                                    _devLoaded.deviceType = findDevice.deviceType;
                                    listBoxReader.SelectedIndex = (int)ListReader[_devLoaded.deviceType];

                                    break;
                                case DeviceType.DT_DSB:
                                case DeviceType.DT_JSC:

                                    _devLoaded.SerialFPMaster = findDevice.SerialFPMaster;
                                    comboBoxSerialFPMaster.Text = findDevice.SerialFPMaster;

                                    _devLoaded.SerialFPSlave = string.Empty;
                                    comboBoxSerialFPSlave.SelectedIndex = -1;

                                    _devLoaded.deviceType = findDevice.deviceType;
                                    listBoxReader.SelectedIndex = (int)ListReader[_devLoaded.deviceType];

                                    break;

                                case DeviceType.DT_SAS:                            

                                    _devLoaded.SerialFPMaster = findDevice.SerialFPMaster;
                                    comboBoxSerialFPMaster.Text = findDevice.SerialFPMaster;

                                    _devLoaded.SerialFPSlave = findDevice.SerialFPSlave;
                                    comboBoxSerialFPSlave.Text = findDevice.SerialFPSlave;

                                    _devLoaded.deviceType = findDevice.deviceType;
                                    listBoxReader.SelectedIndex = (int)ListReader[_devLoaded.deviceType];

                                    break;
                                case DeviceType.DT_MSR:

                                    _devLoaded.SerialFPMaster = findDevice.SerialFPMaster;
                                    comboBoxSerialFPMaster.Text = findDevice.SerialFPMaster;

                                    _devLoaded.SerialFPSlave = findDevice.SerialFPSlave;
                                    comboBoxSerialFPSlave.Text = findDevice.SerialFPSlave;

                                    _devLoaded.deviceType = findDevice.deviceType;
                                    listBoxReader.SelectedIndex = (int)ListReader[_devLoaded.deviceType];

                                    _devLoaded.accessReaderType = findDevice.accessReaderType;
                                    if (_devLoaded.accessReaderType == AccessBagerReaderType.RT_LF)
                                    {
                                        radioLF.Checked = true;
                                        radioHF.Checked = false;
                                    }
                                    else
                                    {
                                        radioLF.Checked = false;
                                        radioHF.Checked = true;
                                    }

                                    _devLoaded.comMasterReader = findDevice.comMasterReader;
                                    _devLoaded.comSlaveReader = findDevice.comSlaveReader;
                                    textBoxMasterBadgeReader.Text = _devLoaded.comMasterReader;
                                    textBoxSlaveBadgeReader.Text = _devLoaded.comSlaveReader;
                                   
                                    break;
                            }
                       // }
                            textBoxDeviceName.Focus();
                    }
                }
                catch { // avoid no connet PC
                }
            }


            if ((!string.IsNullOrEmpty(comboBoxSerialRFID.Text)) && (!string.IsNullOrEmpty(textBoxDeviceName.Text)))
            {
                toolStripButtonApply.Enabled = true;  
                _bSaved = _bComeFromListBox;
            }
        }

        private void textBoxDeviceName_TextChanged(object sender, EventArgs e)
        {
            _devLoaded.DeviceName = textBoxDeviceName.Text;

            if ((!string.IsNullOrEmpty(comboBoxSerialRFID.Text)) && (!string.IsNullOrEmpty(textBoxDeviceName.Text)))
            {
                toolStripButtonApply.Enabled = true;
                _bSaved = _bComeFromListBox;
            }
        }

        private void textBoxSerialFPMaster_TextChanged(object sender, EventArgs e)
        {
            _devLoaded.SerialFPMaster = comboBoxSerialFPMaster.Text;
            toolStripButtonApply.Enabled = true;
            _bSaved = _bComeFromListBox;
        }

        private void textBoxSerialFPSlave_TextChanged(object sender, EventArgs e)
        {
            _devLoaded.SerialFPSlave = comboBoxSerialFPSlave.Text;
            toolStripButtonApply.Enabled = true;
            _bSaved = _bComeFromListBox;
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if (!_db.DeleteDevice(_devLoaded))
            {
                MessageBox.Show(ResStrings.str_Error_during_deleting_device_Device_not_deleted, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Reset();
                UpdateListBoxDevice();
            }

        }

        private void toolStripButtonExit_Click(object sender, EventArgs e)
        {
            if (!_bSaved)
                MessageBox.Show(ResStrings.str_DATA_NOT_SAVED, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                BeginInvoke((MethodInvoker)delegate { Close(); });
        }

        private void checkBoxEnabledDevice_CheckedChanged(object sender, EventArgs e)
        {
            _devLoaded.enabled = Convert.ToInt32(checkBoxEnabledDevice.Checked);
            toolStripButtonApply.Enabled = true;
            _bSaved = _bComeFromListBox;
        }

        private void textBoxComMaster_TextChanged(object sender, EventArgs e)
        {
            _devLoaded.comMasterReader = textBoxMasterBadgeReader.Text;
            toolStripButtonApply.Enabled = true;
            _bSaved = _bComeFromListBox;
        }
        private void textBoxComSlave_TextChanged(object sender, EventArgs e)
        {
            _devLoaded.comSlaveReader = textBoxSlaveBadgeReader.Text;
            toolStripButtonApply.Enabled = true;
            _bSaved = _bComeFromListBox;
        }

        private void textBoxTempReader_TextChanged(object sender, EventArgs e)
        {
            _devLoaded.comTempReader = textBoxTempReader.Text;
            toolStripButtonApply.Enabled = true;
            _bSaved = _bComeFromListBox;
        }

        private void listBoxReader_MouseMove(object sender, MouseEventArgs e)
        {
            int index = listBoxReader.IndexFromPoint(e.X, e.Y);
            switch (index)
            {
                case 0: pB1.Image = imageListReader.Images["DT_DSB"]; break;
                case 1: pB1.Image = imageListReader.Images["DT_JSC"]; break;
                case 2: pB1.Image = imageListReader.Images["DT_SAS"]; break;
                case 3: pB1.Image = imageListReader.Images["DT_MSR"]; break;
                case 4: pB1.Image = imageListReader.Images["DT_SBR"]; break;
                case 5: pB1.Image = imageListReader.Images["DT_SMC"]; break;
                case 6: pB1.Image = imageListReader.Images["DT_SFR"]; break;
                case 7: pB1.Image = imageListReader.Images["DT_STR"]; break;
                case 8: pB1.Image = imageListReader.Images["DT_SFR"]; break;
            }
        }

        private void listBoxReader_MouseLeave(object sender, EventArgs e)
        {
            if (listBoxReader.SelectedIndex >= 0)
            {
                switch (listBoxReader.SelectedIndex)
                {
                    case 0: pB1.Image = imageListReader.Images["DT_DSB"]; break;
                    case 1: pB1.Image = imageListReader.Images["DT_JSC"]; break;
                    case 2: pB1.Image = imageListReader.Images["DT_SAS"]; break;
                    case 3: pB1.Image = imageListReader.Images["DT_MSR"]; break;
                    case 4: pB1.Image = imageListReader.Images["DT_SBR"]; break;
                    case 5: pB1.Image = imageListReader.Images["DT_SMC"]; break;
                    case 6: pB1.Image = imageListReader.Images["DT_SFR"]; break;
                    case 7: pB1.Image = imageListReader.Images["DT_STR"]; break;
                    case 8: pB1.Image = imageListReader.Images["DT_SFR"]; break;
                }
            }
        }

        private void listBoxReader_SelectedIndexChanged(object sender, EventArgs e)
        {
            btUpdateBadgeCom.Visible = false;
            if (listBoxReader.SelectedIndex >= 0)
            {
                comboBoxFridgeType.Enabled = false;
                switch (listBoxReader.SelectedIndex)
                {
                    case 0:
                    {
                        comboBoxSerialFPMaster.Enabled = true;
                        comboBoxSerialFPSlave.Enabled = false;
                        textBoxMasterBadgeReader.ReadOnly = true;
                        textBoxSlaveBadgeReader.ReadOnly = true;
                        textBoxTempReader.ReadOnly = true;
                        groupBox1.Enabled = false;
                        _devLoaded.deviceType = DeviceType.DT_DSB;
                        pB1.Image = imageListReader.Images["DT_DSB"];

                    }
                    break;
                    case 1:
                    {
                        comboBoxSerialFPMaster.Enabled = true;
                        comboBoxSerialFPSlave.Enabled = false;
                        textBoxMasterBadgeReader.ReadOnly = true;
                        textBoxSlaveBadgeReader.ReadOnly = true;
                        textBoxTempReader.ReadOnly = true;
                        groupBox1.Enabled = false;
                        _devLoaded.deviceType = DeviceType.DT_JSC;
                        pB1.Image = imageListReader.Images["DT_JSC"];
                    }
                    break;
                    case 2:
                    {
                        comboBoxSerialFPMaster.Enabled = true;
                        comboBoxSerialFPSlave.Enabled = true;
                        textBoxMasterBadgeReader.ReadOnly = true;
                        textBoxSlaveBadgeReader.ReadOnly = true;
                        textBoxTempReader.ReadOnly = true;
                        groupBox1.Enabled = false;
                        _devLoaded.deviceType = DeviceType.DT_SAS;
                        pB1.Image = imageListReader.Images["DT_SAS"];

                    }
                    break;
                    case 3:
                    {
                        comboBoxSerialFPMaster.Enabled = true;
                        comboBoxSerialFPSlave.Enabled = true;
                        textBoxMasterBadgeReader.ReadOnly = false;
                        textBoxSlaveBadgeReader.ReadOnly = false;
                        textBoxTempReader.ReadOnly = true;
                        groupBox1.Enabled = true;
                        _devLoaded.deviceType = DeviceType.DT_MSR;
                        pB1.Image = imageListReader.Images["DT_MSR"];
                        btUpdateBadgeCom.Visible = true;
                    }
                    break;
                    case 4:
                    {
                        comboBoxSerialFPMaster.Enabled = false;
                        comboBoxSerialFPSlave.Enabled = false;
                        textBoxMasterBadgeReader.ReadOnly = true;
                        textBoxSlaveBadgeReader.ReadOnly = true;
                        textBoxTempReader.ReadOnly = true;
                        groupBox1.Enabled = false;
                        _devLoaded.deviceType = DeviceType.DT_SBR;
                        pB1.Image = imageListReader.Images["DT_SBR"];

                    }
                    break;
                    case 5:
                    {
                        comboBoxSerialFPMaster.Enabled = true; //for HDC
                        comboBoxSerialFPSlave.Enabled = false;
                        textBoxMasterBadgeReader.ReadOnly = false;
                        textBoxSlaveBadgeReader.ReadOnly = true;
                        groupBox1.Enabled = false;
                        textBoxTempReader.ReadOnly = true;
                        _devLoaded.deviceType = DeviceType.DT_SMC;
                        pB1.Image = imageListReader.Images["DT_SMC"];
                    }
                    break;
                    case 6:
                        {
                            comboBoxSerialFPMaster.Enabled = true;
                            comboBoxSerialFPSlave.Enabled = false;
                            textBoxMasterBadgeReader.ReadOnly = false;
                            textBoxSlaveBadgeReader.ReadOnly = true;
                            textBoxTempReader.ReadOnly = false;
                            _devLoaded.deviceType = DeviceType.DT_SFR;
                            groupBox1.Enabled = true;
                            pB1.Image = imageListReader.Images["DT_SFR"];
                            comboBoxFridgeType.Enabled = true;
                        }
                        break;
                    case 7:
                         comboBoxSerialFPMaster.Enabled = false;
                         comboBoxSerialFPSlave.Enabled = false;
                         textBoxMasterBadgeReader.ReadOnly = true;
                        textBoxSlaveBadgeReader.ReadOnly = true;
                         textBoxTempReader.ReadOnly = true;
                         groupBox1.Enabled = false;
                         _devLoaded.deviceType = DeviceType.DT_STR;
                         pB1.Image = imageListReader.Images["DT_STR"];
                    break;
                    case 8:
                    {
                        comboBoxSerialFPMaster.Enabled = true;
                        comboBoxSerialFPSlave.Enabled = false;
                        textBoxMasterBadgeReader.ReadOnly = false;
                        textBoxSlaveBadgeReader.ReadOnly = true;
                        textBoxTempReader.ReadOnly = false;
                        groupBox1.Enabled = true;
                        _devLoaded.deviceType = DeviceType.DT_SBF;
                        pB1.Image = imageListReader.Images["DT_SFR"];
                        comboBoxFridgeType.Enabled = true;
                    }
                    break;
                }
            }
        }

        private void listBoxReader_MouseClick(object sender, MouseEventArgs e)
        {
            comboBoxSerialFPMaster.Text = null;
            comboBoxSerialFPSlave.Text = null;
            textBoxMasterBadgeReader.Text = null;
            textBoxSlaveBadgeReader.Text = null;
            textBoxTempReader.Text = null;
        }

        private void radioLF_Click(object sender, EventArgs e)
        {
            _devLoaded.accessReaderType = AccessBagerReaderType.RT_LF;
            toolStripButtonApply.Enabled = true;
            _bSaved = _bComeFromListBox;
        }

        private void radioHF_Click(object sender, EventArgs e)
        {
            _devLoaded.accessReaderType = AccessBagerReaderType.RT_HF;
            toolStripButtonApply.Enabled = true;
            _bSaved = _bComeFromListBox;
        }

        private void button1_Click(object sender, EventArgs e)
        {
              DeviceInfo findDevice;
              if (deviceUtils.getDeviceInfo(_devLoaded.SerialRFID, out findDevice))
              {
                  textBoxMasterBadgeReader.Text = findDevice.comMasterReader;
                  textBoxSlaveBadgeReader.Text = findDevice.comSlaveReader;
              }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            FindDevice();
        }

        private void comboBoxFridgeType_SelectedIndexChanged(object sender, EventArgs e)
        {
          
            _devLoaded.fridgeType = (FridgeType) comboBoxFridgeType.SelectedIndex;
            toolStripButtonApply.Enabled = true;
            _bSaved = _bComeFromListBox;
        }

       
    }
}
