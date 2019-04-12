using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using ErrorMessage;
using Excel;
using OfficeOpenXml;
using DBClass;
using DataClass;
using OfficeOpenXml.Style;
using SDK_SC_RFID_Devices;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class ItemListForm : Form
    {
        DataTable _dt;
        public LiveDataForm Lvd;   

        AutoFillBox _auto = null;
        ImageListForm _listimgFrm = null;

        ArrayList listTag;
        Hashtable _columnInfo = null;
        Regex _validateUid;

        MainDBClass _db;
        int _indexTagId = 0;
        int _indexLotId = 1;

        bool bShowImage = true;
        public DataTable DtProductRef,DtProductWithColum;
        public DataView Dv = null;
        ToolStripCheckedBox toolCheckBoxOnlyNew = new ToolStripCheckedBox();

        private RFID_Device printerDevice = null;
        private PrinterData printInfo;


        private bool _importAll = true;
        private string _pathImport = null;
       
        public ItemListForm(LiveDataForm lvd, ArrayList listTag)
        {
            Lvd = lvd;
            if (lvd != null) bShowImage = lvd.BShowImage;

            this.listTag = listTag;
            InitializeComponent();
        }

        public void GetProduct()
        {          
            if (_db != null)
            {
                DtProductWithColum = new DataTable();
                 for (int i = 0; i < _columnInfo.Count; i++)
                     DtProductWithColum.Columns.Add(_columnInfo[i].ToString(), typeof(string));
                 DtProductWithColum.Columns.Add(ResStrings.str_ImportStatus, typeof(string));

                DtProductRef = _db.RecoverAllProduct();
                if (DtProductRef == null) return;
                foreach (DataRow dr in DtProductRef.Rows)
                {
                    object[] param = new object[_columnInfo.Count+1];
                    for (int i = 0; i < _columnInfo.Count; i++)
                        param[i] = dr.ItemArray[i];
                    param[_columnInfo.Count] = ResStrings.str_Ok;
                    DtProductWithColum.Rows.Add(param);
                }  
               
            }
        }

        private void toolCheckBoxOnlyNew_Click(object sender, EventArgs e)
        {
            _importAll = !(toolCheckBoxOnlyNew.Control as CheckBox).Checked;
            Import();
            UpdateDg(-1);    
        }

        private void ItemListForm_Load(object sender, EventArgs e)
        {

            if (Lvd != null)
                addProductToListToFindByLedToolStripMenuItem.Visible = true;

            toolCheckBoxOnlyNew.Text = ResStrings.ItemListForm_ItemListForm_Load_Only_New;
            toolCheckBoxOnlyNew.Enabled = true;
            toolCheckBoxOnlyNew.Click += new EventHandler(toolCheckBoxOnlyNew_Click);
            toolStripItemListForm.Items.Insert(2, (ToolStripItem)toolCheckBoxOnlyNew);

            _validateUid = new Regex(@"[0-7]{10}");           

            _db = new MainDBClass();
           
            _db.OpenDB();
            _columnInfo = _db.GetColumnInfo();

            loadImageFromLotIDToolStripMenuItem.Text = string.Format(ResStrings.ItemListForm_ItemListForm_Load_Load_image, _columnInfo[1]);

            GetProduct();
            _dt = new DataTable();         


            for (int i = 0; i < _columnInfo.Count; i++)
                _dt.Columns.Add(_columnInfo[i].ToString(), typeof(string));

            _dt.Columns.Add(ResStrings.str_ImportStatus, typeof(string));

            dataGridViewImport.DataSource = _dt;
            dataGridViewImport.Columns[ResStrings.str_ImageImport].DisplayIndex = dataGridViewImport.Columns.Count - 1;

            Font ft = new Font(dataGridViewImport.Font, FontStyle.Bold);
            dataGridViewImport.ColumnHeadersDefaultCellStyle.Font = ft;

            if (!bShowImage)
            {
                addImageToolStripMenuItem.Visible = false;
                dataGridViewImport.Columns[ResStrings.str_ImageImport].Visible = false;
                dataGridViewImport.RowTemplate.Height = 25;             

            }

            if (listTag != null)
            {
                ProcessList();
            }
            else
            {
                timer1.Enabled = true;                
            }
           
        }

        private void ProcessList()
        {
            if (listTag == null) return;
            if (listTag.Count == 0) return;

            _dt.Rows.Clear();

            foreach (string tagId in listTag)
            {
                string status = ResStrings.str_Ok;
                //todo add get lot id or tag id depend on null value           

                if (string.IsNullOrEmpty(tagId)) return;


                string[] pi = _db.RecoverProductInfo(tagId);

                if (pi != null)
                {
                    object[] row = new object[_columnInfo.Count + 1];
                    for (int loop = 0; loop < _columnInfo.Count; loop++)
                        row[loop] = pi[loop];

                    row[_columnInfo.Count] = status;
                    _dt.Rows.Add(row);
                }
                else
                {
                    object[] row = new object[_columnInfo.Count + 1];
                    row[0] = tagId;
                    row[1] = ResStrings.str_Unreferenced;
                    row[_columnInfo.Count] = status;
                    _dt.Rows.Add(row);
                }
            }


                dataGridViewImport.DataSource = null;
                dataGridViewImport.DataSource = _dt.DefaultView;
                dataGridViewImport.Columns[ResStrings.str_ImageImport].DisplayIndex = dataGridViewImport.Columns.Count - 1;
                CheckDg(-1);    
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
                        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
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
                        //IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        excelReader.IsFirstRowAsColumnNames = true;
                        ds = excelReader.AsDataSet();
                        stream.Close();
                    }
                    dataGridViewImport.DataSource = null;


                    if (ds != null)
                    {
                        DataTable tmpDt = new DataTable();

                        for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                        {
                            string colName = ds.Tables[0].Columns[j].ColumnName;
                            if (colName.Contains("#"))
                            {
                                ds.Tables[0].Columns[j].ColumnName = colName.Replace("#", ".");
                            }
                        }


                        foreach (DataColumn colName in ds.Tables[0].Columns)
                        {
                            if (colName.ColumnName.Equals(_columnInfo[0].ToString())) _indexTagId = colName.Ordinal;
                            if (colName.ColumnName.Equals(_columnInfo[1].ToString())) _indexLotId = colName.Ordinal;
                            tmpDt.Columns.Add(colName.ColumnName, typeof(string));

                        }
                        tmpDt.Columns.Add(ResStrings.str_ImportStatus, typeof(string));

                        string selectString = "[" + _columnInfo[0] + "] is NOT NULL OR [" + _columnInfo[1] + "] is NOT NULL";
                        DataRow[] drResults = ds.Tables[0].Select(selectString);

                        int nbItemsToImport = drResults.Length;
                        int nIndex = 0;
                        int refresh = 0;

                        if (_db != null)
                            _db.startTranscation();
                        foreach (DataRow dr in drResults)
                        {
                            toolStripStatusLabelInfo.Text = String.Format(ResStrings.ItemListForm_Import_Read_and_Check_Item, ++nIndex, nbItemsToImport);
                            refresh++;
                            if (refresh > 10)
                            {
                                refresh = 0;
                                Application.DoEvents();
                            }

                            string status = ResStrings.str_Ok;
                            bool alreadyinDb = false;
                            string tagId = dr.ItemArray[_indexTagId].ToString();
                            string lotId = dr.ItemArray[_indexLotId].ToString();

                            if ((string.IsNullOrEmpty(tagId)) & (string.IsNullOrEmpty(lotId))) continue;

                            if (string.IsNullOrEmpty(tagId))
                            {
                                status = ResStrings.str_Missing_TagUID;
                            }
                            /*else if (!ValidateUID.IsMatch(tagID))
                            {
                                status = "Bad UID Format or Value";
                            }  */
                            else if (string.IsNullOrEmpty(lotId))
                            {
                                status = ResStrings.str_Missing_LotID;
                            }
                            else
                            {
                                string[] pi = _db.RecoverProductInfo(tagId);

                                if (pi != null)
                                {
                                    alreadyinDb = true;
                                    if (!pi[1].Equals(lotId))
                                    {
                                        status = string.Format(ResStrings.str_TagUID_already_associated, _columnInfo[1], pi[1]);
                                    }
                                    else
                                    {
                                        status = ResStrings.str_Product_already_in_database;
                                    }
                                }
                                string tagwithLotInDb = _db.getProductTagID(lotId);
                                if (!string.IsNullOrEmpty(tagwithLotInDb))
                                {
                                    if (!tagwithLotInDb.Equals(tagId))
                                        status = string.Format(ResStrings.str_LotID_already_associated, _columnInfo[0], tagwithLotInDb);
                                }
                            }

                            object[] row = new object[dr.ItemArray.Length + 1];

                            for (int loop = 0; loop < dr.ItemArray.Length; loop++)
                                row[loop] = dr.ItemArray[loop];
                            row[dr.ItemArray.Length] = status;

                            if (_importAll)
                                tmpDt.Rows.Add(row);
                            else
                            {
                                if (!alreadyinDb) tmpDt.Rows.Add(row);
                            }

                        }
                        if (_db != null)
                            _db.endTranscation();

                        dataGridViewImport.DataSource = tmpDt;
                        dataGridViewImport.Columns[ResStrings.str_ImageImport].DisplayIndex = dataGridViewImport.Columns.Count - 1;

                        toolStripStatusLabelInfo.Text = String.Format(ResStrings.ItemListForm_Import_Imported_Item,tmpDt.Rows.Count, nbItemsToImport);
                        Application.DoEvents();
                    }
                }
                catch (IOException exp)
                {
                    ExceptionMessageBox.Show(exp);
                    MessageBox.Show(ResStrings.ItemListForm_Import, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    ExceptionMessageBox.Show(ex);
                }
            }
        }
