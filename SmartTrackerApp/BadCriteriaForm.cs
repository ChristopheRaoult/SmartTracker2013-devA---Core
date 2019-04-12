using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using ErrorMessage;
using OfficeOpenXml;
using BrightIdeasSoftware;
using System.IO;
using DBClass;
using DataClass;
using OfficeOpenXml.Style;
using smartTracker.Properties;
using System.Collections.Generic;

using SDK_SC_RFID_Devices;

namespace smartTracker
{
    public partial class BadCriteriaForm : Form
    {
        DataView dv;
        Hashtable ColumnInfo;
        public DataTable DtProductWithColum;
        private RFID_Device currentDevice;

        public BadCriteriaForm(DataView dv, Hashtable columnInfo, RFID_Device currentDevice)
        {
            InitializeComponent();
            this.dv = dv;
            ColumnInfo = columnInfo;
            dataListView.DataSource = dv;

            this.currentDevice = currentDevice;

            if (currentDevice == null)
                toolStripButtonFindByLed.Enabled = false;

            for (int i = 0; i < dataListView.Columns.Count; i++)
            {
                OLVColumn ol = dataListView.GetColumn(i);
                ol.HeaderFont = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
                ol.HeaderForeColor = Color.AliceBlue;
                ol.IsTileViewColumn = true;
                ol.UseInitialLetterForGroup = false;
                ol.MinimumWidth = 20 + ol.Text.Length * 10;
            }

            if (dataListView.UseTranslucentHotItem)
            {
                dataListView.HotItemStyle.Overlay = new BusinessCardOverlay(columnInfo.Count);
                dataListView.HotItemStyle = dataListView.HotItemStyle;
            }

            dataListView.ItemRenderer = new BusinessCardRenderer();
            dataListView.Invalidate();
        }

        private void BadCriteriaForm_Load(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Enabled = false;
        }

        private void toolStripButtonQuit_Click(object sender, EventArgs e)
        {
            Close();
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

                //dtColumnInfo[] listCol = db.GetdtColumnInfo();

                DataTable dataToexport;

                dataToexport = dv.ToTable();
               

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

                        ws1.Cells[1, 1, 1, ColumnInfo.Count].Style.Font.Bold = true;
                        ws1.Cells[1, 1, 1, ColumnInfo.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws1.Cells[1, 1, 1, ColumnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);


                        for (int loop = 1; loop <= ColumnInfo.Count; loop++)
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

        private void toolStripButtonFindByLed_Click(object sender, EventArgs e)
        {

            if (currentDevice == null) return;
            List<string> selectedTags = new List<string>();

            foreach (ListViewItem currentItem in dataListView.Items)
            {
                OLVColumn olcevent = dataListView.GetColumn(0);
                string eventtype = currentItem.SubItems[olcevent.Index].Text;
                if (eventtype.Equals("Removed"))
                    continue;

                OLVColumn olc = dataListView.GetColumn(1);
                string tagId = currentItem.SubItems[olc.Index].Text;
                selectedTags.Add(tagId);
            }

            int nbTagToLight = selectedTags.Count;
            if (nbTagToLight == 0) return;

                if ((currentDevice.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                    (currentDevice.DeviceStatus == DeviceStatus.DS_Ready))
                {
                    currentDevice.TestLighting(selectedTags);
                    string message = string.Empty;

                    if ((nbTagToLight == 1) && ((nbTagToLight - selectedTags.Count) == 1))
                        message = String.Format(ResStrings.str_LedTagFound, nbTagToLight, nbTagToLight - selectedTags.Count);
                    else if ((nbTagToLight - selectedTags.Count) == 1)
                        message = String.Format(ResStrings.str_LedFound2, nbTagToLight, nbTagToLight - selectedTags.Count);
                    else
                        message = String.Format(ResStrings.str_LedFound3, nbTagToLight, nbTagToLight - selectedTags.Count);

                    if (selectedTags.Count > 0)
                    {
                        message += ResStrings.str_LedMissing;

                        foreach (string missingTag in selectedTags)
                            message = String.Format("{0}\n{1}", message, missingTag);
                    }

                    MessageBox.Show(message, ResStrings.str_LED_Information, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    currentDevice.StopLightingLeds();

                }
        }
    }
}
