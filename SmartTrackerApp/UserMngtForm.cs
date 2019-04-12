using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using SDK_SC_Fingerprint;
using SDK_SC_AccessControl;

using DBClass;
using DataClass;
using SDK_SC_RfidReader;
using smartTracker.Properties;
using TcpIP_class;

namespace smartTracker
{
    public partial class UserMngtForm : Form
    {
        readonly MainDBClass _db = new MainDBClass();
        clBadgeReader _badgeReader = null;
        UserClassTemplate[] _userArray = null;
        FingerPrintClass _myFinger = null;
        UserClassTemplate _userInEnroll;

        private readonly List<TcpIpClient>  _tcpArmClients = new List<TcpIpClient>();

        bool _bSaved = true;
        bool _bComeFromListBox = false;

        int _fingerAlert;
        int _maxItem;
        double _maxValue;

        DataTable _dt;
        private DataTable _dtUserGrant;
        public UserMngtForm()
        {
            InitializeComponent();  
        }

        private void UserMngtForm_Load(object sender, EventArgs e)
        {         

            if (_myFinger != null)
            {
                _myFinger.ReleaseFingerprint();
                _myFinger = null;
            }


            _dtUserGrant = new DataTable();
            _dtUserGrant.Columns.Add("UserGrantID", typeof(int));
            _dtUserGrant.Columns.Add("UserGrantName", typeof(string));           
            _dtUserGrant.Rows.Add(0, getEnumDesc.GetEnumDescription(UserGrant.UG_NONE));
            _dtUserGrant.Rows.Add(1, getEnumDesc.GetEnumDescription(UserGrant.UG_MASTER));
            _dtUserGrant.Rows.Add(2, getEnumDesc.GetEnumDescription(UserGrant.UG_SLAVE));
            _dtUserGrant.Rows.Add(3, getEnumDesc.GetEnumDescription(UserGrant.UG_MASTER_AND_SLAVE));
           
            

            DataGridViewComboBoxColumn dgc = (DataGridViewComboBoxColumn)dataGridViewGrant.Columns[0];
            dgc.DataSource = _dtUserGrant;
            dgc.DisplayMember = "UserGrantName";
            dgc.ValueMember = "UserGrantID";
            dgc.DataPropertyName = "UserGrantID";
            

            _myFinger = new FingerPrintClass();

            comboBoxFinger.SelectedIndex = 0;
            _db.OpenDB();
            _userInEnroll = new UserClassTemplate();
            UpdateListBoxUser();
            CreateGrantTable();           

        }

        private void CreateGrantTable()
        {
            _dt = new DataTable("TableGrant");
            _dt.Columns.Add("Device Serial", typeof(string));
            _dt.Columns.Add(ResStrings.str_Device_Name, typeof(string));
            _dt.Columns.Add(ResStrings.str_Device_Type, typeof(string));

           

            DeviceInfo[] di = _db.RecoverAllDevice();
            if (di != null)
            {
                foreach (DeviceInfo dev in di)
                {
                    //dt.Rows.Add(dev.SerialRFID,dev.DeviceName,dev.deviceType.ToString());
                    _dt.Rows.Add(dev.SerialRFID, dev.DeviceName, getEnumDesc.GetEnumDescription(dev.deviceType));                            
                }                
            }

            dataGridViewGrant.DataSource = null;
            dataGridViewGrant.DataSource = _dt;

            for (int i = 0; i < dataGridViewGrant.RowCount; i++)
            {
                //dataGridViewGrant.Rows[i].Cells["Master"].Value = true;                 
                dataGridViewGrant.Rows[i].Cells["DoorGranted"].Value = 3;               
                
            }
            comboBoxRemoteReader.Items.Clear();
            cbArmReader.Items.Clear();
            _tcpArmClients.Clear();
            DeviceInfo[] tmpDeviceArray = _db.RecoverDevice(false);
            if (tmpDeviceArray != null)
            {
                foreach (DeviceInfo dev in tmpDeviceArray)
                {
                    comboBoxRemoteReader.Items.Add(dev.DeviceName);

                    //Add remote enroll
                    TcpIpClient tmpTcp = new TcpIpClient();
                    try
                    {
                        tmpTcp._tcpArmDevice = new TcpArmDevice(dev.IP_Server,dev.Port_Server);
                        tmpTcp._tcpArmDevice.DeviceEvent += _tcpArmDevice_DeviceEvent;
                        _tcpArmClients.Add(tmpTcp);
                        cbArmReader.Items.Add(dev.DeviceName + " (" + dev.IP_Server + ")");
                    }
                    catch (Exception)
                    {
                        if (tmpTcp._tcpArmDevice!=null)
                            tmpTcp._tcpArmDevice.Release();
                    }
                }
            }
            if (comboBoxRemoteReader.Items.Count > 0)
            {
                comboBoxRemoteReader.Enabled = true;
                comboBoxRemoteReader.SelectedIndex = 0;
                buttonBadge.Enabled = true;
             }
            if (cbArmReader.Items.Count > 0)
            {
                cbArmReader.SelectedIndex = 0;
            }

            cbxFinger.DataSource = Enum.GetValues(typeof(FingerIndexValue));
            cbxFinger.SelectedIndex = 10;

        }

