using System;
using System.Windows.Forms;

namespace smartTracker
{
    public partial class ScanFinish : Form
    {
        public ScanFinish()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.Manual;
            Top = 110;
            Left = 800;  
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
