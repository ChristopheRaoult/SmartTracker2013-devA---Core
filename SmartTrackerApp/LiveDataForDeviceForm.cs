using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Linq;
using System.Text.RegularExpressions;
using DBClass_SQLServer;
using DB_Class_SQLite;
using DBClass;
using DataClass;
using ErrorMessage;
using SDK_SC_AccessControl;
using SDK_SC_Fingerprint;
using SDK_SC_RfidReader;
using SDK_SC_RFID_Devices;
using SDK_SC_MedicalCabinet;
using smartTracker.Properties;
using TcpIP_class;
using Timer = System.Timers.Timer;

namespace smartTracker
{
    public partial class LiveDataForDeviceForm : Form
    {
        delegate void MethodInvoker();

        private msgFrm msgBox = null;
        private Stopwatch s1; 

        #region Variables
        readonly MainDBClass _db = new MainDBClass();

        Regex _myRegexUid;

     
        volatile deviceClass[] _localDeviceArray;
        UserClassTemplate[] _localUserArray;

        public TagSearchLedfrm tslf; 

        bool _bStop = false;
        private Dictionary<deviceClass, bool> CanHandleLeds = new Dictionary<deviceClass, bool>();
       
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
       
        private bool _bClockFridgeAlreadyOn;

        private DateTime _currentAlarmEvent = DateTime.Now;
        private DateTime _lastAlarmEvent = DateTime.Now.AddDays(-1.0);
      
        private DateTime _dateFridgeALarmBlocked;
        private int _timeTcpToRefresh = 10000;
     
        DoorInfo _lastDoorUser;

        readonly TcpIpServer _tcpIpServer;

        RemoteUser _ru;
        
        private double _timeZoneOffset;
        private int _nbRecordToKeep = 1000;
        private int _selIndex = -1;
        private bool _bStoreTagEvent;
        public bool BRestart = false;
        private bool _bCompareOnLotId = true;
        private bool _bUseAlarm;
       
        public bool BShowImage = true;
        public bool BShowAlert = true;
      

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

        public double MaxTempFridgeValue;
        private int _maxTimeOpenDoor;
        private bool _interruptScanWithFp;
        public Timer ClockFridge = new Timer();
        public Timer ClockAlertRemoved = new Timer();
        private readonly object _locker = new object();
        private static readonly Object LogStore = new Object();

        private double TimeZoneOffset = 1.0;
       
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


        private SynchronizedDevice syncDevice = null;
        public bool bUseSynchonisation = false;
        public string DeviceIpRight = string.Empty;
        public int DevicePortRight = 6901;
        public string DeviceIpLeft = string.Empty;
        public int DevicePortLeft = 6901;
        public int TimeoutInSec = 120;

        #endregion


        #region datagridScanHistory
        DataTable _tbReaderScan;
        InventoryData[] _inventoryArray;
        private void InitReaderScanTable()
        {
            _tbReaderScan = new DataTable();
            _tbReaderScan.Columns.Add(ResStrings.str_EventDate, typeof(string));
            _tbReaderScan.Columns.Add(ResStrings.str_Serial_RFID, typeof(string));
            _tbReaderScan.Columns.Add(ResStrings.str_Reader_Name, typeof(string));
            _tbReaderScan.Columns.Add(ResStrings.str_Door_Used, typeof(string));
            _tbReaderScan.Columns.Add(ResStrings.str_First_Name, typeof(string));
            _tbReaderScan.Columns.Add(ResStrings.str_Last_Name, typeof(string));
            _tbReaderScan.Columns.Add(ResStrings.str_All, typeof(int));
            _tbReaderScan.Columns.Add(ResStrings.str_Present, typeof(int));
            _tbReaderScan.Columns.Add(ResStrings.str_Added, typeof(int));
            _tbReaderScan.Columns.Add(ResStrings.str_Removed, typeof(int));
            dataGridViewScan.DataSource = null;
            dataGridViewScan.DataSource = _tbReaderScan.DefaultView;

        }
        private void ProcessData(DeviceInfo di)
        {
            InitReaderScanTable();
           
            InventoryData[] invData = _db.GetInventory(di, null , 25);
            if (invData == null) return;
            _inventoryArray = new InventoryData[invData.Length];
            invData.CopyTo(_inventoryArray, 0);
            foreach (InventoryData dt in _inventoryArray)
            {
                try
                {
                    DeviceInfo tmpdi = _db.RecoverDevice(dt.serialNumberDevice);
                    if (tmpdi != null)
                    _tbReaderScan.Rows.Add(dt.eventDate.ToString("G"), dt.serialNumberDevice, tmpdi.DeviceName,dt.userDoor.ToString(),
                        dt.userFirstName, dt.userLastName,dt.nbTagAll, dt.nbTagPresent, 
                        dt.nbTagAdded, dt.nbTagRemoved);
                }
                catch 
                {

                }
            }
            dataGridViewScan.DataSource = null;
            dataGridViewScan.DataSource = _tbReaderScan.DefaultView;          
        }
        private void UpdateScanHistory(DeviceInfo di)
        {
            if (di == null)
            {
                if (_selectedReader == -1) return;

                if (_selectedReader >= 0)   //reader local
                {
                    if (_localDeviceArray == null) return;
                    di = _localDeviceArray[_selectedReader].infoDev;
                }
            }
            ProcessData(di);
        }
        private void dataGridViewScan_SelectionChanged(object sender, EventArgs e)
        {
           
            if (dataGridViewScan.SelectedRows.Count == 1)
            {
                int selectedData = dataGridViewScan.SelectedRows[0].Index;

                if (_selectedReader == -1) return;

                if (_selectedReader >= 0)   //reader local
                {
                    if (_localDeviceArray == null) return;
                    _localDeviceArray[_selectedReader].currentInventory = _inventoryArray[selectedData];

                    listBoxTag.Invoke((MethodInvoker)delegate { listBoxTag.Items.Clear(); });

                    foreach (string strTag in _localDeviceArray[_selectedReader].currentInventory.listTagAll)
                    {
                        string tag = strTag;
                        listBoxTag.Invoke((MethodInvoker)delegate { listBoxTag.Items.Add(tag); });
                    }
                }

                labelInventoryDate.Invoke((MethodInvoker)delegate { labelInventoryDate.Text = _inventoryArray[selectedData].eventDate.ToLocalTime().ToString("G"); });
                labelInventoryUser.Invoke((MethodInvoker)delegate {                                                                         
                                                                    labelInventoryUser.Text = string.Format("{0} {1}", _inventoryArray[selectedData].userFirstName, _inventoryArray[selectedData].userLastName); 
                                                                    if (_inventoryArray[selectedData].bUserScan)
                                                                    {
                                                                        if (_inventoryArray[selectedData].userDoor == DoorInfo.DI_SLAVE_DOOR)
                                                                            labelInventoryUser.Text += ResStrings.str_SlaveDoor;                                                                               
                                                                        else
                                                                            labelInventoryUser.Text += ResStrings.str_MasterDoor;
                                                                    }
                                                                    });
                  

                timerObjectList.Start();              
                
            }
        }
        #endregion
        #region LiveDataWindowsForDevice
        public LiveDataForDeviceForm(TcpIpServer tcpIpServer)
        {
            InitializeComponent();

            _tcpIpServer = tcpIpServer;
        }
       