        void _tcpArmDevice_DeviceEvent(SDK_SC_RfidReader.rfidReaderArgs args)
        {
            switch (args.RN_Value)
            {
                case rfidReaderArgs.ReaderNotify.RN_EnrollmentSample:
                    int sample = int.Parse(args.Message);
                    backgroundWorkerEnroll.ReportProgress(sample);
                    break;
            }
        }

        private void UpdateGrantTable(UserClassTemplate uct)
        {  
            for (int i = 0; i < dataGridViewGrant.RowCount; i++)
            {
                //dataGridViewGrant.Rows[i].Cells["Master"].Value = false;  
                dataGridViewGrant.Rows[i].Cells["DoorGranted"].Value = 0;
               
            }
           if (uct == null) return;
            
            /*
            string[] allowedSerialMaster = db.recoverUserGrant(uct);            
            if (allowedSerialMaster == null) return;
            foreach (string serial in allowedSerialMaster)
            {
                for (int i = 0; i < dataGridViewGrant.RowCount; i++)
                {
                    if (dataGridViewGrant.Rows[i].Cells["Device Serial"].Value.Equals(serial))
                    {
                        dataGridViewGrant.Rows[i].Cells["Master"].Value = true;
                        break;
                    }
                }
            } */

           DeviceGrant[] allowedSerialMaster = _db.recoverUserGrantFull(uct);
           if (allowedSerialMaster == null) return;

           foreach (DeviceGrant dg in allowedSerialMaster)
           {
               for (int i = 0; i < dataGridViewGrant.RowCount; i++)
               {
                   if (dataGridViewGrant.Rows[i].Cells["Device Serial"].Value.Equals(dg.serialRFID))
                   {                    
                       dataGridViewGrant.Rows[i].Cells["DoorGranted"].Value = (int)dg.userGrant;
                       break;
                   }
               }
           } 


        }

        

        // Store user
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (!GetDouble(out _maxValue))
                _maxValue = 0.0;

