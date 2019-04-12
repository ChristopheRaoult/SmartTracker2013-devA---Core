using System;
using System.Windows.Forms;
using DBClass_SQLServer;
using DB_Class_SQLite;
using ErrorMessage;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class ExportFrm : Form
    {
        DBClassSQLite _db;
        private bool _bChange = false;
        private string _initialConStr;
        private int _bEnableSql = 0;
        private string _tableName;
        public ExportFrm()
        {
            InitializeComponent();
        }

        private void toolStripButtonQuit_Click(object sender, EventArgs e)
        {
            _db.CloseDB();

            if (_bChange)
            {
                MessageBox.Show(ResStrings.str_Application_will_restart_to_take_into_account_column_change, ResStrings.ExportFrm_toolStripButtonQuit_Click__Exportation_Info, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.ExitCode = 2; //the surrounding AppStarter must look for this to restart the app.
                Application.Exit();
            }
            else
                Close();
        }

        private void ExportFrm_Load(object sender, EventArgs e)
        {
            _db = new DBClassSQLite();
            _db.OpenDB();
            _initialConStr = null;
            _tableName = null;
            LoadInfoFromLocalDb();
        }

        public void LoadInfoFromLocalDb()
        {
            try
            {

                _db.getExportInfo(1,out _initialConStr,out _tableName, out _bEnableSql);

                if (_bEnableSql == 1) checkBoxEnableSQL.Checked = true;
                else checkBoxEnableSQL.Checked = false;

                if (string.IsNullOrEmpty(_initialConStr)) return;

                string[] strArray = _initialConStr.Split(';');
                foreach (string str2 in strArray)
                {
                    if (str2.Length == 0) break;
                    string strSwitch = str2.Substring(0, str2.IndexOf('=')).ToUpper().Trim();
                    switch (strSwitch)
                    {
                        case "DATA SOURCE":
                            txtServerName.Text = str2.Substring(str2.IndexOf('=') + 1);
                            break;

                        case "INITIAL CATALOG":
                            txtDatabaseName.Text = str2.Substring(str2.IndexOf('=') + 1);
                            break;

                        case "USER ID":
                            txtLogin.Text = str2.Substring(str2.IndexOf('=') + 1);
                            break;

                        case "PASSWORD":
                            {
                                string str3 = UtilSqlServer.DencryptPassword(str2.Substring(str2.IndexOf('=') + 1));
                                txtPassword.Text = str3;
                                break;
                            }
                    }
                }
                if (txtLogin.Text.Trim().Length > 0)
                {
                    CboAuthentication.SelectedIndex = 1;
                }
                else
                {
                    CboAuthentication.SelectedIndex = 0;
                }
                txtTableName.Text = _tableName;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                MessageBox.Show(exception.Message, ResStrings.Str_Key_Not_Found, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

            }
            finally
            {
                if (txtLogin.Text.Trim().Length > 0)
                {
                    CboAuthentication.SelectedIndex = 1;
                }
                else
                {
                    CboAuthentication.SelectedIndex = 0;
                }
            }

        }
        private void EnableCtrl(bool pEnable)
        {
            txtLogin.Enabled = pEnable;
            txtPassword.Enabled = pEnable;
            lblLogin.Enabled = pEnable;
            lblPassword.Enabled = pEnable;
        }

        private void CboAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (CboAuthentication.SelectedIndex)
            {
                case 0:
                    EnableCtrl(false);
                    txtLogin.Clear();
                    txtPassword.Clear();
                    break;

                case 1:
                    EnableCtrl(true);
                    break;
            }
            btnSave.Enabled = false;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtServerName.Text.Trim().Length == 0)
                {
                    MessageBox.Show(ResStrings.Str_Enter_the_Server_Name, ResStrings.str_Connect_Server, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                }
                else if (txtDatabaseName.Text.Trim().Length == 0)
                {
                    MessageBox.Show(ResStrings.str_Enter_the_Database_Name, ResStrings.str_Connect_Server, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                }
                else
                {
                    //string str = null;
                    string str2 = null;
                    // string str3 = DB_Class_SQLServer.UtilSqlServer.EncryptPassword(this.txtPassword.Text);
                    string str3 = txtPassword.Text;

                    if (CboAuthentication.SelectedIndex == 0)
                    {
                       // str = "Data Source=" + txtServerName.Text.Trim() + ";Initial Catalog=" + txtDatabaseName.Text.Trim() + ";Integrated Security=SSPI";
                        str2 = "Data Source=" + txtServerName.Text.Trim() + ";Initial Catalog=" + txtDatabaseName.Text.Trim() + ";Integrated Security=SSPI;";
                    }
                    else
                    {
                        //str = "Data Source=" + txtServerName.Text.Trim() + ";Initial Catalog=" + txtDatabaseName.Text.Trim() + ";User ID=" + txtLogin.Text.Trim() + ";Password=" + str3;
                        str2 = "Data Source=" + txtServerName.Text.Trim() + ";Initial Catalog=" + txtDatabaseName.Text.Trim() + ";User ID=" + txtLogin.Text.Trim() + ";Password=" + str3 + ";";
                    }

                    /*SqlConnection testCon = new SqlConnection();
                    testCon.ConnectionString = str2;
                    testCon.Open();     */

                    ExportInventory testexp = new ExportInventory(str2, txtTableName.Text.Trim());
                    testexp.OpenDb();

                    if (testexp.IsOpen())
                    {
                        if (testexp.IsTableExist())
                        {
                            MessageBox.Show(ResStrings.ExportFrm_btnTest_Click_Connected_and_Table_Exist, ResStrings.str_Connect_Server, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                           
                            btnSave.Enabled = true;
                        }
                        else
                        {
                            MessageBox.Show(ResStrings.ExportFrm_btnTest_Click_Connected_and_Table_not_Exist, ResStrings.str_Connect_Server, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);                           
                            btnSave.Enabled = false;
                        }
                        testexp.CloseDb();
                    }
                    else
                    {
                        MessageBox.Show(ResStrings.ExportFrm_btnTest_Click_, ResStrings.str_Connect_Server, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    }

                }
            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtServerName.Text.Trim().Length == 0)
                {
                    MessageBox.Show(ResStrings.Str_Enter_the_Server_Name, ResStrings.str_Connect_Server, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                }
                else if (txtDatabaseName.Text.Trim().Length == 0)
                {
                    MessageBox.Show(ResStrings.str_Enter_the_Database_Name, ResStrings.str_Connect_Server, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                }
                else if (txtTableName.Text.Trim().Length == 0)
                {
                    MessageBox.Show(ResStrings.str_Enter_the_Table_Name, ResStrings.str_Connect_Server, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                }
                else
                {
                   // string str = null;
                    string str2 = null;
                    string str3 = UtilSqlServer.EncryptPassword(txtPassword.Text);

                    if (CboAuthentication.SelectedIndex == 0)
                    {
                        //str = "Data Source=" + txtServerName.Text.Trim() + ";Initial Catalog=" + txtDatabaseName.Text.Trim() + ";Integrated Security=SSPI";
                        str2 = "Data Source=" + txtServerName.Text.Trim() + ";Initial Catalog=" + txtDatabaseName.Text.Trim() + ";Integrated Security=SSPI;";
                    }
                    else
                    {
                        //str = "Data Source=" + txtServerName.Text.Trim() + ";Initial Catalog=" + txtDatabaseName.Text.Trim() + ";User ID=" + txtLogin.Text.Trim() + ";Password=" + str3;
                        str2 = "Data Source=" + txtServerName.Text.Trim() + ";Initial Catalog=" + txtDatabaseName.Text.Trim() + ";User ID=" + txtLogin.Text.Trim() + ";Password=" + str3 + ";";
                    }
                    if (_db.AddExportInfo(1, str2, txtTableName.Text.Trim(), Convert.ToInt32(checkBoxEnableSQL.Checked)))
                    {
                        _bChange = true;
                        MessageBox.Show(ResStrings.str_Sql_Connection_Info_Saved, ResStrings.str_Info_SQL_Server, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    }
                    else
                        MessageBox.Show(ResStrings.str_Error_While_Saving_Sql_Connection_Info_Saved, ResStrings.str_Info_SQL_Server, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                }
            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }
        }

        private void checkBoxEnableSQL_Click(object sender, EventArgs e)
        {
            _db.setSqlExportOnOff(1,checkBoxEnableSQL.Checked);
        }

        private void txtServerName_TextChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
        }
    }
}
