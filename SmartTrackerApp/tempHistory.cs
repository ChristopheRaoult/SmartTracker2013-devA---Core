using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using System.Collections;
using DataClass;
using DBClass;
using System.Windows.Forms.DataVisualization.Charting;
using ErrorMessage;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class TempHistory : Form
    {
        MainDBClass _db = null;
        DeviceInfo[] _deviceArray = null;
        DeviceInfo _selectedDevice = null;
        private double _maxTempFridgeValue;
        private readonly List<PtTemp> _tempBottlePoints = new List<PtTemp>();

        private static TextAnnotation _minValueAnnotation, _maxValueAnnotation, _averageValueAnnotation;

        ToolStripControlHost dtMonthComponent;

        public TempHistory()
        {
            InitializeComponent();
            //this.chartTemp.MouseWheel += new MouseEventHandler (MouseWheel);
            dtMonthComponent = new ToolStripControlHost(dateTimePicker);
            toolStripTmpHistory.Items.Insert(4, dtMonthComponent);
        }


        readonly ChartArea _chartArea1 = new ChartArea();
        readonly Legend _legend1 = new Legend();
        private Chart _chartTemp;
        
        private void CreateChart()
        {
            _chartTemp = new Chart();
            _chartTemp.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            _chartArea1.Name = "ChartArea1";
            _chartTemp.ChartAreas.Add(_chartArea1);
            _legend1.Name = "Legend1";
            _chartTemp.Legends.Add(_legend1);
            _chartTemp.Location = new Point(1127, 123);
            _chartTemp.Name = "chartTemp";
            _chartTemp.Size = new Size(436, 199);
            _chartTemp.TabIndex = 40;
            _chartTemp.Text = "chart1";         

            _chartTemp.Dock = DockStyle.Fill;
            panelChart.Controls.Add(_chartTemp);

            _minValueAnnotation = new TextAnnotation();
            _maxValueAnnotation = new TextAnnotation();
            _averageValueAnnotation = new TextAnnotation();

            _chartTemp.Annotations.Add(_minValueAnnotation);
            _chartTemp.Annotations.Add(_maxValueAnnotation);
            _chartTemp.Annotations.Add(_averageValueAnnotation);
        }
      

/*
        private void CreateTempTable()
        {
            _dtTemp = new DataTable();
            _dtTemp.Columns.Add("Date", typeof(string));
            _dtTemp.Columns.Add("T° Bottle", typeof(string));

            try
            {
                foreach (PtTemp point in _tempBottlePoints)
                {
                    DataRow dRow = _dtTemp.NewRow();
                    dRow["Date"] = DateTime.Parse(point.TempAcqDate).ToString("MM/dd/yyyy HH:mm:ss tt");
                    dRow["T° Bottle"] = (point.TempBottle == null) ? "" : point.TempBottle.Value.ToString("0.00");
                    _dtTemp.Rows.Add(dRow);
                }
            }

            catch
            {

            }
        }
*/

        private void PopulateComboReader()
        {
            toolStripComboBoxReader.Items.Clear();
            if (_db.getUsedDBType == MainDBClass.dbUsedType.db_SqlLite)
            {
                DeviceInfo[] tmpDevArray = _db.RecoverAllDevice();
                if (tmpDevArray == null) return;

                _deviceArray = new DeviceInfo[tmpDevArray.Length];
                tmpDevArray.CopyTo(_deviceArray, 0);               

                foreach (DeviceInfo di in _deviceArray)
                {
                    if ((di.deviceType == DeviceType.DT_SFR) || (di.deviceType == DeviceType.DT_SBF))
                    {
                        string str = string.Format("{0}({1})", di.DeviceName, di.SerialRFID);
                        toolStripComboBoxReader.Items.Add(str);
                    }
                }
                if (toolStripComboBoxReader.Items.Count > 0)
                    toolStripComboBoxReader.SelectedIndex = 0;
            }
            else if (_db.getUsedDBType == MainDBClass.dbUsedType.db_SqlServer)
            {
                ArrayList listSerial = _db.RecoverDistinctSerialRFID();
                if ((listSerial != null) && (listSerial.Count > 0))
                {
                    _deviceArray = new DeviceInfo[listSerial.Count];

                    for (int nIndex = 0; nIndex < listSerial.Count; nIndex++)
                    {
                        _deviceArray[nIndex] = _db.RecoverDevice(listSerial[nIndex].ToString());
                    }
                }               

                foreach (DeviceInfo di in _deviceArray)
                {
                    if (di != null)
                    {
                        if ((di.deviceType == DeviceType.DT_SFR) || (di.deviceType == DeviceType.DT_SBF))
                        {
                            string str = string.Format("{0}({1})", di.DeviceName, di.SerialRFID);
                            toolStripComboBoxReader.Items.Add(str);
                        }
                    }
                }
                if (toolStripComboBoxReader.Items.Count > 0)
                    toolStripComboBoxReader.SelectedIndex = 0;
            }
        }

/*
        private DateTime getDatefromCombo()
        {
            switch (toolStripComboBoxTime.SelectedIndex)
            {
                case 0: return DateTime.Now.AddDays(-1);
                case 1: return DateTime.Now.AddDays(-7);
                default: return DateTime.MinValue;
            }
        }
*/

/*
        private int getNbItemfromCombo()
        {
            switch (toolStripComboBoxTime.SelectedIndex)
            {
                case 0: return 24;
                case 1: return 24 * 7;
                default: return 24;
            }
        }
*/
        private void RefreshTemperature()
        {
            _tempBottlePoints.Clear();

            if (_selectedDevice != null)
            {                
                int nbHours = 25;
                if (toolStripComboBoxTime.SelectedIndex == 1) nbHours = 7*24 + 1;
                DateTime selectedDateTime = dateTimePicker.Value.Date.AddHours(DateTime.Now.Hour - 1);
                DateTime selectedDateTimeUtc = DateTime.ParseExact(selectedDateTime.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

                string[] tpInfo = _db.GetFridgeTempAfter(_selectedDevice.SerialRFID, selectedDateTimeUtc.ToString("yyyy-MM-dd HH:mm:ssZ"),nbHours);
               if (tpInfo != null)
               {                 
                   DateTime previousTempInfoDate = selectedDateTime;

                   foreach (string strTemp in tpInfo)
                   {
                       if (strTemp != null)
                       {                          
                           BinaryFormatter bf = new BinaryFormatter();
                           MemoryStream mem = new MemoryStream(Convert.FromBase64String(strTemp));
                           tempInfo currentTempInfo = (tempInfo)bf.Deserialize(mem);
                           DateTime lotDate = currentTempInfo.CreationDate.ToLocalTime();

                           // if one (or more) hour is missing from measures list, let's add empty points in the chart.
                           while ((lotDate - previousTempInfoDate).TotalMinutes > 61)
                           {
                               previousTempInfoDate = previousTempInfoDate.AddHours(1);

                               for (int j = 0; j < 60; ++j)
                               {
                                   PtTemp pt = new PtTemp();
                                   pt.TempAcqDate = previousTempInfoDate.AddMinutes(j).ToString(CultureInfo.InvariantCulture);
                                   pt.TempBottle = null;
                                   pt.TempChamber = null;
                                   _tempBottlePoints.Add(pt);
                               }
                           }

                           // first temperature minute time (e.g "29" if it was measured at 5:29pm)
                           int minuteStart = lotDate.Minute;
                           // for all points missing, from hh:00 to hh:minuteStart, add empty points in the chart.
                           for (int j = 0; j < minuteStart; ++j)
                           {
                               PtTemp pt = new PtTemp();
                               pt.TempAcqDate = lotDate.AddMinutes(-minuteStart + j).ToString(CultureInfo.InvariantCulture);
                               pt.TempBottle = null;
                               pt.TempChamber = null;
                               _tempBottlePoints.Add(pt);
                           }
                           // for all points from hh:minuteStart to the end of the hour, add the point value (if any) or an empty point.
                           for (int j = minuteStart; j < 60; ++j)
                           {
                               PtTemp pt = new PtTemp();
                               pt.TempAcqDate = lotDate.AddMinutes(j - minuteStart).ToString(CultureInfo.InvariantCulture);
                               pt.TempBottle = currentTempInfo.tempBottle.ContainsKey(j) ? currentTempInfo.tempBottle[j] : (double?)null;
                               pt.TempChamber = currentTempInfo.tempBottle.ContainsKey(j) ? currentTempInfo.tempBottle[j] : (double?)null;
                               _tempBottlePoints.Add(pt);
                           }
                           // update the last processed datetime.
                           previousTempInfoDate = lotDate;

                       }
                   }
               }
               else           
                {
                    MessageBox.Show(ResStrings.str_No_data_Temperature_found_in_Database_);
                }
            }
        }

        private void UpdateChart()
        {          
            _chartTemp.ChartAreas.Clear();
            _chartTemp.Series.Clear();
            _chartTemp.Titles.Clear();
            
            if(_tempBottlePoints.Count == 0) return;

            _chartTemp.BackColor = Color.WhiteSmoke;
            _chartTemp.BackGradientStyle = GradientStyle.None;
            _chartTemp.BorderlineColor = Color.FromArgb(64, 0, 64);
            _chartTemp.BorderlineDashStyle = ChartDashStyle.Solid;
            _chartTemp.BorderlineWidth = 2;
            _chartTemp.AntiAliasing = AntiAliasingStyles.All;
                
            _chartTemp.ChartAreas.Add("Main");
            _chartTemp.ChartAreas["Main"].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            _chartTemp.ChartAreas["Main"].AxisX.LabelStyle.Angle = -90;
            _chartTemp.ChartAreas["Main"].AxisX.LabelStyle.Font = new Font("Verdana", 8.0f, FontStyle.Bold);
            _chartTemp.ChartAreas["Main"].AxisX.Title = "Time";
            _chartTemp.ChartAreas["Main"].AxisX.TextOrientation = TextOrientation.Horizontal;
            _chartTemp.ChartAreas["Main"].AxisX.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);
            _chartTemp.ChartAreas["Main"].AxisX.ScaleView.Zoomable = true;
            _chartTemp.ChartAreas["Main"].AxisX.ScrollBar.IsPositionedInside = false;
            _chartTemp.ChartAreas["Main"].AxisX.TitleForeColor = Color.Blue;
            _chartTemp.ChartAreas["Main"].AxisX.IsMarginVisible = false;

            _chartTemp.ChartAreas["Main"].AxisY.LabelStyle.Font = new Font("Verdana", 10.0f, FontStyle.Bold);

            _chartTemp.ChartAreas["Main"].BackColor = Color.WhiteSmoke;
            _chartTemp.ChartAreas["Main"].BackGradientStyle = GradientStyle.VerticalCenter;
            _chartTemp.ChartAreas["Main"].BackSecondaryColor = Color.WhiteSmoke;
            _chartTemp.ChartAreas["Main"].BorderColor = Color.FromArgb(64, 0, 64);
            _chartTemp.ChartAreas["Main"].BorderDashStyle = ChartDashStyle.Solid;
            _chartTemp.ChartAreas["Main"].BorderWidth = 2;
            _chartTemp.ChartAreas["Main"].ShadowOffset = 3;
            _chartTemp.ChartAreas["Main"].ShadowColor = Color.FromArgb(128, 0, 0);

            _chartTemp.ChartAreas["Main"].AxisY.TitleForeColor = Color.Blue;
            _chartTemp.ChartAreas["Main"].AxisY.Title = "T°";
            _chartTemp.ChartAreas["Main"].AxisY.TextOrientation = TextOrientation.Rotated270;
            _chartTemp.ChartAreas["Main"].AxisY.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);

            _chartTemp.Titles.Add(String.Format(ResStrings.strTemperature_Report, dateTimePicker.Value.ToString("d"), _selectedDevice.DeviceName));

            _chartTemp.ChartAreas["Main"].CursorX.IsUserEnabled = true;
            _chartTemp.ChartAreas["Main"].CursorX.IsUserSelectionEnabled = true;
            _chartTemp.ChartAreas["Main"].CursorX.IntervalType = DateTimeIntervalType.Auto;
            _chartTemp.ChartAreas["Main"].CursorX.Interval = 1;
            _chartTemp.ChartAreas["Main"].CursorX.AutoScroll = true;


            _chartTemp.Titles[0].Font = new Font("Verdana", 16.0f, FontStyle.Bold);
            _chartTemp.Titles[0].ForeColor = Color.Blue;

            _chartTemp.Series.Add("T° Alarm High");
            _chartTemp.Series["T° Alarm High"].ChartType = SeriesChartType.Line;

            _chartTemp.Series["T° Alarm High"].YValueType = ChartValueType.Double;
            _chartTemp.Series["T° Alarm High"].IsValueShownAsLabel = false;
            _chartTemp.Series["T° Alarm High"].ToolTip = "#VALX : #VALY";
            _chartTemp.Series["T° Alarm High"].BackGradientStyle = GradientStyle.VerticalCenter;
            _chartTemp.Series["T° Alarm High"].ShadowOffset = 0;
            _chartTemp.Series["T° Alarm High"].BackSecondaryColor = Color.White;
            _chartTemp.Series["T° Alarm High"].ShadowColor = Color.FromArgb(128, 0, 0);
            _chartTemp.Series["T° Alarm High"].Color = Color.Red;
            _chartTemp.Series["T° Alarm High"].BorderWidth = 3;

            _chartTemp.Series.Add("T° Alarm Low");
            _chartTemp.Series["T° Alarm Low"].ChartType = SeriesChartType.Line;
            _chartTemp.Series["T° Alarm Low"].YValueType = ChartValueType.Double;
            _chartTemp.Series["T° Alarm Low"].IsValueShownAsLabel = false;
            _chartTemp.Series["T° Alarm Low"].ToolTip = "#VALX : #VALY";
            _chartTemp.Series["T° Alarm Low"].BackGradientStyle = GradientStyle.VerticalCenter;
            _chartTemp.Series["T° Alarm Low"].ShadowOffset = 0;
            _chartTemp.Series["T° Alarm Low"].BackSecondaryColor = Color.White;
            _chartTemp.Series["T° Alarm Low"].ShadowColor = Color.FromArgb(128, 0, 0);
            _chartTemp.Series["T° Alarm Low"].Color = Color.Red;
            _chartTemp.Series["T° Alarm Low"].BorderWidth = 3;

            _chartTemp.Series.Add("T° Bottle");
            _chartTemp.Series["T° Bottle"].ChartType = SeriesChartType.Line;
            _chartTemp.Series["T° Bottle"].YValueType = ChartValueType.Double;
            _chartTemp.Series["T° Bottle"].IsValueShownAsLabel = false;
            _chartTemp.Series["T° Bottle"].ToolTip = "#VALX : #VALY";
            _chartTemp.Series["T° Bottle"].BackGradientStyle = GradientStyle.VerticalCenter;
            _chartTemp.Series["T° Bottle"].ShadowOffset = 0;
            _chartTemp.Series["T° Bottle"].BackSecondaryColor = Color.White;
            _chartTemp.Series["T° Bottle"].ShadowColor = Color.FromArgb(128, 0, 0);
            _chartTemp.Series["T° Bottle"].Color = Color.DodgerBlue;
            _chartTemp.Series["T° Bottle"].BorderWidth = 3;

            Font ft = new Font("Verdana", 12.0f, FontStyle.Bold);
            _chartTemp.Series["T° Bottle"].Font = ft;

            _chartTemp.Series.Add("Measure presence");
            _chartTemp.Series["Measure presence"].ChartType = SeriesChartType.Area;
            _chartTemp.Series["Measure presence"].YValueType = ChartValueType.Double;
            _chartTemp.Series["Measure presence"].IsValueShownAsLabel = false;
            _chartTemp.Series["Measure presence"].ToolTip = "Measure found at #VALX";
            _chartTemp.Series["Measure presence"].Color = Color.ForestGreen;
            _chartTemp.Series["Measure presence"].BorderWidth = 1;

            List<double> tempBottlePointsValues = new List<double>();

            foreach (PtTemp point in _tempBottlePoints)
            {
                string pointDate = DateTime.Parse(point.TempAcqDate).ToString("dd/MM - HH:mm t");
                _chartTemp.Series["T° Alarm High"].Points.AddXY(pointDate, _maxTempFridgeValue);
                _chartTemp.Series["T° Alarm Low"].Points.AddXY(pointDate, _maxTempFridgeValue - 5);
                _chartTemp.Series["T° Bottle"].Points.AddXY(pointDate, point.TempBottle);

                _chartTemp.Series["Measure presence"].Points.AddXY(pointDate, (point.TempBottle != null) ? 1 : (double?) null);

                if (point.TempBottle == null) continue;
                tempBottlePointsValues.Add(point.TempBottle.Value);
            }

            double coeffHeight = 100 / (double) panelChart.Height;
            double coeffWidth = 100 / (double) panelChart.Width;
            int rightBorderOffset = -50;

            _minValueAnnotation.Text = String.Format(ResStrings.str_minValue, tempBottlePointsValues.Min());
            SizeF annotationSize = TextRenderer.MeasureText(_minValueAnnotation.Text, _minValueAnnotation.Font);
            _minValueAnnotation.X = (panelChart.Width - annotationSize.Width + rightBorderOffset) * coeffWidth;
            _minValueAnnotation.Y = (panelChart.Height / 3.0 - 3 * (annotationSize.Height)+10) * coeffHeight;

            _maxValueAnnotation.Text = String.Format(ResStrings.str_maxvalue, tempBottlePointsValues.Max());
            annotationSize = TextRenderer.MeasureText(_maxValueAnnotation.Text, _minValueAnnotation.Font);
            _maxValueAnnotation.X = (panelChart.Width - annotationSize.Width + rightBorderOffset) * coeffWidth;
            _maxValueAnnotation.Y = (panelChart.Height / 3.0 - 2 * (annotationSize.Height)+10) * coeffHeight;

            _averageValueAnnotation.Text = String.Format(ResStrings.str_Average_value, tempBottlePointsValues.Average());
            annotationSize = TextRenderer.MeasureText(_averageValueAnnotation.Text, _minValueAnnotation.Font);
            _averageValueAnnotation.X = (panelChart.Width - annotationSize.Width + rightBorderOffset) * coeffWidth;
            _averageValueAnnotation.Y = (panelChart.Height / 3.0 - (annotationSize.Height)+10) * coeffHeight;
            
            _chartTemp.SaveImage("graph.bmp", ChartImageFormat.Png); // exporting with .png extension is okay but SmartTracker removes it (on importation)... No problem with ".bmp"           
        }