//        private void import2()
//        {
//            if (!string.IsNullOrEmpty(_pathImport))
//            {
//                DataSet ds = null;
//                try
//                {
//                    if (Path.GetExtension(_pathImport).Contains("xlsx")) // 2010
//                    {
//                        FileInfo newFile = new FileInfo(_pathImport);
//                        using (ExcelPackage pck = new ExcelPackage(newFile))
//                        {
//                            ExcelWorksheet ws = pck.Workbook.Worksheets[1];
//                            string sheetName = ws.Name;
//                            ds = _db.ImportExcelToDS(_pathImport, sheetName);
//                        }
//                    }
//                    else if (Path.GetExtension(_pathImport).Contains("csv"))
//                    {
//                        ds = _db.ImportCSVToDS(_pathImport);
//
//                    }
//                    else
//                    {
//                        ds = _db.ImportExcelToDS(_pathImport);
//                    }
//
//                    dataGridViewImport.DataSource = null;
//                    
//
//                    if (ds != null)
//                    {
//                        DataTable tmpDT = new DataTable();
//
//                        for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
//                        {
//                            string colName = ds.Tables[0].Columns[j].ColumnName;
//                            if (colName.Contains("#"))
//                            {
//                                ds.Tables[0].Columns[j].ColumnName = colName.Replace("#", "."); ;
//                            }
//                        }
//                       
//
//                        foreach (DataColumn colName in ds.Tables[0].Columns)
//                        {
//                            if (colName.ColumnName.Equals(_columnInfo[0].ToString())) _indexTagId = colName.Ordinal;
//                            if (colName.ColumnName.Equals(_columnInfo[1].ToString())) _indexLotId = colName.Ordinal;
//                            tmpDT.Columns.Add(colName.ColumnName, typeof(string));
//
//                        }
//                        tmpDT.Columns.Add(ResStrings.str_ImportStatus, typeof(string));
//
//                        string selectString = "[" + _columnInfo[0] + "] is NOT NULL OR [" + _columnInfo[1] + "] is NOT NULL";                        
//                        DataRow[] drResults = ds.Tables[0].Select(selectString);
//
//                        int nbItemsToImport = drResults.Length;
//                        int nIndex = 0;
//                        int refresh = 0;
//
//                        if (_db != null)
//                            _db.startTranscation();
//                       foreach (DataRow dr in drResults)                       
//                        {                           
//                            toolStripStatusLabelInfo.Text = String.Format(ResStrings.ItemListForm_Import_Read_and_Check_Item,++nIndex,nbItemsToImport);
//                            refresh++;
//                            if (refresh > 10)
//                            {
//                                refresh = 0;
//                                Application.DoEvents();
//                            }
//
//                            string status = ResStrings.str_Ok;
//                            bool alreadyinDB = false;
//                            string tagID = dr.ItemArray[_indexTagId].ToString();
//                            string lotID = dr.ItemArray[_indexLotId].ToString();
//
//                            if ((string.IsNullOrEmpty(tagID)) & (string.IsNullOrEmpty(lotID))) continue;
//
//                            if (string.IsNullOrEmpty(tagID))
//                            {
//                                status = ResStrings.str_Missing_TagUID;
//                            }
//                            /*else if (!ValidateUID.IsMatch(tagID))
//                            {
//                                status = "Bad UID Format or Value";
//                            }  */                         
//                            else if (string.IsNullOrEmpty(lotID))
//                            {
//                                status = ResStrings.str_Missing_LotID;
//                            }
//                            else
//                            {
//                                string[] pi = _db.RecoverProductInfo(tagID);
//
//                                if (pi != null)
//                                {
//                                    alreadyinDB = true;
//                                    if (!pi[1].Equals(lotID))
//                                    {
//                                        status = "TagUID already associated with " + _columnInfo[1] + "  " + pi[1];
//                                    }
//                                    else
//                                    {
//                                        status = ResStrings.str_Product_already_in_database;
//                                    }
//                                }
//                                string tagwithLotInDB = _db.getProductTagID(lotID);
//                                if (!string.IsNullOrEmpty(tagwithLotInDB))
//                                {
//                                    if (!tagwithLotInDB.Equals(tagID))
//                                        status = "LotID already associated with " + _columnInfo[0] + "  " + tagwithLotInDB;
//                                }
//                            }
//                          
//                            object[] row = new object[dr.ItemArray.Length + 1];
//
//                            for (int loop = 0; loop < dr.ItemArray.Length; loop++)
//                                row[loop] = dr.ItemArray[loop];
//                            row[dr.ItemArray.Length] = status;
//
//                            if (_importAll)
//                                tmpDT.Rows.Add(row);
//                            else
//                            {
//                                if (!alreadyinDB) tmpDT.Rows.Add(row);
//                            }
//
//                        }
//                        if (_db!= null)
//                         _db.endTranscation();
//
//                        dataGridViewImport.DataSource = tmpDT;
//                        dataGridViewImport.Columns[ResStrings.str_ImageImport].DisplayIndex = dataGridViewImport.Columns.Count - 1;
//
//                        toolStripStatusLabelInfo.Text = String.Format(ResStrings.ItemListForm_Import_Imported_Item, nbItemsToImport, nbItemsToImport);
//                        Application.DoEvents();
//                    }
//
//
//                    //updateDG(-1);
//                    //CheckDG(-1);
//                }
//                catch (IOException exp)
//                {
//                    MessageBox.Show(ResStrings.ItemListForm_Import, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
//                }
//                catch (Exception ex)
//                {
//                    ExceptionMessageBox.Show(ex);
//                }
//            }
//        }
        private void toolStripButtonImport_Click(object sender, EventArgs e)
        {
             DialogResult ret = openFileDialogXLS.ShowDialog();
             if (ret == DialogResult.OK)
             {
                 _pathImport = Path.GetFullPath(openFileDialogXLS.FileName);
                 toolStripItemListForm.Enabled = false;
                 Import();
                 CheckDuplicate();
                 UpdateDg(-1);
                 toolStripItemListForm.Enabled = true;
             }             

        }

        private void CheckDuplicate()
        {
            ArrayList listTagId = new ArrayList();
            ArrayList listLotId = new ArrayList();

            foreach (DataGridViewRow oRow in dataGridViewImport.Rows)
            {
                int nRow = oRow.Index;
                string tagId = oRow.Cells[_columnInfo[0].ToString()].Value.ToString();
                string lotId = oRow.Cells[_columnInfo[1].ToString()].Value.ToString();

                if (!string.IsNullOrEmpty(tagId))
                {
                    if (listTagId.Contains(tagId))
                    {
                        dataGridViewImport.Rows[nRow].Cells[ResStrings.str_ImportStatus].Value = string.Format(ResStrings.ItemListForm_CheckDuplicate_Duplicate, _columnInfo[0]);
                    }
                    else
                    {
                        listTagId.Add(tagId);
                    }
                }
                if(!string.IsNullOrEmpty(lotId))
                {
                    if (listLotId.Contains(lotId))
                    {
                        dataGridViewImport.Rows[nRow].Cells[ResStrings.str_ImportStatus].Value = string.Format(ResStrings.ItemListForm_CheckDuplicate_Duplicate, _columnInfo[1]);
                    }
                    else
                    {
                        listLotId.Add(lotId);
                    }
                }
            }
        }

  private void UpdateDg(int rowSelect)
  {

        int checkedGood = 0;
        if (rowSelect < 0)
        {
            foreach (DataGridViewRow dgvRow in dataGridViewImport.Rows)
            {
                Object cellValue = dgvRow.Cells[ResStrings.str_ImportStatus].Value;
                if (cellValue != null)
                {

                    string strCase = cellValue.ToString();
                    if (strCase.Equals(ResStrings.str_Ok))
                    {
                        dgvRow.DefaultCellStyle.ForeColor = Color.Green;
                        checkedGood++;
                    }
                    else
                    {
                        dgvRow.DefaultCellStyle.ForeColor = Color.Red;
                    }

                }
            }
            toolStripStatusLabelInfo.Text += String.Format(ResStrings.ItemListForm_UpdateDg_Checked, checkedGood++, dataGridViewImport.Rows.Count);
        }
        else
        {
            Object cellValue = dataGridViewImport.Rows[rowSelect].Cells[ResStrings.str_ImportStatus].Value;
            if (cellValue != null)
            {

                string strCase = cellValue.ToString();
                if (strCase.Equals(ResStrings.str_Ok))
                {
                    dataGridViewImport.Rows[rowSelect].DefaultCellStyle.ForeColor = Color.Green;
                }
                else  dataGridViewImport.Rows[rowSelect].DefaultCellStyle.ForeColor = Color.Red; 
                
            }
        }
    }

        public void CheckDg(int rowSelect)
        {
            try
            {
                if (rowSelect < 0)
                {
                    int nbItemsToCheck = dataGridViewImport.Rows.Count;
                    int nIndex = 0;                
                                      
                    foreach (DataGridViewRow dgvRow in dataGridViewImport.Rows)
                    {
                        toolStripStatusLabelInfo.Text = String.Format(ResStrings.ItemListForm_CheckDg_Check_Item, ++nIndex, nbItemsToCheck);                       
                        Application.DoEvents();
                       
                        string status = ResStrings.str_Ok;

                        string tagId = dgvRow.Cells[_columnInfo[0].ToString()].Value.ToString();
                        string lotId = dgvRow.Cells[_columnInfo[1].ToString()].Value.ToString();

                        if ((string.IsNullOrEmpty(tagId)) & (string.IsNullOrEmpty(lotId))) continue;

                        if (string.IsNullOrEmpty(tagId))
                        {
                            status = ResStrings.str_Missing_TagUID;
                        }
                        /*else if (!ValidateUID.IsMatch(tagID))
                        {
                            status = "Bad UID Format or Value";
                        }*/
                        else if (string.IsNullOrEmpty(lotId))
                        {
                            status = ResStrings.str_Missing_LotID;
                        }
                        else
                        {
                            string[] pi = _db.RecoverProductInfo(tagId);
                            if (pi != null)                           
                            {                               
                                if (!pi[1].Equals(lotId))
                                {
                                    status = string.Format(ResStrings.str_TagUID_already_associated, _columnInfo[1], pi[1]);
                                }                               
                                else
                                {
                                    bool bsame = true;
                                    foreach (DictionaryEntry  entry in _columnInfo )
                                    {
                                        if (string.IsNullOrEmpty(pi[(int)entry.Key])) continue;
                                        if (pi[(int)entry.Key].Equals(" ")) continue;
                                        if (!pi[(int)entry.Key].Equals(dgvRow.Cells[entry.Value.ToString()].Value.ToString()))
                                            bsame = false;
                                    }

                                    if (bsame)
                                    status = ResStrings.str_Product_already_in_database;
                                }
                               
                            }
                            string tagwithLotInDb = _db.getProductTagID(lotId);
                            if (!string.IsNullOrEmpty(tagwithLotInDb))                           
                            {                              
                                if (!tagwithLotInDb.Equals(tagId))
                                    status = string.Format(ResStrings.str_LotID_already_associated, _columnInfo[0], tagwithLotInDb);
                            }
                        }                      
                        dgvRow.Cells[ResStrings.str_ImportStatus].Value = status;
                    }                   
                    toolStripStatusLabelInfo.Text = String.Format(ResStrings.ItemListForm_CheckDg_Checked_Item, nbItemsToCheck, nbItemsToCheck);
                    Application.DoEvents();
                }
                else
                {
                    string status = ResStrings.str_Ok;

                    string tagId = dataGridViewImport.Rows[rowSelect].Cells[_columnInfo[0].ToString()].Value.ToString();
                    string lotId = dataGridViewImport.Rows[rowSelect].Cells[_columnInfo[1].ToString()].Value.ToString();

                    if ((string.IsNullOrEmpty(tagId)) & (string.IsNullOrEmpty(lotId))) return;

                    if (string.IsNullOrEmpty(tagId))
                    {
                        status = ResStrings.str_Missing_TagUID;
                    }
                    /*else if (!ValidateUID.IsMatch(tagID))
                    {
                        status = "Bad UID Format or Value";
                    }*/
                    else if (string.IsNullOrEmpty(lotId))
                    {
                        status = ResStrings.str_Missing_LotID;
                    }
                    else
                    {
                        string[] pi = _db.RecoverProductInfo(tagId);

                        if (pi != null)
                        {
                            if (!pi[1].Equals(lotId))
                            {
                                status = string.Format(ResStrings.str_TagUID_already_associated, _columnInfo[1], pi[1]);
                            }                            
                            else
                            {
                                bool bsame = true;
                                foreach (DictionaryEntry entry in _columnInfo)
                                {
                                    if (!pi[(int)entry.Key].Equals(dataGridViewImport.Rows[rowSelect].Cells[entry.Value.ToString()].Value.ToString()))
                                        bsame = false;
                                }

                                if (bsame)
                                    status = ResStrings.str_Product_already_in_database;
                            }                            
                        }
                        string tagwithLotInDb = _db.getProductTagID(lotId);
                        if (!string.IsNullOrEmpty(tagwithLotInDb))
                        {
                            if (!tagwithLotInDb.Equals(tagId))
                                status = string.Format(ResStrings.str_LotID_already_associated, _columnInfo[0], tagwithLotInDb);
                        }
                    }

                    dataGridViewImport.Rows[rowSelect].Cells[ResStrings.str_ImportStatus].Value = status;
                }
                UpdateDg(rowSelect);
            }           
            catch (Exception ex)
            {
                ExceptionMessageBox.Show(ex);
            }
        }

        private void toolStripButtonGet_Click(object sender, EventArgs e)
        {
            string tagId = textBoxTagUID.Text;
            string lotId = textBoxLotID.Text;

            string status = ResStrings.str_Ok;
            _dt.Rows.Clear();

            
            //todo add get lot id or tag id depend on null value           

            if ((string.IsNullOrEmpty(tagId)) & (string.IsNullOrEmpty(lotId))) return;


            if (string.IsNullOrEmpty(tagId))
                tagId = _db.getProductTagID(lotId);
          
                       
                string[] pi = _db.RecoverProductInfo(tagId);

                if (pi != null)
                {
                    object[] row = new object[_columnInfo.Count + 1];
                    for (int loop = 0; loop < _columnInfo.Count; loop++)
                        row[loop] = pi[loop];                   

                    row[_columnInfo.Count] = status;
                    _dt.Rows.Add(row);
                }
                else
                {
                    object[] row = new object[_columnInfo.Count + 1];
                    row[0] = tagId;
                    row[1] = ResStrings.str_Unreferenced;
                    row[_columnInfo.Count] = status;
                    _dt.Rows.Add(row);
                }


                dataGridViewImport.DataSource = null;
                dataGridViewImport.DataSource = _dt.DefaultView;
                dataGridViewImport.Columns[ResStrings.str_ImageImport].DisplayIndex = dataGridViewImport.Columns.Count - 1;
                CheckDg(-1);        
        }

        private void ItemListForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _db.CloseDB();
            if (Lvd != null)
            Lvd.GetProduct();
        }

        private void toolStripButtonApply_Click(object sender, EventArgs e)
        {
            bool ret = false;
            int nRowIndex;            
            int nRowSaved = 0;
            dataGridViewImport.EndEdit();

            int nbItemsToApply = dataGridViewImport.Rows.Count;
            int nIndex = 0;
            int refresh = 0;

            _db.startTranscation();
            for (nRowIndex = 0; nRowIndex < dataGridViewImport.Rows.Count; nRowIndex++)
            {

                toolStripStatusLabelInfo.Text = String.Format(ResStrings.ItemListForm_toolStripButtonApply_Click_Apply_Items, ++nIndex, nbItemsToApply);
                refresh++;
                if (refresh > 10)
                {
                    refresh = 0;
                    Application.DoEvents();
                }

                ProductClassTemplate pc = new ProductClassTemplate();

                if (!dataGridViewImport.Rows[nRowIndex].Cells[ResStrings.str_ImportStatus].FormattedValue.ToString().Equals(ResStrings.str_Ok)) continue;

                if (string.IsNullOrEmpty(dataGridViewImport.Rows[nRowIndex].Cells[_columnInfo[0].ToString()].FormattedValue.ToString()))
                    continue;

                //pc.tagUID = (string)Convert.ToInt64(dataGridViewImport.Rows[nRowIndex].Cells[ColumnInfo[0].ToString()].Value).ToString();
                //pc.tagUID = dataGridViewImport.Rows[nRowIndex].Cells[ColumnInfo[0].ToString()].Value.ToString().PadRight(10, ' ');
                pc.tagUID = dataGridViewImport.Rows[nRowIndex].Cells[_columnInfo[0].ToString()].Value.ToString();

                if (!string.IsNullOrEmpty(dataGridViewImport.Rows[nRowIndex].Cells[_columnInfo[1].ToString()].FormattedValue.ToString()))
                    pc.reference = dataGridViewImport.Rows[nRowIndex].Cells[_columnInfo[1].ToString()].Value.ToString();
                for (int i = 0; i < _columnInfo.Count-2; i++)
                {                  
                    if (!string.IsNullOrEmpty(dataGridViewImport.Rows[nRowIndex].Cells[_columnInfo[i + 2].ToString()].FormattedValue.ToString()))
                        pc.productInfo[i] = dataGridViewImport.Rows[nRowIndex].Cells[_columnInfo[i + 2].ToString()].Value.ToString();
                }

                if (!string.IsNullOrEmpty(pc.tagUID))
                {
                      if (string.IsNullOrEmpty(pc.reference)) pc.reference = " ";
                      for (int i = 0; i < _columnInfo.Count - 2; i++)
                      {
                          if (string.IsNullOrEmpty(pc.productInfo[i])) pc.productInfo[i] = " ";
                      }
                    
                      ret =  _db.StoreProduct(pc);
                      if (ret) nRowSaved++;                      
                }                
            }
            _db.endTranscation();
            toolStripStatusLabelInfo.Text = String.Format(ResStrings.ItemListForm_toolStripButtonApply_Click_Apply_Items, nbItemsToApply, nbItemsToApply);
            Application.DoEvents();
            GetProduct();
            if (!cbAutoFill.Checked)
            MessageBox.Show(string.Format(ResStrings.ItemListForm_toolStripButtonApply_Product_s__saved, nRowSaved, nRowIndex), ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
           
        }

      
        private void toolStripButtonExit_Click(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate { Close(); });
        }

        private void textBoxTagUID_Click(object sender, EventArgs e)
        {
            textBoxLotID.Text = null;
        }

        private void textBoxLotID_Click(object sender, EventArgs e)
        {
            textBoxTagUID.Text = null; 
        }

        private void dataGridViewImport_Sorted(object sender, EventArgs e)
        {
            UpdateDg(-1);
        }

        private void dataGridViewImport_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CheckDg(e.RowIndex);
        }

        private void overwriteToolStripMenuItem_Click(object sender, EventArgs e)
        {              
              foreach(DataGridViewRow oRow in dataGridViewImport.SelectedRows)
              {                  
                    oRow.Cells[ResStrings.str_ImportStatus].Value = ResStrings.str_Ok;                  
              }
              UpdateDg(-1);
         }

        private void removeProductInImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow oRow in dataGridViewImport.SelectedRows)
            {   
                     oRow.Cells[ResStrings.str_ImportStatus].Value = ResStrings.str_Removed;                                 
            }
            UpdateDg(-1);             
        }

        private void addProductToListToFindByLedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Lvd == null)
                return;
           
            foreach (DataGridViewRow oRow in dataGridViewImport.SelectedRows)
            {
                string tagId = oRow.Cells[_columnInfo[0].ToString()].Value.ToString();
                string selectString = "[" + ResStrings.BoxModeConfig_UpdateGroup_TagUID + "] = '" + tagId + "'";
                DtProductRef.CaseSensitive = true;
                DataRow[] productInfo = DtProductWithColum.Select(selectString);

                if (productInfo.Length > 0)
                {

                    Lvd.DtProductToFind.CaseSensitive = true;
                    DataRow[] productFind = Lvd.DtProductToFind.Select(selectString);

                    if (productFind.Length == 0)
                    {
                        object[] param = new object[_columnInfo.Count];
                        for (int i = 0; i < _columnInfo.Count; i++)
                            param[i] = productInfo[0].ItemArray[i];
                        Lvd.DtProductToFind.Rows.Add(param);
                    }
                }

            }
            if ((Lvd.tslf != null) && (Lvd.tslf.Visible == false))
            {
                Lvd.tslf.Show();
                Lvd.tslf.refreshDataGrid();
            }
        }

        private bool _hasError = false;
        private void textBoxTagUID_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F && e.Modifiers == Keys.Control)
            {
                groupBoxAutoFill.Visible = !groupBoxAutoFill.Visible;
            }
            if (e.KeyValue == (int)Keys.Return)
            {
                if (_hasError) return;

                Regex  regex = new Regex("^[0-9]+$");

                if (regex.IsMatch(textBoxTagUID.Text))
                    toolStripButtonGet_Click(this, null);
                else
                    return;

                if (cbAutoFill.Checked)
                {

                    if (dataGridViewImport.Rows[0].Cells[_columnInfo[1].ToString()].Value.ToString().Equals(ResStrings.str_Unreferenced))
                    {
                        dataGridViewImport.Rows[0].Cells[_columnInfo[1].ToString()].Value = txtConstant.Text + txtVar.Text;

                        int val = int.Parse(txtVar.Text);
                        val++;
                        txtVar.Text = val.ToString();
                        toolStripButtonApply_Click(this, null);
                        textBoxTagUID.Text = string.Empty;
                        textBoxTagUID.Focus();

                    }
                    else
                    {
                        _hasError = true;
                        TopMostMessageBox.Show(ResStrings.str_Product_already_in_database);
                        textBoxTagUID.Text = string.Empty;
                        textBoxTagUID.Focus();
                        _hasError = false;
                    }
                }
            }
        }

        private void addImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridView dgv = dataGridViewImport;
          

            if (dgv == null) return;
            if (dgv.SelectedRows.Count > 0)
            {

                if (openFileDialogImage.ShowDialog() == DialogResult.OK)
                {
                    Image img = Image.FromFile(openFileDialogImage.FileName);
                    Image imgResize = ImageUtils.ResizeImage(img, dgv.SelectedRows[0].Height);

                    string imageName = Path.GetFileNameWithoutExtension(openFileDialogImage.FileName);
                    string imageExtension = Path.GetExtension(openFileDialogImage.FileName);

                    byte[] imgStream = null;

                    switch (imageExtension.ToLower())
                    {
                        case ".jpg":
                        case ".jpeg": imgStream = ImageUtils.imageToByteArray(imgResize, ImageFormat.Jpeg); break;
                        case ".bmp": imgStream = ImageUtils.imageToByteArray(imgResize, ImageFormat.Bmp); break;
                        case ".gif": imgStream = ImageUtils.imageToByteArray(imgResize, ImageFormat.Gif); break;
                        case ".png": imgStream = ImageUtils.imageToByteArray(imgResize, ImageFormat.Png); break;
                        case ".tiff": imgStream = ImageUtils.imageToByteArray(imgResize, ImageFormat.Tiff); break;
                    }

                    if (imgStream == null) return;

                    if (!_db.IsImageExist(imageName))
                    {
                        _db.AddImage(imageName, imgStream);
                    }                   

                    foreach (DataGridViewRow oRow in dgv.SelectedRows)
                    {

                        if (_db.IsImageLinkExist(oRow.Cells[_columnInfo[1].ToString()].Value.ToString()))
                            _db.DeleteImageLink(oRow.Cells[_columnInfo[1].ToString()].Value.ToString());

                        _db.AddImageLink(imageName, oRow.Cells[_columnInfo[1].ToString()].Value.ToString());
                    }
                }


                dataGridViewImport.Invalidate();
            }
            else
            {
                MessageBox.Show(ResStrings.ItemListForm_addImageToolStripMenuItem_Click_Select_the_full_row_to_add_an_image, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void dataGridViewImport_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
           
            if (!bShowImage) return;
            if (dataGridViewImport.Columns[e.ColumnIndex].Name == ResStrings.str_ImageImport)
            {
                Image img = ImageUtils.ResizeImage(Resources.No_Image_Available, dataGridViewImport.Rows[e.RowIndex].Height);
                string lotId = dataGridViewImport.Rows[e.RowIndex].Cells[_columnInfo[1].ToString()].Value.ToString();
                if (_db.IsImageLinkExist(lotId))
                {
                    string imgName = _db.getImageNameLink(lotId);
                    if (imgName != null)
                    {
                        Image imgFromDb = ImageUtils.byteArrayToImage(_db.getImage(imgName));
                        img = ImageUtils.ResizeImage(imgFromDb, dataGridViewImport.Rows[e.RowIndex].Height);
                    }
                }

                e.Value = img;
            }
        }
        private void removeImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
             DataGridView dgv = dataGridViewImport;          

            if (dgv == null) return;
            if (dgv.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow oRow in dgv.SelectedRows)
                {
                    if (_db.IsImageLinkExist(oRow.Cells[_columnInfo[1].ToString()].Value.ToString()))
                        _db.DeleteImageLink(oRow.Cells[_columnInfo[1].ToString()].Value.ToString());                    
                }
                dataGridViewImport.Invalidate();
            }
            else
            {
                MessageBox.Show(ResStrings.ItemListForm_removeImageToolStripMenuItem_Click_Select_the_full_row_to_remove_an_image, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void toolStripButtonFillGrid_Click(object sender, EventArgs e)
        {
            if (_auto != null)
                _auto.Close();
            _auto = new AutoFillBox(this,Lvd);
            _auto.Show();
        }
        private void imageListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_listimgFrm != null)
                _listimgFrm.Close();
            _listimgFrm = new ImageListForm();
            _listimgFrm.ShowDialog();
        }
        private void addImageFromLibraryToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataGridView dgv = dataGridViewImport;


            if (dgv == null) return;
            if (dgv.SelectedRows.Count > 0)
            {
                if (_listimgFrm != null)
                    _listimgFrm.Close();
                _listimgFrm = new ImageListForm();
                _listimgFrm.ShowDialog();

                if (!string.IsNullOrEmpty(_listimgFrm.ImageFromList))
                {
                    foreach (DataGridViewRow oRow in dgv.SelectedRows)
                    {
                        if (_db.IsImageLinkExist(oRow.Cells[_columnInfo[1].ToString()].Value.ToString()))
                            _db.DeleteImageLink(oRow.Cells[_columnInfo[1].ToString()].Value.ToString());

                        _db.AddImageLink(_listimgFrm.ImageFromList, oRow.Cells[_columnInfo[1].ToString()].Value.ToString());
                    }

                    dataGridViewImport.Invalidate();
                }
            }
        }

        private void textBoxTagUID_TextChanged(object sender, EventArgs e)
        {           
            if (!cbAutoFill.Checked)
            {
                string selectString = "[" + _columnInfo[0] + "] Like '" + textBoxTagUID.Text + "%'"; 
                Dv = new DataView(DtProductWithColum);
                Dv.RowFilter = selectString;
                dataGridViewImport.DataSource = null;
                dataGridViewImport.DataSource = Dv;
                toolStripStatusLabelInfo.Text = String.Format(ResStrings.ItemListForm_textBoxTagUID_TextChangedItemfound, Dv.Count);
                dataGridViewImport.Columns[ResStrings.str_ImageImport].DisplayIndex = dataGridViewImport.Columns.Count - 1;

                UpdateDg(-1);  
                
            }
        }

        private void textBoxLotID_TextChanged(object sender, EventArgs e)
        {
            if (!cbAutoFill.Checked)
            {
                string selectString = "[" + _columnInfo[1] + "] Like '" + textBoxLotID.Text + "%'";
                Dv = new DataView(DtProductWithColum);
                Dv.RowFilter = selectString;
                dataGridViewImport.DataSource = null;
                dataGridViewImport.DataSource = Dv;
                toolStripStatusLabelInfo.Text = String.Format(ResStrings.ItemListForm_textBoxTagUID_TextChangedItemfound, Dv.Count);
                dataGridViewImport.Columns[ResStrings.str_ImageImport].DisplayIndex = dataGridViewImport.Columns.Count - 1;
                UpdateDg(-1);  
                
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            dataGridViewImport.DataSource = null;
            dataGridViewImport.DataSource = DtProductWithColum;
            toolStripStatusLabelInfo.Text = String.Format(ResStrings.ItemListForm_textBoxTagUID_TextChangedItemfound, DtProductRef.Rows.Count);
            dataGridViewImport.Columns[ResStrings.str_ImageImport].DisplayIndex = dataGridViewImport.Columns.Count - 1;
            
            UpdateDg(-1);
        }

        private void textBoxTagUID_Leave(object sender, EventArgs e)
        {
            
        }

        private void textBoxLotID_Leave(object sender, EventArgs e)
        {               
           
        }

        private void loadImageFromLotIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Hashtable file = new Hashtable();
                DataGridView dgv = dataGridViewImport;

                DialogResult result = folderBrowserDialogImage.ShowDialog();
                if (result == DialogResult.OK)
                {
                    foreach (string f in Directory.GetFiles(folderBrowserDialogImage.SelectedPath))
                    {
                        if (!file.ContainsKey(Path.GetFileNameWithoutExtension(f)))
                            file.Add(Path.GetFileNameWithoutExtension(f), f);
                    }


                    foreach (DataGridViewRow oRow in dgv.Rows)
                    {
                        if (file.ContainsKey(oRow.Cells[_columnInfo[1].ToString()].Value.ToString()))
                        {
                            string path = (string)file[oRow.Cells[_columnInfo[1].ToString()].Value.ToString()];

                            Image img = Image.FromFile(path);
                            Image imgResize = ImageUtils.ResizeImage(img, oRow.Height);

                            string imageName = Path.GetFileNameWithoutExtension(path);
                            string imageExtension = Path.GetExtension(path);

                            byte[] imgStream = null;

                            switch (imageExtension.ToLower())
                            {
                                case ".jpg":
                                case ".jpeg": imgStream = ImageUtils.imageToByteArray(imgResize, ImageFormat.Jpeg); break;
                                case ".bmp": imgStream = ImageUtils.imageToByteArray(imgResize, ImageFormat.Bmp); break;
                                case ".gif": imgStream = ImageUtils.imageToByteArray(imgResize, ImageFormat.Gif); break;
                                case ".png": imgStream = ImageUtils.imageToByteArray(imgResize, ImageFormat.Png); break;
                                case ".tiff": imgStream = ImageUtils.imageToByteArray(imgResize, ImageFormat.Tiff); break;
                            }

                            if (imgStream == null) return;

                            if (!_db.IsImageExist(imageName))
                            {
                                _db.AddImage(imageName, imgStream);
                            }

                            if (_db.IsImageLinkExist(oRow.Cells[_columnInfo[1].ToString()].Value.ToString()))
                                _db.DeleteImageLink(oRow.Cells[_columnInfo[1].ToString()].Value.ToString());

                            _db.AddImageLink(imageName, oRow.Cells[_columnInfo[1].ToString()].Value.ToString());
                        }
                    }
                    dataGridViewImport.Invalidate();
                }
            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }
        }

        private void toolStripButtonExport_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToExcel();
            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }
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

              //  dtColumnInfo[] listCol = db.GetdtColumnInfo();

                DataTable dataToexport;

                if (Dv != null) dataToexport = Dv.ToTable();
                else
                    dataToexport = DtProductWithColum;

                if (Path.GetExtension(fileSaveName).Contains("xlsx")) // 2010
                {
                    FileInfo newFile = new FileInfo(fileSaveName);
                    ExcelPackage pck = new ExcelPackage(newFile);
                    {
                        //Create the worksheet
                        ExcelWorksheet ws1 = pck.Workbook.Worksheets.Add(ResStrings.str_Export);
                        //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                        ws1.Cells.Style.Font.Size = 10;
                        ws1.Cells.Style.Font.Name = "Verdana";

                        ws1.Cells["A1"].LoadFromDataTable(ConvertForExport(dataToexport), true);

                        ws1.Cells[1, 1, 1, _columnInfo.Count].Style.Font.Bold = true;
                        ws1.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws1.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);


                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                            ws1.Column(loop).AutoFit(25);                         
                        pck.Save();  
                    }
                }
                else
                {
                    //DBClassSQLite.Export(ConvertForExport(dataToexport), fileSaveName, "Export");                   
                    new ErrorExportDLG().Show();
                }
                if (File.Exists(fileSaveName))
                Process.Start(fileSaveName);
            }          
        }
        private DataTable ConvertForExport(DataTable dt)
        {

            MainDBClass db = new MainDBClass();
            db.OpenDB();
            ArrayList columnToExport = db.GetColumnToExport();
            dtColumnInfo[] colList = db.GetdtColumnInfo();
            db.CloseDB();
            if (columnToExport != null)
            {
                DataTable dtToexport = dt.Copy();
                DataColumnCollection dcc = dt.Columns;
                for (int loop = 0; loop < dcc.Count; loop++)
                {
                    if (!columnToExport.Contains(dcc[loop].ColumnName))
                        dtToexport.Columns.Remove(dcc[loop].ColumnName);
                }
                return ConvertColumnType(dtToexport, colList);
            }
            return ConvertColumnType(dt, colList);
        }
        private DataTable ConvertColumnType(DataTable dt, dtColumnInfo[] colList)
        {

            DataTable dtNewColType = new DataTable();

            foreach (dtColumnInfo col in colList)
            {
                dtNewColType.Columns.Add(col.colName, col.colType);
            }
            try
            {
                // if double value empty, is exception so return 
                dtNewColType.Load(dt.CreateDataReader(), LoadOption.OverwriteChanges);
                return dtNewColType;
            }
            catch
            {
                return dt;
            }

        }

        private void dataGridViewImport_SelectionChanged(object sender, EventArgs e)
        {
         
        }

        private void saveXlsFileDialog_FileOk(object sender, CancelEventArgs e)
        {

        }

        private PrinterForm pf = null;
        private void printAssociateTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
             DataGridView dgv = dataGridViewImport;


            if (dgv == null) return;
            if (dgv.SelectedRows.Count == 1 )
            {
                if (
                    dataGridViewImport.SelectedRows[0].Cells[_columnInfo[1].ToString()].Value.ToString()
                        .Equals(ResStrings.str_Unreferenced))
                {
                    MessageBox.Show("Selected product must have a valid reference", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    DataGridViewRow oRow = dataGridViewImport.SelectedRows[0];
                    string tagId = oRow.Cells[_columnInfo[0].ToString()].Value.ToString();
                    string LotId = oRow.Cells[_columnInfo[1].ToString()].Value.ToString();
                    if (!string.IsNullOrEmpty(tagId))
                    {
                        MessageBox.Show("Selected product is aleady associated - Remove association to print a new tag", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        if (pf == null)
                        pf = new PrinterForm(printerDevice,LotId,printInfo);

                        pf.lotID = LotId;
                        pf.ShowDialog();

                        if (pf.device != null)
                        {
                            printerDevice = pf.device;
                            printInfo = pf.printInfo;
                        }

                        if (!string.IsNullOrEmpty(pf.TagUidPrinted))
                        {
                            oRow.Cells[_columnInfo[0].ToString()].Value = pf.TagUidPrinted;
                            CheckDg(oRow.Index);    
                        }
                    }
                    
                }
            }
            else
            {
                MessageBox.Show("Please select Only one product to print and associate", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

       
    }

    public class ProductRefAndDbClass
    {
        public ProductClassTemplate Data;
      
        public MainDBClass Db;       
       
        public ProductRefAndDbClass(ProductClassTemplate data, MainDBClass db)
       
        {
            Data = data;
            Db = db;
        }
    }

    static public class TopMostMessageBox
    {
        static public DialogResult Show(string message)
        {
            return Show(message, string.Empty, MessageBoxButtons.OK);
        }

        static public DialogResult Show(string message, string title)
        {
            return Show(message, title, MessageBoxButtons.OK);
        }

        static public DialogResult Show(string message, string title,
            MessageBoxButtons buttons)
        {
            // Create a host form that is a TopMost window which will be the 
            // parent of the MessageBox.
            Form topmostForm = new Form();
            // We do not want anyone to see this window so position it off the 
            // visible screen and make it as small as possible
            topmostForm.Size = new Size(1, 1);
            topmostForm.StartPosition = FormStartPosition.Manual;
            Rectangle rect = SystemInformation.VirtualScreen;
            topmostForm.Location = new Point(rect.Bottom + 10,
                rect.Right + 10);
            topmostForm.Show();
            // Make this form the active form and make it TopMost
            topmostForm.Focus();
            topmostForm.BringToFront();
            topmostForm.TopMost = true;
            // Finally show the MessageBox with the form just created as its owner
            DialogResult result = MessageBox.Show(topmostForm, message, title,
                buttons);
            topmostForm.Dispose(); // clean it up all the way

            return result;
        }
    }
}
