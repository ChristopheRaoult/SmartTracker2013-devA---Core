using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data.SqlClient;

using DBClass;
using System.IO;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class DbColumnForm : Form
    {
        bool _bComeFromImport = false;
        MainDBClass _db = null;
        SQLiteDataAdapter _daSqlite = null;
        SQLiteCommandBuilder _cbSqlite = null;
        SqlDataAdapter _daSqlServer = null;
        SqlCommandBuilder _cbSqlServer = null;

        DataTable _dt = null;
        BindingSource _bindingSource = null;
        private string _pathImport = null;
        private bool _bChange = false;
        private bool _bHardChange = false;

        public DbColumnForm()
        {
            InitializeComponent();
        }

        private void DbColumnForm_Load(object sender, EventArgs e)
        {
            _db = new MainDBClass();        
            _db.OpenDB();
            LoadDbColumn();           
         
        }

        private void LoadDbColumn()
        {
            if (_db.getUsedDBType == MainDBClass.dbUsedType.db_SqlLite)
            {
                _bComeFromImport = false;
                _daSqlite = (SQLiteDataAdapter)_db.GetColumn();

                if (_daSqlite != null)
                {
                    _cbSqlite = new SQLiteCommandBuilder(_daSqlite);
                    _dt = new DataTable();
                    _daSqlite.Fill(_dt);
                    _bindingSource = new BindingSource();
                    _bindingSource.DataSource = _dt;
                    dataGridViewDB.DataSource = _bindingSource;
                }
            }
            else if (_db.getUsedDBType == MainDBClass.dbUsedType.db_SqlServer)
            {
                _bComeFromImport = false;
                _daSqlServer = (SqlDataAdapter)_db.GetColumn();

                if (_daSqlServer != null)
                {
                    _cbSqlServer = new  SqlCommandBuilder(_daSqlServer);
                    _dt = new DataTable();
                    _daSqlServer.Fill(_dt);
                    _bindingSource = new BindingSource();
                    _bindingSource.DataSource = _dt;
                    dataGridViewDB.DataSource = _bindingSource;
                }
            }
        }

        private void DbColumnForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _db.CloseDB();
        }

        private void toolStripButtonSubmit_Click(object sender, EventArgs e)
        {
             DialogResult ret  = DialogResult.Yes;
             if (_bHardChange)
            {
               MessageBox.Show(this, ResStrings.DbColumnForm_toolStripButtonSubmit_Click,ResStrings.DbColumnForm_toolStripButtonSubmit_Click_Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            }
            if (ret == DialogResult.Yes)
            {
                try
                {
                    if ((_bHardChange) || (_bComeFromImport)) _db.cleanDB(_bComeFromImport);
                    _bindingSource.EndEdit();
                    Validate();

                    if (_db.getUsedDBType == MainDBClass.dbUsedType.db_SqlLite)
                        _daSqlite.Update((DataTable)_bindingSource.DataSource);
                    else if (_db.getUsedDBType == MainDBClass.dbUsedType.db_SqlServer)
                        _daSqlServer.Update((DataTable)_bindingSource.DataSource);
                    toolStripButtonSubmit.Enabled = false;
                    LoadDbColumn();
                    _bChange = true;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(ResStrings.DbColumnForm_toolStripButtonSubmit_Click_Update_failed, ex.Message), ResStrings.DbColumnForm_toolStripButtonSubmit_Click_Updtate_Column_Info, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
           
        }

        private void toolStripButtonExit_Click(object sender, EventArgs e)
        {
            
            if (_bChange)
            {
                MessageBox.Show(ResStrings.str_Application_will_restart_to_take_into_account_column_change,ResStrings.DbColumnForm_toolStripButtonSubmit_Click_Updtate_Column_Info, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.ExitCode = 2; //the surrounding AppStarter must look for this to restart the app.
                Application.Exit();
            }
            else
            Close();
        }

        private void dataGridViewDB_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {            
            toolStripButtonSubmit.Enabled = true;
            if (e.ColumnIndex < 2) _bHardChange = true;
        }

        //datagrigview edit mode
        TextBox _txtEdit;
        private bool _bDelete = true;
        private void dataGridViewDB_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            _txtEdit = (TextBox)e.Control;
            _txtEdit.KeyPress += EditKeyPress; //where EditKeyPress is your keypress event
            _txtEdit.KeyDown += EditKeyDown;

        }       
        private void EditKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!_bDelete)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "[\\sa-zA-Z0-9_\\.\\-%&]+"))
                    e.Handled = true;
            }
        }
        private void EditKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Delete) || (e.KeyCode == Keys.Back))
                _bDelete = true;
            else
                _bDelete = false;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DialogResult ret = openFileDialogXLS.ShowDialog();
            if (ret == DialogResult.OK)
            {
                _pathImport = Path.GetFullPath(openFileDialogXLS.FileName);
                Import();               
            }             
        }

        private void Import()
        {
            if (!string.IsNullOrEmpty(_pathImport))
            {
                DataSet ds = null;
                try
                {
                    if (Path.GetExtension(_pathImport).Contains("xlsx")) // 2010
                    {
                        FileStream stream = File.Open(_pathImport, FileMode.Open, FileAccess.Read);
                        Excel.IExcelDataReader excelReader = Excel.ExcelReaderFactory.CreateOpenXmlReader(stream);
                        excelReader.IsFirstRowAsColumnNames = true;                      
                        ds = excelReader.AsDataSet();
                        stream.Close();
                    }
                    else if (Path.GetExtension(_pathImport).Contains("csv"))
                    {
                        ds = _db.ImportCSVToDS(_pathImport);

                    }
                    else
                    {
                        FileStream stream = File.Open(_pathImport, FileMode.Open, FileAccess.Read);
                        Excel.IExcelDataReader excelReader = Excel.ExcelReaderFactory.CreateBinaryReader(stream);
                        excelReader.IsFirstRowAsColumnNames = true;
                        ds = excelReader.AsDataSet();
                        stream.Close();
                    }

                    if (ds != null)
                    {

                        bool bColumnOk = true;

                        if (ds.Tables.Count == 0)
                        {
                            MessageBox.Show(ResStrings.DbColumnForm_Import, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                        {
                            string colName = ds.Tables[0].Columns[j].ColumnName;
                            if (colName.Contains("#"))
                            {
                                ds.Tables[0].Columns[j].ColumnName = colName.Replace("#", ".");
                            }
                            if (!System.Text.RegularExpressions.Regex.IsMatch(ds.Tables[0].Columns[j].ColumnName, "^[\\sa-zA-Z0-9_\\.\\-%&/'()]*$"))
                                bColumnOk = false;
                        }

                        if (!bColumnOk)
                        {
                            MessageBox.Show(ResStrings.DbColumnForm_Importerror,ResStrings.DbColumnForm_toolStripButtonSubmit_Click_Updtate_Column_Info, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        _dt.Rows.Clear();
                        int nIndex = 0;
                        foreach (DataColumn colName in ds.Tables[0].Columns)
                        {
                            //remove sheet 
                            _dt.Rows.Add(nIndex.ToString(), colName, colName, "STRING", "NONE",1,0);
                            nIndex++;
                        }
                        toolStripButtonSubmit.Enabled = true;
                        _bComeFromImport = true;
                    }                     
                }
                catch (IOException)
                {
                    MessageBox.Show(ResStrings.DbColumnForm_Import_fileopen, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    ErrorMessage.ExceptionMessageBox.Show(ex);
                }
            }
        }

        private void Import2()
        {
            if (!string.IsNullOrEmpty(_pathImport))
            {
                DataSet ds = null;
                try
                {
                    if (Path.GetExtension(_pathImport).Contains("xlsx")) // 2010
                    {
                        FileInfo newFile = new FileInfo(_pathImport);
                        using (OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage(newFile))
                        {
                            OfficeOpenXml.ExcelWorksheet ws = pck.Workbook.Worksheets[1];
                            string sheetName = ws.Name;
                            ds = _db.ImportExcelToDS(_pathImport, sheetName);
                        }
                    }
                    else if (Path.GetExtension(_pathImport).Contains("csv"))
                    {
                        ds = _db.ImportCSVToDS(_pathImport);

                    }
                    else
                    {
                        ds = _db.ImportExcelToDS(_pathImport);
                    }

                    if (ds != null)
                    {

                        bool bColumnOk = true;
                        for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                        {
                            string colName = ds.Tables[0].Columns[j].ColumnName;
                            if (colName.Contains("#"))
                            {
                                ds.Tables[0].Columns[j].ColumnName = colName.Replace("#", ".");
                            }
                            if (!System.Text.RegularExpressions.Regex.IsMatch(ds.Tables[0].Columns[j].ColumnName, "^[\\sa-zA-Z0-9_\\.\\-%&/'()]*$"))
                                bColumnOk = false;
                        }

                        if (!bColumnOk)
                        {
                            MessageBox.Show(ResStrings.DbColumnForm_Importerror,ResStrings.DbColumnForm_toolStripButtonSubmit_Click_Updtate_Column_Info, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        _dt.Rows.Clear();
                        int nIndex = 0;
                        foreach (DataColumn colName in ds.Tables[0].Columns)
                        {
                            //remove sheet 
                            _dt.Rows.Add(nIndex.ToString(), colName, colName, "STRING", "NONE", 1, 0);
                            nIndex++;
                        }
                        toolStripButtonSubmit.Enabled = true;
                        _bComeFromImport = true;
                    }                     
                }
                catch (IOException)
                {
                    MessageBox.Show(ResStrings.DbColumnForm_Import_fileopen, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    ErrorMessage.ExceptionMessageBox.Show(ex);
                }
            }
        }

        private void addAlertValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewDB.SelectedRows.Count > 0)
                {
                    /*for (int i = 0; i < dataGridViewDB.Rows.Count - 1 ; i++)
                    {
                        dataGridViewDB.Rows[i].Cells["AlertLink"].Value = "NONE";
                    }*/
                    dataGridViewDB.SelectedRows[0].Cells[ResStrings.DbColumnForm_addAlertValueToolStripMenuItem_Click_AlertLink].Value = DataClass.AlertType.AT_Limit_Value_Exceed.ToString();
                    toolStripButtonSubmit.Enabled = true;
                }
                else
                    MessageBox.Show(ResStrings.DbColumnForm_addAlertValueToolStripMenuItem_Click_Select_a_full_row_to_add_the_alert, ResStrings.DbColumnForm_toolStripButtonSubmit_Click_Updtate_Column_Info, MessageBoxButtons.OK, MessageBoxIcon.Warning);
              
            }
            catch (Exception ex)
            {
                ErrorMessage.ExceptionMessageBox.Show(ex);
            }
        }

        private void resetAlertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewDB.SelectedRows.Count > 0)
                {
                    dataGridViewDB.SelectedRows[0].Cells[ResStrings.DbColumnForm_addAlertValueToolStripMenuItem_Click_AlertLink].Value = "NONE";
                }
                else
                {
                    for (int i = 0; i < dataGridViewDB.Rows.Count - 1; i++)
                    {
                        dataGridViewDB.Rows[i].Cells[ResStrings.DbColumnForm_addAlertValueToolStripMenuItem_Click_AlertLink].Value = "NONE";
                        toolStripButtonSubmit.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.ExceptionMessageBox.Show(ex);
            }
        }

        private void changeInUINT64ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewDB.SelectedRows.Count > 0)
                {

                    dataGridViewDB.SelectedRows[0].Cells["ColumnType"].Value = "UINT64";
                    toolStripButtonSubmit.Enabled = true;
                }
                else
                    MessageBox.Show(ResStrings.DbColumnForm_changeInUINT64ToolStripMenuItem_Click_Select_a_full_row_to_change_variable_type, ResStrings.DbColumnForm_toolStripButtonSubmit_Click_Updtate_Column_Info, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception ex)
            {
                ErrorMessage.ExceptionMessageBox.Show(ex);
            }
        }

        private void changeInDoubleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewDB.SelectedRows.Count > 0)
                {

                    dataGridViewDB.SelectedRows[0].Cells["ColumnType"].Value = "DOUBLE";
                    toolStripButtonSubmit.Enabled = true;
                }
                else
                    MessageBox.Show(ResStrings.DbColumnForm_changeInUINT64ToolStripMenuItem_Click_Select_a_full_row_to_change_variable_type);

            }
            catch (Exception ex)
            {
                ErrorMessage.ExceptionMessageBox.Show(ex);
            }
        }

        private void changeInStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewDB.SelectedRows.Count > 0)
                {

                    dataGridViewDB.SelectedRows[0].Cells["ColumnType"].Value = "STRING";
                    toolStripButtonSubmit.Enabled = true;
                }
                else
                    MessageBox.Show(ResStrings.DbColumnForm_changeInUINT64ToolStripMenuItem_Click_Select_a_full_row_to_change_variable_type, ResStrings.DbColumnForm_toolStripButtonSubmit_Click_Updtate_Column_Info, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception ex)
            {
                ErrorMessage.ExceptionMessageBox.Show(ex);
            }
        }
       

        private void addAlertDLCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewDB.SelectedRows.Count > 0)
                {
                    /*for (int i = 0; i < dataGridViewDB.Rows.Count - 1; i++)
                    {
                        dataGridViewDB.Rows[i].Cells["AlertLink"].Value = "NONE";
                    }*/
                    dataGridViewDB.SelectedRows[0].Cells[ResStrings.DbColumnForm_addAlertValueToolStripMenuItem_Click_AlertLink].Value = DataClass.AlertType.AT_DLC_Expired.ToString();
                    toolStripButtonSubmit.Enabled = true;
                }
                else
                    MessageBox.Show(ResStrings.DbColumnForm_addAlertValueToolStripMenuItem_Click_Select_a_full_row_to_add_the_alert, ResStrings.DbColumnForm_toolStripButtonSubmit_Click_Updtate_Column_Info, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception ex)
            {
                ErrorMessage.ExceptionMessageBox.Show(ex);
            }
        }

        private void addAlertStockLimitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewDB.SelectedRows.Count > 0)
                {
                    /*for (int i = 0; i < dataGridViewDB.Rows.Count - 1; i++)
                    {
                        dataGridViewDB.Rows[i].Cells["AlertLink"].Value = "NONE";
                    }*/
                    dataGridViewDB.SelectedRows[0].Cells[ResStrings.DbColumnForm_addAlertValueToolStripMenuItem_Click_AlertLink].Value = DataClass.AlertType.AT_Stock_Limit.ToString();
                    toolStripButtonSubmit.Enabled = true;
                }
                else
                    MessageBox.Show(ResStrings.DbColumnForm_addAlertValueToolStripMenuItem_Click_Select_a_full_row_to_add_the_alert, ResStrings.DbColumnForm_toolStripButtonSubmit_Click_Updtate_Column_Info, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception ex)
            {
                ErrorMessage.ExceptionMessageBox.Show(ex);
            }
        }

        private void addAlertBadBloodForPatientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewDB.SelectedRows.Count > 0)
                {
                    /*for (int i = 0; i < dataGridViewDB.Rows.Count - 1; i++)
                    {
                        dataGridViewDB.Rows[i].Cells["AlertLink"].Value = "NONE";
                    }*/
                    dataGridViewDB.SelectedRows[0].Cells[ResStrings.DbColumnForm_addAlertValueToolStripMenuItem_Click_AlertLink].Value = DataClass.AlertType.AT_Bad_Blood_Patient.ToString();
                    toolStripButtonSubmit.Enabled = true;
                }
                else
                    MessageBox.Show(ResStrings.DbColumnForm_addAlertValueToolStripMenuItem_Click_Select_a_full_row_to_add_the_alert, ResStrings.DbColumnForm_toolStripButtonSubmit_Click_Updtate_Column_Info, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception ex)
            {
                ErrorMessage.ExceptionMessageBox.Show(ex);
            }
        }

        private void defineWeightColumnToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewDB.SelectedRows.Count > 0)
                {
                    dataGridViewDB.SelectedRows[0].Cells[ResStrings.DbColumnForm_addAlertValueToolStripMenuItem_Click_AlertLink].Value = "WEIGHT";
                    toolStripButtonSubmit.Enabled = true;
                }
                else
                    MessageBox.Show(ResStrings.DbColumnForm_changeInUINT64ToolStripMenuItem_Click_Select_a_full_row_to_change_variable_type);

            }
            catch (Exception ex)
            {
                ErrorMessage.ExceptionMessageBox.Show(ex);
            }
        }

        
       
    }
}