//        private void MouseWheel(object sender, MouseEventArgs e)
//        {
//            if (e.Delta < 0)
//            {
//                _chartTemp.ChartAreas["Main"].AxisX.ScaleView.ZoomReset();
//            }
//            if (e.Delta > 0)
//            {
//                double xMin =  _chartTemp.ChartAreas["Main"].AxisX.ScaleView.ViewMinimum;
//                double xMax =  _chartTemp.ChartAreas["Main"].AxisX.ScaleView.ViewMaximum;
//              /*  double yMin = chartTemp.ChartAreas["Main"].AxisY.ScaleView.ViewMinimum;
//                double yMax = chartTemp.ChartAreas["Main"].AxisY.ScaleView.ViewMaximum;*/
//
//                double posXStart = _chartTemp.ChartAreas["Main"].AxisX.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
//                double posXFinish = _chartTemp.ChartAreas["Main"].AxisX.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;
//                //double posYStart = chartTemp.ChartAreas["Main"].AxisY.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 4;
//                //double posYFinish = chartTemp.ChartAreas["Main"].AxisY.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 4;
//
//                _chartTemp.ChartAreas["Main"].AxisX.ScaleView.Zoom(posXStart, posXFinish);
//               // chartTemp.ChartAreas["Main"].AxisY.ScaleView.Zoom(posYStart, posYFinish);
//
//                _chartTemp.ChartAreas["Main"].CursorX.IsUserEnabled = true;
//                _chartTemp.ChartAreas["Main"].CursorX.IsUserSelectionEnabled = true;
//                _chartTemp.ChartAreas["Main"].AxisX.ScaleView.Zoomable = true;
//                _chartTemp.ChartAreas["Main"].CursorX.IntervalType = DateTimeIntervalType.Auto;
//                _chartTemp.ChartAreas["Main"].CursorX.Interval = 1;
//                _chartTemp.ChartAreas["Main"].CursorX.AutoScroll = true;
//                _chartTemp.ChartAreas["Main"].AxisX.ScrollBar.IsPositionedInside = false;
//            }
//        }

        private void tempHistory_Load(object sender, EventArgs e)
        {
            _db = new MainDBClass();
            _db.OpenDB();
            _maxTempFridgeValue = _db.GetMaxTempFridgeValue();    
            toolStripComboBoxTime.SelectedIndex = 0;
            PopulateComboReader();            
            CreateChart();
        }

        private void tempHistory_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_db != null)
                    _db.CloseDB();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ExceptionMessageBox.Show(exp);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timerRefresh.Enabled = false;
            RefreshTemperature();
            UpdateChart();
            Cursor = Cursors.Default;
            
        }

        private void toolStripComboBoxReader_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedDevice = null;
            foreach (DeviceInfo di in _deviceArray)
            {
                if(toolStripComboBoxReader.Text.Contains(di.SerialRFID))
                {
                    _selectedDevice = di;
                    break;
                }
            }
        }

        private void toolStripButtonGet_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            timerRefresh.Enabled = true;          
           
        }

        private void toolStripButtonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

