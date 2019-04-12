using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Text.RegularExpressions;

using DBClass;
using DataClass;
using SDK_SC_RfidReader;
using SDK_SC_RFID_Devices;
using smartTracker.Properties;
using TcpIP_class;

namespace smartTracker
{
    public partial class AutoFillBox : Form
    {
        private MainDBClass _db;
        private Hashtable _columnInfo;
        private readonly DataGridView _dgv;
        private readonly ItemListForm _itf;
        private readonly LiveDataForm _ldf;
        private rfidPluggedInfo[] _arrayOfPluggedDevice;
        private int _selectedDeviceIndex;
        private int _selectedIndex = -1;
        //General device object
        private RFID_Device _device;

        private readonly List<string> _listTagId = new List<string>();
        private readonly Dictionary<string, DeviceInfo> _ethernetDevices = new Dictionary<string, DeviceInfo>(); // <Name (from ComboBox), DeviceInfo> 
        private DeviceInfo _currentEthernetDevice; // Set to null when user create a local device. Connected ethernet device otherwise.

        private readonly Regex _myRegexUid;
        public AutoFillBox(ItemListForm itf, LiveDataForm ldf)
        {
            InitializeComponent();
            _dgv = itf.dataGridViewImport;
            _itf = itf;
            _ldf = ldf;

            _myRegexUid = new Regex(@"[0-7]{10}");
        }

        //Function to discover device
        private void FindDevice()
        {
            comboBoxDevice.Items.Clear();
            _ethernetDevices.Clear();
            RFID_Device tmp = new RFID_Device();
            rfidPluggedInfo[] tmpDev = tmp.getRFIDpluggedDevice(true);
            tmp.ReleaseDevice();
            if (tmpDev != null)
            {
                int nIndex = 0;
                _arrayOfPluggedDevice = new rfidPluggedInfo[tmpDev.Length];
                foreach (rfidPluggedInfo dev in tmpDev)
                {
                    if (dev.deviceType != DeviceType.DT_SBR && dev.deviceType != DeviceType.DT_STR) continue;

                    DeviceInfo di = _db.RecoverDevice(dev.SerialRFID);
                    string itemText = (di != null) ? di.DeviceName + " (" + dev.SerialRFID + ")" : dev.SerialRFID;
                    comboBoxDevice.Items.Add(itemText);
                    _arrayOfPluggedDevice[nIndex++] = dev;
                }
            }

            DeviceInfo[] ethernetDevices = _db.RecoverDevice(false); // bLocal = false => only looks for ethernet devices

            if (ethernetDevices != null)
            {
                foreach (DeviceInfo ethernetDevice in ethernetDevices)
                {
                    if (ethernetDevice.deviceType != DeviceType.DT_SBR && ethernetDevice.deviceType != DeviceType.DT_STR) continue;
                    comboBoxDevice.Items.Add(ethernetDevice.DeviceName);
                    _ethernetDevices.Add(ethernetDevice.DeviceName, ethernetDevice);
                }
            }

            if (comboBoxDevice.Items.Count > 0)
            {
                comboBoxDevice.SelectedIndex = 0;
                buttonCreate.Enabled = true;
            }

            else
            {
                MessageBox.Show(ResStrings.str_No_Device_found, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Quit_Live_Data; });
                buttonCreate.Enabled = false;
            }
        }

        private void AutoFillBox_Load(object sender, EventArgs e)
        {
            _db = new MainDBClass();

            _db.OpenDB();
            _columnInfo = _db.GetColumnInfo();


            labelLotID.Text = ResStrings.strEnter + _columnInfo[1];
            labelTagID.Text = ResStrings.strEnter + _columnInfo[0];


            foreach (DataGridViewRow oRow in _dgv.Rows)
            {
                if (!string.IsNullOrEmpty(oRow.Cells[_columnInfo[0].ToString()].Value.ToString()))
                    _listTagId.Add(oRow.Cells[_columnInfo[0].ToString()].Value.ToString());
            }

            textBoxLotID.Focus();
        }

