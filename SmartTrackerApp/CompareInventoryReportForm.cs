using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using DataClass;
using DBClass;
using System.Runtime.Serialization.Formatters.Binary;
using OfficeOpenXml;
using System.Configuration;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class CompareInventoryReportForm : Form
    {

        DeviceInfo[] _deviceArray = null;
        InventoryData _inventoryFrom = null; 
        InventoryData _inventoryTo = null;
        InventoryData _comparedInventory = null;
        Hashtable _columnInfo = null;

        DataTable _dtProductRef;              
        MainDBClass _db;

        public bool BShowImage = true;

        public CompareInventoryReportForm()
        {
            InitializeComponent();

            Font ft = new Font(dataGridViewAll.Font, FontStyle.Bold);
            dataGridViewAll.ColumnHeadersDefaultCellStyle.Font = ft;
            dataGridViewAdded.ColumnHeadersDefaultCellStyle.Font = ft;
            dataGridViewPresent.ColumnHeadersDefaultCellStyle.Font = ft;
            dataGridViewRemoved.ColumnHeadersDefaultCellStyle.Font = ft;

            DateTime now = DateTime.Now;
            DateTime yesterday = now.AddDays(-1);
            dateTo.Value = now;            
            dateFrom.Value = yesterday;

            timeFrom.Value = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, yesterday.Hour, 0, 0);
            timeTo.Value = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
        }

        public void GetProduct()
        {
           
            if (_db != null)
            {
                _dtProductRef = _db.RecoverAllProduct();
            }
        }

        private void ReaderHistoryForm_Load(object sender, EventArgs e)
        {
            bool.TryParse(ConfigurationManager.AppSettings["bShowImage"], out BShowImage);

            if (!BShowImage)
            {

                dataGridViewAll.Columns["ImageAll"].Visible = false;
                dataGridViewPresent.Columns["ImagePresent"].Visible = false;
                dataGridViewRemoved.Columns["ImageRemove"].Visible = false;
                dataGridViewAdded.Columns["Imageadd"].Visible = false;

                dataGridViewAdded.RowTemplate.Height = 25;
                dataGridViewPresent.RowTemplate.Height = 25;
                dataGridViewRemoved.RowTemplate.Height = 25;
                dataGridViewAll.RowTemplate.Height = 25;

            }        

            _db = new MainDBClass();           
            _db.OpenDB();
            _columnInfo = _db.GetColumnInfo();
            GetProduct();
            PopulateComboReader();           
           
        }

        private void PopulateComboReader()
        {
            toolStripComboBoxReader.Items.Clear();
            DeviceInfo[] tmpDevArray = _db.RecoverAllDevice();
            if (tmpDevArray == null) return;

            _deviceArray = new DeviceInfo[tmpDevArray.Length];
            tmpDevArray.CopyTo(_deviceArray, 0);           

            foreach(DeviceInfo di in  _deviceArray)
            {
                string str = di.DeviceName + "(" + di.SerialRFID + ")";
                toolStripComboBoxReader.Items.Add (str);
            } 
            if (_deviceArray.Length > 0)
             toolStripComboBoxReader.SelectedIndex = 0;
        }        

       

         private void ProcessData()
         {

             try
             {
             DeviceInfo di = null;
             _inventoryFrom = null; 
             _inventoryTo = null;

             if (toolStripComboBoxReader.SelectedIndex > -1)
                 di = _deviceArray[toolStripComboBoxReader.SelectedIndex];

             DateTime dFrom = new DateTime(dateFrom.Value.Year, dateFrom.Value.Month, dateFrom.Value.Day, timeFrom.Value.Hour, timeFrom.Value.Minute, 0);
             DateTime dTo = new DateTime(dateTo.Value.Year, dateTo.Value.Month, dateTo.Value.Day, timeTo.Value.Hour, timeTo.Value.Minute, 0);

             if (dTo < dFrom)
             {
                 MessageBox.Show(ResStrings.CompareInventoryReportForm_ProcessData_Error_in_date_selection_, ResStrings.CompareInventoryReportForm_ProcessData_Compare_Inventory_Info, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 return;
             }

             //string[] invFrom = db.GetInventory(di.SerialRFID, dFrom.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ"));
             string[] invFrom = _db.GetInventoryBefore(di.SerialRFID, dFrom.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ"));
             if (invFrom != null)
             {
                 StoredInventoryData sid = new StoredInventoryData();
                 BinaryFormatter bf = new BinaryFormatter();
                 MemoryStream mem = new MemoryStream(Convert.FromBase64String(invFrom[0]));
                 sid = (StoredInventoryData)bf.Deserialize(mem);
                 _inventoryFrom = ConvertInventory.ConvertForUse(sid, _columnInfo);
             }

             //string[] invTo = db.GetInventory(di.SerialRFID, dTo.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ"));
             string[] invTo = _db.GetInventoryAfter(di.SerialRFID, dTo.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ"));
             if (invTo != null)
             {
                 StoredInventoryData sid = new StoredInventoryData();
                 BinaryFormatter bf = new BinaryFormatter();
                 MemoryStream mem = new MemoryStream(Convert.FromBase64String(invTo[0]));
                 sid = (StoredInventoryData)bf.Deserialize(mem);
                 _inventoryTo = ConvertInventory.ConvertForUse(sid, _columnInfo);
             }

             if (_inventoryTo == null) 
             {
                 MessageBox.Show(ResStrings.CompareInventoryReportForm_ProcessData_No_inventory_found_after_Date_to_, ResStrings.CompareInventoryReportForm_ProcessData_Compare_Inventory_Info, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 return;
             }

             if (_inventoryFrom == null)
             {
                 MessageBox.Show(ResStrings.CompareInventoryReportForm_ProcessData_No_inventory_found_after_Date_from_, ResStrings.CompareInventoryReportForm_ProcessData_Compare_Inventory_Info, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 return;
             }

             _comparedInventory = new InventoryData(_columnInfo);

             ArrayList listPrevious = _inventoryFrom.listTagAll;
             ArrayList listCurrent = _inventoryTo.listTagAll;

             foreach (string uid in listCurrent)
             {
                 if (!_comparedInventory.listTagAll.Contains(uid))
                 {
                     _comparedInventory.listTagAll.Add(uid);
                     DtAndTagClass dtAndTagAll = new DtAndTagClass(this, _comparedInventory.dtTagAll, uid, _db);
                     AddTagToDt((object)dtAndTagAll);
                 }

                 if (listPrevious.Contains(uid))
                 {
                     _comparedInventory.listTagPresent.Add(uid);
                     DtAndTagClass dtAndTagPresent = new DtAndTagClass(this, _comparedInventory.dtTagPresent, uid, _db);
                     AddTagToDt((object)dtAndTagPresent);
                 }
                 else
                 {
                     _comparedInventory.listTagAdded.Add(uid);
                     DtAndTagClass dtAndTagAdded = new DtAndTagClass(this, _comparedInventory.dtTagAdded, uid, _db);
                     AddTagToDt((object)dtAndTagAdded);
                 }
             }

             foreach (string uid in listPrevious)
             {
                 if (!listCurrent.Contains(uid))
                 {
                     _comparedInventory.listTagRemoved.Add(uid);
                     DtAndTagClass dtAndTagRemove = new DtAndTagClass(this, _comparedInventory.dtTagRemove, uid, _db);
                     AddTagToDt((object)dtAndTagRemove);
                 }
             }            

            /* foreach (string tagUID in inventoryTo.listTagAll)
             {
                 if (!comparedInventory.listTagAll.Contains(tagUID))
                 {
                     comparedInventory.listTagAll.Add(tagUID);
                     DtAndTagClass DtAndTagAll = new DtAndTagClass(this, comparedInventory.dtTagAll, tagUID, db);
                     AddTagToDt((object)DtAndTagAll);
                 }

                 if (!inventoryFrom.listTagAll.Contains(tagUID))
                 {
                     // Tag Added
                     if (!comparedInventory.listTagAdded.Contains(tagUID))
                     {
                         comparedInventory.listTagAdded.Add(tagUID);
                         DtAndTagClass DtAndTagAdded = new DtAndTagClass(this, comparedInventory.dtTagAdded, tagUID, db);
                         AddTagToDt((object)DtAndTagAdded);
                     }
                 }
                 else
                 {
                     //tag Present
                     if (!comparedInventory.listTagPresent.Contains(tagUID))
                     {
                         comparedInventory.listTagPresent.Add(tagUID);
                         DtAndTagClass DtAndTagPresent = new DtAndTagClass(this, comparedInventory.dtTagPresent, tagUID, db);
                         AddTagToDt((object)DtAndTagPresent);
                     }

                 }
             }

             foreach (string uid in inventoryFrom.listTagAll)
             {
                 if (!comparedInventory.listTagAll.Contains(uid))
                 {
                     if (!comparedInventory.listTagRemoved.Contains(uid))
                     {
                         comparedInventory.listTagRemoved.Add(uid);
                         DtAndTagClass DtAndTagRemove = new DtAndTagClass(this, comparedInventory.dtTagRemove, uid, db);                        
                         AddTagToDt((object)DtAndTagRemove);
                     }
                 }
             }*/

             _comparedInventory.nbTagAll = _comparedInventory.listTagAll.Count;
             _comparedInventory.nbTagPresent = _comparedInventory.listTagPresent.Count;
             _comparedInventory.nbTagAdded = _comparedInventory.listTagAdded.Count;
             _comparedInventory.nbTagRemoved = _comparedInventory.listTagRemoved.Count;

             UpdateTabControl();
             }
             catch (Exception exp)
             {
                 // On affiche l'erreur.
                 ErrorMessage.ExceptionMessageBox.Show(exp);
             }

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
                 ErrorMessage.ExceptionMessageBox.Show(exp);
             }
         }

       

         private void UpdateTabControl()
         {
             dataGridViewAll.DataSource = null;
             dataGridViewAll.DataSource = _comparedInventory.dtTagAll.DefaultView;

             dataGridViewPresent.DataSource = null;
             dataGridViewPresent.DataSource = _comparedInventory.dtTagPresent.DefaultView;

             dataGridViewAdded.DataSource = null;
             dataGridViewAdded.DataSource = _comparedInventory.dtTagAdded.DefaultView;

             dataGridViewRemoved.DataSource = null;
             dataGridViewRemoved.DataSource = _comparedInventory.dtTagRemove.DefaultView;

             int nbTagGridAll = dataGridViewAll.Rows.Count;
             int nbTagGridPresent = dataGridViewPresent.Rows.Count;
             int nbTagGridAdded = dataGridViewAdded.Rows.Count;
             int nbTagGridRemove = dataGridViewRemoved.Rows.Count;

             if (nbTagGridAll < 0) nbTagGridAll = 0;
             if (nbTagGridPresent < 0) nbTagGridPresent = 0;
             if (nbTagGridAdded < 0) nbTagGridAdded = 0;
             if (nbTagGridRemove < 0) nbTagGridRemove = 0;

             tabControlInventory.TabPages[0].Text = ResStrings.CompareInventoryReportForm_UpdateTabControl_All + " (" + nbTagGridAll.ToString() + ")";
             tabControlInventory.TabPages[1].Text = ResStrings.CompareInventoryReportForm_UpdateTabControl_Previous + " (" + nbTagGridPresent.ToString() + ")";
             tabControlInventory.TabPages[2].Text = ResStrings.str_Added + " (" + nbTagGridAdded.ToString() + ")";
             tabControlInventory.TabPages[3].Text = ResStrings.str_Removed + " (" + nbTagGridRemove.ToString() + ")";
           
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
                 ihf.MdiParent = this.ParentForm;
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
                 ihf.MdiParent = this.ParentForm;
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
                 ihf.MdiParent = this.ParentForm;
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
                 ihf.MdiParent = this.ParentForm;
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

                 if (Path.GetExtension(fileSaveName).Contains("xlsx")) // 2010
                 {
                     FileInfo newFile = new FileInfo(fileSaveName);
                     ExcelPackage pck = new ExcelPackage(newFile);
                     {
                         //Create the worksheet
                         ExcelWorksheet ws1 = pck.Workbook.Worksheets.Add(ResStrings.CompareInventoryReportForm_UpdateTabControl_All);

                         ws1.Cells.Style.Font.Size = 12;
                         ws1.Cells.Style.Font.Name = "Verdana";

                         //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                         ws1.Cells["A1"].LoadFromDataTable(_comparedInventory.dtTagAll, true);
                         ws1.Cells[1, 1, 1, _columnInfo.Count].Style.Font.Bold = true;
                         ws1.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                         ws1.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                         for (int loop = 1; loop <= _columnInfo.Count; loop++)
                             ws1.Column(loop).AutoFit(25);

                         ExcelWorksheet ws2 = pck.Workbook.Worksheets.Add(ResStrings.CompareInventoryReportForm_UpdateTabControl_Previous);

                         ws2.Cells.Style.Font.Size = 12;
                         ws2.Cells.Style.Font.Name = "Verdana";
                         //Load the datatable into the sheet, starting from cell A1. Print the column names on 
                         ws2.Cells[1, 1, 1, _columnInfo.Count].Style.Font.Bold = true;
                         ws2.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                         ws2.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                         for (int loop = 1; loop <= _columnInfo.Count; loop++)
                             ws2.Column(loop).AutoFit(25);


                         ExcelWorksheet ws3 = pck.Workbook.Worksheets.Add(ResStrings.str_Added);
                         ws3.Cells.Style.Font.Size = 12;
                         ws3.Cells.Style.Font.Name = "Verdana";
                         //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1                        
                         ws3.Cells["A1"].LoadFromDataTable(_comparedInventory.dtTagAdded, true);

                         ws3.Cells[1, 1, 1, _columnInfo.Count].Style.Font.Bold = true;
                         ws3.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                         ws3.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                         for (int loop = 1; loop <= _columnInfo.Count; loop++)
                             ws3.Column(loop).AutoFit(25);

                         ExcelWorksheet ws4 = pck.Workbook.Worksheets.Add(ResStrings.str_Removed);
                         ws4.Cells.Style.Font.Size = 12;
                         ws4.Cells.Style.Font.Name = "Verdana";
                         //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1                        
                         ws4.Cells["A1"].LoadFromDataTable(_comparedInventory.dtTagRemove, true);

                         ws4.Cells[1, 1, 1, _columnInfo.Count].Style.Font.Bold = true;
                         ws4.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                         ws4.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                         for (int loop = 1; loop <= _columnInfo.Count; loop++)
                             ws4.Column(loop).AutoFit(25);

                         pck.Save();
                     }
                 }
                 else
                 {
                     /*

                     DBClassSQLite.Export(ConvertForExport(comparedInventory.dtTagAll), fileSaveName, "All");
                     DBClassSQLite.Export(ConvertForExport(comparedInventory.dtTagPresent), fileSaveName, "Previous");
                     DBClassSQLite.Export(ConvertForExport(comparedInventory.dtTagAdded), fileSaveName, "Added");
                     DBClassSQLite.Export(ConvertForExport(comparedInventory.dtTagRemove), fileSaveName, "Removed");*/
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
                     if (System.Convert.ToBoolean(e.State & DrawItemState.Selected))
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
                     if (System.Convert.ToBoolean(e.State & DrawItemState.Selected))
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
                     if (System.Convert.ToBoolean(e.State & DrawItemState.Selected))
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
                     if (System.Convert.ToBoolean(e.State & DrawItemState.Selected))
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

             e.Graphics.DrawString(CurrentTab.Text, fo, TextBrush, (RectangleF)ItemRect, sf);

             //Reset any Graphics rotation
             e.Graphics.ResetTransform();

             //Finally, we should Dispose of our brushes.
             FillBrush.Dispose();
             TextBrush.Dispose();
         }
*/

         private void AddTagToDt(object obj)
         {
             DtAndTagClass clToProcess = (DtAndTagClass)obj;
             string selectString = "[" + ResStrings.BoxModeConfig_UpdateGroup_TagUID + "] = '" + clToProcess.TagUid + "'";             
             DataRow[] productInfo = _dtProductRef.Select(selectString);
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
         public class DtAndTagClass
         {
             public Form TheForm;
             public DataTable Dt;
             public string TagUid;

             public MainDBClass Db;

             public DtAndTagClass(Form theForm, DataTable dt, string tagUid, MainDBClass db)
             {
                 TheForm = theForm;
                 Dt = dt;
                 TagUid = tagUid;
                 Db = db;
             }
         }

         private void dataGridViewAll_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
         {
             if (!BShowImage) return;
             if (dataGridViewAll.Columns[e.ColumnIndex].Name == "ImageAll")
             {
                 
                 System.Drawing.Image img = ImageUtils.ResizeImage((System.Drawing.Image)Properties.Resources.No_Image_Available, dataGridViewAll.Rows[e.RowIndex].Height);
                 string LotID = dataGridViewAll.Rows[e.RowIndex].Cells[_columnInfo[1].ToString()].Value.ToString();
                 if (_db.IsImageLinkExist(LotID))
                 {
                     string imgName = _db.getImageNameLink(LotID);
                     if (imgName != null)
                     {
                         System.Drawing.Image imgFromDB = DataClass.ImageUtils.byteArrayToImage(_db.getImage(imgName));
                         img = ImageUtils.ResizeImage(imgFromDB, dataGridViewAll.Rows[e.RowIndex].Height);
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
                 System.Drawing.Image img = ImageUtils.ResizeImage((System.Drawing.Image)Properties.Resources.No_Image_Available, dataGridViewPresent.Rows[e.RowIndex].Height);
                 string LotID = dataGridViewPresent.Rows[e.RowIndex].Cells[_columnInfo[1].ToString()].Value.ToString();
                 if (_db.IsImageLinkExist(LotID))
                 {
                     string imgName = _db.getImageNameLink(LotID);
                     if (imgName != null)
                     {
                         System.Drawing.Image imgFromDB = DataClass.ImageUtils.byteArrayToImage(_db.getImage(imgName));
                         img = ImageUtils.ResizeImage(imgFromDB, dataGridViewPresent.Rows[e.RowIndex].Height);
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
                 System.Drawing.Image img = ImageUtils.ResizeImage((System.Drawing.Image)Properties.Resources.No_Image_Available, dataGridViewAdded.Rows[e.RowIndex].Height);
                 string LotID = dataGridViewAdded.Rows[e.RowIndex].Cells[_columnInfo[1].ToString()].Value.ToString();
                 if (_db.IsImageLinkExist(LotID))
                 {
                     string imgName = _db.getImageNameLink(LotID);
                     if (imgName != null)
                     {
                         System.Drawing.Image imgFromDB = DataClass.ImageUtils.byteArrayToImage(_db.getImage(imgName));
                         img = ImageUtils.ResizeImage(imgFromDB, dataGridViewAdded.Rows[e.RowIndex].Height);
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
                 System.Drawing.Image img = ImageUtils.ResizeImage((System.Drawing.Image)Properties.Resources.No_Image_Available, dataGridViewRemoved.Rows[e.RowIndex].Height);
                 string LotID = dataGridViewRemoved.Rows[e.RowIndex].Cells[_columnInfo[1].ToString()].Value.ToString();
                 if (_db.IsImageLinkExist(LotID))
                 {
                     string imgName = _db.getImageNameLink(LotID);
                     if (imgName != null)
                     {
                         System.Drawing.Image imgFromDB = DataClass.ImageUtils.byteArrayToImage(_db.getImage(imgName));
                         img = ImageUtils.ResizeImage(imgFromDB, dataGridViewRemoved.Rows[e.RowIndex].Height);
                     }
                 }

                 e.Value = img;
             }
         }

         private void tabControlInventory_DrawItem_1(object sender, DrawItemEventArgs e)
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
                     if (System.Convert.ToBoolean(e.State & DrawItemState.Selected))
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
                     if (System.Convert.ToBoolean(e.State & DrawItemState.Selected))
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
                     if (System.Convert.ToBoolean(e.State & DrawItemState.Selected))
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
                     if (System.Convert.ToBoolean(e.State & DrawItemState.Selected))
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

             e.Graphics.DrawString(CurrentTab.Text, fo, TextBrush, (RectangleF)ItemRect, sf);

             //Reset any Graphics rotation
             e.Graphics.ResetTransform();

             //Finally, we should Dispose of our brushes.
             FillBrush.Dispose();
             TextBrush.Dispose();
         }
    }
   
}
