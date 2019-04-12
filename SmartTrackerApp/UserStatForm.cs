using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;
using DataClass;
using DBClass;
using ErrorMessage;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using smartTracker.Properties;

namespace smartTracker
{

    public partial class UserStatForm : Form
    {

        private enum Mode
        {
            ModeUser = 0,
            ModeReader = 1,
        }
        private enum DateMode
        {
            ForDate = 0,
            BetweenTimes = 1,
            Previous = 2,
        }

        private enum TimeMode
        {
              //Minutes = 0,
              //Hours = 1,
              Days = 0,
              Weeks = 1,
              Months = 2,
              Years = 3,
        }

        private enum IntervalMode
        {            
            Hours = 0,
            Days = 1,
            Weeks = 2,
            Months = 3,
            Years = 4,
        }

        MainDBClass _db;
        private Mode _mode;
        private DateMode _dateMode;
        private TimeMode _timeMode;
        private IntervalMode _intervalMode;
        private bool _bVersusTime = false;

        DeviceInfo[] _deviceArray;
        UserClassTemplate[] _userArray;

        // Bug VS 2012 chart 3.5 - needto create out designer
        readonly ChartArea _chartArea1 = new ChartArea();
        readonly Legend _legend1 = new Legend();
        private Chart _chart;
      
     

        public UserStatForm()
        {
            InitializeComponent();
            CreateChart();
            _mode = Mode.ModeUser;
            _dateMode = DateMode.Previous;
            _timeMode = TimeMode.Days;
            _intervalMode = IntervalMode.Hours;
            dateTimePickerTo.Value = DateTime.Now;
            dateTimePickerFrom.Value = DateTime.Now.AddDays(-1);
        }

        private void CreateChart()
        {
            _chart = new Chart();
            _chart.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            _chartArea1.Name = "ChartArea1";
            _chart.ChartAreas.Add(_chartArea1);
            _legend1.Name = "Legend1";
            _chart.Legends.Add(_legend1);
            _chart.Location = new Point(1109, 114);
            _chart.Name = "chartTemp";
            _chart.Size = new Size(454, 209);
            _chart.TabIndex = 40;
            _chart.Text = "chart1";
            _chart.CustomizeLegend += new EventHandler<CustomizeLegendEventArgs>(chartTemp_CustomizeLegend);

            _chart.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(_chart);
        }
        private void chartTemp_CustomizeLegend(object sender, CustomizeLegendEventArgs e)
        {
            e.LegendItems.Clear();
        }

        private void UserStatForm_Load(object sender, EventArgs e)
        {            

            _db = new MainDBClass();
            _db.OpenDB();
            GetDevice();
            GetUser();
            InitForm(_mode);            
        }

        private void InitForm(Mode mode)
        {
            if (mode == Mode.ModeUser)
            {
                labelType.Text = ResStrings.UserStatForm_InitForm_User;
                comboBoxIntervalType.SelectedIndex = (int)_intervalMode;
                comboBoxDateChoice.SelectedIndex = (int)_dateMode;
                comboBoxDatePrevious.SelectedIndex = (int)_timeMode;
                groupBoxFromDate.Enabled = false;
                groupBoxToDate.Enabled = false;
                LoadcomboBoxType(mode);
            }
            else
            {
                labelType.Text = ResStrings.UserStatForm_InitForm_Reader;
                comboBoxIntervalType.SelectedIndex = (int)_intervalMode;
                comboBoxDateChoice.SelectedIndex = (int)_dateMode;
                comboBoxDatePrevious.SelectedIndex = (int)_timeMode;
                groupBoxFromDate.Enabled = false;
                groupBoxToDate.Enabled = false;
                LoadcomboBoxType(mode);
            }
        }

        private void GetDevice()
        {
            UserClassTemplate[] tmpUserArray = _db.RecoverUser();
            if (tmpUserArray != null)
            {
                _userArray = new UserClassTemplate[tmpUserArray.Length];
                tmpUserArray.CopyTo(_userArray, 0);
            }
        }
        private void GetUser()
        {
            DeviceInfo[] tmpDevArray = _db.RecoverAllDevice();
            if (tmpDevArray != null)
            {
                _deviceArray = new DeviceInfo[tmpDevArray.Length];
                tmpDevArray.CopyTo(_deviceArray, 0);
            }
        }

        private void LoadcomboBoxType(Mode mode)
        {
            if (mode == Mode.ModeUser)
            {
                comboBoxType.Items.Clear();
                comboBoxType.Items.Add(ResStrings.str_All);
              
                if (_userArray != null)
                {                  
                    foreach (UserClassTemplate uct in _userArray)
                    {
                        string str = string.Format("{0} {1}", uct.firstName, uct.lastName);
                        comboBoxType.Items.Add(str);
                    }
                }
                comboBoxType.SelectedIndex = 0;
            }
            else
            {
                comboBoxType.Items.Clear();
                comboBoxType.Items.Add(ResStrings.str_All);

                if (_deviceArray != null)
                {
                    foreach (DeviceInfo di in _deviceArray)
                    {
                        string str = string.Format("{0}({1})", di.DeviceName, di.SerialRFID);
                        comboBoxType.Items.Add(str);
                    }
                }
                comboBoxType.SelectedIndex = 0;
            }
        }

        private void readerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _mode = Mode.ModeReader;
            _chart.ChartAreas.Clear();
            _chart.Series.Clear();
            _chart.Titles.Clear();
            InitForm(_mode);   
        }

