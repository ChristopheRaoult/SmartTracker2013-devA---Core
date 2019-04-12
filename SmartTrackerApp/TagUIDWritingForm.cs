using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using DataClass;
using SDK_SC_RFID_Devices;

using DBClass;
using BrightIdeasSoftware;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class TagUidWritingForm : Form
    {
        private readonly string _previousUid;
        private readonly RFID_Device _currentDevice;
        private readonly bool _isEthernetDevice = false;
        private readonly deviceClass _currentDeviceClass; // not used if (USB) local reader
        private DataTable _tbHistory;
        MainDBClass _db;
        List<UidWriteHistory> _listUidHistory = null;
        private string _initialUid = null;
        /// <summary>
        /// Default constructor for (USB) local readers
        /// </summary>
        /// <param name="previousUid">Given tag previous ID</param>
        /// <param name="currentDevice">Current local reader (the one that scanned the given tag)</param>
        public TagUidWritingForm(string previousUid, RFID_Device currentDevice)
        {
            InitializeComponent();
            textBoxPreviousUID.Text = previousUid;

            _previousUid = previousUid;
            _currentDevice = currentDevice;

            comboBoxSelWrite.SelectedIndex = 0;
        }


        /// <summary>
        /// Defaut constructor for (Ethernet) remote readers
        /// </summary>
        /// <param name="previousUid">Given tag previous ID</param>
        /// <param name="currentDeviceClass">deviceClass instance of the TCP device</param>
        public TagUidWritingForm(string previousUid, deviceClass currentDeviceClass)
            : this(previousUid, currentDeviceClass.rfidDev)
        {
            _isEthernetDevice = true;
            _currentDeviceClass = currentDeviceClass;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            WriteCode codeResult = WriteCode.WC_Error;
            string newUid = textBoxNewUID.Text.Trim();

            string initialUid = _db.GetInitialUID(newUid);
            if (!string.IsNullOrEmpty(initialUid))
            {
                List<UidWriteHistory> tmpList = _db.GetUidHistory(initialUid);

                if (tmpList != null)
                {
                    if (tmpList[0]._writtenUid == newUid)
                    {
                        MessageBox.Show(ResStrings.str_UID_already_in_use_Please_Change_UID);
                        return;
                    }
                }
            }

           
            if (comboBoxSelWrite.SelectedIndex == 0)
            {
                if ((!SDK_SC_RfidReader.DeviceBase.SerialRFID.isStringValidToWrite(textBoxNewUID.Text.Trim()) || (textBoxNewUID.Text.Length > 17)))
                {
                    MessageBox.Show(ResStrings.str_Invalid_Tag_ID, ResStrings.strInfo, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }
            else if (comboBoxSelWrite.SelectedIndex == 1)
            {
                System.Text.RegularExpressions.Regex myRegex = new Regex(@"^[0-9]+$");
                if ((!myRegex.IsMatch(textBoxNewUID.Text)) || (textBoxNewUID.Text.Length > 12))
                {
                    MessageBox.Show(ResStrings.str_Invalid_Tag_ID, ResStrings.strInfo, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }

            if (!_isEthernetDevice) // (USB) local reader
            {
                if ((_currentDevice.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                    (_currentDevice.DeviceStatus == DeviceStatus.DS_Ready))

                   
                     if (comboBoxSelWrite.SelectedIndex == 0) //SPCE2 With family
                        codeResult = _currentDevice.WriteNewUidWithFamily(_previousUid, newUid);
                    else if (comboBoxSelWrite.SelectedIndex == 1)  // SPCE2 Decimal
                        codeResult = _currentDevice.WriteNewUidDecimal(_previousUid, newUid);
                else
                {
                    MessageBox.Show(ResStrings.str_RFID_device_not_ready);
                    return;
                }
            }

            else // (Ethernet) Remote reader
            {
                TcpIP_class.TcpIpClient tcpClient = new TcpIP_class.TcpIpClient();
                tcpClient.RequestWriteBlock(_currentDeviceClass.infoDev.IP_Server, _currentDeviceClass.infoDev.Port_Server, _previousUid, newUid, out codeResult, comboBoxSelWrite.SelectedIndex);
                
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
                    MessageBox.Show(ResStrings.str_Tag_blocked_or_not_well_supplied__Operation_failed);
                    break;

                case WriteCode.WC_TagBlocked:
                    MessageBox.Show(ResStrings.str_Tag_blocked__Operation_failed);
                    break;

                case WriteCode.WC_TagNotSupplied:
                    MessageBox.Show(ResStrings.str_Tag_not_well_supplied__Operation_failed);
                    break;

                case WriteCode.WC_ConfirmationFailed:
                    MessageBox.Show(ResStrings.str_Updated_tag_confirmation_has_failed);
                    break;

                case WriteCode.WC_Success:

                    _db.AddUidHistory(string.IsNullOrEmpty(_initialUid) ? _previousUid : _initialUid, newUid);
                    if (_initialUid != null)
                        UpdateHistoryTable(_initialUid);
                    Application.DoEvents();
                    Thread.Sleep(200);
                    MessageBox.Show(ResStrings.str_Tag_UID_succesfully_updated_);

                    break;
            }
            Close();
        }
               
        private void textBoxNewUID_TextChanged(object sender, EventArgs e)
        {
            textBoxNewUID.Text = textBoxNewUID.Text.ToUpper();
            if (textBoxNewUID.Text.Contains("@")) textBoxNewUID.Text = string.Empty;
            if (textBoxNewUID.Text.Length > 0)
            {
                char testedChar = textBoxNewUID.Text[textBoxNewUID.Text.Length - 1];

                int nIndex = SDK_SC_RfidReader.DeviceBase.SerialRFID.GetAlphaIndexRW(testedChar);
                if (nIndex == -1)
                    textBoxNewUID.Text = textBoxNewUID.Text.Substring(0, textBoxNewUID.Text.Length - 1);
                

            }
            textBoxNewUID.SelectionStart = textBoxNewUID.Text.Length;
        }

        private void InitHistoryTable()
        {
            _tbHistory = new DataTable();
            _tbHistory.Columns.Add(ResStrings.str_TagUID, typeof(string));
            _tbHistory.Columns.Add(ResStrings.str_Change_UID_Date, typeof(string));           

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
                    ol.FillsFreeSpace = true;                  

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
                _listUidHistory = null;
                string initialUid = _db.GetInitialUID(tagUid);
                if (!string.IsNullOrEmpty(initialUid))
                    _listUidHistory = _db.GetUidHistory(initialUid);

                InitHistoryTable();
                if (_listUidHistory != null)
                {

                    foreach (UidWriteHistory uwh  in _listUidHistory)
                    {
                        DataRow rowToadd = _tbHistory.NewRow();
                        rowToadd[0] = uwh._writtenUid;
                        rowToadd[1] = uwh._writtenDate;                  
                        _tbHistory.Rows.Add(rowToadd);

                        _initialUid = uwh._initialUid; // last one is initial

                    }
                    if (!string.IsNullOrEmpty(_initialUid))
                    {
                        DataRow rowInitial = _tbHistory.NewRow();
                        rowInitial[0] = _initialUid;
                        _tbHistory.Rows.Add(rowInitial);
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
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
        }

        private void TagUIDWritingForm_Load(object sender, EventArgs e)
        {
            _db = new MainDBClass();
            _db.OpenDB();
            if (!string.IsNullOrEmpty(_previousUid))
                UpdateHistoryTable(_previousUid);
        }

        private void TagUIDWritingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _db.CloseDB();
        }
    }
}
