using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.IO;

using DBClass;
using DataClass;

using OfficeOpenXml;
using BrightIdeasSoftware;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class CompareInventoryForm : Form
    {

        ToolStripCheckedBox toolCheckBox = new ToolStripCheckedBox();
        ToolStripCheckedBox toolCheckBox2 = new ToolStripCheckedBox();

        DataSet _ds;
        DataTable _dt;
        MainDBClass _db;
        InventoryData InvToCompare;
        string _pathXls;

        bool _bDisplayAll = false;
        bool bCompareLotID = true;
        bool _bCompareLotIdAloneAndTagUid = false;

        int _indexTagId = -1;
        int _indexLotId = -1;

        Hashtable _columnInfo = null;    



      

        public CompareInventoryForm(InventoryData invToCompare, bool bCompareLotId)
        {
            InvToCompare = invToCompare;
            bCompareLotID = bCompareLotId;
            InitializeComponent();

            toolCheckBox.Text = ResStrings.CompareInventoryForm_CompareInventoryForm_Display_All;
            
            toolCheckBox.AutoSize = false;
            toolCheckBox.Width = 100;
            toolCheckBox.Enabled = true;
            toolCheckBox.BackColor = Color.White;
            toolCheckBox.Click += new EventHandler(toolCheckBox_Click);
            toolStripCompare.Items.Insert(1, (ToolStripItem)toolCheckBox);

            toolCheckBox2.Text = ResStrings.CompareInventoryForm_CompareInventoryForm_Compare_LotID_and_TagID;
            toolCheckBox2.AutoSize = false;
            toolCheckBox2.Width = 180;
            toolCheckBox2.Enabled = true;
            toolCheckBox2.BackColor = Color.White;
            toolCheckBox2.Click += new EventHandler(toolCheckBox2_Click);
            toolStripCompare.Items.Insert(2, (ToolStripItem)toolCheckBox2);
        }

        private void toolCheckBox_Click(object sender, EventArgs e)
        {
            var checkBox = toolCheckBox.Control as CheckBox;
            if (checkBox != null)
            {
                _bDisplayAll = checkBox.Checked;
                UpdateGrid(_bDisplayAll);
            }

        }

        private void toolCheckBox2_Click(object sender, EventArgs e)
        {
            var checkBox = toolCheckBox2.Control as CheckBox;
            if (checkBox != null)
            {
                _bCompareLotIdAloneAndTagUid = checkBox.Checked;
                ProcessData();
            }
        }

        private void UpdateGrid(bool bDisplayAll)
        {
            /*int nbTot = 0;
            int nbOk = 0;
            int nbMissing = 0;
            int nbNotExpexted = 0;

            if (bDisplayAll)
            {
                dt.DefaultView.RowFilter = null;
                dataGridViewImport.DataSource = null;
                dataGridViewImport.DataSource = dt.DefaultView;
            }
            else
            {
                dt.DefaultView.RowFilter = "Status <> 'Ok'";
                dataGridViewImport.DataSource = null;
                dataGridViewImport.DataSource = dt.DefaultView;
            }


            foreach (DataGridViewRow dgvRow in dataGridViewImport.Rows)
            {
               

                Object cellValue = dgvRow.Cells["Status"].Value;
                if (cellValue != null)
                {
                    nbTot++;
                    switch (cellValue.ToString())
                    {
                        case "Ok": dgvRow.DefaultCellStyle.ForeColor = Color.Green; nbOk++; break;
                        case "Missing": dgvRow.DefaultCellStyle.ForeColor = Color.Red; nbMissing++; break;
                        case "Not Expected": dgvRow.DefaultCellStyle.ForeColor = Color.Blue;  nbNotExpexted++; break;
                    }
                }   
            }
            string strInfo = "Info : Nb Total = " + nbTot;
            if (bDisplayAll)   strInfo += " - Nb Present = " + nbOk;
            strInfo += " - Nb Missing = " + nbMissing;
            strInfo += " - Nb Not Expected = " + nbNotExpexted;

            toolStripStatusLabel.Text = strInfo; */
            int nbTot = 0;
            int nbOk = 0;
            int nbMissing = 0;
            int nbNotExpexted = 0;

            if (bDisplayAll)
            {
                _dt.DefaultView.RowFilter = null;
                dataListViewImport.DataSource = null;
                dataListViewImport.DataSource = _dt.DefaultView;
            }
            else
            {
                _dt.DefaultView.RowFilter = ResStrings.CompareInventoryForm_ProcessData_CompareStatus + " <> '" + ResStrings.str_Ok + "'";
                dataListViewImport.DataSource = null;
                dataListViewImport.DataSource = _dt.DefaultView;
            }

            for (int i = 0; i < dataListViewImport.Columns.Count; i++)
            {
                OLVColumn ol = dataListViewImport.GetColumn(i);
                //ol.FillsFreeSpace = true;
                ol.HeaderFont = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
                ol.HeaderForeColor = Color.AliceBlue;
                ol.IsTileViewColumn = true;
                ol.UseInitialLetterForGroup = false;
                ol.MinimumWidth = 20 + ol.Text.Length * 10;
                ol.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                if (ol.Index == dataListViewImport.Columns.Count - 1)
                    ol.FillsFreeSpace = true;

            }          


            foreach (ListViewItem lvi in dataListViewImport.Items)
            {
                string strCase = lvi.SubItems[0].Text;
                {
                    if(strCase.Equals( ResStrings.str_Ok)) {lvi.ForeColor = Color.Green; nbOk++;}
                    else if (strCase.Equals(ResStrings.CompareInventoryForm_ProcessData_Missing)) { lvi.ForeColor = Color.Red; nbMissing++; } 
                    else if(strCase.Equals(ResStrings.CompareInventoryForm_UpdateGrid_Not_Expected)) {lvi.ForeColor = Color.Blue; nbNotExpexted++;} 
                    else lvi.ForeColor = Color.Black; 
                }
            }

            if (dataListViewImport.OLVGroups != null)
            {
                for (int i = 0; i < dataListViewImport.OLVGroups.Count; i++)
                {
                    OLVGroup grp = dataListViewImport.OLVGroups[i];
                    grp.Collapsed = true;

                }
            }
            
            string strInfo = string.Format(ResStrings.CompareInventoryForm_UpdateGrid_, nbTot);
            if (bDisplayAll) strInfo += string.Format(ResStrings.CompareInventoryForm_UpdateGrid_NbPresent, nbOk);
            strInfo += string.Format(ResStrings.CompareInventoryForm_UpdateGrid_Missing, nbMissing);
            strInfo += string.Format(ResStrings.CompareInventoryForm_UpdateGrid_Notexpected, nbNotExpexted);

            toolStripStatusLabel.Text = strInfo;

        }

        private bool Import()
        {
            DialogResult ret =  openFileXLSDialog.ShowDialog();
            if (ret == DialogResult.OK)
            {
                try
                {
                    _pathXls = openFileXLSDialog.FileName;
                    if (!string.IsNullOrEmpty(_pathXls))
                    {
                        _ds = new DataSet();
                        if (Path.GetExtension(_pathXls).Contains("xlsx")) // 2010
                        {
                            FileStream stream = File.Open(_pathXls, FileMode.Open, FileAccess.Read);
                            Excel.IExcelDataReader excelReader = Excel.ExcelReaderFactory.CreateOpenXmlReader(stream);
                            excelReader.IsFirstRowAsColumnNames = true;
                            _ds = excelReader.AsDataSet();
                            stream.Close();
                        }
                        else if (Path.GetExtension(_pathXls).Contains("csv"))
                        {
                             _ds = _db.ImportCSVToDS(_pathXls);
                             
                        }
                        else
                        {

                            FileStream stream = File.Open(_pathXls, FileMode.Open, FileAccess.Read);
                            Excel.IExcelDataReader excelReader = Excel.ExcelReaderFactory.CreateBinaryReader(stream);
                            excelReader.IsFirstRowAsColumnNames = true;
                            _ds = excelReader.AsDataSet();
                            stream.Close();
                        }

                        return true;
                    }
                }
                catch (IOException)
                {
                    MessageBox.Show(ResStrings.CompareInventoryForm_Import_, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
            }
            return false;
        }
/*
        private bool Import2()
        {
            string sheetName = null;
            DialogResult ret = openFileXLSDialog.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    _pathXls = openFileXLSDialog.FileName;
                    if (!string.IsNullOrEmpty(_pathXls))
                    {
                        _ds = new DataSet();
                        if (Path.GetExtension(_pathXls).Contains("xlsx")) // 2010
                        {
                            FileInfo newFile = new FileInfo(_pathXls);
                            using (OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage(newFile))
                            {
                                OfficeOpenXml.ExcelWorksheet ws = pck.Workbook.Worksheets[1];
                                sheetName = ws.Name;
                                _ds = _db.ImportExcelToDS(_pathXls, sheetName);
                            }
                        }
                        else if (Path.GetExtension(_pathXls).Contains("csv"))
                        {
                            _ds = _db.ImportCSVToDS(_pathXls);

                        }
                        else
                        {

                            _ds = _db.ImportExcelToDS(_pathXls);
                        }

                        return true;
                    }
                }
                catch (IOException exp)
                {
                    MessageBox.Show(ResStrings.CompareInventoryForm_Import_, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
            }
            return false;
        }
*/
        private void CompareInventoryForm_Load(object sender, EventArgs e)
        {
            try
            {
                _db = new MainDBClass();
                _db.OpenDB();
                _columnInfo = _db.GetColumnInfo();
                if (Import())
                    ProcessData();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
        }

        private void ProcessData()
        {          

            DataTable dtTmp = new DataTable();

            bool isAllColumnOk = true;
            string errorInfo = string.Empty;

            for (int j = 0; j < _ds.Tables[0].Columns.Count; j++)
            {
                string colName = _ds.Tables[0].Columns[j].ColumnName;
                if (colName.Contains("#"))
                {
                    _ds.Tables[0].Columns[j].ColumnName = colName.Replace("#", ".");
                }
            }


            for (int i = 0; i < _columnInfo.Count; i++)
            {
                if (!_ds.Tables[0].Columns.Contains(_columnInfo[i].ToString()))
                {
                    isAllColumnOk = false;
                    errorInfo += string.Format(ResStrings.CompareInventoryForm_ProcessData__0__has_no_related_column_in_XLS, _columnInfo[i]);
                }
            }

            if (!isAllColumnOk)
            {
                MessageBox.Show(errorInfo + ResStrings.CompareInventoryForm_ProcessData_);
                return;
            }

            //dtTmp.Columns.Add("Status");
            foreach (DataColumn colName in _ds.Tables[0].Columns)
            {
                if (colName.ColumnName.Equals(_columnInfo[0].ToString())) _indexTagId = colName.Ordinal;
                if (colName.ColumnName.Equals(_columnInfo[1].ToString())) _indexLotId = colName.Ordinal;
                dtTmp.Columns.Add(colName.ColumnName, typeof(string));
            }


            DataRow[] drResults = null;
            if ((_indexLotId >= 0) && (_indexTagId >= 0))
            {
                string selectString = "[" + _columnInfo[0] + "] is NOT NULL OR [" + _columnInfo[1] + "] is NOT NULL";
                drResults = _ds.Tables[0].Select(selectString);
            }
            else if (_indexLotId >= 0)
            {
                string selectString = "[" + _columnInfo[1] + "] is NOT NULL";
                drResults = _ds.Tables[0].Select(selectString);
            }

            foreach (DataRow dr in drResults)
            {
                //object[] row = dr.ItemArray;
                object[] row = new object[dr.ItemArray.Length];
                for (int i = 0; i < dr.ItemArray.Length; i++)
                    row[i] = dr.ItemArray[i].ToString();

                dtTmp.Rows.Add(row);
            }


            ArrayList listTagXls = new ArrayList();

            _dt = new DataTable();
            _dt.Columns.Add(ResStrings.CompareInventoryForm_ProcessData_CompareStatus, typeof(string));
            for (int i = 0; i < _columnInfo.Count; i++)
                _dt.Columns.Add(_columnInfo[i].ToString(), typeof(string));
           

            if (bCompareLotID)
            {
                if (_bCompareLotIdAloneAndTagUid)
                {
                    foreach (DataRow dRow in dtTmp.Rows)
                    {


                        string tagUid = dRow[_columnInfo[0].ToString()].ToString();
                        string lotId = dRow[_columnInfo[1].ToString()].ToString();

                        if (string.IsNullOrEmpty(lotId)) continue;
                        if (string.IsNullOrEmpty(tagUid)) continue;

                        listTagXls.Add(tagUid);
                        string tagUidFromDb = _db.getProductTagID(lotId);

                        if (!string.IsNullOrEmpty(tagUidFromDb))
                        {
                            if (tagUidFromDb.Equals(tagUid))
                            {
                                if (!InvToCompare.listTagAll.Contains(tagUidFromDb))
                                {
                                    
                                    object[] param = new object[_columnInfo.Count + 1];
                                    for (int i = 0; i < _columnInfo.Count; i++)
                                        param[i+1] = dRow[_columnInfo[i].ToString()].ToString();
                                    param[0] = ResStrings.CompareInventoryForm_ProcessData_Missing;
                                    _dt.Rows.Add(param);

                                }
                                else
                                {
                                    
                                    object[] param = new object[_columnInfo.Count + 1];
                                    for (int i = 0; i < _columnInfo.Count; i++)
                                        param[i+1] = dRow[_columnInfo[i].ToString()].ToString();
                                    param[0] = ResStrings.str_Ok;
                                    _dt.Rows.Add(param);
                                }
                            }
                            else
                            {

                               
                                object[] param = new object[_columnInfo.Count + 1];
                                for (int i = 0; i < _columnInfo.Count; i++)
                                    param[i+1] = dRow[_columnInfo[i].ToString()].ToString();
                                param[0] = string.Format(ResStrings.CompareInventoryForm_ProcessData_Not_Same_TagUID_in_DB___0_, tagUidFromDb);
                                _dt.Rows.Add(param);
                            }
                        }
                        else
                        {
                            //dt.Rows.Add("Unknown", dRow["LotID"].ToString(), dRow["Description"].ToString(), "Not In Database");
                            object[] param = new object[_columnInfo.Count + 1];
                            for (int i = 0; i < _columnInfo.Count; i++)
                                param[i+1] = dRow[_columnInfo[i].ToString()].ToString();
                            param[0] = ResStrings.CompareInventoryForm_ProcessData_Not_In_Database;
                            _dt.Rows.Add(param);
                        }

                    }
                    foreach (string tagUid in InvToCompare.listTagAll)
                    {
                        if (!listTagXls.Contains(tagUid))
                        {
                            string[] productInfo = _db.RecoverProductInfo(tagUid);
                            if (productInfo == null)
                            {
                                // dt.Rows.Add(tagUID, " Unreferenced ", " ", "Not Expected");
                                object[] param = new object[_columnInfo.Count + 1];
                                param[1] = tagUid;
                                param[2] = ResStrings.str_Unreferenced;
                                for (int i = 3; i < _columnInfo.Count+1; i++)
                                    param[i] = " ";
                                param[0] = ResStrings.CompareInventoryForm_UpdateGrid_Not_Expected;
                                _dt.Rows.Add(param);
                            }
                            else
                            {
                                // dt.Rows.Add(productInfo[0], productInfo[1], productInfo[2], "Not Expected");
                                object[] param = new object[_columnInfo.Count + 1];
                                for (int i = 0; i < _columnInfo.Count; i++)
                                    param[i+1] = productInfo[i];
                                param[0] = ResStrings.CompareInventoryForm_UpdateGrid_Not_Expected;
                                _dt.Rows.Add(param);

                            }
                        }
                    }
                }
                else
                {
                    foreach (DataRow dRow in dtTmp.Rows)
                    {
                        //string lotID = dRow["LotID"].ToString();
                        string lotId = dRow[_columnInfo[1].ToString()].ToString();
                        if (string.IsNullOrEmpty(lotId)) continue;

                        string selectString = "[" + _columnInfo[1] + "]='" + lotId + "'";
                        DataRow[] lignes = InvToCompare.dtTagAll.Select(selectString);

                        if (lignes.Length == 0)
                        {
                            object[] param = new object[_columnInfo.Count + 1];
                            for (int i = 0; i < _columnInfo.Count; i++)
                                param[i+1] = dRow[_columnInfo[i].ToString()].ToString();
                            param[0] = ResStrings.CompareInventoryForm_ProcessData_Missing;
                            _dt.Rows.Add(param);
                        }
                        else
                        {
                            object[] param = new object[_columnInfo.Count + 1];
                            for (int i = 0; i < _columnInfo.Count; i++)
                                param[i+1] = dRow[_columnInfo[i].ToString()].ToString();
                            param[0] = ResStrings.str_Ok;
                            _dt.Rows.Add(param);
                        }
                    }
                    foreach (DataRow dRow in InvToCompare.dtTagAll.Rows)
                    {

                        string lotId = dRow[_columnInfo[1].ToString()].ToString();
                        if (string.IsNullOrEmpty(lotId)) continue;

                        string selectString = "[" + _columnInfo[1] + "]='" + lotId + "'";
                        DataRow[] lignes = dtTmp.Select(selectString);
                        if (lignes.Length == 0)
                        {
                            object[] param = new object[_columnInfo.Count + 1];
                            for (int i = 0; i < _columnInfo.Count; i++)
                                param[i+1] = dRow[_columnInfo[i].ToString()].ToString();
                            param[0] = ResStrings.CompareInventoryForm_UpdateGrid_Not_Expected;
                            _dt.Rows.Add(param);
                        }
                    }
                }
            }
            else
            {
                foreach (DataRow dRow in dtTmp.Rows)
                {
                   string tagUid = dRow[_columnInfo[0].ToString()].ToString();
                    if (string.IsNullOrEmpty(tagUid)) continue;
                    listTagXls.Add(tagUid);

                    if (!InvToCompare.listTagAll.Contains(tagUid))
                    {
                        object[] param = new object[_columnInfo.Count + 1];
                        for (int i = 0; i < _columnInfo.Count; i++)
                            param[i+1] = dRow[_columnInfo[i].ToString()].ToString();
                        param[0] = ResStrings.CompareInventoryForm_ProcessData_Missing;
                        _dt.Rows.Add(param);
                    }
                    else
                    {
                        object[] param = new object[_columnInfo.Count + 1];
                        for (int i = 0; i < _columnInfo.Count; i++)
                            param[i+1] = dRow[_columnInfo[i].ToString()].ToString();
                        param[0] = ResStrings.str_Ok;
                        _dt.Rows.Add(param);
                    }
                }

                foreach (string tagUid in InvToCompare.listTagAll)
                {
                    if (!listTagXls.Contains(tagUid))
                    {
                        string[] productInfo = _db.RecoverProductInfo(tagUid);
                        if (productInfo == null)
                        {
                            // dt.Rows.Add(tagUID, " Unreferenced ", " ", "Not Expected");
                            object[] param = new object[_columnInfo.Count + 1];
                            param[1] = tagUid;
                            param[2] = ResStrings.str_Unreferenced;
                            for (int i = 2; i < _columnInfo.Count+1; i++)
                                param[i] = " ";
                            param[0] = ResStrings.CompareInventoryForm_UpdateGrid_Not_Expected;
                            _dt.Rows.Add(param);
                        }
                        else
                        {
                            //dt.Rows.Add(productInfo[0], productInfo[1], productInfo[2], "Not Expected");
                            object[] param = new object[_columnInfo.Count + 1];
                            for (int i = 0; i < _columnInfo.Count; i++)
                                param[i+1] = productInfo[i];
                            param[0] = ResStrings.CompareInventoryForm_UpdateGrid_Not_Expected;
                            _dt.Rows.Add(param);
                        }
                    }
                }
            }
            
            UpdateGrid(_bDisplayAll);   
        }

        private void CompareInventoryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _db.CloseDB();
        }

        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            if (Import())
                ProcessData();
        }

        private void toolStripButtonExport_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void ExportToExcel()
        {
           
            saveXlsFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            DialogResult ret = saveXlsFileDialog.ShowDialog();

            if (ret == DialogResult.OK)
            {
                string fileSaveName = Path.GetFullPath(saveXlsFileDialog.FileName);               

                if (File.Exists(fileSaveName))
                    File.Delete(fileSaveName);

                if (Path.GetExtension(fileSaveName).Contains("xlsx")) // 2010
                {

                    FileInfo newFile = new FileInfo(fileSaveName);
                    ExcelPackage pck = new ExcelPackage(newFile);
                    {
                        //Create the worksheet
                        ExcelWorksheet ws1 = pck.Workbook.Worksheets.Add(ResStrings.CompareInventoryForm_ExportToExcel_Comparison);
                        //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1

                        ws1.Cells.Style.Font.Size = 10;
                        ws1.Cells.Style.Font.Name = "Verdana";

                        if (_bDisplayAll)
                            ws1.Cells["A1"].LoadFromDataTable(_dt, true);
                        else
                        {
                           DataTable tmpDtt = _dt.Clone();
                           DataRow[] res = _dt.Select(ResStrings.CompareInventoryForm_ProcessData_CompareStatus + " <> '" + ResStrings.str_Ok + "'");
                           foreach (DataRow dr in res)
                               tmpDtt.ImportRow(dr);
                            ws1.Cells["A1"].LoadFromDataTable(tmpDtt, true);
                        }


                        ws1.Cells[1, 1, 1, _columnInfo.Count + 1].Style.Font.Bold = true;
                        ws1.Cells[1, 1, 1, _columnInfo.Count + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws1.Cells[1, 1, 1, _columnInfo.Count + 1].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                        ws1.Column(loop).AutoFit(25);                       

                        pck.Save();
                    }
                }
                else
                {
                    //DBClassSQLite.Export(ConvertForExport(dt), fileSaveName, "Comparison");
                    new ErrorExportDLG().Show();
                }
            }
        }

/*
        private DataTable ConvertForExport(DataTable dt)
        {
            ArrayList columnToExport = _db.GetColumnToExport();
            if (columnToExport != null)
            {
                DataTable dtToexport = dt.Copy();
                DataColumnCollection dcc = dt.Columns;
                for (int loop = 0; loop < dcc.Count; loop++)
                {
                    if (!columnToExport.Contains(dcc[loop].ColumnName))
                        dtToexport.Columns.Remove(dcc[loop].ColumnName);
                }
                return dtToexport;
            }
            else
                return dt;
        }
*/

        private void toolStripButtonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

/*
        private void dataGridViewImport_Sorted(object sender, EventArgs e)
        {
            UpdateGrid(_bDisplayAll);   
        }
*/

        private void dataListViewImport_HotItemChanged(object sender, HotItemChangedEventArgs e)
        {
            foreach (ListViewItem lvi in dataListViewImport.Items)
            {
                string strCase = lvi.SubItems[0].Text;
                {
                    if (strCase.Equals(ResStrings.str_Ok)) { lvi.ForeColor = Color.Green;  }
                    else if (strCase.Equals(ResStrings.CompareInventoryForm_ProcessData_Missing)) { lvi.ForeColor = Color.Red;  }
                    else if (strCase.Equals(ResStrings.CompareInventoryForm_UpdateGrid_Not_Expected)) { lvi.ForeColor = Color.Blue; }
                    else lvi.ForeColor = Color.Black;
                }
            }
        }
    }
}