        private void textBoxLotID_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Return)
            {
                _selectedIndex = SearchProductRef(_dgv, textBoxLotID.Text);
                if (_selectedIndex > -1)
                {
                    foreach (DataGridViewRow oRow in _dgv.Rows)
                        oRow.Selected = false;

                    _dgv.Rows[_selectedIndex].Selected = true;
                    _dgv.FirstDisplayedScrollingRowIndex = _selectedIndex;

                    if (checkBoxUseRFid.Checked)
                        ScanDevice();

                    else
                        textBoxTagID.Focus();
                }

                else
                    toolStripStatusLabelInfo.Text = ResStrings.str_Reference_not_found_in_the_grid_;
            }
        }

        private void ProcessTag()
        {
            string newUid = textBoxTagID.Text.Trim();

            if (String.IsNullOrEmpty(newUid))
            {
                MessageBox.Show(ResStrings.str_Tag_Empty_input, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!SDK_SC_RfidReader.DeviceBase.SerialRFID.isStringValidToWrite(newUid))
            {
                MessageBox.Show(ResStrings.str_Tag_Invalid_Tag_ID,ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_listTagId.Contains(newUid))
            {
                MessageBox.Show(ResStrings.str_Tag_ID_already_in_the_datagrid,ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxTagID.Text = String.Empty;
                textBoxTagID.Focus();
            }

            else
            {
                if (_selectedIndex < 0 || _selectedIndex > _dgv.RowCount - 1)
                {
                    if (_dgv.SelectedRows.Count > 0)
                        _selectedIndex = _dgv.SelectedRows[0].Index;

                    else
                    {
                        MessageBox.Show(ResStrings.str_No_line_has_been_selected, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                _listTagId.Add(newUid);
                _dgv.Rows[_selectedIndex].Cells[_columnInfo[0].ToString()].Value = newUid;
                _dgv.Rows[_selectedIndex].Selected = false;
                _itf.CheckDg(_selectedIndex);
                textBoxLotID.Focus();
                textBoxLotID.Text = String.Empty;
                textBoxTagID.Text = String.Empty;
            }

        }

        private int SearchProductRef(DataGridView dg, string productRef)
        {
            foreach (DataGridViewRow oRow in dg.Rows)
            {
                if (oRow.Cells[_columnInfo[1].ToString()].Value.ToString().Equals(productRef))
                {
                    return oRow.Index;
                }
            }

            return -1;
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            if (_arrayOfPluggedDevice.Length-1 < _selectedDeviceIndex) // Ethernet devices are added (in comboBox) after USB devices. Their index is greater than _arrayOfPluggedDevice length
            {
                MessageBox.Show(ResStrings.str_create_Ethernet_devices, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // release previous object if not connected
            if (_device != null)
            {
                if (_device.ConnectionStatus != ConnectionStatus.CS_Connected)
                    _device.ReleaseDevice();
            }

            _device = new RFID_Device();
            toolStripStatusLabelInfo.Text = ResStrings.str_In_Connection_;
            buttonCreate.Enabled = false;
            _device.NotifyRFIDEvent += new NotifyHandlerRFIDDelegate(rfidDev_NotifyRFIDEvent);

            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    switch (_arrayOfPluggedDevice[_selectedDeviceIndex].deviceType)
                    {
                        case DeviceType.DT_SBR:
                        case DeviceType.DT_STR:
                            _device.Create_NoFP_Device(_arrayOfPluggedDevice[_selectedDeviceIndex].SerialRFID, _arrayOfPluggedDevice[_selectedDeviceIndex].portCom);
                            _currentEthernetDevice = null;
                            break;

                        default:
                            MessageBox.Show(ResStrings.str_OnlySBR_STR, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                    }

                });
        }

        private void buttonDispose_Click(object sender, EventArgs e)
        {
            if (_device == null) return;
            if (_device.DeviceStatus != DeviceStatus.DS_Ready)
            {
                checkBoxUseRFid.Checked = false;
                Thread.Sleep(1000);
            }
            if (_device.ConnectionStatus == ConnectionStatus.CS_Connected)
                _device.ReleaseDevice();
            buttonCreate.Enabled = true;
        }

        // Function to get rfid event
        private void rfidDev_NotifyRFIDEvent(object sender, rfidReaderArgs args)
        {
            switch (args.RN_Value)
            {       
                case rfidReaderArgs.ReaderNotify.RN_FailedToConnect:
                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Failed_to_Connect; });
                    Invoke((MethodInvoker)delegate { buttonCreate.Enabled = true; });
                    Invoke((MethodInvoker)delegate { buttonDispose.Enabled = false; });

                    break;

                case rfidReaderArgs.ReaderNotify.RN_Disconnected:
                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Device_Disconnected; });
                    Invoke((MethodInvoker)delegate { buttonCreate.Enabled = true; });
                    Invoke((MethodInvoker)delegate { buttonDispose.Enabled = false; });

                    break;

                case rfidReaderArgs.ReaderNotify.RN_Connected:
                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Device_Connected; });
                    Invoke((MethodInvoker)delegate { buttonCreate.Enabled = false; });
                    Invoke((MethodInvoker)delegate { buttonDispose.Enabled = true; });
                    Invoke((MethodInvoker) delegate
                    {
                        // cbxAutoWrite.Visible = !_device.get_RFID_Device.FirmwareVersion.StartsWith("1");
                        double fv = 0.0;
                        double.TryParse(_device.get_RFID_Device.FirmwareVersion.Replace(",", "."),
                            System.Globalization.NumberStyles.Number,
                            System.Globalization.CultureInfo.InvariantCulture, out fv);
                        cbxAutoWrite.Visible = fv > 2.54;
                    });

                        break;

                case rfidReaderArgs.ReaderNotify.RN_ScanStarted:
                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Scan_Started; });
                    break;

                case rfidReaderArgs.ReaderNotify.RN_ReaderFailToStartScan:
                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Failed_to_start_scan; });
                    break;

                case rfidReaderArgs.ReaderNotify.RN_TagAdded:
                    break;

                case rfidReaderArgs.ReaderNotify.RN_ScanCompleted:

                    if (_device.currentInventory.nbTagAll == 0)
                    {
                        Thread.Sleep(500);
                        Application.DoEvents();

                        if (checkBoxUseRFid.Checked)
                            ScanDevice();

                        else
                            Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_No_Tag_Found; });
                    }

                    else if (_device.currentInventory.nbTagAll > 1)
                        Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_More_than_one_tag_scanned; });

                    else
                    {
                        Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str__Scan_Completed___Tag_Scanned___ + _device.currentInventory.listTagAll[0]; });
                        Invoke((MethodInvoker)delegate { textBoxTagID.Text = _device.currentInventory.listTagAll[0].ToString(); });

                        if (cbxAutoWrite.Checked)
                        {
                            if (WriteLotId(_device.currentInventory.listTagAll[0].ToString(), textBoxLotID.Text))
                            {
                                Invoke((MethodInvoker)delegate { textBoxTagID.Text = textBoxLotID.Text; });
                                Invoke((MethodInvoker)ProcessTag);
                            }

                            else
                                Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Writing_new_tag_UID_failed; });
                        }

                        else
                            Invoke((MethodInvoker)ProcessTag);
                    }
                    break;

                case rfidReaderArgs.ReaderNotify.RN_ReaderScanTimeout:
                case rfidReaderArgs.ReaderNotify.RN_ErrorDuringScan:
                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str__Scan_has_error; });
                    break;

                case rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost:
                    Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str__Scan_cancel_by_host; });
                    break;
            }
            Application.DoEvents();
        }

        private bool WriteLotId(string tagId, string lotId)
        {
            lotId = lotId.Trim().ToUpper();

            if ((tagId.Length == 10) && (tagId.StartsWith("30")) && (_myRegexUid.IsMatch(tagId)))
            {
                MessageBox.Show(ResStrings.str_Tag_not_compatible_for_writing,ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!SDK_SC_RfidReader.DeviceBase.SerialRFID.isStringValidToWrite(lotId))
            {
                MessageBox.Show(ResStrings.str_Tag_Invalid_Tag_ID, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            WriteCode codeResult = WriteCode.WC_Error;

            if (_currentEthernetDevice == null) // (USB) local reader
            {
                if ((_device.ConnectionStatus == ConnectionStatus.CS_Connected) && (_device.DeviceStatus == DeviceStatus.DS_Ready))
                    codeResult = _device.WriteNewUID(tagId, lotId);

                else
                    MessageBox.Show(ResStrings.str_device_not_ready, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            else
            {
                TcpIpClient tcpClient = new TcpIpClient();
                tcpClient.RequestWriteBlock(_currentEthernetDevice.IP_Server, _currentEthernetDevice.Port_Server, tagId, lotId, out codeResult);
            }


            switch (codeResult)
            {
                case WriteCode.WC_Error:
                    MessageBox.Show(ResStrings.strWriteStatusUnexpectedError);
                    break;

                case WriteCode.WC_TagNotDetected:
                    MessageBox.Show(ResStrings.strWriteStatusOperation_failed);
                    break;

                case WriteCode.WC_TagNotConfirmed:
                    MessageBox.Show(ResStrings.strWriteStatus_Tag_not_confirmed);
                    break;

                case WriteCode.WC_TagBlockedOrNotSupplied:
                    MessageBox.Show(ResStrings.strWriteStatus_Tag_blocked_or_not_well_supplied);
                    break;

                case WriteCode.WC_TagBlocked:
                    MessageBox.Show(ResStrings.strWriteStatus_Tag_blocked);
                    break;

                case WriteCode.WC_TagNotSupplied:
                    MessageBox.Show(ResStrings.strWriteStatus_Tag_not_well_supplied);
                    break;

                case WriteCode.WC_ConfirmationFailed:
                    MessageBox.Show(ResStrings.strWriteStatus_Updated_tag_confirmation_has_failed);
                    return true;

                case WriteCode.WC_Success:
                {
                    string initialUid = _db.GetInitialUID(tagId);
                    _db.AddUidHistory(string.IsNullOrEmpty(initialUid) ? tagId : initialUid, lotId);
                }
                    return true;
            }

            return false;
        }

        private void ScanDevice()
        {
            if (_currentEthernetDevice != null) // ethernet device : scan request, polling, tag blocks writing (if enabled by user)
            {
                TcpIpClient tcpClient = new TcpIpClient();
                string status;

                if (tcpClient.getStatus(_currentEthernetDevice.IP_Server, _currentEthernetDevice.Port_Server,
                        _currentEthernetDevice.SerialRFID, out status) != TcpIpClient.RetCode.RC_Succeed)
                {
                    MessageBox.Show(ResStrings.strErrorDevice, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DeviceStatus currentStatus = (DeviceStatus)Enum.Parse(typeof(DeviceStatus), status);

                if (currentStatus != DeviceStatus.DS_Ready)
                {
                    MessageBox.Show(ResStrings.strDeviceEthernet_device_is_not_ready, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                InventoryData lastInventoryData;

                do // loop while we don't get a successful scan with ONE tag scanned
                {
                    if (tcpClient.requestScan(_currentEthernetDevice.IP_Server, _currentEthernetDevice.Port_Server,
                            _currentEthernetDevice.SerialRFID) != TcpIpClient.RetCode.RC_Succeed) // scan starting has failed
                    {
                        MessageBox.Show(ResStrings.strdevice_Unable_to_start_scan);
                        return;
                    }

                    int tryIteration = 0;

                    do // loop while scan is not over (or if time limit excedeed [see Thread Sleep])
                    {
                        if (tryIteration > 4)
                        {
                            MessageBox.Show(ResStrings.strDevice_Scan_unexpectedly_long__please_retry, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        ++tryIteration;
                        Thread.Sleep(600); 

                        if (tcpClient.getStatus(_currentEthernetDevice.IP_Server, _currentEthernetDevice.Port_Server,
                         _currentEthernetDevice.SerialRFID, out status) != TcpIpClient.RetCode.RC_Succeed)
                        {
                            MessageBox.Show(ResStrings.strErrorDevice, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        currentStatus = (DeviceStatus)Enum.Parse(typeof(DeviceStatus), status);

                    } while (currentStatus == DeviceStatus.DS_InScan);

                    // scan terminated, now check if correct number of tag scanned (one)

                    if (tcpClient.requestGetLastScan(_currentEthernetDevice.IP_Server,
                        _currentEthernetDevice.Port_Server,
                        _currentEthernetDevice.SerialRFID, out lastInventoryData) != TcpIpClient.RetCode.RC_Succeed) // failed to get last inventorydata
                    {
                        MessageBox.Show(ResStrings.strDevice_Unable_to_get_last_scan_result, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (lastInventoryData == null) // failed to get last inventorydata
                    {
                        MessageBox.Show(ResStrings.strDevice_Unable_to_get_last_scan_result, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                } while (lastInventoryData.nbTagAll != 1);

                // we now have completed a scan with ONE tag
                string tagUid = lastInventoryData.listTagAll[0].ToString().Trim();
                toolStripStatusLabelInfo.Text = ResStrings.strScan_Completed_Tag_Scanned + tagUid;
                Invoke((MethodInvoker)delegate { textBoxTagID.Text = tagUid; });

                if (cbxAutoWrite.Checked)
                {
                    if (WriteLotId(tagUid, textBoxLotID.Text.Trim()))
                    {
                        Invoke((MethodInvoker)delegate { textBoxTagID.Text = textBoxLotID.Text; });
                        Invoke((MethodInvoker)ProcessTag);
                    }

                    else
                        Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.ErrorWriting_new_tag_UID_failed; });
                }

                else
                    Invoke((MethodInvoker)ProcessTag);
            }

            else // (USB) local reader scan : requests a scan and everything is processed in RFID events
            {
                if (_device == null) return;
                if ((_device.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                    (_device.DeviceStatus == DeviceStatus.DS_Ready))
                {
                    _device.ScanDevice();
                }

                else
                    MessageBox.Show(ResStrings.StrDeviceNotReadyOrNotConnected, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AutoFillBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_device == null) return;
            if (_device.DeviceStatus != DeviceStatus.DS_Ready)
            {
                checkBoxUseRFid.Checked = false;
                Thread.Sleep(1000);
            }
            if (_device.ConnectionStatus == ConnectionStatus.CS_Connected)
                _device.ReleaseDevice();

            _db.CloseDB();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<LiveDataForm>().Any())
            {
                MessageBox.Show(ResStrings.StrClick_Live_Data_will_be_closed_to_use_device);
                Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Click_Close_Live_Data_and_release_reader; });
                _ldf.Close();
                Thread.Sleep(1000);
            }

            Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Search_connected_device; });
            FindDevice();
        }

        private bool FindNextEmptyLine()
        {
            foreach (DataGridViewRow oRow in _dgv.Rows)
                oRow.Selected = false;

            foreach (DataGridViewRow oRow in _dgv.Rows)
            {
                if (!string.IsNullOrEmpty(oRow.Cells[_columnInfo[0].ToString()].Value.ToString())) continue;

                textBoxLotID.Text = oRow.Cells[_columnInfo[1].ToString()].Value.ToString();
                _selectedIndex = oRow.Index;
                _dgv.Rows[_selectedIndex].Selected = true;
                _dgv.FirstDisplayedScrollingRowIndex = _selectedIndex;

                return true;
            }

            return false;
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (FindNextEmptyLine())
            {
                if (checkBoxUseRFid.Checked)
                    new Thread(ScanDevice).Start();

                else
                    textBoxTagID.Focus();
            }

            else
            {
                textBoxLotID.Text = String.Empty;
                textBoxLotID.Focus();
            }
        }

        private void textBoxTagID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Return)
                ProcessTag();
        }

        private void checkBoxUseRFid_Click(object sender, EventArgs e)
        {
            if (!checkBoxUseRFid.Checked) 
            {  
                if (_device == null) return;
                if (_device.DeviceStatus != DeviceStatus.DS_Ready)
                {
                    checkBoxUseRFid.Checked = false;
                    Thread.Sleep(1000);
                }
                if (_device.ConnectionStatus == ConnectionStatus.CS_Connected)
                    _device.ReleaseDevice();
                return;
             }
            

            if (Application.OpenForms.OfType<LiveDataForm>().Any())
            {
                MessageBox.Show(ResStrings.StrClick_Live_Data_will_be_closed_to_use_device);
                Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Click_Close_Live_Data_and_release_reader; });
                _ldf.Close();
                Thread.Sleep(1000);
            }

            Invoke((MethodInvoker)delegate { toolStripStatusLabelInfo.Text = ResStrings.str_Search_connected_device; });
            FindDevice();

        }

        private void comboBoxDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedDeviceIndex = comboBoxDevice.SelectedIndex;

            int nbLocalDevices = (_arrayOfPluggedDevice == null) ? 0 : _arrayOfPluggedDevice.Length;
            //(_selectedDeviceIndex >= nbLocalDevices) => ethernet device selected
            buttonCreate.Visible = !(_selectedDeviceIndex >= nbLocalDevices);
            buttonDispose.Visible = buttonCreate.Visible;

            buttonConnect.Visible = !(buttonCreate.Visible);
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            int nbLocalDevices = (_arrayOfPluggedDevice == null) ? 0 : _arrayOfPluggedDevice.Length;

            if (_selectedDeviceIndex <= nbLocalDevices - 1) // Ethernet devices are added (in comboBox) after USB devices. Their index is greater than _arrayOfPluggedDevice length
            {
                MessageBox.Show(ResStrings.str_devicenotexist);
                return;
            }

            string selectedEthernetDeviceName = comboBoxDevice.Items[_selectedDeviceIndex].ToString();

            if (!_ethernetDevices.ContainsKey(selectedEthernetDeviceName))
            {
                MessageBox.Show(String.Format(ResStrings.strDeviceNotKnown, selectedEthernetDeviceName));
                return;
            }

            DeviceInfo deviceInfo = _ethernetDevices[selectedEthernetDeviceName];

            TcpIpClient tcpClient = new TcpIpClient();

            if (tcpClient.pingDevice(deviceInfo.IP_Server, deviceInfo.Port_Server, deviceInfo.SerialRFID) != TcpIpClient.RetCode.RC_Succeed)
            {
                MessageBox.Show(String.Format(ResStrings.strDeviceNotResponding, deviceInfo.SerialRFID, deviceInfo.IP_Server, deviceInfo.Port_Server));
                return;
            }

            bool spce2Available;

            if (tcpClient.RequestIsSPCE2Available(deviceInfo.IP_Server, deviceInfo.Port_Server, out spce2Available) != TcpIpClient.RetCode.RC_Succeed)
            {
                MessageBox.Show(String.Format(ResStrings.strDeviceNotResponding, deviceInfo.SerialRFID, deviceInfo.IP_Server, deviceInfo.Port_Server));
                return;
            }

            toolStripStatusLabelInfo.Text = String.Format(ResStrings.strDeviceNetworkError, deviceInfo.SerialRFID);

            //cbxAutoWrite.Visible = spce2Available;
            double fv = 0.0;
            tcpClient.RequestFirmwareVersion(deviceInfo.IP_Server, deviceInfo.Port_Server, out fv);
            cbxAutoWrite.Visible = fv > 2.54;

            _currentEthernetDevice = deviceInfo;
        }
    }
}