            if (!_bComeFromListBox)
            {
                if (_userArray != null) // TODO : CLEAN UP "UserArray" use in order to not need this nullCheck
                {
                    foreach (UserClassTemplate us in _userArray)
                    {
                        string strUser = string.Format("{0} {1}", us.firstName, us.lastName);
                        string strUserinEnrol = string.Format("{0} {1}", _userInEnroll.firstName, _userInEnroll.lastName);
                        if (strUser == strUserinEnrol)
                        {
                            MessageBox.Show(ResStrings.str_User_already_Exist___Please_change_Firstname_or_LastName, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }
            }
           

            if (_db.StoreUser(_userInEnroll))
            {
                StoreGrant();
                if (!int.TryParse(textBoxRemoveItem.Text, out _maxItem)) _maxItem = 0;
                _db.UpdateUserFingerAlert(_userInEnroll.firstName, _userInEnroll.lastName, _fingerAlert);
                _db.UpdateUserMaxRemovedItem(_userInEnroll.firstName, _userInEnroll.lastName, _maxItem);
                _db.UpdateUserMaxRemoveValue(_userInEnroll.firstName, _userInEnroll.lastName, (float)_maxValue);

                Reset();
                UpdateListBoxUser();
                _bSaved = true;
                MessageBox.Show(ResStrings.str_DATA_SAVED, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
           
        }

        //Enroll
        private void toolStripButton2_Click(object sender, EventArgs e)
        {

            if ((!string.IsNullOrEmpty(textBoxFirstName.Text)) && (!string.IsNullOrEmpty(textBoxLastName.Text)))
            {  
               
                _userInEnroll.firstName = textBoxFirstName.Text;
                _userInEnroll.lastName = textBoxLastName.Text;
                _userInEnroll.template = _myFinger.EnrollUser(null, _userInEnroll.firstName, _userInEnroll.lastName, _userInEnroll.template, false);

                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream mem = new MemoryStream(Convert.FromBase64String(_userInEnroll.template));
                UserClass theUser = new UserClass();
                theUser = (UserClass)bf.Deserialize(mem);

                int nIndex = 0;
                foreach (string template in theUser.strFingerprint)
                {
                    if (string.IsNullOrEmpty(template))
                        _userInEnroll.isFingerEnrolled[nIndex] = false;
                    else
                        _userInEnroll.isFingerEnrolled[nIndex] = true;
                    nIndex++;
                }
                if (_db.StoreUser(_userInEnroll))
                {
                    StoreGrant();
                    _bSaved = true;
                    Reset();
                    MessageBox.Show(ResStrings.str_DATA_SAVED, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                      
                }

                UpdateGridFinger();
                UpdateListBoxUser();
            }
            else
                MessageBox.Show(ResStrings.str_Please_enter_FirstName_and_LastName_field_before_enroll,ResStrings.strInfo,MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void StoreGrant()
        {
            for (int i = 0; i < dataGridViewGrant.RowCount; i++)
            {
                dataGridViewGrant.EndEdit();
                if ((int)dataGridViewGrant.Rows[i].Cells["DoorGranted"].Value != 0)
                {
                    var formattedValue = dataGridViewGrant.Rows[i].Cells["Device Serial"].FormattedValue;
                    if (formattedValue != null)
                        _db.StoreGrant(_userInEnroll, formattedValue.ToString(), null, (UserGrant)dataGridViewGrant.Rows[i].Cells["DoorGranted"].Value);
                }
                else
                {
                    var formattedValue = dataGridViewGrant.Rows[i].Cells["Device Serial"].FormattedValue;
                    if (formattedValue != null)
                        _db.DeleteUserGrant(_userInEnroll, formattedValue.ToString());
                }
            }
        }

        private void UpdateGridFinger()
        {

            string[] strFingerName =
            {ResStrings.strr_Right_Thumb ,ResStrings.str_Right_Index,ResStrings.str_Right_Middle,ResStrings.str_Right_Ring,ResStrings.str_Right_Little,
                ResStrings.str_Left_Thumb,ResStrings.str_Left_Index,ResStrings.str_Left_Middle,ResStrings.str_Left_Ring,ResStrings.str_Left_Little};

      
            dataGridViewFinger.Rows.Clear();          
        
            for (int nIndex = 0; nIndex < 10; nIndex++) 
                dataGridViewFinger.Rows.Insert(nIndex, strFingerName[nIndex], _userInEnroll.isFingerEnrolled[nIndex]);
        }

        private void UpdateListBoxUser()
        {
            listBoxUser.Items.Clear();
            UserClassTemplate[] tmpUserArray  =  _db.RecoverUser();
            if (tmpUserArray != null)
            {
                _userArray = new UserClassTemplate[tmpUserArray.Length];
                tmpUserArray.CopyTo(_userArray, 0);

                foreach (UserClassTemplate us in _userArray)
                {
                    string strUser = string.Format("{0} {1}", us.firstName, us.lastName);
                    listBoxUser.Items.Add(strUser);
                }
            }
        }

        private void listBoxUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxUser.SelectedIndex >= 0)
            {
                _bComeFromListBox = true;
                groupBoxUserCtrl.Enabled = true;
                toolStripButtonEnrollFingerprint.Enabled = true;
                gpBx.Enabled = true;
                toolStripButtonDelete.Enabled = true;
                int rowSelected = listBoxUser.SelectedIndex;
                _userInEnroll = new UserClassTemplate();
                _userInEnroll = _userArray[rowSelected];

                textBoxFirstName.ReadOnly = true;
                textBoxLastName.ReadOnly = true;
                textBoxFirstName.Text = _userInEnroll.firstName;
                textBoxLastName.Text = _userInEnroll.lastName;
                textBoxReaderCard.Text = _userInEnroll.BadgeReaderID;
                UpdateGridFinger();

                UpdateGrantTable(_userInEnroll);

                _fingerAlert = _db.getUserFingerAlert(textBoxFirstName.Text,textBoxLastName.Text);
                comboBoxFinger.SelectedIndex = _fingerAlert + 1;

                _maxItem = _db.getUserMaxRemovedItem(textBoxFirstName.Text, textBoxLastName.Text);
                if (_maxItem >= 0) textBoxRemoveItem.Text = _maxItem.ToString(CultureInfo.InvariantCulture);
                else textBoxRemoveItem.Text = null;

                _maxValue = _db.getUserMaxRemoveValue(textBoxFirstName.Text, textBoxLastName.Text);
                if (_maxValue >= 0) textBoxRemoveValue.Text = _maxValue.ToString(CultureInfo.InvariantCulture);
                else textBoxRemoveValue.Text = null;

            }

        }
       
        private void Reset()
        {
            _bSaved = true;
            _bComeFromListBox = false;
            listBoxUser.SelectedIndex = -1;
            listBoxUser.Enabled = true;
            groupBoxUserCtrl.Enabled = false;
            _userInEnroll = new UserClassTemplate();
            textBoxFirstName.Text = null;
            textBoxLastName.Text = null;
            toolStripButtonEnrollFingerprint.Enabled = false;
            gpBx.Enabled = false;
            toolStripButtonDelete.Enabled = false;
            toolStripButtonApply.Enabled = false;
            textBoxFirstName.ReadOnly = false;
            textBoxLastName.ReadOnly = false;
            textBoxReaderCard.Text = null;
            textBoxRemoveItem.Text = null;
            textBoxRemoveValue.Text = null;
            comboBoxFinger.SelectedIndex = 0;

            UpdateGridFinger();
        }

        private void toolStripButtonReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void toolStripButtonCreateNew_Click(object sender, EventArgs e)
        {
            Reset();
            CreateGrantTable();
            groupBoxUserCtrl.Enabled = true;
            listBoxUser.SelectedIndex = -1;
            listBoxUser.Enabled = false;
            toolStripButtonEnrollFingerprint.Enabled = true;
            gpBx.Enabled = true;
            _bComeFromListBox = false;
        }

        private void textBoxFirstName_TextChanged(object sender, EventArgs e)
        {
            _userInEnroll.firstName = textBoxFirstName.Text;
           
            if ((!string.IsNullOrEmpty(textBoxFirstName.Text)) && (!string.IsNullOrEmpty(textBoxLastName.Text)))
            {
                toolStripButtonApply.Enabled = true;
                _bSaved = _bComeFromListBox;
            }
        }

        private void textBoxLastName_TextChanged(object sender, EventArgs e)
        {
            _userInEnroll.lastName = textBoxLastName.Text;
             if ((!string.IsNullOrEmpty(textBoxFirstName.Text)) && (!string.IsNullOrEmpty(textBoxLastName.Text)))
            {
                toolStripButtonApply.Enabled = true;
                _bSaved = _bComeFromListBox;
            }
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if (!_db.DeleteUser(_userInEnroll.firstName, _userInEnroll.lastName))
            {
                MessageBox.Show(ResStrings.str_Error_During_Deleting_user___User_not_deleted, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                _db.DeleteUserGrant(_userInEnroll.firstName, _userInEnroll.lastName, null);
                Reset();
                UpdateListBoxUser();
            }
        }

        private void toolStripButtonExit_Click(object sender, EventArgs e)
        {
            if (_badgeReader != null)
                _badgeReader.closePort();

            if (!_bSaved)
                MessageBox.Show(ResStrings.str_DATA_NOT_SAVED, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                //this.BeginInvoke((MethodInvoker)delegate { this.Close(); });
                {
                    DeviceInfo[] di = _db.RecoverAllDevice();
                    if (di != null)
                    {
                        foreach (DeviceInfo dev in di)
                        {
                            if (!string.IsNullOrEmpty(dev.IP_Server))
                            {
                                if (CheckIpAddr(dev.IP_Server))
                                {
                                    TcpIpClient tcp = new TcpIpClient();
                                    tcp.setUserRefresh(dev.IP_Server, dev.Port_Server);
                                }
                            }
                        }
                    }

                    _myFinger.ReleaseFingerprint();
                    _db.CloseDB();
                    Close();
                }
        }

        public bool CheckIpAddr(string ipAddress)
        {
            return Regex.IsMatch(ipAddress,
            @"^(25[0-5]|2[0-4]\d|[0-1]?\d?\d)(\.(25[0-5]|2[0-4]\d|[0-1]?\d?\d)){3}$");
        }

        private void UserMngtForm_FormClosing(object sender, FormClosingEventArgs e)
        {
         
        }

        private void textBoxReaderCard_TextChanged(object sender, EventArgs e)
        {
            _userInEnroll.BadgeReaderID = textBoxReaderCard.Text;
        }

        private void comboBoxFinger_SelectedIndexChanged(object sender, EventArgs e)
        {
            _fingerAlert = comboBoxFinger.SelectedIndex - 1;
        }

        private void textBoxRemoveItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!_bDelete)
            {

                if (!Regex.IsMatch(e.KeyChar.ToString(CultureInfo.InvariantCulture), "\\d+"))
                    e.Handled = true;
            }
        }

        private void textBoxRemoveValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!_bDelete)
            {
                if (!Regex.IsMatch(e.KeyChar.ToString(CultureInfo.InvariantCulture), "[\\d\\.\\s,+]"))
                    e.Handled = true;
            }
        }
        private bool GetDouble (out double valret)
        {
            valret = -1;
            string val = textBoxRemoveValue.Text.Replace(" ", "");
            val = val.Replace("'", "");

            if (val.Contains(",") && val.Contains("."))
            {
                MessageBox.Show(ResStrings.str_Use_only_one_separator,ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                return false;
            }
          

             CultureInfo culture;

            if (textBoxRemoveValue.Text.Contains("."))
            {

                if (!Regex.IsMatch(val, "^[0-9]{0,10}\\.?[0-9]{0,2}$"))
                {
                    MessageBox.Show(ResStrings.str_Enter_a_valid_removed_value,ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                // Utilisation de InvariantCulture si présence du . comme séparateur décimal. 
                culture = CultureInfo.InvariantCulture;
            }
            else
            {
                 if (!Regex.IsMatch(val,"^[0-9]{0,10}\\,?[0-9]{0,2}$"))
                 {
                     MessageBox.Show(ResStrings.str_Enter_a_valid_removed_value,ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                     return false;
                 }
                // Utilisation de CurrentCulture sinon (utilisation de , comme séparateur décimal).
                culture = CultureInfo.CurrentCulture;
            }

            // Conversion de la chaîne en double, en utilisant la culture correspondante.            
            if (double.TryParse(val, NumberStyles.Number, culture, out valret))
                return true;
            return false;
        }
        private bool _bDelete = false;
        private void textBoxRemoveItem_KeyDown(object sender, KeyEventArgs e)
        {
            if ( (e.KeyCode == Keys.Delete) || (e.KeyCode == Keys.Back))
                _bDelete = true;
            else
                _bDelete = false;
        }

        private void textBoxRemoveValue_Leave(object sender, EventArgs e)
        {
            string val = textBoxRemoveValue.Text.Replace(" ","");
            val = val.Replace("'", "");
            if (val.Contains(",") && val.Contains("."))
            {
                MessageBox.Show(ResStrings.str_Use_only_one_separator_for_double, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);

                textBoxRemoveValue.SelectAll();
                textBoxRemoveValue.Focus();
                return;
            }

            if (textBoxRemoveValue.Text.Contains("."))
            {

                if (!Regex.IsMatch(val, "^[0-9]*\\.?[0-9]{0,2}$"))
                {
                    MessageBox.Show(ResStrings.str_Enter_a_valid_removed_value, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBoxRemoveValue.SelectAll();
                    textBoxRemoveValue.Focus();
                }
                else
                {
                    double tmpVal;
                    if (double.TryParse(textBoxRemoveValue.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out tmpVal))
                        textBoxRemoveValue.Text = string.Format("{0:N}", tmpVal);
                }
            }
            else
            {
                if (!Regex.IsMatch(val, "^[0-9]*\\,?[0-9]{0,2}$"))
                {
                    MessageBox.Show(ResStrings.str_Enter_a_valid_removed_value, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBoxRemoveValue.SelectAll();
                    textBoxRemoveValue.Focus();
                }
                else
                {
                    double tmpVal;
                    if (double.TryParse(textBoxRemoveValue.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out tmpVal))
                        textBoxRemoveValue.Text = string.Format("{0:N}", tmpVal);
                }
            }

           
        }

        private void textBoxCom_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Return)
            {
                CreateBadgeReader(textBoxCom.Text);
            }
        }


        private void CreateBadgeReader(string port)
        {
           
            if (_badgeReader != null)
                _badgeReader.closePort();

            if (radioButtonLF.Checked)
                _badgeReader = new clBadgeReader(AccessBagerReaderType.RT_LF, port, "xxxxxxxx",true);
            else
                _badgeReader = new clBadgeReader(AccessBagerReaderType.RT_HF, port, "xxxxxxxx", true);
            _badgeReader.NotifyBadgeReaderEvent += new NotifyHandlerBadgeReaderDelegate(badgeReader_NotifyBadgeReaderEvent);
        }

        private void badgeReader_NotifyBadgeReaderEvent(Object sender, string badgeId, string deviceSerial)
        {
            Invoke((MethodInvoker)delegate { textBoxReaderCard.Text = badgeId; });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBoxRemoteReader.Text))
            {
                DeviceInfo[] tmpDeviceArray = _db.RecoverDevice(false);
                if (tmpDeviceArray != null)
                {
                    foreach (DeviceInfo dev in tmpDeviceArray)
                    {
                         if (dev.DeviceName == comboBoxRemoteReader.Text)
                         {
                             string badge;
                             TcpIpClient tcp = new TcpIpClient();
                             TcpIpClient.RetCode ret = tcp.getLastBadge(dev.IP_Server,dev.Port_Server , out badge);
                             if (ret == TcpIpClient.RetCode.RC_Succeed)
                                   Invoke((MethodInvoker)delegate { textBoxReaderCard.Text = badge; });
                             else
                                 MessageBox.Show(string.Format(ResStrings.str_Unable_to_get_Badge_from__0, dev.DeviceName), ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                             break;
                         }                         
                     }
                 }          
              
            }
        }

        private void btEnroll_Click(object sender, EventArgs e)
        {
            if (cbxFinger.SelectedIndex == 10)
            {
                MessageBox.Show("Select A finger To enroll");
                return;
            }

            if ((!string.IsNullOrEmpty(textBoxFirstName.Text)) && (!string.IsNullOrEmpty(textBoxLastName.Text)))
            {

                _userInEnroll.firstName = textBoxFirstName.Text;
                _userInEnroll.lastName = textBoxLastName.Text;

                string strlogin = _userInEnroll.firstName + "_" + _userInEnroll.lastName;
                 List<string> lstInactiveUser = _tcpArmClients[cbArmReader.SelectedIndex]._tcpArmDevice.GetUnregisteredUsers();
                if (lstInactiveUser.Contains(strlogin))
                {
                     _tcpArmClients[cbArmReader.SelectedIndex]._tcpArmDevice.UpdatePermission(strlogin, UserGrant.UG_MASTER_AND_SLAVE);
                }
                DeviceGrant ExistingUser = _tcpArmClients[cbArmReader.SelectedIndex]._tcpArmDevice.GetUserByName(strlogin);
                

                List<object> arguments = new List<object>();
                arguments.Add(cbArmReader.SelectedIndex);
                arguments.Add(strlogin);
                arguments.Add(cbxFinger.SelectedIndex);


                if (ExistingUser != null)
                {
                    backgroundWorkerEnroll.RunWorkerAsync(arguments);
                }
                else
                {
                    DeviceGrant dg = new DeviceGrant();
                    dg.user = new UserClassTemplate();
                    dg.user = _userInEnroll;
                    dg.userGrant = UserGrant.UG_MASTER_AND_SLAVE;
                    if (_tcpArmClients[cbArmReader.SelectedIndex]._tcpArmDevice.AddUser(dg))
                    {

                        backgroundWorkerEnroll.RunWorkerAsync(arguments);
                    }
                    else
                    {
                        updateStatus("Unable to create user");
                    }
                }
               
            }
            else
                MessageBox.Show(ResStrings.str_Please_enter_FirstName_and_LastName_field_before_enroll, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void backgroundWorkerEnroll_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            updateStatus("Press 4 times Fp To enroll");
            List<object> genericlist = e.Argument as List<object>;
            int DeviceIndex = (int) genericlist[0];
            string Login = (string) genericlist[1];
            FingerIndexValue finger = (FingerIndexValue) genericlist[2];

            _tcpArmClients[DeviceIndex]._tcpArmDevice.EnrollFinger(Login,finger, null);

        }

        private void backgroundWorkerEnroll_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            updateStatus("Enroll Sample received  : " + e.ProgressPercentage + "/4 to capture");
            pBarEnroll.Value = e.ProgressPercentage;
        }

        private void backgroundWorkerEnroll_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            string strlogin = _userInEnroll.firstName + "_" + _userInEnroll.lastName;
            DeviceGrant ExistingUser = _tcpArmClients[cbArmReader.SelectedIndex]._tcpArmDevice.GetUserByName(strlogin);
            if (ExistingUser != null)
            {
                if (_db.StoreUser(ExistingUser.user))
                {
                    StoreGrant();
                    _bSaved = true;
                    Reset();
                    MessageBox.Show(ResStrings.str_DATA_SAVED, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

                UpdateGridFinger();
                UpdateListBoxUser();
            }
        }
        private void updateStatus(string msg)
        {
            Invoke((MethodInvoker)(() => lblStatus.Text = DateTime.Now.ToLongTimeString() + " : " + msg));
            Invoke((MethodInvoker)(() => lblStatus.Refresh()));
            Application.DoEvents();
        }
    }

    public class Choice
    {
        public string Name { get; private set; }
        public int Value { get; private set; }
        public Choice(string name, int value)
        {
            Name = name;
            Value = value;
        }

        private static readonly List<Choice> PossibleChoices = new List<Choice>
    {
        new Choice(UserGrant.UG_NONE.ToString(), 0),
        new Choice(UserGrant.UG_MASTER.ToString(), 1),
        new Choice(UserGrant.UG_SLAVE.ToString(), 2),
        new Choice(UserGrant.UG_MASTER_AND_SLAVE.ToString(), 3),

    };

        public static List<Choice> GetChoices()
        {
            return PossibleChoices;
        }
    }

}
