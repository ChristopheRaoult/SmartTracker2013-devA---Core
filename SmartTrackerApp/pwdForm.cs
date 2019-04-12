using System;
using System.Windows.Forms;

using System.IO;
using System.Diagnostics;
using System.Reflection;
using smartTracker.Properties;


namespace smartTracker
{
    public partial class PwdForm : Form
    {
        private const string Pwd = "123456";

        public PwdForm()
        {
            InitializeComponent();
        }

        private void textBoxPwd_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Return)
            {
                buttonProcess_Click(this, null);
            }
        }

        private void buttonProcess_Click(object sender, EventArgs e)
        {
            if (textBoxPwd.Text.Equals(Pwd))
            {

                string pathTestRfid = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "TestRFID.exe";

                Process testProcess = null;

                try
                {
                    ProcessStartInfo p = new ProcessStartInfo(pathTestRfid);
                    p.WorkingDirectory = Path.GetDirectoryName(pathTestRfid);
                    testProcess = Process.Start(p);

                }
                catch
                {
                    ErrorMessage.ExceptionMessageBox.Show(string.Format(ResStrings.str_Started_app, pathTestRfid), ResStrings.strInfo);
                }

                if (testProcess == null) return;
                try
                {
                    Visible = false;
                    testProcess.WaitForExit();
                    Close();
                }
                catch (Exception ex2)
                {
                    ErrorMessage.ExceptionMessageBox.Show(ex2);
                }
            }
            else
            {
                textBoxPwd.Text = string.Empty;
            }
        }
    }
}
