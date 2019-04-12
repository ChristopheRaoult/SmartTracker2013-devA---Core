using System;
using System.Globalization;
using System.Windows.Forms;
using System.Configuration;

namespace smartTracker
{
    public partial class ServerMngtForm : Form    {

        private readonly MainForm _mParent;
        public ServerMngtForm(MainForm frm1)
        {            
            InitializeComponent();
            _mParent = frm1;
            _mParent.TcpServeur.TxtInfoServer = textBoxInfo;
            textBoxPort.Text = _mParent.TcpServeur.Port.ToString(CultureInfo.InvariantCulture);
            textBoxIP.Text = _mParent.TcpServeur.ServerIP;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove("serverPort");
            config.AppSettings.Settings.Add("serverPort", textBoxPort.Text);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            Environment.ExitCode = 2; //the surrounding AppStarter must look for this to restart the app.
            Application.Exit();

                      
        }
    }
}