        private void userToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _mode = Mode.ModeUser;
            _chart.ChartAreas.Clear();
            _chart.Series.Clear();
            _chart.Titles.Clear();
            InitForm(_mode);   
        }

        private void comboBoxDateChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxDateChoice.SelectedIndex)
            {
                case 0:
                        groupBoxPrevious.Enabled = false;
                        groupBoxFromDate.Enabled = true;
                        groupBoxToDate.Enabled = false;
                        _dateMode = DateMode.ForDate;

                    break;
                case 1:
                        groupBoxPrevious.Enabled = false;
                        groupBoxFromDate.Enabled = true;
                        groupBoxToDate.Enabled = true;
                        _dateMode = DateMode.BetweenTimes;
                    break;
                case 2:
                        groupBoxPrevious.Enabled = true;
                        groupBoxFromDate.Enabled = false;
                        groupBoxToDate.Enabled = false;
                        _dateMode = DateMode.Previous;
                    break;
            }
        }

        private void UserStatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            if (_chart != null) _chart.Dispose();
            _db.CloseDB();

            

        }

        private void toolStripButtonQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBoxDatePrevious_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDatePrevious.SelectedIndex >= 0)
                _timeMode = (TimeMode)comboBoxDatePrevious.SelectedIndex;
        }

       


        private void buttonGet_Click(object sender, EventArgs e)
        {
           
            _chart.ChartAreas.Clear();
            _chart.Series.Clear();
            _chart.Titles.Clear();

            ChartArea3DStyle chartArea3DStyle = new ChartArea3DStyle(); 
	        chartArea3DStyle.Enable3D = true;	 
	        chartArea3DStyle.LightStyle = LightStyle.Simplistic;
	        chartArea3DStyle.Rotation = 30;	 
	        chartArea3DStyle.Inclination = 45;	 
	        chartArea3DStyle.PointDepth = 100;
            chartArea3DStyle.Perspective = 30;
            chartArea3DStyle.WallWidth = 10;
            chartArea3DStyle.IsRightAngleAxes = true;           
            
            string dateFrom = null;
            string dateTo = null;

            DateTime dateTimeFrom = DateTime.Now;
            DateTime dateTimeTo = DateTime.Now;

            int colorIndex = 0;

            switch (_dateMode)
            {
                case DateMode.Previous:

                    double  value = (double )numericUpDown.Value;
                    switch (_timeMode)
                    {
                        /*case TimeMode.Hours:
                            dateTimeFrom = DateTime.Now.AddHours(-value);
                            dateTimeFrom = dateTimeFrom.Date.AddMinutes(-dateTimeFrom.Minute).AddSeconds(-dateTimeFrom.Second);
                            dateFrom = dateTimeFrom.ToString("yyyy-MM-dd HH:mm:ssZ"); 
                                break;
                        case TimeMode.Minutes:
                            dateTimeFrom = DateTime.Now.AddMinutes(-value);
                            dateFrom = dateTimeFrom.ToString("yyyy-MM-dd HH:mm:ssZ"); 
                            break;*/
                        case TimeMode.Days:
                            //dateTimeFrom = DateTime.Now.AddDays(-value);
                            dateTimeFrom = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day, 0 , 0 , 0);
                            dateTimeFrom = dateTimeFrom.AddDays(-value);
                            dateTimeTo = dateTimeFrom.AddDays((int)value);                           
                            dateFrom = dateTimeFrom.ToString("yyyy-MM-dd HH:mm:ssZ");
                            dateTo = dateTimeTo.ToString("yyyy-MM-dd HH:mm:ssZ"); 
                            break;
                        case TimeMode.Weeks:
                            dateTimeFrom = DateTimeExtensions.getstartweek(DateTime.Now);                            
                            dateTimeFrom = dateTimeFrom.Date.AddMinutes(-dateTimeFrom.Minute).AddSeconds(-dateTimeFrom.Second);
                            dateTimeFrom=  dateTimeFrom.AddDays(-(7 * value));
                            dateFrom = dateTimeFrom.ToString("yyyy-MM-dd HH:mm:ssZ");

                            dateTimeTo = dateTimeFrom.AddDays((7 * value));
                            dateTo = dateTimeTo.ToString("yyyy-MM-dd HH:mm:ssZ"); 

                            break;
                        case TimeMode.Months:
                            DateTime now = DateTime.Now;
                            dateTimeFrom = new DateTime(now.Year, now.Month, 1, 0, 0, 0);
                            dateTimeFrom = DateTime.Now.AddMonths(-(int)value);
                            dateFrom = dateTimeFrom.ToString("yyyy-MM-dd HH:mm:ssZ");

                            dateTimeTo = dateTimeFrom.AddMonths((int)value);
                            dateTo = dateTimeTo.ToString("yyyy-MM-dd HH:mm:ssZ"); 

                             break;
                        case TimeMode.Years:
                            DateTime now2 = DateTime.Now;
                            dateTimeFrom = new DateTime(now2.Year,1, 1, 0, 0, 0);
                            dateTimeFrom = dateTimeFrom.AddYears(-(int)value);
                            dateFrom = dateTimeFrom.ToString("yyyy-MM-dd HH:mm:ssZ"); 

                            dateTimeTo = dateTimeFrom.AddYears((int)value);
                            dateTo = dateTimeTo.ToString("yyyy-MM-dd HH:mm:ssZ"); 
                            break;
                    }                

                    break;

                case DateMode.BetweenTimes:
                   
                    dateTimeFrom = new DateTime(dateTimePickerFrom.Value.Year, dateTimePickerFrom.Value.Month, dateTimePickerFrom.Value.Day, 0, 0, 0);
                    dateTimeTo = new DateTime(dateTimePickerTo.Value.Year, dateTimePickerTo.Value.Month, dateTimePickerTo.Value.Day, 0, 0, 0);
                    dateTimeTo = dateTimeTo.AddDays(1);
                    dateFrom = dateTimeFrom.ToString("yyyy-MM-dd HH:mm:ssZ");
                    dateTo = dateTimeTo.ToString("yyyy-MM-dd HH:mm:ssZ"); 
                    break;
                case DateMode.ForDate:

                    dateTimeFrom = new DateTime(dateTimePickerFrom.Value.Year, dateTimePickerFrom.Value.Month, dateTimePickerFrom.Value.Day, 0, 0, 0);
                    dateFrom = dateTimeFrom.ToString("yyyy-MM-dd HH:mm:ssZ");
                 
                    break;
            }
            _chart.BackColor = Color.WhiteSmoke;
            _chart.BackGradientStyle = GradientStyle.None;
            _chart.BorderlineColor = Color.FromArgb(64, 0, 64);
            _chart.BorderlineDashStyle = ChartDashStyle.Solid;
            _chart.BorderlineWidth = 2;
            _chart.AntiAliasing = AntiAliasingStyles.All;
            _chart.ChartAreas.Add("Main");
            _chart.ChartAreas["Main"].Area3DStyle = chartArea3DStyle;
            _chart.ChartAreas["Main"].AxisX.Interval = 1;
            _chart.ChartAreas["Main"].AxisX.LabelStyle.Angle = - 45;
            _chart.ChartAreas["Main"].AxisX.LabelStyle.Font = new Font("Verdana", 10.0f, FontStyle.Bold);

            _chart.ChartAreas["Main"].AxisY.LabelStyle.Font = new Font("Verdana", 10.0f, FontStyle.Bold);

            _chart.ChartAreas["Main"].BackColor = Color.WhiteSmoke;
            _chart.ChartAreas["Main"].BackGradientStyle = GradientStyle.VerticalCenter;
            _chart.ChartAreas["Main"].BackSecondaryColor = Color.Purple;
            _chart.ChartAreas["Main"].BorderColor = Color.FromArgb(64, 0, 64);
            _chart.ChartAreas["Main"].BorderDashStyle = ChartDashStyle.Solid;
            _chart.ChartAreas["Main"].BorderWidth = 2;
            _chart.ChartAreas["Main"].ShadowOffset = 3;
            _chart.ChartAreas["Main"].ShadowColor = Color.FromArgb(128, 0, 0);


            //Axis legend and title
            if (_mode == Mode.ModeReader)
            {            
                _chart.ChartAreas["Main"].AxisX.TitleForeColor = Color.Blue;
                _chart.ChartAreas["Main"].AxisY.TitleForeColor = Color.Blue;
                _chart.ChartAreas["Main"].AxisY.Title = ResStrings.UserStatForm_buttonGet_Click_Inventory_Count;
                _chart.ChartAreas["Main"].AxisY.TextOrientation = TextOrientation.Rotated270;
                _chart.ChartAreas["Main"].AxisY.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);
                if (!_bVersusTime)
                {
                    _chart.Titles.Add(ResStrings.UserStatForm_buttonGet_Click_Inventory_count_per_Reader);
                    _chart.ChartAreas["Main"].AxisX.Title = ResStrings.UserStatForm_buttonGet_Click_User_Name;
                    _chart.ChartAreas["Main"].AxisX.TextOrientation = TextOrientation.Horizontal;
                    _chart.ChartAreas["Main"].AxisX.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);                   
                   
                }
                else
                {
                    _chart.Titles.Add(ResStrings.UserStatForm_buttonGet_Click_Inventory_count_versus_Time);
                    _chart.ChartAreas["Main"].AxisX.Title = ResStrings.UserStatForm_buttonGet_Click_Time;
                    _chart.ChartAreas["Main"].AxisX.TextOrientation = TextOrientation.Horizontal;
                    _chart.ChartAreas["Main"].AxisX.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);
                }
                _chart.Titles[0].Font = new Font("Verdana", 16.0f, FontStyle.Underline);
                _chart.Titles[0].ForeColor = Color.Blue;
            }
            else
            {
                _chart.ChartAreas["Main"].AxisX.TitleForeColor = Color.Blue;
                _chart.ChartAreas["Main"].AxisY.TitleForeColor = Color.Blue;
                _chart.ChartAreas["Main"].AxisY.Title = ResStrings.UserStatForm_buttonGet_Click_Inventory_Count;
                _chart.ChartAreas["Main"].AxisY.TextOrientation = TextOrientation.Rotated270;
                _chart.ChartAreas["Main"].AxisY.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);
                if (!_bVersusTime)
                {
                    _chart.Titles.Add(ResStrings.UserStatForm_buttonGet_Click_Inventory_count_per_User);
                    _chart.ChartAreas["Main"].AxisX.Title = ResStrings.str_Reader_Name;
                    _chart.ChartAreas["Main"].AxisX.TextOrientation = TextOrientation.Horizontal;
                    _chart.ChartAreas["Main"].AxisX.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);
                }
                else
                {
                    _chart.Titles.Add(ResStrings.UserStatForm_buttonGet_Click_Inventory_count_versus_Time);
                    _chart.ChartAreas["Main"].AxisX.Title = ResStrings.UserStatForm_buttonGet_Click_Time;
                    _chart.ChartAreas["Main"].AxisX.TextOrientation = TextOrientation.Horizontal;
                    _chart.ChartAreas["Main"].AxisX.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);
                }
                _chart.Titles[0].Font = new Font("Verdana", 16.0f, FontStyle.Underline);
                _chart.Titles[0].ForeColor = Color.Blue;
            }
          

            if (_mode == Mode.ModeUser)
            {      

                if (comboBoxType.SelectedIndex == 0) // all user
                {
                    if (!_bVersusTime)
                    {   
                        foreach (UserClassTemplate uct in _userArray)
                        {
                            string user = string.Format("{0} {1}", uct.firstName, uct.lastName);

                            _chart.Series.Add(string.Format("Serie {0}", user));                            
                            _chart.Series["Serie " + user].ChartType = SeriesChartType.Column;
                            _chart.Series["Serie " + user].Color = GetColorArray(colorIndex++);
                            _chart.Series["Serie " + user].YValueType = ChartValueType.Int32;
                            _chart.Series["Serie " + user].IsValueShownAsLabel = false;
                            _chart.Series["Serie " + user].ToolTip = "#VALX : #VALY";
                            _chart.Series["Serie " + user].BackGradientStyle = GradientStyle.VerticalCenter;
                            _chart.Series["Serie " + user].ShadowOffset = 5;
                            _chart.Series["Serie " + user].BackSecondaryColor = Color.White;
                            _chart.Series["Serie " + user].ShadowColor = Color.FromArgb(128,0,0);
                            _chart.Series["Serie " + user].IsValueShownAsLabel = true;
                            Font ft = new Font("Verdana", 12.0f, FontStyle.Bold);
                            _chart.Series["Serie " + user].Font = ft;


                            foreach (DeviceInfo di in _deviceArray)
                            {
                                int count = _db.GetInventoryCount(di.SerialRFID, uct.firstName, uct.lastName, dateFrom, dateTo);
                                int rg = _chart.Series["Serie " + user].Points.AddXY(di.DeviceName, count);
                                if (count == 0)
                                    _chart.Series["Serie " + user].Points[rg].Label = " ";
                            }                          
                        }
                    }
                    else // versus time
                    {
                        foreach (UserClassTemplate uct in _userArray)
                        {
                            string user = string.Format("{0} {1}", uct.firstName, uct.lastName);

                            _chart.Series.Add("Serie " + user);
                            _chart.Series["Serie " + user].ChartType = SeriesChartType.Column;
                            _chart.Series["Serie " + user].Color = GetColorArray(colorIndex++);
                            _chart.Series["Serie " + user].YValueType = ChartValueType.Int32;
                            _chart.Series["Serie " + user].IsValueShownAsLabel = false;
                            _chart.Series["Serie " + user].ToolTip = "#VALX : #VALY";
                            _chart.Series["Serie " + user].BackGradientStyle = GradientStyle.VerticalCenter;
                            _chart.Series["Serie " + user].ShadowOffset = 5;
                            _chart.Series["Serie " + user].BackSecondaryColor = Color.White;
                            _chart.Series["Serie " + user].ShadowColor = Color.FromArgb(128, 0, 0);

                            _chart.Series["Serie " + user].IsValueShownAsLabel = true;
                            Font ft = new Font("Verdana", 12.0f, FontStyle.Bold);
                            _chart.Series["Serie " + user].Font = ft;

                            DateTime startDate = dateTimeFrom;
                            DateTime endDate = DateTime.Now;
                            if (!string.IsNullOrEmpty(dateTo)) endDate = dateTimeTo;

                            while (endDate > startDate)
                            {
                                DateTime dt1 = startDate;
                                DateTime dt2 = startDate;
                                switch (_intervalMode)
                                {
                                    case IntervalMode.Hours: dt2 = startDate.AddHours(1.0); break;
                                    case IntervalMode.Days: dt2 = startDate.AddDays(1.0); break;
                                    case IntervalMode.Weeks: dt2 = startDate.AddDays(7.0); break;
                                    case IntervalMode.Months: dt2 = startDate.AddMonths(1); break;
                                    case IntervalMode.Years: dt2 = startDate.AddYears(1); break;
                                }
                               
                                int count = _db.GetInventoryCount(null, uct.firstName, uct.lastName, dt1.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ"), dt2.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ"));
                                int rg = 0;
                                if (_intervalMode == IntervalMode.Hours)
                                    rg = _chart.Series["Serie " + user].Points.AddXY(dt1.ToString("HH:mm"), count);
                                else if (_intervalMode == IntervalMode.Days)  
                                    rg = _chart.Series["Serie " + user].Points.AddXY(dt1.ToString("dd/MM/yy"), count);
                                else if (_intervalMode == IntervalMode.Weeks)
                                {
                                    DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                                    if (dfi != null)
                                    {
                                        Calendar cal = dfi.Calendar;
                                        string wk = String.Format("Week {0}", cal.GetWeekOfYear(dt1, dfi.CalendarWeekRule, dfi.FirstDayOfWeek));
                                        rg = _chart.Series["Serie " + user].Points.AddXY(wk, count);
                                    }
                                }
                                else if (_intervalMode == IntervalMode.Months)
                                    rg = _chart.Series["Serie " + user].Points.AddXY(dt1.ToString("MMMM"), count);
                                else if (_intervalMode == IntervalMode.Years)
                                    rg = _chart.Series["Serie " + user].Points.AddXY(dt1.ToString("yyyy"), count);

                                if (count == 0)
                                    _chart.Series["Serie " + user].Points[rg].Label = " ";

                                startDate = dt2;
                            }                           
                        }
                    }

                }//end all user
                else // only one user
                {
                    if (!_bVersusTime)
                    {
                        string user = comboBoxType.Text;
                        int nIndexUser = comboBoxType.SelectedIndex - 1;

                        _chart.Series.Add("Serie " + user);
                        _chart.Series["Serie " + user].ChartType = SeriesChartType.Column;
                        _chart.Series["Serie " + user].Color = GetColorArray(colorIndex++);
                        _chart.Series["Serie " + user].YValueType = ChartValueType.Int32;
                        _chart.Series["Serie " + user].IsValueShownAsLabel = false;
                        _chart.Series["Serie " + user].ToolTip = "#VALX : #VALY";
                        _chart.Series["Serie " + user].BackGradientStyle = GradientStyle.VerticalCenter;
                        _chart.Series["Serie " + user].ShadowOffset = 5;
                        _chart.Series["Serie " + user].BackSecondaryColor = Color.White;
                        _chart.Series["Serie " + user].ShadowColor = Color.FromArgb(128, 0, 0);

                        _chart.Series["Serie " + user].IsValueShownAsLabel = true;
                        Font ft = new Font("Verdana", 12.0f, FontStyle.Bold);
                        _chart.Series["Serie " + user].Font = ft;

                        foreach (DeviceInfo di in _deviceArray)
                        {
                            int count = _db.GetInventoryCount(di.SerialRFID, _userArray[nIndexUser].firstName, _userArray[nIndexUser].lastName, dateFrom, dateTo);
                            int rg = _chart.Series["Serie " + user].Points.AddXY(user, count);
                            if (count == 0)
                                _chart.Series["Serie " + user].Points[rg].Label = " ";
                        }
                    }
                    else //versus time
                    {                       
                        
                        string user = comboBoxType.Text;
                        int nIndexUser = comboBoxType.SelectedIndex - 1;
                        _chart.Series.Add("Serie " + user);
                        _chart.Series["Serie " + user].ChartType = SeriesChartType.Column;
                        _chart.Series["Serie " + user].Color = GetColorArray(colorIndex++);
                        _chart.Series["Serie " + user].YValueType = ChartValueType.Int32;
                        _chart.Series["Serie " + user].IsValueShownAsLabel = false;
                        _chart.Series["Serie " + user].ToolTip = "#VALX : #VALY";
                        _chart.Series["Serie " + user].BackGradientStyle = GradientStyle.VerticalCenter;
                        _chart.Series["Serie " + user].ShadowOffset = 5;
                        _chart.Series["Serie " + user].BackSecondaryColor = Color.White;
                        _chart.Series["Serie " + user].ShadowColor = Color.FromArgb(128, 0, 0);

                        _chart.Series["Serie " + user].IsValueShownAsLabel = true;
                        Font ft = new Font("Verdana", 12.0f, FontStyle.Bold);
                        _chart.Series["Serie " + user].Font = ft;

                        DateTime startDate = dateTimeFrom;
                        DateTime endDate = DateTime.Now;
                        if (!string.IsNullOrEmpty(dateTo)) endDate = dateTimeTo;

                        while (endDate > startDate)
                        {
                            DateTime dt1 = startDate;
                            DateTime dt2 = startDate;
                            switch (_intervalMode)
                            {
                                case IntervalMode.Hours: dt2 = startDate.AddHours(1.0); break;
                                case IntervalMode.Days: dt2 = startDate.AddDays(1.0); break;
                                case IntervalMode.Weeks: dt2 = startDate.AddDays(7.0); break;
                                case IntervalMode.Months: dt2 = startDate.AddMonths(1); break;
                                case IntervalMode.Years: dt2 = startDate.AddYears(1); break;
                            }
                            int count = _db.GetInventoryCount(null, _userArray[nIndexUser].firstName, _userArray[nIndexUser].lastName, dt1.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ"), dt2.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ"));
                            int rg = 0;
                            if (_intervalMode == IntervalMode.Hours)
                                rg = _chart.Series["Serie " + user].Points.AddXY(dt1.ToString("HH:mm"), count);
                            else if (_intervalMode == IntervalMode.Days) 
                                rg = _chart.Series["Serie " + user].Points.AddXY(dt1.ToString("dd/MM/yy"), count);
                            else if (_intervalMode == IntervalMode.Weeks)
                            {
                                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                                if (dfi != null)
                                {
                                    Calendar cal = dfi.Calendar;
                                    string wk = String.Format("Week {0}", cal.GetWeekOfYear(dt1, dfi.CalendarWeekRule, dfi.FirstDayOfWeek));
                                    rg = _chart.Series["Serie " + user].Points.AddXY(wk, count);
                                }
                            }
                            else if (_intervalMode == IntervalMode.Months)
                                rg = _chart.Series["Serie " + user].Points.AddXY(dt1.ToString("MMMM"), count);
                            else if (_intervalMode == IntervalMode.Years)
                                rg = _chart.Series["Serie " + user].Points.AddXY(dt1.ToString("yyyy"), count);
                            if (count == 0)
                                _chart.Series["Serie " + user].Points[rg].Label = " ";
                            startDate = dt2;
                        }
                        
                    }

                }// end onlyone user
            } // end mode user
            else // mode reader
            {               
                
                    if (comboBoxType.SelectedIndex == 0) // all reader
                    {
                        if (!_bVersusTime)
                        {

                            foreach (DeviceInfo di in _deviceArray)
                            {
                                string dev = di.DeviceName + "(" + di.SerialRFID + ")";

                                _chart.Series.Add("Serie " + dev);
                                _chart.Series["Serie " + dev].ChartType = SeriesChartType.Column;
                                _chart.Series["Serie " + dev].Color = GetColorArray(colorIndex++);
                                _chart.Series["Serie " + dev].YValueType = ChartValueType.Int32;
                                _chart.Series["Serie " + dev].IsValueShownAsLabel = false;
                                _chart.Series["Serie " + dev].ToolTip = "#VALX : #VALY";
                                _chart.Series["Serie " + dev].BackGradientStyle = GradientStyle.VerticalCenter;
                                _chart.Series["Serie " + dev].ShadowOffset = 5;
                                _chart.Series["Serie " + dev].BackSecondaryColor = Color.White;

                                _chart.Series["Serie " + dev].IsValueShownAsLabel = true;
                                Font ft = new Font("Verdana", 12.0f, FontStyle.Bold);
                                _chart.Series["Serie " + dev].Font = ft;

                                foreach (UserClassTemplate uct in _userArray)
                                {
                                    int count = _db.GetInventoryCount(di.SerialRFID, uct.firstName, uct.lastName, dateFrom, dateTo);
                                    string user = uct.firstName + " " + uct.lastName;
                                    int rg = _chart.Series["Serie " + dev].Points.AddXY(user, count);
                                    if (count == 0)
                                        _chart.Series["Serie " + dev].Points[rg].Label = " ";
                                   
                                }
                                int count2 = _db.GetInventoryCount(di.SerialRFID, ResStrings.str_Manual, ResStrings.str_Scan, dateFrom, dateTo);
                                string user2 = ResStrings.str_Manual_Scan;
                                int rg2 = _chart.Series["Serie " + dev].Points.AddXY(user2, count2);
                                if (count2 == 0)
                                    _chart.Series["Serie " + dev].Points[rg2].Label = " ";
                            
                            }
                        }
                        else
                        {
                            foreach (DeviceInfo di in _deviceArray)
                            {
                                string dev = di.DeviceName + "(" + di.SerialRFID + ")";

                                _chart.Series.Add("Serie " + dev);
                                _chart.Series["Serie " + dev].ChartType = SeriesChartType.Column;
                                _chart.Series["Serie " + dev].Color = GetColorArray(colorIndex++);
                                _chart.Series["Serie " + dev].YValueType = ChartValueType.Int32;
                                _chart.Series["Serie " + dev].IsValueShownAsLabel = false;
                                _chart.Series["Serie " + dev].ToolTip = "#VALX : #VALY";
                                _chart.Series["Serie " + dev].BackGradientStyle = GradientStyle.VerticalCenter;
                                _chart.Series["Serie " + dev].ShadowOffset = 5;
                                _chart.Series["Serie " + dev].BackSecondaryColor = Color.White;

                                _chart.Series["Serie " + dev].IsValueShownAsLabel = true;
                                Font ft = new Font("Verdana", 12.0f, FontStyle.Bold);
                                _chart.Series["Serie " + dev].Font = ft;

                                DateTime startDate = dateTimeFrom;
                                DateTime endDate = DateTime.Now;
                                if (!string.IsNullOrEmpty(dateTo)) endDate = dateTimeTo;

                                while (endDate > startDate)
                                {
                                    DateTime dt1 = startDate;
                                    DateTime dt2 = startDate;
                                    switch (_intervalMode)
                                    {
                                        case IntervalMode.Hours: dt2 = startDate.AddHours(1.0); break;
                                        case IntervalMode.Days: dt2 = startDate.AddDays(1.0); break;
                                        case IntervalMode.Weeks: dt2 = startDate.AddDays(7.0); break;
                                        case IntervalMode.Months: dt2 = startDate.AddMonths(1); break;
                                        case IntervalMode.Years: dt2 = startDate.AddYears(1); break;
                                    }
                                    int count = _db.GetInventoryCount(di.SerialRFID, null, null, dt1.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ"), dt2.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ"));
                                    int rg = 0;
                                    if (_intervalMode == IntervalMode.Hours)
                                        rg = _chart.Series["Serie " + dev].Points.AddXY(dt1.ToString("HH:mm"), count);
                                    else if (_intervalMode == IntervalMode.Days) 
                                        rg = _chart.Series["Serie " + dev].Points.AddXY(dt1.ToString("dd/MM/yy"), count);
                                    else if (_intervalMode == IntervalMode.Weeks)
                                    {
                                        DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                                        if (dfi != null)
                                        {
                                            Calendar cal = dfi.Calendar;
                                            string wk = String.Format("Week {0}", cal.GetWeekOfYear(dt1, dfi.CalendarWeekRule, dfi.FirstDayOfWeek));
                                            rg = _chart.Series["Serie " + dev].Points.AddXY(wk, count);
                                        }
                                    }
                                    else if (_intervalMode == IntervalMode.Months)
                                        rg = _chart.Series["Serie " + dev].Points.AddXY(dt1.ToString("MMMM"), count);
                                    else if (_intervalMode == IntervalMode.Years)
                                        rg = _chart.Series["Serie " + dev].Points.AddXY(dt1.ToString("yyyy"), count);

                                    if (count == 0)
                                        _chart.Series["Serie " + dev].Points[rg].Label = " ";

                                    startDate = dt2;
                                }                               
                            }   

                        }//end all user
                }

                else // only one reader
                {
                    if (!_bVersusTime)
                    {
                        string dev = comboBoxType.Text;
                        int nIndexDev = comboBoxType.SelectedIndex - 1;

                        _chart.Series.Add("Serie " + dev);
                        _chart.Series["Serie " + dev].ChartType = SeriesChartType.Column;
                        _chart.Series["Serie " + dev].Color = GetColorArray(colorIndex++);
                        _chart.Series["Serie " + dev].YValueType = ChartValueType.Int32;
                        _chart.Series["Serie " + dev].IsValueShownAsLabel = false;
                        _chart.Series["Serie " + dev].ToolTip = "#VALX : #VALY";
                        _chart.Series["Serie " + dev].BackGradientStyle = GradientStyle.VerticalCenter;
                        _chart.Series["Serie " + dev].ShadowOffset = 5;
                        _chart.Series["Serie " + dev].BackSecondaryColor = Color.White;

                        _chart.Series["Serie " + dev].IsValueShownAsLabel = true;
                        Font ft = new Font("Verdana", 12.0f, FontStyle.Bold);
                        _chart.Series["Serie " + dev].Font = ft;

                        foreach (UserClassTemplate uct in _userArray)
                        {
                            int count = _db.GetInventoryCount(_deviceArray[nIndexDev].SerialRFID, uct.firstName, uct.lastName, dateFrom, dateTo);
                            string user = uct.firstName + " " + uct.lastName;
                            int rg = _chart.Series["Serie " + dev].Points.AddXY(user, count);
                            if (count == 0)
                                _chart.Series["Serie " + dev].Points[rg].Label = " ";
                        }
                        int count2 = _db.GetInventoryCount(_deviceArray[nIndexDev].SerialRFID, ResStrings.str_Manual, ResStrings.str_Scan, dateFrom, dateTo);
                        string user2 = ResStrings.str_Manual_Scan;
                        int rg2 =  _chart.Series["Serie " + dev].Points.AddXY(user2, count2);
                        if (count2 == 0)
                            _chart.Series["Serie " + dev].Points[rg2].Label = " ";
                    }
                    else
                    {
                        string dev = comboBoxType.Text;
                        int nIndexDev = comboBoxType.SelectedIndex - 1;

                        _chart.Series.Add("Serie " + dev);
                        _chart.Series["Serie " + dev].ChartType = SeriesChartType.Column;
                        _chart.Series["Serie " + dev].Color = GetColorArray(colorIndex++);
                        _chart.Series["Serie " + dev].YValueType = ChartValueType.Int32;
                        _chart.Series["Serie " + dev].IsValueShownAsLabel = false;
                        _chart.Series["Serie " + dev].ToolTip = "#VALX : #VALY";
                        _chart.Series["Serie " + dev].BackGradientStyle = GradientStyle.VerticalCenter;
                        _chart.Series["Serie " + dev].ShadowOffset = 5;
                        _chart.Series["Serie " + dev].BackSecondaryColor = Color.White;

                        _chart.Series["Serie " + dev].IsValueShownAsLabel = true;
                        Font ft = new Font("Verdana", 12.0f, FontStyle.Bold);
                        _chart.Series["Serie " + dev].Font = ft;

                        DateTime startDate = dateTimeFrom;
                        DateTime endDate = DateTime.Now;
                        if (!string.IsNullOrEmpty(dateTo)) endDate = dateTimeTo;

                        while (endDate > startDate)
                        {
                            DateTime dt1 = startDate;
                            DateTime dt2 = startDate;
                            switch (_intervalMode)
                            {
                                case IntervalMode.Hours: dt2 = startDate.AddHours(1.0); break;
                                case IntervalMode.Days: dt2 = startDate.AddDays(1.0); break;
                                case IntervalMode.Weeks: dt2 = startDate.AddDays(7.0); break;
                                case IntervalMode.Months: dt2 = startDate.AddMonths(1); break;
                                case IntervalMode.Years: dt2 = startDate.AddYears(1); break;
                            }
                            int count = _db.GetInventoryCount(_deviceArray[nIndexDev].SerialRFID, null, null, dt1.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ"), dt2.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ssZ"));
                            int rg = 0;
                            if (_intervalMode == IntervalMode.Hours)
                                rg = _chart.Series["Serie " + dev].Points.AddXY(dt1.ToString("HH:mm"), count);
                            else if (_intervalMode == IntervalMode.Days)
                                rg = _chart.Series["Serie " + dev].Points.AddXY(dt1.ToString("dd/MM/yy"), count);
                            else if (_intervalMode == IntervalMode.Weeks)
                            {
                                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                                if (dfi != null)
                                {
                                    Calendar cal = dfi.Calendar;
                                    string wk = String.Format("Week {0}", cal.GetWeekOfYear(dt1, dfi.CalendarWeekRule, dfi.FirstDayOfWeek));
                                    rg = _chart.Series["Serie " + dev].Points.AddXY(wk, count);
                                }
                            }
                            else if (_intervalMode == IntervalMode.Months)
                                rg = _chart.Series["Serie " + dev].Points.AddXY(dt1.ToString("MMMM"), count);
                            else if (_intervalMode == IntervalMode.Years)
                                rg = _chart.Series["Serie " + dev].Points.AddXY(dt1.ToString("yyyy"), count);

                            if (count == 0)
                                _chart.Series["Serie " + dev].Points[rg].Label = " ";
                            startDate = dt2;
                        }
                    }

                }// end onlyone reader     

            } // end mode reader
        }

        private void toolStripButtonReset_Click(object sender, EventArgs e)
        {
            _chart.ChartAreas.Clear();
            _chart.Series.Clear();
            _chart.Titles.Clear();
        }

        private void ckx_versusTime_CheckedChanged(object sender, EventArgs e)
        {
            _bVersusTime = ckx_versusTime.Checked;
        }

        Color GetColorArray(int nIndex)
        {

            // declare an Array for 20 colors
            Color[] aColors = new Color[20];

            // fill the array of colors for chart items
            // use browser-safe colors (multiples of #33)
            aColors[0] = Color.FromArgb(204, 0, 0);      // red
            aColors[1] = Color.FromArgb(255, 153, 0);    // orange
            aColors[2] = Color.FromArgb(255, 255, 0);    // yellow
            aColors[3] = Color.FromArgb(0, 255, 0);      // green
            aColors[4] = Color.FromArgb(0, 255, 255);    // cyan
            aColors[5] = Color.FromArgb(51, 102, 255);   // blue
            aColors[6] = Color.FromArgb(255, 0, 255);    // magenta
            aColors[7] = Color.FromArgb(102, 0, 102);    // purple
            aColors[8] = Color.FromArgb(153, 0, 0);      // dark red
            aColors[9] = Color.FromArgb(153, 153, 0);    // khaki
            aColors[10] = Color.FromArgb(0, 102, 0);     // dark green
            aColors[11] = Color.FromArgb(51, 51, 102);   // dark blue
            aColors[12] = Color.FromArgb(102, 51, 0);    // brown
            aColors[13] = Color.FromArgb(204, 204, 204); // light gray
            aColors[14] = Color.FromArgb(0, 0, 0);       // black
            aColors[15] = Color.FromArgb(102, 204, 255); // sky
            aColors[16] = Color.FromArgb(255, 204, 255); // pink
            aColors[17] = Color.FromArgb(255, 255, 204); // chiffon
            aColors[18] = Color.FromArgb(255, 204, 204); // flesh
            aColors[19] = Color.FromArgb(153, 255, 204); // pale green

            if (nIndex > 19) nIndex = 0;

            return aColors[nIndex];

        }      

        private void dateTimePickerTo_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePickerTo.Value < dateTimePickerFrom.Value)
                dateTimePickerTo.Value = dateTimePickerFrom.Value.AddDays(1);
        }

        private void dateTimePickerFrom_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePickerTo.Value < dateTimePickerFrom.Value)
                dateTimePickerTo.Value = dateTimePickerFrom.Value.AddDays(1);
        }

        private void comboBoxIntervalType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _intervalMode = (IntervalMode)comboBoxIntervalType.SelectedIndex;
        }

        private string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        private void toolStripButtonExport_Click(object sender, EventArgs e)
        {
           

            try
            {

                string path = Application.StartupPath + Path.DirectorySeparatorChar +  RandomString(5,true) + ".png";
               
                _chart.SaveImage(path, ChartImageFormat.Png);
                
                string xls = Application.StartupPath + "\\exportchart.xlsx";
                if (File.Exists(xls)) File.Delete(xls);
                FileInfo newPng = new FileInfo(path);

                FileInfo newFile = new FileInfo(xls);
                ExcelPackage pck = new ExcelPackage(newFile);
                {
                    //Create the worksheet
                    ExcelWorksheet ws1 = pck.Workbook.Worksheets.Add(ResStrings.UserStatForm_toolStripButtonExport_Click_export);
                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                    ws1.Cells.Style.Font.Size = 10;
                    ws1.Cells.Style.Font.Name = "Verdana";

                    DataSet ds = _chart.DataManipulator.ExportSeriesValues();
                    int nCol = 3;
                    int nRow = 2;
                    if (ds.Tables.Count > 0)
                    {
                       
                        ws1.Cells[1,2].Value = _chart.Series[0].Name;
                        foreach (DataRow oRow in ds.Tables[0].Rows)
                        {
                            ws1.Cells[nRow, 1].Value = oRow[0].ToString();
                            ws1.Cells[nRow++, 2].Value = oRow[1].ToString();
                        }


                        nCol = 3;
                        for (int loop = 1; loop < ds.Tables.Count; loop++)
                        {
                            nRow = 2;
                            ws1.Cells[1, nCol].Value = _chart.Series[loop].Name;
                            foreach (DataRow oRow in ds.Tables[loop].Rows)
                                ws1.Cells[nRow++, nCol].Value = oRow[1].ToString();

                            nCol++;
                        }
                    }

                    ws1.Cells[1, 1, 1, nCol].Style.Font.Bold = true;
                    ws1.Cells[1, 1, 1, nCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws1.Cells[1, 1, 1, nCol].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                    for (int loop = 1; loop <= nCol; loop++)
                        ws1.Column(loop).AutoFit(25);

                    ws1.Drawings.AddPicture("chart", newPng);
                    ws1.Drawings[0].SetPosition(nRow * 20,50);
                    ws1.Drawings[0].SetSize(75);
                    
                    pck.Save();           

                    Process.Start(xls);

                  
                }
            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }
        
        }
    }
}