        private void LiveDataForm_Load(object sender, EventArgs e)
        {
            try
            {
                timerObjectList.Stop();    
                _db.OpenDB();
                _columnInfo = _db.GetColumnInfo();
                _columnInfoFull = _db.GetdtColumnInfo();
               
                InitAlert();
                GetExportSqlInfo();
                GetTcpNotificationInfo();
            
               
                int.TryParse(ConfigurationManager.AppSettings["NbRecordToKeep"], out _nbRecordToKeep);
                int.TryParse(ConfigurationManager.AppSettings["timeTcpToRefresh"], out _timeTcpToRefresh);
                bool.TryParse(ConfigurationManager.AppSettings["bStoreEventTag"], out _bStoreTagEvent);
                bool.TryParse(ConfigurationManager.AppSettings["bCompareOnLotID"], out _bCompareOnLotId);
                bool.TryParse(ConfigurationManager.AppSettings["bShowImage"], out BShowImage);
                bool.TryParse(ConfigurationManager.AppSettings["bShowAlert"], out BShowAlert);
                bool.TryParse(ConfigurationManager.AppSettings["bUseAlarm"], out _bUseAlarm);
                bool.TryParse(ConfigurationManager.AppSettings["bInterruptScanWithFP"], out _interruptScanWithFp);

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
                    }

                }

                MaxTempFridgeValue = _db.GetMaxTempFridgeValue();
                _maxTimeOpenDoor = _db.GetDoorOpenToLongTime();  

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
            
           
                CreateLocalDevice();
                UpdateTreeView();
                UpdateScanHistory(null);

                _dateFridgeALarmBlocked = DateTime.Now;
                ClockFridge.Interval = 1000;
                ClockFridge.Elapsed += ClockFridgeTimer_Tick;

                if (treeViewDevice.Nodes.Count > 0)
                    _selectedReader = 0;

                DeleteOldEntry();

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

                //synchonisation

                bool.TryParse(ConfigurationManager.AppSettings["bSynchonizedDevice"], out bUseSynchonisation);
                DeviceIpRight = ConfigurationManager.AppSettings["DeviceIpRight"];
                DeviceIpLeft = ConfigurationManager.AppSettings["DeviceIpLeft"];
                int.TryParse(ConfigurationManager.AppSettings["DevicePortRight"], out DevicePortRight);
                int.TryParse(ConfigurationManager.AppSettings["DevicePortLeft"], out DevicePortLeft);
                int.TryParse(ConfigurationManager.AppSettings["TimeoutInSec"], out TimeoutInSec);

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

            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
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

             
                _bWasInAccumulation = false;
             
