using System;
using System.Data;
using System.Windows.Forms;
using DataClass;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class AboutUs : Form
    {
        private readonly DataTable _dt;
        private DataRow _dr;

        public AboutUs()
        {
            _dt = new DataTable();
            InitializeComponent();
        }

        private void AboutUS_Load(object sender, EventArgs e)
        {
            _dt.Columns.Add("Name", Type.GetType("System.String"));
            _dt.Columns.Add("Value", Type.GetType("System.String"));
            _dr = _dt.NewRow();
            _dr[0] = ResStrings.strApplication;
            _dr[1] = "";
            _dt.Rows.Add(_dr);
            _dr = _dt.NewRow();
            _dr[0] = ResStrings.strAppName;
            _dr[1] = ResStrings.strSoftName;
            _dt.Rows.Add(_dr);
            _dr = _dt.NewRow();
            _dr[0] = ResStrings.strSoftVersion;

            _dr[1] = Application.ProductVersion;
            _dt.Rows.Add(_dr);

            _dr = _dt.NewRow();
            _dr[0] = ResStrings.strSoftDevBy;
            _dr[1] = ResStrings.strSpacecode;
            _dt.Rows.Add(_dr);

            _dr = _dt.NewRow();
            _dr[0] = ResStrings.strEnvInfo;
            _dr[1] = "";
            _dt.Rows.Add(_dr);

            _dr = _dt.NewRow();
            _dr[0] = ResStrings.strOSName;
            _dr[1] = OSInfo.Name;
            _dt.Rows.Add(_dr);

            _dr = _dt.NewRow();
            _dr[0] = ResStrings.strOSEdition;
            _dr[1] = OSInfo.Edition;
            _dt.Rows.Add(_dr);

            _dr = _dt.NewRow();
            _dr[0] = ResStrings.strPackEdition;
            _dr[1] = OSInfo.ServicePack;
            _dt.Rows.Add(_dr);

            _dr = _dt.NewRow();
            _dr[0] = ResStrings.strSysVer;
            _dr[1] = OSInfo.Version;
            _dt.Rows.Add(_dr);

            _dr = _dt.NewRow();
            _dr[0] = ResStrings.strSoftBit;
            _dr[1] = OSInfo.Bits;
            _dt.Rows.Add(_dr);


            dataGridViewInfo.DataSource = _dt.DefaultView;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker) delegate { Close(); });
        }
    }
}