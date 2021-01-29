using BrightIdeasSoftware;
using DataClass;
using DB_Class_SQLite;
using DBClass;
using DBClass_SQLServer;
using ErrorMessage;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SDK_SC_AccessControl;
using SDK_SC_Fingerprint;
using SDK_SC_MedicalCabinet;
using SDK_SC_RFID_Devices;
using SDK_SC_RfidReader;
using smartTracker.LIB;
using smartTracker.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using TcpIP_class;
using Cursor = System.Windows.Forms.Cursor;
using Timer = System.Timers.Timer;

namespace smartTracker
{
    public partial class LiveDataForm : Form
    {
        delegate void MethodInvoker();

        private msgFrm msgBox = null;
        private Stopwatch s1; 

        #region Variables
        readonly MainDBClass _db = new MainDBClass();

        Regex _myRegexUid;

        DataTable _dtGroup;
        int _nbTagBox;
        BoxTagInfo _bti;

        CompareInventoryForm _cptInvForm;
        volatile deviceClass[] _localDeviceArray;
        volatile deviceClass[] _networkDeviceArray;
        UserClassTemplate[] _localUserArray;

        public TagSearchLedfrm tslf; 

        ScanInProgress _sip;
        bool _bStop = false;
        private Dictionary<deviceClass, bool> CanHandleLeds = new Dictionary<deviceClass, bool>();

        private Dictionary<string,TcpIpClient> DeviceClientTcp = new Dictionary<string, TcpIpClient>(); 
       
        private string _firstName;
        private string _lastName;
        public  string LastBadge = null;
        private bool _bUserScan;  
        private AccessType _lastAccessType;

        private int _selectedReader = -1;

        private bool _bComeFromFridge;

        private bool _bAccumulateSbr;
        private bool _bWasInAccumulation;
        private bool _bFirstScanAccumulate = true;
        volatile private bool _bClosing;
        volatile private bool _bCancelHost;

        private int _nbLocalDevice;
      
        private bool _checkedAccumulate;
        private bool _checkedWaitMode;
        private bool _checkedInOut;
        private bool _bKeepLastScan;
        private bool _bClockFridgeAlreadyOn;

        private DateTime _currentAlarmEvent = DateTime.Now;
        private DateTime _lastAlarmEvent = DateTime.Now.AddDays(-1.0);
      
        private DateTime _dateFridgeALarmBlocked;
        private int _timeTcpToRefresh = 10000;
      
        public deviceClass SelectedNetworkDevice = null;
        public int SelectedNetworkIndex = -1;
        DoorInfo _lastDoorUser;
       
        readonly ToolStripCheckedBox _toolKeepLastScan = new ToolStripCheckedBox();

        readonly TcpIpServer _tcpIpServer;

        RemoteUser _ru;
        
        private double _timeZoneOffset;
        private int _nbRecordToKeep = 1000;
        private int _selIndex = -1;
        private bool _bStoreTagEvent;
        public DataTable DtProductRef, DtProductToFind;
        public bool BRestart = false;
        private bool _bCompareOnLotId = true;
        private bool _bUseAlarm;
       
        public bool BShowImage = true;
        public bool BShowAlert = true;
        private readonly bool[] _treeviewExpandArray = new bool[100];

        Hashtable _columnInfo;
        dtColumnInfo[] _columnInfoFull;
        readonly Dictionary<DateTime, deviceClass> _removedInventoryAlert = new Dictionary<DateTime, deviceClass>();       
        int _maxTimeBeforeAlertRemoved;
        readonly ArrayList _removedReaderDevice = new ArrayList();
        ImageList _treeImageList;
        readonly Thread _netThread = null;

        public volatile bool InConnection = false;
        public volatile bool InScan = false;
        private bool _bFirstInLoop = true;

        private bool _bShowServer = false;
        public double MaxTempFridgeValue;
        private int _maxTimeOpenDoor;
        private bool _interruptScanWithFp;
        public Timer ClockFridge = new Timer();
        public Timer ClockAlertRemoved = new Timer();
        private readonly object _locker = new object();
        private static readonly Object LogStore = new Object();

        private double TimeZoneOffset = 1.0;

        FormuleData _formule;
        bool _runMacro;
        private int _rowDlcDate = -1;
        readonly string[] _formats = { "dd/MM/yyyy", "dd/MM/yy", "dd-MM-yyyy", "dd-MM-yy", "dd MM yyyy", "dd MM yy" };
        string _columnExpiredDate = string.Empty;
        string _selectedPatient = string.Empty;
        int _previousSelectedreader = -1;

        bool _bUseSqlExport = false;
        bool _bUsePhpExport = false;
        SqlThreadHandle _sqlExportThreadInfo;


        TcpNotificationServer notifyServer;
         private int tcpNotifyPort = 6902;
         bool _bUseTcpNotification;
         TcpThreadHandle _tcpNotificationThreadInfo;
         private readonly EventWaitHandle _eventTestTcp = new AutoResetEvent(false);

        public DateTime LastRenewFp = DateTime.MinValue;

        public int MaxTemp =  4 * 60;
        public ArrayList[] ListTempBottle;
        FridgeTempFrm _ftf = null;

        private bool bUseCvsExchange = false;
        private string pathCsvFolder;
        private string cvsMachineId="0000";
        private FileSystemWatcher csvWatcher = null;
        private bool bScanforCSV = false;
        private bool bFisrtScanDone = false;
        private string pathCsvReport;
        private string pathCsvInventory;
        private string pathCsvLed;
        private string pathCsvError;
        private string pathCsvInScan;
        private string pathCsvLog;

        private SynchronizedDevice syncDevice = null;
        public bool bUseSynchonisation = false;
        public string DeviceIpRight = string.Empty;
        public int DevicePortRight = 6901;
        public string DeviceIpLeft = string.Empty;
        public int DevicePortLeft = 6901;
        public int TimeoutInSec = 120;
        public bool DoDoorScan = true;

        #endregion
        #region group
        private void UpdateGroup()
        {
            if (_bClosing) return;

            _dtGroup = new DataTable();

            _dtGroup = _db.RecoverAllGroup();
            _dtGroup.Columns[0].ColumnName = ResStrings.str_TagUID;
            _dtGroup.Columns[1].ColumnName = ResStrings.str_Group_Reference;
            _dtGroup.Columns[2].ColumnName = ResStrings.str_Group_Description;
            _dtGroup.Columns[3].ColumnName = ResStrings.str_Criteria;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_bti != null)
            {
                _bti.Criteria = txtCriteria.Text;
                if (CheckCriteria()) // display message for good in case of check
                    MessageBox.Show(string.Format(ResStrings.LiveDataForm_button1_Click_All_Tag_in_box_fit_criteria, _bti.BoxRef, _bti.Criteria), ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion 
        #region Formule
        public void LoadFormule()
        {
            _formule = _db.getFormuleInfo();            
        }
        #endregion
        #region LiveDataWindows
        public LiveDataForm(TcpIpServer tcpIpServer)
        {
            InitializeComponent();

            _tcpIpServer = tcpIpServer;
            tcpIpServer.CryptedAuthorization =  ConfigurationManager.AppSettings["TcpUnlockingAuthorization"];


       
            _toolKeepLastScan.Text = ResStrings.LiveDataForm_LiveDataForm_Keep_Last_Scan;
            _toolKeepLastScan.Enabled = false;
            _toolKeepLastScan.BackColor = Color.White;
            _toolKeepLastScan.Click += toolKeepLastScan_Click;
            _toolKeepLastScan.Font = toolStripButtonScan.Font;
            toolStripLiveData.Items.Insert(4, _toolKeepLastScan);
            

            normalToolStripMenuItem.Checked = true;
            accumulateToolStripMenuItem.Checked = false;
            waitModeToolStripMenuItem.Checked = false;
            
        }
        private void toolKeepLastScan_Click(object sender, EventArgs e)
        {
            CheckBox checkBox = _toolKeepLastScan.Control as CheckBox;
            if (checkBox != null)
                _bKeepLastScan = checkBox.Checked;
            CheckBox box = _toolKeepLastScan.Control as CheckBox;
            if (box != null)
                listBoxTag.Visible = box.Checked;
        }

        private void LiveDataForm_Load(object sender, EventArgs e)
        {
            try
            {
                timerObjectList.Stop();    
                _db.OpenDB();
                _columnInfo = _db.GetColumnInfo();
                _columnInfoFull = _db.GetdtColumnInfo();
                LoadFormule();
                UpdateGroup();
                InitAlert();
                _columnExpiredDate = _db.GetColumnNameForAlert(AlertType.AT_DLC_Expired);
                GetExportSqlInfo();
                GetTcpNotificationInfo();
                initTcpServerNotification();
               
                int.TryParse(ConfigurationManager.AppSettings["NbRecordToKeep"], out _nbRecordToKeep);
                int.TryParse(ConfigurationManager.AppSettings["timeTcpToRefresh"], out _timeTcpToRefresh);
                bool.TryParse(ConfigurationManager.AppSettings["bStoreEventTag"], out _bStoreTagEvent);
                bool.TryParse(ConfigurationManager.AppSettings["bCompareOnLotID"], out _bCompareOnLotId);
                bool.TryParse(ConfigurationManager.AppSettings["bShowImage"], out BShowImage);
                bool.TryParse(ConfigurationManager.AppSettings["bShowAlert"], out BShowAlert);
                bool.TryParse(ConfigurationManager.AppSettings["bUseAlarm"], out _bUseAlarm);
                bool.TryParse(ConfigurationManager.AppSettings["bInterruptScanWithFP"], out _interruptScanWithFp);
                bool.TryParse(ConfigurationManager.AppSettings["bShowServer"], out _bShowServer);

                //synchonisation

                bool.TryParse(ConfigurationManager.AppSettings["bSynchonizedDevice"], out bUseSynchonisation);
                DeviceIpRight = ConfigurationManager.AppSettings["DeviceIpRight"];
                DeviceIpLeft = ConfigurationManager.AppSettings["DeviceIpLeft"];
                int.TryParse(ConfigurationManager.AppSettings["DevicePortRight"], out DevicePortRight);
                int.TryParse(ConfigurationManager.AppSettings["DevicePortLeft"], out DevicePortLeft);
                int.TryParse(ConfigurationManager.AppSettings["TimeoutInSec"], out TimeoutInSec);
                bool.TryParse(ConfigurationManager.AppSettings["DoDoorScan"], out DoDoorScan); 

                if (bUseSynchonisation)
                {
                    syncDevice = new SynchronizedDevice()
                    {
                        bUseSynchonisation = true,
                        DeviceIpRight = DeviceIpRight,
                        DeviceIpLeft = DeviceIpLeft,
                        DevicePortLeft = DevicePortLeft,
                        DevicePortRight = DevicePortRight,
                        TimeoutInSec = TimeoutInSec
                    };
                    _tcpIpServer.syncDevice = syncDevice;
                }


                MaxTempFridgeValue = _db.GetMaxTempFridgeValue();
                _maxTimeOpenDoor = _db.GetDoorOpenToLongTime();               
                if (_bUseAlarm)
                {                    
                    if (_alertRemoved != null)
                    {
                        _removedReaderDevice.Clear();
                        string[] data = _alertRemoved.alertData.Split(';');
                        int.TryParse(data[0], out _maxTimeBeforeAlertRemoved);
                        if (data.Length > 2)
                        {
                            for (int i = 1; i < data.Length; i++)
                                _removedReaderDevice.Add(data[i]);
                        }

                        ClockAlertRemoved.Interval = 60000;
                        ClockAlertRemoved.Elapsed += ClockAlertRemoved_Tick;
                        ClockAlertRemoved.Start();
                    }
                }

                ConfigureList();

       
                _sip = new ScanInProgress();
                _sip.Hide();

                string valTime = ConfigurationManager.AppSettings["TimeZoneOffset"];

                CultureInfo culture;

                if (valTime.Contains("."))
                {
                    // Utilisation de InvariantCulture si présence du . comme séparateur décimal. 
                    culture = CultureInfo.InvariantCulture;
                }
                else
                {
                    // Utilisation de CurrentCulture sinon (utilisation de , comme séparateur décimal).
                    culture = CultureInfo.CurrentCulture;
                }

                // Conversion de la chaîne en double, en utilisant la culture correspondante.            
                double.TryParse(valTime, NumberStyles.Number, culture, out _timeZoneOffset);

                _tcpIpServer.TimeZoneOffset = _timeZoneOffset;



                _treeImageList = new ImageList();
                _treeImageList.Images.Add(new Bitmap(Resources.button_yellow_off));
                _treeImageList.Images.Add(new Bitmap(Resources.button_green_on));
                _treeImageList.Images.Add(new Bitmap(Resources.button_red_on));
                _treeImageList.Images.Add(new Bitmap(Resources.refresh));

                treeViewDevice.ImageList = _treeImageList;

                for (int i = 0; i < 100; i++) _treeviewExpandArray[i] = false;

                copyLotIDToClipBoardToolStripMenuItem.Text = string.Format(ResStrings.str_Copy_to_ClipBoard, _columnInfo[1]);


                GetProduct();
                CreateLocalDevice();
                CreateNetworkDevice();               
                UpdateTreeView();
                //UpdateScanHistory(null);

                _dateFridgeALarmBlocked = DateTime.Now;
                ClockFridge.Interval = 1000;
                ClockFridge.Elapsed += ClockFridgeTimer_Tick;

                if (treeViewDevice.Nodes.Count > 0)
                    _selectedReader = 0;

                //Eric test KSA Not remove old Inventory
                if (!File.Exists(@"C:\temp\AutoScan.txt"))
                {
                    if (_bShowServer)
                        DeleteOldEntry();
                }


                UserClassTemplate[] users = _db.RecoverUser();
                if (users != null)
                {
                    _localUserArray = new UserClassTemplate[users.Length];
                    users.CopyTo(_localUserArray, 0);
                }
                else
                    _localUserArray = null;
                timerStartup.Enabled = true;
                timerStartup.Start();
                _bClosing = false;

                _firstName = ResStrings.str_Emergency;
                _lastName = ResStrings.str_Opening;
                LastBadge = string.Empty;
                _lastDoorUser = DoorInfo.DI_NO_DOOR;
                _lastAccessType = AccessType.AT_NONE;

                _myRegexUid = new Regex(@"[0-7]{10}");
                

            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }

            bool.TryParse(ConfigurationManager.AppSettings["bUseCsvExchange"], out bUseCvsExchange);
            if (bUseCvsExchange)
            {
                try
                {
                    pathCsvFolder = ConfigurationManager.AppSettings["pathCsvExchange"];
                    cvsMachineId = ConfigurationManager.AppSettings["CsvMachineId"];
                    if (Directory.Exists(pathCsvFolder))
                        initCsvWatcher();
                }
                catch (Exception er)
                {
                    ErrorMessage.ExceptionMessageBox.Show(er);
                    bUseCvsExchange = false;
                }

            }

           

        }
        private void LiveDataForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_ftf != null)
                {
                    _ftf.Close();
                    _ftf.Dispose();
                }
                ClockFridge.Stop();
                _bClosing = true;
                timerStartup.Enabled = false;
                

                if (_netThread != null)
                    _netThread.Abort();
                closeTcpServerNotification();
                _db.CloseDB();
                CloseReader();

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ExceptionMessageBox.Show(exp);
            }
        }
        private void toolStripButtonScan_Click(object sender, EventArgs e)
        {
           
            try
            {
                _bti = null;
                UpdateTabBox(_bti);
                _checkedAccumulate = accumulateToolStripMenuItem.Checked;
                _checkedWaitMode = waitModeToolStripMenuItem.Checked;
                _bWasInAccumulation = _checkedAccumulate;
             
                if ((_selectedReader >= 0) & (_selectedReader < _nbLocalDevice))  //reader local
                {
                    _bFirstScanAccumulate = true;
                    _bUserScan = false;
                    _firstName = ResStrings.str_Manual;
                    _lastName = ResStrings.str_Scan;
                    _lastAccessType = AccessType.AT_NONE;                    
                    LastBadge = string.Empty;
                    _lastDoorUser = DoorInfo.DI_NO_DOOR;                   
                    if (treeViewDevice.SelectedNode != null)
                    {
                        int treeNodeSelected = treeViewDevice.SelectedNode.Index;
                        _localDeviceArray[treeNodeSelected].inventorySequence = 0;
                        _localDeviceArray[treeNodeSelected].tagList.Clear();                    
                     


                        if ((_localDeviceArray[treeNodeSelected].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                            (_localDeviceArray[treeNodeSelected].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
                        {

                            if (_localDeviceArray[treeNodeSelected].infoDev.deviceType == DeviceType.DT_SBR)
                            {
                                _bFirstScanAccumulate = !_bKeepLastScan;
                                _bAccumulateSbr = _checkedAccumulate || _checkedInOut;
                            }
                            else
                                _bAccumulateSbr = false;

                            toolStripButtonScan.Enabled = false;
                            toolStripButtonStopScan.Enabled = true;

                            if (_localDeviceArray[treeNodeSelected].infoDev.deviceType == DeviceType.DT_SBR)
                            {
                                if (_checkedWaitMode)
                                    _localDeviceArray[treeNodeSelected].rfidDev.EnableWaitMode();
                                else
                                    _localDeviceArray[treeNodeSelected].rfidDev.ScanDevice(false);
                            }
                            else
                            {
                                if (bUseSynchonisation)
                                {                                  
                                    if (syncDevice != null)
                                    {
                                        _localDeviceArray[treeNodeSelected].rfidDev.DeviceStatus = DeviceStatus.DS_WaitForScan;
                                        UpdateTreeView();
                                        syncDevice.bIsWaitingScan = true;
                                        Thread thScan = new Thread(() => syncDevice.CanStartScan());
                                        thScan.IsBackground = true;
                                        thScan.Start();
                                        while (syncDevice.bIsWaitingScan)
                                        {
                                            tcpUtils.NonBlockingSleep(1000);
                                        }
                                    }
                                }
                                _localDeviceArray[treeNodeSelected].rfidDev.ScanDevice();
                            }
                        }
                        else
                        {
                            if (_localDeviceArray[treeNodeSelected].infoDev.enabled == 1)
                                MessageBox.Show(ResStrings.StrDeviceNotReadyOrNotConnected, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            else
                                MessageBox.Show(ResStrings.str_Device_disabled, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                else //Network device
                {
                    if (treeViewDevice.SelectedNode != null)
                    {
                       
                        SelectedNetworkDevice.bnetAccumulateMode = false;
                        SelectedNetworkDevice.bnetWaitTagMode = false;
                        timerRefreshScan.Interval = 500;
                        timerRefreshScan.Enabled = true;

                       
                        InventoryData lastScan = _db.GetLastScan(SelectedNetworkDevice.infoDev.SerialRFID);
                        if (lastScan != null)
                            SelectedNetworkDevice.previousInventory = lastScan;
                        else
                            SelectedNetworkDevice.previousInventory = SelectedNetworkDevice.currentInventory;


                        listBoxTag.Items.Clear();
                        labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags,0);

                        Invoke((MethodInvoker)delegate { toolStripButtonScan.Enabled = false; });
                        Invoke((MethodInvoker)delegate { toolStripButtonStopScan.Enabled = true; });

                        if (SelectedNetworkDevice.netDeviceStatus == DeviceStatus.DS_Ready)
                        {
                            InScan = true;
                            SelectedNetworkDevice.netDeviceStatus = DeviceStatus.DS_InScan;
                            TcpIpClient tcp = DeviceClientTcp[SelectedNetworkDevice.infoDev.IP_Server];
                            tcp.requestScan(SelectedNetworkDevice.infoDev.IP_Server, SelectedNetworkDevice.infoDev.Port_Server, SelectedNetworkDevice.infoDev.SerialRFID);
                        }
                        else
                            MessageBox.Show(ResStrings.str_device_not_ready,ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    }
                }

                Invoke((MethodInvoker)delegate { labelPresent.Text = string.Format(ResStrings.str_PRESENT_format,0); pictureBoxPresent.Refresh(); });
                Invoke((MethodInvoker)delegate { labelAdded.Text = string.Format(ResStrings.str_ADDED_format,0); pictureBoxAdded.Refresh(); });
                Invoke((MethodInvoker)delegate { labelRemoved.Text = string.Format(ResStrings.str_REMOVED_format, 0); pictureBoxRemoved.Refresh(); });

                UpdateTreeView();
              
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ExceptionMessageBox.Show(exp);
            }
        }
        private void toolStripButtonStopScan_Click(object sender, EventArgs e)
        {
            _bStop = true;
            if ((_selectedReader >= 0) & (_selectedReader < _nbLocalDevice))  //reader local
            {             

                if (_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SBR)
                {
                    if (_checkedWaitMode)
                    {
                        if ((_localDeviceArray[_selectedReader].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                       (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus == DeviceStatus.DS_InScan))
                        {
                            _localDeviceArray[_selectedReader].rfidDev.StopScan();
                            Thread.Sleep(1000);
                        }
                        if ((_localDeviceArray[_selectedReader].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                           (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus == DeviceStatus.DS_WaitTag))
                        {
                            _localDeviceArray[_selectedReader].rfidDev.DisableWaitMode();
                            _localDeviceArray[_selectedReader].bComeFromKZ = false;
                        }
                    }
                    else if (_checkedAccumulate || _checkedInOut)
                    {
                        _bAccumulateSbr = false;
                        _checkedAccumulate = false;
                        toolStripButtonScan.Enabled = false;
                        toolStripButtonStopScan.Enabled = false;

                        if ((_localDeviceArray[_selectedReader].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                       (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus == DeviceStatus.DS_InScan))
                        {
                            _localDeviceArray[_selectedReader].rfidDev.StopScan();
                        }

                        Application.DoEvents();

                    }
                    else
                    {
                        if ((_localDeviceArray[_selectedReader].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                        (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus == DeviceStatus.DS_InScan))
                        {
                            _localDeviceArray[_selectedReader].rfidDev.StopScan();
                        }
                    }
                }
                else
                {
                    if ((_localDeviceArray[_selectedReader].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                       (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus == DeviceStatus.DS_InScan))
                    {
                        _localDeviceArray[_selectedReader].rfidDev.StopScan();
                    }
                    if ((_localDeviceArray[_selectedReader].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                       (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus == DeviceStatus.DS_LedOn))
                         _localDeviceArray[_selectedReader].rfidDev.StopLightingLeds();
                }

                UpdateTreeView();
            }
            else
            {
                if (SelectedNetworkDevice == null) return;

                TcpIpClient tcp = DeviceClientTcp[SelectedNetworkDevice.infoDev.IP_Server];
                tcp.requestStopScan(SelectedNetworkDevice.infoDev.IP_Server, SelectedNetworkDevice.infoDev.Port_Server, SelectedNetworkDevice.infoDev.SerialRFID);
                tcp.RequestStopLighting(SelectedNetworkDevice.infoDev.IP_Server, SelectedNetworkDevice.infoDev.Port_Server);
                InScan = false;
            }



        }
        private void toolStripButtonExit_Click(object sender, EventArgs e)
        {
            _bClosing = true;
            BeginInvoke((MethodInvoker)delegate { Close(); });
        }
        private void toolStripButExportXLS_Click(object sender, EventArgs e)
        {
            _runMacro = false;
            if ((_selectedReader >= 0) & (_selectedReader < _nbLocalDevice))  //reader local
            {
                if (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus != DeviceStatus.DS_InScan)
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
                else
                    MessageBox.Show(ResStrings.str_Unable_to_Export_Device_In_Scan,ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
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


        }
        private void exportAndRunExcelMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _runMacro = true;
            if ((_selectedReader >= 0) & (_selectedReader < _nbLocalDevice))  //reader local
            {
                if (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus != DeviceStatus.DS_InScan)
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
                else
                    MessageBox.Show(ResStrings.str_Unable_to_Export_Device_In_Scan, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
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
        }
        private void toolStripCompare_Click(object sender, EventArgs e)
        {
            if (SelectedNetworkDevice != null)
            {
                if (SelectedNetworkDevice.netDeviceStatus != DeviceStatus.DS_InScan)
                {
                    _cptInvForm = new CompareInventoryForm(SelectedNetworkDevice.currentInventory, _bCompareOnLotId);
                    _cptInvForm.Show();
                }
                else
                    MessageBox.Show(ResStrings.str_Unable_to_Compare_Device_in_Scan, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus != DeviceStatus.DS_InScan)
                {
                    _cptInvForm = new CompareInventoryForm(_localDeviceArray[_selectedReader].currentInventory, _bCompareOnLotId);
                    _cptInvForm.Show();
                }
                else
                    MessageBox.Show(ResStrings.str_Unable_to_Compare_Device_in_Scan, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void toolStripButtonRemoveLink_Click(object sender, EventArgs e)
        {
            int nbToDelete = listBoxTag.Items.Count;
            int nbDelete = 0;
            DialogResult res = MessageBox.Show(this, string.Format(ResStrings.strk_Are_You_sure_you_want_to_remove_the_association, nbToDelete), ResStrings.str_Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (res == DialogResult.Yes)
            {

                for (int loop = 0; loop < listBoxTag.Items.Count; loop++)
                {
                    if (_db.DeleteProduct(listBoxTag.Items[loop].ToString())) nbDelete++;
                }
                MessageBox.Show(this, string.Format(ResStrings.str_product_deleted, nbDelete, nbToDelete), ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                GetProduct();
            }


        }
        private void networkUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedNetworkDevice.netConnectionStatus == ConnectionStatus.CS_Connected)
            {
                _ru = new RemoteUser(SelectedNetworkDevice);
                _ru.ShowDialog(this);
            }
        }
        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckBox checkBox = _toolKeepLastScan.Control as CheckBox;
            if (checkBox != null) checkBox.Enabled = true;
            normalToolStripMenuItem.Checked = true;
            accumulateToolStripMenuItem.Checked = false;
            waitModeToolStripMenuItem.Checked = false;         
            _checkedAccumulate = false;
            _checkedWaitMode = false;            
            listBoxTag.Visible = false;
        }
        private void accumulateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckBox checkBox = _toolKeepLastScan.Control as CheckBox;
            if (checkBox != null) checkBox.Enabled = true;
            normalToolStripMenuItem.Checked = false;
            accumulateToolStripMenuItem.Checked = true;
            waitModeToolStripMenuItem.Checked = false;        
            _checkedAccumulate = true;
            _checkedWaitMode = false;
            _checkedInOut = false;           
            listBoxTag.Visible = accumulateToolStripMenuItem.Checked;        
        }
        private void waitModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            normalToolStripMenuItem.Checked = false;
            accumulateToolStripMenuItem.Checked = false;        
            _checkedAccumulate = false;
            _checkedWaitMode = true;
            _checkedInOut = false;           
            listBoxTag.Visible = true;
            CheckBox checkBox = _toolKeepLastScan.Control as CheckBox;
            if (checkBox != null) checkBox.Checked = false;
            CheckBox box = _toolKeepLastScan.Control as CheckBox;
            if (box != null) box.Enabled = false;
        }
       
        private void LiveDataForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                if (toolStripButtonScan.Enabled)
                    toolStripButtonScan_Click(this, null);
            }
            if (e.KeyCode == Keys.F3)
            {
                if (toolStripButtonStopScan.Enabled)
                    toolStripButtonStopScan_Click(this, null);
            }
            if (e.KeyCode == Keys.F4)
            {
                if (_toolKeepLastScan != null)
                {
                    CheckBox checkBox = _toolKeepLastScan.Control as CheckBox;
                    if (checkBox != null)
                        checkBox.Checked = !checkBox.Checked;
                }
            }

        }
        private void LiveDataForm_Activated(object sender, EventArgs e)
        {
            try
            {
                if (!_bClosing)
                    UpdateGroup();
            }
            catch
            {
                //Ie issue when reboot PC before leaving live data
            }
        }
        private void toolStripSplitButtonSBRMode_Click(object sender, EventArgs e)
        {
            if ((_selectedReader >= 0) & (_selectedReader < _nbLocalDevice))  //reader local
            {
                if (treeViewDevice.SelectedNode != null)
                {
                    int treeNodeSelected = treeViewDevice.SelectedNode.Index;
                    if ((_localDeviceArray[treeNodeSelected].infoDev.deviceType == DeviceType.DT_SBR) &&
                         (_localDeviceArray[treeNodeSelected].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
                    {
                        normalToolStripMenuItem.Enabled = true;
                        accumulateToolStripMenuItem.Enabled = true;
                        waitModeToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        normalToolStripMenuItem.Enabled = false;
                        accumulateToolStripMenuItem.Enabled = false;
                        waitModeToolStripMenuItem.Enabled = false;
                    }
                }
            }
            else
            {
                normalToolStripMenuItem.Enabled = false;
                accumulateToolStripMenuItem.Enabled = false;
                waitModeToolStripMenuItem.Enabled = false;
            }
        }
        #endregion
        #region treeview
        private void UpdateTreeView()
        {
            try
            {
                
                if (_bClosing) return;

                if (treeViewDevice.InvokeRequired)
                {
                    treeViewDevice.Invoke(new MethodInvoker(UpdateTreeView));
                }
                else
                {
                    if (treeViewDevice.SelectedNode != null)
                        _selIndex = treeViewDevice.SelectedNode.Index;
                    treeViewDevice.Nodes.Clear();
                    if (_localDeviceArray != null)
                    {
                        foreach (deviceClass dc in _localDeviceArray)
                        {
                            TreeNode serialnode = new TreeNode(string.Format(ResStrings.str_SerialSN, dc.infoDev.SerialRFID));
                            serialnode.StateImageIndex = 0;
                            TreeNode typeNode = new TreeNode(string.Format(ResStrings.str_Type, dc.infoDev.deviceType));
                            TreeNode conNode = new TreeNode(string.Format(ResStrings.str_Connection_Status_, getEnumDesc.GetEnumDescription(dc.rfidDev.ConnectionStatus)));
                            TreeNode rfidNode = new TreeNode(string.Format(ResStrings.str_Statut_, getEnumDesc.GetEnumDescription(dc.rfidDev.DeviceStatus)));

                            TreeNode[] ndArray = null;
                            TreeNode fpNodeMaster;
                            TreeNode fpNodeSlave;
                            TreeNode tempNode;
                            TreeNode tempMax;
                            TreeNode tempMin;
                            TreeNode tempMean;
                            TreeNode tempDate;
                            TreeNode tempCreation;
                          
                            switch (dc.infoDev.deviceType)
                            {
                                case DeviceType.DT_STR:
                                case DeviceType.DT_SBR:
                                case DeviceType.DT_SMC:
                                    ndArray = new[] { serialnode, typeNode, conNode, rfidNode };
                                    break;
                                case DeviceType.DT_SFR:
                                case DeviceType.DT_SBF:

                                    if ((dc.myFridgeCabinet != null) && (dc.myFridgeCabinet.GetTempInfo != null))
                                    {
                                        tempInfo fridgeTmpInfo = dc.myFridgeCabinet.GetTempInfo;
                                        // todo localTime.
                                        DateTime dtAcq = DateTime.SpecifyKind(fridgeTmpInfo.lastTempAcq, DateTimeKind.Utc);
                                        DateTime dtCeation = DateTime.SpecifyKind(fridgeTmpInfo.CreationDate, DateTimeKind.Utc);
                                        tempDate = new TreeNode(string.Format(ResStrings.str_Tacq, dtAcq.ToLocalTime().ToString("dd/MM/yy HH:mm:ss")));
                                        tempNode = new TreeNode(string.Format(ResStrings.str_Temp, fridgeTmpInfo.lastTempValue.ToString("0.00")));
                                        tempMean = new TreeNode(string.Format(ResStrings.str_TempAverage, fridgeTmpInfo.mean.ToString("0.00"), fridgeTmpInfo.nbValueTemp));
                                        tempMax = new TreeNode(string.Format(ResStrings.str_TempMax, fridgeTmpInfo.max.ToString("0.00")));
                                        tempMin = new TreeNode(string.Format(ResStrings.str_TempMin, fridgeTmpInfo.min.ToString("0.00")));
                                        tempCreation = new TreeNode(string.Format(ResStrings.strTempDate, dtCeation.ToLocalTime().ToString("dd/MM/yy HH:mm:ss")));
                                        ndArray = new[] { serialnode, typeNode, conNode, rfidNode, tempDate, tempNode, tempCreation, tempMean, tempMax, tempMin };
                                    }
                                    else
                                        ndArray = new[] { serialnode, typeNode, conNode, rfidNode };
                                    break;

                                case DeviceType.DT_JSC:
                                case DeviceType.DT_DSB:
                                    fpNodeMaster = new TreeNode(string.Format(ResStrings.strFP_Master_Statut_, dc.rfidDev.FPStatusMaster));
                                    ndArray = new[] { serialnode, typeNode, conNode, rfidNode, fpNodeMaster };
                                    break;

                                case DeviceType.DT_SAS:
                                case DeviceType.DT_MSR:
                                    fpNodeMaster = new TreeNode(string.Format(ResStrings.strFP_Master_Statut_, dc.rfidDev.FPStatusMaster));
                                    fpNodeSlave = new TreeNode(string.Format(ResStrings.str_FP_Slave_Statut_, dc.rfidDev.FPStatusSlave));
                                    ndArray = new[] { serialnode, typeNode, conNode, rfidNode, fpNodeMaster, fpNodeSlave };
                                    break;

                                default :
                                    _bClosing = true;
                                    MessageBox.Show(ResStrings.str_treeeviewerror, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    break;
                            }


                            if (ndArray != null)
                            {
                                TreeNode fullNode = new TreeNode(dc.infoDev.DeviceName + "      ", ndArray);
                                fullNode.NodeFont = new Font(treeViewDevice.Font, FontStyle.Bold);
                                if (dc.rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected)
                                    fullNode.ImageIndex = fullNode.SelectedImageIndex = 1;
                                else
                                    fullNode.ImageIndex = fullNode.SelectedImageIndex = 2;
                                int rg = treeViewDevice.Nodes.Add(fullNode);

                                if (_treeviewExpandArray[rg]) treeViewDevice.Nodes[rg].Expand();
                                else treeViewDevice.Nodes[rg].Collapse();

                            }



                        }

                        if (_bClosing)
                        {
                            _localDeviceArray = null;
                            _networkDeviceArray = null;
                            Close();
                        }
                    }



                    if (_networkDeviceArray != null)
                    {
                        foreach (deviceClass dc in _networkDeviceArray)
                        {
                            TreeNode serialnode = new TreeNode(string.Format(ResStrings.str_SerialSN, dc.infoDev.SerialRFID));
                            serialnode.StateImageIndex = 0;

                            TreeNode ipNode = new TreeNode(string.Format(ResStrings.str_IP_address_, dc.infoDev.IP_Server));
                            TreeNode portNode = new TreeNode(string.Format(ResStrings.str_serverport, dc.infoDev.Port_Server));
                            TreeNode typeNode = new TreeNode(string.Format(ResStrings.str_Type, dc.infoDev.deviceType));
                            TreeNode conNode = new TreeNode(string.Format(ResStrings.str_Connection_Status_, getEnumDesc.GetEnumDescription(dc.netConnectionStatus)));
                            TreeNode rfidNode = new TreeNode(string.Format(ResStrings.str_Statut_, getEnumDesc.GetEnumDescription(dc.netDeviceStatus)));
                            TreeNode[] ndArray = null;

                            switch (dc.infoDev.deviceType)
                            {
                                case DeviceType.DT_SFR:
                                case DeviceType.DT_SBF:
                                    if (dc.currentTemp != null)
                                    {
                                        TreeNode tempNode;
                                        TreeNode tempMax;
                                        TreeNode tempMin;
                                        TreeNode tempMean;
                                        TreeNode tempDate;
                                        TreeNode tempCreation;
                                       // TreeNode tempDefrostStatus;
                                        DateTime dtAcq = DateTime.SpecifyKind(dc.currentTemp.lastTempAcq, DateTimeKind.Utc);
                                        DateTime dtCeation = DateTime.SpecifyKind(dc.currentTemp.CreationDate, DateTimeKind.Utc);
                                        tempDate = new TreeNode(string.Format(ResStrings.str_Tacq, dtAcq.ToLocalTime().ToString("dd/MM/yy HH:mm:ss")));
                                        tempNode = new TreeNode(string.Format(ResStrings.str_Temp, dc.currentTemp.lastTempValue.ToString("0.00")));
                                        tempMean = new TreeNode(string.Format(ResStrings.str_TempAverage, dc.currentTemp.mean.ToString("0.00"), dc.currentTemp.nbValueTemp));
                                        tempMax = new TreeNode(string.Format(ResStrings.str_TempMax, dc.currentTemp.max.ToString("0.00")));
                                        tempMin = new TreeNode(string.Format(ResStrings.str_TempMin, dc.currentTemp.min.ToString("0.00")));
                                        tempCreation = new TreeNode(string.Format(ResStrings.strTempDate, dtCeation.ToLocalTime().ToString("dd/MM/yy HH:mm:ss")));
                                        ndArray = new[] { serialnode, ipNode, portNode, typeNode, conNode, rfidNode, tempDate, tempNode, tempCreation, tempMean, tempMax, tempMin};

                                    }
                                    else
                                    {
                                        ndArray = new[] { serialnode, ipNode, portNode, typeNode, conNode, rfidNode };
                                    }
                                    break;

                                default:

                                    ndArray = new[] { serialnode, ipNode, portNode, typeNode, conNode, rfidNode };
                                    break;

                            }
                            
                            TreeNode fullNode = new TreeNode(dc.infoDev.DeviceName + "      ", ndArray);
                            fullNode.NodeFont = new Font(treeViewDevice.Font, FontStyle.Bold);

                            if (dc.netConnectionStatus == ConnectionStatus.CS_Connected)
                                fullNode.ImageIndex = fullNode.SelectedImageIndex = 1;
                            else
                                fullNode.ImageIndex = fullNode.SelectedImageIndex = 2;                                

                            int rg = treeViewDevice.Nodes.Add(fullNode);

                            if (_treeviewExpandArray[rg]) treeViewDevice.Nodes[rg].Expand();
                            else treeViewDevice.Nodes[rg].Collapse();
                        }
                    }
                    if (_selIndex >= 0)
                        treeViewDevice.SelectedNode = treeViewDevice.Nodes[_selIndex];
                    treeViewDevice.Refresh();
                }

            }
            catch
            {

            }

        }
        private void treeViewDevice_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Invoke((MethodInvoker)delegate { _toolKeepLastScan.Enabled = false; });
            if (treeViewDevice.SelectedNode != null)
            {
                if (treeViewDevice.SelectedNode.Parent != null)
                {
                    TreeNode parNode = treeViewDevice.SelectedNode.Parent;
                    treeViewDevice.SelectedNode = parNode;
                    return;
                }
                _selectedReader = treeViewDevice.SelectedNode.Index;
            }
            else
            {
                if (treeViewDevice.Nodes.Count > 0)
                {
                    TreeNode parNode = treeViewDevice.Nodes[0];
                    treeViewDevice.SelectedNode = parNode;
                    _selectedReader = 0;
                }
            }

            SelectedNetworkDevice = null;
            treeViewDevice.ContextMenuStrip = null;
            if ((_selectedReader >= 0) & (_selectedReader < _nbLocalDevice))  //reader local
            {
                toolStripSplitButtonSBRMode.Enabled = true;
                if ((_localDeviceArray[_selectedReader].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                    (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
                {
                    Invoke((MethodInvoker)delegate { toolStripButtonScan.Enabled = true; });
                    if (_localDeviceArray[_selectedReader].rfidDev.get_RFID_Device.FirmwareVersion.StartsWith("1"))
                        Invoke((MethodInvoker)delegate { 
                            ledONToolStripMenuItem.Visible = false;
                            updateTagUIDToolStripMenuItem.Visible = false; 
                        });
                    else
                        Invoke((MethodInvoker)delegate { 
                            ledONToolStripMenuItem.Visible = true;
                            updateTagUIDToolStripMenuItem.Visible = true;
                        });

                }
                else
                {
                    Invoke((MethodInvoker)delegate { toolStripButtonScan.Enabled = false; });
                    Invoke((MethodInvoker)delegate { 
                        ledONToolStripMenuItem.Visible = false;
                        updateTagUIDToolStripMenuItem.Visible = false;
                    });
                }

                
                if (_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SBF)
                {
                    
                    toolStripLabelPatientName.Visible = true;
                    toolStripComboBoxPatientName.Visible = true;
                    if (_previousSelectedreader != _selectedReader)
                    {
                        UpdatePatientCombo(_localDeviceArray[_selectedReader].currentInventory.dtTagAll);
                        _previousSelectedreader = _selectedReader;
                    }
                }
                else
                {
                    toolStripLabelPatientName.Visible = false;
                    toolStripComboBoxPatientName.Visible = false;
                    _selectedPatient = ResStrings.str_None;
                }

                // if ((!bAccumulateSBR))
                if (!_bWasInAccumulation)
                {
                    if (!_bComeFromFridge)
                    {
                        dataListView.Items.Clear();
                        Thread.Sleep(5);
                        RefreshInventory();

                       /* if (tabControlInfo.SelectedIndex == 1)
                            UpdateScanHistory(null);*/
                    }
                }
                _bComeFromFridge = false;

               
            }
            else
            {
                SelectedNetworkDevice = _networkDeviceArray[_selectedReader - _nbLocalDevice];
                SelectedNetworkIndex = _selectedReader - _nbLocalDevice;
                
                if (CanHandleLeds.ContainsKey(SelectedNetworkDevice) && CanHandleLeds[SelectedNetworkDevice])
                {
                    Invoke((MethodInvoker)delegate { stepByStepToolStripMenuItem.Visible = false; }); // No step by step lighting in ethernet mode
                    Invoke((MethodInvoker)delegate { ledONToolStripMenuItem.Visible = true; });
                    Invoke((MethodInvoker)delegate { updateTagUIDToolStripMenuItem.Visible = true; });
                }

                toolStripSplitButtonSBRMode.Enabled = false;
                if (_networkDeviceArray[_selectedReader - _nbLocalDevice].netConnectionStatus == ConnectionStatus.CS_Connected)
                {
                    Invoke((MethodInvoker)delegate { toolStripButtonScan.Enabled = true; });
                    Invoke((MethodInvoker)delegate { toolStripButtonStopScan.Enabled = true; });
                }
                else
                {
                    Invoke((MethodInvoker)delegate { toolStripButtonScan.Enabled = false; });
                    Invoke((MethodInvoker)delegate { toolStripButtonStopScan.Enabled = true; });
                }
                if (SelectedNetworkDevice.infoDev.deviceType != DeviceType.DT_SBR)
                    treeViewDevice.ContextMenuStrip = contextMenuStripUser;
                else
                    Invoke((MethodInvoker)delegate { _toolKeepLastScan.Enabled = true; });
                
                if (SelectedNetworkDevice.infoDev.deviceType == DeviceType.DT_SBF)
                {
                    toolStripLabelPatientName.Visible = true;
                    toolStripComboBoxPatientName.Visible = true;

                    if (_previousSelectedreader != _selectedReader)
                    {
                        UpdatePatientCombo(SelectedNetworkDevice.currentInventory.dtTagAll);
                        _previousSelectedreader = _selectedReader;
                    }
                }
                else
                {
                    toolStripLabelPatientName.Visible = false;
                    toolStripComboBoxPatientName.Visible = false;
                    _selectedPatient = ResStrings.str_None;
                }

                // if ((!bAccumulateSBR))
                if (!_bWasInAccumulation)
                {
                    if (!_bComeFromFridge) 
                    {
                        if (SelectedNetworkDevice.netDeviceStatus != DeviceStatus.DS_InScan)
                        {
                            dataListView.Items.Clear();
                            RefreshInventory();
                        }

                        /*if (tabControlInfo.SelectedIndex == 1)
                            UpdateScanHistory(null);*/
                    }
                }
                _bComeFromFridge = false;
                
            }

            
         
          

        }
        private void treeViewDevice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D && e.Modifiers == Keys.Control)
            {
                if (treeViewDevice.ContextMenuStrip == null)
                    treeViewDevice.ContextMenuStrip = contextMenuStripReader;
                else
                    treeViewDevice.ContextMenuStrip = null;
            }
        }
        private void treeViewDevice_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            _treeviewExpandArray[e.Node.Index] = false;
        }
        private void treeViewDevice_AfterExpand(object sender, TreeViewEventArgs e)
        {
            _treeviewExpandArray[e.Node.Index] = true;
        }
        private void treeViewDevice_DoubleClick(object sender, EventArgs e)
        {
            currentTempValue = 0.0;
           
            if (SelectedNetworkDevice != null)
            {
                if ((SelectedNetworkDevice.infoDev.deviceType == DeviceType.DT_SBF) || (SelectedNetworkDevice.infoDev.deviceType == DeviceType.DT_SFR))
                {
                    if (!SelectedNetworkDevice.netFridgeTemperatureProcessDone)
                    {
                        MessageBox.Show(ResStrings.str_Temperature_data_in_acquisition___Please_wait__, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }    
                    if (SelectedNetworkDevice.currentTemp == null)
                    {
                        MessageBox.Show(ResStrings.str_No_temperature_data_acquired, ResStrings.strInfo,MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                        return;
                    }
                    UpdateTemp(SelectedNetworkIndex, SelectedNetworkDevice.infoDev.SerialRFID);

                    if (_ftf == null)
                    {
                        _ftf = new FridgeTempFrm(this);
                        _ftf.StartPosition = FormStartPosition.Manual;
                        _ftf.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - _ftf.Width, Screen.PrimaryScreen.WorkingArea.Bottom - _ftf.Height);
                        _ftf.Show();
                    }
                    else
                    {
                        _ftf.Close();
                        _ftf.Dispose();
                        _ftf = new FridgeTempFrm(this);
                        _ftf.StartPosition = FormStartPosition.Manual;
                        _ftf.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - _ftf.Width, Screen.PrimaryScreen.WorkingArea.Bottom - _ftf.Height);
                        _ftf.Show();
                    }
                    _bComeFromFridge = true;
                    UpdateTreeView();
                    ClockFridge.Interval = 100;
                    ClockFridge.Start();
                }
            }
            else
            {
              

                UpdateTreeView();
                ClockFridge.Interval = 100;
                ClockFridge.Start();

            }
        }
        private void UpdatePatientCombo(DataTable dt)
        {
            try
            {
                string columnPatient = _db.GetColumnNameForAlert(AlertType.AT_Bad_Blood_Patient);
                if (!string.IsNullOrEmpty(columnPatient))
                {
                    toolStripComboBoxPatientName.Items.Clear();
                    toolStripComboBoxPatientName.Sorted = true;
                    foreach (DataRow oRow in dt.Rows)
                    {
                        if (oRow[columnPatient] != DBNull.Value)
                        {
                            string tmpPat = (string)oRow[columnPatient];
                            if (!toolStripComboBoxPatientName.Items.Contains(tmpPat))
                                toolStripComboBoxPatientName.Items.Add(tmpPat);
                        }
                    }
                    toolStripComboBoxPatientName.Sorted = false;
                    toolStripComboBoxPatientName.Items.Insert(0,ResStrings.str_None);

                    toolStripComboBoxPatientName.SelectedIndex = 0;
                    _selectedPatient = toolStripComboBoxPatientName.Text;
                }
            }
            catch
            {

            }
        }
        private void toolStripComboBoxPatientName_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedPatient = toolStripComboBoxPatientName.Text;
        }
        #endregion
        #region Alert

        private alertInfo _alertItems;
        private alertInfo _alertValues;
        private alertInfo _alertStock;
        private alertInfo _alertBloodPatient;
        private alertInfo _alertRemoved;
        private alertInfo _alertDoors;
        private alertInfo _alertFinger;
        private alertInfo _alertFridge;
        private void InitAlert()
        {
             
             _alertItems = _db.getAlertInfo(AlertType.AT_Remove_Too_Many_Items, true);
             _alertValues = _db.getAlertInfo(AlertType.AT_Limit_Value_Exceed, true);
             _alertStock = _db.getAlertInfo(AlertType.AT_Stock_Limit, true);
             _alertBloodPatient = _db.getAlertInfo(AlertType.AT_Bad_Blood_Patient, true);
             _alertRemoved = _db.getAlertInfo(AlertType.AT_Remove_Tag_Max_Time, true);
             _alertDoors = _db.getAlertInfo(AlertType.AT_Door_Open_Too_Long, true);
             _alertFinger = _db.getAlertInfo(AlertType.AT_Finger_Alert, true);
             _alertFridge = _db.getAlertInfo(AlertType.AT_Max_Fridge_Temp, true);
        }
        private void ClockAlertRemoved_Tick(object sender, EventArgs eArgs)
        {
            if (sender == ClockAlertRemoved)
            {
                ClockAlertRemoved.Stop();

                if (_removedInventoryAlert.Count > 0)
                {
                    List<DateTime> listDate = new List<DateTime>(_removedInventoryAlert.Keys);

                    foreach (DateTime dt in listDate)
                    {
                        if (dt < DateTime.Now)
                        {
                            TreatAlertRemove(dt);
                        }
                    }

                }
                ClockAlertRemoved.Start();
            }
        }
        private void TreatAlertRemove(DateTime dt)
        {
            DateTime dateToTreat = dt;
            List<string> tagToAlert = new List<string>();
            if (_removedInventoryAlert.ContainsKey(dateToTreat))
            {
                deviceClass dc = _removedInventoryAlert[dateToTreat];
                foreach (string tagId in dc.currentInventory.listTagRemoved)
                {
                    TagEventClass tec = _db.GetLastTagEvent(tagId);
                    if (tec == null) continue;
                    if (dt > DateTime.Parse(tec.eventDate))
                    {
                        tagToAlert.Add(tagId);
                    }
                }

                if (tagToAlert.Count > 0)
                {
                    string spareData = _maxTimeBeforeAlertRemoved.ToString(CultureInfo.InvariantCulture);
                    foreach (string str in tagToAlert)
                    {
                        spareData += ";" + str;
                        foreach (DataRow row in dc.currentInventory.dtTagRemove.Rows)
                        {
                            if (row.ItemArray[0].ToString().Equals(str))
                            {
                                for (int i = 1; i < row.ItemArray.Length; i++)
                                    spareData += " - " + row.ItemArray[i];
                                break;
                            }
                        }
                    }

                    _db.storeAlert(AlertType.AT_Remove_Tag_Max_Time, dc.infoDev, null, spareData);
                    AlertMgtClass.treatAlert(AlertType.AT_Remove_Tag_Max_Time, dc.infoDev, null, null, spareData, BShowAlert);
                }
            }
            _removedInventoryAlert.Remove(dt);


        }
        private double ProcessValueAlert(DataTable dt, string colName)
        {
            double sumValue = 0.0;

            foreach (DataRow oRow in dt.Rows)
            {
                string strValue = oRow[colName].ToString();
                strValue = strValue.Replace(" ", "");
                strValue = strValue.Replace("'", "");

                if (strValue.Contains("."))
                {
                    double tmpVal;
                    if (double.TryParse(strValue, NumberStyles.Number, CultureInfo.InvariantCulture, out tmpVal))
                        sumValue += tmpVal;
                }
                else
                {
                    double tmpVal;
                    if (double.TryParse(strValue, NumberStyles.Number, CultureInfo.CurrentCulture, out tmpVal))
                        sumValue += tmpVal;
                }
            }
            return sumValue;
        }
        private double ProcessValueAlert(DataTable dt, string colName , string colToGroup , string valToGroup )
        {
            double sumValue = 0.0;

            foreach (DataRow oRow in dt.Rows)
            {
                string groupValue = oRow[colToGroup].ToString();
                if (groupValue != valToGroup) continue;
                string strValue = oRow[colName].ToString();
                strValue = strValue.Replace(" ", "");
                strValue = strValue.Replace("'", "");

                if (strValue.Contains("."))
                {
                    double tmpVal;
                    if (double.TryParse(strValue, NumberStyles.Number, CultureInfo.InvariantCulture, out tmpVal))
                        sumValue += tmpVal;
                }
                else
                {
                    double tmpVal;
                    if (double.TryParse(strValue, NumberStyles.Number, CultureInfo.CurrentCulture, out tmpVal))
                        sumValue += tmpVal;
                }
            }
            return sumValue;
        }
        #endregion
        #region dataList
        public void ConfigureList()
        {
            // cretate datatable for column
            DataTable newDt = new DataTable();
            newDt.Columns.Add(ResStrings.str_Event, typeof(string));

            for (int i = 0; i < _columnInfo.Count; i++)
            {
                newDt.Columns.Add(_columnInfo[i].ToString(), typeof(string));
            }            
            newDt.Columns.Add(ResStrings.str_Tag_Location, typeof(string));
            newDt.Columns.Add(ResStrings.str_Expiration, typeof(string));
            dataListView.DataSource = null;
            dataListView.DataSource = newDt;

            for (int i = 0; i < _columnInfo.Count; i++)
            {
                dataListView.Columns[i + 1].Name = _columnInfo[i].ToString();
                dataListView.Columns[i + 1].Text = _columnInfo[i].ToString();
            }
            

            //empty List

            dataListView.EmptyListMsg = "";   
            TextOverlay textOverlay = dataListView.EmptyListMsgOverlay as TextOverlay;
            if (textOverlay != null)
            {
                textOverlay.TextColor = Color.Firebrick;
                textOverlay.BackColor = Color.AntiqueWhite;
                textOverlay.BorderColor = Color.DarkRed;
                textOverlay.BorderWidth = 4.0f;
                textOverlay.Font = new Font("Chiller", 36);
                textOverlay.Rotation = -5;
            }


            for (int i = 0; i < dataListView.Columns.Count; i++)
            {
                OLVColumn ol = dataListView.GetColumn(i);
                //ol.FillsFreeSpace = true;
                ol.HeaderFont = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
                ol.HeaderForeColor = Color.AliceBlue;
                ol.IsTileViewColumn = true;
                ol.UseInitialLetterForGroup = false;
                ol.MinimumWidth = 20 + ol.Text.Length * 10;

            }

            if (BShowImage)
            {
                if (dataListView.UseTranslucentHotItem)
                {
                    dataListView.HotItemStyle.Overlay = new BusinessCardOverlay(_columnInfo.Count);
                    dataListView.HotItemStyle = dataListView.HotItemStyle;
                }
            }
            else
            {

                RowBorderDecoration rbd = new RowBorderDecoration();
                rbd.BorderPen = new Pen(Color.SeaGreen, 2);
                rbd.FillBrush = null;
                rbd.CornerRounding = 4.0f;
                HotItemStyle hotItemStyle2 = new HotItemStyle();
                hotItemStyle2.Decoration = rbd;
                dataListView.HotItemStyle = hotItemStyle2;
            }           
       

            // Install a custom renderer that draws the Tile view in a special way
            dataListView.ItemRenderer = new BusinessCardRenderer();
            dataListView.Invalidate();

                }
        private void timerObjectList_Tick(object sender, EventArgs e)
        {

            timerObjectList.Stop();
            Stopwatch s2 = Stopwatch.StartNew();
            RefreshDatalist();
            s2.Stop();
            Debug.WriteLine("Refresh datalist " + s2.Elapsed.Milliseconds + "  ms");
            if (bScanFromTimer)
            {
                bScanFromTimer = false;
                Invoke((MethodInvoker) delegate { timerAutoPad.Enabled = true; });
            }

        }
        private void RefreshDatalist()
        {
            Invoke((MethodInvoker)delegate
            {
                Cursor.Current = Cursors.WaitCursor;
                try
                {

                    if (!string.IsNullOrEmpty(_columnExpiredDate))
                    {
                        for (int i = 0; i < dataListView.Columns.Count; i++)
                        {
                            OLVColumn ol = dataListView.GetColumn(i);
                            if (ol.Name == _columnExpiredDate)
                            {
                                _rowDlcDate = i;
                            }
                        }
                    }

                    if ((_selectedReader >= 0) & (_selectedReader < _nbLocalDevice)) //reader local
                    {
                        if (_localDeviceArray == null) return;
                        lock (_locker)
                        {
                            dataListView.Items.Clear();

                            if (_bShowServer)
                                dataListView.ShowGroups = false;

                            if (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus == DeviceStatus.DS_InScan)
                                return;


                            // rebuil before empty list
                            bool needUpdate = false;

                            OLVColumn aColumn = dataListView.AllColumns[dataListView.AllColumns.Count - 2];
                            bool previousState = aColumn.IsVisible;
                            aColumn.IsVisible = false;
                            if ((_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SFR) ||
                                (_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SBF))
                                aColumn.IsVisible = true;

                            if (aColumn.IsVisible != previousState) needUpdate = true;

                            OLVColumn bColumn = dataListView.AllColumns[dataListView.AllColumns.Count - 1];
                            previousState = bColumn.IsVisible;
                            if (!string.IsNullOrEmpty(_columnExpiredDate))
                                bColumn.IsVisible = true;
                            else
                                bColumn.IsVisible = false;
                            if (bColumn.IsVisible != previousState) needUpdate = true;
                            if (needUpdate) dataListView.RebuildColumns();
                            /***************/

                            DataTable newDt = new DataTable();
                            newDt.Columns.Add(ResStrings.str_Event, typeof (string));

                            for (int i = 0; i < _columnInfo.Count; i++)
                            {
                                newDt.Columns.Add(_columnInfo[i].ToString(), typeof (string));
                            }

                            newDt.Columns.Add(ResStrings.str_Tag_Location, typeof (string));
                            newDt.Columns.Add(ResStrings.str_Expiration, typeof (string));

                            foreach (DataRow dr in _localDeviceArray[_selectedReader].currentInventory.dtTagAdded.Rows)
                            {

                                DataRow rowToadd = newDt.NewRow();

                                rowToadd[0] = ResStrings.str_Added;
                                for (int i = 0; i < dr.ItemArray.Length; i++)
                                    rowToadd[i + 1] = dr.ItemArray[i];

                                if ((_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SFR) ||
                                    (_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SBF))
                                {
                                    if ((_localDeviceArray[_selectedReader].currentInventory.ListTagWithChannel != null) &&
                                        (_localDeviceArray[_selectedReader].currentInventory.ListTagWithChannel.Contains
                                            (dr.ItemArray[0])))
                                    {
                                        string tmpInfo = string.Format(ResStrings.str_Shelf,
                                            _localDeviceArray[_selectedReader].currentInventory.ListTagWithChannel[
                                                dr.ItemArray[0]]);
                                        rowToadd[dr.ItemArray.Length + 1] = tmpInfo;
                                    }
                                    else
                                    {
                                        // put shelve 1 if no value
                                        string tmpInfo = string.Format(ResStrings.str_Shelf, 1);
                                        rowToadd[dr.ItemArray.Length + 1] = tmpInfo;
                                    }
                                }
                                if (!string.IsNullOrEmpty(_columnExpiredDate))
                                {
                                    string date = (string) dr.ItemArray[_rowDlcDate - 1];
                                    DateTime dt;
                                    if (DateTime.TryParseExact(date, _formats, CultureInfo.CurrentUICulture,
                                        DateTimeStyles.None, out dt))
                                    {
                                        DateTime now = DateTime.Now;
                                        TimeSpan elapsed = dt.Subtract(now);
                                        double daysAgo = elapsed.TotalDays;

                                        if (daysAgo < 0)
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Expired;
                                        else if ((daysAgo >= 0) & (daysAgo < 30))
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Less_than_30_Days_Left;
                                        else if ((daysAgo >= 31) & (daysAgo < 90))
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Less_than_90_Days_Left;
                                        else if (daysAgo >= 91)
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_More_than_90_Days_Left;

                                    }
                                    else
                                    {
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_No_Date;
                                    }
                                }
                                newDt.Rows.Add(rowToadd);
                            }

                            foreach (DataRow dr in _localDeviceArray[_selectedReader].currentInventory.dtTagRemove.Rows)
                            {
                                DataRow rowToadd = newDt.NewRow();

                                rowToadd[0] = ResStrings.str_Removed;
                                for (int i = 0; i < dr.ItemArray.Length; i++)
                                    rowToadd[i + 1] = dr.ItemArray[i];
                                if ((_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SFR) ||
                                    (_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SBF))
                                {
                                    if ((_localDeviceArray[_selectedReader].currentInventory.ListTagWithChannel != null) &&
                                        (_localDeviceArray[_selectedReader].currentInventory.ListTagWithChannel.Contains
                                            (dr.ItemArray[0])))
                                    {
                                        string tmpInfo = string.Format(ResStrings.str_Shelf,
                                            _localDeviceArray[_selectedReader].currentInventory.ListTagWithChannel[
                                                dr.ItemArray[0]]);
                                        rowToadd[dr.ItemArray.Length + 1] = tmpInfo;
                                    }
                                }
                                if (!string.IsNullOrEmpty(_columnExpiredDate))
                                {
                                    string date = (string) dr.ItemArray[_rowDlcDate - 1];
                                    DateTime dt;
                                    if (DateTime.TryParseExact(date, _formats, CultureInfo.CurrentUICulture,
                                        DateTimeStyles.None, out dt))
                                    {
                                        DateTime now = DateTime.Now;
                                        TimeSpan elapsed = dt.Subtract(now);
                                        double daysAgo = elapsed.TotalDays;

                                        if (daysAgo < 0)
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Expired;
                                        else if ((daysAgo >= 0) & (daysAgo < 30))
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Less_than_30_Days_Left;
                                        else if ((daysAgo >= 31) & (daysAgo < 90))
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Less_than_90_Days_Left;
                                        else if (daysAgo >= 91)
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_More_than_90_Days_Left;

                                    }
                                    else
                                    {
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_No_Date;
                                    }
                                }

                                newDt.Rows.Add(rowToadd);
                            }
                            foreach (DataRow dr in _localDeviceArray[_selectedReader].currentInventory.dtTagPresent.Rows
                                )
                            {

                                DataRow rowToadd = newDt.NewRow();
                                rowToadd[0] = ResStrings.str_Present;
                                for (int i = 0; i < dr.ItemArray.Length; i++)
                                    rowToadd[i + 1] = dr.ItemArray[i];
                                if ((_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SFR) ||
                                    (_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SBF))
                                {
                                    if ((_localDeviceArray[_selectedReader].currentInventory.ListTagWithChannel != null) &&
                                        (_localDeviceArray[_selectedReader].currentInventory.ListTagWithChannel.Contains
                                            (dr.ItemArray[0])))
                                    {
                                        string tmpInfo = string.Format(ResStrings.str_Shelf,
                                            _localDeviceArray[_selectedReader].currentInventory.ListTagWithChannel[
                                                dr.ItemArray[0]]);
                                        rowToadd[dr.ItemArray.Length + 1] = tmpInfo;
                                    }
                                }

                                if (!string.IsNullOrEmpty(_columnExpiredDate))
                                {
                                    string date = (string) dr.ItemArray[_rowDlcDate - 1];
                                    DateTime dt;
                                    if (DateTime.TryParseExact(date, _formats, CultureInfo.CurrentUICulture,
                                        DateTimeStyles.None, out dt))
                                    {
                                        DateTime now = DateTime.Now;
                                        TimeSpan elapsed = dt.Subtract(now);
                                        double daysAgo = elapsed.TotalDays;

                                        if (daysAgo < 0)
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Expired;
                                        else if ((daysAgo >= 0) & (daysAgo < 30))
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Less_than_30_Days_Left;
                                        else if ((daysAgo >= 31) & (daysAgo < 90))
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Less_than_90_Days_Left;
                                        else if (daysAgo >= 91)
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_More_than_90_Days_Left;

                                    }
                                    else
                                    {
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_No_Date;
                                    }
                                }
                                newDt.Rows.Add(rowToadd);
                            }

                            dataListView.DataSource = null;
                            dataListView.DataSource = newDt;

                        }

                        Invoke((MethodInvoker) delegate
                        {
                            labelPresent.Text = string.Format(ResStrings.str_PRESENT_format,
                                _localDeviceArray[_selectedReader].currentInventory.dtTagPresent.Rows.Count);
                            pictureBoxPresent.Refresh();
                        });
                        Invoke((MethodInvoker) delegate
                        {
                            labelAdded.Text = string.Format(ResStrings.str_ADDED_format,
                                _localDeviceArray[_selectedReader].currentInventory.dtTagAdded.Rows.Count);
                            pictureBoxAdded.Refresh();
                        });
                        Invoke((MethodInvoker) delegate
                        {
                            labelRemoved.Text = string.Format(ResStrings.str_REMOVED_format,
                                _localDeviceArray[_selectedReader].currentInventory.dtTagRemove.Rows.Count);
                            pictureBoxRemoved.Refresh();
                        });
                        labelInventoryTagCount.Invoke(
                            (MethodInvoker)
                                delegate
                                {
                                    labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags,
                                        _localDeviceArray[_selectedReader].currentInventory.dtTagAll.Rows.Count);
                                });
                        for (int i = 0; i < dataListView.Columns.Count; i++)
                        {
                            OLVColumn ol = dataListView.GetColumn(i);
                            ol.Width = 25;
                            ol.FillsFreeSpace = false;
                            ol.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                            if (ol.Index == dataListView.Columns.Count - 1)
                                ol.FillsFreeSpace = true;
                        }

                    }
                    else
                    {
                        if (_networkDeviceArray == null) return;
                        lock (_locker)
                        {
                            bool isMailNeeded = false;
                            dataListView.Items.Clear();
                            //put column
                            bool needUpdate = false;




                            OLVColumn aColumn = dataListView.AllColumns[dataListView.AllColumns.Count - 2];
                            bool previousState = aColumn.IsVisible;
                            aColumn.IsVisible = false;
                            if ((_networkDeviceArray[_selectedReader - _nbLocalDevice].infoDev.deviceType ==
                                 DeviceType.DT_SFR) ||
                                (_networkDeviceArray[_selectedReader - _nbLocalDevice].infoDev.deviceType ==
                                 DeviceType.DT_SBF))
                                aColumn.IsVisible = true;

                            if (aColumn.IsVisible != previousState) needUpdate = true;

                            OLVColumn bColumn = dataListView.AllColumns[dataListView.AllColumns.Count - 1];
                            previousState = bColumn.IsVisible;
                            if (!string.IsNullOrEmpty(_columnExpiredDate))
                                bColumn.IsVisible = true;
                            else
                                bColumn.IsVisible = false;
                            if (bColumn.IsVisible != previousState) needUpdate = true;
                            if (needUpdate) dataListView.RebuildColumns();


                            DataTable newDt = new DataTable();
                            newDt.Columns.Add(ResStrings.str_Event, typeof (string));

                            for (int i = 0; i < _columnInfo.Count; i++)
                            {
                                newDt.Columns.Add(_columnInfo[i].ToString(), typeof (string));
                            }

                            newDt.Columns.Add(ResStrings.str_Tag_Location, typeof (string));
                            newDt.Columns.Add(ResStrings.str_Expiration, typeof (string));


                            foreach (
                                DataRow dr in
                                    _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.dtTagAdded
                                        .Rows)
                            {
                                DataRow rowToadd = newDt.NewRow();

                                rowToadd[0] = ResStrings.str_Added;
                                for (int i = 0; i < dr.ItemArray.Length; i++)
                                    rowToadd[i + 1] = dr.ItemArray[i];
                                if ((_networkDeviceArray[_selectedReader - _nbLocalDevice].infoDev.deviceType ==
                                     DeviceType.DT_SFR) ||
                                    (_networkDeviceArray[_selectedReader - _nbLocalDevice].infoDev.deviceType ==
                                     DeviceType.DT_SBF))
                                    if (
                                        (_networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory
                                            .ListTagWithChannel != null) &&
                                        (_networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory
                                            .ListTagWithChannel.Contains(dr.ItemArray[0])))
                                    {
                                        string tmpInfo = string.Format(ResStrings.str_Shelf,
                                            _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory
                                                .ListTagWithChannel[dr.ItemArray[0]]);
                                        rowToadd[dr.ItemArray.Length + 1] = tmpInfo;
                                    }
                                if (!string.IsNullOrEmpty(_columnExpiredDate))
                                {
                                    string date = (string) dr.ItemArray[_rowDlcDate - 1];
                                    DateTime dt;
                                    if (DateTime.TryParseExact(date, _formats, CultureInfo.CurrentUICulture,
                                        DateTimeStyles.None, out dt))
                                    {
                                        DateTime now = DateTime.Now;
                                        TimeSpan elapsed = dt.Subtract(now);
                                        double daysAgo = elapsed.TotalDays;

                                        if (daysAgo < 0)
                                        {
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Expired;
                                            isMailNeeded = true;
                                        }
                                        else if ((daysAgo >= 0) & (daysAgo < 30))
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_30_Days_Left;
                                        else if ((daysAgo >= 31) & (daysAgo < 90))
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_90_Days_Left;
                                        else if (daysAgo >= 91)
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_p90_Days_Left;

                                    }
                                    else
                                    {
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_No_Date;
                                    }
                                }
                                newDt.Rows.Add(rowToadd);
                            }

                            foreach (
                                DataRow dr in
                                    _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.dtTagRemove
                                        .Rows)
                            {
                                DataRow rowToadd = newDt.NewRow();

                                rowToadd[0] = ResStrings.str_Removed;
                                for (int i = 0; i < dr.ItemArray.Length; i++)
                                    rowToadd[i + 1] = dr.ItemArray[i];
                                if ((_networkDeviceArray[_selectedReader - _nbLocalDevice].infoDev.deviceType ==
                                     DeviceType.DT_SFR) ||
                                    (_networkDeviceArray[_selectedReader - _nbLocalDevice].infoDev.deviceType ==
                                     DeviceType.DT_SBF))
                                    if (
                                        _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory
                                            .ListTagWithChannel.Contains(dr.ItemArray[0]))
                                    {
                                        string tmpInfo = string.Format(ResStrings.str_Shelf,
                                            _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory
                                                .ListTagWithChannel[dr.ItemArray[0]]);
                                        rowToadd[dr.ItemArray.Length + 1] = tmpInfo;
                                    }
                                if (!string.IsNullOrEmpty(_columnExpiredDate))
                                {
                                    string date = (string) dr.ItemArray[_rowDlcDate - 1];
                                    DateTime dt;
                                    if (DateTime.TryParseExact(date, _formats, CultureInfo.CurrentUICulture,
                                        DateTimeStyles.None, out dt))
                                    {
                                        DateTime now = DateTime.Now;
                                        TimeSpan elapsed = dt.Subtract(now);
                                        double daysAgo = elapsed.TotalDays;

                                        if (daysAgo < 0)
                                        {
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Expired;
                                            isMailNeeded = true;
                                        }
                                        else if ((daysAgo >= 0) & (daysAgo < 30))
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Less_than_30_Days_Left;
                                        else if ((daysAgo >= 31) & (daysAgo < 90))
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Less_than_90_Days_Left;
                                        else if (daysAgo >= 91)
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_More_than_90_Days_Left;

                                    }
                                    else
                                    {
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_No_Date;
                                    }
                                }
                                newDt.Rows.Add(rowToadd);

                            }
                            foreach (
                                DataRow dr in
                                    _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.dtTagPresent
                                        .Rows)
                            {
                                DataRow rowToadd = newDt.NewRow();

                                rowToadd[0] = ResStrings.str_Present;
                                for (int i = 0; i < dr.ItemArray.Length; i++)
                                    rowToadd[i + 1] = dr.ItemArray[i];
                                if ((_networkDeviceArray[_selectedReader - _nbLocalDevice].infoDev.deviceType ==
                                     DeviceType.DT_SFR) ||
                                    (_networkDeviceArray[_selectedReader - _nbLocalDevice].infoDev.deviceType ==
                                     DeviceType.DT_SBF))
                                    if (
                                        (_networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory
                                            .ListTagWithChannel != null) &&
                                        (_networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory
                                            .ListTagWithChannel.Contains(dr.ItemArray[0])))
                                    {
                                        string tmpInfo = string.Format(ResStrings.str_Shelf,
                                            _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory
                                                .ListTagWithChannel[dr.ItemArray[0]]);
                                        rowToadd[dr.ItemArray.Length + 1] = tmpInfo;
                                    }
                                if (!string.IsNullOrEmpty(_columnExpiredDate))
                                {
                                    string date = (string) dr.ItemArray[_rowDlcDate - 1];
                                    DateTime dt;
                                    if (DateTime.TryParseExact(date, _formats, CultureInfo.CurrentUICulture,
                                        DateTimeStyles.None, out dt))
                                    {
                                        DateTime now = DateTime.Now;
                                        TimeSpan elapsed = dt.Subtract(now);
                                        double daysAgo = elapsed.TotalDays;

                                        if (daysAgo < 0)
                                        {
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Expired;
                                            isMailNeeded = true;
                                        }
                                        else if ((daysAgo >= 0) & (daysAgo < 30))
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Less_than_30_Days_Left;
                                        else if ((daysAgo >= 31) & (daysAgo < 90))
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_Less_than_90_Days_Left;
                                        else if (daysAgo >= 91)
                                            rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_More_than_90_Days_Left;

                                    }
                                    else
                                    {
                                        rowToadd[dr.ItemArray.Length + 2] = ResStrings.str_No_Date;
                                    }
                                }
                                newDt.Rows.Add(rowToadd);
                            }

                            if (isMailNeeded)
                            {
                                _db.storeAlert(AlertType.AT_DLC_Expired, _networkDeviceArray[_selectedReader - _nbLocalDevice].infoDev, null, null);
                                AlertMgtClass.treatAlert(AlertType.AT_DLC_Expired, _networkDeviceArray[_selectedReader - _nbLocalDevice].infoDev, null,
                                    _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory, null, BShowAlert);
                            }

                            dataListView.DataSource = null;
                            dataListView.DataSource = newDt;

                        }


                        Invoke((MethodInvoker) delegate
                        {
                            labelPresent.Text = string.Format(ResStrings.str_PRESENT_format,
                                _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.dtTagPresent.Rows
                                    .Count);
                            pictureBoxPresent.Refresh();
                        });
                        Invoke((MethodInvoker) delegate
                        {
                            labelAdded.Text = string.Format(ResStrings.str_ADDED_format,
                                _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.dtTagAdded.Rows
                                    .Count);
                            pictureBoxAdded.Refresh();
                        });
                        Invoke((MethodInvoker) delegate
                        {
                            labelRemoved.Text = string.Format(ResStrings.str_REMOVED_format,
                                _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.dtTagRemove.Rows
                                    .Count);
                            pictureBoxRemoved.Refresh();
                        });
                        labelInventoryTagCount.Invoke(
                            (MethodInvoker)
                                delegate
                                {
                                    labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags,
                                        _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.dtTagAll
                                            .Rows.Count);
                                });
                        for (int i = 0; i < dataListView.Columns.Count; i++)
                        {
                            OLVColumn ol = dataListView.GetColumn(i);
                            ol.Width = 25;
                            ol.FillsFreeSpace = false;
                            ol.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                            if (ol.Index == dataListView.Columns.Count - 1)
                                ol.FillsFreeSpace = true;
                        }

                    }

                    if (!_bWasInAccumulation)
                    {
                        if ((_bti != null) && (!string.IsNullOrEmpty(_bti.Criteria)) && (_nbTagBox == 1))
                            CheckCriteria();
                    }

                }
                catch (Exception exp)
                {
                    ExceptionMessageBox.Show(exp);
                }
                Cursor.Current = Cursors.Default;
            });
        }
        private void dataListView_AfterCreatingGroups(object sender, CreateGroupsEventArgs e)
        {
            if ((dataListView == null) || (dataListView.OLVGroups == null)) return;
            //OLVGroup grp = dataListView.OLVGroups[0];
            //string headerValue = grp.Header;
            //if ((grp.Header.Contains("Present") || (grp.Header.Contains("Added")) || (grp.Header.Contains("Removed"))))
          
            if (e.Parameters.GroupByColumn.AspectName.Equals(ResStrings.str_Event))
            {
                foreach (ListViewItem lvi in dataListView.Items)
                {
                    string strCase = lvi.SubItems[0].Text;
                    if (strCase.Equals(ResStrings.str_Present)) lvi.ForeColor = Color.Blue;
                    else if (strCase.Equals(ResStrings.str_Added)) lvi.ForeColor = Color.Green;
                    else if (strCase.Equals(ResStrings.str_Removed)) lvi.ForeColor = Color.Red;
                    else lvi.ForeColor = Color.Black; 
                }                
            }
            else if (e.Parameters.GroupByColumn.AspectName.Equals(ResStrings.str_Expiration))
            {
                foreach (ListViewItem lvi in dataListView.Items)
                {
                    string strCase = lvi.SubItems[lvi.SubItems.Count - 1].Text;
                    
                    if (strCase.Equals(ResStrings.str_Less_than_30_Days_Left))  lvi.ForeColor = Color.OrangeRed; 
                    else if (strCase.Equals(ResStrings.str_Less_than_90_Days_Left)) lvi.ForeColor = Color.Blue; 
                    else if  (strCase.Equals(ResStrings.str_More_than_90_Days_Left)) lvi.ForeColor = Color.Green; 
                    else if (strCase.Equals(ResStrings.str_Expired))  lvi.ForeColor = Color.Red; 
                    else lvi.ForeColor = Color.Black; 
                }
            }


            foreach (OLVGroup grp2 in dataListView.OLVGroups)
            {
                grp2.Collapsed = true;
             
            }
            dataListView.Refresh();
            
        }
        private void dataListView_BeforeCreatingGroups(object sender, CreateGroupsEventArgs e)
        {
            
            OLVColumn olToSort = dataListView.GetColumn(1);
            e.Parameters.SortItemsByPrimaryColumn = false;
            e.Parameters.PrimarySort = olToSort;
            e.Parameters.PrimarySortOrder = SortOrder.Ascending;

            if (e.Parameters.GroupByColumn.AspectName.Equals(ResStrings.str_Event))
            {
                dataListView.GetColumn(ResStrings.str_Event).MakeGroupies(
                    new object[] { ResStrings.str_Added, ResStrings.str_Present, ResStrings.str_Removed },
                    new[] { "Rien", ResStrings.str_Added, ResStrings.str_Present, ResStrings.str_Removed },
                    //new[] { "not", "add", "present", "remove" },
                    new object[] { "not", "ban_added", "ban_present", "ban_removed" },
                    new[] { "", GetSumValue(2), GetSumValue(1), GetSumValue(3) },
                    new[] { "Rien", ResStrings.str_Tag_s__added_at_last_scan, ResStrings.Livstr_Tag_s__already_present_in_previous_scan, ResStrings.str_Tag_s__removed_from_previous_scan }
                );
            }
           

            else if (!string.IsNullOrEmpty(_columnExpiredDate))
            {
                if (e.Parameters.GroupByColumn.AspectName.Equals(ResStrings.str_Expiration))
                {
                    dataListView.GetColumn(ResStrings.str_Expiration).MakeGroupies(
                        new[] { ResStrings.str_Expired, ResStrings.str_Less_than_30_Days_Left, ResStrings.str_Less_than_90_Days_Left, ResStrings.str_More_than_90_Days_Left, ResStrings.str_No_Date },
                        new[] { "Rien", ResStrings.str_Expired, ResStrings.str_Less_than_30_Days_Left, ResStrings.str_Between_30_to_90_Days_Left, ResStrings.str_More_than_90_Days_Left, ResStrings.str_No_Date },
                        new object[] { "not", "expired", "attention-1mois", "attention-3mois", "attention", "not" }
                        );
                }
             }
            else
            {
                string columnToGroup = e.Parameters.GroupByColumn.AspectName;
                Dictionary<string, int> col = new Dictionary<string, int>();
                DataTable gpDt = (DataTable)dataListView.DataSource;

                foreach (DataRow dr in gpDt.Rows)
                {
                    string grpName = dr[columnToGroup].ToString();
                    if (col.ContainsKey(grpName))
                        col[grpName]++;
                    else
                        col.Add(grpName, 1);
                }

                string[] group = new string[col.Count];
                string[] groupName = new string[col.Count + 1];
                object[] groupImage = new object[col.Count + 1];
                string[] groupSubTitle = new string[col.Count + 1];

                int nIndex = 0;
                groupName[0] = "Rien";
                groupImage[0] = "not";
                groupSubTitle[0] = "";
                // Use Linq to sort list as group need to be form in ascending mode to perform well sorting.
                var items = from pair in col
                            orderby pair.Key ascending
                            select pair;
                foreach (KeyValuePair<string, int> pair in items)
                {
                    group[nIndex] = pair.Key;
                    groupName[nIndex + 1] = pair.Key;
                    groupSubTitle[nIndex + 1] = GetSumValue(gpDt, columnToGroup, pair.Key);
                    groupImage[nIndex + 1] = "present";
                    nIndex++;
                }
                dataListView.GetColumn(columnToGroup).MakeGroupies(group, groupName, groupImage, groupSubTitle);
            }
        }
        private void dataListView_HotItemChanged(object sender, HotItemChangedEventArgs e)
        {

            if ((dataListView == null) || (dataListView.OLVGroups == null)) return;
            OLVGroup grp = dataListView.OLVGroups[0];
           
            if ((grp.Header.Contains(ResStrings.str_Present) || (grp.Header.Contains(ResStrings.str_Added)) || (grp.Header.Contains(ResStrings.str_Removed))))
            {
                foreach (ListViewItem lvi in dataListView.Items)
                {
                    string strCase = lvi.SubItems[0].Text;
                    if (strCase.Equals(ResStrings.str_Present)) lvi.ForeColor = Color.Blue;
                    else if (strCase.Equals(ResStrings.str_Added)) lvi.ForeColor = Color.Green;
                    else if (strCase.Equals(ResStrings.str_Removed)) lvi.ForeColor = Color.Red;
                    else lvi.ForeColor = Color.Black; 
                }
            }
            else
            {
                foreach (ListViewItem lvi in dataListView.Items)
                {
                    string strCase = lvi.SubItems[lvi.SubItems.Count - 1].Text;

                    if (strCase.Equals(ResStrings.str_Less_than_30_Days_Left)) lvi.ForeColor = Color.OrangeRed;
                    else if (strCase.Equals(ResStrings.str_Less_than_90_Days_Left)) lvi.ForeColor = Color.Blue;
                    else if (strCase.Equals(ResStrings.str_More_than_90_Days_Left)) lvi.ForeColor = Color.Green;
                    else if (strCase.Equals(ResStrings.str_Expired)) lvi.ForeColor = Color.Red;
                    else lvi.ForeColor = Color.Black; 
                }
            }          
            
        }
        private void dataListView_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            if (dataListView.SelectedItems.Count > 0)
            {
                ListViewItem item = dataListView.SelectedItems[0];
                string tagClicked = item.SubItems[1].Text;

                if (!string.IsNullOrEmpty(tagClicked))
                {
                    ItemHistoryForm ihf = new ItemHistoryForm(tagClicked, null);
                    ihf.MdiParent = ParentForm;
                    ihf.WindowState = FormWindowState.Maximized;
                    ihf.Show();
                }
            }
        }
        private void toolStripDropDownButtonView_Click(object sender, EventArgs e)
        {
            dataListView.TileSize = new Size(250, 50 + (_columnInfo.Count * 12));
            if (toolStripDropDownButtonView.Text == ResStrings.str_Display_in_Tile)
            {
                toolStripDropDownButtonView.Text = ResStrings.str_Display_in_Details;
                dataListView.View = View.Tile;
            }
            else
            {
                toolStripDropDownButtonView.Text = ResStrings.str_Display_in_Tile;
                dataListView.View = View.Details;
            }
        }
        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataListView.OLVGroups.Count; i++)
            {
                OLVGroup grp = dataListView.OLVGroups[i];
                grp.Collapsed = true;

            }
        }
        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            dataListView.ShowGroups = !dataListView.ShowGroups;
            toolStripSplitButton1.Text = (dataListView.ShowGroups) ? ResStrings.str_Group_Mode___OFF : ResStrings.str_Group_Mode__ON;
            timerObjectList.Start();
        }      
        private void dataListView_SelectionChanged(object sender, EventArgs e)
        {
            string menuTxt = ResStrings.str_Turn_LEDs_On;
            string menuTxtError = ResStrings.str_Turn_LEDs_On_Tag_s__Not_Compatible;
            bool bSelectionOk = true;

           /* foreach (ListViewItem currentItem in dataListView.SelectedItems)
            {
                OLVColumn olc = dataListView.GetColumn(_columnInfo[0].ToString());
                string uid = currentItem.SubItems[olc.Index].Text;


                if ((uid.Length == 10) && ( uid.StartsWith("30")) && (_myRegexUid.IsMatch(uid))) 
                {
                    bSelectionOk = false;
                    break;
                }
            }

            if (bSelectionOk)
            {
                ledONToolStripMenuItem.Text = menuTxt;
                ledONToolStripMenuItem.Enabled = true;
                updateTagUIDToolStripMenuItem.Enabled = true;
                updateTagUIDToolStripMenuItem.Text = ResStrings.str_Update_Tag_UID;
            }
            else
            {
                ledONToolStripMenuItem.Text = menuTxtError;
                ledONToolStripMenuItem.Enabled = false;
                updateTagUIDToolStripMenuItem.Enabled = false;
                updateTagUIDToolStripMenuItem.Text = ResStrings.str_Update_Tag_UID_Tag_s__Not_Compatible_;
            }*/

            ledONToolStripMenuItem.Text = menuTxt;
            ledONToolStripMenuItem.Enabled = true;
            updateTagUIDToolStripMenuItem.Enabled = true;
            updateTagUIDToolStripMenuItem.Text = ResStrings.str_Update_Tag_UID;
        }      

        #endregion
        #region Device
        private void CreateNetworkDevice()
        {
            try
            {
                if (_bClosing) return;
                DeviceInfo[] tmpDeviceArray = _db.RecoverDevice(false);
                if (tmpDeviceArray != null)
                {
                    _networkDeviceArray = new deviceClass[tmpDeviceArray.Length];
                    ListTempBottle = new ArrayList[tmpDeviceArray.Length];
                    for (int nIndex = 0; nIndex < tmpDeviceArray.Length; nIndex++)
                    {
                        ListTempBottle[nIndex] = new ArrayList(MaxTemp);
                        _networkDeviceArray[nIndex] = new deviceClass(_columnInfo);
                        _networkDeviceArray[nIndex].infoDev = new DeviceInfo();
                        _networkDeviceArray[nIndex].infoDev = tmpDeviceArray[nIndex];

                        //for test Xu4
                        if (_networkDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_MSR)
                            _networkDeviceArray[nIndex].infoDev.deviceType = DeviceType.DT_SBF;
                       
                        InventoryData tmpInv = _db.GetLastScan(_networkDeviceArray[nIndex].infoDev.SerialRFID);
                        if (tmpInv != null)
                        {
                            _networkDeviceArray[nIndex].currentInventory = tmpInv;
                            _networkDeviceArray[nIndex].lastProcessInventoryGmtDate = truncateMs(tmpInv.eventDate.ToUniversalTime());                         
                            
                        }

                        if (!DeviceClientTcp.ContainsKey(_networkDeviceArray[nIndex].infoDev.IP_Server))
                            DeviceClientTcp.Add(_networkDeviceArray[nIndex].infoDev.IP_Server,new TcpIpClient());

                    }
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ExceptionMessageBox.Show(exp);
            }
        }
        private void CreateLocalDevice()
        {
            try
            {
                if (_bClosing) return;
                DeviceInfo[] tmpDeviceArray = _db.RecoverDevice(true);
                if (tmpDeviceArray != null)
                {
                    _localDeviceArray = new deviceClass[tmpDeviceArray.Length];
                    _nbLocalDevice = _localDeviceArray.Length;

                    for (int nIndex = 0; nIndex < tmpDeviceArray.Length; nIndex++)
                    {
                        _localDeviceArray[nIndex] = new deviceClass(_columnInfo);
                        _localDeviceArray[nIndex].infoDev = new DeviceInfo();
                        _localDeviceArray[nIndex].infoDev = tmpDeviceArray[nIndex];
                        _localDeviceArray[nIndex].rfidDev = new RFID_Device();
                        _localDeviceArray[nIndex].rfidDev.TimeDoorOpenTooLong = _maxTimeOpenDoor;
                        _localDeviceArray[nIndex].rfidDev.InterruptScanWithFP = _interruptScanWithFp;                       

                        InventoryData tmpInv = _db.GetLastScan(_localDeviceArray[nIndex].infoDev.SerialRFID);
                        if (tmpInv != null)
                        {
                            _localDeviceArray[nIndex].currentInventory = tmpInv;
                            _localDeviceArray[nIndex].lastProcessInventoryGmtDate = truncateMs(tmpInv.eventDate.ToUniversalTime());
                        }
                    }
                }
                _tcpIpServer.LocalDeviceArray = _localDeviceArray;                
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ExceptionMessageBox.Show(exp);
            }
        }       

        private void rfidDev_NotifyRFIDEvent(object sender, rfidReaderArgs args)
        {
            try
            {
            if (_bClosing) return;
                switch (args.RN_Value)
                {
                    #region Disconnect/FailedToDisconnect
                    case rfidReaderArgs.ReaderNotify.RN_Disconnected:
                    case rfidReaderArgs.ReaderNotify.RN_FailedToConnect:

                        bool bOneConnected = false;
                        if (_localDeviceArray != null)
                        {
                            foreach (deviceClass dc in _localDeviceArray)
                            {
                                if (dc.rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected)
                                    bOneConnected = true;
                            }
                        }
                        if (_networkDeviceArray != null)
                        {
                            foreach (deviceClass dc in _networkDeviceArray)
                            {
                                if (dc.netConnectionStatus == ConnectionStatus.CS_Connected)
                                    bOneConnected = true;
                            }
                        }

                        if (!bOneConnected)
                        {
                            Invoke((MethodInvoker)delegate { toolStripButtonScan.Enabled = false; });
                            Invoke((MethodInvoker)delegate { toolStripButtonStopScan.Enabled = false; });
                        }
                        else
                        {
                            Invoke((MethodInvoker)delegate { toolStripButtonScan.Enabled = true; });
                            Invoke((MethodInvoker)delegate { toolStripButtonStopScan.Enabled = true; });
                        }
                        if (_localDeviceArray != null)
                        {
                            for (int nIndex = 0; nIndex < _localDeviceArray.Length; nIndex++)
                            {
                                if (_localDeviceArray[nIndex].infoDev.SerialRFID.Equals(args.SerialNumber))
                                {
                                    int index = nIndex;
                                    Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].ImageIndex = treeViewDevice.Nodes[index].SelectedImageIndex = 2; });
                                    Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[2].Text = string.Format(ResStrings.str_Connection_Status_, getEnumDesc.GetEnumDescription(_localDeviceArray[index].rfidDev.ConnectionStatus)); });
                                    Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[3].Text = string.Format(ResStrings.str_Statut_, getEnumDesc.GetEnumDescription(_localDeviceArray[index].rfidDev.DeviceStatus)); });
                                    Invoke((MethodInvoker)delegate { treeViewDevice.Refresh(); });
                                }
                            }
                        }

                        Invoke((MethodInvoker)delegate { timerStartup.Interval = 10000; });
                        Invoke((MethodInvoker)delegate { timerStartup.Enabled = true; });


                        break;
                    #endregion
                    #region Connect
                    case rfidReaderArgs.ReaderNotify.RN_Connected:

                        Invoke((MethodInvoker)delegate { toolStripButtonScan.Enabled = true; });
                        Invoke((MethodInvoker)delegate { toolStripButtonStopScan.Enabled = true; });
                        for (int nIndex = 0; nIndex < _localDeviceArray.Length; nIndex++)
                        {
                            if (_localDeviceArray[nIndex].infoDev.SerialRFID.Equals(args.SerialNumber))
                            {
                                int index = nIndex;
                                Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].ImageIndex = treeViewDevice.Nodes[index].SelectedImageIndex = 1; });
                                Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[2].Text = string.Format(ResStrings.str_Connection_Status_, getEnumDesc.GetEnumDescription(_localDeviceArray[index].rfidDev.ConnectionStatus)); });
                                Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[3].Text = string.Format(ResStrings.str_Statut_, getEnumDesc.GetEnumDescription(_localDeviceArray[index].rfidDev.DeviceStatus)); });
                                Invoke((MethodInvoker)delegate { treeViewDevice.Refresh(); });

                                switch (_localDeviceArray[nIndex].infoDev.deviceType)
                                {
                                    case DeviceType.DT_PAD:
                                    case DeviceType.DT_SBR:
                                    case DeviceType.DT_STR:
                                        break;
                                    default: Invoke((MethodInvoker)delegate { toolStripButtonScan_Click(null, null); });
                                        break;
                                }

                            }
                        }

                        if (bUseCvsExchange) Invoke((MethodInvoker)delegate { toolStripButtonScan_Click(null, null); });

                        break;
                    #endregion
                    #region TagPresence
                    case rfidReaderArgs.ReaderNotify.RN_TagPresenceDetected:

                        for (int nIndex = 0; nIndex < _localDeviceArray.Length; nIndex++)
                        {
                            if (_localDeviceArray[nIndex].infoDev.SerialRFID.Equals(args.SerialNumber))
                            {
                                if (_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_SBR)
                                {
                                    _localDeviceArray[nIndex].bComeFromKZ = !_checkedAccumulate;
                                    _localDeviceArray[nIndex].rfidDev.DisableWaitMode();

                                    _bFirstScanAccumulate = !_bKeepLastScan;
                                    _bUserScan = false;
                                    _firstName = ResStrings.str_Manual;
                                    _lastName = ResStrings.str_Scan;
                                    _lastAccessType = AccessType.AT_NONE;
                                    _bAccumulateSbr = true;
                                    if ((_localDeviceArray[nIndex].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                                     (_localDeviceArray[nIndex].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
                                    {

                                        Invoke((MethodInvoker)delegate { toolStripButtonScan.Enabled = false; });
                                        Invoke((MethodInvoker)delegate { toolStripButtonStopScan.Enabled = true; });
                                        _localDeviceArray[nIndex].rfidDev.ScanDevice(false);

                                    }
                                    else
                                    {
                                        if (_localDeviceArray[nIndex].infoDev.enabled == 1)
                                            MessageBox.Show(ResStrings.StrDeviceNotReadyOrNotConnected, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        else
                                            MessageBox.Show(ResStrings.str_Device_disabled, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                            }
                        }

                        break;
                    #endregion
                    #region ScanStarted
                    case rfidReaderArgs.ReaderNotify.RN_ScanStarted:


                        if (_tcpIpServer.requestScanFromServer)
                        {
                            _bFirstScanAccumulate = true;
                            _bUserScan = false;
                            _firstName = ResStrings.str_Manual;
                            _lastName = ResStrings.str_Scan;
                            _lastAccessType = AccessType.AT_NONE;
                            _lastDoorUser = DoorInfo.DI_NO_DOOR;
                            LastBadge = null;
                        }

                        if ((string.IsNullOrEmpty(_firstName)) || (string.IsNullOrEmpty(_lastName)))
                        {
                            _firstName = ResStrings.str_Manual;
                            _lastName = ResStrings.str_Scan;
                            _lastAccessType = AccessType.AT_NONE;
                            _lastDoorUser = DoorInfo.DI_NO_DOOR;
                            LastBadge = null;
                        }
                        _nbTagBox = 0;
                        _bti = null;
                        if (_localDeviceArray != null)
                            for (int nIndex = 0; nIndex < _localDeviceArray.Length; nIndex++)
                            {
                                if ((_localDeviceArray[nIndex].infoDev != null) && (_localDeviceArray[nIndex].infoDev.SerialRFID.Equals(args.SerialNumber)))
                                {
                                    _localDeviceArray[nIndex].inventorySequence++;
                                    _localDeviceArray[nIndex].cptOut--;
                                    _localDeviceArray[nIndex].bDataCompleted = false;

                                    if (treeViewDevice.InvokeRequired)
                                    {
                                        int index = nIndex;
                                        Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].SelectedImageIndex = 3; });
                                        Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[2].Text = string.Format(ResStrings.str_Connection_Status_, getEnumDesc.GetEnumDescription(_localDeviceArray[index].rfidDev.ConnectionStatus)); });
                                        Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[3].Text = string.Format(ResStrings.str_Statut_, getEnumDesc.GetEnumDescription(_localDeviceArray[index].rfidDev.DeviceStatus)); });
                                        Invoke((MethodInvoker)delegate { treeViewDevice.Refresh(); });
                                    }

                                    if (_localDeviceArray[nIndex].infoDev.deviceType != DeviceType.DT_SBR)
                                    {
                                        _bFirstScanAccumulate = false;

                                        if (!_bKeepLastScan)
                                        {
                                            InventoryData lastScan = _db.GetLastScan(_localDeviceArray[nIndex].infoDev.SerialRFID);
                                            if (lastScan != null)
                                                _localDeviceArray[nIndex].previousInventory = lastScan;
                                            else
                                                _localDeviceArray[nIndex].previousInventory = _localDeviceArray[nIndex].currentInventory;
                                        }
                                        else
                                            _localDeviceArray[nIndex].previousInventory = _localDeviceArray[nIndex].currentInventory;

                                        _localDeviceArray[nIndex].currentInventory = new InventoryData(_columnInfo);
                                        _localDeviceArray[nIndex].currentInventory.bUserScan = _bUserScan;
                                        _localDeviceArray[nIndex].currentInventory.userFirstName = _firstName;
                                        _localDeviceArray[nIndex].currentInventory.userLastName = _lastName;
                                        _localDeviceArray[nIndex].currentInventory.BadgeID = LastBadge;
                                        _localDeviceArray[nIndex].currentInventory.userDoor = _lastDoorUser;
                                        _localDeviceArray[nIndex].currentInventory.accessType = _lastAccessType;
                                        _localDeviceArray[nIndex].currentInventory.serialNumberDevice = _localDeviceArray[nIndex].infoDev.SerialRFID;
                                        _localDeviceArray[nIndex].currentInventory.eventDate = DateTime.UtcNow;



                                        if (nIndex == _selectedReader)
                                        {
                                            int index = nIndex;
                                            labelInventoryDate.Invoke((MethodInvoker)delegate { labelInventoryDate.Text = _localDeviceArray[index].currentInventory.eventDate.ToLocalTime().ToString("G"); });
                                            labelInventoryUser.Invoke((MethodInvoker)delegate {
                                                labelInventoryUser.Text = string.Format("{0} {1}", _localDeviceArray[index].currentInventory.userFirstName, _localDeviceArray[index].currentInventory.userLastName);
                                                if (_localDeviceArray[index].currentInventory.bUserScan)
                                                {
                                                    if (_localDeviceArray[index].currentInventory.userDoor == DoorInfo.DI_SLAVE_DOOR)
                                                        labelInventoryUser.Text += ResStrings.str_SlaveDoor;
                                                    else
                                                        labelInventoryUser.Text += ResStrings.str_MasterDoor;
                                                }
                                            });
                                            listBoxTag.Invoke((MethodInvoker)delegate { listBoxTag.Items.Clear(); });
                                            labelInventoryTagCount.Invoke((MethodInvoker)delegate { labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags, 0); });
                                        }
                                    }
                                    else
                                    {
                                        if (_bFirstScanAccumulate)
                                        {
                                            _bFirstScanAccumulate = false;
                                            if (!_bKeepLastScan)
                                            {
                                                InventoryData lastScan = _db.GetLastScan(_localDeviceArray[nIndex].infoDev.SerialRFID);
                                                if (lastScan != null)
                                                    _localDeviceArray[nIndex].previousInventory = lastScan;
                                                else
                                                    _localDeviceArray[nIndex].previousInventory = _localDeviceArray[nIndex].currentInventory;
                                            }
                                            else
                                                _localDeviceArray[nIndex].previousInventory = _localDeviceArray[nIndex].currentInventory;

                                            _localDeviceArray[nIndex].currentInventory = new InventoryData(_columnInfo);
                                            _localDeviceArray[nIndex].currentInventory.bUserScan = _bUserScan;
                                            _localDeviceArray[nIndex].currentInventory.userFirstName = _firstName;
                                            _localDeviceArray[nIndex].currentInventory.userLastName = _lastName;
                                            _localDeviceArray[nIndex].currentInventory.accessType = _lastAccessType;
                                            _localDeviceArray[nIndex].currentInventory.BadgeID = LastBadge;
                                            _localDeviceArray[nIndex].currentInventory.userDoor = DoorInfo.DI_NO_DOOR;
                                            _localDeviceArray[nIndex].currentInventory.serialNumberDevice = _localDeviceArray[nIndex].infoDev.SerialRFID;
                                            _localDeviceArray[nIndex].currentInventory.eventDate = DateTime.UtcNow;

                                            if (nIndex == _selectedReader)
                                            {
                                                int index = nIndex;
                                                labelInventoryDate.Invoke((MethodInvoker)delegate { labelInventoryDate.Text = _localDeviceArray[index].currentInventory.eventDate.ToLocalTime().ToString("G"); });
                                                labelInventoryUser.Invoke((MethodInvoker)delegate {
                                                    labelInventoryUser.Text = string.Format("{0} {1}", _localDeviceArray[index].currentInventory.userFirstName, _localDeviceArray[index].currentInventory.userLastName);
                                                    if (_localDeviceArray[index].currentInventory.bUserScan)
                                                    {
                                                        if (_localDeviceArray[index].currentInventory.userDoor == DoorInfo.DI_SLAVE_DOOR)
                                                            labelInventoryUser.Text += ResStrings.str_SlaveDoor;
                                                        else
                                                            labelInventoryUser.Text += ResStrings.str_MasterDoor;
                                                    }
                                                });

                                                listBoxTag.Invoke((MethodInvoker)delegate { listBoxTag.Items.Clear(); });
                                                labelInventoryTagCount.Invoke((MethodInvoker)delegate { labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags, 0); });
                                            }
                                        }
                                        else
                                        {

                                            foreach (string uid in _localDeviceArray[nIndex].currentInventory.listTagAdded)
                                            {
                                                if (!_localDeviceArray[nIndex].currentInventory.listTagPresent.Contains(uid))
                                                {
                                                    _localDeviceArray[nIndex].currentInventory.listTagPresent.Add(uid);
                                                    DtAndTagClass dtAndTagPresent = new DtAndTagClass(this, _localDeviceArray[nIndex].currentInventory.dtTagPresent, uid, _db);
                                                    AddTagToDt(dtAndTagPresent);
                                                }
                                            }

                                            _localDeviceArray[nIndex].currentInventory.listTagAdded.Clear();
                                            _localDeviceArray[nIndex].currentInventory.nbTagAdded = 0;
                                            _localDeviceArray[nIndex].currentInventory.dtTagAdded.Rows.Clear();


                                            _localDeviceArray[nIndex].currentInventory.eventDate = DateTime.UtcNow;
                                            int index = nIndex;
                                            labelInventoryDate.Invoke((MethodInvoker)delegate { labelInventoryDate.Text = _localDeviceArray[index].currentInventory.eventDate.ToLocalTime().ToString("G"); });
                                            labelInventoryUser.Invoke((MethodInvoker)delegate {
                                                labelInventoryUser.Text = string.Format("{0} {1}", _localDeviceArray[index].currentInventory.userFirstName, _localDeviceArray[index].currentInventory.userLastName);
                                                if (_localDeviceArray[index].currentInventory.bUserScan)
                                                {
                                                    if (_localDeviceArray[index].currentInventory.userDoor == DoorInfo.DI_SLAVE_DOOR)
                                                        labelInventoryUser.Text += ResStrings.str_SlaveDoor;
                                                    else
                                                        labelInventoryUser.Text += ResStrings.str_MasterDoor;
                                                }
                                            });
                                            labelInventoryTagCount.Invoke((MethodInvoker)delegate { labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags, listBoxTag.Items.Count); });
                                        }
                                    }
                                    #region TcpNotification


                                    if (_bUseTcpNotification)
                                    {
                                        if (_tcpNotificationThreadInfo != null)
                                        {
                                            if (tcpUtils.PingAddress(_tcpNotificationThreadInfo.HostIp, 2000))
                                            {
                                                _tcpNotificationThreadInfo.SetParam(TcpThreadHandle.TcpNotificationType.ScanStarted, _localDeviceArray[nIndex].infoDev.SerialRFID, _tcpIpServer.ServerIP, _tcpIpServer.Port, -1, 0);
                                                _tcpNotificationThreadInfo.ThreadTcpLoop();
                                            }
                                        }

                                    }
                                    #endregion
                                }
                            }

                        if (bUseCvsExchange)
                        {
                            if (File.Exists(pathCsvInScan))
                                File.Delete(pathCsvInScan);
                            CsvPutInScan();

                            if (File.Exists(pathCsvLed))
                                File.Delete(pathCsvLed);
                        }
                        if ((_tcpIpServer != null) && (_tcpIpServer.eventScanStart != null))
                            _tcpIpServer.eventScanStart.Set();

                        break;
                    #endregion
                    #region Scan completed
                    case rfidReaderArgs.ReaderNotify.RN_ScanCompleted:

                        s1 = Stopwatch.StartNew();
                        bool bAllScanFinish = true;
                        for (int nIndex = 0; nIndex < _localDeviceArray.Length; nIndex++)
                        {
                            if (_localDeviceArray[nIndex].infoDev.SerialRFID.Equals(args.SerialNumber))
                            {
                                _localDeviceArray[nIndex].rfidDev.bStopAccessDuringProcessData = true;

                                /* if ((_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_SFR) ||
                                      (_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_SBF))
                                 {
                                     _localDeviceArray[nIndex].currentInventory.ListTagWithChannel = _localDeviceArray[nIndex].myFridgeCabinet.ListTagWithChannel;
                                 }
                                 else*/
                                _localDeviceArray[nIndex].currentInventory.ListTagWithChannel = _localDeviceArray[nIndex].rfidDev.ListTagWithChannel;


                                if (bScanFromTimer)
                                {
                                    if (_localDeviceArray[nIndex].currentInventory.ListTagWithChannel.Count ==
                                        _localDeviceArray[nIndex].previousInventory.listTagAll.Count)
                                    {
                                        bScanFromTimer = false;
                                        Invoke((MethodInvoker)delegate { timerAutoPad.Enabled = true; });
                                        return;
                                    }
                                }
                                if (((_bAccumulateSbr) || (_tcpIpServer.AccumulateMode)) && (_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_SBR))
                                {
                                    Thread.Sleep(100);
                                    if ((_localDeviceArray[nIndex].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                                     (_localDeviceArray[nIndex].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
                                    {

                                        if (!_localDeviceArray[nIndex].bComeFromKZ)
                                        {
                                            if (_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_SBR)
                                                _bAccumulateSbr = _checkedAccumulate || _checkedInOut;
                                            else
                                                _bAccumulateSbr = false;
                                        }
                                        else
                                        {
                                            if (_localDeviceArray[nIndex].cptOut > 0)
                                                _bAccumulateSbr = true;
                                            else
                                                _bAccumulateSbr = false;

                                        }

                                        Invoke((MethodInvoker)delegate { toolStripButtonScan.Enabled = false; });
                                        _localDeviceArray[nIndex].rfidDev.ScanDevice(false);
                                    }
                                }
                                else
                                {
                                    int idscanEvent = -1;
                                    lock (LogStore)
                                    {
                                        // ie if keep last scan -not take into acount removed

                                        // remove remove last scan
                                        if ((_bKeepLastScan) || (_bWasInAccumulation))
                                        {
                                            _localDeviceArray[nIndex].currentInventory.listTagRemoved.Clear();
                                            _localDeviceArray[nIndex].currentInventory.nbTagRemoved = 0;
                                            _localDeviceArray[nIndex].currentInventory.dtTagRemove.Rows.Clear();
                                        }


                                        // test duplicate in list - // should no appear

                                        HashSet<string> hs = new HashSet<string>((string[])_localDeviceArray[nIndex].rfidDev.get_RFID_Device.ReaderData.strListTag.ToArray(typeof(string)));
                                        if (hs.Count != _localDeviceArray[nIndex].rfidDev.get_RFID_Device.ReaderData.strListTag.Count)
                                        {
                                            ExceptionMessageBox.Show(string.Format(ResStrings.str_errorduplicate), ResStrings.strInfo);
                                            return;
                                        }

                                        if ((_bWasInAccumulation) && ((_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_SBR) || (_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_STR)))
                                        {
                                            foreach (string uidSdk in listBoxTag.Items)
                                            {
                                                string escapedTagUid = uidSdk.Replace("'", "''"); // escape ' for DataTable Select() filter
                                                DataRow[] boxInfo = _dtGroup.Select(ResStrings.str_TagUID + "='" + escapedTagUid + "'");

                                                if (boxInfo.Length > 0) continue;

                                                if (!_localDeviceArray[nIndex].currentInventory.listTagAll.Contains(uidSdk))
                                                {
                                                    //Add all
                                                    _localDeviceArray[nIndex].currentInventory.listTagAll.Add(uidSdk);
                                                    DtAndTagClass dtAndTagAll = new DtAndTagClass(this, _localDeviceArray[nIndex].currentInventory.dtTagAll, uidSdk, _db);
                                                    AddTagToDt(dtAndTagAll);

                                                    //tag Present
                                                    if (!_localDeviceArray[nIndex].currentInventory.listTagPresent.Contains(uidSdk))
                                                    {
                                                        _localDeviceArray[nIndex].currentInventory.listTagPresent.Add(uidSdk);
                                                        DtAndTagClass dtAndTagPresent = new DtAndTagClass(this, _localDeviceArray[nIndex].currentInventory.dtTagPresent, uidSdk, _db);
                                                        AddTagToDt(dtAndTagPresent);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {

                                            foreach (string uidSdk in _localDeviceArray[nIndex].rfidDev.get_RFID_Device.ReaderData.strListTag)
                                            {
                                                string escapedTagUid = uidSdk.Replace("'", "''"); // escape ' for DataTable Select() filter
                                                DataRow[] boxInfo = _dtGroup.Select(ResStrings.str_TagUID + "='" + escapedTagUid + "'");

                                                if (boxInfo.Length > 0) continue;

                                                if (!_localDeviceArray[nIndex].currentInventory.listTagAll.Contains(uidSdk))
                                                {
                                                    //Add all
                                                    _localDeviceArray[nIndex].currentInventory.listTagAll.Add(uidSdk);
                                                    DtAndTagClass dtAndTagAll = new DtAndTagClass(this, _localDeviceArray[nIndex].currentInventory.dtTagAll, uidSdk, _db);
                                                    AddTagToDt(dtAndTagAll);

                                                    if (!((_bAccumulateSbr) || (_tcpIpServer.AccumulateMode)))
                                                    {
                                                        if (!_localDeviceArray[nIndex].previousInventory.listTagAll.Contains(uidSdk))
                                                        {
                                                            // Tag Added
                                                            if ((!_localDeviceArray[nIndex].currentInventory.listTagAdded.Contains(uidSdk)) &&
                                                                (!_localDeviceArray[nIndex].currentInventory.listTagPresent.Contains(uidSdk)))
                                                            {
                                                                _localDeviceArray[nIndex].currentInventory.listTagAdded.Add(uidSdk);
                                                                DtAndTagClass dtAndTagAdded = new DtAndTagClass(this, _localDeviceArray[nIndex].currentInventory.dtTagAdded, uidSdk, _db);
                                                                AddTagToDt(dtAndTagAdded);
                                                            }

                                                        }
                                                        else
                                                        {
                                                            //tag Present
                                                            if ((!_localDeviceArray[nIndex].currentInventory.listTagPresent.Contains(uidSdk)) &&
                                                                (!_localDeviceArray[nIndex].currentInventory.listTagAdded.Contains(uidSdk)))
                                                            {
                                                                _localDeviceArray[nIndex].currentInventory.listTagPresent.Add(uidSdk);
                                                                DtAndTagClass dtAndTagPresent = new DtAndTagClass(this, _localDeviceArray[nIndex].currentInventory.dtTagPresent, uidSdk, _db);
                                                                AddTagToDt(dtAndTagPresent);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //tag Present
                                                        if (!_localDeviceArray[nIndex].currentInventory.listTagPresent.Contains(uidSdk))
                                                        {
                                                            _localDeviceArray[nIndex].currentInventory.listTagPresent.Add(uidSdk);
                                                            DtAndTagClass dtAndTagPresent = new DtAndTagClass(this, _localDeviceArray[nIndex].currentInventory.dtTagPresent, uidSdk, _db);
                                                            AddTagToDt(dtAndTagPresent);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        //Check and display error if missing tag during processing
                                        // KB
                                        if (_checkedAccumulate)
                                        {
                                            _localDeviceArray[nIndex].currentInventory.listTagRemoved.Clear();
                                            _localDeviceArray[nIndex].currentInventory.dtTagRemove.Rows.Clear();
                                            _localDeviceArray[nIndex].currentInventory.listTagAdded.Clear();
                                            _localDeviceArray[nIndex].currentInventory.dtTagAdded.Rows.Clear();
                                        }
                                        else
                                        {
                                            if (!_bKeepLastScan)
                                            {
                                                int nbTagSdk = _localDeviceArray[nIndex].rfidDev.get_RFID_Device.ReaderData.strListTag.Count;
                                                if (((_localDeviceArray[nIndex].currentInventory.listTagAll.Count + _nbTagBox) != nbTagSdk)
                                                    || ((_localDeviceArray[nIndex].currentInventory.dtTagAll.Rows.Count + _nbTagBox) != nbTagSdk))
                                                {
                                                    ExceptionMessageBox.Show(string.Format(ResStrings.str_errorInventoryList, nbTagSdk, _localDeviceArray[nIndex].currentInventory.listTagAll.Count, _localDeviceArray[nIndex].currentInventory.dtTagAll.Rows.Count), ResStrings.strInfo);
                                                }
                                            }
                                            if (!_bWasInAccumulation)
                                            {
                                                foreach (string uid in _localDeviceArray[nIndex].previousInventory.listTagAll)
                                                {
                                                    if (!_localDeviceArray[nIndex].currentInventory.listTagAll.Contains(uid))
                                                    {
                                                        if (!_localDeviceArray[nIndex].currentInventory.listTagRemoved.Contains(uid))
                                                        {
                                                            _localDeviceArray[nIndex].currentInventory.listTagRemoved.Add(uid);
                                                            DtAndTagClass dtAndTagRemove = new DtAndTagClass(this, _localDeviceArray[nIndex].currentInventory.dtTagRemove, uid, _db);
                                                            AddTagToDt(dtAndTagRemove);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        // Commit and check before store                             


                                        if (_localDeviceArray[nIndex].currentInventory.dtTagAll != null)
                                            _localDeviceArray[nIndex].currentInventory.dtTagAll.AcceptChanges();
                                        if (_localDeviceArray[nIndex].currentInventory.dtTagPresent != null)
                                            _localDeviceArray[nIndex].currentInventory.dtTagPresent.AcceptChanges();
                                        if (_localDeviceArray[nIndex].currentInventory.dtTagAdded != null)
                                            _localDeviceArray[nIndex].currentInventory.dtTagAdded.AcceptChanges();
                                        if (_localDeviceArray[nIndex].currentInventory.dtTagRemove != null)
                                            _localDeviceArray[nIndex].currentInventory.dtTagRemove.AcceptChanges();

                                        _localDeviceArray[nIndex].currentInventory.nbTagAll = _localDeviceArray[nIndex].currentInventory.listTagAll.Count;
                                        _localDeviceArray[nIndex].currentInventory.nbTagPresent = _localDeviceArray[nIndex].currentInventory.listTagPresent.Count;
                                        _localDeviceArray[nIndex].currentInventory.nbTagAdded = _localDeviceArray[nIndex].currentInventory.listTagAdded.Count;
                                        _localDeviceArray[nIndex].currentInventory.nbTagRemoved = _localDeviceArray[nIndex].currentInventory.listTagRemoved.Count;

                                        _localDeviceArray[nIndex].currentInventory.scanStatus = _localDeviceArray[nIndex].rfidDev.currentInventory.scanStatus;

                                        if (string.IsNullOrEmpty(_localDeviceArray[nIndex].currentInventory.BadgeID))
                                        {

                                            _localDeviceArray[nIndex].currentInventory.spareData1 = _tcpIpServer.SpareData1;
                                            _localDeviceArray[nIndex].currentInventory.spareData2 = _tcpIpServer.SpareData2;
                                        }
                                        else
                                        {
                                            if (_tcpIpServer.SpareDataCol.ContainsKey(_localDeviceArray[nIndex].currentInventory.BadgeID))
                                            {
                                                string[] spareDataSplit = _tcpIpServer.SpareDataCol[_localDeviceArray[nIndex].currentInventory.BadgeID].Split(';');
                                                if (spareDataSplit.Length == 2)
                                                {
                                                    _localDeviceArray[nIndex].currentInventory.spareData1 = spareDataSplit[0];
                                                    _localDeviceArray[nIndex].currentInventory.spareData2 = spareDataSplit[1];
                                                }
                                            }
                                            else
                                            {
                                                _localDeviceArray[nIndex].currentInventory.spareData1 = _tcpIpServer.SpareData1;
                                                _localDeviceArray[nIndex].currentInventory.spareData2 = _tcpIpServer.SpareData2;
                                            }
                                        }


                                        InventoryAndDbClass invCl = new InventoryAndDbClass(_localDeviceArray[nIndex].infoDev, _localDeviceArray[nIndex].currentInventory, _db, _bStoreTagEvent);

                                        bool ret = StoreInventory(invCl, _bStoreTagEvent, out idscanEvent);

                                        if (ret)
                                        {
                                            _localDeviceArray[nIndex].currentInventory.IdScanEvent = idscanEvent;
                                            _localDeviceArray[nIndex].lastProcessInventoryGmtDate = truncateMs(_localDeviceArray[nIndex].currentInventory.eventDate.ToUniversalTime());
                                            Invoke((MethodInvoker)delegate
                                           {
                                               /*if (tabControlInfo.SelectedIndex == 1)
                                                  UpdateScanHistory(null);*/
                                           });
                                        }

                                        _bFirstScanAccumulate = !_bKeepLastScan;
                                        _bWasInAccumulation = false;
                                    }

                                    if (!_bAccumulateSbr) //KB will be updated by the start of the next scan in accumulation or in wait tag. avoid crash on few PC
                                    {
                                        if (nIndex == _selectedReader)
                                        {
                                            Invoke((MethodInvoker)delegate { _sip.Hide(); });
                                            Thread.Sleep(50); Application.DoEvents(); Thread.Sleep(50);
                                            Invoke((MethodInvoker)delegate { new ScanFinish().Show(); });
                                            //Invoke((MethodInvoker)delegate { ProcessData(null); });
                                        }
                                    }

                                    //alert Remove time exceed
                                    bool bReaderForRemovetoLong = false;


                                    if (_removedReaderDevice.Contains(_localDeviceArray[nIndex].infoDev.SerialRFID)) bReaderForRemovetoLong = true;

                                    if (_bUseAlarm && _localDeviceArray[nIndex].currentInventory.nbTagRemoved > 0 && _maxTimeBeforeAlertRemoved > 0 && bReaderForRemovetoLong)
                                    {
                                        TimeSpan ts = new TimeSpan(0, _maxTimeBeforeAlertRemoved, 0);
                                        DateTime dateToTest = DateTime.Now.Add(ts);
                                        _removedInventoryAlert.Add(dateToTest, _localDeviceArray[nIndex]);
                                    }


                                    // alert remove nb item
                                    if (_bUseAlarm && _localDeviceArray[nIndex].currentInventory.bUserScan)
                                    {
                                        #region AT_Remove_Too_Many_Items
                                        if (_alertItems != null)
                                        {

                                            int nbMaxItem = _db.getUserMaxRemovedItem(_localDeviceArray[nIndex].currentInventory.userFirstName, _localDeviceArray[nIndex].currentInventory.userLastName);
                                            if (nbMaxItem > 0)
                                            {
                                                if (_localDeviceArray[nIndex].currentInventory.nbTagRemoved > nbMaxItem)
                                                {
                                                    UserClassTemplate tmpUtc = new UserClassTemplate();
                                                    tmpUtc.firstName = _firstName;
                                                    tmpUtc.lastName = _lastName;
                                                    string spData = string.Format(ResStrings.str_items_allowed, _localDeviceArray[nIndex].currentInventory.nbTagRemoved, nbMaxItem);
                                                    _db.storeAlert(AlertType.AT_Remove_Too_Many_Items, _localDeviceArray[nIndex].infoDev, tmpUtc, spData);
                                                    AlertMgtClass.treatAlert(AlertType.AT_Remove_Too_Many_Items, _localDeviceArray[nIndex].infoDev, tmpUtc, _localDeviceArray[nIndex].currentInventory, spData, BShowAlert);
                                                }
                                            }
                                        }
                                        #endregion
                                        #region AT_Limit_Value_Exceed
                                        if (_alertValues != null)
                                        {
                                            string columnValueName = _db.GetColumnNameForAlert(AlertType.AT_Limit_Value_Exceed);
                                            if (!string.IsNullOrEmpty(columnValueName))
                                            {
                                                double userLimit = _db.getUserMaxRemoveValue(_firstName, _lastName);
                                                if (userLimit > 0)
                                                {
                                                    double sumValue = ProcessValueAlert(_localDeviceArray[nIndex].currentInventory.dtTagRemove, columnValueName);

                                                    if (sumValue > userLimit)
                                                    {
                                                        UserClassTemplate tmpUtc = new UserClassTemplate();
                                                        tmpUtc.firstName = _firstName;
                                                        tmpUtc.lastName = _lastName;
                                                        string spData = string.Format(ResStrings.str_0_on_1_allowed, sumValue, userLimit);
                                                        _db.storeAlert(AlertType.AT_Limit_Value_Exceed, _localDeviceArray[nIndex].infoDev, tmpUtc, spData);
                                                        AlertMgtClass.treatAlert(AlertType.AT_Limit_Value_Exceed, _localDeviceArray[nIndex].infoDev, tmpUtc, _localDeviceArray[nIndex].currentInventory, spData, BShowAlert);
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }

                                    if (_bUseAlarm)
                                    {
                                        #region limit Stock
                                        // stock limit only for stent inmedical cabinet
                                        if (_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_SMC)
                                        {
                                            if (_alertStock != null)
                                            {
                                                string colAlert = _db.GetColumnNameForAlert(AlertType.AT_Stock_Limit);
                                                if (!string.IsNullOrEmpty(colAlert))
                                                {
                                                    int nbMin = 0;
                                                    int.TryParse(_alertStock.alertData, out nbMin);

                                                    Dictionary<string, int> stockProduct = new Dictionary<string, int>();
                                                    foreach (DataRow oRow in _localDeviceArray[nIndex].currentInventory.dtTagAll.Rows)
                                                    {

                                                        string productName = (string)oRow[colAlert];

                                                        if (string.IsNullOrEmpty(productName)) continue;
                                                        if (productName.Equals(ResStrings.str_Unreferenced)) continue;
                                                        if (stockProduct.ContainsKey(productName))
                                                            stockProduct[productName]++;
                                                        else
                                                            stockProduct.Add(productName, 1);
                                                    }
                                                    foreach (DataRow oRow in _localDeviceArray[nIndex].currentInventory.dtTagRemove.Rows)
                                                    {

                                                        string productName = (string)oRow[colAlert];
                                                        if (string.IsNullOrEmpty(productName)) continue;
                                                        if (productName.Equals(ResStrings.str_Unreferenced)) continue;
                                                        if (!stockProduct.ContainsKey(productName))
                                                            stockProduct.Add(productName, 0);
                                                    }

                                                    string spData = string.Empty;
                                                    foreach (KeyValuePair<string, int> pair in stockProduct)
                                                    {
                                                        if (pair.Value <= nbMin)
                                                        {
                                                            if (spData.Length > 0) spData += ";";
                                                            spData += pair.Key + ";" + pair.Value;
                                                        }
                                                    }

                                                    if (!string.IsNullOrEmpty(spData))
                                                    {
                                                        _db.storeAlert(AlertType.AT_Stock_Limit, _localDeviceArray[nIndex].infoDev, null, spData);
                                                        AlertMgtClass.treatAlert(AlertType.AT_Stock_Limit, _localDeviceArray[nIndex].infoDev, null, _localDeviceArray[nIndex].currentInventory, spData, BShowAlert);
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                        #region BloodPatient

                                        if ((!_selectedPatient.Equals(ResStrings.str_None))
                                                && (_localDeviceArray[nIndex].currentInventory.dtTagRemove.Rows.Count > 0))
                                        {
                                            if (_alertBloodPatient != null)
                                            {
                                                string columnPatient = _db.GetColumnNameForAlert(AlertType.AT_Bad_Blood_Patient);
                                                if (!string.IsNullOrEmpty(columnPatient))
                                                {

                                                    ArrayList badBlood = new ArrayList();
                                                    foreach (DataRow oRow in _localDeviceArray[nIndex].currentInventory.dtTagRemove.Rows)
                                                    {
                                                        if (oRow[columnPatient] != DBNull.Value)
                                                        {
                                                            if (!_selectedPatient.Equals((string)oRow[columnPatient]))
                                                                badBlood.Add(oRow);
                                                        }
                                                    }

                                                    if (badBlood.Count > 0)
                                                    {
                                                        string spData = string.Empty;
                                                        spData += _selectedPatient;
                                                        foreach (DataRow oRow in badBlood)
                                                        {
                                                            string rowValue = string.Empty;
                                                            for (int x = 0; x < oRow.ItemArray.Length; x++)
                                                            {
                                                                if (rowValue.Length > 0) rowValue += " - ";
                                                                rowValue += oRow[x].ToString();
                                                            }

                                                            if (spData.Length > 0) spData += ";";
                                                            spData += rowValue;
                                                        }
                                                        Invoke((MethodInvoker)delegate { new BadStopPatient().ShowDialog(); });
                                                        _db.storeAlert(AlertType.AT_Bad_Blood_Patient, _localDeviceArray[nIndex].infoDev, null, spData);
                                                        AlertMgtClass.treatAlert(AlertType.AT_Bad_Blood_Patient, _localDeviceArray[nIndex].infoDev, null, _localDeviceArray[nIndex].currentInventory, spData, BShowAlert);
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }

                                    #region group
                                    if (!_bWasInAccumulation)
                                    {
                                        if (_nbTagBox == 1)
                                        {
                                            _db.DeleteTagLinkFromBox(_bti.TagBox);
                                            foreach (string uid in _localDeviceArray[nIndex].currentInventory.listTagAll)
                                            {
                                                _db.DeleteGroupLink(uid);
                                                _db.AddGroupLink(_bti.TagBox, uid);
                                            }
                                        }
                                    }
                                    #endregion
                                    #region exportSQL
                                    if (_bUseSqlExport)
                                    {
                                        int index = nIndex;
                                        Invoke((MethodInvoker)delegate
                                        {
                                            SqlThreadHandle sqlExportThreadInfo1 = new SqlThreadHandle(_localDeviceArray[index].infoDev.SerialRFID);
                                            Thread exportThread1 = new Thread(new ThreadStart(sqlExportThreadInfo1.ThreadSqlLoop));
                                            exportThread1.Start();
                                        });
                                    }

                                    #endregion
                                    #region TcpNotification
                                    if (_bUseTcpNotification)
                                    {
                                        if (_tcpNotificationThreadInfo != null)
                                        {
                                            if (tcpUtils.PingAddress(_tcpNotificationThreadInfo.HostIp, 2000))
                                            {
                                                _tcpNotificationThreadInfo.SetParam(TcpThreadHandle.TcpNotificationType.ScanCompleted, _localDeviceArray[nIndex].infoDev.SerialRFID, _tcpIpServer.ServerIP, _tcpIpServer.Port, idscanEvent, 0);
                                                _tcpNotificationThreadInfo.ThreadTcpLoop();
                                            }
                                        }
                                    }

                                    #endregion
                                    #region cvs
                                    if (bUseCvsExchange)
                                        ReportCsvInventory(_localDeviceArray[nIndex].currentInventory);
                                    #endregion
                                    #region php

                                    /*   string res;
                                   if ((_localDeviceArray[nIndex].currentInventory.nbTagAll == 0) &&
                                       (_localDeviceArray[nIndex].currentInventory.nbTagRemoved == 0))
                                   {
                                       //nothing to save
                                   }
                                   else
                                   {
                                       phpExport.exportPhp(_localDeviceArray[nIndex].currentInventory, out res);
                                       MessageBox.Show(res);
                                   }*/

                                    if (_bUsePhpExport)
                                    {
                                        if ((_localDeviceArray[nIndex].currentInventory.nbTagAll == 0) &&
                                       (_localDeviceArray[nIndex].currentInventory.nbTagRemoved == 0))
                                        {
                                            //nothing to save
                                        }
                                        else
                                        {
                                            int index = nIndex;
                                            Invoke((MethodInvoker)delegate
                                            {
                                                PhpThreadHandle phpExportThread = new PhpThreadHandle(_localDeviceArray[index].infoDev.SerialRFID);
                                                Thread phpThread = new Thread(new ThreadStart(phpExportThread.ThreadSqlLoop));
                                                phpThread.Start();
                                            });
                                        }
                                    }

                                    #endregion

                                    if (_localDeviceArray[nIndex].bComeFromKZ)
                                    {
                                        _localDeviceArray[nIndex].bComeFromKZ = false;
                                        _localDeviceArray[nIndex].rfidDev.EnableWaitMode();
                                        Invoke((MethodInvoker)delegate { toolStripButtonStopScan.Enabled = true; });
                                    }
                                }
                                _localDeviceArray[nIndex].rfidDev.get_RFID_Device.UserActionPending = false;
                                _localDeviceArray[nIndex].rfidDev.get_RFID_Device.TCPActionPending = false;
                            }

                        }


                        bAllScanFinish = true;
                        if (_localDeviceArray != null)
                        {
                            foreach (deviceClass dc in _localDeviceArray)
                            {
                                if (dc.rfidDev.DeviceStatus == DeviceStatus.DS_InScan)
                                    bAllScanFinish = false;
                            }
                        }
                        if (_networkDeviceArray != null)
                        {
                            foreach (deviceClass dc in _networkDeviceArray)
                            {
                                if (dc.netDeviceStatus == DeviceStatus.DS_InScan)
                                    bAllScanFinish = false;
                            }
                        }
                        if (bAllScanFinish)
                        {

                            Invoke((MethodInvoker)delegate { toolStripButtonScan.Enabled = true; });
                            Invoke((MethodInvoker)delegate { toolStripButtonStopScan.Enabled = true; });
                        }

                        if (!_bAccumulateSbr) //KB will be updated by the start of the next scan in accumulation or in wait tag. avoid crash on few PC
                        {
                            UpdateTreeView();
                            Invoke((MethodInvoker)delegate { timerObjectList.Start(); });
                        }

                        for (int nIndex = 0; nIndex < _localDeviceArray.Length; nIndex++)
                        {
                            if (_localDeviceArray[nIndex].infoDev.SerialRFID.Equals(args.SerialNumber))
                            {
                                _localDeviceArray[nIndex].bDataCompleted = true;
                                _localDeviceArray[nIndex].rfidDev.bStopAccessDuringProcessData = false;
                                _localDeviceArray[nIndex].rfidDev.setLightvsState();

                                if ((_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_SBF) || (_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_SFR) || (_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_SMC))
                                {
                                    _firstName = ResStrings.str_Emergency;
                                    _lastName = ResStrings.str_Opening;
                                    LastBadge = string.Empty;
                                    _lastDoorUser = DoorInfo.DI_NO_DOOR;
                                    _lastAccessType = AccessType.AT_NONE;
                                }
                            }
                        }

                        if ((_tcpIpServer != null) && (_tcpIpServer.requestScanFromServer))
                        {
                            _tcpIpServer.requestScanFromServer = false;
                        }
                        if ((_tcpIpServer != null) && (_tcpIpServer.eventScanCompleted != null))
                            _tcpIpServer.eventScanCompleted.Set();

                        s1.Stop();
                        Debug.WriteLine("end completed " + s1.Elapsed.Milliseconds + "  ms");

                        break;
                    #endregion
                    #region CancelByHost
                    case rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost:

                        // KB when stoping sometimes twice notification is receive from board
                        // avoid the blocking and treat data only one time
                        if (_bCancelHost)
                            break;

                        _bCancelHost = true;
                        for (int nIndex = 0; nIndex < _localDeviceArray.Length; nIndex++)
                        {
                            if (_localDeviceArray[nIndex].infoDev.SerialRFID.Equals(args.SerialNumber))
                            {
                                _localDeviceArray[nIndex].rfidDev.bStopAccessDuringProcessData = true;
                                if (_bWasInAccumulation)
                                {

                                    // ie if keep last scan -not take into acount removed
                                    // remove remove last scan
                                    if ((_bKeepLastScan) || (_bWasInAccumulation))
                                    {
                                        _localDeviceArray[nIndex].currentInventory.listTagRemoved.Clear();
                                        _localDeviceArray[nIndex].currentInventory.nbTagRemoved = 0;
                                        _localDeviceArray[nIndex].currentInventory.dtTagRemove.Rows.Clear();
                                    }

                                    if ((_bWasInAccumulation) && (_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_SBR))
                                    {
                                        foreach (string uidSdk in listBoxTag.Items)
                                        {
                                            string escapedTagUid = uidSdk.Replace("'", "''"); // escape ' for DataTable Select() filter
                                            DataRow[] boxInfo = _dtGroup.Select(ResStrings.str_TagUID + "= '" + escapedTagUid + "'");

                                            if (boxInfo.Length > 0) continue;

                                            if (!_localDeviceArray[nIndex].currentInventory.listTagAll.Contains(uidSdk))
                                            {
                                                //Add all
                                                _localDeviceArray[nIndex].currentInventory.listTagAll.Add(uidSdk);
                                                DtAndTagClass dtAndTagAll = new DtAndTagClass(this, _localDeviceArray[nIndex].currentInventory.dtTagAll, uidSdk, _db);
                                                AddTagToDt(dtAndTagAll);

                                                //tag Present
                                                if (!_localDeviceArray[nIndex].currentInventory.listTagPresent.Contains(uidSdk))
                                                {
                                                    _localDeviceArray[nIndex].currentInventory.listTagPresent.Add(uidSdk);
                                                    DtAndTagClass dtAndTagPresent = new DtAndTagClass(this, _localDeviceArray[nIndex].currentInventory.dtTagPresent, uidSdk, _db);
                                                    AddTagToDt(dtAndTagPresent);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (string uidSdk in _localDeviceArray[nIndex].rfidDev.get_RFID_Device.ReaderData.strListTag)
                                        {
                                            string escapedTagUid = uidSdk.Replace("'", "''"); // escape ' for DataTable Select() filter
                                            DataRow[] boxInfo = _dtGroup.Select(ResStrings.str_TagUID + "= '" + escapedTagUid + "'");

                                            if (boxInfo.Length > 0) continue;

                                            if (!_localDeviceArray[nIndex].currentInventory.listTagAll.Contains(uidSdk))
                                            {
                                                //Add all
                                                _localDeviceArray[nIndex].currentInventory.listTagAll.Add(uidSdk);
                                                DtAndTagClass dtAndTagAll = new DtAndTagClass(this, _localDeviceArray[nIndex].currentInventory.dtTagAll, uidSdk, _db);
                                                AddTagToDt(dtAndTagAll);

                                                if (!((_bAccumulateSbr) || (_tcpIpServer.AccumulateMode)))
                                                {
                                                    if (!_localDeviceArray[nIndex].previousInventory.listTagAll.Contains(uidSdk))
                                                    {
                                                        // Tag Added
                                                        if ((!_localDeviceArray[nIndex].currentInventory.listTagAdded.Contains(uidSdk)) &&
                                                            (!_localDeviceArray[nIndex].currentInventory.listTagPresent.Contains(uidSdk)))
                                                        {
                                                            _localDeviceArray[nIndex].currentInventory.listTagAdded.Add(uidSdk);
                                                            DtAndTagClass dtAndTagAdded = new DtAndTagClass(this, _localDeviceArray[nIndex].currentInventory.dtTagAdded, uidSdk, _db);
                                                            AddTagToDt(dtAndTagAdded);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //tag Present
                                                        if ((!_localDeviceArray[nIndex].currentInventory.listTagPresent.Contains(uidSdk)) &&
                                                            (!_localDeviceArray[nIndex].currentInventory.listTagAdded.Contains(uidSdk)))
                                                        {
                                                            _localDeviceArray[nIndex].currentInventory.listTagPresent.Add(uidSdk);
                                                            DtAndTagClass dtAndTagPresent = new DtAndTagClass(this, _localDeviceArray[nIndex].currentInventory.dtTagPresent, uidSdk, _db);
                                                            AddTagToDt(dtAndTagPresent);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //tag Present
                                                    if (!_localDeviceArray[nIndex].currentInventory.listTagPresent.Contains(uidSdk))
                                                    {
                                                        _localDeviceArray[nIndex].currentInventory.listTagPresent.Add(uidSdk);
                                                        DtAndTagClass dtAndTagPresent = new DtAndTagClass(this, _localDeviceArray[nIndex].currentInventory.dtTagPresent, uidSdk, _db);
                                                        AddTagToDt(dtAndTagPresent);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //Check and display error if missing tag during processing
                                    // KB

                                    if ((_checkedAccumulate) || (_bAccumulateSbr))
                                    {
                                        _localDeviceArray[nIndex].currentInventory.listTagRemoved.Clear();
                                        _localDeviceArray[nIndex].currentInventory.dtTagRemove.Rows.Clear();
                                        _localDeviceArray[nIndex].currentInventory.listTagAdded.Clear();
                                        _localDeviceArray[nIndex].currentInventory.dtTagAdded.Rows.Clear();
                                    }
                                    else
                                    {
                                        if (!_bWasInAccumulation)
                                        {
                                            foreach (string uid in _localDeviceArray[nIndex].previousInventory.listTagAll)
                                            {
                                                if (!_localDeviceArray[nIndex].currentInventory.listTagAll.Contains(uid))
                                                {
                                                    if (!_localDeviceArray[nIndex].currentInventory.listTagRemoved.Contains(uid))
                                                    {
                                                        _localDeviceArray[nIndex].currentInventory.listTagRemoved.Add(uid);
                                                        DtAndTagClass dtAndTagRemove = new DtAndTagClass(this, _localDeviceArray[nIndex].currentInventory.dtTagRemove, uid, _db);
                                                        AddTagToDt(dtAndTagRemove);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (_localDeviceArray[nIndex].currentInventory.dtTagAll != null)
                                        _localDeviceArray[nIndex].currentInventory.dtTagAll.AcceptChanges();
                                    if (_localDeviceArray[nIndex].currentInventory.dtTagPresent != null)
                                        _localDeviceArray[nIndex].currentInventory.dtTagPresent.AcceptChanges();
                                    if (_localDeviceArray[nIndex].currentInventory.dtTagAdded != null)
                                        _localDeviceArray[nIndex].currentInventory.dtTagAdded.AcceptChanges();
                                    if (_localDeviceArray[nIndex].currentInventory.dtTagRemove != null)
                                        _localDeviceArray[nIndex].currentInventory.dtTagRemove.AcceptChanges();

                                    _localDeviceArray[nIndex].currentInventory.nbTagAll = _localDeviceArray[nIndex].currentInventory.listTagAll.Count;
                                    _localDeviceArray[nIndex].currentInventory.nbTagPresent = _localDeviceArray[nIndex].currentInventory.listTagPresent.Count;
                                    _localDeviceArray[nIndex].currentInventory.nbTagAdded = _localDeviceArray[nIndex].currentInventory.listTagAdded.Count;
                                    _localDeviceArray[nIndex].currentInventory.nbTagRemoved = _localDeviceArray[nIndex].currentInventory.listTagRemoved.Count;

                                    _localDeviceArray[nIndex].currentInventory.scanStatus = _localDeviceArray[nIndex].rfidDev.currentInventory.scanStatus;


                                    InventoryAndDbClass invCl = new InventoryAndDbClass(_localDeviceArray[nIndex].infoDev, _localDeviceArray[nIndex].currentInventory, _db, _bStoreTagEvent);
                                    int idScanEvent = -1;
                                    bool ret = StoreInventory(invCl, _bStoreTagEvent, out idScanEvent);
                                    if (ret)
                                    {
                                        _localDeviceArray[nIndex].currentInventory.IdScanEvent = idScanEvent;
                                        _localDeviceArray[nIndex].lastProcessInventoryGmtDate = truncateMs(_localDeviceArray[nIndex].currentInventory.eventDate.ToUniversalTime());
                                        Invoke((MethodInvoker)delegate
                                       {
                                           /*if (tabControlInfo.SelectedIndex == 1)
                                               //UpdateScanHistory(null);*/
                                       });
                                    }
                                    _bFirstScanAccumulate = true;
                                    _bWasInAccumulation = false;
                                    if (nIndex == _selectedReader)
                                    {
                                        Invoke((MethodInvoker)delegate { _sip.Hide(); });
                                        Thread.Sleep(50); Application.DoEvents(); Thread.Sleep(50);
                                        Invoke((MethodInvoker)delegate { new ScanFinish().Show(); });
                                        //Invoke((MethodInvoker)delegate { ProcessData(null); });
                                    }
                                    Invoke((MethodInvoker)delegate { RefreshInventory(); });
                                    UpdateTreeView();

                                }
                                else
                                {

                                    bAllScanFinish = true;
                                    if (_localDeviceArray != null)
                                    {
                                        foreach (deviceClass dc in _localDeviceArray)
                                        {
                                            if (dc.rfidDev.DeviceStatus == DeviceStatus.DS_InScan)
                                                bAllScanFinish = false;
                                        }
                                    }
                                    if (_networkDeviceArray != null)
                                    {
                                        foreach (deviceClass dc in _networkDeviceArray)
                                        {
                                            if (dc.netDeviceStatus == DeviceStatus.DS_InScan)
                                                bAllScanFinish = false;
                                        }
                                    }
                                    if (bAllScanFinish)
                                    {

                                        Invoke((MethodInvoker)delegate { toolStripButtonScan.Enabled = true; });
                                        Invoke((MethodInvoker)delegate { toolStripButtonStopScan.Enabled = true; });
                                    }

                                    if (!_bAccumulateSbr) //KB will be updated by the start of the next scan in accumulation or in wait tag. avoid crash on few PC
                                    {

                                        //Invoke((MethodInvoker)delegate { timerObjectList.Start(); });
                                        Invoke((MethodInvoker)delegate { RefreshInventory(); });
                                        UpdateTreeView();
                                        _bFirstScanAccumulate = true;
                                    }
                                    _localDeviceArray[nIndex].currentInventory = _localDeviceArray[nIndex].previousInventory;

                                }
                                #region TcpNotification
                                if (_bUseTcpNotification)
                                {
                                    if (_tcpNotificationThreadInfo != null)
                                    {
                                        if (tcpUtils.PingAddress(_tcpNotificationThreadInfo.HostIp, 2000))
                                        {
                                            _tcpNotificationThreadInfo.SetParam(TcpThreadHandle.TcpNotificationType.ScanCancelByHost, _localDeviceArray[nIndex].infoDev.SerialRFID, _tcpIpServer.ServerIP, _tcpIpServer.Port, -1, 0);
                                            _tcpNotificationThreadInfo.ThreadTcpLoop();

                                        }
                                    }
                                }
                                #endregion
                                _localDeviceArray[nIndex].rfidDev.get_RFID_Device.UserActionPending = false;
                                _localDeviceArray[nIndex].rfidDev.get_RFID_Device.TCPActionPending = false;
                            }
                        }

                        for (int nIndex = 0; nIndex < _localDeviceArray.Length; nIndex++)
                        {
                            if (_localDeviceArray[nIndex].infoDev.SerialRFID.Equals(args.SerialNumber))
                            {
                                _localDeviceArray[nIndex].bDataCompleted = true;
                                _localDeviceArray[nIndex].rfidDev.bStopAccessDuringProcessData = false;
                                _localDeviceArray[nIndex].rfidDev.setLightvsState();
                            }
                        }

                        if ((_tcpIpServer != null) && (_tcpIpServer.requestScanFromServer))
                        {
                            _tcpIpServer.requestScanFromServer = false;
                        }
                        if ((_tcpIpServer != null) && (_tcpIpServer.eventScanCancelled != null))
                            _tcpIpServer.eventScanCancelled.Set();
                        if ((_tcpIpServer != null) && (_tcpIpServer.eventScanCompleted != null))
                            _tcpIpServer.eventScanCompleted.Set();

                        _bCancelHost = false;
                        break;
                    #endregion
                    #region timeout/error scan
                    case rfidReaderArgs.ReaderNotify.RN_ReaderScanTimeout:
                    case rfidReaderArgs.ReaderNotify.RN_ErrorDuringScan:


                        ExceptionMessageBox.Show(ResStrings.str_Error_During_Scan, string.Format(ResStrings.str_Scan_return, args.RN_Value));

                        for (int nIndex = 0; nIndex < _localDeviceArray.Length; nIndex++)
                        {
                            if (_localDeviceArray[nIndex].infoDev.SerialRFID.Equals(args.SerialNumber))
                            {
                                _localDeviceArray[nIndex].rfidDev.get_RFID_Device.UserActionPending = false;
                                _localDeviceArray[nIndex].rfidDev.get_RFID_Device.TCPActionPending = false;
                                _localDeviceArray[nIndex].bDataCompleted = true;
                                _localDeviceArray[nIndex].rfidDev.bStopAccessDuringProcessData = false;
                                _localDeviceArray[nIndex].rfidDev.setLightvsState();
                            }
                            if (bScanforCSV)
                                ReportCsvError("Error During Scan");
                        }

                        _bFirstScanAccumulate = true;
                        bAllScanFinish = true;
                        foreach (deviceClass dc in _localDeviceArray)
                        {
                            if (dc.rfidDev.DeviceStatus == DeviceStatus.DS_InScan)
                                bAllScanFinish = false;
                        }

                        if (bAllScanFinish)
                        {
                            Invoke((MethodInvoker)delegate { toolStripButtonScan.Enabled = true; });
                            Invoke((MethodInvoker)delegate { toolStripButtonStopScan.Enabled = true; });
                        }

                        UpdateTreeView();
                        _bFirstScanAccumulate = true;
                        if ((_tcpIpServer != null) && (_tcpIpServer.requestScanFromServer))
                        {
                            _tcpIpServer.requestScanFromServer = false;
                        }
                        if ((_tcpIpServer != null) && (_tcpIpServer.eventScanCompleted != null))
                            _tcpIpServer.eventScanCompleted.Set();
                        break;

                    case rfidReaderArgs.ReaderNotify.RN_ReaderFailToStartScan:

                        for (int nIndex = 0; nIndex < _localDeviceArray.Length; nIndex++)
                        {
                            if (_localDeviceArray[nIndex].infoDev.SerialRFID.Equals(args.SerialNumber))
                            {
                                string str;
                                _localDeviceArray[nIndex].rfidDev.get_RFID_Device.getStatus(out str);
                                if (str.Contains("Local Open"))
                                {
                                    _localDeviceArray[nIndex].rfidDev.DeviceStatus = DeviceStatus.DS_DoorOpen;
                                }
                                _localDeviceArray[nIndex].rfidDev.get_RFID_Device.UserActionPending = false;
                                _localDeviceArray[nIndex].rfidDev.get_RFID_Device.TCPActionPending = false;
                                _localDeviceArray[nIndex].bDataCompleted = true;
                                _localDeviceArray[nIndex].rfidDev.bStopAccessDuringProcessData = false;
                                _localDeviceArray[nIndex].rfidDev.setLightvsState();

                                if (bScanforCSV)
                                    ReportCsvError("failed to start scan");
                            }

                        }

                        _bFirstScanAccumulate = true;
                        bAllScanFinish = true;
                        foreach (deviceClass dc in _localDeviceArray)
                        {
                            if (dc.rfidDev.DeviceStatus == DeviceStatus.DS_InScan)
                                bAllScanFinish = false;
                        }

                        if (bAllScanFinish)
                        {
                            Invoke((MethodInvoker)delegate { toolStripButtonScan.Enabled = true; });
                            Invoke((MethodInvoker)delegate { toolStripButtonStopScan.Enabled = true; });
                        }

                        UpdateTreeView();
                        _bFirstScanAccumulate = true;
                        if ((_tcpIpServer != null) && (_tcpIpServer.eventScanCompleted != null))
                            _tcpIpServer.eventScanCompleted.Set();
                        break;
                    #endregion
                    #region tag added
                    case rfidReaderArgs.ReaderNotify.RN_TagAdded:


                        for (int nIndex = 0; nIndex < _localDeviceArray.Length; nIndex++)
                        {
                            if (_localDeviceArray[nIndex].infoDev.SerialRFID.Equals(args.SerialNumber))
                            {
                                string escapedTagUid = args.Message.Replace("'", "''"); // escape ' for DataTable Select() filter
                                DataRow[] boxInfo = _dtGroup.Select(ResStrings.str_TagUID + "= '" + escapedTagUid + "'");

                                if (boxInfo.Length > 0)
                                {
                                    _nbTagBox++;
                                    if (!_bWasInAccumulation)
                                    {
                                        if (_nbTagBox == 1)
                                        {
                                            _bti = new BoxTagInfo();

                                            _bti.TagBox = boxInfo[0].ItemArray[0] != DBNull.Value ? (string)boxInfo[0].ItemArray[0] : string.Empty;
                                            _bti.BoxRef = boxInfo[0].ItemArray[1] != DBNull.Value ? (string)boxInfo[0].ItemArray[1] : string.Empty;
                                            _bti.BoxDesc = boxInfo[0].ItemArray[2] != DBNull.Value ? (string)boxInfo[0].ItemArray[2] : string.Empty;
                                            _bti.Criteria = boxInfo[0].ItemArray[3] != DBNull.Value ? (string)boxInfo[0].ItemArray[3] : string.Empty;
                                            _bti.Criteria = _bti.Criteria.Replace("&quote", "'");
                                            UpdateTabBox(_bti);
                                        }
                                    }
                                }
                                else
                                {
                                    _localDeviceArray[nIndex].cptOut = 1;
                                    if (nIndex == _selectedReader)
                                    {
                                        listBoxTag.Invoke((MethodInvoker)delegate { if (!listBoxTag.Items.Contains(args.Message)) listBoxTag.Items.Add(args.Message); });
                                        labelInventoryTagCount.Invoke((MethodInvoker)delegate { labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags, listBoxTag.Items.Count); });
                                    }
                                }
                            }
                        }
                        break;

                    #endregion
                    #region Power OFF
                    case rfidReaderArgs.ReaderNotify.RN_Power_OFF:
                        for (int nIndex = 0; nIndex < _localDeviceArray.Length; nIndex++)
                        {
                            if (_localDeviceArray[nIndex].infoDev.SerialRFID.Equals(args.SerialNumber))
                            {
                                _db.storeAlert(AlertType.AT_Power_Cut, _localDeviceArray[nIndex].infoDev, null, null);
                                AlertMgtClass.treatAlert(AlertType.AT_Power_Cut, _localDeviceArray[nIndex].infoDev, null, null, null, BShowAlert);
                            }
                        }
                        break;
                    #endregion
                    #region Door
                    case rfidReaderArgs.ReaderNotify.RN_DoorOpenTooLong:
                        for (int nIndex = 0; nIndex < _localDeviceArray.Length; nIndex++)
                        {
                            if (_localDeviceArray[nIndex].infoDev.SerialRFID.Equals(args.SerialNumber))
                            {
                                if (_bUseAlarm)
                                {
                                    if (_alertDoors != null)
                                    {
                                        UserClassTemplate tmpUtc = new UserClassTemplate();
                                        tmpUtc.firstName = _firstName;
                                        tmpUtc.lastName = _lastName;
                                        _db.storeAlert(AlertType.AT_Door_Open_Too_Long, _localDeviceArray[nIndex].infoDev, tmpUtc, null);
                                        AlertMgtClass.treatAlert(AlertType.AT_Door_Open_Too_Long, _localDeviceArray[nIndex].infoDev, tmpUtc, null, null, BShowAlert);
                                    }
                                }
                            }
                        }


                        break;
                    case rfidReaderArgs.ReaderNotify.RN_Locked_Before_Open:
                        {

                        }
                        break;
                    case rfidReaderArgs.ReaderNotify.RN_Door_Opened:
                        {
                            UpdateTreeView();
                            #region TcpNotification
                            if (_bUseTcpNotification)
                            {
                                if (_tcpNotificationThreadInfo != null)
                                {
                                    if (tcpUtils.PingAddress(_tcpNotificationThreadInfo.HostIp, 2000))
                                    {
                                        _tcpNotificationThreadInfo.SetParam(TcpThreadHandle.TcpNotificationType.DoorOpen, args.SerialNumber, _tcpIpServer.ServerIP, _tcpIpServer.Port, -1, 0);
                                        _tcpNotificationThreadInfo.ThreadTcpLoop();
                                    }
                                }
                            }
                            #endregion
                        }
                        break;

                    case rfidReaderArgs.ReaderNotify.RN_Door_Closed:

                        for (int nIndex = 0; nIndex < _localDeviceArray.Length; nIndex++)
                        {
                            if (_localDeviceArray[nIndex].infoDev.SerialRFID.Equals(args.SerialNumber))
                            {
                                if (DoDoorScan)
                                {
                                    if (bUseSynchonisation)
                                    {                                        
                                        if (syncDevice != null)
                                        {
                                            _localDeviceArray[nIndex].rfidDev.DeviceStatus = DeviceStatus.DS_WaitForScan;
                                            UpdateTreeView();
                                            syncDevice.bIsWaitingScan = true;
                                            Thread thScan = new Thread(() => syncDevice.CanStartScan());
                                            thScan.IsBackground = true;
                                            thScan.Start();
                                            while (syncDevice.bIsWaitingScan)
                                            {
                                                tcpUtils.NonBlockingSleep(1000);
                                            }
                                        }
                                        _localDeviceArray[nIndex].rfidDev.CanStartScan.Set();
                                    }                                
                                }
                                else
                                {
                                    _localDeviceArray[nIndex].rfidDev.DeviceStatus = DeviceStatus.DS_Ready;
                                }
                            } 
                        }                       
                        UpdateTreeView();
                        break;
                    #endregion
                    #region USB
                    case rfidReaderArgs.ReaderNotify.RN_UsbCableUnplug:
                    foreach (deviceClass t in _localDeviceArray)
                    {
                        if (t.infoDev.SerialRFID.Equals(args.SerialNumber))
                        {
                            _db.storeAlert(AlertType.AT_Usb_Unplug, t.infoDev, null, null);
                            AlertMgtClass.treatAlert(AlertType.AT_Usb_Unplug, t.infoDev, null, null, null, BShowAlert);
                        }
                    }
                    break;
                #endregion
                //case SDK_SC_RfidReader.rfidReaderArgs.ReaderNotify.RN_IntrusionDetected:
                #region sensor movement
                case rfidReaderArgs.ReaderNotify.RN_MovementDetected:
                    if (_bUseAlarm)
                    {
                        _currentAlarmEvent = DateTime.Now;
                        TimeSpan ts = _currentAlarmEvent - _lastAlarmEvent;
                        double timeInSecond = ts.TotalSeconds;
                        _lastAlarmEvent = _currentAlarmEvent;

                        if (timeInSecond > 5.0)
                        {
                            foreach (deviceClass t in _localDeviceArray)
                            {
                                if (t.infoDev.SerialRFID.Equals(args.SerialNumber))
                                {
                                    _db.storeAlert(AlertType.AT_Usb_Unplug, t.infoDev, null, null);
                                    AlertMgtClass.treatAlert(AlertType.AT_Move_Sensor, t.infoDev, null, null, null, BShowAlert);
                                }
                            }
                        }
                    }
                #endregion

                    break;
                case rfidReaderArgs.ReaderNotify.RN_ActiveChannnelChange:
                    //just to avoid refresh of datagrid
                    break;
                  
                  }
            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }
            Application.DoEvents();
        }

        private void UpdateTabBox(BoxTagInfo bti)
        {
            Invoke((MethodInvoker)delegate
            {
                if (bti != null)
                {
                    //tabControlInfo.SelectedIndex = 2;
                    txtBoxTagUid.Text = bti.TagBox;
                    txtBoxRef.Text = bti.BoxRef;
                    txtBoxDesc.Text = bti.BoxDesc;
                    txtCriteria.Text = bti.Criteria.Replace("&quote", "'");
                }
                else
                {
                    txtBoxTagUid.Text = null;
                    txtBoxRef.Text = null;
                    txtBoxDesc.Text = null;
                    txtCriteria.Text = null;
                }

            });
        }
        private void myMedicalCabinet_BagdeReader(object sender, string badgeRead)
        {
            if (_bClosing) return;
            _tcpIpServer.LastBadgeRead = badgeRead;
            //   bFirstScanAccumulate = true;
            MedicalCabinet myCab = (MedicalCabinet)sender;
            Cabinet_BadgeReader(myCab, badgeRead);
        }
        private void myFridgeCabinet_BagdeReader(object sender, string badgeRead)
        {            
            if (_bClosing) return;
            //   bFirstScanAccumulate = true;
            FridgeCabinet myFridge = (FridgeCabinet)sender;
            Cabinet_BadgeReader(myFridge, badgeRead);
        }
        private void Cabinet_BadgeReader(Cabinet cabObject, string badgeRead)
        {
                if (_tcpIpServer.requestScanFromServer) return;
                if (_localUserArray == null) return;
                if (cabObject.DoorStatus == Door_Status.Door_Open) return;

                InitializeBadgeReadingAttributes(badgeRead);

                foreach (UserClassTemplate uct in _localUserArray)
                {
                    if (String.IsNullOrEmpty(uct.BadgeReaderID)) continue; // skip this user
                    if (!uct.BadgeReaderID.Equals(badgeRead)) continue;

                    _bUserScan = true;
                    _firstName = uct.firstName;
                    _lastName = uct.lastName;
                    LastBadge = uct.BadgeReaderID;
                    _lastAccessType = AccessType.AT_BADGEREADER;
                    _lastDoorUser = DoorInfo.DI_MASTER_DOOR;
                    break;
                }
            }
        private void InitializeBadgeReadingAttributes(string badgeRead, bool lastBadgeNull = true)
        {           
            _tcpIpServer.LastBadgeRead = badgeRead;
            _bUserScan = false;
            _firstName = ResStrings.str_Manual;
            _lastName = ResStrings.str_Scan;
            if(lastBadgeNull) LastBadge = null;
            _lastAccessType = AccessType.AT_NONE;
            _lastDoorUser = DoorInfo.DI_NO_DOOR;
        }
        private void masterBadgerReader_NotifyBadgeReaderEvent(Object sender, string badgeId, string deviceSerial)
        {
             DoorsBadgerReader_NotifyBadgeReaderEvent(badgeId, deviceSerial);
        }
        private void slaveBadgerReader_NotifyBadgeReaderEvent(Object sender, string badgeId, string deviceSerial)
        {
            DoorsBadgerReader_NotifyBadgeReaderEvent(badgeId, deviceSerial, false); // 3rd parameter : master = false
        }
        private void DoorsBadgerReader_NotifyBadgeReaderEvent(string badgeId, string deviceSerial, bool master = true)
        {
            if (_bClosing) return;
            //   bFirstScanAccumulate = true;
            InitializeBadgeReadingAttributes(badgeId, master); // if master = false, then we won't put LastBadge at null in InitializeBadgeReadingAttributes       

            if (_localUserArray == null) return;

            // currentDevice = null OR a the device in localDeviceArray with the good serial (string deviceSerial)
            deviceClass currentDevice = _localDeviceArray.FirstOrDefault(dc => dc.infoDev.SerialRFID.Equals(deviceSerial));

            foreach (UserClassTemplate uct in _localUserArray)
            {
                if (String.IsNullOrEmpty(uct.BadgeReaderID)) continue; // skip this user
                if (!uct.BadgeReaderID.Equals(badgeId)) continue;

                    _bUserScan = true;
                    _firstName = uct.firstName;
                    _lastName = uct.lastName;
                    _lastAccessType = AccessType.AT_BADGEREADER;
                    LastBadge = uct.BadgeReaderID;
                if (!master) _lastDoorUser = DoorInfo.DI_SLAVE_DOOR;

                if (currentDevice != null)
                {
                    // README/note : this two instructions was out of user verification loop, before.
                    if(master) currentDevice.rfidDev.get_RFID_Device.OpenDoorMaster();
                    else currentDevice.rfidDev.get_RFID_Device.OpenDoorSlave();
                    currentDevice.rfidDev.startDoorOpenTimer();
                }

                 break;
            }
        }
        private void rfidDev_NotifyFPEvent(object sender, FingerArgs args)
        {
            if (_bClosing) return;

            try
            {
                string info = String.Format("{0} - FP Event {1} - Event Type : {2} - Data : {3}\r\n", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"), args.Master ? "Master" : "Slave", args.RN_Value.ToString(), args.Message);
                File.AppendAllText(@"C:\temp\Log.txt", info);
            }
            catch (IOException)
            {
                //erreur naresh PC - 
            }
            catch
            {
                // do nothing for now
            }
           
           

            switch (args.RN_Value)
            {
                case FingerArgs.FingerNotify.RN_FingerprintConnect:
                    _tcpIpServer.NeedRefreshUser = true;
                    break;              
                   
                case FingerArgs.FingerNotify.RN_FingerTouch:
                   
                    //ie if finger touch but not authenticated

                     if  ((_firstName.Equals(ResStrings.str_Emergency)) && (_lastName.Equals(ResStrings.str_Opening)))
                     {    
                        _bUserScan = true;
                        _firstName = ResStrings.str_Unknown;
                        _lastName =  ResStrings.str_User;
                        _lastAccessType = AccessType.AT_FINGERPRINT;   
                     }
                    break;

                case FingerArgs.FingerNotify.RN_AuthentificationCompleted:
                    _bUserScan = true;
                    //bFirstScanAccumulate = true;
                    string[] strUser = args.Message.Split(';');
                    _firstName = strUser[0];
                    _lastName = strUser[1];
                    _lastAccessType = AccessType.AT_FINGERPRINT;
                    int fingerUse = -2; // value to avoid error while return from DB - 1 
                    int.TryParse(strUser[2], out fingerUse);                    
                    if (!string.IsNullOrEmpty(strUser[3]))
                    {
                        try
                        {
                            _lastDoorUser = (DoorInfo)Enum.Parse(typeof(DoorInfo), strUser[3]);
                        }
                        catch
                        {
                            _lastDoorUser = DoorInfo.DI_NO_DOOR;
                        }
                    }
                  
                    //Finger Alert                   
                    if (_alertFinger != null)
                    {
                        int fingerAlert = _db.getUserFingerAlert(_firstName, _lastName);
                        if (fingerAlert == fingerUse)
                        {
                            UserClassTemplate tmpUtc = new UserClassTemplate();
                            tmpUtc.firstName = _firstName;
                            tmpUtc.lastName = _lastName;

                            DeviceInfo tmpDi = new DeviceInfo();
                            for (int nIndex = 0; nIndex < _localDeviceArray.Length; nIndex++)
                            {
                                if (_localDeviceArray[nIndex].infoDev.SerialRFID.Equals(args.SerialNumber))
                                {
                                    tmpDi.SerialRFID = args.SerialNumber;
                                    tmpDi.DeviceName = _localDeviceArray[nIndex].infoDev.DeviceName;
                                    break;
                                }
                            }
                            _db.storeAlert(AlertType.AT_Finger_Alert, tmpDi, tmpUtc, null);
                            AlertMgtClass.treatAlert(AlertType.AT_Finger_Alert, tmpDi, tmpUtc, null, null, BShowAlert);
                        }
                    }

                    break;
                case FingerArgs.FingerNotify.RN_CaptureBad:

                    break;
            }
            try
            {
                for (int nIndex = 0; nIndex < _localDeviceArray.Length; nIndex++)
                {
                    int index = nIndex;
                    if ((_localDeviceArray[index].infoDev.SerialFPMaster.ToUpper().Contains(args.SerialNumber.ToUpper())) ||
                        (_localDeviceArray[index].infoDev.SerialFPSlave.ToUpper().Contains(args.SerialNumber.ToUpper())))
                    {
                        
                        if ((_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_SAS) || (_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_MSR))
                        {
                           
                            Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[4].Text = string.Format(ResStrings.strt_FP_Master_Statut_format, _localDeviceArray[index].rfidDev.FPStatusMaster); });
                            Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[5].Text = string.Format(ResStrings.strt_FP_Slave_Statut_format, _localDeviceArray[index].rfidDev.FPStatusSlave); });
                        }
                        else
                        {
                            Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[4].Text = string.Format(ResStrings.strt_FP_Master_Statut_format, _localDeviceArray[index].rfidDev.FPStatusMaster); });
                        }
                        Invoke((MethodInvoker)delegate { treeViewDevice.Refresh(); });
                    }
                }
            }
            catch
            {

            }
        }
        private void ConnectLocalDevice()
        {
            try
            {
                if (_bClosing) return;
                if (_localDeviceArray == null) return;

                RFID_Device discoverRfid = new RFID_Device();
                rfidPluggedInfo[] rpi = discoverRfid.getRFIDpluggedDevice(true);
                discoverRfid.ReleaseDevice();
                if (rpi == null) return;

                foreach (deviceClass dc in _localDeviceArray)
                {
                    if (_bClosing) return;

                    rfidPluggedInfo deviceToConnect = null;
                    foreach (rfidPluggedInfo tmprfid in rpi)
                    {
                        if (tmprfid.SerialRFID == dc.infoDev.SerialRFID)
                        {
                            deviceToConnect = tmprfid;
                            break;
                        }
                    }

                    if (deviceToConnect == null) continue;

                    if (dc.rfidDev != null)
                    {
                        if ((dc.rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) || (dc.infoDev.enabled == 0))
                            continue;
                            dc.rfidDev.ReleaseDevice();
                    }

                    dc.rfidDev = new RFID_Device();
                    dc.rfidDev.TimeDoorOpenTooLong = _maxTimeOpenDoor;
                    dc.rfidDev.UseUTCdateFormat = true;
                    dc.rfidDev.InterruptScanWithFP = _interruptScanWithFp;
                    switch (dc.infoDev.deviceType)
                    {
                        case DeviceType.DT_STR:
                        case DeviceType.DT_SBR:
                            dc.rfidDev.NotifyRFIDEvent += rfidDev_NotifyRFIDEvent;
                            dc.rfidDev.Create_NoFP_Device(dc.infoDev.SerialRFID, deviceToConnect.portCom);
                            dc.rfidDev.get_RFID_Device.EnabledLog = false;
                            break;

                        case DeviceType.DT_DSB:
                        case DeviceType.DT_JSC:
                            dc.rfidDev.NotifyRFIDEvent += rfidDev_NotifyRFIDEvent;
                            dc.rfidDev.NotifyFPEvent += rfidDev_NotifyFPEvent;
                            dc.rfidDev.Create_1FP_Device(dc.infoDev.SerialRFID, deviceToConnect.portCom, dc.infoDev.SerialFPMaster, true);
                          
                            break;
                        case DeviceType.DT_SAS:
                      
                            dc.rfidDev.NotifyRFIDEvent += rfidDev_NotifyRFIDEvent;
                            dc.rfidDev.NotifyFPEvent += rfidDev_NotifyFPEvent;
                            dc.rfidDev.Create_2FP_Device(dc.infoDev.SerialRFID, deviceToConnect.portCom, dc.infoDev.SerialFPMaster, dc.infoDev.SerialFPSlave, true);
                         
                            break;
                        case DeviceType.DT_MSR:
                            dc.rfidDev.NotifyRFIDEvent += rfidDev_NotifyRFIDEvent;
                            dc.rfidDev.NotifyFPEvent += rfidDev_NotifyFPEvent;
                            dc.rfidDev.Create_2FP_Device(dc.infoDev.SerialRFID, deviceToConnect.portCom, dc.infoDev.SerialFPMaster, dc.infoDev.SerialFPSlave, true);

                            if (!string.IsNullOrEmpty(dc.infoDev.comMasterReader))
                            {
                                if (dc.masterBadgerReader != null) dc.masterBadgerReader.closePort();
                                dc.masterBadgerReader = new clBadgeReader(dc.infoDev.accessReaderType, dc.infoDev.comMasterReader, dc.infoDev.SerialRFID, false);
                                dc.masterBadgerReader.NotifyBadgeReaderEvent += masterBadgerReader_NotifyBadgeReaderEvent;

                                ArrayList listBadge0 = new ArrayList();
                               
                                DeviceGrant[] userForMiniSas = _db.RecoverAllowedUser(dc.infoDev.SerialRFID);
                                if (userForMiniSas == null) break;
                               
                                foreach (DeviceGrant uct in userForMiniSas)
                                {
                                    if ((uct.userGrant == UserGrant.UG_MASTER_AND_SLAVE) || (uct.userGrant == UserGrant.UG_MASTER))
                                    {
                                        if (!string.IsNullOrEmpty(uct.user.BadgeReaderID))
                                            listBadge0.Add(uct.user.BadgeReaderID);
                                    }
                                }
                                dc.masterBadgerReader.loadBadge(listBadge0);

                            }
                            if (!string.IsNullOrEmpty(dc.infoDev.comSlaveReader))
                            {
                                if (dc.slaveBadgerReader != null) dc.slaveBadgerReader.closePort();
                                dc.slaveBadgerReader = new clBadgeReader(dc.infoDev.accessReaderType,dc.infoDev.comSlaveReader, dc.infoDev.SerialRFID, false);
                                dc.slaveBadgerReader.NotifyBadgeReaderEvent +=slaveBadgerReader_NotifyBadgeReaderEvent;

                                ArrayList listBadge1 = new ArrayList();
                              
                                DeviceGrant[] userForMiniSas = _db.RecoverAllowedUser(dc.infoDev.SerialRFID);
                                if (userForMiniSas == null) break;
                               
                                foreach (DeviceGrant uct in userForMiniSas)
                                {
                                    if ((uct.userGrant == UserGrant.UG_MASTER_AND_SLAVE) || (uct.userGrant == UserGrant.UG_SLAVE))
                                    {
                                        if (!string.IsNullOrEmpty(uct.user.BadgeReaderID))
                                            listBadge1.Add(uct.user.BadgeReaderID);
                                    }
                                }
                                dc.slaveBadgerReader.loadBadge(listBadge1);
                            }
                          

                            break;
                        case DeviceType.DT_SMC:

                            //dc.rfidDev.NotifyRFIDEvent += rfidDev_NotifyRFIDEvent;                           
                            //dc.rfidDev.Create_NoFP_Device(dc.infoDev.SerialRFID, deviceToConnect.portCom);

                            dc.rfidDev.NotifyRFIDEvent += (rfidDev_NotifyRFIDEvent);
                            dc.rfidDev.NotifyFPEvent += (rfidDev_NotifyFPEvent);
                            if (string.IsNullOrEmpty( dc.infoDev.SerialFPMaster))
                             dc.rfidDev.Create_NoFP_Device(dc.infoDev.SerialRFID, deviceToConnect.portCom);
                            else
                                dc.rfidDev.Create_1FP_Device(dc.infoDev.SerialRFID, deviceToConnect.portCom, dc.infoDev.SerialFPMaster, true);

                            if (dc.myMedicalCabinet != null) dc.myMedicalCabinet.Dispose();
                            dc.myMedicalCabinet = new MedicalCabinet(dc.rfidDev.get_RFID_Device, dc.infoDev.comMasterReader);
                            dc.myMedicalCabinet.BadgeReader += (myMedicalCabinet_BagdeReader);
                            
                            int attempt = 0;
                           
                            while (!dc.myMedicalCabinet.Rs232Module.IsConnected && attempt < 5)
                            {
                                Thread.Sleep(5000);
                                if (dc.myMedicalCabinet != null) dc.myMedicalCabinet.Dispose();
                                dc.myMedicalCabinet = new MedicalCabinet(dc.rfidDev.get_RFID_Device, dc.infoDev.comMasterReader);
                                dc.myMedicalCabinet.BadgeReader += (myMedicalCabinet_BagdeReader);
                                attempt++;
                            }

                            dc.myMedicalCabinet.TimeZoneOffset = _timeZoneOffset;

                            ArrayList listBadge2 = new ArrayList();
                           
                            DeviceGrant[] userForMedical = _db.RecoverAllowedUser(dc.infoDev.SerialRFID);
                            if (userForMedical == null) break;
                           
                            foreach (DeviceGrant uct in userForMedical)
                            {
                                if (!string.IsNullOrEmpty(uct.user.BadgeReaderID))
                                    listBadge2.Add(uct.user.BadgeReaderID);
                            }
                            dc.myMedicalCabinet.LoadBadges(listBadge2);

                            if (bUseSynchonisation)
                            {
                                dc.rfidDev.bUseSynchronisation = true;
                                dc.rfidDev.TimeoutInSec = TimeoutInSec;
                            }
                            else
                               dc.rfidDev.bUseSynchronisation = false;

                            dc.rfidDev.DoDoorScan = DoDoorScan;

                            break;

                        case DeviceType.DT_SFR:


                            dc.rfidDev.NotifyRFIDEvent += (rfidDev_NotifyRFIDEvent);
                            dc.rfidDev.NotifyFPEvent += (rfidDev_NotifyFPEvent);
                            if (string.IsNullOrEmpty( dc.infoDev.SerialFPMaster))
                             dc.rfidDev.Create_NoFP_Device(dc.infoDev.SerialRFID, deviceToConnect.portCom);
                            else
                                dc.rfidDev.Create_1FP_Device(dc.infoDev.SerialRFID, deviceToConnect.portCom, dc.infoDev.SerialFPMaster, true);

                            if (dc.myFridgeCabinet != null) dc.myFridgeCabinet.Dispose();

                            if (dc.infoDev.fridgeType == FridgeType.FT_UNKNOWN)
                            {

                                if (dc.infoDev.comTempReader.ToUpper() == "USB") // use PT100 Probe for kirsch
                                {
                                    dc.myFridgeCabinet = new FridgeCabinet(dc.rfidDev.get_RFID_Device,
                                        dc.infoDev.comMasterReader, dc.infoDev.comTempReader, FridgeType.FT_PT100);
                                    dc.myFridgeCabinet.BadgeReader += (myFridgeCabinet_BagdeReader);
                                }
                                else
                                {
                                    dc.myFridgeCabinet = new FridgeCabinet(dc.rfidDev.get_RFID_Device,
                                        dc.infoDev.comMasterReader, dc.infoDev.comTempReader, FridgeType.FT_CAREL);
                                    dc.myFridgeCabinet.BadgeReader += (myFridgeCabinet_BagdeReader);
                                    int attempt3 = 0;
                                    while (!dc.myFridgeCabinet.Rs232Module.IsConnected && attempt3 < 5)
                                    {
                                        Thread.Sleep(5000);
                                        if (dc.myFridgeCabinet != null) dc.myFridgeCabinet.Dispose();
                                        dc.myFridgeCabinet = new FridgeCabinet(dc.rfidDev.get_RFID_Device,
                                            dc.infoDev.comMasterReader, dc.infoDev.comTempReader, FridgeType.FT_CAREL);
                                        dc.myFridgeCabinet.BadgeReader += (myFridgeCabinet_BagdeReader);
                                        attempt3++;
                                    }
                                }
                            }
                            else
                            {
                                dc.myFridgeCabinet = new FridgeCabinet(dc.rfidDev.get_RFID_Device,
                                      dc.infoDev.comMasterReader, dc.infoDev.comTempReader,dc.infoDev.fridgeType);
                                dc.myFridgeCabinet.BadgeReader += (myFridgeCabinet_BagdeReader);
                            }
                            if (!_bClockFridgeAlreadyOn)
                            {
                                ClockFridge.Start();
                                _bClockFridgeAlreadyOn = true;
                            }

                            ArrayList listBadge3 = new ArrayList();
                          
                            DeviceGrant[] userForMedical2 = _db.RecoverAllowedUser(dc.infoDev.SerialRFID);
                            if (userForMedical2 == null) break;
                          
                            foreach (DeviceGrant uct in userForMedical2)
                            {
                                if (!string.IsNullOrEmpty(uct.user.BadgeReaderID))
                                    listBadge3.Add(uct.user.BadgeReaderID);
                            }
                            dc.myFridgeCabinet.LoadBadges(listBadge3);

                          

                            break;
                        case DeviceType.DT_SBF:

                            dc.rfidDev.NotifyRFIDEvent += rfidDev_NotifyRFIDEvent;
                            dc.rfidDev.NotifyFPEvent += (rfidDev_NotifyFPEvent);
                            if (string.IsNullOrEmpty( dc.infoDev.SerialFPMaster))
                            dc.rfidDev.Create_NoFP_Device(dc.infoDev.SerialRFID, deviceToConnect.portCom);
                            else
                                dc.rfidDev.Create_1FP_Device(dc.infoDev.SerialRFID, deviceToConnect.portCom, dc.infoDev.SerialFPMaster, true);

                            if (dc.myFridgeCabinet != null) dc.myFridgeCabinet.Dispose();


                            if (dc.infoDev.fridgeType == FridgeType.FT_UNKNOWN)
                            {
                                if (dc.infoDev.comTempReader.ToUpper() == "USB") // use PT100 Probe for kirsch
                                {
                                    dc.myFridgeCabinet = new FridgeCabinet(dc.rfidDev.get_RFID_Device,
                                        dc.infoDev.comMasterReader, dc.infoDev.comTempReader, FridgeType.FT_PT100);
                                    dc.myFridgeCabinet.BadgeReader += (myFridgeCabinet_BagdeReader);
                                }
                                else
                                {
                                    dc.myFridgeCabinet = new FridgeCabinet(dc.rfidDev.get_RFID_Device,
                                        dc.infoDev.comMasterReader, dc.infoDev.comTempReader, FridgeType.FT_EVERCOM);
                                    dc.myFridgeCabinet.BadgeReader += (myFridgeCabinet_BagdeReader);
                                    int attempt3 = 0;
                                    while (!dc.myFridgeCabinet.Rs232Module.IsConnected && attempt3 < 5)
                                    {
                                        Thread.Sleep(5000);
                                        if (dc.myFridgeCabinet != null) dc.myFridgeCabinet.Dispose();
                                        dc.myFridgeCabinet = new FridgeCabinet(dc.rfidDev.get_RFID_Device,
                                            dc.infoDev.comMasterReader, dc.infoDev.comTempReader, FridgeType.FT_EVERCOM);
                                        dc.myFridgeCabinet.BadgeReader += (myFridgeCabinet_BagdeReader);
                                        attempt3++;
                                    }
                                }
                            }
                            else
                            {
                                dc.myFridgeCabinet = new FridgeCabinet(dc.rfidDev.get_RFID_Device,
                                     dc.infoDev.comMasterReader, dc.infoDev.comTempReader, dc.infoDev.fridgeType);
                                dc.myFridgeCabinet.BadgeReader += (myFridgeCabinet_BagdeReader);
                            }

                            if (!_bClockFridgeAlreadyOn)
                            {
                                ClockFridge.Start();
                                _bClockFridgeAlreadyOn = true;
                            }

                            ArrayList listBadge4 = new ArrayList();
                          
                            DeviceGrant[] userForMedical3 = _db.RecoverAllowedUser(dc.infoDev.SerialRFID);
                            if (userForMedical3 == null) break;
                           
                            foreach (DeviceGrant uct in userForMedical3)
                            {
                                if (!string.IsNullOrEmpty(uct.user.BadgeReaderID))
                                    listBadge4.Add(uct.user.BadgeReaderID);
                            }
                            dc.myFridgeCabinet.LoadBadges(listBadge4);
                           

                            break;

                    }
                }
                //updateTreeView();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ExceptionMessageBox.Show(exp);
            }

        }

        private void JobConnect(deviceClass dc)
        {
            bool isLedLightingAvailable;

            if (tcpUtils.PingAddress(dc.infoDev.IP_Server, 500))
            {
                TcpIpClient tcp = null;
                if ((DeviceClientTcp != null) && (DeviceClientTcp.ContainsKey(dc.infoDev.IP_Server)))
                    tcp = DeviceClientTcp[dc.infoDev.IP_Server] = new TcpIpClient();

                if (tcp == null) return;
                if (tcp.pingServer(dc.infoDev.IP_Server, dc.infoDev.Port_Server) ==
                    TcpIpClient.RetCode.RC_Succeed)
                {
                    dc.netConnectionStatus = ConnectionStatus.CS_Connected;
                    tcp.RequestIsSPCE2Available(dc.infoDev.IP_Server, dc.infoDev.Port_Server,
                        out isLedLightingAvailable);
                    CanHandleLeds[dc] = isLedLightingAvailable;

                    ProcessNotification(dc, tcp);
                   
                    // recover previous scan. 
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        ProcessScan(dc, tcp);
                    });


                    //Processuser
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        if ((dc.infoDev.deviceType == DeviceType.DT_SFR) ||
                            (dc.infoDev.deviceType == DeviceType.DT_SBF))
                        {

                            ProcessTemp(dc, tcp);
                        }
                        Invoke((MethodInvoker) delegate { toolStripButtonScan.Enabled = true; });
                        Invoke((MethodInvoker) delegate { toolStripButtonStopScan.Enabled = true; });
                        if (tcp.CpuKindValue == TcpIpClient.CpuKind.IsWindows)
                        {
                            ProcessUserWindows(dc, tcp);
                        }
                        if (tcp.CpuKindValue == TcpIpClient.CpuKind.IsArm)
                        {
                            ProcessUserArm(dc, tcp);
                        }
                    });
                }
            }
        }

        private void ProcessNotification(deviceClass dc, TcpIpClient tcp)
        {
            // Check use tcp notification
            dc.bUseTcpNotification = false;
            _eventTestTcp.Reset();

            if (tcp.CpuKindValue == TcpIpClient.CpuKind.IsWindows)
            {
                if (
                    tcp.RequestIsUsingTcpNotification(dc.infoDev.IP_Server, dc.infoDev.Port_Server) ==
                    TcpIpClient.RetCode.RC_Succeed)
                {
                    if (
                        tcp.SetTcpServerNotificationInfo(dc.infoDev.IP_Server,
                            dc.infoDev.Port_Server,
                            true,
                            _tcpIpServer.ServerIP, _tcpIpServer.Port + 1) ==
                        TcpIpClient.RetCode.RC_Succeed)
                    {


                        bool result;
                        string errorMsg;
                        TcpIpClient tcptest = DeviceClientTcp[dc.infoDev.IP_Server];
                        tcptest.TestTcpServerNotification(dc.infoDev.IP_Server,
                            dc.infoDev.Port_Server,
                            out result, out errorMsg);
                        //dc.bUseTcpNotification = true;
                        Thread.Sleep(1);

                    }

                }
            }
            if (tcp.CpuKindValue == TcpIpClient.CpuKind.IsArm)
            {
                tcp.DeviceEventTcp += tcp_DeviceEventTcp;
                dc.bUseTcpNotification = true;
            }
        }

        private void ProcessUserWindows(deviceClass dc, TcpIpClient tcp)
        {
            // remove all previous user before connect to refresh it.
            if (dc.infoDev.deviceType != DeviceType.DT_SBR)
            {
                UserClassTemplate[] userInDevice = null;

              /*  if (
                    tcp.getUserList(dc.infoDev.IP_Server, dc.infoDev.Port_Server,
                        dc.infoDev.SerialRFID, out userInDevice) ==
                    TcpIpClient.RetCode.RC_Succeed)
                {
                    foreach (UserClassTemplate utc in userInDevice)
                    {
                        tcp.deleteUser(dc.infoDev.IP_Server, dc.infoDev.Port_Server,
                            utc.firstName,
                            utc.lastName, dc.infoDev.SerialRFID);
                    }
                }*/
                tcp.deleteUserGrant(dc.infoDev.IP_Server, dc.infoDev.Port_Server, "ALL", "ALL", dc.infoDev.SerialRFID);
                DeviceGrant[] userTemp = _db.RecoverAllowedUser(dc.infoDev.SerialRFID);

                if (userTemp != null)
                {
                    foreach (DeviceGrant us in userTemp)
                    {

                        if (string.IsNullOrEmpty(us.user.BadgeReaderID))
                            tcp.addUserFromTemplate(dc.infoDev.IP_Server, dc.infoDev.Port_Server,
                                us.user.firstName, us.user.lastName, us.user.template);
                        else
                            tcp.addUserFromTemplate(dc.infoDev.IP_Server, dc.infoDev.Port_Server,
                                us.user.firstName, us.user.lastName, us.user.template,
                                us.user.BadgeReaderID);
                        tcp.addUserGrant(dc.infoDev.IP_Server, dc.infoDev.Port_Server,
                            us.user.firstName, us.user.lastName, dc.infoDev.SerialRFID,
                            us.userGrant);
                    }
                }
            }
        }
        private void ProcessUserArm(deviceClass dc, TcpIpClient tcp)
        {
            // For ARM - Need To get User Active  and remove all permission.
            List<DeviceGrant> lstusers = tcp._tcpArmDevice.GetUsersList();
            foreach (DeviceGrant dg in lstusers)
            {
                if (dg.userGrant == UserGrant.UG_NONE)
                    continue;
                string login = dg.user.firstName + "_" + dg.user.lastName;
                tcp._tcpArmDevice.UpdatePermission(login, UserGrant.UG_NONE);
            }

            // Get User for device
            DeviceGrant[] userTemp = _db.RecoverAllowedUser(dc.infoDev.SerialRFID);


            // Get User Inactive in device (all as all permission just removed
            List<string> lstInactiveUser = tcp._tcpArmDevice.GetUnregisteredUsers();
            lstusers = tcp._tcpArmDevice.GetUsersList(); //tous a NONE

            if (userTemp != null)
            {
                foreach (DeviceGrant us in userTemp)
                {
                    string login = us.user.firstName + "_" + us.user.lastName;

                    bool bExist = false;
                    foreach (DeviceGrant tmpDg in lstusers)
                    {
                        string tmpLogin = tmpDg.user.firstName + "_" + tmpDg.user.lastName;
                        if (tmpLogin == login)
                        {
                            bExist = true;
                            break;
                        }
                    }

                    if ((bExist) || (lstInactiveUser.Contains(login)))
                    //update User - need update it and put permission
                    {
                        // Reactive it by udpate permission
                        tcp._tcpArmDevice.UpdatePermission(login, us.userGrant);

                        // Get device user
                        DeviceGrant remoteUser = tcp._tcpArmDevice.GetUserByName(login);
                        if (remoteUser == null) continue;

                        if (remoteUser.user.template == us.user.template)
                            //template equal - no need to update
                            continue;

                        // Need to get each finger template to pass to compare it.
                        UserClass currentRemoteUser = new UserClass();
                        BinaryFormatter bf1 = new BinaryFormatter();
                        MemoryStream mem1 =
                            new MemoryStream(Convert.FromBase64String(us.user.template));
                        currentRemoteUser = (UserClass)bf1.Deserialize(mem1);
                        FingerData currentRemoteFinger = new FingerData();
                        currentRemoteFinger.CopyUserToFinger(currentRemoteUser);

                        UserClass currentLocalUser = new UserClass();
                        BinaryFormatter bf2 = new BinaryFormatter();
                        MemoryStream mem2 =
                            new MemoryStream(Convert.FromBase64String(us.user.template));
                        currentLocalUser = (UserClass)bf2.Deserialize(mem2);
                        FingerData currentLocalFinger = new FingerData();
                        currentLocalFinger.CopyUserToFinger(currentLocalUser);

                        for (int index = 0; index < 10; index++)
                        {
                            if (currentLocalFinger.Templates[index] !=
                                currentRemoteFinger.Templates[index])
                            {
                                if (currentLocalFinger.Templates[index] == null)
                                {
                                    tcp._tcpArmDevice.RemoveFingerprint(login,
                                        (FingerIndexValue)index);
                                }
                                else
                                {
                                    tcp._tcpArmDevice.EnrollFinger(login,
                                        (FingerIndexValue)index,
                                        currentLocalUser.strFingerprint[index]);
                                }
                            }
                        }
                    }
                    else //Create User
                    {
                        tcp._tcpArmDevice.AddUser(us);
                    }
                }
            }
        }
        private void ProcessTemp(deviceClass dc, TcpIpClient tcp)
        {
            if (tcp.CpuKindValue != TcpIpClient.CpuKind.Unknown)
            {
                tempInfo lastTemp = _db.GetLastFridgeTemp(dc.infoDev.SerialRFID);
                tempInfo lastCurrentTemp = null;
                tcp.getFridgeCurrentTemp(dc.infoDev.IP_Server, dc.infoDev.Port_Server,
                    dc.infoDev.SerialRFID, out lastCurrentTemp);
                if (lastTemp != null)
                {
                    tempInfo[] tempArray = null;
                    tcp.getFridgeTempFromDate(dc.infoDev.IP_Server, dc.infoDev.Port_Server,
                        dc.infoDev.SerialRFID, lastTemp.CreationDate.ToLocalTime(),
                        out tempArray);

                    if (tempArray != null)
                    {
                        foreach (tempInfo tmp in tempArray)
                        {
                            if ((tmp.CreationDate > lastTemp.CreationDate) &&
                                (tmp.nbValueTemp == 60))
                                _db.StoreTempFridge(dc.infoDev.SerialRFID, tmp);
                        }
                    }
                }
                else
                {
                    if ((lastCurrentTemp != null) && (lastCurrentTemp.nbValueTemp == 60))
                    {
                        _db.StoreTempFridge(dc.infoDev.SerialRFID, lastCurrentTemp);
                    }
                }
            }


            if (!_bClockFridgeAlreadyOn)
            {
                ClockFridge.Start();
                _bClockFridgeAlreadyOn = true;
            }
        }
        private void ProcessScan(deviceClass dc, TcpIpClient tcp)
        {
            InventoryData invtmp = null;
            tcp.getLastScanID(dc.infoDev.IP_Server, dc.infoDev.Port_Server, dc.infoDev.SerialRFID,
                out dc.netLastScanEventID);
            if (dc.netLastScanEventID > 1)
            {

                tcp.requestGetScanFromIdEvent(dc.infoDev.IP_Server, dc.infoDev.Port_Server,
                    dc.infoDev.SerialRFID, dc.netLastScanEventID, out invtmp);
                Thread.Sleep(10);
            }

            if (invtmp != null)
            {
                InventoryData[] inventoryArray = null;
                TcpIpClient.RetCode ret = tcp.getScanFromDateWithDB(dc.infoDev.IP_Server,
                    dc.infoDev.Port_Server, dc.infoDev.SerialRFID,
                    invtmp.eventDate.ToUniversalTime(),
                    out inventoryArray);
                if (ret == TcpIpClient.RetCode.RC_Succeed)
                {
                    if (inventoryArray != null)
                    {
                        foreach (InventoryData inv in inventoryArray)
                        {
                            DateTime dtUtc = DateTime.SpecifyKind(inv.eventDate, DateTimeKind.Utc);
                            inv.eventDate = dtUtc.ToLocalTime();
                            StoreNetworkinventory(dc, inv);
                        }
                    }
                }
            }

        }


        private void ConnectNetworkDevice()
        {
            try
            {
                if (_bClosing) return;
                if (_networkDeviceArray == null) return;

                for (int loop = 0 ; loop < _networkDeviceArray.Count() ; loop++)
                {
               
                    if (_bClosing) return;
                    if (!CheckIpAddr(_networkDeviceArray[loop].infoDev.IP_Server)) continue;

                    if (_networkDeviceArray[loop].netConnectionStatus == ConnectionStatus.CS_Connected)
                    {
                        continue;
                    }

                    //deviceClass tmpDc =  (deviceClass)_networkDeviceArray[loop].Clone();
                    //ThreadPool.QueueUserWorkItem(delegate { JobConnect(_networkDeviceArray[loop]); });
                    JobConnect(_networkDeviceArray[loop]);
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ExceptionMessageBox.Show(exp);
            }
        }          
        public void ProcessFridgeDb(string serialRfid)
        {
            // GetLast Temp V2
            MainDBClass dbLocal = new MainDBClass();
            dbLocal.OpenDB();
            bool bNeedStartDate = true;
            string dateLastLocal = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            PtTemp lastPtTemp = _db.GetLastTempPoint(serialRfid, true);
            if (lastPtTemp != null)
            {
                dateLastLocal = lastPtTemp.TempAcqDate;
                bNeedStartDate = false;
            }

            // To avoid long time store only last four hour
           // DateTime tmpLocal = DateTime.ParseExact(dateLastLocal, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            DateTime tmpLocal = DateTime.Now.AddHours(-4);
            DateTime tmpUtc = tmpLocal.AddHours(-1.0).ToUniversalTime();

            // DateTime ConvertDateTime = DateTime.Parse(TmpUtc.ToString());
          
            //string[] fridgeTemp = _db.GetFridgeTempAfter(serialRfid, tmpUtc.ToString("yyyy-MM-dd HH:mm:ssZ"));
            string[] fridgeTemp = _db.GetFridgeTempAfter(serialRfid, tmpUtc.ToString("yyyy-MM-dd HH:mm:ssZ"));

            // Load all previous point from DB
            DateTime datePointToAdd = new DateTime(tmpLocal.Year, tmpLocal.Month, tmpLocal.Day, tmpLocal.Hour, tmpLocal.Minute, 0);
            string tmpInfo = toolStripStatusLabelInfo.Text;
            if (fridgeTemp != null)
            {
                int cpt = 0;
                foreach (string strTemp in fridgeTemp)
                {
                    cpt++;
                                      
                    Invoke(new MethodInvoker(
                        delegate
                        {
                            toolStripStatusLabelInfo.Text = string.Format("{0} ({1}/{2})", tmpInfo, cpt,
                                fridgeTemp.Length);
                        }));
                    Invoke(new MethodInvoker(delegate { toolStripStatusLabelInfo.Invalidate(); }));     

                    Thread.Sleep(50);
                    Application.DoEvents();
                    Thread.Sleep(50);
                    if (strTemp != null)
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream mem = new MemoryStream(Convert.FromBase64String(strTemp));
                        tempInfo tempData = (tempInfo)bf.Deserialize(mem);

                        if (bNeedStartDate)
                        {
                            bNeedStartDate = false;
                            datePointToAdd = new DateTime(tempData.CreationDate.ToLocalTime().Year, tempData.CreationDate.ToLocalTime().Month, tempData.CreationDate.ToLocalTime().Day, tempData.CreationDate.ToLocalTime().Hour, tempData.CreationDate.ToLocalTime().Minute, 0);
                        }

                        //dbLocal.startTranscation();
                        for (int loop = 0; loop < tempData.nbValueTemp; loop++)
                        {
                            dbLocal.startTranscation();
                            // add missing point
                            
                            DateTime curentPointDate = tempData.CreationDate.AddMinutes(loop);
                            TimeSpan ts = curentPointDate.ToLocalTime() - datePointToAdd;
                            bool bValueNull;
                           
                            while (ts.TotalSeconds > 60)
                            {
                                if (!dbLocal.IsPointExist(serialRfid, datePointToAdd.ToString("yyyy-MM-ddTHH:mm:00"),out bValueNull))
                                    dbLocal.AddNewPtTemp(serialRfid, datePointToAdd.ToString("yyyy-MM-ddTHH:mm:00"));
                                datePointToAdd = datePointToAdd.AddMinutes(1);
                                ts = curentPointDate.ToLocalTime() - datePointToAdd;
                            }
                            dbLocal.endTranscation();
                            Thread.Sleep(50);
                            //Update temperature
                            dbLocal.startTranscation();
                            if ( (ts.TotalSeconds > 0) && (ts.TotalSeconds < 60))
                            {
                                // add curentPoint
                                if (tempData.tempBottle == null) 
                                    continue;

                                if (!dbLocal.IsPointExist(serialRfid, datePointToAdd.ToString("yyyy-MM-ddTHH:mm:00"), out bValueNull))
                                {
                                    if (tempData.tempBottle.ContainsKey(loop + tempData.CreationDate.Minute))
                                    {
                                        double pt = tempData.tempBottle[loop + tempData.CreationDate.Minute];
                                        dbLocal.AddNewPtTemp(serialRfid, datePointToAdd.ToString("yyyy-MM-ddTHH:mm:00"), pt, pt);
                                    }
                                    else
                                    {
                                        dbLocal.AddNewPtTemp(serialRfid, datePointToAdd.ToString("yyyy-MM-ddTHH:mm:00"));
                                    }
                                }
                                else
                                {
                                    if (bValueNull) // point exist avec valeur null - on updtate si on peut
                                    {
                                        if (tempData.tempBottle.ContainsKey(loop + tempData.CreationDate.Minute))
                                        {
                                            double pt = tempData.tempBottle[loop + tempData.CreationDate.Minute];
                                            dbLocal.UpdatePtTemp(serialRfid, datePointToAdd.ToString("yyyy-MM-ddTHH:mm:00"), pt, pt);
                                        }
                                    }
                                }
                                
                                datePointToAdd = datePointToAdd.AddMinutes(1);
                            }
                            dbLocal.endTranscation();
                        }
                        //dbLocal.endTranscation();
                    }
                }
            }
            dbLocal.CloseDB();
        }
        public void ProcessFridgeRealTime(tempInfo currentTemp, string serialRfid)
        {
            TimeSpan ts;
            DateTime datePointToAdd;
            PtTemp lastPtTemp = _db.GetLastTempPoint(serialRfid, true);
            if (lastPtTemp == null) 
                datePointToAdd = DateTime.Now.AddHours(-1.0);
            else
                datePointToAdd = DateTime.ParseExact(lastPtTemp.TempAcqDate, "yyyy-MM-ddTHH:mm:ss", null);

            for (int loop = 0; loop < currentTemp.nbValueTemp; loop++)
            {
                // add missing point
                Application.DoEvents();
                DateTime curentPointDate = currentTemp.CreationDate.AddMinutes(loop);
                ts = curentPointDate.ToLocalTime() - datePointToAdd;
                bool bValueNull;
                while (ts.TotalSeconds > 60)
                {

                    if (!_db.IsPointExist(serialRfid, datePointToAdd.ToString("yyyy-MM-ddTHH:mm:00") , out bValueNull))
                        _db.AddNewPtTemp(serialRfid, datePointToAdd.ToString("yyyy-MM-ddTHH:mm:00"));
                    datePointToAdd = datePointToAdd.AddMinutes(1);
                    ts = curentPointDate.ToLocalTime() - datePointToAdd;
                }
                if (ts.TotalSeconds < 0) continue;
                if (currentTemp.tempBottle == null) continue;
                // add currentPoint
                if (!_db.IsPointExist(serialRfid, datePointToAdd.ToString("yyyy-MM-ddTHH:mm:00"), out bValueNull))
                {
                    if (currentTemp.tempBottle.ContainsKey(loop + currentTemp.CreationDate.Minute))
                    {
                        double pt = currentTemp.tempBottle[loop + currentTemp.CreationDate.Minute];
                        _db.AddNewPtTemp(serialRfid, datePointToAdd.ToString("yyyy-MM-ddTHH:mm:00"), pt, pt);
                    }
                    else
                    {
                        _db.AddNewPtTemp(serialRfid, datePointToAdd.ToString("yyyy-MM-ddTHH:mm:00"));
                    }
                }
                else
                {
                    if (bValueNull) // point exist avec valeur null - on updtate si on peut
                    {
                        if (currentTemp.tempBottle.ContainsKey(loop + currentTemp.CreationDate.Minute))
                        {
                            double pt = currentTemp.tempBottle[loop + currentTemp.CreationDate.Minute];
                            _db.AddNewPtTemp(serialRfid, datePointToAdd.ToString("yyyy-MM-ddTHH:mm:00"), pt, pt);
                        }
                    }
                }
                datePointToAdd = datePointToAdd.AddMinutes(1);
            }
        }
        public void UpdateTemp(int nIndexDevice, string serialRfid)
        {
            try
            {
                if (_networkDeviceArray[nIndexDevice].currentTemp == null) return; 
                ListTempBottle[nIndexDevice].Clear();

                DateTime selectedDateTime = DateTime.Now.AddHours(-6);
                //DateTime selectedDateTimeUTC = DateTime.ParseExact(selectedDateTime.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal);
                DateTime selectedDateTimeUtc = selectedDateTime.ToUniversalTime();
                string[] tpInfo = _db.GetFridgeTempAfter(serialRfid, selectedDateTimeUtc.ToString("yyyy-MM-dd HH:mm:ssZ"));
                if (tpInfo != null)
                {
                    DateTime previousTempInfoDate = selectedDateTimeUtc;

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
                                int nbpoint;
                                nbpoint = currentTempInfo.CreationDate.Minute == 0 ? 60 : currentTempInfo.CreationDate.Minute;
                                if (nbpoint != 60) break;
                                for (int j = 0; j < nbpoint; ++j)
                                {

                                    PtTemp pt = new PtTemp();
                                    pt.TempAcqDate = previousTempInfoDate.AddMinutes(j).ToString(CultureInfo.InvariantCulture);
                                    pt.TempBottle = null;
                                    pt.TempChamber = null;
                                    ListTempBottle[nIndexDevice].Add(pt.TempBottle);
                                   
                                }
                            }

                            DateTime lotdatecurrent = _networkDeviceArray[nIndexDevice].currentTemp.CreationDate.ToLocalTime();
                            if (lotdatecurrent != lotDate)
                            {
                                // first temperature minute time (e.g "29" if it was measured at 5:29pm)
                                int minuteStart = lotDate.Minute;
                                // for all points missing, from hh:00 to hh:minuteStart, add empty points in the chart.
                                for (int j = 0; j < minuteStart; ++j)
                                {
                                    PtTemp pt = new PtTemp();
                                    pt.TempAcqDate =
                                        lotDate.AddMinutes(j).ToString(CultureInfo.InvariantCulture);
                                    pt.TempBottle = null;
                                    pt.TempChamber = null;
                                    ListTempBottle[nIndexDevice].Add(pt.TempBottle);

                                }
                                // for all points from hh:minuteStart to the end of the hour, add the point value (if any) or an empty point.
                                for (int j = minuteStart; j < currentTempInfo.nbValueTemp; ++j)
                                {
                                    PtTemp pt = new PtTemp();
                                    pt.TempAcqDate =
                                        lotDate.AddMinutes(j - minuteStart).ToString(CultureInfo.InvariantCulture);
                                    pt.TempBottle = currentTempInfo.tempBottle.ContainsKey(j)
                                        ? currentTempInfo.tempBottle[j]
                                        : (double?) null;
                                    pt.TempChamber = currentTempInfo.tempBottle.ContainsKey(j)
                                        ? currentTempInfo.tempBottle[j]
                                        : (double?) null;
                                    ListTempBottle[nIndexDevice].Add(pt.TempBottle);

                                }
                                // update the last processed datetime.
                                previousTempInfoDate = lotDate;
                            }

                        }
                    }
                   
                }

                // add current one
                if ( _networkDeviceArray[nIndexDevice].currentTemp != null)
                {
                    DateTime lotDate2 = _networkDeviceArray[nIndexDevice].currentTemp.CreationDate.ToLocalTime();

                    int minuteStart2 = lotDate2.Minute;
                    for (int j = 0; j < minuteStart2; ++j)
                    {
                        PtTemp pt = new PtTemp();
                        pt.TempAcqDate =
                            lotDate2.AddMinutes(-minuteStart2 + j).ToString(CultureInfo.InvariantCulture);
                        pt.TempBottle = null;
                        pt.TempChamber = null;
                        ListTempBottle[nIndexDevice].Add(pt.TempBottle);

                    }
                    // for all points from hh:minuteStart to the end of the hour, add the point value (if any) or an empty point.
                    for (int j = minuteStart2;
                        j < (minuteStart2 + _networkDeviceArray[nIndexDevice].currentTemp.nbValueTemp);
                        ++j)
                    {
                        PtTemp pt = new PtTemp();
                        pt.TempAcqDate = lotDate2.AddMinutes(j - minuteStart2)
                            .ToString(CultureInfo.InvariantCulture);
                        pt.TempBottle = _networkDeviceArray[nIndexDevice].currentTemp.tempBottle.ContainsKey(j)
                            ? _networkDeviceArray[nIndexDevice].currentTemp.tempBottle[j]
                            : (double?) null;
                        pt.TempChamber = _networkDeviceArray[nIndexDevice].currentTemp.tempBottle.ContainsKey(j)
                            ? _networkDeviceArray[nIndexDevice].currentTemp.tempBottle[j]
                            : (double?) null;
                        ListTempBottle[nIndexDevice].Add(pt.TempBottle);

                    }
                }

                int nbPoint = ListTempBottle[nIndexDevice].Count;

                if (nbPoint > 240)
                {
                    ListTempBottle[nIndexDevice].RemoveRange(0, nbPoint - 240);
                }
              
                
            }
            catch
            {
            }
            
        }
        public void ProcessFullTemp(string serialRfid)
        {
            try
            {
                int index = -1;
                if (_networkDeviceArray == null) return;
                for (int i = 0; i < _networkDeviceArray.Length; i++)
                {
                    if (_networkDeviceArray[i].infoDev.SerialRFID.Equals(serialRfid))
                    {
                        index = i;
                        break;
                    }
                }
                if (index != -1)
                {
                    Invoke(new MethodInvoker(() => treeViewDevice.Enabled = false));
                    Invoke(new MethodInvoker(() => Cursor = Cursors.WaitCursor));
                    Invoke(new MethodInvoker(() => toolStripStatusLabelInfo.Text = string.Format(ResStrings.str__Please_Wait___Get_previous_temperature_for_device, _networkDeviceArray[index].infoDev.DeviceName)));
                    Application.DoEvents();

                    tempInfo lastCurrentTemp = null;
                    ProcessFridgeDb(serialRfid);
                    Thread.Sleep(50);
                    Application.DoEvents();
                    Thread.Sleep(50);
                    // Redo if a new teminfo is availbale during the previous long one
                    ProcessFridgeDb(serialRfid);
                    Thread.Sleep(50);
                    Application.DoEvents();
                    Thread.Sleep(50);
                    if (_networkDeviceArray == null) return; // redo as request close can arrive in loop.
                    TcpIpClient tcp1 = DeviceClientTcp[_networkDeviceArray[index].infoDev.IP_Server];
                    Invoke(new MethodInvoker(() => toolStripStatusLabelInfo.Text = string.Format(ResStrings.str_Please_Wait___Get_current_temperature_for_device, _networkDeviceArray[index].infoDev.DeviceName)));
                    tcp1.getFridgeCurrentTemp(_networkDeviceArray[index].infoDev.IP_Server, _networkDeviceArray[index].infoDev.Port_Server, _networkDeviceArray[index].infoDev.SerialRFID, out lastCurrentTemp);
                    if (lastCurrentTemp != null)
                        ProcessFridgeRealTime(lastCurrentTemp, serialRfid);
                    Thread.Sleep(50);
                    Application.DoEvents();
                    Thread.Sleep(50);
                    Invoke(new MethodInvoker(() => toolStripStatusLabelInfo.Text = string.Format(ResStrings.str_Set_current_temperature_for_device_0, _networkDeviceArray[index].infoDev.DeviceName)));
                    UpdateTemp(index, serialRfid);
                    if (_networkDeviceArray == null) return; // redo as request close can arrive in loop.
                    _networkDeviceArray[index].netFridgeTemperatureProcessDone = true;
                }


            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }
            finally
            {
                Invoke(new MethodInvoker(() => treeViewDevice.Enabled = true));
                Invoke(new MethodInvoker(() => Cursor = Cursors.Default));
                Invoke(new MethodInvoker(() => toolStripStatusLabelInfo.Text = null));
                Thread.Sleep(50);
                Application.DoEvents();
                Thread.Sleep(50);
                DateTime dt = DateTime.Now;
                if (dt.Second < 59)
                    ClockFridge.Interval = ((59 - dt.Second) * 1000);
                else
                    ClockFridge.Interval = 60000;

               
                ClockFridge.Start();
            }
        }
        private void CloseReader()
        {
            _bClosing = true;
            timerStartup.Enabled = false;
            if (_localDeviceArray == null) return;
            foreach (deviceClass dc in _localDeviceArray)
            {
                switch (dc.infoDev.deviceType)
                {
                    case DeviceType.DT_SBR:

                        if ((dc.rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) && (dc.rfidDev.DeviceStatus == DeviceStatus.DS_WaitTag))
                            dc.rfidDev.DisableWaitMode();

                        if (dc.rfidDev == null) return;
                        dc.rfidDev.ReleaseDevice();
                        break;
                    case DeviceType.DT_SAS:
                    case DeviceType.DT_MSR:
                    case DeviceType.DT_STR:
                    case DeviceType.DT_DSB:
                    case DeviceType.DT_JSC:

                        if (dc.masterBadgerReader != null) dc.masterBadgerReader.closePort();
                        if (dc.slaveBadgerReader != null) dc.slaveBadgerReader.closePort();

                        if (dc.rfidDev == null) return;
                        dc.rfidDev.ReleaseDevice();
                        break;

                    case DeviceType.DT_SMC:
                        if (dc.myMedicalCabinet != null)
                        {
                            dc.myMedicalCabinet.Dispose();
                        }
                        if (dc.rfidDev == null) return;
                        dc.rfidDev.ReleaseDevice();
                        break;
                    case DeviceType.DT_SFR:
                    case DeviceType.DT_SBF:
                        /*if (dc.myFridgeCabinet != null)
                        {
                            // store last temp before quit
                            if (dc.myFridgeCabinet.GetPreviousTempInfo != null)
                            {
                                lock (_locker)
                                {
                                    if (_db.StoreTempFridge(dc.infoDev.SerialRFID, dc.myFridgeCabinet.GetPreviousTempInfo))
                                        dc.myFridgeCabinet.GetPreviousTempInfo = null;
                                }
                            }
                            dc.myFridgeCabinet.Dispose();
                        }*/
                        if (dc.rfidDev == null) return;
                        dc.rfidDev.ReleaseDevice();
                        break;
                }

            }
        }
        private static void ConnectAll(object obj)
        {
            if (obj == null) return;
            LiveDataForm ldf = (LiveDataForm)obj;
            try
            {

                if (ldf._bClosing) return;
                if (ldf._localDeviceArray != null)
                {
                    bool allConnected = true;
                    foreach (deviceClass dc in ldf._localDeviceArray)
                    {
                        if ((dc.rfidDev.ConnectionStatus != ConnectionStatus.CS_Connected) && (dc.infoDev.enabled == 1))
                        {
                            allConnected = false;
                            break;
                        }
                    }
                    if (!allConnected)
                        ldf.ConnectLocalDevice();
                }

                if (ldf._networkDeviceArray != null)
                {
                    bool allConnected = true;
                    foreach (deviceClass dc in ldf._networkDeviceArray)
                    {
                        if (dc.netConnectionStatus != ConnectionStatus.CS_Connected)
                        {
                            allConnected = false;
                            break;
                        }
                    }
                    if (!allConnected)
                        ldf.ConnectNetworkDevice();
                }
            }
            catch
            {

            }
            finally
            {
                ldf.InConnection = false;
            }
        }
        private void ClockFridgeTimer_Tick(object sender, EventArgs eArgs)
        {
            if (sender == ClockFridge)
            {               
                ClockFridge.Stop();             
                if (_localDeviceArray != null)
                {
               
                    foreach (deviceClass dc in _localDeviceArray)
                    {
                        if ((dc.infoDev.deviceType == DeviceType.DT_SFR) || (dc.infoDev.deviceType == DeviceType.DT_SBF))
                        {
                            if ((dc.myFridgeCabinet != null) && (dc.rfidDev.DeviceStatus == DeviceStatus.DS_Ready) && (dc.infoDev.bLocal == 1))
                            {
                                if (dc.myFridgeCabinet.GetPreviousTempInfo != null)
                                {
                                    lock (_locker)
                                    {
                                        if (_db.StoreTempFridge(dc.infoDev.SerialRFID, dc.myFridgeCabinet.GetPreviousTempInfo))
                                            dc.myFridgeCabinet.GetPreviousTempInfo = null;
                                    }
                                }
                                _bComeFromFridge = true;
                                UpdateTreeView();
                                DateTime dt = DateTime.Now;
                                if (dt.Second < 59)
                                    ClockFridge.Interval = ((59 - dt.Second) * 1000);
                                else
                                    ClockFridge.Interval = 60000;
                                ClockFridge.Start();
                               
                            }
                        }
                    }
                   
                }
                if (_networkDeviceArray != null)
                {   ////
                    for (int i = 0; i < _networkDeviceArray.Length; i++)
                    {
                        //device already connected but might be in use
                        if ((_networkDeviceArray[i].infoDev.deviceType == DeviceType.DT_SFR) || (_networkDeviceArray[i].infoDev.deviceType == DeviceType.DT_SBF))
                        {
                            if (_networkDeviceArray[i].netConnectionStatus != ConnectionStatus.CS_Connected)
                                continue;

                            // get previous temp on first pass

                            _networkDeviceArray[i].netFridgeTemperatureProcessDone = true;
                            if (!_networkDeviceArray[i].netFridgeTemperatureProcessDone)
                            {
                                new Thread(() =>
                                {
                                    _networkDeviceArray[i].netFridgeTemperatureProcessDone = true;
                                    ProcessFullTemp(_networkDeviceArray[i].infoDev.SerialRFID);
                                   
                                }).Start();

                            }  

                            /*****************/

                            if (tcpUtils.PingAddress(_networkDeviceArray[i].infoDev.IP_Server, 1000))
                            {
                                    TcpIpClient tcp2 = DeviceClientTcp[_networkDeviceArray[i].infoDev.IP_Server];
                                    if (tcp2.pingServer(_networkDeviceArray[i].infoDev.IP_Server, _networkDeviceArray[i].infoDev.Port_Server) == TcpIpClient.RetCode.RC_Succeed)
                                {
                                    _networkDeviceArray[i].netConnectionStatus = ConnectionStatus.CS_Connected;
                                    string status;
                                        if (tcp2.getStatus(_networkDeviceArray[i].infoDev.IP_Server, _networkDeviceArray[i].infoDev.Port_Server, _networkDeviceArray[i].infoDev.SerialRFID, out  status) == TcpIpClient.RetCode.RC_Succeed)
                                    {
                                        _networkDeviceArray[i].netDeviceStatus = (DeviceStatus)Enum.Parse(typeof(DeviceStatus), status, true);
                                        //if (networkDeviceArray[i].netDeviceStatus == DeviceStatus.DS_Ready)
                                        //{
                                            tempInfo ti = null;
                                            if (tcp2.getFridgeCurrentTemp(_networkDeviceArray[i].infoDev.IP_Server, _networkDeviceArray[i].infoDev.Port_Server, _networkDeviceArray[i].infoDev.SerialRFID, out ti) == TcpIpClient.RetCode.RC_Succeed)
                                            {
                                                if (ti != null)
                                                {
                                                    //ProcessFridgeRealTime(ti, networkDeviceArray[i].infoDev.SerialRFID);
                                                    _networkDeviceArray[i].previousTemp = _networkDeviceArray[i].currentTemp;
                                                    _networkDeviceArray[i].currentTemp = ti;
                                                    UpdateTemp(i, _networkDeviceArray[i].infoDev.SerialRFID);

                                              

                                                    // defrost active block alarm 10 minutes
                                                    //if (ti.DefrostActive) dateFridgeALarmBlocked = DateTime.Now.AddMinutes(10);

                                                    if (_bUseAlarm)
                                                    {
                                                        if ((ti.lastTempValue > MaxTempFridgeValue) && (DateTime.Now > _dateFridgeALarmBlocked))
                                                        {                                                           
                                                            // realarm in 15 minutes
                                                            _dateFridgeALarmBlocked = DateTime.Now.AddMinutes(15);
                                                            if (_alertFridge != null)
                                                            {
                                                                _db.storeAlert(AlertType.AT_Max_Fridge_Temp, _networkDeviceArray[i].infoDev, null, null);
                                                                AlertMgtClass.treatAlert(AlertType.AT_Max_Fridge_Temp, _networkDeviceArray[i].infoDev, null, null, "T° : " + ti.lastTempValue + " / Alarm T° at : " + MaxTempFridgeValue, BShowAlert);
                                                            }
                                                        }
                                                    }
                                                }
                                                _bComeFromFridge = true;
                                                UpdateTreeView();
                                                DateTime dt = DateTime.Now;

                                                if ((dt.Minute == 0) || (dt.Minute == 1)) // changement heure
                                               // if (true)
                                                {
                                                    //_db.StoreTempFridge(_networkDeviceArray[i].infoDev.SerialRFID, _networkDeviceArray[i].previousTemp);
                                                    tempInfo lastTemp = _db.GetLastFridgeTemp(_networkDeviceArray[i].infoDev.SerialRFID);
                                                    tempInfo lastCurrentTemp = null;
                                                    tcp2.getFridgeCurrentTemp(_networkDeviceArray[i].infoDev.IP_Server, _networkDeviceArray[i].infoDev.Port_Server, _networkDeviceArray[i].infoDev.SerialRFID, out lastCurrentTemp);
                                                    if (lastTemp != null)
                                                    {
                                                        tempInfo[] tempArray = null;
                                                        tcp2.getFridgeTempFromDate(_networkDeviceArray[i].infoDev.IP_Server, _networkDeviceArray[i].infoDev.Port_Server, _networkDeviceArray[i].infoDev.SerialRFID, lastTemp.CreationDate.ToLocalTime(), out tempArray);

                                                        if (tempArray != null)
                                                        {
                                                            foreach (tempInfo tmp in tempArray)
                                                            {
                                                                if ((tmp.CreationDate > lastTemp.CreationDate) && (tmp.nbValueTemp == 60))
                                                                    _db.StoreTempFridge(_networkDeviceArray[i].infoDev.SerialRFID, tmp);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        tempInfo[] tempArray = null;
                                                        tcp2.getFridgeTempFromDate(_networkDeviceArray[i].infoDev.IP_Server, _networkDeviceArray[i].infoDev.Port_Server, _networkDeviceArray[i].infoDev.SerialRFID, DateTime.Now.AddHours(-4), out tempArray);

                                                        if (tempArray != null)
                                                        {
                                                            foreach (tempInfo tmp in tempArray)
                                                            {
                                                                if ((tmp.CreationDate > DateTime.Now.AddHours(-4)) && (tmp.nbValueTemp == 60))
                                                                    _db.StoreTempFridge(_networkDeviceArray[i].infoDev.SerialRFID, tmp);
                                                            }
                                                        }
                                                    }
                                                }

                                                if (dt.Second < 59)
                                                    ClockFridge.Interval = ((59 - dt.Second) * 1000);
                                                else
                                                    ClockFridge.Interval = 60000;
                                                ClockFridge.Start();
                                        }
                                        //}
                                    }
                                }
                            }
                        }
                    }                    
                }
              
            }

        }
        private void timerStartup_Tick(object sender, EventArgs e)
        {
            timerStartup.Interval = 10000;
            timerStartup.Enabled = true;
            timerStartup.Start();
            if (_bClosing) return;
            InScan = false;
            if (_localDeviceArray != null)
            {
                foreach (deviceClass dc in _localDeviceArray)
                {
                    if (dc.rfidDev.DeviceStatus == DeviceStatus.DS_InScan)
                    {
                        InScan = true;
                        break;
                    }
                }
            }

            if ((!InConnection) && (!InScan))
            {
                InConnection = true;
                ThreadPool.SetMaxThreads(16, 16);
                ThreadPool.QueueUserWorkItem(ConnectAll, this);
                //ConnectAll(this);
                timerNetworkRefreshScan.Enabled = true;
            }
            

        }
        private void TreatNetwork()
        {
                bool isChanged = false;
                if (_networkDeviceArray != null)
                {
                    for (int i = 0; i < _networkDeviceArray.Length; i++)
                    {
                        try
                        {
                            //device already connected but might be in use
                            if (_networkDeviceArray[i].netConnectionStatus != ConnectionStatus.CS_Connected)
                                continue;

                            if (tcpUtils.PingAddress(_networkDeviceArray[i].infoDev.IP_Server, 1000))
                            {
                                TcpIpClient tcp = DeviceClientTcp[_networkDeviceArray[i].infoDev.IP_Server];
                                if (tcp.pingServer(_networkDeviceArray[i].infoDev.IP_Server, _networkDeviceArray[i].infoDev.Port_Server) == TcpIpClient.RetCode.RC_Succeed)
                                {

                                    _networkDeviceArray[i].netConnectionStatus = ConnectionStatus.CS_Connected;
                                    string status;
                                    if (tcp.getStatus(_networkDeviceArray[i].infoDev.IP_Server, _networkDeviceArray[i].infoDev.Port_Server, _networkDeviceArray[i].infoDev.SerialRFID, out  status) == TcpIpClient.RetCode.RC_Succeed)
                                    {
                                        if (status.Equals("READER_NOT_EXIST"))
                                        {
                                            MessageBox.Show(string.Format(ResStrings.str_badIP, _networkDeviceArray[i].infoDev.SerialRFID), ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            continue; // KB changement adresse IP sur device existant precedement
                                        }
                                        _networkDeviceArray[i].netDeviceStatus = (DeviceStatus)Enum.Parse(typeof(DeviceStatus), status, true);

                                        if (_networkDeviceArray[i].netDeviceStatus == DeviceStatus.DS_DoorOpen)
                                        {
                                            // Door open - disable alarm temp fridge 10 minutes
                                            _dateFridgeALarmBlocked = DateTime.Now.AddMinutes(10);
                                            if (_bUseAlarm)
                                            {
                                                if (_alertDoors != null)
                                                {
                                                    if (_networkDeviceArray[i].firstDoorOpenDetected)
                                                    {
                                                        _networkDeviceArray[i].lastDoorOpen = DateTime.Now;
                                                        _networkDeviceArray[i].firstDoorOpenDetected = false;
                                                    }
                                                    if (DateTime.Now > _networkDeviceArray[i].lastDoorOpen.AddSeconds(30.0))
                                                    {
                                                        UserClassTemplate tmpUtc = new UserClassTemplate();
                                                        tmpUtc.firstName = ResStrings.str_Unknown;
                                                        tmpUtc.lastName = ResStrings.str_User;
                                                        _db.storeAlert(AlertType.AT_Door_Open_Too_Long, _networkDeviceArray[i].infoDev, tmpUtc, null);
                                                        AlertMgtClass.treatAlert(AlertType.AT_Door_Open_Too_Long, _networkDeviceArray[i].infoDev, tmpUtc, null, null, BShowAlert);
                                                        _networkDeviceArray[i].lastDoorOpen = DateTime.Now.AddYears(1); // to stop process of mail until door close
                                                    }
                                                }
                                            }
                                        }

                                        //Check if use TcpNotification - if yes continue  - check a counter - if 10 times let classic to poll - if inscan reset cpt
                                        if (_networkDeviceArray[i].bUseTcpNotification)
                                        {
                                            _networkDeviceArray[i].cptPoll++;
                                            if (_networkDeviceArray[i].netDeviceStatus == DeviceStatus.DS_InScan)
                                                _networkDeviceArray[i].cptPoll = 0;

                                            if (_networkDeviceArray[i].cptPoll < 4)
                                                continue;
                                            _networkDeviceArray[i].cptPoll = 0;
                                        }


                                        DateTime lastScanIp = DateTime.MaxValue;

                                        if (_networkDeviceArray[i].netDeviceStatus == DeviceStatus.DS_InScan)
                                        {
                                            Invoke((MethodInvoker) delegate
                                            {
                                                if (!_sip.Visible)
                                                {
                                                    _sip.SetInfo(
                                                        _networkDeviceArray[_selectedReader - _nbLocalDevice].infoDev);
                                                    _sip.Show();
                                                }

                                                if (msgBox != null)
                                                {
                                                    if (msgBox.name == _networkDeviceArray[_selectedReader - _nbLocalDevice].infoDev.DeviceName )
                                                        msgBox.Close();
                                                }
                                            
                                            });
                                        }
                                        else
                                        {
                                            Invoke((MethodInvoker)delegate
                                            {
                                                if ((_sip.Visible) && (_sip.di.DeviceName == _networkDeviceArray[_selectedReader - _nbLocalDevice].infoDev.DeviceName ))
                                                {
                                                    
                                                    _sip.Hide();
                                                }
                                            });

                                        }

                                        if (_networkDeviceArray[i].netDeviceStatus == DeviceStatus.DS_Ready)
                                        {
                                            InScan = false;
                                                
            
                                                _networkDeviceArray[i].firstDoorOpenDetected = true;



                                                //if (_networkDeviceArray[i].infoDev.deviceType != DeviceType.DT_SBR)
                                                //{

                                                        tcp.getLastDateScan(_networkDeviceArray[i].infoDev.IP_Server, _networkDeviceArray[i].infoDev.Port_Server, _networkDeviceArray[i].infoDev.SerialRFID, out lastScanIp);
                                                        if (lastScanIp > _networkDeviceArray[i].lastProcessInventoryGmtDate)
                                                        {
                                                                Debug.WriteLine("Get from treatNetWork");
                                                                InventoryData[] inventoryArray = null;
                                                                TcpIpClient.RetCode ret = tcp.getScanFromDateWithDB(_networkDeviceArray[i].infoDev.IP_Server, _networkDeviceArray[i].infoDev.Port_Server, _networkDeviceArray[i].infoDev.SerialRFID, _networkDeviceArray[i].currentInventory.eventDate.ToUniversalTime(), out inventoryArray);
                                                                if (ret == TcpIpClient.RetCode.RC_Succeed)
                                                                {
                                                                    if (inventoryArray != null)
                                                                    {
                                                                        foreach (InventoryData inv in inventoryArray)
                                                                        {
                                                                            DateTime dtUtc = DateTime.SpecifyKind(inv.eventDate, DateTimeKind.Utc);
                                                                            inv.eventDate = dtUtc.ToLocalTime();
                                                                            StoreNetworkinventory(_networkDeviceArray[i], inv);
                                                                        }

                                                                      Invoke((MethodInvoker)delegate
                                                                      {
                                                                          if ((_sip.Visible) && (_sip.di.DeviceName == _networkDeviceArray[_selectedReader - _nbLocalDevice].infoDev.DeviceName ))
                                                                          {
                                                    
                                                                              _sip.Hide();
                                                                          }
                                                                      });
                                                                   
                                                                    isChanged = true;

                                                                    //alert Remove time exceed
                                                                    bool bReaderForRemovetoLong = false;
                                                                    if (_removedReaderDevice.Contains(_networkDeviceArray[i].infoDev.SerialRFID)) bReaderForRemovetoLong = true;

                                                                    if (_bUseAlarm && _networkDeviceArray[i].currentInventory.nbTagRemoved > 0 && _maxTimeBeforeAlertRemoved > 0 && bReaderForRemovetoLong)
                                                                    {
                                                                        TimeSpan ts = new TimeSpan(0, _maxTimeBeforeAlertRemoved, 0);
                                                                        DateTime dateToTest = DateTime.Now.Add(ts);
                                                                        _removedInventoryAlert.Add(dateToTest, _networkDeviceArray[i]);
                                                                    }
                                                                    #region BloodPatient

                                                                    if ((!_selectedPatient.Equals(ResStrings.str_None))
                                                                            && (_networkDeviceArray[i].currentInventory.dtTagRemove.Rows.Count > 0))
                                                                    {
                                                                        if (_alertBloodPatient != null)
                                                                        {
                                                                            string columnPatient = _db.GetColumnNameForAlert(AlertType.AT_Bad_Blood_Patient);
                                                                            if (!string.IsNullOrEmpty(columnPatient))
                                                                            {

                                                                                ArrayList badBlood = new ArrayList();
                                                                                foreach (DataRow oRow in _networkDeviceArray[i].currentInventory.dtTagRemove.Rows)
                                                                                {
                                                                                    if (oRow[columnPatient] != DBNull.Value)
                                                                                    {
                                                                                        if (!_selectedPatient.Equals((string)oRow[columnPatient]))
                                                                                            badBlood.Add(oRow);
                                                                                    }
                                                                                }

                                                                                if (badBlood.Count > 0)
                                                                                {
                                                                                    string spData = string.Empty;
                                                                                    spData += _selectedPatient;
                                                                                    foreach (DataRow oRow in badBlood)
                                                                                    {
                                                                                        string rowValue = string.Empty;
                                                                                        for (int x = 0; x < oRow.ItemArray.Length; x++)
                                                                                        {
                                                                                            if (rowValue.Length > 0) rowValue += " - ";
                                                                                            rowValue += oRow[x].ToString();
                                                                                        }

                                                                                        if (spData.Length > 0) spData += ";";
                                                                                        spData += rowValue;
                                                                                    }
                                                                                    Invoke((MethodInvoker)delegate { new BadStopPatient().ShowDialog(); });
                                                                                    _db.storeAlert(AlertType.AT_Bad_Blood_Patient, _networkDeviceArray[i].infoDev, null, spData);
                                                                                    AlertMgtClass.treatAlert(AlertType.AT_Bad_Blood_Patient, _networkDeviceArray[i].infoDev, null, _networkDeviceArray[i].currentInventory, spData, BShowAlert);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    #endregion

                                                                    // alert remove nb item
                                                                    if (_bUseAlarm && _networkDeviceArray[i].currentInventory.bUserScan)
                                                                    {

                                                                        if (_alertItems != null)
                                                                        {

                                                                            int nbMaxItem = _db.getUserMaxRemovedItem(_networkDeviceArray[i].currentInventory.userFirstName, _networkDeviceArray[i].currentInventory.userLastName);
                                                                            if (nbMaxItem > 0)
                                                                            {
                                                                                if (_networkDeviceArray[i].currentInventory.nbTagRemoved > nbMaxItem)
                                                                                {
                                                                                    UserClassTemplate tmpUtc = new UserClassTemplate();
                                                                                    tmpUtc.firstName = _firstName;
                                                                                    tmpUtc.lastName = _lastName;
                                                                                    string spData = _networkDeviceArray[i].currentInventory.nbTagRemoved + " item(s) on " + nbMaxItem + " allowed";
                                                                                    _db.storeAlert(AlertType.AT_Remove_Too_Many_Items, _networkDeviceArray[i].infoDev, tmpUtc, spData);
                                                                                    AlertMgtClass.treatAlert(AlertType.AT_Remove_Too_Many_Items, _networkDeviceArray[i].infoDev, tmpUtc, _networkDeviceArray[i].currentInventory, spData, BShowAlert);
                                                                                }
                                                                            }
                                                                        }
                                                                        if (_alertValues != null)
                                                                        {
                                                                            string columnValueName = _db.GetColumnNameForAlert(AlertType.AT_Limit_Value_Exceed);
                                                                            if (!string.IsNullOrEmpty(columnValueName))
                                                                            {
                                                                                double userLimit = _db.getUserMaxRemoveValue(_firstName, _lastName);
                                                                                if (userLimit > 0)
                                                                                {
                                                                                    double sumValue = ProcessValueAlert(_networkDeviceArray[i].currentInventory.dtTagRemove, columnValueName);

                                                                                    if (sumValue > userLimit)
                                                                                    {
                                                                                        UserClassTemplate tmpUtc = new UserClassTemplate();
                                                                                        tmpUtc.firstName = _firstName;
                                                                                        tmpUtc.lastName = _lastName;
                                                                                        string spData = string.Format(ResStrings.str_0_on_1_allowed, sumValue, userLimit);
                                                                                        _db.storeAlert(AlertType.AT_Limit_Value_Exceed, _networkDeviceArray[i].infoDev, tmpUtc, spData);
                                                                                        AlertMgtClass.treatAlert(AlertType.AT_Limit_Value_Exceed, _networkDeviceArray[i].infoDev, tmpUtc, _networkDeviceArray[i].currentInventory, spData, BShowAlert);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        if (_alertStock != null)
                                                                        {
                                                                            string colAlert = _db.GetColumnNameForAlert(AlertType.AT_Stock_Limit);
                                                                            if (!string.IsNullOrEmpty(colAlert))
                                                                            {
                                                                                int nbMin = 0;
                                                                                int.TryParse(_alertStock.alertData, out nbMin);

                                                                                Dictionary<string, int> stockProduct = new Dictionary<string, int>();
                                                                                foreach (DataRow oRow in _networkDeviceArray[i].currentInventory.dtTagAll.Rows)
                                                                                {

                                                                                    string productName = (string)oRow[colAlert];

                                                                                    if (string.IsNullOrEmpty(productName)) continue;
                                                                                    if (productName.Equals(ResStrings.str_Unreferenced)) continue;
                                                                                    if (stockProduct.ContainsKey(productName))
                                                                                        stockProduct[productName]++;
                                                                                    else
                                                                                        stockProduct.Add(productName, 1);
                                                                                }
                                                                                foreach (DataRow oRow in _networkDeviceArray[i].currentInventory.dtTagRemove.Rows)
                                                                                {

                                                                                    string productName = (string)oRow[colAlert];
                                                                                    if (string.IsNullOrEmpty(productName)) continue;
                                                                                    if (productName.Equals(ResStrings.str_Unreferenced)) continue;
                                                                                    if (!stockProduct.ContainsKey(productName))
                                                                                        stockProduct.Add(productName, 0);
                                                                                }

                                                                                string spData = string.Empty;
                                                                                foreach (KeyValuePair<string, int> pair in stockProduct)
                                                                                {
                                                                                    if (pair.Value <= nbMin)
                                                                                    {
                                                                                        if (spData.Length > 0) spData += ";";
                                                                                        spData += pair.Key + ";" + pair.Value;
                                                                                    }
                                                                                }

                                                                                if (!string.IsNullOrEmpty(spData))
                                                                                {
                                                                                    _db.storeAlert(AlertType.AT_Stock_Limit, _networkDeviceArray[i].infoDev, null, spData);
                                                                                    AlertMgtClass.treatAlert(AlertType.AT_Stock_Limit, _networkDeviceArray[i].infoDev, null, _networkDeviceArray[i].currentInventory, spData, BShowAlert);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        
                                                    //}
                                                }
                                           
                                        }
                                    }
                                }
                                else
                                {
                                    _networkDeviceArray[i].netConnectionStatus = ConnectionStatus.CS_Disconnected;
                                }
                            }
                            else
                            {
                                _networkDeviceArray[i].netConnectionStatus = ConnectionStatus.CS_Disconnected;
                            }
                         }
                    catch
                    {
                        // No exception fired as can occurs when ethernet device disconnect
               
                    }
                    }
                }
                if ((isChanged) && (!_bClosing))
                {
                    RefreshInventory();

                }
           
        }
        private void timerNetworkREfresh_Tick(object sender, EventArgs e)
        {
           
            if (_tcpIpServer == null) return;
            if (_networkDeviceArray == null) return;
            if (_bClosing) return;
            
            InScan = false;
            if (_localDeviceArray != null)
            {
                foreach (deviceClass dc in _localDeviceArray)
                {
                    if (dc.rfidDev.DeviceStatus == DeviceStatus.DS_InScan)
                    {
                        InScan = true;
                        break;
                    }
                }
            }
            if (InScan) return;
            timerNetworkRefreshScan.Interval = _timeTcpToRefresh;
            timerNetworkRefreshScan.Stop();
            
            lock (_locker)
            {
                if (_networkDeviceArray.Length > 0)
                {
                    TreatNetwork();

                    for (int i = 0; i < _networkDeviceArray.Length; i++)
                    {
                        if (_networkDeviceArray[i].netConnectionStatus == ConnectionStatus.CS_Connected)
                        {
                            if (treeViewDevice != null)
                                treeViewDevice.Nodes[_nbLocalDevice + i].ImageIndex = treeViewDevice.Nodes[_nbLocalDevice + i].SelectedImageIndex = 1;
                        }
                        else
                        {
                            if (treeViewDevice != null)
                                treeViewDevice.Nodes[_nbLocalDevice + i].ImageIndex = treeViewDevice.Nodes[_nbLocalDevice + i].SelectedImageIndex = 2;
                        }

                        if (_networkDeviceArray[i].netDeviceStatus == DeviceStatus.DS_InScan)
                        {
                            if (!timerRefreshScan.Enabled)
                            {
                                timerRefreshScan.Interval = 500;
                                timerRefreshScan.Enabled = true;
                            }
                        }

                        int i1 = i;
                        Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[i1 + _nbLocalDevice].Nodes[4].Text = string.Format(ResStrings.str_Connection_Status_, getEnumDesc.GetEnumDescription(_networkDeviceArray[i1].netConnectionStatus)); });
                        Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[i1 + _nbLocalDevice].Nodes[5].Text = string.Format(ResStrings.str_Statut_, getEnumDesc.GetEnumDescription(_networkDeviceArray[i1].netDeviceStatus)); });
                        Invoke((MethodInvoker)delegate { treeViewDevice.Refresh(); });
                    }
                }
            }

            timerNetworkRefreshScan.Start();
        }

        private double  previousTempValue = -100.0;
        private double  currentTempValue = 0.0;


        private void timerRestart_Tick(object sender, EventArgs e)
        {

            //add temp notification
            
            if ((_localDeviceArray != null) &&  (_bUseTcpNotification))
            {
                if (_localDeviceArray[0] != null)
                {
                    if ((_localDeviceArray[0].infoDev.deviceType == DeviceType.DT_SBF) || (_localDeviceArray[0].infoDev.deviceType == DeviceType.DT_SFR)) //C'est un fridge
                    {
                        if (_localDeviceArray[0].myFridgeCabinet != null)
                        {
                            if (_localDeviceArray[0].myFridgeCabinet.GetTempInfo != null)
                            {
                                currentTempValue = _localDeviceArray[0].myFridgeCabinet.GetTempInfo.lastTempValue;
                                if (!currentTempValue.Equals(previousTempValue))
                                {
                                    if (_bUseTcpNotification)
                                    {
                                        if (_tcpNotificationThreadInfo != null)
                                        {
                                            if (tcpUtils.PingAddress(_tcpNotificationThreadInfo.HostIp, 2000))
                                            {
                                                previousTempValue = currentTempValue;
                                                _tcpNotificationThreadInfo.SetParam(TcpThreadHandle.TcpNotificationType.TempChanged, _localDeviceArray[0].infoDev.SerialRFID, _tcpIpServer.ServerIP, _tcpIpServer.Port, 0, currentTempValue);
                                                _tcpNotificationThreadInfo.ThreadTcpLoop();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (File.Exists(@"C:\temp\renewFP.txt"))
            {
                if (_localDeviceArray != null)
                {
                    deviceClass cd = _localDeviceArray[0];
                    File.Delete(@"C:\temp\renewFP.txt");
                    cd.rfidDev.RenewFP(false, true);
                    _tcpIpServer.NeedRefreshUser = true;
                }
            }
            if (_tcpIpServer.NeedRefreshFP)
            {
                _tcpIpServer.NeedRefreshFP = false;
                RenewFp(true);
            }
            if (_tcpIpServer.requestRestart)
            {
                timerRestart.Enabled = false;
                BRestart = true;
                Close();
            }
            if (_tcpIpServer.NeedRefreshTree)
            {
                _tcpIpServer.NeedRefreshTree = false;              
                UpdateTreeView();
            }
            if (_tcpIpServer.NeedRefreshSQL)
            {
                GetExportSqlInfo();
                GetTcpNotificationInfo();
                _tcpIpServer.NeedRefreshSQL = false;
            }
            if (_tcpIpServer.NeedRefreshUser)
            {
                _tcpIpServer.NeedRefreshUser = false;
                if (_localDeviceArray == null) return;

                foreach (deviceClass dc in _localDeviceArray)
                {
                    //UserClassTemplate[] UsersAllowed = db.RecoverAllowedUser(dc.infoDev.SerialRFID);
                    DeviceGrant[] usersAllowed = _db.RecoverAllowedUser(dc.infoDev.SerialRFID);
                    if (usersAllowed != null)
                    {
                        ArrayList listBadge;
                        switch (dc.infoDev.deviceType)
                        {
                            case DeviceType.DT_DSB:
                            case DeviceType.DT_JSC:
                                if (dc.rfidDev.get_FP_Master != null)
                                    dc.rfidDev.LoadFPTemplateFromDB(dc.rfidDev.get_FP_Master, UserGrant.UG_MASTER_AND_SLAVE);
                                break;

                            case DeviceType.DT_SAS:
                            case DeviceType.DT_MSR:

                                if (dc.rfidDev.get_FP_Master != null)
                                    dc.rfidDev.LoadFPTemplateFromDB(dc.rfidDev.get_FP_Master, UserGrant.UG_MASTER);
                                if (dc.rfidDev.get_FP_Slave != null)
                                    dc.rfidDev.LoadFPTemplateFromDB(dc.rfidDev.get_FP_Slave, UserGrant.UG_SLAVE);


                            if (!string.IsNullOrEmpty(dc.infoDev.comMasterReader))
                            {                               

                                ArrayList listBadge0 = new ArrayList();                              
                                DeviceGrant[] userForMiniSas = _db.RecoverAllowedUser(dc.infoDev.SerialRFID);
                                if (userForMiniSas != null)
                                {
                                    foreach (DeviceGrant uct in userForMiniSas)
                                    {
                                        if ((uct.userGrant == UserGrant.UG_MASTER_AND_SLAVE) || (uct.userGrant == UserGrant.UG_MASTER))
                                        {
                                            if (!string.IsNullOrEmpty(uct.user.BadgeReaderID))
                                                listBadge0.Add(uct.user.BadgeReaderID);
                                        }
                                    }
                                    if (dc.masterBadgerReader != null)
                                        dc.masterBadgerReader.loadBadge(listBadge0);
                                }

                            }
                            if (!string.IsNullOrEmpty(dc.infoDev.comSlaveReader))
                            {                             

                                ArrayList listBadge1 = new ArrayList();
                                
                                DeviceGrant[] userForMiniSas = _db.RecoverAllowedUser(dc.infoDev.SerialRFID);
                                if (userForMiniSas != null)
                                {                                    
                                    foreach (DeviceGrant uct in userForMiniSas)
                                    {
                                        if ((uct.userGrant == UserGrant.UG_MASTER_AND_SLAVE) || (uct.userGrant == UserGrant.UG_SLAVE))
                                        {
                                            if (!string.IsNullOrEmpty(uct.user.BadgeReaderID))
                                                listBadge1.Add(uct.user.BadgeReaderID);
                                        }
                                    }
                                    if (dc.slaveBadgerReader != null)
                                    dc.slaveBadgerReader.loadBadge(listBadge1);
                                }
                            }

                                break;
                            case DeviceType.DT_SMC:
                                if (dc.myMedicalCabinet == null) return;
                                listBadge = new ArrayList();
                               
                                foreach (DeviceGrant uct in usersAllowed)
                                {
                                    if (!string.IsNullOrEmpty(uct.user.BadgeReaderID))
                                        listBadge.Add(uct.user.BadgeReaderID);
                                }

                                dc.myMedicalCabinet.LoadBadges(listBadge);

                                if (dc.rfidDev.get_FP_Master != null)
                                    dc.rfidDev.LoadFPTemplateFromDB(dc.rfidDev.get_FP_Master, UserGrant.UG_MASTER);

                                break;
                            case DeviceType.DT_SFR:
                            case DeviceType.DT_SBF:
                                if (dc.myFridgeCabinet == null) return;
                                listBadge = new ArrayList();
                                
                                foreach (DeviceGrant uct in usersAllowed)
                                {
                                    if (!string.IsNullOrEmpty(uct.user.BadgeReaderID))
                                        listBadge.Add(uct.user.BadgeReaderID);
                                }
                                dc.myFridgeCabinet.LoadBadges(listBadge);

                                if (dc.rfidDev.get_FP_Master != null)
                                {
                                      dc.rfidDev.LoadFPTemplateFromDB(dc.rfidDev.get_FP_Master, UserGrant.UG_MASTER_AND_SLAVE);
                                }

                               
                                break;
                        }
                    }
                    else
                    {
                        switch (dc.infoDev.deviceType)
                        {
                            case DeviceType.DT_DSB:
                            case DeviceType.DT_JSC:
                                if (dc.rfidDev.get_FP_Master != null)
                                    dc.rfidDev.ClearFPTemplate(dc.rfidDev.get_FP_Master);
                                break;

                            case DeviceType.DT_SAS:
                            case DeviceType.DT_MSR:

                                if (dc.rfidDev.get_FP_Master != null)
                                    dc.rfidDev.ClearFPTemplate(dc.rfidDev.get_FP_Master);
                                if (dc.rfidDev.get_FP_Slave != null)
                                    dc.rfidDev.ClearFPTemplate(dc.rfidDev.get_FP_Slave);
                                break;
                            case DeviceType.DT_SMC:
                                if (dc.myMedicalCabinet == null) return;
                                    dc.myMedicalCabinet.ClearBadgeList();
                                break;
                            case DeviceType.DT_SFR:
                            case DeviceType.DT_SBF:
                                if (dc.myFridgeCabinet == null) return;
                                dc.myFridgeCabinet.ClearBadgeList();

                                if (dc.rfidDev.get_FP_Master != null)
                                {
                                  
                                     dc.rfidDev.ClearFPTemplate(dc.rfidDev.get_FP_Master);
                                }
                                break;
                        }
                    }
                }
                UserClassTemplate[] users = _db.RecoverUser();
                if (users != null)
                {
                    _localUserArray = new UserClassTemplate[users.Length];
                    users.CopyTo(_localUserArray, 0);
                }
                else
                    _localUserArray = null;
            }

            if (bUseCvsExchange)
            {
                if (csvWatcher == null) return;
                
                if (Directory.Exists(pathCsvFolder))
                {
                    if (bwasConnected)
                    {
                        if (File.Exists(pathCsvInventory))
                        {
                            File.Delete(pathCsvInventory);
                            ReportCsvError("Error Network : Deleted startID");
                        }

                        if (File.Exists(pathCsvLed))
                        {
                            File.Delete(pathCsvLed);
                            ReportCsvError("Error Network : Deleted ledID");
                        }
                        bwasConnected = false;
                    }
                    csvWatcher.EnableRaisingEvents = true;
                }
                else
                {
                    csvWatcher.EnableRaisingEvents = false;
                    bwasConnected = true;
                }
            }

            //Eric Test KSA Fridge
            if (File.Exists(@"C:\temp\AutoScan.txt"))
            {
                string readStr = File.ReadAllText(@"C:\temp\AutoScan.txt");
                int elapsedTime = -1;

                if (int.TryParse(readStr, NumberStyles.Number, null, out elapsedTime))
                {

                    if (_localDeviceArray != null)
                    {
                         DateTime lastScan = DateTime.Now.AddDays(1);
                        deviceClass cd = _localDeviceArray[0];
                        if (cd.currentInventory != null)
                           lastScan = cd.currentInventory.eventDate;

                        TimeSpan span = DateTime.Now.ToUniversalTime() -
                                        lastScan.ToUniversalTime().AddSeconds(elapsedTime);

                        if (span.TotalSeconds > 0)
                        {

                            if ((cd.rfidDev != null) && (cd.rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                                (cd.rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
                                toolStripButtonScan_Click(null, null);
                        }
                    }
                }
            }
        }

        private bool bwasConnected = true;

        private void timerRefreshScan_Tick(object sender, EventArgs e)
        {
            
            if (SelectedNetworkDevice == null) return;

            lock (_locker)
            {
                timerRefreshScan.Enabled = false;
                string status;
                int nbTag;
                DeviceStatus ds = DeviceStatus.DS_NotReady;
                TcpIpClient tcp = DeviceClientTcp[SelectedNetworkDevice.infoDev.IP_Server];
                TcpIpClient.RetCode ret = tcp.getStatusWithNumberOfTag(SelectedNetworkDevice.infoDev.IP_Server, SelectedNetworkDevice.infoDev.Port_Server, SelectedNetworkDevice.infoDev.SerialRFID, out status, out nbTag);


                if (ret == TcpIpClient.RetCode.RC_Succeed)
                {
                    ds = (DeviceStatus)Enum.Parse(typeof(DeviceStatus), status);
                    if (ds == DeviceStatus.DS_InScan)
                    {   

                        if (_bFirstInLoop)
                        {
                            listBoxTag.Invoke((MethodInvoker)delegate { listBoxTag.Items.Clear(); });
                            _bFirstInLoop = false;
                        }
                        labelInventoryTagCount.Invoke((MethodInvoker)delegate { 
                                                                                labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags, nbTag);
                                                                                labelInventoryTagCount.Refresh();
                                                                              });
                        //Application.DoEvents();

                    }
                    if (SelectedNetworkDevice != null)
                        SelectedNetworkDevice.netDeviceStatus = ds;
                }

                if (ds == DeviceStatus.DS_InScan)
                    timerRefreshScan.Enabled = true;
                else
                {
                    //Check if use TcpNotification - if yes continue
                    if (SelectedNetworkDevice.bUseTcpNotification) return;

                    _bFirstInLoop = true;
                    ret = tcp.getStatus(SelectedNetworkDevice.infoDev.IP_Server, SelectedNetworkDevice.infoDev.Port_Server, SelectedNetworkDevice.infoDev.SerialRFID, out status);
                    if (ret == TcpIpClient.RetCode.RC_Succeed)
                    {
                        ds = (DeviceStatus)Enum.Parse(typeof(DeviceStatus), status);
                        SelectedNetworkDevice.netDeviceStatus = ds;
                        InventoryData invData = null;
                        ret = tcp.requestGetLastScanWithDB(SelectedNetworkDevice.infoDev.IP_Server, SelectedNetworkDevice.infoDev.Port_Server, SelectedNetworkDevice.infoDev.SerialRFID, out invData);                        

                        if ((ret == TcpIpClient.RetCode.RC_Succeed) && (invData != null))
                        {
                                _sip.Hide(); 
                                //Thread.Sleep(50); Application.DoEvents(); Thread.Sleep(50); 
                                DateTime dtUtc = DateTime.SpecifyKind(invData.eventDate, DateTimeKind.Utc);
                                invData.eventDate = dtUtc.ToLocalTime();
                                StoreNetworkinventory(SelectedNetworkDevice, invData);
                                RefreshInventory();
                                Invoke((MethodInvoker)delegate
                                {
                                    timerNetworkRefreshScan.Interval = _timeTcpToRefresh;
                                    timerNetworkRefreshScan.Stop();
                                    timerNetworkRefreshScan.Start();
                                    new ScanFinish().Show();
                                });   
                                
                        }
                    }
                }
            }
        }
        #endregion
        #region inventory
       /* private Hashtable CloneHashTable(Hashtable input)
        {
            Hashtable ret = new Hashtable();

            foreach (DictionaryEntry dictionaryEntry in input)
            {
                if (dictionaryEntry.Value is string)
                {
                    ret.Add(dictionaryEntry.Key, new string(dictionaryEntry.Value.ToString().ToCharArray()));
                }
                else if (dictionaryEntry.Value is Hashtable)
                {
                    ret.Add(dictionaryEntry.Key, CloneHashTable((Hashtable)dictionaryEntry.Value));
                }
                else if (dictionaryEntry.Value is ArrayList)
                {
                    ret.Add(dictionaryEntry.Key, new ArrayList((ArrayList)dictionaryEntry.Value));
                }
            }

            return ret;
        }*/
        private InventoryData RecreateDatatable(InventoryData inv)
        {
            if (inv == null) return null;
             Hashtable ColumnInfo = null;
             MainDBClass db2 = new MainDBClass();

            InventoryData retInv = null;

            if (db2.OpenDB())
            {
                ColumnInfo = db2.GetColumnInfo();
                retInv = new InventoryData(ColumnInfo);
                retInv.BadgeID = inv.BadgeID;
                retInv.IdScanEvent = inv.IdScanEvent;

                if (inv.ListTagWithChannel != null)
                    retInv.ListTagWithChannel = new Hashtable(inv.ListTagWithChannel);
                //cloneHashTable(inv.ListTagWithChannel);

                retInv.accessType = inv.accessType;
                retInv.bUserScan = inv.bUserScan;
                retInv.eventDate = inv.eventDate;
                retInv.idDevice = inv.idDevice;
                retInv.idUser = inv.idUser;
                retInv.listTagAdded = new ArrayList(inv.listTagAdded);
                retInv.listTagRemoved = new ArrayList(inv.listTagRemoved);
                retInv.listTagPresent = new ArrayList(inv.listTagPresent);
                retInv.listTagAll = new ArrayList(inv.listTagAll);

                retInv.nbTagAdded = retInv.listTagAdded.Count;
                retInv.nbTagRemoved = retInv.listTagRemoved.Count;
                retInv.nbTagPresent = retInv.listTagPresent.Count;
                retInv.nbTagAll = retInv.listTagAll.Count;

                retInv.scanStatus = inv.scanStatus;
                retInv.serialNumberDevice = inv.serialNumberDevice;
                retInv.spareData1 = inv.spareData1;
                retInv.spareData2 = retInv.spareData2;

                retInv.userDoor = inv.userDoor;
                retInv.userFirstName = inv.userFirstName;
                retInv.userLastName = inv.userLastName;

                foreach (string uidSdk in retInv.listTagAll)
                {
                    DtAndTagClass dtAndTagAll = new DtAndTagClass(this, retInv.dtTagAll, uidSdk, _db);
                    AddTagToDt(dtAndTagAll);
                }
                foreach (string uidSdk in retInv.listTagAdded)
                {
                    DtAndTagClass dtAndTagAll = new DtAndTagClass(this, retInv.dtTagAdded, uidSdk, _db);
                    AddTagToDt(dtAndTagAll);
                }
                foreach (string uidSdk in retInv.listTagRemoved)
                {
                    DtAndTagClass dtAndTagAll = new DtAndTagClass(this, retInv.dtTagRemove, uidSdk, _db);
                    AddTagToDt(dtAndTagAll);
                }
                foreach (string uidSdk in retInv.listTagPresent)
                {
                    DtAndTagClass dtAndTagAll = new DtAndTagClass(this, retInv.dtTagPresent, uidSdk, _db);
                    AddTagToDt(dtAndTagAll);
                }

                db2.CloseDB();
            }
            
            return retInv;
            }

        public void GetProduct()
        {
            if (_bClosing) return;
            if (_db != null)
            {
                DtProductRef = _db.RecoverAllProduct();

                DtProductToFind = new DataTable();
                for (int i = 0; i < _columnInfo.Count; i++)
                    DtProductToFind.Columns.Add(_columnInfo[i].ToString(), typeof (string));
            }
        }
       
        public string GetSumValue(DataTable dt, string colGroup ,string valColGroup)
        {
            string upStatus = string.Empty;
            dtColumnInfo[] listCol = _db.GetdtColumnInfo();
            if (listCol == null) return ResStrings.str_Error_while_computing_Sum;
            foreach (dtColumnInfo col1 in listCol)
            {
                if (col1.colDoSum != 1) continue;
                if (string.IsNullOrEmpty(col1.colName)) continue;
                double sumValue = ProcessValueAlert(dt, col1.colName, colGroup, valColGroup);
                upStatus += string.Format(ResStrings.str_sum, col1.colName, sumValue.ToString("0.000"));
            }

            return upStatus;
        }
        public string GetSumValue(int selectedDt)
        {
            try
            {
                lock (_locker)
                {
                    MathParser mp = new MathParser();
                    int nIndex = 0;
                    dtColumnInfo[] listCol = _db.GetdtColumnInfo();
                    if (listCol == null) return ResStrings.str_Error_while_computing_Sum;
                    string upStatus = string.Empty;
                    if (_nbLocalDevice == 0)
                    {
                        foreach (dtColumnInfo col in listCol)
                        {
                            if (col.colDoSum != 1) continue;
                            if (string.IsNullOrEmpty(col.colName)) continue;
                            double sumValue = 0.0;
                            switch (selectedDt)
                            {
                                case 0:
                                    sumValue = ProcessValueAlert(_networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.dtTagAll, col.colName);
                                    break;
                                case 1:
                                    sumValue = ProcessValueAlert(_networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.dtTagPresent, col.colName);
                                    break;
                                case 2:
                                    sumValue = ProcessValueAlert(_networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.dtTagAdded, col.colName);
                                    break;
                                case 3:
                                    sumValue = ProcessValueAlert(_networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.dtTagRemove, col.colName);
                                    break;
                            }

                            upStatus += string.Format(ResStrings.str_GetSumValue, col.colName, sumValue.ToString("0.000"));
                            mp.Parameters.Add((Parameters)nIndex++, (decimal)sumValue);
                        }
                    }
                    else if (_selectedReader > (_nbLocalDevice - 1))
                    {
                        foreach (dtColumnInfo col in listCol)
                        {
                            if (col.colDoSum != 1) continue;
                            if (string.IsNullOrEmpty(col.colName)) continue;
                            double sumValue = 0.0;
                            switch (selectedDt)
                            {
                                case 0:
                                    sumValue = ProcessValueAlert(_networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.dtTagAll, col.colName);
                                    break;
                                case 1:
                                    sumValue = ProcessValueAlert(_networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.dtTagPresent, col.colName);
                                    break;
                                case 2:
                                    sumValue = ProcessValueAlert(_networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.dtTagAdded, col.colName);
                                    break;
                                case 3:
                                    sumValue = ProcessValueAlert(_networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.dtTagRemove, col.colName);
                                    break;
                            }

                            upStatus += string.Format(ResStrings.str_GetSumValue, col.colName, sumValue.ToString("0.000"));

                            mp.Parameters.Add((Parameters)nIndex++, (decimal)sumValue);
                        }
                    }
                    else
                    {
                        foreach (dtColumnInfo col in listCol)
                        {
                            if (col.colDoSum == 1)
                            {
                                if (string.IsNullOrEmpty(col.colName)) continue;
                                double sumValue = 0.0;
                                switch (selectedDt)
                                {
                                    case 0:
                                        sumValue = ProcessValueAlert(_localDeviceArray[_selectedReader].currentInventory.dtTagAll, col.colName);
                                        break;
                                    case 1:
                                        sumValue = ProcessValueAlert(_localDeviceArray[_selectedReader].currentInventory.dtTagPresent, col.colName);
                                        break;
                                    case 2:
                                        sumValue = ProcessValueAlert(_localDeviceArray[_selectedReader].currentInventory.dtTagAdded, col.colName);
                                        break;
                                    case 3:
                                        sumValue = ProcessValueAlert(_localDeviceArray[_selectedReader].currentInventory.dtTagRemove, col.colName);
                                        break;
                                }

                                upStatus += string.Format(ResStrings.str_GetSumValue, col.colName, sumValue.ToString("0.000"));
                                mp.Parameters.Add((Parameters)nIndex++, (decimal)sumValue);
                            }

                        }
                    }

                    if (_formule != null)
                    {
                        if (_formule.Enable == 1)
                        {                            
                            decimal result = mp.Calculate(_formule.Formule);
                            upStatus += _formule.Title + result.ToString("0.000");
                        }
                    }

                    return upStatus;
                }
            }

            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
                return ResStrings.str_Error_while_computing_Sum;
            }
        }

        private void DeleteOldEntry()
        {
            if (_localDeviceArray != null)
            {
                foreach (deviceClass dc in _localDeviceArray)
                {
                    _db.DeleteOldInventory(dc.infoDev.SerialRFID, _nbRecordToKeep);
                }
            }
            if (_networkDeviceArray != null)
            {
                foreach (deviceClass dc in _networkDeviceArray)
                {
                    _db.DeleteOldInventory(dc.infoDev.SerialRFID, _nbRecordToKeep);
                }
            }
        }
        private void RefreshInventory()
        {

            try

            {


                Invoke((MethodInvoker) delegate
                {
                    if (treeViewDevice.SelectedNode != null)
                    {
                        if (treeViewDevice.SelectedNode.Parent != null)
                        {
                            TreeNode parNode = treeViewDevice.SelectedNode.Parent;
                            treeViewDevice.SelectedNode = parNode;
                            return;
                        }
                        _selectedReader = treeViewDevice.SelectedNode.Index;
                    }
                    else
                    {
                        if (treeViewDevice.Nodes.Count <= 0) return;
                        TreeNode parNode = treeViewDevice.Nodes[0];
                        treeViewDevice.SelectedNode = parNode;
                        _selectedReader = 0;
                    }
                });

                if ((_localDeviceArray != null) && (!_bWasInAccumulation) && (!_checkedWaitMode))
                {
                    if ((_selectedReader >= 0) & (_selectedReader < _nbLocalDevice)) //reader local
                    {
                        if (_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SBR)
                        {
                            Invoke((MethodInvoker) delegate { _toolKeepLastScan.Enabled = true; });
                        }
                        else
                        {
                            Invoke((MethodInvoker) delegate { _toolKeepLastScan.Enabled = false; });
                        }

                        if (_localDeviceArray[_selectedReader].currentInventory != null)
                        {
                            lock (_locker)
                            {
                                labelInventoryDate.Invoke(
                                    (MethodInvoker)
                                        delegate
                                        {
                                            labelInventoryDate.Text =
                                                _localDeviceArray[_selectedReader].currentInventory.eventDate
                                                    .ToLocalTime().ToString("G");
                                        });
                                labelInventoryUser.Invoke((MethodInvoker) delegate
                                {
                                    labelInventoryUser.Text = string.Format("{0} {1}",
                                        _localDeviceArray[_selectedReader].currentInventory.userFirstName,
                                        _localDeviceArray[_selectedReader].currentInventory.userLastName);
                                    if (_localDeviceArray[_selectedReader].currentInventory.bUserScan)
                                    {
                                        if (_localDeviceArray[_selectedReader].currentInventory.userDoor ==
                                            DoorInfo.DI_SLAVE_DOOR)
                                            labelInventoryUser.Text += ResStrings.str_SlaveDoor;
                                        else
                                            labelInventoryUser.Text += ResStrings.str_MasterDoor;
                                    }
                                });

                                if (!_bKeepLastScan)
                                {
                                    listBoxTag.Invoke((MethodInvoker) delegate { listBoxTag.Items.Clear(); });

                                    try
                                    {
                                        if (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus ==
                                            DeviceStatus.DS_InScan)
                                        {
                                            foreach (
                                                string strTag in
                                                    _localDeviceArray[_selectedReader].rfidDev.get_RFID_Device
                                                        .ReaderData.strListTag)
                                            {
                                                string tag = strTag;
                                                listBoxTag.Invoke((MethodInvoker) (() => listBoxTag.Items.Add(tag)));
                                            }
                                        }
                                        else
                                        {
                                            foreach (
                                                string strTag in
                                                    _localDeviceArray[_selectedReader].currentInventory.listTagAll)
                                            {
                                                string tag = strTag;
                                                listBoxTag.Invoke((MethodInvoker) (() => listBoxTag.Items.Add(tag)));
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        // do noting - the reader surely not exist
                                    }
                                }

                                // labelInventoryTagCount.Invoke((MethodInvoker)delegate { labelInventoryTagCount.Text = "Tag(s): " + listBoxTag.Items.Count.ToString(); });

                            }

                        }
                        else
                        {
                            labelInventoryDate.Invoke((MethodInvoker) delegate { labelInventoryDate.Text = ""; });
                            labelInventoryUser.Invoke((MethodInvoker) delegate { labelInventoryUser.Text = ""; });
                            listBoxTag.Invoke((MethodInvoker) delegate { listBoxTag.Items.Clear(); });
                            labelInventoryTagCount.Invoke(
                                (MethodInvoker)
                                    delegate { labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags, 0); });
                        }


                    }
                    else // reader network
                    {
                        if (SelectedNetworkDevice == null) return;
                        if (_networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory != null)
                        {
                            labelInventoryDate.Invoke(
                                (MethodInvoker)
                                    (() =>
                                        labelInventoryDate.Text =
                                            _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory
                                                .eventDate.ToLocalTime().ToString("G")));
                            labelInventoryUser.Invoke((MethodInvoker) (() =>
                            {
                                labelInventoryUser.Text = string.Format("{0} {1}",
                                    _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.userFirstName,
                                    _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.userLastName);
                                if (_networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.bUserScan)
                                {
                                    if (
                                        _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.userDoor ==
                                        DoorInfo.DI_SLAVE_DOOR)
                                        labelInventoryUser.Text += ResStrings.str_SlaveDoor;
                                    else
                                        labelInventoryUser.Text += ResStrings.str_MasterDoor;
                                }
                            }));


                            lock (_locker)
                            {
                                if (!_bKeepLastScan)
                                {
                                    listBoxTag.Invoke((MethodInvoker) (() => listBoxTag.Items.Clear()));
                                    foreach (
                                        string strTag in
                                            _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory
                                                .listTagAll)
                                    {
                                        string tag = strTag;
                                        listBoxTag.Invoke((MethodInvoker) (() => listBoxTag.Items.Add(tag)));
                                    }
                                }
                            }

                            //labelInventoryTagCount.Invoke((MethodInvoker)delegate { labelInventoryTagCount.Text = "Tag(s): " + listBoxTag.Items.Count.ToString(); });
                        }
                        else
                        {
                            labelInventoryDate.Invoke((MethodInvoker) (() => labelInventoryDate.Text = ""));
                            labelInventoryUser.Invoke((MethodInvoker) (() => labelInventoryUser.Text = ""));
                            listBoxTag.Invoke((MethodInvoker) (() => listBoxTag.Items.Clear()));
                            labelInventoryTagCount.Invoke(
                                (MethodInvoker)
                                    (() => labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags, 0)));
                        }
                    }
                }
                else if (_networkDeviceArray != null)
                {
                    if (SelectedNetworkDevice == null) return;
                    if (SelectedNetworkDevice.infoDev.deviceType == DeviceType.DT_SBR)
                    {
                        Invoke((MethodInvoker) (() => _toolKeepLastScan.Enabled = true));
                    }
                    else
                    {
                        Invoke((MethodInvoker) (() => _toolKeepLastScan.Enabled = false));
                    }
                    if (_networkDeviceArray[_selectedReader].currentInventory != null)
                    {
                        labelInventoryDate.Invoke(
                            (MethodInvoker)
                                (() =>
                                    labelInventoryDate.Text =
                                        _networkDeviceArray[_selectedReader].currentInventory.eventDate.ToLocalTime()
                                            .ToString("G")));
                        labelInventoryUser.Invoke((MethodInvoker) (() =>
                        {
                            labelInventoryUser.Text = string.Format("{0} {1}",
                                _networkDeviceArray[_selectedReader].currentInventory.userFirstName,
                                _networkDeviceArray[_selectedReader].currentInventory.userLastName);
                            if (_networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.bUserScan)
                            {
                                if (_networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory.userDoor ==
                                    DoorInfo.DI_SLAVE_DOOR)
                                    labelInventoryUser.Text += ResStrings.str_SlaveDoor;
                                else
                                    labelInventoryUser.Text += ResStrings.str_MasterDoor;
                            }
                        }));


                        lock (_locker)
                        {
                            listBoxTag.Invoke((MethodInvoker) (() => listBoxTag.Items.Clear()));
                            foreach (string strTag in _networkDeviceArray[_selectedReader].currentInventory.listTagAll)
                            {
                                string tag = strTag;
                                listBoxTag.Invoke((MethodInvoker) (() => listBoxTag.Items.Add(tag)));
                            }
                        }

                        //labelInventoryTagCount.Invoke((MethodInvoker)delegate { labelInventoryTagCount.Text = "Tag(s): " + listBoxTag.Items.Count.ToString(); });
                    }
                    else
                    {
                        labelInventoryDate.Invoke((MethodInvoker) (() => labelInventoryDate.Text = ""));
                        labelInventoryUser.Invoke((MethodInvoker) (() => labelInventoryUser.Text = ""));
                        listBoxTag.Invoke((MethodInvoker) (() => listBoxTag.Items.Clear()));
                        labelInventoryTagCount.Invoke(
                            (MethodInvoker) (() => labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags, 0)));
                    }
                }

                timerObjectList.Start();
            }
            catch (Exception exp)
            {
                string str = exp.Message;
            }

        }
        private void AddTagToDt(object obj)
        {
           
            try
            {
                DtAndTagClass clToProcess = (DtAndTagClass)obj;
                
                // Mandaatory to let TagUID here as column Product is tagUID and other are columnInfo[0]

                string escapedTagUid = clToProcess.TagUid.Replace("'", "''"); // escape ' for DataTable Select() filter
                DtProductRef.CaseSensitive = true;
                DataRow[] productInfo = DtProductRef.Select("["+ ResStrings.str_TagUID + "]= '" + escapedTagUid + "'");

                if (productInfo.Length == 1)
                {
                    object[] param = new object[_columnInfo.Count];
                    for (int i = 0; i < _columnInfo.Count; i++)
                        param[i] = productInfo[0].ItemArray[i];

                    clToProcess.Dt.Rows.Add(param);
                }
                else if (productInfo.Length > 0)
                {
                    ErrorMessage.ExceptionMessageBox.Show(string.Format(ResStrings.LiveDataForm_AddTagToDt, escapedTagUid),ResStrings.LiveDataForm_AddTagToDt_ERROR);
                }
                else
                {
                    object[] param = new object[_columnInfoFull.Length];
                    for (int i = 0; i < _columnInfoFull.Length; i++)
                    {
                        if (_columnInfoFull[i].colIndex == 0) param[0] = clToProcess.TagUid;
                        else if (_columnInfoFull[i].colIndex == 1) param[1] = ResStrings.str_Unreferenced;
                        else
                        {
                            if (_columnInfoFull[i].colType == typeof(double)) param[i] = "0";
                            else param[i] = " ";
                        }
                    }
                    clToProcess.Dt.Rows.Add(param);
                }
            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }
            
        }
       
        private bool StoreInventory(object obj, bool bStoreTagEvent , out int idScanEvent)
        {
            idScanEvent = -1;
            bool ret = false;
            lock (_locker)
            {
                InventoryAndDbClass clToProcess = (InventoryAndDbClass)obj;
                InventoryData invTmp = _db.GetLastScan(clToProcess.Device.SerialRFID);

                if ((invTmp != null) && (clToProcess.Data.eventDate > invTmp.eventDate.ToUniversalTime()))              
                {

                    int nbTostore = clToProcess.Data.nbTagAdded + clToProcess.Data.nbTagPresent + clToProcess.Data.nbTagRemoved;
                    int checkNbToStore = clToProcess.Data.dtTagAdded.Rows.Count + clToProcess.Data.dtTagPresent.Rows.Count + clToProcess.Data.dtTagRemove.Rows.Count;

                    if ((nbTostore != checkNbToStore) && (!_bWasInAccumulation) && (!_bKeepLastScan))
                    {
                        /*string idStream; 
                        try
                        {
                            BinaryFormatter bf = new BinaryFormatter();
                            MemoryStream mem = new MemoryStream();
                            bf.Serialize(mem, clToProcess.Data);
                            idStream = Convert.ToBase64String(mem.ToArray());
                        }
                        catch
                        {
                            idStream = ResStrings.str_failed_to_serialized;
                           
                        }*/
                        string info = string.Format(ResStrings.str_errorserialization, clToProcess.Data.nbTagAdded, clToProcess.Data.dtTagAdded.Rows.Count, clToProcess.Data.nbTagPresent, clToProcess.Data.dtTagPresent.Rows.Count, clToProcess.Data.nbTagRemoved, clToProcess.Data.dtTagRemove.Rows.Count);
                        ExceptionMessageBox.Show(ResStrings.str_errorDataStoreInventory + info, ResStrings.str_Info_on_store_inventory);
                    }
                    else
                    {

                        if (!clToProcess.Db.StoreInventory(clToProcess.Data))
                        {
                            ExceptionMessageBox.Show(ResStrings.str_Unable_to_store_inventory_Data_will_be_loose, ResStrings.str_Info_on_store_inventory);
                        }
                        else
                        {
                            idScanEvent = clToProcess.Db.getRowInsertIndex();

                            if (bStoreTagEvent)
                            {                               
                                clToProcess.Db.storeTagEvent(clToProcess.Data, idScanEvent);
                            }
                            ret = true;
                        }
                    }
                }
                else
                {
                    if (invTmp == null)
                    {
                        int nbTostore = clToProcess.Data.nbTagAdded + clToProcess.Data.nbTagPresent + clToProcess.Data.nbTagRemoved;
                        int checkNbToStore = clToProcess.Data.dtTagAdded.Rows.Count + clToProcess.Data.dtTagPresent.Rows.Count + clToProcess.Data.dtTagRemove.Rows.Count;

                        if ((nbTostore != checkNbToStore) && (!_bWasInAccumulation))
                        {
                            /*string idStream = string.Empty;
                            try
                            {
                                BinaryFormatter bf = new BinaryFormatter();
                                MemoryStream mem = new MemoryStream();
                                bf.Serialize(mem, clToProcess.Data);
                                idStream = Convert.ToBase64String(mem.ToArray());
                            }
                            catch
                            {

                            }*/

                            string info = string.Format(ResStrings.str_errorserialization, clToProcess.Data.nbTagAdded, clToProcess.Data.dtTagAdded.Rows.Count, clToProcess.Data.nbTagPresent, clToProcess.Data.dtTagPresent.Rows.Count, clToProcess.Data.nbTagRemoved, clToProcess.Data.dtTagRemove.Rows.Count);
                            ExceptionMessageBox.Show(ResStrings.str_errorDataStoreInventory + info, ResStrings.str_Info_on_store_inventory);
                            
                        }
                        else
                        {

                            if (!clToProcess.Db.StoreInventory(clToProcess.Data))
                            {
                                ExceptionMessageBox.Show(ResStrings.str_Unable_to_store_inventory_Data_will_be_loose, ResStrings.str_Info_on_store_inventory);
                            }
                            else                               
                            {
                                idScanEvent = clToProcess.Db.getRowInsertIndex();
                                if (bStoreTagEvent)
                                {                                   
                                    clToProcess.Db.storeTagEvent(clToProcess.Data, idScanEvent);
                                }
                               
                                ret = true;
                            }
                        }
                    }
                }
            }
            return ret;
        }
        private void StoreNetworkinventory(deviceClass dc, InventoryData invData)
        {
            Debug.WriteLine("Enter store :" + DateTime.Now.ToString("hh:mm:ss.fff"));
            if (!_bKeepLastScan)
            {
                InventoryData lastScan = _db.GetLastScan(dc.infoDev.SerialRFID);
                if (lastScan != null)
                    dc.previousInventory = lastScan;
                else
                    dc.previousInventory = dc.currentInventory;          
            }
            else
                dc.previousInventory = dc.currentInventory;


            Debug.WriteLine("End Last Scan stored :" + DateTime.Now.ToString("hh:mm:ss.fff"));
            if (!_db.IsInventoryExist(invData))
            {
                dc.currentInventory = new InventoryData(_columnInfo);
                dc.currentInventory.bUserScan = invData.bUserScan;
                dc.currentInventory.userFirstName = invData.userFirstName;
                dc.currentInventory.userLastName = invData.userLastName;
                dc.currentInventory.accessType = invData.accessType;
                dc.currentInventory.userDoor = invData.userDoor;
                dc.currentInventory.BadgeID = invData.BadgeID;
                dc.currentInventory.serialNumberDevice = invData.serialNumberDevice;
                dc.currentInventory.eventDate = invData.eventDate;
                dc.currentInventory.ListTagWithChannel = invData.ListTagWithChannel;
            
             if (_bKeepLastScan) //copy from previousinventory all tag and add it in all and present.
             {   
                foreach (string uid in dc.previousInventory.listTagAll)
                {
                    dc.currentInventory.listTagAll.Add(uid);
                    DtAndTagClass dtAndTagAll = new DtAndTagClass(this, dc.currentInventory.dtTagAll, uid, _db);
                    AddTagToDt(dtAndTagAll);

                    dc.currentInventory.listTagPresent.Add(uid);
                    DtAndTagClass dtAndTagPresent = new DtAndTagClass(this, dc.currentInventory.dtTagPresent, uid, _db);
                    AddTagToDt(dtAndTagPresent);
                }
            }

            foreach (string uid in invData.listTagAll)
            {
                if (!dc.currentInventory.listTagAll.Contains(uid))
                {
                    dc.currentInventory.listTagAll.Add(uid);
                    DtAndTagClass dtAndTagAll = new DtAndTagClass(this, dc.currentInventory.dtTagAll, uid, _db);
                    AddTagToDt(dtAndTagAll);
                }

                if (!dc.bnetAccumulateMode)
                {
                    if (!dc.previousInventory.listTagAll.Contains(uid))
                    {
                        // Tag Added
                        if (dc.currentInventory.listTagAdded.Contains(uid)) continue;
                        dc.currentInventory.listTagAdded.Add(uid);
                        DtAndTagClass dtAndTagAdded = new DtAndTagClass(this, dc.currentInventory.dtTagAdded, uid, _db);
                        AddTagToDt(dtAndTagAdded);
                    }
                    else
                    {
                        //tag Present
                        if (dc.currentInventory.listTagPresent.Contains(uid)) continue;
                        dc.currentInventory.listTagPresent.Add(uid);
                        DtAndTagClass dtAndTagPresent = new DtAndTagClass(this, dc.currentInventory.dtTagPresent, uid, _db);
                        AddTagToDt(dtAndTagPresent);
                    }
                }
                else
                {
                    //tag Present
                    if (dc.currentInventory.listTagPresent.Contains(uid)) continue;
                    dc.currentInventory.listTagPresent.Add(uid);
                    DtAndTagClass dtAndTagPresent = new DtAndTagClass(this, dc.currentInventory.dtTagPresent, uid, _db);
                    AddTagToDt(dtAndTagPresent);
                }

            }

            foreach (string uid in dc.previousInventory.listTagAll)
            {
                if (dc.currentInventory.listTagAll.Contains(uid)) continue;
                if (dc.currentInventory.listTagRemoved.Contains(uid)) continue;
                dc.currentInventory.listTagRemoved.Add(uid);
                DtAndTagClass dtAndTagRemove = new DtAndTagClass(this, dc.currentInventory.dtTagRemove, uid, _db);
                AddTagToDt(dtAndTagRemove);
            }

            dc.currentInventory.nbTagAll = dc.currentInventory.listTagAll.Count;
            dc.currentInventory.nbTagPresent = dc.currentInventory.listTagPresent.Count;
            dc.currentInventory.nbTagAdded = dc.currentInventory.listTagAdded.Count;
            dc.currentInventory.nbTagRemoved = dc.currentInventory.listTagRemoved.Count;

            //if (!_db.IsInventoryExist(dc.currentInventory))
            //{
                Debug.WriteLine("End  prepared Scan :" + DateTime.Now.ToString("hh:mm:ss.fff"));

                InventoryAndDbClass invCl = new InventoryAndDbClass(dc.infoDev, dc.currentInventory, _db, _bStoreTagEvent);
                int idScanEvent = -1;
                bool ret = StoreInventory(invCl, _bStoreTagEvent , out idScanEvent);
                Debug.WriteLine("End Scan stored :" + DateTime.Now.ToString("hh:mm:ss.fff"));
                if (ret)
                {
                    dc.currentInventory.IdScanEvent = idScanEvent;
                    dc.lastProcessInventoryGmtDate = truncateMs(dc.currentInventory.eventDate.ToUniversalTime());
                    Invoke((MethodInvoker)delegate
                    {
                        /*if (tabControlInfo.SelectedIndex == 1)
                            UpdateScanHistory(null);*/
                    });
                }
            }
            else
            {
                dc.lastProcessInventoryGmtDate = truncateMs(dc.currentInventory.eventDate.ToUniversalTime());
            }
            Debug.WriteLine("Quit store :" + DateTime.Now.ToString("hh:mm:ss.fff"));
        }
        #endregion
        #region contextmenudebug
        private void calibrateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int treeNodeSelected = treeViewDevice.SelectedNode.Index;

            if ((_localDeviceArray[treeNodeSelected].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                       (_localDeviceArray[treeNodeSelected].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
            {

                _localDeviceArray[treeNodeSelected].rfidDev.get_RFID_Device.CalibrateDialog();

            }
        }
        private void findThresholdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int treeNodeSelected = treeViewDevice.SelectedNode.Index;

            if ((_localDeviceArray[treeNodeSelected].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                       (_localDeviceArray[treeNodeSelected].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
            {
                _localDeviceArray[treeNodeSelected].rfidDev.get_RFID_Device.FindThresholdDialog();
            }
        }
        private void conversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int treeNodeSelected = treeViewDevice.SelectedNode.Index;

            if ((_localDeviceArray[treeNodeSelected].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                       (_localDeviceArray[treeNodeSelected].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
            {
                _localDeviceArray[treeNodeSelected].rfidDev.get_RFID_Device.ConversionDialog();
            }
        }
        private void tagSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int treeNodeSelected = treeViewDevice.SelectedNode.Index;

            if ((_localDeviceArray[treeNodeSelected].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                       (_localDeviceArray[treeNodeSelected].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
            {
                _localDeviceArray[treeNodeSelected].rfidDev.get_RFID_Device.TagSetDialog();
            }
        }
        private void doorLightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int treeNodeSelected = treeViewDevice.SelectedNode.Index;

            if ((_localDeviceArray[treeNodeSelected].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                       (_localDeviceArray[treeNodeSelected].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
            {
                _localDeviceArray[treeNodeSelected].rfidDev.get_RFID_Device.DoorAndLightDiag();
            }
        }       
        private void renewFingerprintToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            RenewFp(true);    
        }
        public void RenewFp(bool bFullRenew)
        {
            FingerPrintClass mfp = new FingerPrintClass();
            mfp.InitFingerPrint(null, false);
            mfp.renewFP(false);
            mfp.ReleaseFingerprint();
            if (_localDeviceArray == null) return;
            foreach (deviceClass dc in _localDeviceArray)
            {
                if ((dc.infoDev.deviceType == DeviceType.DT_MSR) || (dc.infoDev.deviceType == DeviceType.DT_SAS))
                {
                    dc.rfidDev.RenewFP(true, true);                    
                }
                else
                    dc.rfidDev.RenewFP(false, true);     
            }      
        }
        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int treeNodeSelected = treeViewDevice.SelectedNode.Index;

            if ((_localDeviceArray[treeNodeSelected].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                       (_localDeviceArray[treeNodeSelected].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
            {

                if (_localDeviceArray[treeNodeSelected].currentInventory.ListTagWithChannel != null)
                {
                   
                    string strLit = null;
                   
                    ArrayList ar = new ArrayList(_localDeviceArray[treeNodeSelected].currentInventory.ListTagWithChannel.Keys);
                    strLit += "\r\n Live data Count : " + ar.Count;
                    foreach (var k in ar)
                    {
                        strLit += k + " ; " + _localDeviceArray[treeNodeSelected].currentInventory.ListTagWithChannel[k] + "\r\n";
                    }

                    strLit += "\r\n device Count : " + ar.Count;
                    MessageBox.Show(strLit);

                }


            }
        }
        #endregion
        #region contextmenuList
        private void copyLotIDToClipBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataListView.SelectedItems.Count < 0)
            {
                StringBuilder buffer = new StringBuilder();
                // Loop over all the selected items
                foreach (ListViewItem currentItem in dataListView.Items)
                {
                    // Don't need to look at currentItem, because it is in subitem[0]
                    // So just loop over all the subitems of this selected item
                    //foreach (ListViewItem.ListViewSubItem sub in currentItem.SubItems)
                    //{
                    // Append the text and tab
                    OLVColumn olc = dataListView.GetColumn(_columnInfo[1].ToString());
                    buffer.Append(currentItem.SubItems[olc.Index].Text);
                    buffer.Append("\t");
                    //}
                    // Annoyance: there is a trailing tab in the buffer, get rid of it
                    buffer.Remove(buffer.Length - 1, 1);
                    // If you only use \n, not all programs (notepad!!!) will recognize the newline
                    buffer.Append("\r\n");
                }
                // Set output to clipboard.
                Clipboard.SetDataObject(buffer.ToString());
            }
            else
            {
                StringBuilder buffer = new StringBuilder();
                // Loop over all the selected items
                foreach (ListViewItem currentItem in dataListView.SelectedItems)
                {
                    // Don't need to look at currentItem, because it is in subitem[0]
                    // So just loop over all the subitems of this selected item
                    // foreach(ListViewItem.ListViewSubItem sub in currentItem.SubItems)
                    //{
                    // Append the text and tab
                    OLVColumn olc = dataListView.GetColumn(_columnInfo[1].ToString());
                    buffer.Append(currentItem.SubItems[olc.Index].Text);
                    buffer.Append("\t");
                    //}
                    // Annoyance: there is a trailing tab in the buffer, get rid of it
                    buffer.Remove(buffer.Length - 1, 1);
                    // If you only use \n, not all programs (notepad!!!) will recognize the newline
                    buffer.Append("\r\n");
                }
                // Set output to clipboard.
                Clipboard.SetDataObject(buffer.ToString());

            }
        }
        private void copyToClipBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder buffer = new StringBuilder();
            // Loop over all the selected items
            foreach (ListViewItem currentItem in dataListView.Items)
            {
                foreach (ListViewItem.ListViewSubItem sub in currentItem.SubItems)
                {
                    buffer.Append(sub.Text);
                    buffer.Append("\t");
                }

                buffer.Remove(buffer.Length - 1, 1);
                // If you only use \n, not all programs (notepad!!!) will recognize the newline
                buffer.Append("\r\n");
            }
            // Set output to clipboard.
            Clipboard.SetDataObject(buffer.ToString());
        }
        private void unselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataListView.SelectedItems.Clear();
        }
        private void itemListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArrayList uid = new ArrayList();
            foreach (ListViewItem currentItem in dataListView.SelectedItems)
            {
                OLVColumn olc = dataListView.GetColumn(_columnInfo[0].ToString());
                uid.Insert(0, currentItem.SubItems[olc.Index].Text);
            }

            ItemListForm ilf = new ItemListForm(this, uid);
            ilf.Show();

        }
        private void updateTagUIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataListView.SelectedItems.Count == 1)
            {
                ListViewItem currentItem = dataListView.SelectedItems[0];
                OLVColumn olc = dataListView.GetColumn(_columnInfo[0].ToString());
                string tagId = currentItem.SubItems[olc.Index].Text;
                deviceClass currentDeviceClass;

                TagUidWritingForm twf = null;
                if ((_selectedReader >= 0) & (_selectedReader < _nbLocalDevice))
                {
                    currentDeviceClass = _localDeviceArray[_selectedReader];
                    double fv = 0.0;
                    double.TryParse(currentDeviceClass.rfidDev.get_RFID_Device.FirmwareVersion.Replace(",","."),NumberStyles.Number, CultureInfo.InvariantCulture, out fv);
                    if (fv > 2.54)
                    {
                        twf = new TagUidWritingForm(tagId, currentDeviceClass.rfidDev);
                    }
                    else
                    {
                        string mes = string.Format(ResStrings.LiveDataForm_updateTagUIDToolStripMenuItem_Click_Update_firmware_with_version_above_2_54_to_use_writing_feature);
                        MessageBox.Show(mes, ResStrings.strInfo);
                    }
                }

                else
                {
                    if (SelectedNetworkDevice == null)
                    {
                        MessageBox.Show(ResStrings.str_Unable_to_find_selected_network_device, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    double fv = 0.0;
                    currentDeviceClass = SelectedNetworkDevice;
                    TcpIpClient tcp = DeviceClientTcp[currentDeviceClass.infoDev.IP_Server];
                    tcp.RequestFirmwareVersion(currentDeviceClass.infoDev.IP_Server,currentDeviceClass.infoDev.Port_Server, out fv);

                    if (fv > 2.54)
                    {
                        twf = new TagUidWritingForm(tagId, currentDeviceClass);
                    }
                    else
                    {
                        string mes = string.Format(ResStrings.LiveDataForm_updateTagUIDToolStripMenuItem_Click_Update_firmware_with_version_above_2_54_to_use_writing_feature);
                        MessageBox.Show(mes, ResStrings.strInfo);
                    }
                }
                if (twf != null) twf.ShowDialog();
                
            }

            else
                MessageBox.Show(ResStrings.str_Please_select_only_one_tag, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void stepByStepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            List<string> tags = new List<string>();
            Hashtable tagLedStateTable = new Hashtable();

            if ((_selectedReader >= 0) & (_selectedReader < _nbLocalDevice)) //reader local
            {
                RFID_Device currentDevice = _localDeviceArray[_selectedReader].rfidDev;
                if (currentDevice.get_RFID_Device.HardwareVersion.StartsWith("11"))
                {
                    MessageBox.Show("Group per Group Not supported by SmartPad - please use All at once Mode");
                    return;
                }
                if ((currentDevice.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                    (currentDevice.DeviceStatus == DeviceStatus.DS_Ready))
                {
                    foreach (ListViewItem currentItem in dataListView.SelectedItems)
                    {

                        OLVColumn olcevent = dataListView.GetColumn(0);
                        string eventtype = currentItem.SubItems[olcevent.Index].Text;
                        if (eventtype.Equals("Removed"))
                            continue;

                        OLVColumn olc = dataListView.GetColumn(_columnInfo[0].ToString());
                        string tagId = currentItem.SubItems[olc.Index].Text;
                        tags.Add(tagId);
                        tagLedStateTable.Add(tagId, false);
                    }

                    int nbLighted = 0, totalLighted = 0, currentChannel = 0;
                    bool userChoice = true, isLastStep = false;

                    int maxChannnel = currentDevice.get_RFID_Device.DeviceBoard.getNumberMaxChannel();

                    for (int i = 1; i < maxChannnel + 1; ++i)
                        currentDevice.get_RFID_Device.StartLedOn(i);

                    while (userChoice && !isLastStep) // While users want to go on searching and we didn't browse all device axis
                    {
                        isLastStep = currentDevice.StartLightingLeds(tags, tagLedStateTable, out currentChannel, out nbLighted);

                        if (nbLighted == 0) continue;

                        totalLighted += nbLighted;

                        if (totalLighted == tags.Count) break;

                        string message = String.Format(ResStrings.str_LedFoundOnChannel,
                            nbLighted,
                            tags.Count, totalLighted, currentChannel);

                        DialogResult dialogChoice = MessageBox.Show(message, ResStrings.str_Research_in_progress, MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        userChoice = (dialogChoice != DialogResult.No);
                    }


                    if (totalLighted == tags.Count)
                    {
                        string message = String.Format(ResStrings.str_Led2,
                            nbLighted,
                            tags.Count, totalLighted, currentChannel);
                        MessageBox.Show(message, ResStrings.str_Research_over, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    else
                    {
                        if (isLastStep)
                        {
                            string message = String.Format(ResStrings.str_Led3, tags.Count - totalLighted);

                            foreach (DictionaryEntry entryTag in tagLedStateTable)
                                if (!(bool)entryTag.Value)
                                    message = String.Format("{0}\n{1}", message, entryTag.Key);

                            MessageBox.Show(message, ResStrings.str_Research_over, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }

                    currentDevice.StopLightingLeds();
                    if (currentDevice.DeviceStatus != DeviceStatus.DS_Ready)
                    {
                        MessageBox.Show(ResStrings.str_LedModeError, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        currentDevice.DeviceStatus = DeviceStatus.DS_Ready;
                    }

                    Cursor.Current = Cursors.Default;
                }
            }
            UpdateTreeView();
        }
        private static ManualResetEvent resetEvent = new ManualResetEvent(false);
        public static void DoWorkForLed(RFID_Device currentDevice , List<string> selectedTags)
        {
            currentDevice.TestLighting(selectedTags);
        }

        public static void NonBlockingSleep(int timeInMilliseconds)
        {
            DispatcherFrame df = new DispatcherFrame();

            new Thread((ThreadStart)(() =>
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(timeInMilliseconds));
                df.Continue = false;

            })).Start();

            Dispatcher.PushFrame(df);
        }

        private void allAtOnceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> selectedTags = new List<string>();

            foreach (ListViewItem currentItem in dataListView.SelectedItems)
            {
                OLVColumn olcevent = dataListView.GetColumn(0);
                string eventtype = currentItem.SubItems[olcevent.Index].Text;
                if (eventtype.Equals("Removed"))
                    continue;

                OLVColumn olc = dataListView.GetColumn(_columnInfo[0].ToString());
                string tagId = currentItem.SubItems[olc.Index].Text;
                selectedTags.Add(tagId);
            }

            int nbTagToLight = selectedTags.Count;
            if (nbTagToLight == 0) return;

            if ((_selectedReader >= 0) & (_selectedReader < _nbLocalDevice)) //reader local
            {
                RFID_Device currentDevice = _localDeviceArray[_selectedReader].rfidDev;

                if ((currentDevice.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                    (currentDevice.DeviceStatus == DeviceStatus.DS_Ready))
                {

                    if (syncDevice != null)
                    {
                        currentDevice.DeviceStatus = DeviceStatus.DS_WaitForLed;
                        UpdateTreeView();
                        syncDevice.bIsWaitingLed = true;
                        Thread thLed = new Thread(() => syncDevice.CanStartLed());
                        thLed.IsBackground = true;
                        thLed.Start();
                        while (syncDevice.bIsWaitingLed)
                        {
                            NonBlockingSleep(1000);
                        }                       
                    }

                    currentDevice.DeviceStatus = DeviceStatus.DS_LedOn;
                    UpdateTreeView();

                    currentDevice.get_RFID_Device.DeviceBoard.setBridgeState(false,167,167);
                    currentDevice.isInSearchTag = true;
                    Thread th = new Thread(() => currentDevice.TestLighting(selectedTags));
                    th.IsBackground = true;
                    th.Start();
                    while (currentDevice.isInSearchTag)
                    {
                        NonBlockingSleep(1000);
                    }
                   

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

                    /*MessageBox.Show(message, ResStrings.str_LED_Information, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    currentDevice.StopLightingLeds();*/

                    msgBox = new msgFrm(message, _localDeviceArray[_selectedReader].infoDev.DeviceName);
                    msgBox.ShowDialog();
                    if (msgBox.BUserClose)
                        currentDevice.StopLightingLeds();
                    msgBox.Dispose();
                    msgBox = null;

                }
            }

            else //Network device
            {
                if (treeViewDevice.SelectedNode != null)
                {
                    if (SelectedNetworkDevice == null) return;
                    TcpIpClient tcp = DeviceClientTcp[SelectedNetworkDevice.infoDev.IP_Server];
                    TcpIpClient.RetCode rr = tcp.RequestStartLighting(SelectedNetworkDevice.infoDev.IP_Server,SelectedNetworkDevice.infoDev.Port_Server, selectedTags);
                    if (rr != TcpIpClient.RetCode.RC_Succeed)
                    {
                        if (rr == TcpIpClient.RetCode.RC_Data_Error)
                        {
                            MessageBox.Show(ResStrings.LiveDataForm_allAtOnceToolStripMenuItem_Click_Perform_a_fresh_scan_before_use_LED_Feature);
                        }
                        else
                        {
                             MessageBox.Show(ResStrings.str_An_unexpected_error_occurred_during_communication_with_device);
                        }
                       
                        tcp.RequestStopLighting(SelectedNetworkDevice.infoDev.IP_Server, SelectedNetworkDevice.infoDev.Port_Server);
                        return;
                    }

                    StringBuilder resultMessage;
                    if (nbTagToLight == 1)
                    {
                        if ((nbTagToLight - selectedTags.Count) >= 1)
                            resultMessage = new StringBuilder(String.Format(ResStrings.str_LedTagFound, nbTagToLight, nbTagToLight - selectedTags.Count));
                        else
                            resultMessage = new StringBuilder(String.Format(ResStrings.str_tagfind, nbTagToLight, nbTagToLight - selectedTags.Count));


                    }
                    else if ((nbTagToLight - selectedTags.Count) == 1)
                    {
                        resultMessage = new StringBuilder(String.Format(ResStrings.str_LedFound2, nbTagToLight, nbTagToLight - selectedTags.Count));
                    }
                    else
                        resultMessage = new StringBuilder(String.Format(ResStrings.str_LedFound3, nbTagToLight, nbTagToLight - selectedTags.Count));

                    if (selectedTags.Count > 0)
                    {
                        resultMessage.AppendLine("\r\n" + ResStrings.str_Missing_tags_ID);

                        foreach (string missingTag in selectedTags)
                            resultMessage.AppendLine(missingTag);
                    }

                   
                      /*  MessageBox.Show(resultMessage.ToString());
                        tcpClient.RequestStopLighting(SelectedNetworkDevice.infoDev.IP_Server,
                            SelectedNetworkDevice.infoDev.Port_Server);*/


                        msgBox = new msgFrm(resultMessage.ToString(), SelectedNetworkDevice.infoDev.DeviceName);
                        msgBox.ShowDialog(this);
                        if (msgBox.BUserClose)
                            tcp.RequestStopLighting(SelectedNetworkDevice.infoDev.IP_Server,SelectedNetworkDevice.infoDev.Port_Server);
                        msgBox.Dispose();
                        msgBox = null;

                 
                
                }
            }
            UpdateTreeView();
        }

        private void findTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {


           
            /*List<string> selectedTags = new List<string>();
            selectedTags.Add(("O613T00000"));
            selectedTags.Add(("O613W00000"));
            selectedTags.Add(("O61XX00000"));
            selectedTags.Add(("O61Ba00000"));
            selectedTags.Add(("O60qo00000"));
            selectedTags.Add(("O61jc00000"));
            selectedTags.Add(("O60V600000"));
            selectedTags.Add(("O60cT00000"));
            selectedTags.Add(("O60kj00000"));

            _bStop = false;
            int nbTagToLight = selectedTags.Count;
            if (nbTagToLight == 0) return;

            if ((_selectedReader >= 0) & (_selectedReader < _nbLocalDevice)) //reader local
            {
                
                DialogResult ret = DialogResult.No;
                RFID_Device currentDevice = _localDeviceArray[_selectedReader].rfidDev;
               
                if ((currentDevice.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                    (currentDevice.DeviceStatus == DeviceStatus.DS_Ready))
                {
                    while ( (ret == DialogResult.No) && (selectedTags.Count > 0) && (!_bStop))
                    {
                        Application.DoEvents();
                        nbTagToLight = selectedTags.Count;
                        currentDevice.TestLightingOneAxis(selectedTags,1);
                        if (nbTagToLight != selectedTags.Count) // des tags on ete trouvé
                        {
                            string message = string.Empty;
                           
                            if ((nbTagToLight == 1) && ((nbTagToLight - selectedTags.Count) == 1))
                                message = String.Format(ResStrings.str_LedTagFound, nbTagToLight, nbTagToLight - selectedTags.Count);
                            else if ((nbTagToLight - selectedTags.Count) == 1)
                                message = String.Format(ResStrings.str_LedFound2, nbTagToLight, nbTagToLight - selectedTags.Count);
                            else
                                message = String.Format(ResStrings.str_LedFound3, nbTagToLight, nbTagToLight - selectedTags.Count);

                            if (selectedTags.Count > 0)
                            {
                                message += "\r\n Would you like to stop to search?";
                                ret = MessageBox.Show(message, ResStrings.str_LED_Information, MessageBoxButtons.YesNo,MessageBoxIcon.Information);
                            }
                            else
                            {
                                message += "\r\n All tags Found - Process will stop?";
                                ret = MessageBox.Show(message, ResStrings.str_LED_Information, MessageBoxButtons.OK,MessageBoxIcon.Information);
                            }

                        }
                        currentDevice.StopLightingLeds();
                    }
                }
            }*/

        }

        private void findTagsByLedToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if ((_selectedReader >= 0) && (_selectedReader < _nbLocalDevice) && (_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SBR))//reader local
            {
                if (tslf != null)
                {
                    tslf.Close();
                    tslf.Dispose();
                    
                }
                
                tslf = new TagSearchLedfrm(this.DtProductToFind, _localDeviceArray[_selectedReader].rfidDev);
                tslf.StartPosition = FormStartPosition.Manual;
                tslf.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - tslf.Width, Screen.PrimaryScreen.WorkingArea.Bottom - tslf.Height);
                tslf.Show();
            }
            else
            {
                MessageBox.Show("Feature unable only on USB SBR device");
            }
        }

        private void findTagsByLedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if ((_selectedReader >= 0) && (_selectedReader < _nbLocalDevice) && (_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SBR))//reader local
            {
                if (tslf == null)
                    tslf = new TagSearchLedfrm(this.DtProductToFind, _localDeviceArray[_selectedReader].rfidDev);
                    tslf.StartPosition = FormStartPosition.Manual;
                    tslf.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - tslf.Width, Screen.PrimaryScreen.WorkingArea.Bottom - tslf.Height);
                    tslf.Show();
            }
            else
            {
                MessageBox.Show("Feature unable only on USB SBR device");
            }
        }

        #endregion            
        #region exportexcel
        private DataTable ConvertForExport(DataTable dt)
        {
            try
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
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
                return null;
            }

        }
        private DataTable ConvertColumnType(DataTable dt, IEnumerable<dtColumnInfo> colList)
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
       
        private void ExportToExcel()
        {
            string fileSaveName = null;
            string macroName = null;
            string pathMacro = null;   

            if (!_runMacro)
            {
                saveXlsFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                DialogResult ret = saveXlsFileDialog.ShowDialog();

                if (ret == DialogResult.OK)
                {
                   fileSaveName = Path.GetFullPath(saveXlsFileDialog.FileName);
                }
            }
            else
            {
                pathMacro = ConfigurationManager.AppSettings["pathExcelSheet"];
                macroName = ConfigurationManager.AppSettings["MacroName"];
                fileSaveName = Application.StartupPath + Path.DirectorySeparatorChar + "XLMacro" + Path.DirectorySeparatorChar + "RFID_export.xlsx";
               
                if (string.IsNullOrEmpty(pathMacro) | string.IsNullOrEmpty(macroName) | !File.Exists(pathMacro))
                {
                    MessageBox.Show(ResStrings.str_ErrorMAcroExcel, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }


            if (fileSaveName == null) return;

                if (File.Exists(fileSaveName))
                    File.Delete(fileSaveName);

                dtColumnInfo[] listCol = _db.GetdtColumnInfo();

                InventoryData dataToExport;

                if ((_selectedReader >= 0) & (_selectedReader < _nbLocalDevice))  //reader local
                    dataToExport = _localDeviceArray[_selectedReader].currentInventory;
                else
                    dataToExport = _networkDeviceArray[_selectedReader - _nbLocalDevice].currentInventory;

            if (Path.GetExtension(fileSaveName).Contains("csv"))
            {
                StringBuilder sb = new StringBuilder();
                // declare this outside the loop!
                char[] csvTokens = new[] { '\"', ',', '\n', '\r' };

                string[] columnNames = dataToExport.dtTagAll.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();

                for (int loop = 0; loop < columnNames.Length ; loop++)
                {
                    if (columnNames[loop].IndexOfAny(csvTokens) >= 0)
                    {
                        columnNames[loop] = "\"" + columnNames[loop].Replace("\"", "\"\"") + "\"";
                    } 
                }
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in dataToExport.dtTagAll.Rows)
                {
                    string[] fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                    for (int loop = 0; loop < fields.Length; loop++)
                    {
                        if (fields[loop].IndexOfAny(csvTokens) >= 0)
                        {
                            fields[loop] = "\"" + fields[loop].Replace("\"", "\"\"") + "\"";
                        }
                    }
                    sb.AppendLine(string.Join(",", fields));
                }

                File.WriteAllText(fileSaveName, sb.ToString());

            }
            else if (Path.GetExtension(fileSaveName).Contains("xlsx")) // 2010
                {

                    FileInfo newFile = new FileInfo(fileSaveName);
                    ExcelPackage pck = new ExcelPackage(newFile);
                    {
                        //Create the worksheet
                        ExcelWorksheet ws1 = pck.Workbook.Worksheets.Add(ResStrings.str_All);
                        //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                        ws1.Cells.Style.Font.Size = 10;
                        ws1.Cells.Style.Font.Name = "Verdana";

                        ws1.Cells["A1"].LoadFromDataTable(ConvertForExport(dataToExport.dtTagAll), true);

                        ws1.Cells[1, 1, 1, _columnInfo.Count].Style.Font.Bold = true;
                        ws1.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws1.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);


                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                            ws1.Column(loop).AutoFit(25);


                        if (listCol != null)
                        {
                            foreach (dtColumnInfo col1 in listCol)
                            {
                                if (col1.colDoSum == 1)
                                {
                                    if (!string.IsNullOrEmpty(col1.colName))
                                    {
                                        double sumValue = ProcessValueAlert(dataToExport.dtTagAll, col1.colName);

                                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                                        {
                                            if (ws1.Cells[1, loop].Value.ToString().Equals(col1.colName))
                                            {
                                                ws1.Cells[dataToExport.dtTagAll.Rows.Count + 2, loop].Value = sumValue;
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        ExcelWorksheet ws2 = pck.Workbook.Worksheets.Add("Prior");

                        ws2.Cells.Style.Font.Size = 10;
                        ws2.Cells.Style.Font.Name = "Verdana";

                        //Load the datatable into the sheet, starting from cell A1. Print the column names on 
                        ws2.Cells["A1"].LoadFromDataTable(ConvertForExport(dataToExport.dtTagPresent), true);
                        ws2.Cells[1, 1, 1, _columnInfo.Count].Style.Font.Bold = true;
                        ws2.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws2.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                            ws2.Column(loop).AutoFit(25);

                        if (listCol != null)
                        {
                            foreach (dtColumnInfo col1 in listCol)
                            {
                                if (col1.colDoSum == 1)
                                {
                                    if (!string.IsNullOrEmpty(col1.colName))
                                    {
                                        double sumValue = ProcessValueAlert(dataToExport.dtTagPresent, col1.colName);

                                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                                        {
                                            if (ws2.Cells[1, loop].Value.ToString().Equals(col1.colName))
                                            {
                                                ws2.Cells[dataToExport.dtTagPresent.Rows.Count + 2, loop].Value = sumValue;

                                            }
                                        }
                                    }
                                }
                            }
                        }



                        ExcelWorksheet ws3 = pck.Workbook.Worksheets.Add(ResStrings.str_Added);
                        ws3.Cells.Style.Font.Size = 10;
                        ws3.Cells.Style.Font.Name = "Verdana";

                        //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1                        
                        ws3.Cells["A1"].LoadFromDataTable(ConvertForExport(dataToExport.dtTagAdded), true);

                        ws3.Cells[1, 1, 1, _columnInfo.Count].Style.Font.Bold = true;
                        ws3.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws3.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                            ws3.Column(loop).AutoFit(25);
                        if (listCol != null)
                        {
                            foreach (dtColumnInfo col1 in listCol)
                            {
                                if (col1.colDoSum == 1)
                                {
                                    if (!string.IsNullOrEmpty(col1.colName))
                                    {
                                        double sumValue = ProcessValueAlert(dataToExport.dtTagAdded, col1.colName);

                                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                                        {
                                            if (ws3.Cells[1, loop].Value.ToString().Equals(col1.colName))
                                            {
                                                ws3.Cells[dataToExport.dtTagAdded.Rows.Count + 2, loop].Value = sumValue;

                                            }
                                        }
                                    }
                                }
                            }
                        }


                        ExcelWorksheet ws4 = pck.Workbook.Worksheets.Add(ResStrings.str_Removed);

                        ws4.Cells.Style.Font.Size = 10;
                        ws4.Cells.Style.Font.Name = "Verdana";
                        //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1                        
                        ws4.Cells["A1"].LoadFromDataTable(ConvertForExport(dataToExport.dtTagRemove), true);

                        ws4.Cells[1, 1, 1, _columnInfo.Count].Style.Font.Bold = true;
                        ws4.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws4.Cells[1, 1, 1, _columnInfo.Count].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                            ws4.Column(loop).AutoFit(25);

                        if (listCol != null)
                        {
                            foreach (dtColumnInfo col1 in listCol)
                            {
                                if (col1.colDoSum == 1)
                                {
                                    if (!string.IsNullOrEmpty(col1.colName))
                                    {
                                        double sumValue = ProcessValueAlert(dataToExport.dtTagRemove, col1.colName);

                                        for (int loop = 1; loop <= _columnInfo.Count; loop++)
                                        {
                                            if (ws4.Cells[1, loop].Value.ToString().Equals(col1.colName))
                                            {
                                                ws4.Cells[dataToExport.dtTagRemove.Rows.Count + 2, loop].Value = sumValue;

                                            }
                                        }
                                    }
                                }
                            }
                        }
                        pck.Save();
                    }
                }
                else
                {
                   new ErrorExportDLG().Show();
                }


                if (_runMacro)
                {
                    string processMacro = Application.StartupPath + Path.DirectorySeparatorChar + "XLMacro" + Path.DirectorySeparatorChar + "RunMacro.vbs";
                    string args = "\"" +  pathMacro + "\" " + macroName;

                    Process p = new Process
                    {
                        StartInfo = {FileName = processMacro, Arguments = args, UseShellExecute = true}
                    };
                    p.Start();
                  
                }
                else
                {
                  
                    if (File.Exists(fileSaveName))
                    if (Path.GetExtension(fileSaveName).Contains("xlsx"))
                            Process.Start(fileSaveName);
                }
           
          
        }
        #endregion
        #region TCPNotification
        private void GetTcpNotificationInfo()
        {
            string hostIp;
            string hostPortStr;
            int bEnable;
            DBClassSQLite dbSqlite = new DBClassSQLite();
            dbSqlite.OpenDB();
            if (dbSqlite.getExportInfo(2, out hostIp, out hostPortStr, out bEnable))
            {
                if (bEnable == 1)
                {
                    _bUseTcpNotification = true;
                    _tcpNotificationThreadInfo = new TcpThreadHandle(TcpThreadHandle.TcpNotificationType.None, "11111111", _tcpIpServer.ServerIP, _tcpIpServer.Port, hostIp);
                }
                else
                    _bUseTcpNotification = false;
            }
            dbSqlite.CloseDB();
            dbSqlite = null;
        }       
        public class TcpThreadHandle
        {
            public enum TcpNotificationType
            {
                None = 0x00,
                DoorOpen = 0x01,
                ScanStarted = 0x02,
                ScanCompleted = 0x03,
                ScanCancelByHost = 0x04,
                TestTcp = 0x05,
                TempChanged = 0x06,
            }
            string _serialRfid;           
            string _myLocalIp;
            int _myPort;
            int _idScanEvent;
            private double _temp = 0.0;
            public bool IsRunning = false;
            TcpNotificationType _tcpType = TcpNotificationType.None;

            public string HostIp;
            int _hostPort;
            int _bEnable;

            public TcpThreadHandle(TcpNotificationType tcpType, string serialRfid, string myLocalIp, int myPort, string hostIp)
            {
                _tcpType = tcpType;
                _serialRfid = serialRfid;
                _myLocalIp = myLocalIp;
                _myPort = myPort;
                HostIp = hostIp;
            }
            public void SetParam(TcpNotificationType tcpType, string serialRfid, string myLocalIp, int myPort, int idScanEvent,double temp)
            {
                _tcpType = tcpType;
                _serialRfid = serialRfid;
                _myLocalIp = myLocalIp;
                _myPort = myPort;
                _idScanEvent = idScanEvent;
                _temp = temp;
            }
            public void ThreadTcpLoop()
            {
                IsRunning = true;
                DBClassSQLite dbSqlite = null;
                TcpClient tcpNot = null;

                try
                {
                    dbSqlite = new DBClassSQLite();
                    dbSqlite.OpenDB();
                    string hostPortStr;
                    if (dbSqlite.getExportInfo(2, out HostIp, out hostPortStr, out _bEnable))
                    {
                        Debug.WriteLine("TCP sent " + _tcpType.ToString());
                        tcpNot = new TcpClient();
                        int.TryParse(hostPortStr, out _hostPort);
                        IAsyncResult result = tcpNot.BeginConnect(HostIp, _hostPort, null, null);
                        WaitHandle timeoutHandler = result.AsyncWaitHandle;
                        if (result.AsyncWaitHandle.WaitOne(2000, false))
                        {
                            tcpNot.EndConnect(result);
                            string testInfo = null;
                            switch (_tcpType)
                            {
                                case TcpNotificationType.ScanCompleted:
                                    testInfo = "CR_DISPATCH CC_SB_NEWINV " + _myLocalIp + " " + _myPort + " " + _serialRfid + " " + _idScanEvent;
                                    break;
                                case TcpNotificationType.DoorOpen:
                                    testInfo = "CR_DISPATCH CC_SB_DOOR_OPEN " + _myLocalIp + " " + _myPort + " " + _serialRfid;
                                    break;
                                case TcpNotificationType.ScanStarted:
                                    testInfo = "CR_DISPATCH CC_SB_SCAN_STARTED " + _myLocalIp + " " + _myPort + " " + _serialRfid;
                                    break;
                                case TcpNotificationType.ScanCancelByHost:
                                    testInfo = "CR_DISPATCH CC_SB_SCAN_CANCEL_BY_HOST " + _myLocalIp + " " + _myPort + " " + _serialRfid;
                                    break;
                                case TcpNotificationType.TestTcp:
                                    testInfo = "CR_DISPATCH CC_SB_TEST_TCP " + _myLocalIp + " " + _myPort + " " + _serialRfid;
                                    break;
                                case TcpNotificationType.TempChanged:
                                    testInfo = "CR_DISPATCH CC_SB_TEMP_CHANGED " + _myLocalIp + " " + _myPort + " " + _serialRfid + " " + _temp;
                                    break;

                            }
                            if (!string.IsNullOrEmpty(testInfo))
                            {
                                Stream stm = tcpNot.GetStream();
                                ASCIIEncoding asen = new ASCIIEncoding();
                                byte[] data = asen.GetBytes(testInfo);
                                stm.Write(data, 0, data.Length);
                            }
                            timeoutHandler.Close();                           

                        }                    

                    }
                }
                catch (SocketException exp)
                {
                    //ErrorMessage.ExceptionMessageBox.Show(exp);
                    Debug.WriteLine(_tcpType.ToString() + ": " + exp.ErrorCode);
                }
                finally
                {
                    if (tcpNot != null)
                    {
                        if (tcpNot.Connected)
                            tcpNot.Close();
                    }
                    if (dbSqlite != null)
                    {
                        if (dbSqlite.isOpen())
                            dbSqlite.CloseDB();
                    }
                  
                }

                IsRunning = false; 
            }
        }
        void initTcpServerNotification()
        {
            int.TryParse(ConfigurationManager.AppSettings["serverPort"], out tcpNotifyPort);
            notifyServer = new TcpNotificationServer(tcpNotifyPort + 1);
            notifyServer.TcpNotifyEvent += new TcpNotificationServer.TcpNotifyHandlerDelegate(notifyServer_TcpNotifyEvent);
            notifyServer.StartServer();
        }
        void closeTcpServerNotification()
        {
            if (notifyServer != null)
                notifyServer.StopSocket();
        }
        private void notifyServer_TcpNotifyEvent(Object sender, rfidTcpNotArg arg)
        {
            Debug.WriteLine("TCP recieved :" + DateTime.Now.ToString("hh:mm:ss.fff"));
            Debug.WriteLine("TCP recieved :" + arg.SerialNumber + " " + arg.RN_Value.ToString());
            
            if (_networkDeviceArray == null) return;
            if (_networkDeviceArray.Length > 0)
            {
                for (int i = 0; i < _networkDeviceArray.Length; i++)
                {
                    if (_networkDeviceArray[i].infoDev.SerialRFID == arg.SerialNumber)
                    {
                        _networkDeviceArray[i].netConnectionStatus = ConnectionStatus.CS_Connected;
                        switch (arg.RN_Value)
                        {
                            case rfidTcpNotArg.ReaderTcpNotify.RN_TestNotification:
                                _networkDeviceArray[i].bUseTcpNotification = true;
                            break;
                            case rfidTcpNotArg.ReaderTcpNotify.RN_Door_Opened:
                                _networkDeviceArray[i].netDeviceStatus = DeviceStatus.DS_DoorOpen;
                               if (!_networkDeviceArray[i].bUseTcpNotification)
                                return;
                                return;

                            case rfidTcpNotArg.ReaderTcpNotify.RN_ScanStarted:

                              
                                if (!_networkDeviceArray[i].bUseTcpNotification)
                                    return;
                                _networkDeviceArray[i].netDeviceStatus = DeviceStatus.DS_InScan;
                                _networkDeviceArray[i].cptPoll = 0;
                               // UpdateTreeView();
                                Invoke((MethodInvoker)delegate
                                {
                                    if (!_sip.Visible)
                                    {
                                        _sip.SetInfo(_networkDeviceArray[i].infoDev);
                                        _sip.Show();
                                        if (msgBox != null)
                                        {
                                            if (msgBox.name == _networkDeviceArray[i].infoDev.DeviceName)
                                                msgBox.Close();
                                          
                                        }
                                    }
                                    timerRefreshScan.Interval = 500;
                                    timerRefreshScan.Enabled = true;
                                });
                              
                                return;
                            case rfidTcpNotArg.ReaderTcpNotify.RN_ScanCompleted:

                                Debug.WriteLine("Request RN_ScanCompleted :" + DateTime.Now.ToString("hh:mm:ss.fff"));
                                if (!_networkDeviceArray[i].bUseTcpNotification)
                                    return;

                                //StoreNetworkinventory(networkDeviceArray[i], invData);
                                
                              

                                Debug.WriteLine("Before store :" + DateTime.Now.ToString("hh:mm:ss.fff"));
                                DateTime lastScanIp = DateTime.MaxValue;
                                TcpIpClient tcp = DeviceClientTcp[_networkDeviceArray[i].infoDev.IP_Server];
                                tcp.getLastDateScan(_networkDeviceArray[i].infoDev.IP_Server, _networkDeviceArray[i].infoDev.Port_Server, _networkDeviceArray[i].infoDev.SerialRFID, out lastScanIp);
                                    if (lastScanIp > _networkDeviceArray[i].lastProcessInventoryGmtDate)
                                    {
                                        InventoryData[] inventoryArray = null;
                                        TcpIpClient.RetCode ret = tcp.getScanFromDateWithDB(_networkDeviceArray[i].infoDev.IP_Server, _networkDeviceArray[i].infoDev.Port_Server, _networkDeviceArray[i].infoDev.SerialRFID, _networkDeviceArray[i].currentInventory.eventDate.ToUniversalTime(), out inventoryArray);
                                        if (ret == TcpIpClient.RetCode.RC_Succeed)
                                        {
                                            if (inventoryArray != null)
                                            {
                                                foreach (InventoryData inv in inventoryArray)
                                                {
                                                    DateTime dtUtc = DateTime.SpecifyKind(inv.eventDate, DateTimeKind.Utc);
                                                    inv.eventDate = dtUtc.ToLocalTime();
                                                    StoreNetworkinventory(_networkDeviceArray[i], inv);
                                                }


                                                Debug.WriteLine("Afterstore :" + DateTime.Now.ToString("hh:mm:ss.fff"));

                                            //alert Remove time exceed
                                            bool bReaderForRemovetoLong = false;
                                            if (_removedReaderDevice.Contains(_networkDeviceArray[i].infoDev.SerialRFID)) bReaderForRemovetoLong = true;

                                            if (_bUseAlarm && _networkDeviceArray[i].currentInventory.nbTagRemoved > 0 && _maxTimeBeforeAlertRemoved > 0 && bReaderForRemovetoLong)
                                            {
                                                TimeSpan ts = new TimeSpan(0, _maxTimeBeforeAlertRemoved, 0);
                                                DateTime dateToTest = DateTime.Now.Add(ts);
                                                _removedInventoryAlert.Add(dateToTest, _networkDeviceArray[i]);
                                            }
                                            #region BloodPatient

                                            if ((!_selectedPatient.Equals(ResStrings.str_None))
                                                    && (_networkDeviceArray[i].currentInventory.dtTagRemove.Rows.Count > 0))
                                            {
                                                if (_alertBloodPatient != null)
                                                {
                                                    string columnPatient = _db.GetColumnNameForAlert(AlertType.AT_Bad_Blood_Patient);
                                                    if (!string.IsNullOrEmpty(columnPatient))
                                                    {

                                                        ArrayList badBlood = new ArrayList();
                                                        foreach (DataRow oRow in _networkDeviceArray[i].currentInventory.dtTagRemove.Rows)
                                                        {
                                                            if (oRow[columnPatient] != DBNull.Value)
                                                            {
                                                                if (!_selectedPatient.Equals((string)oRow[columnPatient]))
                                                                    badBlood.Add(oRow);
                                                            }
                                                        }

                                                        if (badBlood.Count > 0)
                                                        {
                                                            string spData = string.Empty;
                                                            spData += _selectedPatient;
                                                            foreach (DataRow oRow in badBlood)
                                                            {
                                                                string rowValue = string.Empty;
                                                                for (int x = 0; x < oRow.ItemArray.Length; x++)
                                                                {
                                                                    if (rowValue.Length > 0) rowValue += " - ";
                                                                    rowValue += oRow[x].ToString();
                                                                }

                                                                if (spData.Length > 0) spData += ";";
                                                                spData += rowValue;
                                                            }
                                                            Invoke((MethodInvoker)delegate { new BadStopPatient().ShowDialog(); });
                                                            _db.storeAlert(AlertType.AT_Bad_Blood_Patient, _networkDeviceArray[i].infoDev, null, spData);
                                                            AlertMgtClass.treatAlert(AlertType.AT_Bad_Blood_Patient, _networkDeviceArray[i].infoDev, null, _networkDeviceArray[i].currentInventory, spData, BShowAlert);
                                                        }
                                                    }
                                                }
                                            }
                                            #endregion

                                            // alert remove nb item
                                            //if (_bUseAlarm && _networkDeviceArray[i].currentInventory.bUserScan)
                                            if (_bUseAlarm)
                                            {

                                                if (_alertItems != null)
                                                {

                                                    int nbMaxItem = _db.getUserMaxRemovedItem(_networkDeviceArray[i].currentInventory.userFirstName, _networkDeviceArray[i].currentInventory.userLastName);
                                                    if (nbMaxItem > 0)
                                                    {
                                                        if (_networkDeviceArray[i].currentInventory.nbTagRemoved > nbMaxItem)
                                                        {
                                                            UserClassTemplate tmpUtc = new UserClassTemplate();
                                                            tmpUtc.firstName = _firstName;
                                                            tmpUtc.lastName = _lastName;
                                                            string spData = _networkDeviceArray[i].currentInventory.nbTagRemoved + " item(s) on " + nbMaxItem + " allowed";
                                                            _db.storeAlert(AlertType.AT_Remove_Too_Many_Items, _networkDeviceArray[i].infoDev, tmpUtc, spData);
                                                            AlertMgtClass.treatAlert(AlertType.AT_Remove_Too_Many_Items, _networkDeviceArray[i].infoDev, tmpUtc, _networkDeviceArray[i].currentInventory, spData, BShowAlert);
                                                        }
                                                    }
                                                }
                                                if (_alertValues != null)
                                                {
                                                    string columnValueName = _db.GetColumnNameForAlert(AlertType.AT_Limit_Value_Exceed);
                                                    if (!string.IsNullOrEmpty(columnValueName))
                                                    {
                                                        double userLimit = _db.getUserMaxRemoveValue(_firstName, _lastName);
                                                        if (userLimit > 0)
                                                        {
                                                            double sumValue = ProcessValueAlert(_networkDeviceArray[i].currentInventory.dtTagRemove, columnValueName);

                                                            if (sumValue > userLimit)
                                                            {
                                                                UserClassTemplate tmpUtc = new UserClassTemplate();
                                                                tmpUtc.firstName = _firstName;
                                                                tmpUtc.lastName = _lastName;
                                                                string spData = string.Format(ResStrings.str_0_on_1_allowed, sumValue, userLimit);
                                                                _db.storeAlert(AlertType.AT_Limit_Value_Exceed, _networkDeviceArray[i].infoDev, tmpUtc, spData);
                                                                AlertMgtClass.treatAlert(AlertType.AT_Limit_Value_Exceed, _networkDeviceArray[i].infoDev, tmpUtc, _networkDeviceArray[i].currentInventory, spData, BShowAlert);
                                                            }
                                                        }
                                                    }
                                                }
                                                if (_alertStock != null)
                                                {
                                                    string colAlert = _db.GetColumnNameForAlert(AlertType.AT_Stock_Limit);
                                                    if (!string.IsNullOrEmpty(colAlert))
                                                    {
                                                        int nbMin = 0;
                                                        int.TryParse(_alertStock.alertData, out nbMin);

                                                        Dictionary<string, int> stockProduct = new Dictionary<string, int>();
                                                        foreach (DataRow oRow in _networkDeviceArray[i].currentInventory.dtTagAll.Rows)
                                                        {

                                                            string productName = (string)oRow[colAlert];

                                                            if (string.IsNullOrEmpty(productName)) continue;
                                                            if (productName.Equals(ResStrings.str_Unreferenced)) continue;
                                                            if (stockProduct.ContainsKey(productName))
                                                                stockProduct[productName]++;
                                                            else
                                                                stockProduct.Add(productName, 1);
                                                        }
                                                        foreach (DataRow oRow in _networkDeviceArray[i].currentInventory.dtTagRemove.Rows)
                                                        {

                                                            string productName = (string)oRow[colAlert];
                                                            if (string.IsNullOrEmpty(productName)) continue;
                                                            if (productName.Equals(ResStrings.str_Unreferenced)) continue;
                                                            if (!stockProduct.ContainsKey(productName))
                                                                stockProduct.Add(productName, 0);
                                                        }

                                                        string spData = string.Empty;
                                                        foreach (KeyValuePair<string, int> pair in stockProduct)
                                                        {
                                                            if (pair.Value <= nbMin)
                                                            {
                                                                if (spData.Length > 0) spData += ";";
                                                                spData += pair.Key + ";" + pair.Value;
                                                            }
                                                        }

                                                        if (!string.IsNullOrEmpty(spData))
                                                        {
                                                            _db.storeAlert(AlertType.AT_Stock_Limit, _networkDeviceArray[i].infoDev, null, spData);
                                                            AlertMgtClass.treatAlert(AlertType.AT_Stock_Limit, _networkDeviceArray[i].infoDev, null, _networkDeviceArray[i].currentInventory, spData, BShowAlert);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                
                                Invoke((MethodInvoker) delegate
                                {
                                    Debug.WriteLine("Request RefreshInventory " + DateTime.Now.ToString("hh:mm:ss.fff"));
                                    RefreshInventory();
                                    Debug.WriteLine("End Request RefreshInventory " + DateTime.Now.ToString("hh:mm:ss.fff"));
                                });
                                
                                Invoke((MethodInvoker)delegate { _sip.Hide(); });
                                Thread.Sleep(50); 
                                Application.DoEvents(); 
                                Thread.Sleep(50);
                                Invoke((MethodInvoker)delegate { new ScanFinish().Show(); });

                            
                                   
                                _networkDeviceArray[i].netDeviceStatus = DeviceStatus.DS_Ready;

                                Debug.WriteLine("End TCP completed " + DateTime.Now.ToString("hh:mm:ss.fff"));
                                
                                return;
                            case rfidTcpNotArg.ReaderTcpNotify.RN_TempEvent:
                                if (!_networkDeviceArray[i].bUseTcpNotification)
                                    return;
                                return;
                            case rfidTcpNotArg.ReaderTcpNotify.RN_TempEventChanged:
                                if (!_networkDeviceArray[i].bUseTcpNotification)
                                    return;

                                return;
                        }

                    }
                }
            }
        }

        // for arm device
        private void tcp_DeviceEventTcp(SDK_SC_RfidReader.rfidReaderArgs arg)
        {

            if (_networkDeviceArray == null) return;
            if (_networkDeviceArray.Length > 0)
            {
                for (int i = 0; i < _networkDeviceArray.Length; i++)
                {
                    if (_networkDeviceArray[i].infoDev.SerialRFID == arg.SerialNumber)
                    {
                        _networkDeviceArray[i].netConnectionStatus = ConnectionStatus.CS_Connected;
                        switch (arg.RN_Value)
                        {

                            case rfidReaderArgs.ReaderNotify.RN_Door_Opened:
                                _networkDeviceArray[i].netDeviceStatus = DeviceStatus.DS_DoorOpen;
                             return;

                            case rfidReaderArgs.ReaderNotify.RN_ScanStarted:


                                if (!_networkDeviceArray[i].bUseTcpNotification)
                                    return;
                                _networkDeviceArray[i].netDeviceStatus = DeviceStatus.DS_InScan;
                                _networkDeviceArray[i].cptPoll = 0;
                                // UpdateTreeView();
                                if (IsHandleCreated)
                                {
                                    this.BeginInvoke((MethodInvoker) delegate
                                    {
                                        if (!_sip.Visible)
                                        {
                                            _sip.SetInfo(_networkDeviceArray[i].infoDev);
                                            _sip.Show();
                                            if (msgBox != null)
                                            {
                                                if (msgBox.name == _networkDeviceArray[i].infoDev.DeviceName)
                                                    msgBox.Close();

                                            }
                                        }
                                        timerRefreshScan.Interval = 500;
                                        timerRefreshScan.Enabled = true;
                                    });
                                }

                                return;
                            case rfidReaderArgs.ReaderNotify.RN_ScanCompleted:

                                Debug.WriteLine("Request RN_ScanCompleted :" + DateTime.Now.ToString("hh:mm:ss.fff"));
                                if (!_networkDeviceArray[i].bUseTcpNotification)
                                    return;

                                //StoreNetworkinventory(networkDeviceArray[i], invData);



                                Debug.WriteLine("Before store :" + DateTime.Now.ToString("hh:mm:ss.fff"));
                                DateTime lastScanIp = DateTime.MaxValue;
                                TcpIpClient tcp = DeviceClientTcp[_networkDeviceArray[i].infoDev.IP_Server];
                                tcp.getLastDateScan(_networkDeviceArray[i].infoDev.IP_Server, _networkDeviceArray[i].infoDev.Port_Server, _networkDeviceArray[i].infoDev.SerialRFID, out lastScanIp);
                                if (lastScanIp > _networkDeviceArray[i].lastProcessInventoryGmtDate)
                                {
                                    InventoryData[] inventoryArray = null;
                                    TcpIpClient.RetCode ret = tcp.getScanFromDateWithDB(_networkDeviceArray[i].infoDev.IP_Server, _networkDeviceArray[i].infoDev.Port_Server, _networkDeviceArray[i].infoDev.SerialRFID, _networkDeviceArray[i].currentInventory.eventDate.ToUniversalTime(), out inventoryArray);
                                    if (ret == TcpIpClient.RetCode.RC_Succeed)
                                    {
                                        if (inventoryArray != null)
                                        {
                                            foreach (InventoryData inv in inventoryArray)
                                            {
                                                DateTime dtUtc = DateTime.SpecifyKind(inv.eventDate, DateTimeKind.Utc);
                                                inv.eventDate = dtUtc.ToLocalTime();
                                                StoreNetworkinventory(_networkDeviceArray[i], inv);
                                            }


                                            Debug.WriteLine("Afterstore :" + DateTime.Now.ToString("hh:mm:ss.fff"));

                                            //alert Remove time exceed
                                            bool bReaderForRemovetoLong = false;
                                            if (_removedReaderDevice.Contains(_networkDeviceArray[i].infoDev.SerialRFID)) bReaderForRemovetoLong = true;

                                            if (_bUseAlarm && _networkDeviceArray[i].currentInventory.nbTagRemoved > 0 && _maxTimeBeforeAlertRemoved > 0 && bReaderForRemovetoLong)
                                            {
                                                TimeSpan ts = new TimeSpan(0, _maxTimeBeforeAlertRemoved, 0);
                                                DateTime dateToTest = DateTime.Now.Add(ts);
                                                _removedInventoryAlert.Add(dateToTest, _networkDeviceArray[i]);
                                            }
                                            #region BloodPatient

                                            if ((!_selectedPatient.Equals(ResStrings.str_None))
                                                    && (_networkDeviceArray[i].currentInventory.dtTagRemove.Rows.Count > 0))
                                            {
                                                if (_alertBloodPatient != null)
                                                {
                                                    string columnPatient = _db.GetColumnNameForAlert(AlertType.AT_Bad_Blood_Patient);
                                                    if (!string.IsNullOrEmpty(columnPatient))
                                                    {

                                                        ArrayList badBlood = new ArrayList();
                                                        foreach (DataRow oRow in _networkDeviceArray[i].currentInventory.dtTagRemove.Rows)
                                                        {
                                                            if (oRow[columnPatient] != DBNull.Value)
                                                            {
                                                                if (!_selectedPatient.Equals((string)oRow[columnPatient]))
                                                                    badBlood.Add(oRow);
                                                            }
                                                        }

                                                        if (badBlood.Count > 0)
                                                        {
                                                            string spData = string.Empty;
                                                            spData += _selectedPatient;
                                                            foreach (DataRow oRow in badBlood)
                                                            {
                                                                string rowValue = string.Empty;
                                                                for (int x = 0; x < oRow.ItemArray.Length; x++)
                                                                {
                                                                    if (rowValue.Length > 0) rowValue += " - ";
                                                                    rowValue += oRow[x].ToString();
                                                                }

                                                                if (spData.Length > 0) spData += ";";
                                                                spData += rowValue;
                                                            }
                                                            Invoke((MethodInvoker)delegate { new BadStopPatient().ShowDialog(); });
                                                            _db.storeAlert(AlertType.AT_Bad_Blood_Patient, _networkDeviceArray[i].infoDev, null, spData);
                                                            AlertMgtClass.treatAlert(AlertType.AT_Bad_Blood_Patient, _networkDeviceArray[i].infoDev, null, _networkDeviceArray[i].currentInventory, spData, BShowAlert);
                                                        }
                                                    }
                                                }
                                            }
                                            #endregion

                                            // alert remove nb item
                                            if (_bUseAlarm && _networkDeviceArray[i].currentInventory.bUserScan)
                                            {

                                                if (_alertItems != null)
                                                {

                                                    int nbMaxItem = _db.getUserMaxRemovedItem(_networkDeviceArray[i].currentInventory.userFirstName, _networkDeviceArray[i].currentInventory.userLastName);
                                                    if (nbMaxItem > 0)
                                                    {
                                                        if (_networkDeviceArray[i].currentInventory.nbTagRemoved > nbMaxItem)
                                                        {
                                                            UserClassTemplate tmpUtc = new UserClassTemplate();
                                                            tmpUtc.firstName = _firstName;
                                                            tmpUtc.lastName = _lastName;
                                                            string spData = _networkDeviceArray[i].currentInventory.nbTagRemoved + " item(s) on " + nbMaxItem + " allowed";
                                                            _db.storeAlert(AlertType.AT_Remove_Too_Many_Items, _networkDeviceArray[i].infoDev, tmpUtc, spData);
                                                            AlertMgtClass.treatAlert(AlertType.AT_Remove_Too_Many_Items, _networkDeviceArray[i].infoDev, tmpUtc, _networkDeviceArray[i].currentInventory, spData, BShowAlert);
                                                        }
                                                    }
                                                }
                                                if (_alertValues != null)
                                                {
                                                    string columnValueName = _db.GetColumnNameForAlert(AlertType.AT_Limit_Value_Exceed);
                                                    if (!string.IsNullOrEmpty(columnValueName))
                                                    {
                                                        double userLimit = _db.getUserMaxRemoveValue(_firstName, _lastName);
                                                        if (userLimit > 0)
                                                        {
                                                            double sumValue = ProcessValueAlert(_networkDeviceArray[i].currentInventory.dtTagRemove, columnValueName);

                                                            if (sumValue > userLimit)
                                                            {
                                                                UserClassTemplate tmpUtc = new UserClassTemplate();
                                                                tmpUtc.firstName = _firstName;
                                                                tmpUtc.lastName = _lastName;
                                                                string spData = string.Format(ResStrings.str_0_on_1_allowed, sumValue, userLimit);
                                                                _db.storeAlert(AlertType.AT_Limit_Value_Exceed, _networkDeviceArray[i].infoDev, tmpUtc, spData);
                                                                AlertMgtClass.treatAlert(AlertType.AT_Limit_Value_Exceed, _networkDeviceArray[i].infoDev, tmpUtc, _networkDeviceArray[i].currentInventory, spData, BShowAlert);
                                                            }
                                                        }
                                                    }
                                                }
                                                if (_alertStock != null)
                                                {
                                                    string colAlert = _db.GetColumnNameForAlert(AlertType.AT_Stock_Limit);
                                                    if (!string.IsNullOrEmpty(colAlert))
                                                    {
                                                        int nbMin = 0;
                                                        int.TryParse(_alertStock.alertData, out nbMin);

                                                        Dictionary<string, int> stockProduct = new Dictionary<string, int>();
                                                        foreach (DataRow oRow in _networkDeviceArray[i].currentInventory.dtTagAll.Rows)
                                                        {

                                                            string productName = (string)oRow[colAlert];

                                                            if (string.IsNullOrEmpty(productName)) continue;
                                                            if (productName.Equals(ResStrings.str_Unreferenced)) continue;
                                                            if (stockProduct.ContainsKey(productName))
                                                                stockProduct[productName]++;
                                                            else
                                                                stockProduct.Add(productName, 1);
                                                        }
                                                        foreach (DataRow oRow in _networkDeviceArray[i].currentInventory.dtTagRemove.Rows)
                                                        {

                                                            string productName = (string)oRow[colAlert];
                                                            if (string.IsNullOrEmpty(productName)) continue;
                                                            if (productName.Equals(ResStrings.str_Unreferenced)) continue;
                                                            if (!stockProduct.ContainsKey(productName))
                                                                stockProduct.Add(productName, 0);
                                                        }

                                                        string spData = string.Empty;
                                                        foreach (KeyValuePair<string, int> pair in stockProduct)
                                                        {
                                                            if (pair.Value <= nbMin)
                                                            {
                                                                if (spData.Length > 0) spData += ";";
                                                                spData += pair.Key + ";" + pair.Value;
                                                            }
                                                        }

                                                        if (!string.IsNullOrEmpty(spData))
                                                        {
                                                            _db.storeAlert(AlertType.AT_Stock_Limit, _networkDeviceArray[i].infoDev, null, spData);
                                                            AlertMgtClass.treatAlert(AlertType.AT_Stock_Limit, _networkDeviceArray[i].infoDev, null, _networkDeviceArray[i].currentInventory, spData, BShowAlert);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                Invoke((MethodInvoker)delegate
                                {
                                    Debug.WriteLine("Request RefreshInventory " + DateTime.Now.ToString("hh:mm:ss.fff"));
                                    RefreshInventory();
                                    Debug.WriteLine("End Request RefreshInventory " + DateTime.Now.ToString("hh:mm:ss.fff"));
                                });

                                Invoke((MethodInvoker)delegate { _sip.Hide(); });
                                Thread.Sleep(50);
                                Application.DoEvents();
                                Thread.Sleep(50);
                                Invoke((MethodInvoker)delegate { new ScanFinish().Show(); });



                                _networkDeviceArray[i].netDeviceStatus = DeviceStatus.DS_Ready;

                                Debug.WriteLine("End TCP completed " + DateTime.Now.ToString("hh:mm:ss.fff"));
                                return;
                        }

                    }
                }
            }
        }


        #endregion
        #region ExportSQL
        private void GetExportSqlInfo()
        {
            string connectionstring;
            string tableName;
            int bEnable;
            DBClassSQLite dbSqlite = new DBClassSQLite();
            dbSqlite.OpenDB();
            if (dbSqlite.getExportInfo(1, out connectionstring, out tableName,out bEnable))
            {
                if (bEnable == 1)
                {                   
                    _bUseSqlExport = true;
                    _sqlExportThreadInfo = new SqlThreadHandle("00000000");
                }
                else
                    _bUseSqlExport = false;
            }
            dbSqlite.CloseDB();
            dbSqlite = null;
        }       
        public class SqlThreadHandle
        {
            //private object lockSQL = new object();
            string _serialRfid;
            public bool IsRunning = false;
            public SqlThreadHandle(string serialRfid)
            {
                _serialRfid = serialRfid;
            }
            public void SetParam(string serialRfid)
            {
                _serialRfid = serialRfid;
            }

            public  void ThreadSqlLoop()
            {
                IsRunning = true;
                DBClassSQLite dbSqlite = null;
                ExportInventory exportSql = null;
                DateTime lastExported = DateTime.MaxValue;

                MainDBClass mainDb = null;

                try
                {
                    dbSqlite = new DBClassSQLite();
                    dbSqlite.OpenDB();
                    string connectionstring;
                    string tableName;
                    int bEnable;
                    if (dbSqlite.getExportInfo(1, out connectionstring,out tableName, out bEnable))
                    {
                        if (bEnable == 1)
                        {
                            exportSql = new ExportInventory(UtilSqlServer.ConvertConnectionString(connectionstring), tableName);
                            if ((exportSql.OpenDb()) && (exportSql.IsTableExist()))
                            {
                                
                                lastExported = exportSql.GetLastScanExported(_serialRfid);

                                mainDb = new MainDBClass();
                                mainDb.OpenDB();

                                string[] notExportInv = mainDb.GetInventoryAfter(_serialRfid, lastExported.ToString("yyyy-MM-dd HH:mm:ssZ"));

                                if (notExportInv != null)
                                {
                                    foreach (string invToStore in notExportInv)
                                    {
                                        BinaryFormatter bf = new BinaryFormatter();
                                        MemoryStream mem = new MemoryStream(Convert.FromBase64String(invToStore));
                                        StoredInventoryData sid = (StoredInventoryData)bf.Deserialize(mem);
                                        InventoryData inventoryToStore = ConvertInventory.ConvertForUse(sid, null);
                                        exportSql.StoreScan(inventoryToStore);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exp)
                {
                    ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (dbSqlite != null)
                    {
                        if (dbSqlite.isOpen())
                            dbSqlite.CloseDB();
                    }
                    if (exportSql != null)
                    {
                        if (exportSql.IsOpen())
                            exportSql.CloseDb();
                    }
                    if (mainDb != null)
                    {
                        if (mainDb.isOpen())
                            mainDb.CloseDB();
                    }
                }
                IsRunning = false;                
            }
        }       
        #endregion
        #region php
        public class PhpThreadHandle
        {
            //private object lockSQL = new object();
            string _serialRfid;
            public bool IsRunning = false;
            public PhpThreadHandle(string serialRfid)
            {
                _serialRfid = serialRfid;
            }
            public void SetParam(string serialRfid)
            {
                _serialRfid = serialRfid;
            }

            public void ThreadSqlLoop()
            {
                IsRunning = true;
                MainDBClass db = new MainDBClass();
                string res;
                try
                {
                    db.OpenDB();
                   //Get Last Scan ID
                   // send all scan not already stored

                    int lastId = -1;
                    if (phpExport.getLastScanId(_serialRfid, out lastId))
                    {
                        if (lastId == -1)
                        {
                            InventoryData inv = db.GetLastScan(_serialRfid);
                            if (inv != null)
                                phpExport.exportPhp(inv, out res);
                        }
                        else
                        {
                            InventoryData inv = db.GetLastScan(_serialRfid);

                            for (int loop = lastId + 1; loop <= inv.IdScanEvent ; loop++)
                            {
                                InventoryData inv2 = db.GetLastScanFromID(_serialRfid,loop);
                                if (inv2 != null)
                                {
                                    if (! phpExport.exportPhp(inv2, out res))
                                        break;
                                }
                            }
                        }
                    }
                }
                catch (Exception exp)
                {
                    ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    db.CloseDB();
                }
                IsRunning = false;
            }
        }       
        #endregion
        #region cvsExport
        private void initCsvWatcher()
        {

            pathCsvReport = pathCsvFolder + @"\inventory" + cvsMachineId.PadLeft(6,'0') + ".txt";
            pathCsvInventory = pathCsvFolder + @"\start" + cvsMachineId.PadLeft(6, '0') + ".txt";
            pathCsvInScan = pathCsvFolder + @"\deviceInScan" + cvsMachineId.PadLeft(6, '0') + ".txt";
            pathCsvLed = pathCsvFolder + @"\led" + cvsMachineId.PadLeft(6, '0') + ".txt";
            pathCsvError = pathCsvFolder + @"\error" + cvsMachineId.PadLeft(6, '0') + ".txt";
            pathCsvLog = pathCsvFolder + @"\log" + cvsMachineId.PadLeft(6, '0') + ".txt";

            csvWatcher = new FileSystemWatcher();
            csvWatcher.Path = pathCsvFolder;
            csvWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite| NotifyFilters.FileName | NotifyFilters.DirectoryName;
            csvWatcher.Filter = "*.txt";
            //csvWatcher.Changed += new FileSystemEventHandler(OnCreated);
            csvWatcher.Created += new FileSystemEventHandler(OnCreated);
            csvWatcher.Deleted += new FileSystemEventHandler(OnDeleted);
            csvWatcher.EnableRaisingEvents = false;
        }
        private void OnCreated(object source, FileSystemEventArgs e)
        {
            //Copies file to another directory.
            string fileName = e.Name;
            WatcherChangeTypes action = e.ChangeType;
            try
            {
                csvWatcher.EnableRaisingEvents = false;
                AddCsvLog(fileName + " / " + action.ToString());

                if (fileName.Equals("start" + cvsMachineId.PadLeft(6, '0') + ".txt"))
                {
                    if ((action == WatcherChangeTypes.Created) || (action == WatcherChangeTypes.Changed))
                    {
                        if (File.Exists(pathCsvError))
                            File.Delete(pathCsvError);
                        if (!bScanforCSV) // do a scan
                        {
                            if ((_localDeviceArray != null) && (_localDeviceArray.Length > 0))
                            {

                                if (_localDeviceArray[0].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected)
                                {
                                    bScanforCSV = true;
                                    _localDeviceArray[0].rfidDev.get_RFID_Device.StopField();
                                    _localDeviceArray[0].rfidDev.DeviceStatus = DeviceStatus.DS_Ready;
                                }

                                if ((_localDeviceArray[0].rfidDev.DeviceStatus == DeviceStatus.DS_Ready) &&
                                    (_localDeviceArray[0].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected))
                                {
                                    bScanforCSV = true;
                                    _bFirstScanAccumulate = true;
                                    _bUserScan = false;
                                    _firstName = ResStrings.str_Manual;
                                    _lastName = ResStrings.str_Scan;
                                    _lastAccessType = AccessType.AT_NONE;
                                    LastBadge = string.Empty;
                                    _lastDoorUser = DoorInfo.DI_NO_DOOR;
                                    _localDeviceArray[0].rfidDev.ScanDevice(false);
                                    AddCsvLog("Scan requested");

                                }
                                else
                                {
                                    StringBuilder sb = new StringBuilder();
                                    sb.Append("Error device - Query scan ");
                                    sb.Append("\r\nConnection status : " + _localDeviceArray[0].rfidDev.ConnectionStatus);
                                    sb.Append("\r\nDevice status : " + _localDeviceArray[0].rfidDev.DeviceStatus);
                                    ReportCsvError(sb.ToString());
                                }
                            }
                            else
                            {
                                ReportCsvError("No reader Exist");
                            }
                        }
                        else
                        {
                            ReportCsvError("device alreay in scan");
                        }
                    }

                }
                if (fileName.Equals("led" + cvsMachineId.PadLeft(6, '0') + ".txt"))
                {
                    //do led
                    if (File.Exists(pathCsvError))
                        File.Delete(pathCsvError);
                    int cpt = 10;
                    List<String> tagsToLight = null;
                    while (cpt > 0)
                    {
                        Thread.Sleep(200);
                        try
                        {
                            cpt--;
                            tagsToLight = File.ReadAllLines(pathCsvLed).ToList();
                            break;
                        }
                        catch
                        {
                            //Avoid  10 times the IO exception
                            Debug.WriteLine("Error Led try " + cpt);
                        }

                    }
                    if ((tagsToLight == null) || (tagsToLight.Count == 0))
                    {
                        ReportCsvError("Error Led : Unable to read file or file is empty");
                        return;
                    }

                    for (int loop = 0; loop < tagsToLight.Count; loop ++)
                    {
                        if (string.IsNullOrEmpty(tagsToLight[loop]))
                        {
                            tagsToLight.RemoveAt(loop);
                            loop = 0;
                        }
                    }


                    if ((_localDeviceArray != null) && (_localDeviceArray.Length > 0))
                    {
                        if (_localDeviceArray[0].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected)
                        {
                            bScanforCSV = true;
                            _localDeviceArray[0].rfidDev.get_RFID_Device.StopField();
                            _localDeviceArray[0].rfidDev.DeviceStatus = DeviceStatus.DS_Ready;
                        }

                        if ((_localDeviceArray[0].rfidDev.DeviceStatus == DeviceStatus.DS_Ready) &&
                            (_localDeviceArray[0].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected))
                        {
                            bScanforCSV = true;
                            bool ret = _localDeviceArray[0].rfidDev.TestLighting(tagsToLight);

                            if (!ret)
                            {
                                if (!bFisrtScanDone)
                                    ReportCsvError(
                                        "Error In Led Feature - A Inventory is mandatory before each Led Request");
                                else
                                    ReportCsvError("Error In Led Feature - Not all requested Tag Found");
                            }
                            else
                            {
                                _localDeviceArray[0].rfidDev.DeviceStatus = DeviceStatus.DS_LedOn;
                                AddCsvLog("Led On");
                            }

                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("Error device - Query Led");
                            sb.Append("\r\nConnection status : " + _localDeviceArray[0].rfidDev.ConnectionStatus);
                            sb.Append("\r\nDevice status : " + _localDeviceArray[0].rfidDev.DeviceStatus);
                            ReportCsvError(sb.ToString());

                        }
                    }
                    else
                    {
                        ReportCsvError("No reader Exist");

                    }
                }
            }
            catch
            {
                
            }
            finally
            {
                csvWatcher.EnableRaisingEvents = true;
            }
        }
        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            string fileName = e.Name;
            WatcherChangeTypes action = e.ChangeType;
            AddCsvLog(fileName + " / " + action.ToString());
            if (fileName.Equals("led" + cvsMachineId.PadLeft(6, '0') + ".txt"))
            {
                if (action == WatcherChangeTypes.Deleted)
                {
                    //Todo stop the Led
                    if ((_localDeviceArray != null) && (_localDeviceArray.Length > 0))
                    {
                        if (_localDeviceArray[0].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected)
                        {
                            bScanforCSV = false;
                            _localDeviceArray[0].rfidDev.StopLightingLeds();
                            _localDeviceArray[0].rfidDev.get_RFID_Device.StopField();
                            _localDeviceArray[0].rfidDev.DeviceStatus = DeviceStatus.DS_Ready;
                            AddCsvLog("Stop Led");
                       }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("Error device \r\n");
                            sb.Append("\r\nConnection status : " + _localDeviceArray[0].rfidDev.ConnectionStatus);
                            sb.Append("\r\nDevice status : " + _localDeviceArray[0].rfidDev.DeviceStatus);
                            ReportCsvError(sb.ToString());
                          
                        }
                    }
                    else
                    {
                        ReportCsvError("No reader Exist");
                    }
                }
            }
        }
        private void ReportCsvInventory(InventoryData inv)
        {
            bFisrtScanDone = true;
            StringBuilder sb = new StringBuilder();
            sb.Append(inv.userFirstName + " " + inv.userLastName + "\r\n");
            sb.Append(inv.eventDate.ToLocalTime().ToString(("yyyy_MM_dd_hh_mm_ss"))+ "\r\n");
            foreach (string uid in inv.listTagAll)
                sb.Append(uid + "\r\n");
            try
            {
                if (File.Exists(pathCsvReport))
                    File.Delete(pathCsvReport);

                File.AppendAllText(pathCsvReport, sb.ToString());

                if (File.Exists(pathCsvInventory))
                    File.Delete(pathCsvInventory);
            }
            catch 
            {
               ReportCsvError(pathCsvReport + " Error Write : Inventory Not reachable");
               string pathInv = pathCsvReport = pathCsvFolder + @"\inventory" + cvsMachineId.PadLeft(6, '0') + inv.eventDate.ToLocalTime().ToString("_yyyy_MM_dd_hh_mm_ss") + ".txt";
               File.AppendAllText(pathInv, sb.ToString());
            }
            if (File.Exists(pathCsvInScan))
                File.Delete(pathCsvInScan);

            bScanforCSV = false;
        }
        private void ReportCsvError(string errMsg)
        {
            AddCsvLog(errMsg);
            File.AppendAllText(pathCsvError, errMsg);
            if (File.Exists(pathCsvInventory))
                File.Delete(pathCsvInventory);
            bScanforCSV = false;
        }

        private void AddCsvLog(string Msg)
        {
            const int nbMaxLine = 100;
            if (File.Exists(pathCsvLog))
            {
                string[] lines = System.IO.File.ReadAllLines(pathCsvLog).ToArray();
                if (lines.Count() > nbMaxLine)
                System.IO.File.WriteAllLines(pathCsvLog, lines.Skip(lines.Count() - nbMaxLine).ToArray());
            }
            string str = string.Format("{0} : {1} \r\n", DateTime.Now.ToString("hh:mm:ss:fff") ,Msg);
            File.AppendAllText(pathCsvLog, str);
        }
        private void CsvPutInScan()
        {
            File.AppendAllText(pathCsvInScan, "InScan");
        }
        #endregion

        private bool CheckCriteria()
        {
            try
            {
                DataTable tmpDt = (DataTable)dataListView.DataSource;
                string selectString = "NOT (" + _bti.Criteria + ") AND (" + ResStrings.str_Event + " = '" + ResStrings.str_Present + "' OR " + ResStrings.str_Event + " = '" + ResStrings.str_Added + "')";
                DataView dv = new DataView(tmpDt);
                dv.RowFilter = selectString;

                if (dv.Count > 0)
                {
                    //string mes = string.Format(ResStrings.str_CheckCriteria,dv.Count,_bti.Criteria);
                    //DialogResult res = MessageBox.Show(this,mes, ResStrings.str_Criteria_info, MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                    //if (res == DialogResult.Yes)
                    //{
                        if ((_selectedReader >= 0) & (_selectedReader < _nbLocalDevice)) //reader local
                        {
                            BadCriteriaForm bcf = new BadCriteriaForm(dv, _columnInfo,_localDeviceArray[_selectedReader].rfidDev);
                            bcf.Show();
                        }
                        else
                        {
                           BadCriteriaForm bcf = new BadCriteriaForm(dv, _columnInfo,null);
                           bcf.Show();
                        }
                      
                   // }
                    return false; // Bad Criteria found
                }
               
            }
            catch(Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }
            return true; // criteria Ok or error

        }      
        public static bool CheckIpAddr(string ipAddress)
        {
            return Regex.IsMatch(ipAddress,
            @"^(25[0-5]|2[0-4]\d|[0-1]?\d?\d)(\.(25[0-5]|2[0-4]\d|[0-1]?\d?\d)){3}$");
        }      
        private DateTime truncateMs(DateTime dt)
        {
            return  dt.AddMilliseconds(-dt.Millisecond);
        }
        private void pictureBoxPresent_Paint(object sender, PaintEventArgs e)
        {
            TextRenderer.DrawText(e.Graphics,
                          labelPresent.Text,
                          labelPresent.Font,
                          new Point(12, 12),
                          Color.White);
        }
        private void pictureBoxAdded_Paint(object sender, PaintEventArgs e)
        {
            TextRenderer.DrawText(e.Graphics,
                         labelAdded.Text,
                         labelAdded.Font,
                         new Point(12, 12),
                         Color.White);
        }
        private void pictureBoxRemoved_Paint(object sender, PaintEventArgs e)
        {
            TextRenderer.DrawText(e.Graphics,
                         labelRemoved.Text,
                         labelRemoved.Font,
                         new Point(12, 12),
                         Color.White);
        }
        private void tabControlInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Invoke((MethodInvoker)(() => UpdateScanHistory(null)));
            Debug.WriteLine("End update history :" + DateTime.Now.ToString("hh:mm:ss.fff"));
        }

        private void timerOldEntry_Tick(object sender, EventArgs e)
        {
            //Eric test KSA Not remove old Inventory
            if (!File.Exists(@"C:\temp\AutoScan.txt"))
            {
                if (_bShowServer)
                    DeleteOldEntry();
            }
        }

        private void renewFingerprintToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (SelectedNetworkDevice.netConnectionStatus == ConnectionStatus.CS_Connected)
            {
                TcpIpClient _tcpTmp = new TcpIpClient();
                _tcpTmp.renewFP(SelectedNetworkDevice.infoDev.IP_Server, SelectedNetworkDevice.infoDev.Port_Server);
                
            }
        }

        private void contextMenuStripDebug_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }


        private volatile bool bScanFromTimer = false;
        private void timerAutoPad_Tick(object sender, EventArgs e)
        {
            timerAutoPad.Enabled = false;
            if ((_selectedReader >= 0) & (_selectedReader < _nbLocalDevice)) //reader local
            {
                if ((_localDeviceArray[_selectedReader].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                    (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
                {
                    if ((_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_STR) &&
                        (_localDeviceArray[_selectedReader].rfidDev.get_RFID_Device.HardwareVersion.StartsWith("11")))
                    {
                        RFID_Device currentDevice = _localDeviceArray[_selectedReader].rfidDev;
                        if (currentDevice.DeviceStatus == DeviceStatus.DS_Ready)
                        {
                            bScanFromTimer = true;
                            currentDevice.ScanDevice();

                        }
                    }
                }

            }
            if (!bScanFromTimer)
                timerAutoPad.Enabled = true;
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
    public class InventoryAndDbClass
    {
        public bool BStoreTagEvent;
        public DeviceInfo Device;
        public InventoryData Data;
        
        public MainDBClass Db;
        
        public InventoryAndDbClass(DeviceInfo device, InventoryData data, MainDBClass db, bool bStoreTagEvent)
       
        {
            Device = device;
            Data = data;
            Db = db;
            BStoreTagEvent = bStoreTagEvent;
        }
    }    
    public class ToolStripCheckedBox : ToolStripControlHost
    {
        public ToolStripCheckedBox()
            : base(new CheckBox())
        {
        }        
    }
}


