using System.Windows.Forms;
using DataClass;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class ScanInProgress : Form
    {
        public DeviceInfo di = null;
        public ScanInProgress()
        {
            InitializeComponent();
            
                StartPosition = FormStartPosition.Manual;
                Top = 110;
                Left = 800;              
        }
        public void SetInfo (DeviceInfo di)
        {
            if (di != null)
            {
                this.di = di;
                if (di.bLocal == 1)
                    labelInfo.Text = string.Format(ResStrings.str_DeviceFormat, di.SerialRFID);
                else
                    labelInfo.Text = string.Format(ResStrings.str_DeviceFormat, di.IP_Server);

                BringToFront();
            }
        }

        private void ScanInProgress_VisibleChanged(object sender, System.EventArgs e)
        {
            BringToFront();
        }
    }
}
