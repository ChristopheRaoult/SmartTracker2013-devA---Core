using System;
using System.Windows.Forms;

namespace smartTracker
{
    public partial class Confirm : Form
    {
              
        private Conversive.AutoUpdater.AutoUpdater autoUpdater;
        public Confirm(Conversive.AutoUpdater.AutoUpdater autoUpdater)
        {
            InitializeComponent();
            this.autoUpdater = autoUpdater;
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.BeginInvoke((MethodInvoker)delegate { this.Close(); });
       
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.BeginInvoke((MethodInvoker)delegate { this.Close(); });
        }

        private void Confirm_Activated(object sender, System.EventArgs e)
        {
            string strChange = this.autoUpdater.LatestConfigChanges;
            this.latestChangesTextBox.Text = strChange.Replace("\n","\r\n");
        }

        private void Confirm_FormClosed(object sender, FormClosedEventArgs e)
        {
    
        }      
     
    }
}
