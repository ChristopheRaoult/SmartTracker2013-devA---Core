using System;
using System.Windows.Forms;

using DataClass;
using SDK_SC_RFID_Devices;
using TcpIP_class;

namespace smartTracker
{
    public partial class RemoteUser : Form
    {
        readonly deviceClass _dc;
        TcpIpClient _tcp;
        UserClassTemplate[] _userArrayAllowed = null;       

        public RemoteUser(deviceClass dc)
        {
            InitializeComponent();
            _dc = dc;
        }

        private void remoteUser_Load(object sender, EventArgs e)
        {
            UpdateListBoxUser();
        }

        private void UpdateListBoxUser()
        {
            _userArrayAllowed = null;

            listBoxUser.Items.Clear();
            _tcp = new TcpIpClient();     
            _tcp.getUserList(_dc.infoDev.IP_Server, _dc.infoDev.Port_Server, _dc.infoDev.SerialRFID, out _userArrayAllowed);
           
            if (_userArrayAllowed != null)
            {
                foreach (UserClassTemplate us in _userArrayAllowed)
                {
                    string strUser = string.Format("{0} {1}", us.firstName, us.lastName);
                    listBoxUser.Items.Add(strUser);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBoxUser.SelectedIndex >= 0)
            {
                int rowSelected = listBoxUser.SelectedIndex;
                _tcp.deleteUser(_dc.infoDev.IP_Server, _dc.infoDev.Port_Server, _userArrayAllowed[rowSelected].firstName,
                    _userArrayAllowed[rowSelected].lastName, _dc.infoDev.SerialRFID);

                _tcp.deleteUserGrant(_dc.infoDev.IP_Server, _dc.infoDev.Port_Server, _userArrayAllowed[rowSelected].firstName,
                    _userArrayAllowed[rowSelected].lastName, _dc.infoDev.SerialRFID);
                UpdateListBoxUser();
            }
        }
    }
}
