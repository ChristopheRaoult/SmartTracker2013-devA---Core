using System;
using System.Windows.Forms;

namespace smartTracker
{
    public partial class ErrorExportDLG : Form
    {
        public ErrorExportDLG()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(" http://www.microsoft.com/en-us/download/details.aspx?id=3");
            Close();
        }
    }
}