                if (_selectedReader >= 0)   //reader local
                {
                    _bFirstScanAccumulate = true;
                    _bAccumulateSbr = false;
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

                          

                            toolStripButtonScan.Enabled = false;
                            toolStripButtonStopScan.Enabled = true;

                            if (_localDeviceArray[treeNodeSelected].infoDev.deviceType == DeviceType.DT_SBR)
                            {

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
                                        syncDevice.CanStartScan();
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
            if (_selectedReader >= 0)   //reader local
            {             

                if (_localDeviceArray[_selectedReader].infoDev.deviceType == DeviceType.DT_SBR)
                {
                   
                    if ((_localDeviceArray[_selectedReader].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                    (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus == DeviceStatus.DS_InScan))
                    {
                        _localDeviceArray[_selectedReader].rfidDev.StopScan();
                    }
                   
                }
                else
                {
                    if ((_localDeviceArray[_selectedReader].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                       (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus == DeviceStatus.DS_InScan))
                    {
                        _localDeviceArray[_selectedReader].rfidDev.StopScan();
                    }
                }

                UpdateTreeView();
            }

        }
        private void toolStripButtonExit_Click(object sender, EventArgs e)
        {
            _bClosing = true;
            BeginInvoke((MethodInvoker)delegate { Close(); });
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

        }
        private void LiveDataForm_Activated(object sender, EventArgs e)
        {
            
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
                            TreeNode devInfo;
                            if ((dc.rfidDev != null) && (dc.rfidDev.get_RFID_Device != null))
                            {
                                devInfo =
                                    new TreeNode(string.Format("Info : HW={0} SW={1}",
                                        dc.rfidDev.get_RFID_Device.HardwareVersion,
                                        dc.rfidDev.get_RFID_Device.FirmwareVersion));
                            }
                            else
                            {

                                devInfo = new TreeNode(string.Format("Info : HW={0} SW={1}", "0.00", "0.00"));
                            }
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
                                    ndArray = new[] { serialnode, typeNode,devInfo, conNode, rfidNode };
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
                                        ndArray = new[] { serialnode, typeNode, devInfo, conNode, rfidNode, tempDate, tempNode, tempCreation, tempMean, tempMax, tempMin };
                                    }
                                    else
                                        ndArray = new[] { serialnode, typeNode,devInfo ,  conNode, rfidNode };
                                    break;

                                case DeviceType.DT_JSC:
                                case DeviceType.DT_DSB:
                                    fpNodeMaster = new TreeNode(string.Format(ResStrings.strFP_Master_Statut_, dc.rfidDev.FPStatusMaster));
                                    ndArray = new[] { serialnode, typeNode , devInfo, conNode, rfidNode, fpNodeMaster };
                                    break;

                                case DeviceType.DT_SAS:
                                case DeviceType.DT_MSR:
                                    fpNodeMaster = new TreeNode(string.Format(ResStrings.strFP_Master_Statut_, dc.rfidDev.FPStatusMaster));
                                    fpNodeSlave = new TreeNode(string.Format(ResStrings.str_FP_Slave_Statut_, dc.rfidDev.FPStatusSlave));
                                    ndArray = new[] { serialnode, typeNode,devInfo, conNode, rfidNode, fpNodeMaster, fpNodeSlave };
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

                                 treeViewDevice.Nodes[rg].Expand();

                            }
                        }

                        if (_bClosing)
                        {
                            _localDeviceArray = null;
                           
                            Close();
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

           
            treeViewDevice.ContextMenuStrip = null;
            if (!_bWasInAccumulation)
            {
                if (!_bComeFromFridge)
                {   
                    RefreshInventory();
                    UpdateScanHistory(null);
                }
            }
            _bComeFromFridge = false;
          

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
            
        }
        private void treeViewDevice_AfterExpand(object sender, TreeViewEventArgs e)
        {
           
        }
        private void treeViewDevice_DoubleClick(object sender, EventArgs e)
        {
          
            UpdateTreeView();
            ClockFridge.Interval = 100;
            ClockFridge.Start();
           
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
      
       

        #endregion
        #region dataList
       

        private void timerObjectList_Tick(object sender, EventArgs e)
        {

            timerObjectList.Stop();
            Stopwatch s2 = Stopwatch.StartNew();
            RefreshDatalist();
            s2.Stop();
            Debug.WriteLine("Refresh datalist " + s2.Elapsed.Milliseconds + "  ms");
        }

        private void RefreshDatalist()
        {
            Invoke((MethodInvoker)delegate
            {
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    if (_selectedReader >= 0) //reader local
                    {
                        if (_localDeviceArray == null) return;
                        lock (_locker)
                        {

                            if (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus == DeviceStatus.DS_InScan)
                                return;

                            Invoke((MethodInvoker) delegate
                            {
                                labelPresent.Text = string.Format(ResStrings.str_PRESENT_format,
                                    _localDeviceArray[_selectedReader].currentInventory.listTagPresent.Count);
                                pictureBoxPresent.Refresh();
                            });
                            Invoke((MethodInvoker) delegate
                            {
                                labelAdded.Text = string.Format(ResStrings.str_ADDED_format,
                                    _localDeviceArray[_selectedReader].currentInventory.listTagAdded.Count);
                                pictureBoxAdded.Refresh();
                            });
                            Invoke((MethodInvoker) delegate
                            {
                                labelRemoved.Text = string.Format(ResStrings.str_REMOVED_format,
                                    _localDeviceArray[_selectedReader].currentInventory.listTagRemoved.Count);
                                pictureBoxRemoved.Refresh();
                            });
                            labelInventoryTagCount.Invoke(
                                (MethodInvoker)
                                    delegate
                                    {
                                        labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags,
                                            _localDeviceArray[_selectedReader].currentInventory.listTagAll.Count);
                                    });

                        }
                    }
                }

                catch
                    (Exception exp)
                {
                    ExceptionMessageBox.Show(exp);
                }
                Cursor.Current = Cursors.Default;
            });
        }