/*
        private void ExportToExcel()
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
                        ExcelWorksheet ws1 = pck.Workbook.Worksheets.Add("Fridge Temperature " + _selectedDevice.DeviceName);

                        ws1.Cells.Style.Font.Size = 12;
                        ws1.Cells.Style.Font.Name = "Verdana";

                        //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                        ws1.Cells["A1"].LoadFromDataTable(_dtTemp, true);

                        ws1.Cells[1, 1, 1, _dtTemp.Columns.Count].Style.Font.Bold = true;
                        ws1.Cells[1, 1, 1, _dtTemp.Columns.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws1.Cells[1, 1, 1, _dtTemp.Columns.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                        for (int loop = 1; loop <= _dtTemp.Columns.Count; loop++)
                            ws1.Column(loop).AutoFit(25);                           

                        pck.Save();
                    }
                }                
                else
                {

                    DBClassSQLite.Export(_dtTemp, fileSaveName, "Fridge Temperature " + _selectedDevice.DeviceName);                      
                   
                }
            }             
        }
*/

        private void toolStripButtonExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveGraphAsPictureDialog = new SaveFileDialog();
            saveGraphAsPictureDialog.Filter = "PNG image (*.png)|*.png|Bitmap image (*.bmp)|*.bmp|JPEG image (*.jpg)|*.jpg";
            saveGraphAsPictureDialog.RestoreDirectory = true;

            if (saveGraphAsPictureDialog.ShowDialog() == DialogResult.OK)
            {
                Stream fileStream;

                if ((fileStream = saveGraphAsPictureDialog.OpenFile()) != null)
                {
                    ChartImageFormat format = ChartImageFormat.Png;

                    switch (saveGraphAsPictureDialog.FilterIndex)
                    {
                        case 1:
                            format = ChartImageFormat.Png;
                            break;
                        case 2:
                            format = ChartImageFormat.Bmp;
                            break;
                        case 3:
                            format = ChartImageFormat.Jpeg;
                            break;
                    }

                    _chartTemp.SaveImage(fileStream, format);
                    fileStream.Close();
                }
            }
        }

        private void saveExportFileDialog_FileOk(object sender, CancelEventArgs e)
        {

        }



        static public void CreateGraphFile(string filePath, string graphTitle, List<PtTemp> points)
        {
            MainDBClass db = new MainDBClass();
            db.OpenDB();
            double tempTreshold = db.GetMaxTempFridgeValue();
            db.CloseDB();
 
            _minValueAnnotation = new TextAnnotation();
            _maxValueAnnotation = new TextAnnotation();
            _averageValueAnnotation = new TextAnnotation();

            Chart exportChart = new Chart();
            exportChart.Legends.Add(new Legend()); 
            exportChart.Width = 1300;
            exportChart.Height = 750;

            if (points.Count == 0)
            {
                exportChart.SaveImage(filePath, ChartImageFormat.Png);
                return;
            }

            exportChart.BackColor = Color.WhiteSmoke;
            exportChart.BackGradientStyle = GradientStyle.None;
            exportChart.BorderlineColor = Color.FromArgb(64, 0, 64);
            exportChart.BorderlineDashStyle = ChartDashStyle.Solid;
            exportChart.BorderlineWidth = 2;
            exportChart.AntiAliasing = AntiAliasingStyles.All;

            exportChart.ChartAreas.Add("Main");
            exportChart.ChartAreas["Main"].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            exportChart.ChartAreas["Main"].AxisX.LabelStyle.Angle = -90;
            exportChart.ChartAreas["Main"].AxisX.LabelStyle.Font = new Font("Verdana", 8.0f, FontStyle.Bold);

            exportChart.ChartAreas["Main"].AxisY.LabelStyle.Font = new Font("Verdana", 10.0f, FontStyle.Bold);

            exportChart.ChartAreas["Main"].BackColor = Color.WhiteSmoke;
            exportChart.ChartAreas["Main"].BackGradientStyle = GradientStyle.VerticalCenter;
            exportChart.ChartAreas["Main"].BackSecondaryColor = Color.WhiteSmoke;
            exportChart.ChartAreas["Main"].BorderColor = Color.FromArgb(64, 0, 64);
            exportChart.ChartAreas["Main"].BorderDashStyle = ChartDashStyle.Solid;
            exportChart.ChartAreas["Main"].BorderWidth = 2;
            exportChart.ChartAreas["Main"].ShadowOffset = 3;
            exportChart.ChartAreas["Main"].ShadowColor = Color.FromArgb(128, 0, 0);

            exportChart.ChartAreas["Main"].AxisX.TitleForeColor = Color.Blue;
            exportChart.ChartAreas["Main"].AxisY.TitleForeColor = Color.Blue;
            exportChart.ChartAreas["Main"].AxisY.Title = "T°";
            exportChart.ChartAreas["Main"].AxisY.TextOrientation = TextOrientation.Rotated270;
            exportChart.ChartAreas["Main"].AxisY.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);

            exportChart.Titles.Add(graphTitle);
            exportChart.ChartAreas["Main"].AxisX.Title = "Time";
            exportChart.ChartAreas["Main"].AxisX.TextOrientation = TextOrientation.Horizontal;
            exportChart.ChartAreas["Main"].AxisX.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);

            exportChart.ChartAreas["Main"].CursorX.IsUserEnabled = true;
            exportChart.ChartAreas["Main"].CursorX.IsUserSelectionEnabled = true;
            exportChart.ChartAreas["Main"].AxisX.ScaleView.Zoomable = true;
            exportChart.ChartAreas["Main"].CursorX.IntervalType = DateTimeIntervalType.Auto;
            exportChart.ChartAreas["Main"].CursorX.Interval = 1;
            exportChart.ChartAreas["Main"].CursorX.AutoScroll = true;
            exportChart.ChartAreas["Main"].AxisX.ScrollBar.IsPositionedInside = false;
            exportChart.ChartAreas["Main"].AxisX.IsMarginVisible = false;


            exportChart.Titles[0].Font = new Font("Verdana", 16.0f, FontStyle.Bold);
            exportChart.Titles[0].ForeColor = Color.Blue;

            exportChart.Series.Add("T° Alarm High");
            exportChart.Series["T° Alarm High"].ChartType = SeriesChartType.Line;

            exportChart.Series["T° Alarm High"].YValueType = ChartValueType.Double;
            exportChart.Series["T° Alarm High"].IsValueShownAsLabel = false;
            exportChart.Series["T° Alarm High"].ToolTip = "#VALX : #VALY";
            exportChart.Series["T° Alarm High"].BackGradientStyle = GradientStyle.VerticalCenter;
            exportChart.Series["T° Alarm High"].ShadowOffset = 0;
            exportChart.Series["T° Alarm High"].BackSecondaryColor = Color.White;
            exportChart.Series["T° Alarm High"].ShadowColor = Color.FromArgb(128, 0, 0);
            exportChart.Series["T° Alarm High"].Color = Color.Red;
            exportChart.Series["T° Alarm High"].BorderWidth = 3;

            exportChart.Series.Add("T° Alarm Low");
            exportChart.Series["T° Alarm Low"].ChartType = SeriesChartType.Line;
            exportChart.Series["T° Alarm Low"].YValueType = ChartValueType.Double;
            exportChart.Series["T° Alarm Low"].IsValueShownAsLabel = false;
            exportChart.Series["T° Alarm Low"].ToolTip = "#VALX : #VALY";
            exportChart.Series["T° Alarm Low"].BackGradientStyle = GradientStyle.VerticalCenter;
            exportChart.Series["T° Alarm Low"].ShadowOffset = 0;
            exportChart.Series["T° Alarm Low"].BackSecondaryColor = Color.White;
            exportChart.Series["T° Alarm Low"].ShadowColor = Color.FromArgb(128, 0, 0);
            exportChart.Series["T° Alarm Low"].Color = Color.Red;
            exportChart.Series["T° Alarm Low"].BorderWidth = 3;

            exportChart.Series.Add("T° Bottle");
            exportChart.Series["T° Bottle"].ChartType = SeriesChartType.Line;
            exportChart.Series["T° Bottle"].YValueType = ChartValueType.Double;
            exportChart.Series["T° Bottle"].IsValueShownAsLabel = false;
            exportChart.Series["T° Bottle"].ToolTip = "#VALX : #VALY";
            exportChart.Series["T° Bottle"].BackGradientStyle = GradientStyle.VerticalCenter;
            exportChart.Series["T° Bottle"].ShadowOffset = 0;
            exportChart.Series["T° Bottle"].BackSecondaryColor = Color.White;
            exportChart.Series["T° Bottle"].ShadowColor = Color.FromArgb(128, 0, 0);
            exportChart.Series["T° Bottle"].Color = Color.DodgerBlue;
            exportChart.Series["T° Bottle"].BorderWidth = 3;

            Font ft = new Font("Verdana", 12.0f, FontStyle.Bold);
            exportChart.Series["T° Bottle"].Font = ft;

            exportChart.Series.Add("Measure presence");
            exportChart.Series["Measure presence"].ChartType = SeriesChartType.Area;
            exportChart.Series["Measure presence"].YValueType = ChartValueType.Double;
            exportChart.Series["Measure presence"].IsValueShownAsLabel = false;
            exportChart.Series["Measure presence"].ToolTip = "Measure found at #VALX";
            exportChart.Series["Measure presence"].Color = Color.ForestGreen;
            exportChart.Series["Measure presence"].BorderWidth = 1;

            List<double> tempBottlePointsValues = new List<double>();

            foreach (PtTemp point in points)
            {
                string pointDate = DateTime.Parse(point.TempAcqDate).ToString("dd/MM - HH:mm t");
                exportChart.Series["T° Alarm High"].Points.AddXY(pointDate, tempTreshold);
                exportChart.Series["T° Alarm Low"].Points.AddXY(pointDate, tempTreshold-5);
                exportChart.Series["T° Bottle"].Points.AddXY(pointDate, point.TempBottle);

                exportChart.Series["Measure presence"].Points.AddXY(pointDate, (point.TempBottle != null) ? 1 : (double?)null);

                if (point.TempBottle == null) continue;
                tempBottlePointsValues.Add(point.TempBottle.Value);
            }

            double coeffHeight = 100 / (double)exportChart.Height;
            double coeffWidth = 100 / (double)exportChart.Width;
            int rightBorderOffset = -50;

            _minValueAnnotation.Text = String.Format(ResStrings.str_minValue, tempBottlePointsValues.Min());
            SizeF annotationSize = TextRenderer.MeasureText(_minValueAnnotation.Text, _minValueAnnotation.Font);
            _minValueAnnotation.X = (exportChart.Width - annotationSize.Width + rightBorderOffset) * coeffWidth;
            _minValueAnnotation.Y = (exportChart.Height / 3.0 - 3 * (annotationSize.Height) + 10) * coeffHeight;

            _maxValueAnnotation.Text = String.Format(ResStrings.str_maxvalue, tempBottlePointsValues.Max());
            annotationSize = TextRenderer.MeasureText(_maxValueAnnotation.Text, _minValueAnnotation.Font);
            _maxValueAnnotation.X = (exportChart.Width - annotationSize.Width + rightBorderOffset) * coeffWidth;
            _maxValueAnnotation.Y = (exportChart.Height / 3.0 - 2 * (annotationSize.Height) + 10) * coeffHeight;

            _averageValueAnnotation.Text = String.Format(ResStrings.str_Average_value, tempBottlePointsValues.Average());
            annotationSize = TextRenderer.MeasureText(_averageValueAnnotation.Text, _minValueAnnotation.Font);
            _averageValueAnnotation.X = (exportChart.Width - annotationSize.Width + rightBorderOffset) * coeffWidth;
            _averageValueAnnotation.Y = (exportChart.Height / 3.0 - (annotationSize.Height) + 10) * coeffHeight;

            exportChart.Annotations.Add(_minValueAnnotation);
            exportChart.Annotations.Add(_maxValueAnnotation);
            exportChart.Annotations.Add(_averageValueAnnotation);

            exportChart.SaveImage(filePath, ChartImageFormat.Png);
        }

        Bitmap _myChartPanel;
        public void GetPrintArea(Panel pnl)
        {
            _myChartPanel = new Bitmap(pnl.Width, pnl.Height);
            _myChartPanel.SetResolution(150, 150);
            pnl.DrawToBitmap(_myChartPanel, new Rectangle(0, 0, pnl.Width, pnl.Height));
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_myChartPanel != null)
            {
                e.Graphics.DrawImage(_myChartPanel, 0, 0);
                base.OnPaint(e);
            }
        }
        private void toolStripButtonPrint_Click(object sender, EventArgs e)
        {
            GetPrintArea(panelChart);

            PrintDialog myPrintDialog = new PrintDialog();
            myPrintDialog.UseEXDialog = true;
           
            if (myPrintDialog.ShowDialog() == DialogResult.OK)
            {
                myPrintDocument.DefaultPageSettings.Landscape = true;
                myPrintDialog.Document = myPrintDocument;              
                myPrintDocument.PrintController = new StandardPrintController();
                myPrintDocument.Print();
            }
        }

        private void myPrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            /*Rectangle pagearea = e.PageBounds;
            e.Graphics.DrawImage(MyChartPanel, (pagearea.Width / 2) - (this.panelChart.Width / 2), this.panelChart.Location.Y);*/

            SizeF sz = new SizeF(e.PageBounds.Width - e.MarginBounds.Width, e.PageBounds.Height - e.MarginBounds.Height);
            PointF p = new PointF((sz.Width) / 70, (sz.Height) / 70);
            e.Graphics.DrawImage(_myChartPanel, p);
        }
    }
}
