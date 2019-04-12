using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using DataClass;
using smartTracker.Properties;

using TcpIP_class;

namespace smartTracker
{
    public partial class FridgeTempFrm : Form
    {
        public LiveDataForm Lvd;
        public FridgeTempFrm(LiveDataForm lvd)
        {
            Lvd = lvd; 
            InitializeComponent();
            CreateChart();
           
        }

          // Bug VS 2012 chart 3.5 - needto create out designer
        ChartArea chartArea1 = new ChartArea();
        Legend legend1 = new Legend();
        private Chart _chartTemp;
        private void CreateChart()
        {
            _chartTemp = new Chart();
            _chartTemp.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            chartArea1.Name = "ChartArea1";
            _chartTemp.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            _chartTemp.Legends.Add(legend1);
            _chartTemp.Location = new Point(1127,123);
            _chartTemp.Name = "chartTemp";
            _chartTemp.Size = new Size(436,199);
            _chartTemp.TabIndex = 40;
            _chartTemp.Text = "chart1";
            _chartTemp.CustomizeLegend += new EventHandler<CustomizeLegendEventArgs>(chartTemp_CustomizeLegend);
            _chartTemp.BackColor = Color.White;
            _chartTemp.Dock = DockStyle.Fill;
            panelChart.Controls.Add(_chartTemp);
        }
        private void chartTemp_CustomizeLegend(object sender, CustomizeLegendEventArgs e)
        {
            e.LegendItems.Clear();
        }
        public void UpdateChart()
        {
            if (_chartTemp == null) return;
            if ((Lvd.SelectedNetworkDevice != null) && (Lvd.SelectedNetworkIndex >= 0))
            {

                _chartTemp.ChartAreas.Clear();
                _chartTemp.Series.Clear();
                _chartTemp.Titles.Clear();
                Lvd.UpdateTemp(Lvd.SelectedNetworkIndex, Lvd.SelectedNetworkDevice.infoDev.SerialRFID);

                int nbPointToDraw = Lvd.ListTempBottle[Lvd.SelectedNetworkIndex].Count;
                if (nbPointToDraw == 0) return;
                double? currentTemp = (double?)Lvd.ListTempBottle[Lvd.SelectedNetworkIndex][nbPointToDraw - 1];
                if (currentTemp != null)
                    labelCurrentTemp.Text = string.Format(ResStrings.FridgeTempFrm_UpdateChart_currenttemp, DateTime.Now.ToLongTimeString(), Lvd.SelectedNetworkDevice.infoDev.DeviceName, currentTemp.Value.ToString("0.00°"));
                else
                    labelCurrentTemp.Text = DateTime.Now.ToLongTimeString() + " : " + Lvd.SelectedNetworkDevice.infoDev.DeviceName;
                if (nbPointToDraw > 0)
                {
                    ChartArea3DStyle chartArea3DStyle = new ChartArea3DStyle();
                    chartArea3DStyle.Enable3D = true;
                    chartArea3DStyle.LightStyle = LightStyle.Simplistic;
                    chartArea3DStyle.Rotation = 30;
                    chartArea3DStyle.Inclination = 45;
                    chartArea3DStyle.PointDepth = 100;
                    chartArea3DStyle.Perspective = 30;
                    chartArea3DStyle.WallWidth = 10;
                    chartArea3DStyle.IsRightAngleAxes = true;

                    _chartTemp.BackColor = Color.WhiteSmoke;
                    _chartTemp.BackGradientStyle = GradientStyle.None;
                    _chartTemp.BorderlineColor = Color.FromArgb(64, 0, 64);
                    _chartTemp.BorderlineDashStyle = ChartDashStyle.Solid;
                    _chartTemp.BorderlineWidth = 2;
                    _chartTemp.AntiAliasing = AntiAliasingStyles.All;
                    _chartTemp.ChartAreas.Add("Main");
                    // chartTemp.ChartAreas["Main"].Area3DStyle = chartArea3DStyle;

                    int nbInterval = 20;
                    //chartTemp.ChartAreas["Main"].AxisX.Interval = 10;
                    _chartTemp.ChartAreas["Main"].AxisX.Interval = (nbPointToDraw / Lvd.MaxTemp) * nbInterval;
                    _chartTemp.ChartAreas["Main"].AxisX.LabelStyle.Angle = -90;
                    _chartTemp.ChartAreas["Main"].AxisX.LabelStyle.Font = new Font("Verdana", 8.0f, FontStyle.Bold);

                    _chartTemp.ChartAreas["Main"].AxisY.LabelStyle.Font = new Font("Verdana", 10.0f, FontStyle.Bold);

                    _chartTemp.ChartAreas["Main"].BackColor = Color.WhiteSmoke;
                    _chartTemp.ChartAreas["Main"].BackGradientStyle = GradientStyle.VerticalCenter;
                    _chartTemp.ChartAreas["Main"].BackSecondaryColor = Color.WhiteSmoke;
                    _chartTemp.ChartAreas["Main"].BorderColor = Color.FromArgb(64, 0, 64);
                    _chartTemp.ChartAreas["Main"].BorderDashStyle = ChartDashStyle.Solid;
                    _chartTemp.ChartAreas["Main"].BorderWidth = 2;
                    _chartTemp.ChartAreas["Main"].ShadowOffset = 3;
                    _chartTemp.ChartAreas["Main"].ShadowColor = Color.FromArgb(128, 0, 0);

                    _chartTemp.ChartAreas["Main"].AxisX.TitleForeColor = Color.Blue;
                    _chartTemp.ChartAreas["Main"].AxisY.TitleForeColor = Color.Blue;
                    _chartTemp.ChartAreas["Main"].AxisY.Title = "T°";
                    _chartTemp.ChartAreas["Main"].AxisY.TextOrientation = TextOrientation.Rotated270;
                    _chartTemp.ChartAreas["Main"].AxisY.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);

                    _chartTemp.Titles.Add("Temperature");
                    _chartTemp.ChartAreas["Main"].AxisX.Title = "Time (min)";
                    _chartTemp.ChartAreas["Main"].AxisX.TextOrientation = TextOrientation.Horizontal;
                    _chartTemp.ChartAreas["Main"].AxisX.TitleFont = new Font("Verdana", 12.0f, FontStyle.Italic);

                    _chartTemp.Titles[0].Font = new Font("Verdana", 16.0f, FontStyle.Underline);
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

                    _chartTemp.Series.Add("T° Chamber");
                    _chartTemp.Series["T° Chamber"].ChartType = SeriesChartType.Line;

                    _chartTemp.Series["T° Chamber"].YValueType = ChartValueType.Double;
                    _chartTemp.Series["T° Chamber"].IsValueShownAsLabel = false;
                    _chartTemp.Series["T° Chamber"].ToolTip = "#VALX : #VALY";
                    _chartTemp.Series["T° Chamber"].BackGradientStyle = GradientStyle.VerticalCenter;
                    _chartTemp.Series["T° Chamber"].ShadowOffset = 0;
                    _chartTemp.Series["T° Chamber"].BackSecondaryColor = Color.White;
                    _chartTemp.Series["T° Chamber"].ShadowColor = Color.FromArgb(128, 0, 0);
                    _chartTemp.Series["T° Chamber"].Color = Color.Orange;
                    _chartTemp.Series["T° Chamber"].BorderWidth = 3;

                    Font ft = new Font("Verdana", 12.0f, FontStyle.Bold);
                    _chartTemp.Series["T° Bottle"].Font = ft;
                    _chartTemp.Series["T° Chamber"].Font = ft;


                    int rgPt = -nbPointToDraw + 1;
                    for (int loop = 0; loop < nbPointToDraw; loop++)
                    {

                        _chartTemp.Series["T° Alarm High"].Points.AddXY(rgPt, (double?)Lvd.MaxTempFridgeValue);
                        _chartTemp.Series["T° Alarm Low"].Points.AddXY(rgPt, (double?)(Lvd.MaxTempFridgeValue - 5.0));

                        if (loop >= Lvd.ListTempBottle[Lvd.SelectedNetworkIndex].Count) break;
                        if ((double?)Lvd.ListTempBottle[Lvd.SelectedNetworkIndex][loop] == null)
                        {
                            DataPoint point = new DataPoint(rgPt, 0);
                            point.IsEmpty = true;
                            _chartTemp.Series["T° Bottle"].Points.Add(point);
                        }
                        else
                            _chartTemp.Series["T° Bottle"].Points.AddXY(rgPt, (double?)Lvd.ListTempBottle[Lvd.SelectedNetworkIndex][loop]);


                        rgPt++;
                    }
                }
                else
                    labelCurrentTemp.Text = "Info : Temperature unavailable";
            }
        }