        #endregion
        #region Device
      
        private void CreateLocalDevice()
        {
            try
            {
                if (_bClosing) return;
                DeviceInfo[] tmpDeviceArray = _db.RecoverDevice(true);
                if (tmpDeviceArray != null)
                {
                    _localDeviceArray = new deviceClass[tmpDeviceArray.Length];

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
                                Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[3].Text = string.Format(ResStrings.str_Connection_Status_, getEnumDesc.GetEnumDescription(_localDeviceArray[index].rfidDev.ConnectionStatus)); });
                                Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[4].Text = string.Format(ResStrings.str_Statut_, getEnumDesc.GetEnumDescription(_localDeviceArray[index].rfidDev.DeviceStatus)); });
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
                            Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[3].Text = string.Format(ResStrings.str_Connection_Status_, getEnumDesc.GetEnumDescription(_localDeviceArray[index].rfidDev.ConnectionStatus)); });
                            Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[4].Text = string.Format(ResStrings.str_Statut_, getEnumDesc.GetEnumDescription(_localDeviceArray[index].rfidDev.DeviceStatus)); });
                            
                            Invoke((MethodInvoker) delegate { UpdateTreeView(); });
                            Invoke((MethodInvoker)delegate { treeViewDevice.Refresh(); });
                        }
                    }
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
                                _localDeviceArray[nIndex].bComeFromKZ = true;
                                _localDeviceArray[nIndex].rfidDev.DisableWaitMode();

                                _bFirstScanAccumulate = false;
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
                        _tcpIpServer.requestScanFromServer = false;
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
                  
                    if (_localDeviceArray!= null)
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
                                Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[3].Text = string.Format(ResStrings.str_Connection_Status_, getEnumDesc.GetEnumDescription(_localDeviceArray[index].rfidDev.ConnectionStatus)); });
                                Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[4].Text = string.Format(ResStrings.str_Statut_, getEnumDesc.GetEnumDescription(_localDeviceArray[index].rfidDev.DeviceStatus)); });
                                Invoke((MethodInvoker)delegate { treeViewDevice.Refresh(); });
                            }

                            if (_localDeviceArray[nIndex].infoDev.deviceType != DeviceType.DT_SBR)
                            {
                                _bFirstScanAccumulate = false;
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
                                    labelInventoryTagCount.Invoke((MethodInvoker)delegate { labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags,0); });
                                }
                            }
                            else
                            {                               
                                if (_bFirstScanAccumulate)
                                {
                                    _bFirstScanAccumulate = false; 
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
                                        }
                                    }

                                    _localDeviceArray[nIndex].currentInventory.listTagAdded.Clear();
                                    _localDeviceArray[nIndex].currentInventory.nbTagAdded = 0;
                                  
                                    

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
                                        _tcpNotificationThreadInfo.SetParam(TcpThreadHandle.TcpNotificationType.ScanStarted, _localDeviceArray[nIndex].infoDev.SerialRFID, _tcpIpServer.ServerIP, _tcpIpServer.Port, -1);
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
                    if ((_tcpIpServer != null) && (_tcpIpServer.eventScanStart!= null))
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

                                /*if ((_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_SFR) ||
                                     (_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_SBF))
                                {
                                    _localDeviceArray[nIndex].currentInventory.ListTagWithChannel = _localDeviceArray[nIndex].myFridgeCabinet.ListTagWithChannel;
                                }
                                else*/
                                 _localDeviceArray[nIndex].currentInventory.ListTagWithChannel = _localDeviceArray[nIndex].rfidDev.ListTagWithChannel;

                                if (((_bAccumulateSbr) || (_tcpIpServer.AccumulateMode)) && (_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_SBR))
                                {
                                    Thread.Sleep(100);
                                    if ((_localDeviceArray[nIndex].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected) &&
                                     (_localDeviceArray[nIndex].rfidDev.DeviceStatus == DeviceStatus.DS_Ready))
                                    {

                                        if (!_localDeviceArray[nIndex].bComeFromKZ)
                                        {
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
                                        if (_bWasInAccumulation)
                                        {
                                            _localDeviceArray[nIndex].currentInventory.listTagRemoved.Clear();
                                            _localDeviceArray[nIndex].currentInventory.nbTagRemoved = 0;
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

                                                if (!_localDeviceArray[nIndex].currentInventory.listTagAll.Contains(uidSdk))
                                                {
                                                    //Add all
                                                    _localDeviceArray[nIndex].currentInventory.listTagAll.Add(uidSdk);

                                                    //tag Present
                                                    if (!_localDeviceArray[nIndex].currentInventory.listTagPresent.Contains(uidSdk))
                                                    {
                                                        _localDeviceArray[nIndex].currentInventory.listTagPresent.Add(uidSdk);
                                                      
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {

                                            foreach (string uidSdk in _localDeviceArray[nIndex].rfidDev.get_RFID_Device.ReaderData.strListTag)
                                            {
                                               

                                                if (!_localDeviceArray[nIndex].currentInventory.listTagAll.Contains(uidSdk))
                                                {
                                                    //Add all
                                                    _localDeviceArray[nIndex].currentInventory.listTagAll.Add(uidSdk);

                                                    if (!((_bAccumulateSbr) || (_tcpIpServer.AccumulateMode)))
                                                    {
                                                        if (!_localDeviceArray[nIndex].previousInventory.listTagAll.Contains(uidSdk))
                                                        {
                                                            // Tag Added
                                                            if ((!_localDeviceArray[nIndex].currentInventory.listTagAdded.Contains(uidSdk)) &&
                                                                (!_localDeviceArray[nIndex].currentInventory.listTagPresent.Contains(uidSdk)))
                                                            {
                                                                _localDeviceArray[nIndex].currentInventory.listTagAdded.Add(uidSdk);
                                                            }

                                                        }
                                                        else
                                                        {
                                                            //tag Present
                                                            if ((!_localDeviceArray[nIndex].currentInventory.listTagPresent.Contains(uidSdk)) &&
                                                                (!_localDeviceArray[nIndex].currentInventory.listTagAdded.Contains(uidSdk)))
                                                            {
                                                                _localDeviceArray[nIndex].currentInventory.listTagPresent.Add(uidSdk);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //tag Present
                                                        if (!_localDeviceArray[nIndex].currentInventory.listTagPresent.Contains(uidSdk))
                                                        {
                                                            _localDeviceArray[nIndex].currentInventory.listTagPresent.Add(uidSdk);
                                                        }
                                                    }
                                                }
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
                                                        }
                                                    }
                                                }
                                            }
                                        
                                        // Commit and check before store       
                                       

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
                                     
                                        bool ret = StoreInventory(invCl, _bStoreTagEvent , out idscanEvent);
                                     
                                        if (ret)
                                        {
                                            _localDeviceArray[nIndex].currentInventory.IdScanEvent = idscanEvent;
                                            _localDeviceArray[nIndex].lastProcessInventoryGmtDate = truncateMs(_localDeviceArray[nIndex].currentInventory.eventDate.ToUniversalTime());
                                             Invoke((MethodInvoker) delegate
                                             {
                                                    UpdateScanHistory(null);
                                             });
                                        }

                                        _bFirstScanAccumulate = true;
                                        _bWasInAccumulation = false;
                                    }

                                    if (!_bAccumulateSbr) //KB will be updated by the start of the next scan in accumulation or in wait tag. avoid crash on few PC
                                    {
                                       
                                        Invoke((MethodInvoker)delegate { ProcessData(null); });
                                        Invoke((MethodInvoker)delegate { new ScanFinish().Show(); });
                                       
                                    }

                                   
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
                                                _tcpNotificationThreadInfo.SetParam(TcpThreadHandle.TcpNotificationType.ScanCompleted, _localDeviceArray[nIndex].infoDev.SerialRFID, _tcpIpServer.ServerIP, _tcpIpServer.Port, idscanEvent);
                                                _tcpNotificationThreadInfo.ThreadTcpLoop();
                                             }
                                        }
                                    }
                                    
                                    #endregion
                                    #region cvs
                                    if (bScanforCSV)
                                        ReportCsvInventory(_localDeviceArray[nIndex].currentInventory);
                                    #endregion
                                    #region php
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
                        if ((_tcpIpServer != null) && (_tcpIpServer.eventScanCompleted != null))
                            _tcpIpServer.eventScanCompleted.Set();
                    
                        s1.Stop();
                        Debug.WriteLine("end completed "+ s1.Elapsed.Milliseconds + "  ms");
                                    
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
                                        if  (_bWasInAccumulation)
                                        {
                                            _localDeviceArray[nIndex].currentInventory.listTagRemoved.Clear();
                                            _localDeviceArray[nIndex].currentInventory.nbTagRemoved = 0;
                                        }

                                        if ((_bWasInAccumulation) && (_localDeviceArray[nIndex].infoDev.deviceType == DeviceType.DT_SBR))
                                        {
                                            foreach (string uidSdk in listBoxTag.Items)
                                            {

                                                if (!_localDeviceArray[nIndex].currentInventory.listTagAll.Contains(uidSdk))
                                                {
                                                    //Add all
                                                    _localDeviceArray[nIndex].currentInventory.listTagAll.Add(uidSdk);

                                                    //tag Present
                                                    if (!_localDeviceArray[nIndex].currentInventory.listTagPresent.Contains(uidSdk))
                                                    {
                                                        _localDeviceArray[nIndex].currentInventory.listTagPresent.Add(uidSdk);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            foreach (string uidSdk in _localDeviceArray[nIndex].rfidDev.get_RFID_Device.ReaderData.strListTag)
                                            {

                                                if (!_localDeviceArray[nIndex].currentInventory.listTagAll.Contains(uidSdk))
                                                {
                                                    //Add all
                                                    _localDeviceArray[nIndex].currentInventory.listTagAll.Add(uidSdk);

                                                    if (!((_bAccumulateSbr) || (_tcpIpServer.AccumulateMode)))
                                                    {
                                                        if (!_localDeviceArray[nIndex].previousInventory.listTagAll.Contains(uidSdk))
                                                        {
                                                            // Tag Added
                                                            if ((!_localDeviceArray[nIndex].currentInventory.listTagAdded.Contains(uidSdk)) &&
                                                                (!_localDeviceArray[nIndex].currentInventory.listTagPresent.Contains(uidSdk)))
                                                            {
                                                                _localDeviceArray[nIndex].currentInventory.listTagAdded.Add(uidSdk);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            //tag Present
                                                            if ((!_localDeviceArray[nIndex].currentInventory.listTagPresent.Contains(uidSdk)) &&
                                                                (!_localDeviceArray[nIndex].currentInventory.listTagAdded.Contains(uidSdk)))
                                                            {
                                                                _localDeviceArray[nIndex].currentInventory.listTagPresent.Add(uidSdk);
                                                               
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //tag Present
                                                        if (!_localDeviceArray[nIndex].currentInventory.listTagPresent.Contains(uidSdk))
                                                        {
                                                            _localDeviceArray[nIndex].currentInventory.listTagPresent.Add(uidSdk);
                                                           
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        //Check and display error if missing tag during processing
                                        // KB

                                        if (_bAccumulateSbr)
                                        {
                                            _localDeviceArray[nIndex].currentInventory.listTagRemoved.Clear();
                                            _localDeviceArray[nIndex].currentInventory.listTagAdded.Clear();
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
                                                           
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                     

                                        _localDeviceArray[nIndex].currentInventory.nbTagAll = _localDeviceArray[nIndex].currentInventory.listTagAll.Count;
                                        _localDeviceArray[nIndex].currentInventory.nbTagPresent = _localDeviceArray[nIndex].currentInventory.listTagPresent.Count;
                                        _localDeviceArray[nIndex].currentInventory.nbTagAdded = _localDeviceArray[nIndex].currentInventory.listTagAdded.Count;
                                        _localDeviceArray[nIndex].currentInventory.nbTagRemoved = _localDeviceArray[nIndex].currentInventory.listTagRemoved.Count;

                                        _localDeviceArray[nIndex].currentInventory.scanStatus = _localDeviceArray[nIndex].rfidDev.currentInventory.scanStatus;


                                        InventoryAndDbClass invCl = new InventoryAndDbClass(_localDeviceArray[nIndex].infoDev, _localDeviceArray[nIndex].currentInventory, _db, _bStoreTagEvent);
                                        int idScanEvent = -1;
                                        bool ret = StoreInventory(invCl, _bStoreTagEvent , out idScanEvent);
                                        if (ret)
                                        {
                                            _localDeviceArray[nIndex].currentInventory.IdScanEvent = idScanEvent;
                                            _localDeviceArray[nIndex].lastProcessInventoryGmtDate = truncateMs(_localDeviceArray[nIndex].currentInventory.eventDate.ToUniversalTime());
                                            Invoke((MethodInvoker) delegate
                                            {
                                               
                                                    UpdateScanHistory(null);
                                            });
                                        }
                                        _bFirstScanAccumulate = true;
                                        _bWasInAccumulation = false;
                                        if (nIndex == _selectedReader)
                                        {
                                            
                                            Invoke((MethodInvoker)delegate { ProcessData(null); });
                                            Invoke((MethodInvoker)delegate { new ScanFinish().Show(); });
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
                                            _tcpNotificationThreadInfo.SetParam(TcpThreadHandle.TcpNotificationType.ScanCancelByHost, _localDeviceArray[nIndex].infoDev.SerialRFID, _tcpIpServer.ServerIP, _tcpIpServer.Port, -1);
                                            _tcpNotificationThreadInfo.ThreadTcpLoop();

                                        }
                                    }
                                }
                                #endregion
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

                     if ((_tcpIpServer != null) && (_tcpIpServer.eventScanCompleted != null))
                            _tcpIpServer.eventScanCompleted.Set();
                     if ((_tcpIpServer != null) && (_tcpIpServer.eventScanCancelled != null))
                           _tcpIpServer.eventScanCancelled.Set();

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
                          
                            _localDeviceArray[nIndex].cptOut = 1;
                            if (nIndex == _selectedReader)
                            {                                    
                                    listBoxTag.Invoke((MethodInvoker)delegate { if (!listBoxTag.Items.Contains(args.Message)) listBoxTag.Items.Add(args.Message); });
                                    labelInventoryTagCount.Invoke((MethodInvoker)delegate { labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags, listBoxTag.Items.Count); });                                   
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
                        #region TcpNotification
                        if (_bUseTcpNotification)
                        {                           
                            if (_tcpNotificationThreadInfo != null)
                            {
                                if (tcpUtils.PingAddress(_tcpNotificationThreadInfo.HostIp, 2000))
                                {
                                    _tcpNotificationThreadInfo.SetParam(TcpThreadHandle.TcpNotificationType.DoorOpen, args.SerialNumber, _tcpIpServer.ServerIP, _tcpIpServer.Port, -1);
                                    _tcpNotificationThreadInfo.ThreadTcpLoop();
                                }
                            }
                        }
                        #endregion
                    }
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

       
        private void myMedicalCabinet_BagdeReader(object sender, string badgeRead)
        {
            if (_bClosing) return;
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
                InitializeBadgeReadingAttributes(badgeRead);

                if (_localUserArray == null) return;
                if (cabObject.DoorStatus == Door_Status.Door_Open) return;

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
                           
                            Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[5].Text = string.Format(ResStrings.strt_FP_Master_Statut_format, _localDeviceArray[index].rfidDev.FPStatusMaster); });
                            Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[6].Text = string.Format(ResStrings.strt_FP_Slave_Statut_format, _localDeviceArray[index].rfidDev.FPStatusSlave); });
                        }
                        else
                        {
                            Invoke((MethodInvoker)delegate { treeViewDevice.Nodes[index].Nodes[5].Text = string.Format(ResStrings.strt_FP_Master_Statut_format, _localDeviceArray[index].rfidDev.FPStatusMaster); });
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

                            dc.rfidDev.NotifyRFIDEvent += rfidDev_NotifyRFIDEvent;                           
                            dc.rfidDev.Create_NoFP_Device(dc.infoDev.SerialRFID, deviceToConnect.portCom);

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


            DateTime tmpLocal = DateTime.ParseExact(dateLastLocal, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            DateTime tmpUtc = tmpLocal.AddHours(-1.0).ToUniversalTime();
            // DateTime ConvertDateTime = DateTime.Parse(TmpUtc.ToString());
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
                        if (dc.myFridgeCabinet != null)
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
                        }
                        if (dc.rfidDev == null) return;
                        dc.rfidDev.ReleaseDevice();
                        break;
                }

            }
        }
        private static void ConnectAll(object obj)
        {
            if (obj == null) return;
            LiveDataForDeviceForm ldf = (LiveDataForDeviceForm)obj;
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
                ThreadPool.QueueUserWorkItem(ConnectAll, this);
                //ConnectAll(this);
            }
            

        }
       
        private void timerRestart_Tick(object sender, EventArgs e)
        {

            if (File.Exists(@"C:\devcon\renewFP.txt"))
            {
                if (_localDeviceArray != null)
                {
                    deviceClass cd = _localDeviceArray[0];
                    File.Delete(@"C:\devcon\renewFP.txt");
                    if ((cd.infoDev.deviceType == DeviceType.DT_SAS) || (cd.infoDev.deviceType == DeviceType.DT_MSR))
                        cd.rfidDev.RenewFP(false, true);
                    else
                        cd.rfidDev.RenewFP(false, true);
                }
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
        }

        private bool bwasConnected = true;
        #endregion
        #region inventory
       
       
       

        private void DeleteOldEntry()
        {
            if (_localDeviceArray != null)
            {
                foreach (deviceClass dc in _localDeviceArray)
                {
                    _db.DeleteOldInventory(dc.infoDev.SerialRFID, _nbRecordToKeep);
                }
            }
        }
        private void RefreshInventory()
        {
            Invoke((MethodInvoker)delegate
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

            if ((_localDeviceArray != null) && (!_bWasInAccumulation))
            {
                if (_selectedReader >= 0)   //reader local
                {

                    if (_localDeviceArray[_selectedReader].currentInventory != null)
                    {
                        lock (_locker)
                        {
                            labelInventoryDate.Invoke((MethodInvoker)delegate { labelInventoryDate.Text = _localDeviceArray[_selectedReader].currentInventory.eventDate.ToLocalTime().ToString("G"); });
                            labelInventoryUser.Invoke((MethodInvoker)delegate { 
                                                                                    labelInventoryUser.Text = string.Format("{0} {1}", _localDeviceArray[_selectedReader].currentInventory.userFirstName, _localDeviceArray[_selectedReader].currentInventory.userLastName);
                                                                                    if (_localDeviceArray[_selectedReader].currentInventory.bUserScan)
                                                                                    {
                                                                                        if (_localDeviceArray[_selectedReader].currentInventory.userDoor == DoorInfo.DI_SLAVE_DOOR)
                                                                                            labelInventoryUser.Text += ResStrings.str_SlaveDoor;
                                                                                        else
                                                                                            labelInventoryUser.Text += ResStrings.str_MasterDoor;
                                                                                    }
                                                                                });

                                listBoxTag.Invoke((MethodInvoker)delegate { listBoxTag.Items.Clear(); });

                                try
                                {
                                    if (_localDeviceArray[_selectedReader].rfidDev.DeviceStatus == DeviceStatus.DS_InScan)
                                    {
                                        foreach (string strTag in _localDeviceArray[_selectedReader].rfidDev.get_RFID_Device.ReaderData.strListTag)
                                        {
                                            string tag = strTag;
                                            listBoxTag.Invoke((MethodInvoker)(() => listBoxTag.Items.Add(tag)));
                                        }
                                    }
                                    else
                                    {
                                        foreach (string strTag in _localDeviceArray[_selectedReader].currentInventory.listTagAll)
                                        {
                                            string tag = strTag;
                                            listBoxTag.Invoke((MethodInvoker)(() => listBoxTag.Items.Add(tag)));
                                        }
                                    }
                                }
                                catch
                                {
                                    // do noting - the reader surely not exist
                                }
                            

                           // labelInventoryTagCount.Invoke((MethodInvoker)delegate { labelInventoryTagCount.Text = "Tag(s): " + listBoxTag.Items.Count.ToString(); });

                        }

                    }
                    else
                    {
                        labelInventoryDate.Invoke((MethodInvoker)delegate { labelInventoryDate.Text = ""; });
                        labelInventoryUser.Invoke((MethodInvoker)delegate { labelInventoryUser.Text = ""; });
                        listBoxTag.Invoke((MethodInvoker)delegate { listBoxTag.Items.Clear(); });
                        labelInventoryTagCount.Invoke((MethodInvoker)delegate { labelInventoryTagCount.Text = string.Format(ResStrings.str_Tags,0); });
                    }


                }
            }
            timerObjectList.Start();

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

                    if (!clToProcess.Db.StoreInventory(clToProcess.Data,true))
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
                else
                {
                    if (invTmp == null)
                    {
                        if (!clToProcess.Db.StoreInventory(clToProcess.Data,true))
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
            return ret;
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
            }
            string _serialRfid;           
            string _myLocalIp;
            int _myPort;
            int _idScanEvent;
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
            public void SetParam(TcpNotificationType tcpType, string serialRfid, string myLocalIp, int myPort, int idScanEvent)
            {
                _tcpType = tcpType;
                _serialRfid = serialRfid;
                _myLocalIp = myLocalIp;
                _myPort = myPort;
                _idScanEvent = idScanEvent;
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

            pathCsvReport = pathCsvFolder + @"\inventory" + cvsMachineId.PadLeft(6, '0') + ".txt";
            pathCsvInventory = pathCsvFolder + @"\start" + cvsMachineId.PadLeft(6, '0') + ".txt";
            pathCsvInScan = pathCsvFolder + @"\deviceInScan" + cvsMachineId.PadLeft(6, '0') + ".txt";
            pathCsvLed = pathCsvFolder + @"\led" + cvsMachineId.PadLeft(6, '0') + ".txt";
            pathCsvError = pathCsvFolder + @"\error" + cvsMachineId.PadLeft(6, '0') + ".txt";

            csvWatcher = new FileSystemWatcher();
            csvWatcher.Path = pathCsvFolder;
            csvWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
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

                            }
                            else
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.Append("Error device");
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

                for (int loop = 0; loop < tagsToLight.Count; loop++)
                {
                    if (string.IsNullOrEmpty(tagsToLight[loop]))
                    {
                        tagsToLight.RemoveAt(loop);
                        loop = 0;
                    }
                }


                if ((_localDeviceArray != null) && (_localDeviceArray.Length > 0))
                {
                    if ((_localDeviceArray[0].rfidDev.DeviceStatus == DeviceStatus.DS_Ready) &&
                        (_localDeviceArray[0].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected))
                    {
                        bScanforCSV = true;
                        bool ret = _localDeviceArray[0].rfidDev.TestLighting(tagsToLight);

                        if (!ret)
                        {
                            if (!bFisrtScanDone)
                                ReportCsvError("Error In Led Feature - A Inventory is mandatory before each Led Request");
                            else
                                ReportCsvError("Error In Led Feature - Not all requested Tag Found");
                        }

                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Error device");
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


        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            string fileName = e.Name;
            WatcherChangeTypes action = e.ChangeType;
            if (fileName.Equals("led" + cvsMachineId.PadLeft(6, '0') + ".txt"))
            {
                if (action == WatcherChangeTypes.Deleted)
                {
                    //Todo stop the Led
                    if ((_localDeviceArray != null) && (_localDeviceArray.Length > 0))
                    {
                        if ((_localDeviceArray[0].rfidDev.DeviceStatus == DeviceStatus.DS_LedOn) &&
                            (_localDeviceArray[0].rfidDev.ConnectionStatus == ConnectionStatus.CS_Connected))
                        {
                            bScanforCSV = true;
                            _localDeviceArray[0].rfidDev.StopLightingLeds();
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("Error device\r\n");
                            sb.Append("Connection status : " + _localDeviceArray[0].rfidDev.ConnectionStatus);
                            sb.Append("Device status : " + _localDeviceArray[0].rfidDev.DeviceStatus);
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
            sb.Append(inv.eventDate.ToLocalTime().ToString(("yyyy_MM_dd_hh_mm_ss")) + "\r\n");
            foreach (string uid in inv.listTagAll)
                sb.Append(uid + "\r\n");
            try
            {
                File.AppendAllText(pathCsvReport, sb.ToString());

                if (File.Exists(pathCsvInventory))
                    File.Delete(pathCsvInventory);
            }
            catch
            {
                ReportCsvError(pathCsvReport + " Error Write : Inventory Not reachable");
            }

            if (File.Exists(pathCsvInScan))
                File.Delete(pathCsvInScan);

            bScanforCSV = false;
        }

        private void ReportCsvError(string errMsg)
        {
            File.AppendAllText(pathCsvError, errMsg);
            if (File.Exists(pathCsvInventory))
                File.Delete(pathCsvInventory);
            bScanforCSV = false;
        }

        private void CsvPutInScan()
        {
            File.AppendAllText(pathCsvInScan, "InScan");
        }

        #endregion

            
       
       
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
    }  
}


