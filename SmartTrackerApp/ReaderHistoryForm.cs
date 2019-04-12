using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using DataClass;
using DBClass;
using System.Configuration;
using ErrorMessage;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class ReaderHistoryForm : Form
    {
        DeviceInfo[] _deviceArray;
        UserClassTemplate[] _userArray;
        InventoryData[] _inventoryArray;
       
        MainDBClass _db;
        Hashtable _columnInfo = null;
        DataTable _tbReaderScan;
        public bool BShowImage = true;

        public DataTable DtProductRef;
        private bool bProcessed = false;

        public ReaderHistoryForm()
        {
            InitializeComponent();

            Font ft = new Font(dataGridViewAll.Font, FontStyle.Bold);
            dataGridViewAll.ColumnHeadersDefaultCellStyle.Font = ft;
            dataGridViewAdded.ColumnHeadersDefaultCellStyle.Font = ft;
            dataGridViewPresent.ColumnHeadersDefaultCellStyle.Font = ft;
            dataGridViewRemoved.ColumnHeadersDefaultCellStyle.Font = ft;            
        }

        private void ReaderHistoryForm_Load(object sender, EventArgs e)
        {

            bool.TryParse(ConfigurationManager.AppSettings["bShowImage"], out BShowImage);

            if (!BShowImage)
            {
                DataGridViewColumn dgvc1 = dataGridViewAll.Columns["ImageAll"];
                if (dgvc1 != null) dgvc1.Visible = false;
                DataGridViewColumn dvgc2 = dataGridViewPresent.Columns["ImagePresent"];
                if (dvgc2 != null)
                    dvgc2.Visible = false;
                DataGridViewColumn dgvc3 = dataGridViewRemoved.Columns["ImageRemove"];
                if (dgvc3 != null)
                    dgvc3.Visible = false;
                DataGridViewColumn dvgc4 = dataGridViewAdded.Columns["Imageadd"];
                if (dvgc4 != null)
                    dvgc4.Visible = false;

                dataGridViewAdded.RowTemplate.Height = 25;
                dataGridViewPresent.RowTemplate.Height = 25;
                dataGridViewRemoved.RowTemplate.Height = 25;
                dataGridViewAll.RowTemplate.Height = 25;
            }        
           
            _db = new MainDBClass();           
            _db.OpenDB();
            _columnInfo = _db.GetColumnInfo();
            DtProductRef = _db.RecoverAllProduct();           

            PopulateComboReader();
            PopulateComboUser();
            InitReaderScanTable();
            ProcessData();
        }

        private void PopulateComboReader()
        {
            toolStripComboBoxReader.Items.Clear();
            DeviceInfo[] tmpDevArray = _db.RecoverAllDevice();
            if (tmpDevArray == null) return;

            _deviceArray = new DeviceInfo[tmpDevArray.Length];
            tmpDevArray.CopyTo(_deviceArray, 0);


            toolStripComboBoxReader.Items.Add (ResStrings.str_All);

            foreach(DeviceInfo di in  _deviceArray)
            {
                string str = string.Format("{0}({1})", di.DeviceName, di.SerialRFID);
                toolStripComboBoxReader.Items.Add (str);
            } 
            toolStripComboBoxReader.SelectedIndex = 0;
        }

         private void PopulateComboUser()
        {
            toolStripComboBoxUser.Items.Clear();
            toolStripComboBoxUser.Items.Add(ResStrings.str_All);

            UserClassTemplate[] tmpUserArray = _db.RecoverUser();
            if (tmpUserArray != null)
            {
                _userArray = new UserClassTemplate[tmpUserArray.Length];
                tmpUserArray.CopyTo(_userArray, 0);
                foreach (UserClassTemplate uct in _userArray)
                {
                    string str = string.Format("{0} {1}", uct.firstName, uct.lastName);
                    toolStripComboBoxUser.Items.Add(str);
                }
            }
            toolStripComboBoxUser.SelectedIndex = 0;
        }

         private void InitReaderScanTable()
         {
             _tbReaderScan = new DataTable();
             _tbReaderScan.Columns.Add(ResStrings.str_Event_Date, typeof(string));
             _tbReaderScan.Columns.Add(ResStrings.str_Serial_RFID, typeof(string));
             _tbReaderScan.Columns.Add(ResStrings.str_Reader_Name, typeof(string));
             _tbReaderScan.Columns.Add(ResStrings.str_User_Scan, typeof(int));
             _tbReaderScan.Columns.Add(ResStrings.str_Door_Used, typeof(string));
             _tbReaderScan.Columns.Add(ResStrings.str_First_Name, typeof(string));
             _tbReaderScan.Columns.Add(ResStrings.str_Last_Name, typeof(string));
             _tbReaderScan.Columns.Add(ResStrings.str_All, typeof(int));
             _tbReaderScan.Columns.Add(ResStrings.str_Previous, typeof(int));
             _tbReaderScan.Columns.Add(ResStrings.str_Added, typeof(int));
             _tbReaderScan.Columns.Add(ResStrings.str_Removed, typeof(int));
             dataGridViewScan.DataSource = null;
             dataGridViewScan.DataSource = _tbReaderScan.DefaultView;

         }

         private void ProcessData()
         {
             InitReaderScanTable();

             UserClassTemplate uct = null;
             DeviceInfo di = null;

             if (toolStripComboBoxReader.SelectedIndex > 0)
                 di = _deviceArray[toolStripComboBoxReader.SelectedIndex - 1];

             if (toolStripComboBoxUser.SelectedIndex > 0)
                 uct = _userArray[toolStripComboBoxUser.SelectedIndex - 1];

             InventoryData[] invData = _db.GetInventory(di, uct);
             if (invData == null) return;
             _inventoryArray = new InventoryData[invData.Length];
             //invData.CopyTo(_inventoryArray, 0);
             int nIndex = 0;
             foreach (InventoryData dt in invData)
             {
                 try
                 {
                     DeviceInfo tmpdi = _db.RecoverDevice(dt.serialNumberDevice);
                     if (tmpdi != null)
                     {
                         _tbReaderScan.Rows.Add(dt.eventDate.ToString("G"), dt.serialNumberDevice, tmpdi.DeviceName,
                             Convert.ToInt32(dt.bUserScan), dt.userDoor.ToString(), dt.userFirstName, dt.userLastName,
                             dt.nbTagAll, dt.nbTagPresent, dt.nbTagAdded, dt.nbTagRemoved);

                         _inventoryArray[nIndex++] = dt;
                     }
                 }
                 catch (Exception exp)
                 {
                     // On affiche l'erreur.
                     ExceptionMessageBox.Show(exp);
                 }
             }
             dataGridViewScan.DataSource = null;
             dataGridViewScan.DataSource = _tbReaderScan.DefaultView;
             if (dataGridViewScan.Rows.Count > 0)
                 dataGridViewScan.Rows[0].Selected = true;

             bProcessed = true;
         }

         private void toolStripButtonGet_Click(object sender, EventArgs e)
         {
             ProcessData();
            
         }

         private void ReaderHistoryForm_FormClosing(object sender, FormClosingEventArgs e)
         {
             try
             {
                 _db.CloseDB();               
             }
             catch (Exception exp)
             {
                 // On affiche l'erreur.
                 ExceptionMessageBox.Show(exp);
             }
         }

         private void dataGridViewScan_SelectionChanged(object sender, EventArgs e)
         {
             try
             {
                 if (!bProcessed) return;
                 if (dataGridViewScan.SelectedRows.Count == 1)
                 {
                     int selectedData = dataGridViewScan.SelectedRows[0].Index;
                     UpdateTabControl(selectedData);
                 }
                 else if (dataGridViewScan.SelectedRows.Count == 2)
                 {
                     int selected1 = dataGridViewScan.SelectedRows[0].Index;
                     int selected2 = dataGridViewScan.SelectedRows[1].Index;

                     if (_inventoryArray[selected1].eventDate > _inventoryArray[selected2].eventDate)
                         UpdateTabControlCompare(selected1, selected2);
                     else
                         UpdateTabControlCompare(selected2, selected1);

                 }
                 else if (dataGridViewScan.SelectedRows.Count > 2)
                     MessageBox.Show(ResStrings.str_Select_only_one_for_display_data_or_2_to_compare_two_inventories, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
             }
             catch (Exception exp)
             {
                 ExceptionMessageBox.Show(exp);
             }

         }

         private void UpdateTabControlCompare(int selectedCurrent, int selectedPrevious)
         {

             InventoryData invData = new InventoryData(_columnInfo);
             ArrayList listPrevious = _inventoryArray[selectedPrevious].listTagAll;
             ArrayList listCurrent = _inventoryArray[selectedCurrent].listTagAll;             

             foreach (string uid in listCurrent)
             {
                 if (!invData.listTagAll.Contains(uid))
                 {
                     invData.listTagAll.Add(uid);
                     DtAndTagClass dtAndTagAll = new DtAndTagClass(this, invData.dtTagAll, uid, _db);
                     AddTagToDt(dtAndTagAll);
                 }

                 if (listPrevious.Contains(uid))
                 {
                     invData.listTagPresent.Add(uid);
                     DtAndTagClass dtAndTagPresent = new DtAndTagClass(this, invData.dtTagPresent, uid, _db);
                     AddTagToDt(dtAndTagPresent);
                 }
                 else
                 {
                     invData.listTagAdded.Add(uid);
                     DtAndTagClass dtAndTagAdded = new DtAndTagClass(this, invData.dtTagAdded, uid, _db);
                     AddTagToDt(dtAndTagAdded);
                 }
             }

             foreach (string uid in listPrevious)
             {
                 if (!listCurrent.Contains(uid))
                 {
                     invData.listTagRemoved.Add(uid);
                     DtAndTagClass dtAndTagRemove = new DtAndTagClass(this, invData.dtTagRemove, uid, _db);
                     AddTagToDt(dtAndTagRemove);
                 }
             }            

             

            dataGridViewAll.DataSource = null;
            dataGridViewAll.DataSource = invData.dtTagAll.DefaultView;

            dataGridViewPresent.DataSource = null;
            dataGridViewPresent.DataSource = invData.dtTagPresent.DefaultView;

            dataGridViewAdded.DataSource = null;
            dataGridViewAdded.DataSource = invData.dtTagAdded.DefaultView;

            dataGridViewRemoved.DataSource = null;
            dataGridViewRemoved.DataSource = invData.dtTagRemove.DefaultView;

             int nbTagGridAll = dataGridViewAll.Rows.Count;
             int nbTagGridPresent = dataGridViewPresent.Rows.Count;
             int nbTagGridAdded = dataGridViewAdded.Rows.Count;
             int nbTagGridRemove = dataGridViewRemoved.Rows.Count;

             if (nbTagGridAll < 0) nbTagGridAll = 0;
             if (nbTagGridPresent < 0) nbTagGridPresent = 0;
             if (nbTagGridAdded < 0) nbTagGridAdded = 0;
             if (nbTagGridRemove < 0) nbTagGridRemove = 0;

             tabControlInventory.TabPages[0].Text = string.Format(ResStrings.ReaderHistoryForm_UpdateTabControlCompare_All, nbTagGridAll);
             tabControlInventory.TabPages[1].Text = string.Format(ResStrings.ReaderHistoryForm_UpdateTabControlCompare_Previous, nbTagGridPresent);
             tabControlInventory.TabPages[2].Text = string.Format(ResStrings.ReaderHistoryForm_UpdateTabControlCompare_Added, nbTagGridAdded);
             tabControlInventory.TabPages[3].Text = string.Format(ResStrings.ReaderHistoryForm_UpdateTabControlCompare_Removed, nbTagGridRemove);

             DataGridViewColumn dvgc1 = dataGridViewAll.Columns["ImageAll"];
             if (dvgc1 != null)
                 dvgc1.DisplayIndex = dataGridViewAll.Columns.Count - 1;
             DataGridViewColumn dvgc2 = dataGridViewPresent.Columns["ImagePresent"];
             if (dvgc2 != null)
                 dvgc2.DisplayIndex = dataGridViewPresent.Columns.Count - 1;
             DataGridViewColumn dvgc3 = dataGridViewRemoved.Columns["ImageRemove"];
             if (dvgc3 != null)
                 dvgc3.DisplayIndex = dataGridViewRemoved.Columns.Count - 1;

             dataGridViewAdded.Columns["ImageAdd"].DisplayIndex = dataGridViewAdded.Columns.Count - 1;
         }

         private void AddTagToDt(object obj)
         {
             try
             {
                 DtAndTagClass clToProcess = (DtAndTagClass)obj;
                 //string selectString = "[" + ColumnInfo[0].ToString() + "]= " + clToProcess.tagUID;
                 string selectString = "[" + ResStrings.str_TagUID + "]= '" + clToProcess.TagUid + "'";
                 DataRow[] productInfo = DtProductRef.Select(selectString);
                 if (productInfo.Length > 0)
                 {
                     object[] param = new object[_columnInfo.Count];
                     for (int i = 0; i < _columnInfo.Count; i++)
                         param[i] = productInfo[0].ItemArray[i];

                     clToProcess.Dt.Rows.Add(param);
                 }
                 else
                 {
                     object[] param = new object[_columnInfo.Count];
                     param[0] = clToProcess.TagUid;
                     param[1] = ResStrings.str_Unreferenced;
                     for (int i = 2; i < _columnInfo.Count; i++)
                         param[i] = " ";

                     clToProcess.Dt.Rows.Add(param);
                 }
             }
             catch (Exception exp)
             {
                 ExceptionMessageBox.Show(exp);
             }
         }
         private void UpdateTabControl(int selectedData)
         {
             try
             {
                 if (_inventoryArray[selectedData] == null)
                     return;

                 dataGridViewAll.DataSource = null;
                 dataGridViewAll.DataSource = _inventoryArray[selectedData].dtTagAll.DefaultView;

                 dataGridViewPresent.DataSource = null;
                 dataGridViewPresent.DataSource = _inventoryArray[selectedData].dtTagPresent.DefaultView;

                 dataGridViewAdded.DataSource = null;
                 dataGridViewAdded.DataSource = _inventoryArray[selectedData].dtTagAdded.DefaultView;

                 dataGridViewRemoved.DataSource = null;
                 dataGridViewRemoved.DataSource = _inventoryArray[selectedData].dtTagRemove.DefaultView;

                 int nbTagGridAll = dataGridViewAll.Rows.Count;
                 int nbTagGridPresent = dataGridViewPresent.Rows.Count;
                 int nbTagGridAdded = dataGridViewAdded.Rows.Count;
                 int nbTagGridRemove = dataGridViewRemoved.Rows.Count;

                 if (nbTagGridAll < 0) nbTagGridAll = 0;
                 if (nbTagGridPresent < 0) nbTagGridPresent = 0;
                 if (nbTagGridAdded < 0) nbTagGridAdded = 0;
                 if (nbTagGridRemove < 0) nbTagGridRemove = 0;


                 tabControlInventory.TabPages[0].Text = string.Format(ResStrings.ReaderHistoryForm_UpdateTabControlCompare_All, nbTagGridAll);
                 tabControlInventory.TabPages[1].Text = string.Format(ResStrings.ReaderHistoryForm_UpdateTabControlCompare_Previous, nbTagGridPresent);
                 tabControlInventory.TabPages[2].Text = string.Format(ResStrings.ReaderHistoryForm_UpdateTabControlCompare_Added, nbTagGridAdded);
                 tabControlInventory.TabPages[3].Text = string.Format(ResStrings.ReaderHistoryForm_UpdateTabControlCompare_Removed, nbTagGridRemove);

                 DataGridViewColumn dgvc1 = dataGridViewAll.Columns["ImageAll"];
                 if (dgvc1 != null)
                     dgvc1.DisplayIndex = dataGridViewAll.Columns.Count - 1;
                 DataGridViewColumn dvgc2 = dataGridViewPresent.Columns["ImagePresent"];
                 if (dvgc2 != null)
                     dvgc2.DisplayIndex = dataGridViewPresent.Columns.Count - 1;
                 DataGridViewColumn dvgc3 = dataGridViewRemoved.Columns["ImageRemove"];
                 if (dvgc3 != null)
                     dvgc3.DisplayIndex = dataGridViewRemoved.Columns.Count - 1;
                 DataGridViewColumn dvgc4 = dataGridViewAdded.Columns["ImageAdd"];
                 if (dvgc4 != null)
                     dvgc4.DisplayIndex = dataGridViewAdded.Columns.Count - 1;
             }
             catch (Exception)
             {
                 
             }
            
         }

         private void toolStripButtonExit_Click(object sender, EventArgs e)
         {
             BeginInvoke((MethodInvoker)delegate { Close(); });
         }

/*
         private void dataGridViewAll_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
         {
             string tagClicked = (string)dataGridViewAll.Rows[e.RowIndex].Cells[0].Value;
             
             if (!string.IsNullOrEmpty(tagClicked))
             {
                 ItemHistoryForm ihf = new ItemHistoryForm(tagClicked,null);
                 ihf.MdiParent = ParentForm;
                 ihf.WindowState = FormWindowState.Maximized;
                 ihf.Show();
             }
         }
*/

/*
         private void dataGridViewPresent_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
         {
             string tagClicked = (string)dataGridViewPresent.Rows[e.RowIndex].Cells[0].Value;

             if (!string.IsNullOrEmpty(tagClicked))
             {
                 ItemHistoryForm ihf = new ItemHistoryForm(tagClicked,null);
                 ihf.MdiParent = ParentForm;
                 ihf.WindowState = FormWindowState.Maximized;
                 ihf.Show();
             }
         }
*/

/*
         private void dataGridViewAdded_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
         {
             string tagClicked = (string)dataGridViewAdded.Rows[e.RowIndex].Cells[0].Value;

             if (!string.IsNullOrEmpty(tagClicked))
             {
                 ItemHistoryForm ihf = new ItemHistoryForm(tagClicked,null);
                 ihf.MdiParent = ParentForm;
                 ihf.WindowState = FormWindowState.Maximized;
                 ihf.Show();
             }
         }
*/

/*
         private void dataGridViewRemoved_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
         {
             string tagClicked = (string)dataGridViewRemoved.Rows[e.RowIndex].Cells[0].Value;

             if (!string.IsNullOrEmpty(tagClicked))
             {
                 ItemHistoryForm ihf = new ItemHistoryForm(tagClicked,null);
                 ihf.MdiParent = ParentForm;
                 ihf.WindowState = FormWindowState.Maximized;
                 ihf.Show();
             }
         }
*/

         private void toolStripButtonExport_Click(object sender, EventArgs e)
         {
             
             saveExportFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
             DialogResult ret = saveExportFileDialog.ShowDialog();

             if (ret == DialogResult.OK)
             {
                 string fileSaveName = saveExportFileDialog.FileName;

                 if (File.Exists(fileSaveName))
                     File.Delete(fileSaveName);


                 string extension = Path.GetExtension(fileSaveName);
                 if (extension != null && extension.Contains("xlsx")) // 2010
                 {
                     if (dataGridViewScan.SelectedRows.Count > 0)
                     {
                         int selectedData = dataGridViewScan.SelectedRows[0].Index;
                         FileInfo newFile = new FileInfo(fileSaveName);
                         ExcelPackage pck = new ExcelPackage(newFile);
                         {
                             //Create the worksheet
                             ExcelWorksheet ws1 = pck.Workbook.Worksheets.Add(ResStrings.str_All);

                             ws1.Cells.Style.Font.Size = 12;
                             ws1.Cells.Style.Font.Name = "Verdana";

                             //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                             ws1.Cells["A1"].LoadFromDataTable(_inventoryArray[selectedData].dtTagAll, true);

                             ws1.Cells[1, 1, 1, _columnInfo.Count].Style.Font.Bold = true;
                             ws1.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                             ws1.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                             for (int loop = 1; loop <= _columnInfo.Count; loop++)
                                 ws1.Column(loop).AutoFit(25);

                             ExcelWorksheet ws2 = pck.Workbook.Worksheets.Add(ResStrings.str_Previous);

                             ws2.Cells.Style.Font.Size = 12;
                             ws2.Cells.Style.Font.Name = "Verdana";
                             //Load the datatable into the sheet, starting from cell A1. Print the column names on 
                             ws2.Cells["A1"].LoadFromDataTable(_inventoryArray[selectedData].dtTagPresent, true);
                             ws2.Cells[1, 1, 1, _columnInfo.Count].Style.Font.Bold = true;
                             ws2.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                             ws2.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                             for (int loop = 1; loop <= _columnInfo.Count; loop++)
                                 ws2.Column(loop).AutoFit(25);


                             ExcelWorksheet ws3 = pck.Workbook.Worksheets.Add(ResStrings.str_Added);
                             ws3.Cells.Style.Font.Size = 12;
                             ws3.Cells.Style.Font.Name = "Verdana";
                             //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1                        
                             ws3.Cells["A1"].LoadFromDataTable(_inventoryArray[selectedData].dtTagAdded, true);

                             ws3.Cells[1, 1, 1, _columnInfo.Count].Style.Font.Bold = true;
                             ws3.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                             ws3.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                             for (int loop = 1; loop <= _columnInfo.Count; loop++)
                                 ws3.Column(loop).AutoFit(25);

                             ExcelWorksheet ws4 = pck.Workbook.Worksheets.Add(ResStrings.str_Removed);
                             ws4.Cells.Style.Font.Size = 12;
                             ws4.Cells.Style.Font.Name = "Verdana";
                             //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1                        
                             ws4.Cells["A1"].LoadFromDataTable(_inventoryArray[selectedData].dtTagRemove, true);

                             ws4.Cells[1, 1, 1, _columnInfo.Count].Style.Font.Bold = true;
                             ws4.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                             ws4.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                             for (int loop = 1; loop <= _columnInfo.Count; loop++)
                                 ws4.Column(loop).AutoFit(25);

                             pck.Save();
                         }
                     }
                 }
                 else
                 {
                    /* if (dataGridViewScan.SelectedRows.Count > 0)
                     {
                         int selectedData = dataGridViewScan.SelectedRows[0].Index;

                         DBClassSQLite.Export(ConvertForExport(inventoryArray[selectedData].dtTagAll), fileSaveName, "All");
                         DBClassSQLite.Export(ConvertForExport(inventoryArray[selectedData].dtTagPresent), fileSaveName, "Prior");
                         DBClassSQLite.Export(ConvertForExport(inventoryArray[selectedData].dtTagAdded), fileSaveName, "Added");
                         DBClassSQLite.Export(ConvertForExport(inventoryArray[selectedData].dtTagRemove), fileSaveName, "Removed");
                     }*/
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

/*
         private void tabControlInventory_DrawItem(object sender, DrawItemEventArgs e)
         {
             TabPage CurrentTab = tabControlInventory.TabPages[e.Index];
             Rectangle ItemRect = tabControlInventory.GetTabRect(e.Index);
             StringFormat sf = new StringFormat();
             sf.Alignment = StringAlignment.Center;
             sf.LineAlignment = StringAlignment.Center;

             SolidBrush FillBrush;
             SolidBrush TextBrush;

             FillBrush = new SolidBrush(Color.White);
             TextBrush = new SolidBrush(Color.Black);



             switch (e.Index)
             {
                 case 0:
                     FillBrush = new SolidBrush(Color.White);
                     TextBrush = new SolidBrush(Color.Black);

                     //If we are currently painting the Selected TabItem we'll
                     //change the brush colors and inflate the rectangle.
                     if (Convert.ToBoolean(e.State & DrawItemState.Selected))
                     {
                         FillBrush.Color = Color.Black;
                         TextBrush.Color = Color.White;
                         ItemRect.Inflate(2, 2);
                     }
                     break;
                 case 1:
                     FillBrush = new SolidBrush(Color.White);
                     TextBrush = new SolidBrush(Color.Blue);

                     //If we are currently painting the Selected TabItem we'll
                     //change the brush colors and inflate the rectangle.
                     if (Convert.ToBoolean(e.State & DrawItemState.Selected))
                     {
                         FillBrush.Color = Color.Blue;
                         TextBrush.Color = Color.White;
                         ItemRect.Inflate(2, 2);
                     }

                     break;

                 case 2:
                     FillBrush = new SolidBrush(Color.White);
                     TextBrush = new SolidBrush(Color.Green);

                     //If we are currently painting the Selected TabItem we'll
                     //change the brush colors and inflate the rectangle.
                     if (Convert.ToBoolean(e.State & DrawItemState.Selected))
                     {
                         FillBrush.Color = Color.Green;
                         TextBrush.Color = Color.White;
                         ItemRect.Inflate(2, 2);
                     }

                     break;
                 case 3:
                     FillBrush = new SolidBrush(Color.White);
                     TextBrush = new SolidBrush(Color.Red);

                     //If we are currently painting the Selected TabItem we'll
                     //change the brush colors and inflate the rectangle.
                     if (Convert.ToBoolean(e.State & DrawItemState.Selected))
                     {
                         FillBrush.Color = Color.Red;
                         TextBrush.Color = Color.White;
                         ItemRect.Inflate(2, 2);
                     }

                     break;
             }



             //Set up rotation for left and right aligned tabs
             if (tabControlInventory.Alignment == TabAlignment.Left || tabControlInventory.Alignment == TabAlignment.Right)
             {
                 float RotateAngle = 90;
                 if (tabControlInventory.Alignment == TabAlignment.Left)
                     RotateAngle = 270;
                 PointF cp = new PointF(ItemRect.Left + (ItemRect.Width / 2), ItemRect.Top + (ItemRect.Height / 2));
                 e.Graphics.TranslateTransform(cp.X, cp.Y);
                 e.Graphics.RotateTransform(RotateAngle);
                 ItemRect = new Rectangle(-(ItemRect.Height / 2), -(ItemRect.Width / 2), ItemRect.Height, ItemRect.Width);
             }

             //Next we'll paint the TabItem with our Fill Brush          
             e.Graphics.FillRectangle(FillBrush, ItemRect);

             //Now draw the text. 

             Font fo = new Font(e.Font.Name, 12, FontStyle.Bold, e.Font.Unit, e.Font.GdiCharSet, e.Font.GdiVerticalFont);

             e.Graphics.DrawString(CurrentTab.Text, fo, TextBrush, ItemRect, sf);

             //Reset any Graphics rotation
             e.Graphics.ResetTransform();

             //Finally, we should Dispose of our brushes.
             FillBrush.Dispose();
             TextBrush.Dispose();
         }
*/

         private void dataGridViewAll_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
         {
             if (!BShowImage) return;
             if (dataGridViewAll.Columns[e.ColumnIndex].Name == "ImageAll")
             {
              
                 Image img = ImageUtils.ResizeImage(Resources.No_Image_Available, dataGridViewAll.Rows[e.RowIndex].Height);
                 string lotId = dataGridViewAll.Rows[e.RowIndex].Cells[_columnInfo[1].ToString()].Value.ToString();
                 if (_db.IsImageLinkExist(lotId))
                 {
                     string imgName = _db.getImageNameLink(lotId);
                     if (imgName != null)
                     {
                         Image imgFromDb = ImageUtils.byteArrayToImage(_db.getImage(imgName));
                         img = ImageUtils.ResizeImage(imgFromDb, dataGridViewAll.Rows[e.RowIndex].Height);
                     }
                 }

                 e.Value = img;
             }
         }

         private void dataGridViewPresent_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
         {
             if (!BShowImage) return;
             if (dataGridViewPresent.Columns[e.ColumnIndex].Name == "ImagePresent")
             {
                 Image img = ImageUtils.ResizeImage(Resources.No_Image_Available, dataGridViewPresent.Rows[e.RowIndex].Height);
                 string lotId = dataGridViewPresent.Rows[e.RowIndex].Cells[_columnInfo[1].ToString()].Value.ToString();
                 if (_db.IsImageLinkExist(lotId))
                 {
                     string imgName = _db.getImageNameLink(lotId);
                     if (imgName != null)
                     {
                         Image imgFromDb = ImageUtils.byteArrayToImage(_db.getImage(imgName));
                         img = ImageUtils.ResizeImage(imgFromDb, dataGridViewPresent.Rows[e.RowIndex].Height);
                     }
                 }

                 e.Value = img;
             }
         }

         private void dataGridViewAdded_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
         {
             if (!BShowImage) return;
             if (dataGridViewAdded.Columns[e.ColumnIndex].Name == "ImageAdd")
             {
                 Image img = ImageUtils.ResizeImage(Resources.No_Image_Available, dataGridViewAdded.Rows[e.RowIndex].Height);
                 string lotId = dataGridViewAdded.Rows[e.RowIndex].Cells[_columnInfo[1].ToString()].Value.ToString();
                 if (_db.IsImageLinkExist(lotId))
                 {
                     string imgName = _db.getImageNameLink(lotId);
                     if (imgName != null)
                     {
                         Image imgFromDb = ImageUtils.byteArrayToImage(_db.getImage(imgName));
                         img = ImageUtils.ResizeImage(imgFromDb, dataGridViewAdded.Rows[e.RowIndex].Height);
                     }
                 }

                 e.Value = img;
             }
         }

         private void dataGridViewRemoved_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
         {
             if (!BShowImage) return;
             if (dataGridViewRemoved.Columns[e.ColumnIndex].Name == "ImageRemove")
             {
                 Image img = ImageUtils.ResizeImage(Resources.No_Image_Available, dataGridViewRemoved.Rows[e.RowIndex].Height);
                 string lotId = dataGridViewRemoved.Rows[e.RowIndex].Cells[_columnInfo[1].ToString()].Value.ToString();
                 if (_db.IsImageLinkExist(lotId))
                 {
                     string imgName = _db.getImageNameLink(lotId);
                     if (imgName != null)
                     {
                         Image imgFromDb = ImageUtils.byteArrayToImage(_db.getImage(imgName));
                         img = ImageUtils.ResizeImage(imgFromDb, dataGridViewRemoved.Rows[e.RowIndex].Height);
                     }
                 }

                 e.Value = img;
             }
         }

         private void tabControlInventory_DrawItem_1(object sender, DrawItemEventArgs e)
         {
             TabPage currentTab = tabControlInventory.TabPages[e.Index];
             Rectangle itemRect = tabControlInventory.GetTabRect(e.Index);
             StringFormat sf = new StringFormat();
             sf.Alignment = StringAlignment.Center;
             sf.LineAlignment = StringAlignment.Center;

             SolidBrush fillBrush = new SolidBrush(Color.White);
             SolidBrush textBrush = new SolidBrush(Color.Black);

             switch (e.Index)
             {
                 case 0:
                     fillBrush = new SolidBrush(Color.White);
                     textBrush = new SolidBrush(Color.Black);

                     //If we are currently painting the Selected TabItem we'll
                     //change the brush colors and inflate the rectangle.
                     if (Convert.ToBoolean(e.State & DrawItemState.Selected))
                     {
                         fillBrush.Color = Color.Black;
                         textBrush.Color = Color.White;
                         itemRect.Inflate(2, 2);
                     }
                     break;
                 case 1:
                     fillBrush = new SolidBrush(Color.White);
                     textBrush = new SolidBrush(Color.Blue);

                     //If we are currently painting the Selected TabItem we'll
                     //change the brush colors and inflate the rectangle.
                     if (Convert.ToBoolean(e.State & DrawItemState.Selected))
                     {
                         fillBrush.Color = Color.Blue;
                         textBrush.Color = Color.White;
                         itemRect.Inflate(2, 2);
                     }

                     break;

                 case 2:
                     fillBrush = new SolidBrush(Color.White);
                     textBrush = new SolidBrush(Color.Green);

                     //If we are currently painting the Selected TabItem we'll
                     //change the brush colors and inflate the rectangle.
                     if (Convert.ToBoolean(e.State & DrawItemState.Selected))
                     {
                         fillBrush.Color = Color.Green;
                         textBrush.Color = Color.White;
                         itemRect.Inflate(2, 2);
                     }

                     break;
                 case 3:
                     fillBrush = new SolidBrush(Color.White);
                     textBrush = new SolidBrush(Color.Red);

                     //If we are currently painting the Selected TabItem we'll
                     //change the brush colors and inflate the rectangle.
                     if (Convert.ToBoolean(e.State & DrawItemState.Selected))
                     {
                         fillBrush.Color = Color.Red;
                         textBrush.Color = Color.White;
                         itemRect.Inflate(2, 2);
                     }

                     break;
             }



             //Set up rotation for left and right aligned tabs
             if (tabControlInventory.Alignment == TabAlignment.Left || tabControlInventory.Alignment == TabAlignment.Right)
             {
                 float rotateAngle = 90;
                 if (tabControlInventory.Alignment == TabAlignment.Left)
                     rotateAngle = 270;
                 PointF cp = new PointF(itemRect.Left + (itemRect.Width / 2), itemRect.Top + (itemRect.Height / 2));
                 e.Graphics.TranslateTransform(cp.X, cp.Y);
                 e.Graphics.RotateTransform(rotateAngle);
                 itemRect = new Rectangle(-(itemRect.Height / 2), -(itemRect.Width / 2), itemRect.Height, itemRect.Width);
             }

             //Next we'll paint the TabItem with our Fill Brush          
             e.Graphics.FillRectangle(fillBrush, itemRect);

             //Now draw the text. 

             Font fo = new Font(e.Font.Name, 12, FontStyle.Bold, e.Font.Unit, e.Font.GdiCharSet, e.Font.GdiVerticalFont);

             e.Graphics.DrawString(currentTab.Text, fo, textBrush, itemRect, sf);

             //Reset any Graphics rotation
             e.Graphics.ResetTransform();

             //Finally, we should Dispose of our brushes.
             fillBrush.Dispose();
             textBrush.Dispose();
         }

         private void toolStripButtonExportHistory_Click(object sender, EventArgs e)
         {
             saveExportFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
             DialogResult ret = saveExportFileDialog.ShowDialog();

             if (ret == DialogResult.OK)
             {
                 string fileSaveName = saveExportFileDialog.FileName;

                 if (File.Exists(fileSaveName))
                     File.Delete(fileSaveName);


                 string extension = Path.GetExtension(fileSaveName);
                 if (extension != null && extension.Contains("xlsx")) // 2010
                 {                    
                       
                         FileInfo newFile = new FileInfo(fileSaveName);
                         ExcelPackage pck = new ExcelPackage(newFile);
                         {
                             //Create the worksheet
                             ExcelWorksheet ws1 = pck.Workbook.Worksheets.Add("Reader History");

                             ws1.Cells.Style.Font.Size = 12;
                             ws1.Cells.Style.Font.Name = "Verdana";

                             //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                             ws1.Cells["A1"].LoadFromDataTable(_tbReaderScan, true);

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

                   //  DBClassSQLite.Export(tbReaderScan, fileSaveName, "Reader History");                     
                     new ErrorExportDLG().Show();
                 }
             }             
         }

    }
}