        public void UpdateFanem()
        {
            if (Lvd.SelectedNetworkDevice.infoDev.fridgeType != FridgeType.FT_FANEM) return;
            DataFanemInfo dfi = null;
            TcpIpClient tcp = new TcpIpClient();
            tcp.getFridgeFanemInfo(Lvd.SelectedNetworkDevice.infoDev.IP_Server,
                Lvd.SelectedNetworkDevice.infoDev.Port_Server, Lvd.SelectedNetworkDevice.infoDev.SerialRFID, out dfi);
            if (dfi != null)
            {

                labelCurrentTemp.Text = string.Format(ResStrings.FridgeTempFrm_UpdateChart_currenttemp, DateTime.Now.ToLongTimeString(), Lvd.SelectedNetworkDevice.infoDev.DeviceName, dfi.GetT0.ToString("##.0 °C"));
               

                lblInfo.Items.Clear();
                lblInfo.Items.Add("Model : \t" + dfi.GetModel);
                lblInfo.Items.Add("Serial : \t" + dfi.GetSerial);
                lblInfo.Items.Add("Date : \t" + dfi.GetDateTime.ToString("dd/MM/yyyy - hh:mm:ss"));
                lblInfo.Items.Add("T0 : \t" + dfi.GetT0.ToString("##.0"));
                lblInfo.Items.Add("T1 : \t" + dfi.GetT1.ToString("##.0"));
                lblInfo.Items.Add("T2 : \t" + dfi.GetT2.ToString("##.0"));

                lblInfo.Items.Add("Set Temperature  : \t" + dfi.GetTconsigne.ToString("##.0"));
                lblInfo.Items.Add("Alarm High : \t" + dfi.GetAlarmHigh.ToString("##.0"));
                lblInfo.Items.Add("Alarm Low : \t" + dfi.GetAlarmLow.ToString("##.0"));
                lblInfo.Items.Add("Max Temperature  : " + dfi.GetMaxT.ToString("##.0"));
                lblInfo.Items.Add("Min Temperature : \t" + dfi.GetMinT.ToString("##.0"));

                lblInfo.Items.Add("Door : \t\t" + dfi.GetDoor);
                lblInfo.Items.Add("AC power : \t" + dfi.GetAcPower);
                lblInfo.Items.Add("Refrigeration: \t" + dfi.GetRefrigeration);
                lblInfo.Items.Add("Flash drive  : \t" + dfi.GetFlashDrive);
                lblInfo.Items.Add("Defrost  : \t" + dfi.GetDefrost);

                
            }
        }

        private void FridgeTempFrm_Load(object sender, EventArgs e)
        {
            if (Lvd.SelectedNetworkDevice.infoDev.fridgeType != FridgeType.FT_FANEM)
            {
                lblInfo.Visible = false;
                this.panelChart.Width = this.panelChart.Width + 200;
            }
            else
            {
                lblInfo.Visible = true;
            }

            UpdateChart();
            UpdateFanem();
        }
              

        private void timerTemp_Tick_1(object sender, EventArgs e)
        {
            UpdateChart();
            UpdateFanem();

        }

       
    }
}
