using System;
using System.Windows.Forms;


using DBClass;
using DataClass;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class Formule : Form
    {

        public LiveDataForm Lvd;       

        MainDBClass _db = null;
        FormuleData _fd = null;
        dtColumnInfo[] _colInfo = null;
        public Formule(LiveDataForm lvd)
        {
            InitializeComponent();
            Lvd = lvd;
        }

        private void Formule_Load(object sender, EventArgs e)
        {
            _db = new MainDBClass();

            _db.OpenDB();
            _fd = _db.getFormuleInfo();
            UpdateList();
            if (_fd != null)
            {
                textBoxFormule.Text = _fd.Formule;
                textBoxFormuleTitle.Text = _fd.Title;
                checkBoxEnable.Checked = _fd.Enable == 1;
            }
        }

        private void UpdateList()
        {
            _colInfo = _db.GetdtColumnInfo();
            int nIndex = 0;
            if (_colInfo != null)
            {
                listBoxParam.Items.Clear();

                for (int i = 0; i < _colInfo.Length; i++)
                {
                    if (_colInfo[i].colDoSum == 1)
                    {
                        byte letter = (byte)('A' + nIndex++);
                        string colStr = string.Format(ResStrings.Formule_UpdateList_Sum_of, Convert.ToChar(letter), _colInfo[i].colName);
                        listBoxParam.Items.Add(colStr);
                    }
                }
            }

        }

        private void Formule_FormClosing(object sender, FormClosingEventArgs e)
        {
            _db.CloseDB();
        }

        private void listBoxParam_Click(object sender, EventArgs e)
        {
            if (listBoxParam.SelectedIndex > -1)
            {
                byte letter = (byte)('A' + listBoxParam.SelectedIndex);
                textBoxFormule.Text += Convert.ToChar(letter).ToString();
                textBoxFormule.Focus();
                textBoxFormule.Select(textBoxFormule.Text.Length, 0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _fd = new FormuleData();
                _fd.Enable = checkBoxEnable.Checked ? 1 : 0;
                _fd.Title = textBoxFormuleTitle.Text;
                _fd.Formule = textBoxFormule.Text;
                _db.setFormuleInfo(_fd);
                MessageBox.Show(ResStrings.Formule_button1_Click_Formulae_saved, Properties.ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (Lvd != null)
                    Lvd.LoadFormule();
                
            }
            catch (Exception exp)
            {
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
        }

        private void toolStripButtonQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBoxFormule_TextChanged(object sender, EventArgs e)
        {
            textBoxFormule.Text = textBoxFormule.Text.ToUpper();
        }
    }
}
