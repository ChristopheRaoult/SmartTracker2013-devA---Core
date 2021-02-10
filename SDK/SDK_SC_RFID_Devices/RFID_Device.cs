using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Collections;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DataClass;
using SDK_SC_RfidReader;
using SDK_SC_Fingerprint;
using SDK_SC_MedicalCabinet;
using SDK_SC_AccessControl;

namespace SDK_SC_RFID_Devices
{
    /// <summary>
    /// Delegate for Notification of the RFID
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="arg"></param>
    public delegate void NotifyHandlerRFIDDelegate(Object sender, rfidReaderArgs arg);
    /// <summary>
    /// Delegate for Notification of the FP
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="arg"></param>
    public delegate void NotifyHandlerFPDelegate(Object sender, FingerArgs arg);
    /// <summary>
    /// Delegate for Notification of badge reader(s)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="arg"></param>
    public delegate void NotifyHandlerBrDelegate(Object sender, BadgeReaderArgs arg);
    /// <summary>
    /// Class Device
    /// </summary>
    public class RFID_Device : IRFID_Device
    {
        /// <summary>
        /// Event variable for RFID events (scan, door opened, etc)
        /// </summary>
        public event NotifyHandlerRFIDDelegate NotifyRFIDEvent;
        /// <summary>
        /// Event variable for Finger print reader (fp touched, user unknown, etc)
        /// </summary>
        public event NotifyHandlerFPDelegate NotifyFPEvent;
        /// <summary>
        /// Event variable for badge reader
        /// </summary>
        public event NotifyHandlerBrDelegate NotifyBrEvent;

        private string serialNumberRFID;
        private string serialNumberFP_Master;
        private string serialNumberFP_Slave;
        /// <summary>
        /// 
        /// </summary>
        /// 
        public RfidReader get_RFID_Device { get { return myDevice; } }
        private RfidReader myDevice = null;        
        /// <summary>
        /// 
        /// </summary>
        public FingerPrintClass get_FP_Master { get { return myFPMaster; } }
        private FingerPrintClass myFPMaster = null;
        /// <summary>
        /// 
        /// </summary>
        public FingerPrintClass get_FP_Slave { get { return myFPSlave; } }
        private FingerPrintClass myFPSlave = null;

        /// <summary>
        /// Badge reader - Master
        /// </summary>
        public clBadgeReader BrMaster { get; private set; }
        /// <summary>
        /// Badge reader - Slave
        /// </summary>
        public clBadgeReader BrSlave { get; private set; }

        private FingerPrintClass myFPforEnroll = null;

        private bool useUTCdateFormat = false;
        /// <summary>
        /// 
        /// </summary>
        public bool UseUTCdateFormat { get { return useUTCdateFormat; } set { useUTCdateFormat = value; } }

        ConnectionStatus conStatus;
        DeviceStatus deviceStatus;
        DeviceType deviceType;
        FPStatus fpStatusMaster;
        FPStatus fpStatusSlave;
        private string strComPort = null;

        private ushort lightInIdle = 300;
        private ushort lightInScan = 0;
        private ushort lightDoorOpen = 300;

        private bool isInthread = false;
        private int timeToReconnect = 100;

        private string _firstName;
        private string _lastName;
        private DoorInfo _lastDoorEvent = DoorInfo.DI_NO_DOOR;
        private AccessType _lastAccessType = AccessType.AT_NONE;
        private bool _bUserScan;
        private byte _channelInScan;
        private byte _corrValueOnLastKB;

        //synchronisation
        public bool bUseSynchronisation = false;
        public EventWaitHandle CanStartScan = new AutoResetEvent(false);
        public int TimeoutInSec = 120;

        public bool DoDoorScan = true;

        private Hashtable listTagWithChannel;
        public Hashtable ListTagWithChannel { get { return listTagWithChannel; } set { listTagWithChannel = value; } }

        public class TagAxisAndCorInfo
        {
            private byte axis;
            private byte corr;

            public TagAxisAndCorInfo(byte axis , byte corr)
            {
                this.axis = axis;
                this.corr = corr;
            }

            public byte Axis
            {
                get { return axis; }
                set { axis = value; }
            }
            public byte Corr
            {
                get { return corr; }
                set { corr = value; }
            }
        }

        Dictionary<string,TagAxisAndCorInfo> betterAxisForTag  = new Dictionary<string, TagAxisAndCorInfo>();
        public Dictionary<string, TagAxisAndCorInfo> BetterAxisForTag
        {
            get { return betterAxisForTag; }
            set { betterAxisForTag = value; }
        }

        public Thread LedThread = null;         

        public bool bStopAccessDuringProcessData = false;

        /// <summary>
        /// Previoous inventory
        /// </summary>
        public InventoryData previousInventory;
        /// <summary>
        /// Current Inventory
        /// </summary>
        public InventoryData currentInventory;

        // thread used to call the method that will continuously turn on/off LED 
        private Thread _currentLightingThread;

        /// <summary>
        /// Property to set and get Light in Ready Mode
        /// </summary>
        public ushort LightInIdle { get { return lightInIdle; } set { lightInIdle = value; } }
        /// <summary>
        /// Property to set and get Light in Scan Mode
        /// </summary>
        public ushort LightInScan { get { return lightInScan; } set { lightInScan = value; } }
        /// <summary>
        /// Property to set and get Light when Door is Open
        /// </summary>
        public ushort LightDoorOpen { get { return lightDoorOpen; } set { lightDoorOpen = value; } }
        /// <summary>
        /// Property to  get the Serial number of RFID
        /// </summary>
        public string SerialNumberRFID { get { return serialNumberRFID; } }
        /// <summary>
        /// Property to  get the Device Type
        /// </summary>
       /* public string DeviceTypeReader
        {           
            get
            {
                string strType;
                switch (deviceType)
                {
                    case DeviceType.DT_DSB:
                        strType = "DSB (Diamond Smart Cabinet)";
                        break;
                    case DeviceType.DT_JSC:
                        strType = "JSC (Diamond Smart Cabinet)";
                        break;
                    case DeviceType.DT_SBR:
                        strType = "SBR (SmartBoard Reader)";
                        break;
                    default :
                        strType = "Unknown Device";
                        break;
                }
                return strType;
            }
        }*/
        /// <summary>
        /// Property to  get the Connection Status
        /// </summary>
        public ConnectionStatus ConnectionStatus { get { return conStatus; } }
        /// <summary>
        /// Property to  get the Device  Status
        /// </summary>
        public  DeviceStatus DeviceStatus { get { return deviceStatus; } set { deviceStatus = value; } }
        /// <summary>
        /// Property to  get the FingerPrint Master Status
        /// </summary>
        public FPStatus FPStatusMaster { get { return fpStatusMaster; } }
        /// <summary>
        /// Property to  get the FingerPrint Slave Status
        /// </summary>
        public FPStatus FPStatusSlave { get { return fpStatusSlave; } }

        /// <summary>
        /// Property to get the  LastScanResult
        /// </summary>
        public InventoryData LastScanResult { get { return currentInventory; } }
        /// <summary>
        /// Property to set the previou scan result
        /// </summary>
        public InventoryData setPreviousScan { set { currentInventory = value; } }

        // change default behavior
        private bool interruptScanWithFP = false;
        /// <summary>
        /// Property to setor get the fact to stop scan if a valid finger is verify during scan
        /// </summary>
        public bool InterruptScanWithFP { get { return interruptScanWithFP; } set { interruptScanWithFP = value; } }


        private int timeBeforeCloseLock  = 10;
        /// <summary>
        /// Property to set/Get value in second before close automatically the door
        /// </summary>
        public int TimeBeforeCloseLock
                                    {
                                        get { return timeBeforeCloseLock; }
                                        set {
                                            timeBeforeCloseLock = value;
                                            if (timeBeforeCloseLock != 0)
                                                closeLockTimer.Interval = timeBeforeCloseLock*1000;
                                            else
                                                closeLockTimer.Interval = 10 * 1000;
                                        }
                                     }

        private int timeDoorOpenTooLong = 30;
        /// <summary>
        /// Property to get/set the time in second before sendthe notification dooropentoolong
        /// </summary>
        public int TimeDoorOpenTooLong
        {
            get { return timeDoorOpenTooLong; }
            set
            {
                timeDoorOpenTooLong = value;
                if (timeDoorOpenTooLong != 0)
                    doorOpenTooLongTimer.Interval = timeDoorOpenTooLong * 1000; 
                else
                    doorOpenTooLongTimer.Interval = 30 * 1000; 
              
            }
        }

       System.Timers.Timer closeLockTimer;
       System.Timers.Timer doorOpenTooLongTimer;
       System.Timers.Timer MSRLockClosTimer;

       Hashtable ColumnInfo = null;

        private volatile bool bWasInLedDoorOpen = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public RFID_Device()
        {
            conStatus = ConnectionStatus.CS_Disabled;
            deviceStatus = DeviceStatus.DS_NotReady;
            deviceType = DeviceType.DT_UNKNOWN;
            fpStatusMaster = DataClass.FPStatus.FS_Disconnected;
            fpStatusSlave = DataClass.FPStatus.FS_Disconnected;
            currentInventory = new InventoryData();
            previousInventory = new InventoryData(); 

            closeLockTimer = new System.Timers.Timer();
            closeLockTimer.Elapsed += new System.Timers.ElapsedEventHandler(doorOpenTimer_Tick);
            closeLockTimer.Interval = timeBeforeCloseLock * 1000;
            closeLockTimer.Stop();

            doorOpenTooLongTimer = new System.Timers.Timer();
            doorOpenTooLongTimer.Elapsed += new System.Timers.ElapsedEventHandler(doorOpenTooLongTimer_Tick);
            doorOpenTooLongTimer.Interval = timeDoorOpenTooLong * 1000;
            doorOpenTooLongTimer.Stop();

            MSRLockClosTimer = new System.Timers.Timer();
            MSRLockClosTimer.Elapsed += new System.Timers.ElapsedEventHandler(MSRLockClosTimer_Tick);
            MSRLockClosTimer.Interval = 10000;
            MSRLockClosTimer.Stop();

            listTagWithChannel = new Hashtable();
            
           
        }
        public RFID_Device(Hashtable ColumnInfo)
        {
            this.ColumnInfo = ColumnInfo;
            conStatus = ConnectionStatus.CS_Disabled;
            deviceStatus = DeviceStatus.DS_NotReady;
            deviceType = DeviceType.DT_UNKNOWN;
            fpStatusMaster = DataClass.FPStatus.FS_Disconnected;
            fpStatusSlave = DataClass.FPStatus.FS_Disconnected;
            currentInventory = new InventoryData(ColumnInfo);
            previousInventory = new InventoryData(ColumnInfo); 

            closeLockTimer = new System.Timers.Timer();
            closeLockTimer.Elapsed += new System.Timers.ElapsedEventHandler(doorOpenTimer_Tick);
            closeLockTimer.Interval = timeBeforeCloseLock * 1000;
            closeLockTimer.Stop();

            doorOpenTooLongTimer = new System.Timers.Timer();
            doorOpenTooLongTimer.Elapsed += new System.Timers.ElapsedEventHandler(doorOpenTooLongTimer_Tick);
            doorOpenTooLongTimer.Interval = timeDoorOpenTooLong * 1000;
            doorOpenTooLongTimer.Stop();

            MSRLockClosTimer = new System.Timers.Timer();
            MSRLockClosTimer.Elapsed += new System.Timers.ElapsedEventHandler(MSRLockClosTimer_Tick);
            MSRLockClosTimer.Interval = 10000;
            MSRLockClosTimer.Stop();

            listTagWithChannel = new Hashtable();

        }

        void doorOpenTooLongTimer_Tick(object sender, EventArgs e)
        {
            if (myDevice == null) return;
            rfidReaderArgs notifyEvent = new rfidReaderArgs(myDevice.SerialNumber, rfidReaderArgs.ReaderNotify.RN_DoorOpenTooLong, "Door Open Too Long");
            if (NotifyRFIDEvent != null) NotifyRFIDEvent(this, notifyEvent);
        }

        void doorOpenTimer_Tick(object sender, EventArgs e)
        {
            if (myDevice == null) return;
            myDevice.CloseDoorMaster();
            myDevice.CloseDoorSlave();
            if (LedThread != null)
            {
                LedThread.Abort();
                LedThread.Join(1000);
                LedThread = null;
            }
            deviceStatus = DeviceStatus.DS_Ready;
            if (bWasInLedDoorOpen)
                SetLight(0);
            else
                SetLight(300);
           
        }

        public void startDoorOpenTimer()
        {
            if (closeLockTimer != null)
            {
                closeLockTimer.Interval = timeBeforeCloseLock * 1000;
                closeLockTimer.Start();
            }
           
        }

        void MSRLockClosTimer_Tick(object sender, EventArgs e)
        {
            MSRLockClosTimer.Stop();
            myDevice.CloseDoorMaster();
            myDevice.CloseDoorSlave();
            myDevice.NotifyRelock();

            if (LedThread != null)
            {
                LedThread.Abort();
                LedThread.Join(1000);
                LedThread = null;
            }
            deviceStatus = DeviceStatus.DS_Ready;
            if (bWasInLedDoorOpen)
                SetLight(0);
            else
                SetLight(300);
               
      
        }

        private EventWaitHandle eventEndDiscover = new AutoResetEvent(false);
        /// <summary>
        /// Method to retrieve plugged devices
        /// </summary>
        /// <returns></returns>
        public rfidPluggedInfo[] getRFIDpluggedDevice(bool bsearchCom2Plus)
        {
            try
            {
                ArrayList listdev = new ArrayList();
                //string[] ports = System.IO.Ports.SerialPort.GetPortNames();

                List<string> ports = GetDevicePortCom(bsearchCom2Plus);
                foreach (string s in ports)
                {

                    RfidReader deviceForDiscover = new RfidReader();
                    deviceForDiscover.NotifyEvent += new SDK_SC_RfidReader.NotifyHandlerDelegate(deviceForDiscover_NotifyEvent);
                    deviceForDiscover.ConnectReader(s);
                    eventEndDiscover.WaitOne(500, false);
                    if (deviceForDiscover.IsConnected)
                    {
                        rfidPluggedInfo tmpdev = new rfidPluggedInfo();
                        if (!deviceForDiscover.HardwareVersion.Contains(".")) break; // for modem 
                        string hw = deviceForDiscover.HardwareVersion.Substring(0, deviceForDiscover.HardwareVersion.IndexOf('.'));
                        tmpdev.deviceType = (DeviceType)int.Parse(hw);
                        tmpdev.SerialRFID = deviceForDiscover.SerialNumber;
                        tmpdev.portCom = deviceForDiscover.StrCom;
                        listdev.Add(tmpdev);
                        deviceForDiscover.Dispose();
                    }
                }
                if (listdev.Count > 0)
                {
                    int nIndex = 0;
                    rfidPluggedInfo[] arrayDev = new rfidPluggedInfo[listdev.Count];
                    foreach (rfidPluggedInfo dev in listdev)
                        arrayDev[nIndex++] = dev;
                    return arrayDev;
                }
                else
                    return null;
            }
            catch (Exception exp)
            {
                ErrorMessage.ExceptionMessageBox.Show(exp);
                return null;
            }            
        }
        private List<string> GetDevicePortCom(bool bsearchCom2Plus)
        {
            
            List<string> comPortList= new List<string>(System.IO.Ports.SerialPort.GetPortNames());
            List<string> comPortListChecked = new List<string>();
            foreach (string s in comPortList)
            {
                if (s.StartsWith("COM"))
                {
                    string strNumCom = s.Substring(3);

                    while (strNumCom.Length > 0)
                    {
                        int nbPort;
                        if (int.TryParse(strNumCom, out nbPort))
                        {
                            comPortListChecked.Add("COM" + nbPort);
                            break;
                        }
                        else
                        {
                           strNumCom=  strNumCom.Remove(strNumCom.Length - 1);
                        }
                    }

                }
            }

            return comPortListChecked;

           

           /* List<string> comPortList = new List<string>(System.IO.Ports.SerialPort.GetPortNames());
            try
            {
                const string VID = "0403";
                const string PID1 = "6001";
                const string PID2 = "6010";
                //ArrayList comPortList = new ArrayList(System.IO.Ports.SerialPort.GetPortNames());
               
                string pattern = String.Format("^VID_{0}.PID_{1}", VID, PID1);
                Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
                List<string> comports = new List<string>();
                RegistryKey rk1 = Registry.LocalMachine;
                RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
                foreach (String s3 in rk2.GetSubKeyNames())
                {
                    RegistryKey rk3 = rk2.OpenSubKey(s3);
                    //KB filter on FTDIBUS only
                    if (!rk3.Name.Equals(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Enum\FTDIBUS"))
                        continue;
                    foreach (String s in rk3.GetSubKeyNames())
                    {
                        if (_rx.Match(s).Success)
                        {
                            RegistryKey rk4 = rk3.OpenSubKey(s);
                            foreach (String s2 in rk4.GetSubKeyNames())
                            {
                                RegistryKey rk5 = rk4.OpenSubKey(s2);
                                RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");

                                string prt = (string)rk6.GetValue("PortName");
                                if (!string.IsNullOrEmpty(prt))
                                {      
                                    if (comPortList.Contains(prt))
                                        comports.Add(prt);
                                }
                            }
                        }
                    }
                }
                if (bsearchCom2Plus)
                {
                    pattern = String.Format("^VID_{0}.PID_{1}", VID, PID2);
                    _rx = new Regex(pattern, RegexOptions.IgnoreCase);
                    rk1 = Registry.LocalMachine;
                    rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
                    foreach (String s3 in rk2.GetSubKeyNames())
                    {
                        RegistryKey rk3 = rk2.OpenSubKey(s3);
                        //KB filter on FTDIBUS only
                        if (!rk3.Name.Equals(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Enum\FTDIBUS"))
                            continue;
                        foreach (String s in rk3.GetSubKeyNames())
                        {
                            if (_rx.Match(s).Success)
                            {
                                RegistryKey rk4 = rk3.OpenSubKey(s);
                                foreach (String s2 in rk4.GetSubKeyNames())
                                {
                                    RegistryKey rk5 = rk4.OpenSubKey(s2);
                                    RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");

                                    string prt = (string)rk6.GetValue("PortName");
                                    if (!string.IsNullOrEmpty(prt))
                                    {
                                        if (comPortList.Contains(prt))
                                            comports.Add(prt);
                                    }
                                }
                            }
                        }
                    }
                }

                if ((comports != null)  && (comports.Count > 0))
                    return comports;
                else
                    return comPortList;

            }
             catch
            {
                return comPortList;
            }  */
        }
                /// <summary>
        /// Method to retreive FP devices plugged
        /// </summary>
        /// <returns></returns>
        public string[] getFingerprintPluggedGUID()
        {
            FingerPrintClass tmpFP = new FingerPrintClass();

            string[] fpDevArray = tmpFP.GetPluggedFingerprint();
            tmpFP.ReleaseFingerprint();
            return fpDevArray;
        }

        private void deviceForDiscover_NotifyEvent(object sender, SDK_SC_RfidReader.rfidReaderArgs args)
        {
            switch (args.RN_Value)
            {
                case rfidReaderArgs.ReaderNotify.RN_Connected:
                case rfidReaderArgs.ReaderNotify.RN_Disconnected:
                case rfidReaderArgs.ReaderNotify.RN_FailedToConnect:  
                    if (eventEndDiscover != null)
                        eventEndDiscover.Set();
                    break;
            }
        }

        /// <summary>
        /// Method to create device with no FP (SBR , MC )
        /// </summary>
        /// <param name="serialNumberRFID">String of the serial number of the RFID </param>
        /// <returns>true if created</returns>
        public bool Create_NoFP_Device(string serialNumberRFID)
        {
            this.serialNumberRFID = serialNumberRFID;

            if (myDevice != null)
            {
                return false;
            }
            myDevice = new RfidReader(true);
            myDevice.NotifyEvent += new SDK_SC_RfidReader.NotifyHandlerDelegate(myDevice_NotifyEvent);
            conStatus = ConnectionStatus.CS_InConnection;            

            myDevice.DiscoverPluggedDevices();
           
            return true;
        }
        /// <summary>
        /// Method to create device with no Fingerprint (SBR)
        /// </summary>
        /// <param name="serialNumberRFID"></param>
        /// <param name="portCom"></param>
        /// <returns></returns>
        public bool Create_NoFP_Device(string serialNumberRFID, string portCom)
        {
            this.serialNumberRFID = serialNumberRFID;

            if (myDevice != null)
            {
                return false;
            }
            myDevice = new RfidReader(true);
            myDevice.NotifyEvent += new SDK_SC_RfidReader.NotifyHandlerDelegate(myDevice_NotifyEvent);
            conStatus = ConnectionStatus.CS_InConnection;
            myDevice.ConnectReader(portCom);

            return true;
        }
        /// <summary>
        /// Method to create device with one FP (DSB , JSC )
        /// </summary>
        /// <param name="serialNumberRFID">String of the serial number of the RFID</param>
        /// <param name="serialNumberFP_Master">String of the serial number of the FP Master</param>
        /// <param name="bLoadTemplateFromDB">true to load automatically the FP template from  the local DB access</param>
        /// <returns>true if created</returns>
        public bool Create_1FP_Device(string serialNumberRFID, string serialNumberFP_Master , bool bLoadTemplateFromDB)
        {
            this.serialNumberRFID = serialNumberRFID;
            this.serialNumberFP_Master = serialNumberFP_Master;
            if (myDevice != null)
            {
                return false;
            }

            RenewFP(false, bLoadTemplateFromDB);

            myDevice = new RfidReader(true);
            myDevice.NotifyEvent += new SDK_SC_RfidReader.NotifyHandlerDelegate(myDevice_NotifyEvent);
            conStatus = ConnectionStatus.CS_InConnection;
            myDevice.DiscoverPluggedDevices();
            return true;
        }
        /// <summary>
        /// Method to create device with 1 FP (JSC,DSB)
        /// </summary>
        /// <param name="serialNumberRFID"></param>
        /// <param name="portCom"></param>
        /// <param name="serialNumberFP_Master"></param>
        /// <param name="bLoadTemplateFromDB"></param>
        /// <returns></returns>
        public bool Create_1FP_Device(string serialNumberRFID,string portCom,string serialNumberFP_Master, bool bLoadTemplateFromDB)
        {
            this.serialNumberRFID = serialNumberRFID;
            this.serialNumberFP_Master = serialNumberFP_Master;
            if (myDevice != null)
            {
                return false;
            }

            RenewFP(false, bLoadTemplateFromDB);

            myDevice = new RfidReader(true);
            myDevice.NotifyEvent += new SDK_SC_RfidReader.NotifyHandlerDelegate(myDevice_NotifyEvent);
            conStatus = ConnectionStatus.CS_InConnection;
            myDevice.ConnectReader(portCom);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialNumberRFID"></param>
        /// <param name="serialNumberFP_Master"></param>
        /// <param name="serialNumberFP_Slave"></param>
        /// <param name="bLoadTemplateFromDB"></param>
        /// <returns></returns>
        public bool Create_2FP_Device(string serialNumberRFID, string serialNumberFP_Master, string serialNumberFP_Slave, bool bLoadTemplateFromDB)
        {
            this.serialNumberRFID = serialNumberRFID;
            this.serialNumberFP_Master = serialNumberFP_Master;
            this.serialNumberFP_Slave = serialNumberFP_Slave;
            if (myDevice != null)
            {
                return false;
            }

            RenewFP(true, bLoadTemplateFromDB);


            myDevice = new RfidReader(true);
            myDevice.NotifyEvent += new SDK_SC_RfidReader.NotifyHandlerDelegate(myDevice_NotifyEvent);
            conStatus = ConnectionStatus.CS_InConnection;
            myDevice.DiscoverPluggedDevices();
            return true;
        }
        /// <summary>
        /// Methods to create device with 2 FP (SAS)
        /// </summary>
        /// <param name="serialNumberRFID"></param>
        /// <param name="portCom"></param>
        /// <param name="serialNumberFP_Master"></param>
        /// <param name="serialNumberFP_Slave"></param>
        /// <param name="bLoadTemplateFromDB"></param>
        /// <returns></returns>
        public bool Create_2FP_Device(string serialNumberRFID,string portCom, string serialNumberFP_Master, string serialNumberFP_Slave, bool bLoadTemplateFromDB)
        {
            this.serialNumberRFID = serialNumberRFID;
            this.serialNumberFP_Master = serialNumberFP_Master;
            this.serialNumberFP_Slave = serialNumberFP_Slave;
            if (myDevice != null)
            {
                return false;
            }

            RenewFP(true, bLoadTemplateFromDB);


            myDevice = new RfidReader(true);
            myDevice.NotifyEvent += new SDK_SC_RfidReader.NotifyHandlerDelegate(myDevice_NotifyEvent);
            conStatus = ConnectionStatus.CS_InConnection;
            myDevice.ConnectReader(portCom);
            return true;
        }

        public bool Create_2FP_2BR_Device(string serialNumberRFID, string serialNumberFP_Master, string serialNumberFP_Slave, string comPortBR_Master, string comPortBR_Slave, bool bLoadTemplateFromDB)
        {
            this.serialNumberRFID = serialNumberRFID;
            this.serialNumberFP_Master = serialNumberFP_Master;
            this.serialNumberFP_Slave = serialNumberFP_Slave;
            if (myDevice != null) return false;

            BrMaster = new clBadgeReader(AccessBagerReaderType.RT_HF, comPortBR_Master,
                serialNumberRFID, true);
            BrSlave = new clBadgeReader(AccessBagerReaderType.RT_HF, comPortBR_Slave,
                serialNumberRFID, true);

            RenewFP(true, bLoadTemplateFromDB);

            myDevice = new RfidReader(true);
            myDevice.NotifyEvent += (myDevice_NotifyEvent);
            conStatus = ConnectionStatus.CS_InConnection;
            myDevice.DiscoverPluggedDevices();
            return true;
        }
        /// <summary>
        /// Methods to create device with 2 FP (SAS)
        /// </summary>
        /// <param name="serialNumberRFID"></param>
        /// <param name="portCom"></param>
        /// <param name="serialNumberFP_Master"></param>
        /// <param name="serialNumberFP_Slave"></param>
        /// <param name="bLoadTemplateFromDB"></param>
        /// <returns></returns>
        public bool Create_2FP_2BR_Device(string serialNumberRFID, string portCom, string serialNumberFP_Master, string serialNumberFP_Slave, string comPortBR_Master, string comPortBR_Slave, bool bLoadTemplateFromDB)
        {
            this.serialNumberRFID = serialNumberRFID;
            this.serialNumberFP_Master = serialNumberFP_Master;
            this.serialNumberFP_Slave = serialNumberFP_Slave;
            if (myDevice != null) return false;

            RenewFP(true, bLoadTemplateFromDB);

            BrMaster = new clBadgeReader(AccessBagerReaderType.RT_HF, comPortBR_Master,
                serialNumberRFID, true);
            BrSlave = new clBadgeReader(AccessBagerReaderType.RT_HF, comPortBR_Slave,
                serialNumberRFID, true);

            BrMaster.NotifyEvent += MasterBadgeReader_NotifyEvent;
            BrSlave.NotifyEvent += SlaveBadgeReader_NotifyEvent; 

            myDevice = new RfidReader(true);
            myDevice.NotifyEvent += (myDevice_NotifyEvent);
            conStatus = ConnectionStatus.CS_InConnection;
            myDevice.ConnectReader(portCom);
            return true;
        }

        public void RenewFP(bool renewboth, bool bLoadTemplateFromDB)
        {
            if (myFPMaster != null)
                myFPMaster.ReleaseFingerprint();

            myFPMaster = new FingerPrintClass();
            myFPMaster.NotifyEvent += myFP_NotifyMasterEvent;
            myFPMaster.DebugFP = false;
            myFPMaster.DebugFPFormVisible = false;
            myFPMaster.DebugOnFailureOnly = true;
            myFPMaster.DebugWindowTimeout = 1;
            myFPMaster.SendDebugMail = false;
            myFPMaster.InitFingerPrint(this.serialNumberRFID, this.serialNumberFP_Master);
            
            if (bLoadTemplateFromDB)
                LoadFPTemplateFromDB(myFPMaster, UserGrant.UG_MASTER);

            if (!renewboth) return;

            if (myFPSlave != null)
                myFPSlave.ReleaseFingerprint();

            myFPSlave = new FingerPrintClass();
            myFPSlave.NotifyEvent += myFP_NotifySlaveEvent;
            myFPSlave.DebugFP = false;
            myFPSlave.DebugFPFormVisible = false;
            myFPSlave.DebugOnFailureOnly = true;
            myFPSlave.DebugWindowTimeout = 1;
            myFPSlave.SendDebugMail = false;
            myFPSlave.InitFingerPrint(this.serialNumberRFID, this.serialNumberFP_Slave);
            
            if (bLoadTemplateFromDB)
                LoadFPTemplateFromDB(myFPSlave, UserGrant.UG_SLAVE);
        }
        /// <summary>
        /// Function to release the device
        /// </summary>
        
        public void ReleaseDevice()
        {
            deviceType = DeviceType.DT_UNKNOWN;
            while (isInthread) Thread.Sleep(100);
            if (myDevice != null)
                myDevice.Dispose();
            if (myFPMaster != null)
                myFPMaster.ReleaseFingerprint();
            if (myFPSlave != null)
                myFPSlave.ReleaseFingerprint();
            if(BrMaster != null)
                BrMaster.closePort();
            if(BrSlave != null)
                BrSlave.closePort();

            myFPMaster = null;
            myDevice = null;
            BrMaster = null;
            BrSlave = null;
        }

        public void ClearFPTemplate(FingerPrintClass theFP)
        {
            if (theFP != null)
                theFP.ClearFingerprintTemplates();
        }

        /// <summary>
        /// Method to load the template from access
        /// </summary>
        /// <returns></returns>
        public int LoadFPTemplateFromDB(FingerPrintClass theFP,UserGrant allowedGrant)
        {
            int ret = -1;
            string[] templateFP = null;
            int nIndex = 0;
            try
            {

                DBClass.MainDBClass db = new DBClass.MainDBClass();
               
                db.OpenDB();
                //UserClassTemplate[] Users = db.RecoverUser();
                //UserClassTemplate[] Users = db.RecoverAllowedUser(SerialNumberRFID);
                DeviceGrant[] Users = db.RecoverAllowedUser(SerialNumberRFID);
                db.CloseDB();
                if (Users == null) return 0;
                templateFP = new string[Users.Length];
               // foreach (UserClassTemplate us in Users)
                foreach (DeviceGrant us in Users)
                {
                    switch (allowedGrant)
                    {
                        case UserGrant.UG_MASTER:
                        case UserGrant.UG_MASTER_AND_SLAVE:
                            if ((us.userGrant == UserGrant.UG_MASTER_AND_SLAVE) || (us.userGrant == UserGrant.UG_MASTER))
                            {
                                if (us.user.template != null)
                                {
                                    templateFP[nIndex++] = us.user.template;
                                }
                            }
                            break;
                        case UserGrant.UG_SLAVE:
                            if ((us.userGrant == UserGrant.UG_MASTER_AND_SLAVE) || (us.userGrant == UserGrant.UG_SLAVE))
                            {
                                if (us.user.template != null)
                                {
                                    templateFP[nIndex++] = us.user.template;
                                }
                            }
                            break; 
                        default:
                            break;
                    }                    
                }
                if (templateFP != null)
                ret = theFP.LoadFingerprintTemplates(templateFP);

            }
            catch 
            {

            }

            return ret;
        }
        /// <summary>
        /// Method to load the template 
        /// </summary>
        /// <param name="templates"></param>
        /// <param name="theFP"></param>
        /// <returns></returns>
        public int LoadFPTemplate(string[] templates, FingerPrintClass theFP)
        {
             return theFP.LoadFingerprintTemplates(templates);  
        }

        public void LoadBrTemplate(UserClassTemplate[] userTemplates, clBadgeReader currentBr)
        {
            currentBr.LoaderUserTemplates(userTemplates);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FPSerialNumber"></param>
        /// <param name="FirstName"></param>
        /// <param name="LastName"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public string EnrollUser(string FPSerialNumber, string FirstName, string LastName, string template)
        {


            if ((string.IsNullOrEmpty(FirstName)) && (string.IsNullOrEmpty(LastName)))
                return null;

            if (myFPforEnroll != null)
                myFPforEnroll.ReleaseFingerprint();
            
            myFPforEnroll = new FingerPrintClass();
            string strTemplate = myFPforEnroll.EnrollUser(FPSerialNumber, FirstName, LastName, template, false);

            return strTemplate;

        }

        public void MasterBadgeReader_NotifyEvent(Object sender, BadgeReaderArgs args)
        {
            BadgeReader_NotifyEvent(sender, args);
        }


        public void SlaveBadgeReader_NotifyEvent(Object sender, BadgeReaderArgs args)
        {
            args.IsMaster = false;
            BadgeReader_NotifyEvent(sender, args);
        }


        public void BadgeReader_NotifyEvent(Object sender, BadgeReaderArgs args)
        {
            
            if (bStopAccessDuringProcessData)
                return;
            _bUserScan = true;

            _lastDoorEvent = (args.IsMaster) ? DoorInfo.DI_MASTER_DOOR : DoorInfo.DI_SLAVE_DOOR;
            _firstName = args.UserTemplate.firstName;
            _lastName = args.UserTemplate.lastName;
            _lastAccessType = AccessType.AT_BADGEREADER;

            if ((deviceType == DeviceType.DT_SAS) || (deviceType == DeviceType.DT_MSR))
            {
                if (args.IsMaster)  myDevice.OpenDoorMaster();
                else                myDevice.OpenDoorSlave();
            }

            else
                myDevice.OpenDoor();

            if ((deviceType == DeviceType.DT_SAS) || (deviceType == DeviceType.DT_DSB) || (deviceType == DeviceType.DT_JSC) || (deviceType == DeviceType.DT_CAT)) 
            if (LedThread == null)
            {
                MyThreadHandleForLed threadHandle = new MyThreadHandleForLed(myDevice, 500);
                Thread t = new Thread(new ThreadStart(threadHandle.ThreadLoop));
                t.Start();
            }

            closeLockTimer.Interval = timeBeforeCloseLock * 1000;
            closeLockTimer.Start();

            if (NotifyBrEvent != null) NotifyBrEvent(this, args);
        }


        private volatile bool _fpMasterTouch = false;
        private volatile bool _fpSlaveTouch = false;

        private void myFP_NotifyMasterEvent(object sender, SDK_SC_Fingerprint.FingerArgs args)
        {
            switch (args.RN_Value)
            {
                case FingerArgs.FingerNotify.RN_FingerprintConnect: 
                    fpStatusMaster = DataClass.FPStatus.FS_Ready; 
                    break;
                case FingerArgs.FingerNotify.RN_FingerprintDisconnect: fpStatusMaster = DataClass.FPStatus.FS_Disconnected; break;
                case FingerArgs.FingerNotify.RN_FingerTouch: 
                     fpStatusMaster = DataClass.FPStatus.FS_FingerTouch;
                    _fpMasterTouch = true;
                    break;
                case FingerArgs.FingerNotify.RN_FingerGone: 
                     fpStatusMaster = DataClass.FPStatus.FS_Ready;
                     _fpMasterTouch = false;
                     break;
                case FingerArgs.FingerNotify.RN_FingerUserUnknown: fpStatusMaster = DataClass.FPStatus.FS_UnknownUser; break;
                case FingerArgs.FingerNotify.RN_AuthentificationCompleted:

                    if (_fpSlaveTouch)
                        return;
                     if (myDevice.Door_Status == Door_Status.Door_Open)
                        return;
                    if (myDevice.Lock_Status == Lock_Status.Lock_Open)
                        return;
                     if (bStopAccessDuringProcessData) 
                         return;

                     if (myDevice.IsInScan)
                     {
                         if (interruptScanWithFP)
                         {
                             myDevice.RequestEndScan();
                             Thread.Sleep(100);
                             myDevice.OpenDoor();

                               if ((deviceType == DeviceType.DT_SAS) || (deviceType == DeviceType.DT_DSB) || (deviceType == DeviceType.DT_JSC) || (deviceType == DeviceType.DT_CAT)) 
                             if (LedThread == null)
                             {
                                 MyThreadHandleForLed threadHandle = new MyThreadHandleForLed(myDevice, 500);
                                 Thread t = new Thread(new ThreadStart(threadHandle.ThreadLoop));
                                 t.Start();
                             }
                         }
                         else
                         {
                             rfidReaderArgs notifyEvent = new rfidReaderArgs(serialNumberRFID, rfidReaderArgs.ReaderNotify.RN_ReaderNotReady, "Reader already in scan");
                             if (NotifyRFIDEvent != null) NotifyRFIDEvent(this, notifyEvent);
                             return;
                         }
                     }
                     if ((deviceStatus == DataClass.DeviceStatus.DS_Ready) || (DeviceStatus == DataClass.DeviceStatus.DS_LedOn))
                     {

                         _bUserScan = true;
                         string[] strUser = args.Message.Split(';');
                         _firstName = strUser[0];
                         _lastName = strUser[1];
                         _lastDoorEvent = DoorInfo.DI_MASTER_DOOR;
                         _lastAccessType = AccessType.AT_FINGERPRINT;

                         fpStatusMaster = DataClass.FPStatus.FS_CaptureComplete;
                         if ((deviceType == DeviceType.DT_SAS) || (deviceType == DeviceType.DT_MSR))
                             myDevice.OpenDoorMaster();

                         else
                             myDevice.OpenDoor();
                          
                         if ((deviceType == DeviceType.DT_SAS) || (deviceType == DeviceType.DT_DSB) || (deviceType == DeviceType.DT_JSC) || (deviceType == DeviceType.DT_CAT)) 
                         if (LedThread == null)
                         {
                             MyThreadHandleForLed threadHandle = new MyThreadHandleForLed(myDevice, 500);
                             LedThread = new Thread(new ThreadStart(threadHandle.ThreadLoop));
                             LedThread.Start();
                         }

                         closeLockTimer.Interval = timeBeforeCloseLock * 1000;
                         closeLockTimer.Start();
                     }
                    break;
            }

            string newMessage = args.Message + ";" + _lastDoorEvent.ToString();
            FingerArgs newArgsWithDoor = new FingerArgs(args.SerialNumber, args.RN_Value, newMessage);
            // if (NotifyFPEvent != null) NotifyFPEvent(this, args);
            if (NotifyFPEvent != null) NotifyFPEvent(this, newArgsWithDoor);
        }


        private void myFP_NotifySlaveEvent(object sender, SDK_SC_Fingerprint.FingerArgs args)
        {
            switch (args.RN_Value)
            {
                case FingerArgs.FingerNotify.RN_FingerprintConnect: fpStatusSlave = DataClass.FPStatus.FS_Ready; break;
                case FingerArgs.FingerNotify.RN_FingerprintDisconnect: fpStatusSlave = DataClass.FPStatus.FS_Disconnected; break;
                case FingerArgs.FingerNotify.RN_FingerTouch: 
                     fpStatusSlave = DataClass.FPStatus.FS_FingerTouch;
                    _fpSlaveTouch = true;
                    break;
                case FingerArgs.FingerNotify.RN_FingerGone: 
                    fpStatusSlave = DataClass.FPStatus.FS_Ready;
                    _fpSlaveTouch = false;
                    break;
                case FingerArgs.FingerNotify.RN_FingerUserUnknown: fpStatusSlave = DataClass.FPStatus.FS_UnknownUser; break;
                case FingerArgs.FingerNotify.RN_AuthentificationCompleted:

                    if (_fpMasterTouch == true) return;
                    if (myDevice.Door_Status == Door_Status.Door_Open)
                        return;
                    if (myDevice.Lock_Status == Lock_Status.Lock_Open)
                        return;
                    if (bStopAccessDuringProcessData)
                        return;
                    if ((!interruptScanWithFP) && (myDevice.IsInScan))
                        return;

                    if (myDevice.IsInScan) 
                    {

                        if (interruptScanWithFP)
                        {
                            myDevice.RequestEndScan();
                            Thread.Sleep(1000);
                            myDevice.OpenDoor();

                            if ((deviceType == DeviceType.DT_SAS) || (deviceType == DeviceType.DT_DSB) || (deviceType == DeviceType.DT_JSC) || (deviceType == DeviceType.DT_CAT)) 
                            if (LedThread == null)
                            {
                                MyThreadHandleForLed threadHandle = new MyThreadHandleForLed(myDevice, 500);
                                LedThread = new Thread(new ThreadStart(threadHandle.ThreadLoop));
                                LedThread.Start();
                            }
                        }
                        else
                        {
                            rfidReaderArgs notifyEvent = new rfidReaderArgs(serialNumberRFID, rfidReaderArgs.ReaderNotify.RN_ReaderNotReady, "Reader already in scan");
                            if (NotifyRFIDEvent != null) NotifyRFIDEvent(this, notifyEvent);
                            return;
                        }
                    }
                    if ((deviceStatus == DataClass.DeviceStatus.DS_Ready) || (DeviceStatus == DataClass.DeviceStatus.DS_LedOn))
                    {
                        _bUserScan = true;
                        string[] strUser = args.Message.Split(';');
                        _firstName = strUser[0];
                        _lastName = strUser[1];
                        _lastDoorEvent = DoorInfo.DI_SLAVE_DOOR;
                        _lastAccessType = AccessType.AT_FINGERPRINT;
                        fpStatusSlave = DataClass.FPStatus.FS_CaptureComplete;

                        if ((deviceType == DeviceType.DT_SAS) || (deviceType == DeviceType.DT_MSR))
                            myDevice.OpenDoorSlave();

                        else
                            myDevice.OpenDoor();

                        if ((deviceType == DeviceType.DT_SAS) || (deviceType == DeviceType.DT_DSB) || (deviceType == DeviceType.DT_JSC) || (deviceType == DeviceType.DT_CAT)) 
                        if (LedThread == null)
                        {
                            MyThreadHandleForLed threadHandle = new MyThreadHandleForLed(myDevice, 500);
                            LedThread = new Thread(new ThreadStart(threadHandle.ThreadLoop));
                            LedThread.Start();
                        }
                        closeLockTimer.Interval = timeBeforeCloseLock * 1000;
                        closeLockTimer.Start();
                    }

                    break;

            }

            string newMessage = args.Message + ";" + _lastDoorEvent.ToString();
            FingerArgs newArgsWithDoor = new FingerArgs(args.SerialNumber, args.RN_Value, newMessage, false);
          // if (NotifyFPEvent != null) NotifyFPEvent(this, args);
            if (NotifyFPEvent != null) NotifyFPEvent(this, newArgsWithDoor);
        }

        //Led Flash
        public class MyThreadHandleForLed
        {
            RfidReader device;          
            int timeout;
            public MyThreadHandleForLed(RfidReader device, int timeout)
            {
                this.device = device;             
                this.timeout = timeout;
            }
            public void ThreadLoop()
            {
                while (Thread.CurrentThread.IsAlive)
                {
                    if (device.IsConnected)
                    {
                        device.DeviceBoard.SetLightDuty(0);
                        Thread.Sleep(timeout);
                        device.DeviceBoard.SetLightDuty(300);
                        Thread.Sleep(200);
                    }
                }
            }
        }


        private void myDevice_NotifyEvent(object sender, SDK_SC_RfidReader.rfidReaderArgs args)
        {
            if (myDevice == null) return;
            switch (args.RN_Value)
            {
                    case rfidReaderArgs.ReaderNotify.RN_CorrValueOnKB:
                    byte.TryParse(args.Message, out _corrValueOnLastKB);
                    break;
                case rfidReaderArgs.ReaderNotify.RN_ActiveChannnelChange:
                    byte.TryParse(args.Message, out _channelInScan);
                    break;
                case  rfidReaderArgs.ReaderNotify.RN_DiscoverPluggedDevicesCompleted:

                        isInthread = false;
                        strComPort = null;

                        foreach (string str in myDevice.ListOfSerialPluggedDevices)
                        {
                            string[] strTmp = str.Split(';');
                            if (serialNumberRFID.Equals(strTmp[0]))
                            {
                                strComPort = strTmp[1];
                            }
                            if (!string.IsNullOrEmpty(strComPort))
                                break;
                        }
                        if ((myDevice != null) && (!string.IsNullOrEmpty(strComPort)))
                            myDevice.ConnectReader(strComPort);
                        else
                        {
                            rfidReaderArgs notifyEvent = new rfidReaderArgs(serialNumberRFID, rfidReaderArgs.ReaderNotify.RN_FailedToConnect, "Reader Connection failed on  " + strComPort);
                            if (NotifyRFIDEvent != null) NotifyRFIDEvent(this, notifyEvent);
                        }
                        timeToReconnect = 10000;
                    
                    break;

                case rfidReaderArgs.ReaderNotify.RN_Connected:

                    SetDeviceType();
                    conStatus = ConnectionStatus.CS_Connected;
                    deviceStatus = DeviceStatus.DS_Ready;
                    if ((deviceType == DeviceType.DT_SAS) || (deviceType == DeviceType.DT_MSR))
                    {
                        myDevice.CloseDoorMaster();
                        myDevice.CloseDoorSlave();
                    }
                    setLightvsState();
                    if (NotifyRFIDEvent != null)  NotifyRFIDEvent(this, args);
                    break;

                case rfidReaderArgs.ReaderNotify.RN_Disconnected:
                case rfidReaderArgs.ReaderNotify.RN_FailedToConnect:

                    deviceType = DeviceType.DT_UNKNOWN;
                    conStatus = ConnectionStatus.CS_Disconnected;
                    deviceStatus = DeviceStatus.DS_NotReady;
                    if (NotifyRFIDEvent != null)  NotifyRFIDEvent(this, args);

                    /*if (myDevice != null)
                        myDevice.Dispose();
                     myDevice = new rfidReader(true);
                     myDevice.NotifyEvent += new SDK_SC_RfidReader.NotifyHandlerDelegate(myDevice_NotifyEvent);
                     conStatus = ConnectionStatus.CS_InConnection;                         
                     ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectDevice), (object)this);
                    */

                    break;
                case rfidReaderArgs.ReaderNotify.RN_ScanStarted:
                        deviceStatus = DeviceStatus.DS_InScan;

                        listTagWithChannel = new Hashtable();
                        betterAxisForTag = new Dictionary<string, TagAxisAndCorInfo>();
                        _channelInScan = 1;
                        _corrValueOnLastKB = 0;
                        previousInventory = currentInventory;
                        if ( ColumnInfo != null)
                            currentInventory = new InventoryData(ColumnInfo);
                        else
                            currentInventory = new InventoryData();
                        currentInventory.bUserScan = _bUserScan;
                        if (_bUserScan)
                        {
                            currentInventory.userFirstName = _firstName;
                            currentInventory.userLastName = _lastName;
                            currentInventory.userDoor = _lastDoorEvent;
                            currentInventory.accessType = _lastAccessType;
                        }

                        else
                        {
                            currentInventory.userFirstName = "Manual";
                            currentInventory.userLastName =  "Scan";
                            currentInventory.userDoor = DoorInfo.DI_NO_DOOR;
                            currentInventory.accessType = AccessType.AT_NONE;
                        }
                    
                        currentInventory.serialNumberDevice = myDevice.SerialNumber;
                        if (useUTCdateFormat)
                             currentInventory.eventDate = DateTime.UtcNow;
                        else
                             currentInventory.eventDate = DateTime.Now;

                        if (NotifyRFIDEvent != null) NotifyRFIDEvent(this, args);                  
                        
                    

                    break;
                case rfidReaderArgs.ReaderNotify.RN_ScanCompleted:
                case rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost:



                    if (args.RN_Value == rfidReaderArgs.ReaderNotify.RN_ScanCompleted)
                    {
                        currentInventory.scanStatus = "OK";
                        foreach (string uid in previousInventory.listTagAll)
                        {
                            if (!currentInventory.listTagAll.Contains(uid))
                            {
                                currentInventory.listTagRemoved.Add(uid);

                            }
                        }
                        currentInventory.nbTagAll = currentInventory.listTagAll.Count;
                        currentInventory.nbTagPresent = currentInventory.listTagPresent.Count;
                        currentInventory.nbTagAdded = currentInventory.listTagAdded.Count;
                        currentInventory.nbTagRemoved = currentInventory.listTagRemoved.Count;
                       
                    }
                    else
                        currentInventory.scanStatus = "Cancel_By_Host";

                        
                        _bUserScan = false;
                        deviceStatus = DataClass.DeviceStatus.DS_Ready;
                        //setLightvsState();
                        if (NotifyRFIDEvent != null) NotifyRFIDEvent(this, args);  
                    break;

                case rfidReaderArgs.ReaderNotify.RN_TagAdded:
                    
                    // new featue if multiple axis to store wich one has better corr
                    TagAxisAndCorInfo taaci;
                    if (betterAxisForTag.TryGetValue(args.Message, out taaci))
                    {
                        if (taaci.Corr < _corrValueOnLastKB)
                        {
                            betterAxisForTag[args.Message] = new TagAxisAndCorInfo(_channelInScan, _corrValueOnLastKB);
                        }
                    }
                    else
                    {
                        betterAxisForTag.Add(args.Message, new TagAxisAndCorInfo(_channelInScan,_corrValueOnLastKB));
                    }

                    // New tag tag axis in scan , known  tag tag axis of better corr collection
                    if (!listTagWithChannel.ContainsKey(args.Message))
                            listTagWithChannel.Add(args.Message, _channelInScan);
                    else
                    {
                        listTagWithChannel[args.Message] = betterAxisForTag[args.Message].Axis;
                    }



                    // Tag All
                    if (!currentInventory.listTagAll.Contains(args.Message))
                        currentInventory.listTagAll.Add(args.Message); 
                   
                    if (!previousInventory.listTagAll.Contains(args.Message))
                    {
                        // Tag Added
                        if (!currentInventory.listTagAdded.Contains(args.Message))
                            currentInventory.listTagAdded.Add(args.Message);                       

                    }
                    else
                    {
                        //tag Present
                        if (!currentInventory.listTagPresent.Contains(args.Message))
                            currentInventory.listTagPresent.Add(args.Message);                       

                    }
              

                    if (NotifyRFIDEvent != null) NotifyRFIDEvent(this, args);
                    
                    break;

                case rfidReaderArgs.ReaderNotify.RN_ReaderFailToStartScan:
                    deviceStatus = DataClass.DeviceStatus.DS_Ready;
                    setLightvsState();
                    currentInventory.scanStatus = "Error_Start_Scan";
                    _bUserScan = false;
                    if (NotifyRFIDEvent != null)  NotifyRFIDEvent(this, args);
                    break;
                   
                case rfidReaderArgs.ReaderNotify.RN_ReaderScanTimeout:
                    deviceStatus = DataClass.DeviceStatus.DS_Ready;
                    setLightvsState();
                    currentInventory.scanStatus = "Scan_Timeout";
                    _bUserScan = false;
                    if (NotifyRFIDEvent != null)  NotifyRFIDEvent(this, args);
                    break;

                case rfidReaderArgs.ReaderNotify.RN_ErrorDuringScan:
                    currentInventory.scanStatus = "Error_During_Scan";
                    deviceStatus = DataClass.DeviceStatus.DS_Ready;
                    setLightvsState();
                    _bUserScan = false;
                    if (NotifyRFIDEvent != null)  NotifyRFIDEvent(this, args);
                    break;
                    

                case rfidReaderArgs.ReaderNotify.RN_Door_Opened:

                    deviceStatus = DataClass.DeviceStatus.DS_DoorOpen;
                    setLightvsState();
                    Thread.Sleep(500);

                    if (LedThread != null)
                    {
                        LedThread.Abort();
                        LedThread.Join(1000);
                        LedThread = null;
                    }
                   
                    if (DeviceStatus == DeviceStatus.DS_InScan)
                    {
                        myDevice.RequestEndScan();
                        deviceStatus = DataClass.DeviceStatus.DS_DoorOpen;
                    }                

                   

                    /*if ((deviceType == DeviceType.DT_SAS) || (deviceType == DeviceType.DT_MSR))
                    {
                        myDevice.CloseDoorMaster();
                        myDevice.CloseDoorSlave();
                    }                        
                    else
                    {
                        myDevice.CloseDoor(); 
                    }*/
                    // traitemment particulier du SAS avec pb de detection du sensor
                    if (deviceType == DeviceType.DT_MSR)
                    {
                        if (serialNumberRFID.Equals("21031401"))
                        {
                            MSRLockClosTimer.Interval = 10000;
                            MSRLockClosTimer.Start();
                        }
                        else
                        {
                            myDevice.CloseDoorMaster();
                            myDevice.CloseDoorSlave();
                        }                       
                    }
                    else
                    {
                        if (deviceType == DeviceType.DT_SAS)
                        {
                            myDevice.CloseDoorMaster();
                            myDevice.CloseDoorSlave();
                        }
                        else
                        {
                            myDevice.CloseDoor();
                        }
                       
                    }
                     closeLockTimer.Stop();
                     doorOpenTooLongTimer.Interval = timeDoorOpenTooLong * 1000;
                     doorOpenTooLongTimer.Start();
                     if (NotifyRFIDEvent != null) NotifyRFIDEvent(this, args);
                   
                    break;

                case rfidReaderArgs.ReaderNotify.RN_Door_Closed:
                    
                    StopLightingLeds();
                    deviceStatus = DeviceStatus.DS_DoorClose;
                   
                    setLightvsState();
                    doorOpenTooLongTimer.Stop();
                    Thread.Sleep(500);
                    // Semaphore ????
                    CanStartScan.Reset();
                    if (NotifyRFIDEvent != null) NotifyRFIDEvent(this, args);                
                    
                   
                    if (DoDoorScan)
                    {                       

                        if (bUseSynchronisation)
                        {
                            CanStartScan.WaitOne(TimeoutInSec * 1000, false);
                        }
                        ScanDevice();
                    }
                    else
                    {
                        Thread.Sleep(500);
                        deviceStatus = DeviceStatus.DS_Ready;
                    }
                    break;
                case rfidReaderArgs.ReaderNotify.RN_UsbCableUnplug:
                case rfidReaderArgs.ReaderNotify.RN_Power_OFF:
                    if (NotifyRFIDEvent != null) NotifyRFIDEvent(this, args);
                    break;
                case rfidReaderArgs.ReaderNotify.RN_TagPresenceDetected:

                    if (enableWaitTagNotification)
                    {
                        enableWaitTagNotification = false;
                        if (NotifyRFIDEvent != null) NotifyRFIDEvent(this, args);
                    }
                    break;
                case rfidReaderArgs.ReaderNotify.RN_FirmwareStarted:
                    deviceStatus = DataClass.DeviceStatus.DS_FlashFirmware;
                    if (NotifyRFIDEvent != null) NotifyRFIDEvent(this, args);
                    break;
                case rfidReaderArgs.ReaderNotify.RN_FirmwareSuccedToFinish:
                    deviceStatus = DataClass.DeviceStatus.DS_Ready;
                    if (NotifyRFIDEvent != null) NotifyRFIDEvent(this, args);
                    break;
                case rfidReaderArgs.ReaderNotify.RN_FirmwareFailedToFinish:
                case rfidReaderArgs.ReaderNotify.RN_FirmwareCorruptedHexFile:
               
                    deviceStatus = DataClass.DeviceStatus.DS_InError;
                    if (NotifyRFIDEvent != null) NotifyRFIDEvent(this, args);
                    break;
              
                default: 
                    if (NotifyRFIDEvent != null) NotifyRFIDEvent(this, args);
                    break;
            }           
      
        }
        /// <summary>
        /// Method to scan a device
        /// </summary>
        /// <returns></returns>
        public bool ScanDevice(bool bUseKR = true , bool bUnlockAllTag = true)
        {
            bWasInLedDoorOpen = false;
            if ((conStatus == DataClass.ConnectionStatus.CS_Connected) &&
                ((deviceStatus == DataClass.DeviceStatus.DS_Ready) || (deviceStatus == DataClass.DeviceStatus.DS_DoorClose) || (deviceStatus == DataClass.DeviceStatus.DS_WaitForScan)))
            {

                myDevice.RequestScan3D(bUseKR, bUnlockAllTag);
                return true;
            }
            else
                return false;

        }

        public bool ScanDevice(uint Dcu, byte axis)
        {
            if ((axis < 0) && (axis > 16)) return false;
            if ((Dcu < 0) && (Dcu > 168)) return false;
            bWasInLedDoorOpen = false;
            if ((conStatus == DataClass.ConnectionStatus.CS_Connected) &&
                ((deviceStatus == DataClass.DeviceStatus.DS_Ready) || (deviceStatus == DataClass.DeviceStatus.DS_DoorClose)))
            {
                myDevice.DeviceBoard.sendSwitchCommand(1,axis);
                Thread.Sleep(10);  
                myDevice.DeviceBoard.setBridgeState(false,Dcu,167);                    
                myDevice.RequestScan3D(true, true);
                return true;                
            }
            else
                return false;
        }

        public bool ScanDeviceMono()
        {
            bWasInLedDoorOpen = false; 
            if ((conStatus == DataClass.ConnectionStatus.CS_Connected) &&
                ((deviceStatus == DataClass.DeviceStatus.DS_Ready) || (deviceStatus == DataClass.DeviceStatus.DS_DoorClose)))
            {

                myDevice.RequestScan(true, false, true, true);
                return true;
            }
            else          
            return false;
        }

        /// <summary>
        /// Method to scan device
        /// </summary>
        /// <param name="UseMutex">Control parallele scan or not</param>
        /// <returns></returns>
        public bool ScanDevice(bool UseMutex)
        {
            bWasInLedDoorOpen = false;
            if ((conStatus == DataClass.ConnectionStatus.CS_Connected) &&
                ((deviceStatus == DataClass.DeviceStatus.DS_Ready)) || (deviceStatus == DataClass.DeviceStatus.DS_WaitForScan))
            {
                myDevice.RequestScan3D(true, true,UseMutex);
                return true;
            }
            else
                return false;
        }
         /// <summary>
        /// Method to stop a scan
        /// </summary>
        /// <returns></returns>
        public bool StopScan()
        {
            StopLightingLeds();

            if ((conStatus == DataClass.ConnectionStatus.CS_Connected) &&
              (deviceStatus == DataClass.DeviceStatus.DS_InScan))
            {

                myDevice.RequestEndScan();
                return true;
            }
            else
                return false;
        }

        public void setLightvsState()
        {
            if (myDevice == null) return;
            if ((deviceType == DeviceType.DT_SBR) || (deviceType == DeviceType.DT_STR) || (deviceType == DeviceType.DT_SFR) || (deviceType == DeviceType.DT_SBF))
                return;
            if (bWasInLedDoorOpen)
            {
                if (myDevice.IsConnected)
                    myDevice.SetLightPower(0);
                return;
            }
            

            switch (deviceStatus)
            {
                case DataClass.DeviceStatus.DS_Ready:
                    if (myDevice.IsConnected)
                        myDevice.SetLightPower(lightInIdle);
                    break;

                case DataClass.DeviceStatus.DS_InScan:
                case DataClass.DeviceStatus.DS_DoorClose:
                    if (myDevice.IsConnected)
                         myDevice.SetLightPower(lightInScan);
                    break;

                case DataClass.DeviceStatus.DS_DoorOpen:
                        if (myDevice.IsConnected)
                            myDevice.SetLightPower(lightDoorOpen);

                    break;
                    case DeviceStatus.DS_LedOn:
                     if (myDevice.IsConnected)
                      myDevice.SetLightPower(0);
                    break;
            }
        }
        /// <summary>
        /// method to unlock the door
        /// </summary>
        /// <returns></returns>
        public bool UnLock()
        {
            if (myDevice == null) return false;
            if (myDevice.IsConnected)
            {
                myDevice.OpenDoor();
                closeLockTimer.Interval = timeBeforeCloseLock * 1000;
                closeLockTimer.Start();
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// method to lock the door
        /// </summary>
        /// <returns></returns>
        public bool Lock()
        {
            if (myDevice == null) return false;
            if (myDevice.IsConnected)
            {
                myDevice.CloseDoor();
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// methos to set the light (0 switch off - 300 max light)
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        public bool SetLight(ushort power)
        {
            if (myDevice == null) return false;
            if ((myDevice.IsConnected) && (!myDevice.IsInScan))
            {
                myDevice.SetLightPower(power);
                return true;
            }
            else
                return false;
        }

        private object locker = new object();
        private  void ConnectDevice(object obj)
        {
            lock (locker)
            {

                RFID_Device rd = (RFID_Device)obj;
                Thread.Sleep(rd.timeToReconnect);
                rd.isInthread = true;
                if (rd.myDevice != null)
                    rd.myDevice.DiscoverPluggedDevices();
                else
                    rd.isInthread = false;
            }           
        }
        private void SetDeviceType()
        {
            string devtype = myDevice.HardwareVersion.Substring(0,myDevice.HardwareVersion.IndexOf('.'));
            switch (devtype)
            {
                case "1": deviceType = DeviceType.DT_DSB; break;
                case "2": deviceType = DeviceType.DT_JSC; break;
                case "3": deviceType = DeviceType.DT_SMC; break;
                case "4": deviceType = DeviceType.DT_DSB; break;
                case "6": deviceType = DeviceType.DT_SBR; break;
                case "7": deviceType = DeviceType.DT_SAS; break;
                case "8": deviceType = DeviceType.DT_SFR; break;
                case "9": deviceType = DeviceType.DT_MSR; break;
                case "10": deviceType = DeviceType.DT_SBF; break;
                default :
                    deviceType = DeviceType.DT_UNKNOWN; break;

            }
        }

        bool enableWaitTagNotification = false;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool EnableWaitMode()
        {
            if (myDevice == null) return false;
            if (myDevice.IsConnected)
            {
               
                enableWaitTagNotification = true;
                bool ret = myDevice.SetWaitForTag(true);
                if (ret)
                {
                    deviceStatus = DataClass.DeviceStatus.DS_WaitTag;
                    return true;
                }
                else
                    return false;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool DisableWaitMode()
        {
            if (myDevice == null) return false;
            if (myDevice.IsConnected)
            {
               
                enableWaitTagNotification = false;
                bool ret = myDevice.SetWaitForTag(false);
                if (ret) 
                {                   
                    deviceStatus = DataClass.DeviceStatus.DS_Ready;
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        public bool setAlarm(bool bOnOff)
        {
            if (myDevice == null) return false;
          
            if (myDevice.IsConnected)
            {
                myDevice.SetIRSensorON(bOnOff);
                return true;
            }
            return false;
        }


        private int _currentLightingAxis = 0;

        /// <summary>
        /// Start "step by step" lighting. Will try to turn on led of all tags in tagList
        /// </summary>
        /// <param name="tagList">List of tags to light on</param>
        /// <param name="tagLedStateTable">hashtable string/bool : tagId/state. State : true = lighted</param>
        /// <param name="currentChannel">channel (axis number + 1) currently in use</param>
        /// <param name="nbLighted">number of tags successfully lighted</param>
        /// <returns>true if lastStep (last axis has been browsed). False otherwise.</returns>
        public bool StartLightingLeds(List<string> tagList, Hashtable tagLedStateTable, out int currentChannel, out int nbLighted)
        {
            if (_currentLightingThread != null && _currentLightingThread.IsAlive)
            {
                _currentLightingThread.Abort();
                _currentLightingThread.Join(1000);
                myDevice.StopField();
            }
            SetLight(0);  // Led OFF

            bool tagsOnThixAxis = false;
            nbLighted = 0;

            int nbAxisOnDevice = GetCurrentNumberOfAxis();

            currentChannel = ++_currentLightingAxis;

            if (_currentLightingAxis > nbAxisOnDevice) return true;

            foreach (string currentTag in tagList)
            {
                if (!listTagWithChannel.Contains(currentTag)) continue;

                int axis = (byte)listTagWithChannel[currentTag];
                if (axis != _currentLightingAxis) continue;

                tagsOnThixAxis = true;
                break;
            }

            if (!tagsOnThixAxis) return false;

            nbLighted = myDevice.ConfirmAndLed(_currentLightingAxis, tagLedStateTable);
            _currentLightingThread = new Thread(() => ContinousLightingThreaded(_currentLightingAxis, 1));
            _currentLightingThread.Start();
            bWasInLedDoorOpen = true;
            deviceStatus = DeviceStatus.DS_LedOn;
            return false;
        }
       


        public void StopLightingLeds()
        {

            if ((deviceStatus == DeviceStatus.DS_LedOn) || (deviceStatus == DeviceStatus.DS_DoorOpen))// KB if led on on device with door and door is open
            {
                myDevice.StopField();
                _currentLightingAxis = 0;
            }
            if (_currentLightingThread != null)
            {
                _currentLightingThread.Abort();
                _currentLightingThread.Join(1000);
            }
            bWasInLedDoorOpen = false;
            deviceStatus = DeviceStatus.DS_Ready;
        }


        public volatile bool isInSearchTag = false;
        /// <summary>
        /// Turn on the LED of each tag present in tagList, and launches a threaded anonym function that will 
        /// light leds periodically (endless, until StopLightingLeds() is called).
        /// </summary>
        /// <param name="tagList">List of tags to light (tags are removed once they're lighted)</param>
        /// <returns>True if all tags in the list has been turned On. False otherwise.</returns>
        public bool TestLighting(List<string> tagList)
        {
            deviceStatus = DeviceStatus.DS_LedOn;
            int tagsLeft = 0;
            if (listTagWithChannel == null)
            {
                isInSearchTag = false;
                return false;
            }

            isInSearchTag = true;

            bWasInLedDoorOpen = true;
            

            int nbAxisOnDevice = GetCurrentNumberOfAxis();
            List<int> axisNotEmpty = new List<int>();
            List<string> tagRmoved = new List<string>();

            foreach (var uid in tagList)
            {
                if (!listTagWithChannel.ContainsKey(uid))
                    tagRmoved.Add(uid);
            }
        

            if (tagRmoved.Count > 0)
            {
                 foreach (var uid in tagRmoved)
                {
                    if (tagList.Contains(uid))
                        tagList.Remove(uid);
                }    
            }

            if (tagList.Count > 0)
            {
                deviceStatus = DeviceStatus.DS_LedOn;
                // Fill the list of used axis (= axis upon which scan has detected tags)
                foreach (DictionaryEntry entry in listTagWithChannel)
                    if (tagList.Contains((string)entry.Key)) // only if the tag is concerned by our tagList
                    {
                        if (!axisNotEmpty.Contains((byte)entry.Value)) // only if we didn't already add the axis to our axisNotEmpty list
                            axisNotEmpty.Add((byte)entry.Value); // axis has to be used : add it to our list               
                    }
                    else  //remove tag not there taht will be for sure not lighted.
                    {
                        if (tagList.Contains((string)entry.Key))
                        {
                            tagList.Remove((string)entry.Key);
                            tagRmoved.Add((string)entry.Key);
                        }
                    }

                int nbTagAtStartToLight = tagList.Count;

                SetLight(0);  // Led OFF
                              // unlock all axis
                if (nbAxisOnDevice == 1)
                {
                    myDevice.ConfirmAndLightWithKD(axisNotEmpty[0], tagList);
                    myDevice.StopField();
                    tagsLeft = tagList.Count;
                }
                else
                {
                    for (int i = 1; i < nbAxisOnDevice + 1; ++i)
                        myDevice.StartLedOn(i);

                    // confirms presence of tags & turn their light on, on each axis
                    foreach (int currentAxis in axisNotEmpty)
                    {
                        myDevice.ConfirmAndLight(currentAxis, tagList);
                        myDevice.StopField();
                    }

                    // ConfirmAndLight has been called "axisNotEmpty.Count" times but may have not lighted all Tags
                    // Check if tags left can be found on other axis
                    tagsLeft = tagList.Count;
                    if (tagsLeft > 0) // if tags left to be found
                    {
                        for (int i = 1; i < nbAxisOnDevice + 1; ++i)
                        {
                            if (axisNotEmpty.Contains(i)) continue;
                            myDevice.ConfirmAndLight(i, tagList);  // tagList is passed to ConfirmAndLight again, but on a new axis                                              

                            if (tagsLeft > tagList.Count) // tag(s) removed from tagList by ConfirmAndList : 1 or more tags have been found on this new axis
                                axisNotEmpty.Add(i); // add the axis to our axisNotEmpty list

                            if (tagList.Count == 0) break;
                        }
                    }
                }


                if (nbTagAtStartToLight != tagList.Count) //au moins 1 enleve on blinke
                {
                    // Enable lighting on all axis NOT EMPTY in a thread
                    axisNotEmpty.Sort(); // sort axis numbers to light them from the smallest to the biggest and avoiding useless click-click-click (and lighting from top to bottom in Fridges) :)
                    _currentLightingThread = new Thread(() => LightAllAxisThreaded(axisNotEmpty, 1));
                    _currentLightingThread.Start();
                }
                else //no tag found 
                {
                    deviceStatus = DeviceStatus.DS_Ready;
                }
            }
            else //No tag to light so put deviec ready
                deviceStatus = DeviceStatus.DS_Ready;

            if (tagRmoved.Count > 0)
            {
                foreach (string uid in tagRmoved)
                {
                    if (!tagList.Contains(uid))
                        tagList.Add(uid);
                }
            }

            isInSearchTag = false;
            return (tagList.Count == tagsLeft);
           
            /*LedOnAll();
            return true;*/
        }

        private void LedOnAll()
        {
            int nbAxisOnDevice = GetCurrentNumberOfAxis();
            List<int> axisNotEmpty = new List<int>();

            for (int loop = 1 ; loop <= nbAxisOnDevice ; loop++)
                axisNotEmpty.Add(loop);

            // Enable lighting on all axis NOT EMPTY in a thread
            axisNotEmpty.Sort(); // sort axis numbers to light them from the smallest to the biggest and avoiding useless click-click-click (and lighting from top to bottom in Fridges) :)
            _currentLightingThread = new Thread(() => LightAllAxisThreaded(axisNotEmpty, 1));
            _currentLightingThread.Start();
            bWasInLedDoorOpen = true;
            deviceStatus = DeviceStatus.DS_LedOn;

        }


        public bool TestLightingOneAxis(List<string> tagList , int axis , bool bRunThread = true)
        {

           SetLight(0);  // Led OFF
           bool ret =  myDevice.StartLedOn2(axis);
           int tagsLeft = tagList.Count;
           List<int> axisNotEmpty = new List<int>();
            if (ret)
            {
               
                tagsLeft = tagList.Count;
                if (tagsLeft > 0) // if tags left to be found
                {
                    myDevice.ConfirmAndLightWithKD(axis, tagList); // tagList is passed to ConfirmAndLight again, but on a new axis
                    myDevice.StopField();

                }
            }

            if (bRunThread)
            {
                if (tagsLeft > tagList.Count)
                    // tag(s) removed from tagList by ConfirmAndList : 1 or more tags have been found on this new axis
                {
                    bWasInLedDoorOpen = true;
                    deviceStatus = DeviceStatus.DS_LedOn;
                    axisNotEmpty.Add(1);
                    _currentLightingThread = new Thread(() => LightAllAxisThreaded(axisNotEmpty, 1));
                    _currentLightingThread.Start();

                }
            }
            else
            {
               // if (myDevice != null)
                   // myDevice.LEdOnAll( 1, 1, false);
                bWasInLedDoorOpen = true;
                deviceStatus = DeviceStatus.DS_LedOn;
            }

            return (tagList.Count == tagsLeft);
        }

        public bool ConfirmList(List<string> tagList,TagType tagtype)
        {
            if (listTagWithChannel == null) return false;

            int nbAxisOnDevice = GetCurrentNumberOfAxis();
            List<int> axisNotEmpty = new List<int>();

            // Fill the list of used axis (= axis upon which scan has detected tags)
            foreach (DictionaryEntry entry in listTagWithChannel)
                if (tagList.Contains((string)entry.Key)) // only if the tag is concerned by our tagList
                    if (!axisNotEmpty.Contains((byte)entry.Value)) // only if we didn't already add the axis to our axisNotEmpty list
                        axisNotEmpty.Add((byte)entry.Value); // axis has to be used : add it to our list

           
            // unlock all axis
            for (int i = 1; i < nbAxisOnDevice + 1; ++i)
                myDevice.StartLedOn(i);

            // confirms presence of tags & turn their light on, on each axis
            foreach (int currentAxis in axisNotEmpty)
            {
                myDevice.ConfirmList(currentAxis, tagList, tagtype);
                myDevice.StopField();
            }

            // ConfirmAndLight has been called "axisNotEmpty.Count" times but may have not lighted all Tags
            // Check if tags left can be found on other axis
            int tagsLeft = tagList.Count;
            if (tagsLeft > 0) // if tags left to be found
            {
                for (int i = 1; i < nbAxisOnDevice + 1; ++i)
                {
                    if (axisNotEmpty.Contains(i)) continue;
                    myDevice.ConfirmList(i, tagList, tagtype); // tagList is passed to ConfirmAndLight again, but on a new axis

                    if (tagsLeft > tagList.Count) // tag(s) removed from tagList by ConfirmAndList : 1 or more tags have been found on this new axis
                        axisNotEmpty.Add(i); // add the axis to our axisNotEmpty list

                    if (tagList.Count == 0) break;
                }
            }
            return (tagList.Count == tagsLeft);
        }

        public bool ConfirmAlphaUID(string AlphatagUID, int forcedAxis = -1)
        {
            int axis = 0;

            if (forcedAxis == -1)
            {
            if (listTagWithChannel == null) return false;
            foreach (DictionaryEntry entry in listTagWithChannel)
            {
                if (entry.Key.Equals(AlphatagUID))
                {
                    axis = int.Parse(entry.Value.ToString());
                    break;
                }
            }
            }

            else
                axis = forcedAxis;

            if (axis < 1) return false;

            //Convert Tag UID alphaNumerique to 24 digits octal.
            string alphaUID = AlphatagUID + "§00000000000000000000";
            string octTagID = SDK_SC_RfidReader.DeviceBase.SerialRFID.ConvertAlphaNumToOct(alphaUID.Substring(0, 10), TagType.TT_SPCE2_RO);
            return myDevice.ConfirmTagUID(axis, octTagID);
        }


        public WriteCode WriteBackRO(string oldAlphaUID, string newAlphaUID)
        {
            int[] statusWrite = null;
            if (listTagWithChannel == null) return WriteCode.WC_Error;

            if ((!SDK_SC_RfidReader.DeviceBase.SerialRFID.isStringValidToWrite(newAlphaUID)) || (newAlphaUID.Length > 10))
                return WriteCode.WC_Error;

            int axis = 0;
           
            // search axis of the last scan where tag was found
            foreach (DictionaryEntry entry in listTagWithChannel)
            {
                if (!entry.Key.Equals(oldAlphaUID)) continue;

                axis = int.Parse(entry.Value.ToString());
                break;
            }

            // if not found put the first axis
            if (axis == 0) axis = 1;

            try
            {
                string octNewAlpha = string.Empty;

                newAlphaUID += "00000000000000000000";
                //newAlphaUID = newAlphaUID.PadRight(10, ' ').ToUpper(); // pad with space to have the good length (10 chars max for an alphanumeric ID)
                newAlphaUID = newAlphaUID.Substring(0, 10).ToUpper();

                //octNewAlpha = SDK_SC_RfidReader.DeviceBase.SerialRFID.ConvertAlphaNumToOct(newAlphaUID, TagType.TT_SPCE2_RO);
                octNewAlpha = newAlphaUID;
                string octTagID = myDevice.getTagUidOct(oldAlphaUID).Substring(0, 18);

                if ((octTagID == null) || (octTagID.Length > 20))
                    return WriteCode.WC_Error;


                byte[] codeRead = SDK_SC_RfidReader.DeviceBase.SerialRFID.ConvertOctToUIDCodeWithCRC(octNewAlpha);
                string[] eElatchWord = SDK_SC_RfidReader.DeviceBase.SerialRFID.ConvertCodeToHexa(codeRead);

                int nbAxisPass = 0;
                int axisUsedForWrite = 0;
                do
                {
                    axisUsedForWrite = axis;
                    statusWrite = myDevice.WriteBlockFullMem(4, eElatchWord, octTagID, axis++);
                    if (axis > GetCurrentNumberOfAxis()) axis = 1; // we tried the last axis. Get back to the first one, to test all axises
                    ++nbAxisPass; // how many axises we tried (we want to try all axises)

                } while ((statusWrite[4] != 3) && (nbAxisPass < GetCurrentNumberOfAxis()));

                if (statusWrite[4] == 3) //Write process send completely, comfirm new UID
                {
                    string NewUidWithCrc = string.Empty;

                    for (int i = 0; i < 42; i++)
                        NewUidWithCrc += (char)('0' + codeRead[i]);
                    if (myDevice.StartLedOn2(axisUsedForWrite))
                        if (myDevice.ConfirmTagUIDFullMem(NewUidWithCrc, 42))
                        statusWrite[5] = 1;
                    else
                        statusWrite[5] = 0;
                }

            }
            catch (Exception exp)
            {
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            finally
            {
                myDevice.StopField();
            }


            if (statusWrite == null)
                return WriteCode.WC_Error;


            WriteCode resultCode = WriteCode.WC_TagNotDetected;

            if (statusWrite[0] == 0)
                resultCode = WriteCode.WC_TagNotDetected;

            else if (statusWrite[1] == 0)
                resultCode = WriteCode.WC_TagNotConfirmed;

            else if (statusWrite[2] != 3)
            {
                switch (statusWrite[2])
                {
                    case 0: resultCode = WriteCode.WC_TagBlockedOrNotSupplied; break;
                    case 1: resultCode = WriteCode.WC_TagNotSupplied; break;
                    case 2: resultCode = WriteCode.WC_TagBlocked; break;
                }
            }

            else if (statusWrite[3] != 3)
            {
                switch (statusWrite[3])
                {
                    case 0: resultCode = WriteCode.WC_TagBlockedOrNotSupplied; break;
                    case 1: resultCode = WriteCode.WC_TagNotSupplied; break;
                    case 2: resultCode = WriteCode.WC_TagBlocked; break;
                }
            }

            else if (statusWrite[4] != 3)
            {
                switch (statusWrite[4])
                {
                    case 0: resultCode = WriteCode.WC_TagBlockedOrNotSupplied; break;
                    case 1: resultCode = WriteCode.WC_TagNotSupplied; break;
                    case 2: resultCode = WriteCode.WC_TagBlocked; break;
                }
            }

            else if (statusWrite[5] != 1)
            {
                resultCode = WriteCode.WC_ConfirmationFailed;
            }

            else
            {
                resultCode = WriteCode.WC_Success;
            }

            return resultCode;
        }

        public WriteCode WriteNewUID(string oldAlphaUID , string newAlphaUID , bool bLock=false)
        {
            int[] statusWrite = null;
            if (listTagWithChannel == null) return WriteCode.WC_Error;

            if ((!SDK_SC_RfidReader.DeviceBase.SerialRFID.isStringValidToWrite(newAlphaUID)) || (newAlphaUID.Length > 10))
                return WriteCode.WC_Error;

            int axis = 0;
            /*oldAlphaUID += "§0000000000";
           // oldAlphaUID = oldAlphaUID.PadRight(10, ' ').ToUpper();
            oldAlphaUID = oldAlphaUID.Substring(0, 10).ToUpper();*/
            // search axis of the last scan where tag was found
            foreach (DictionaryEntry entry in listTagWithChannel)
            {
                if (!entry.Key.Equals(oldAlphaUID)) continue;

                    axis = int.Parse(entry.Value.ToString());
                    break;
                }

            // if not found put the first axis
            if (axis == 0) axis = 1;

            try
            {
                string octNewAlpha = string.Empty;
               
                newAlphaUID += "§00000000000000000000";
                //newAlphaUID = newAlphaUID.PadRight(10, ' ').ToUpper(); // pad with space to have the good length (10 chars max for an alphanumeric ID)
                newAlphaUID = newAlphaUID.Substring(0, 10).ToUpper();

                octNewAlpha = SDK_SC_RfidReader.DeviceBase.SerialRFID.ConvertAlphaNumToOct(newAlphaUID, TagType.TT_SPCE2_RW);
                //string octTagID = SDK_SC_RfidReader.DeviceBase.SerialRFID.ConvertAlphaNumToOct(oldAlphaUID);*/
                
               
                string octTagID = myDevice.getTagUidOct(oldAlphaUID);
                 
                if ((octTagID == null) || (octTagID.Length >20))
                    return WriteCode.WC_Error;


                byte[] codeRead = SDK_SC_RfidReader.DeviceBase.SerialRFID.ConvertOctToUIDCodeWithCRC(octNewAlpha);
                string[] eElatchWord = SDK_SC_RfidReader.DeviceBase.SerialRFID.ConvertCodeToHexa(codeRead);
              
                if (bLock) //to do to set Lock bits
                {
                    string highVal = eElatchWord[3].Substring(0, 2);
                    string LowVal = eElatchWord[3].Substring(2);
                    int hexVal = int.Parse(highVal, NumberStyles.HexNumber);
                    int LockMask = 0x40;
                    int newVal = hexVal |LockMask;
                    eElatchWord[3] = newVal.ToString("X2") + LowVal;
                }


                int nbAxisPass = 0;
                int axisUsedForWrite = 0;
                do
                {
                    axisUsedForWrite = axis;
                    statusWrite = myDevice.WriteBlock(3, eElatchWord, octTagID, axis++);
                    if (axis > GetCurrentNumberOfAxis()) axis = 1; // we tried the last axis. Get back to the first one, to test all axises
                    ++nbAxisPass; // how many axises we tried (we want to try all axises)

                } while ((statusWrite[4] != 3) && (nbAxisPass < GetCurrentNumberOfAxis()));

                if (statusWrite[4] == 3) //Write process send completely, comfirm new UID
                {
                    if (myDevice.StartLedOn2(axisUsedForWrite))
                        if (myDevice.ConfirmTagUID(axisUsedForWrite, octNewAlpha))
                         statusWrite[5] = 1;
                    else
                         statusWrite[5] = 0;
                }

            } 
            catch (Exception exp)
            {
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            finally
            {
                myDevice.StopField();
            }


            if (statusWrite == null)
                return WriteCode.WC_Error;


            WriteCode resultCode = WriteCode.WC_TagNotDetected;

            if (statusWrite[0] == 0)
                resultCode = WriteCode.WC_TagNotDetected;

            else if (statusWrite[1] == 0)
                resultCode = WriteCode.WC_TagNotConfirmed;

            else if (statusWrite[2] != 3)
            {
                switch (statusWrite[2])
                {
                    case 0: resultCode = WriteCode.WC_TagBlockedOrNotSupplied; break;
                    case 1: resultCode = WriteCode.WC_TagNotSupplied; break;
                    case 2: resultCode = WriteCode.WC_TagBlocked; break;
                }
            }

            else if (statusWrite[3] != 3)
            {
                switch (statusWrite[3])
                {
                    case 0: resultCode = WriteCode.WC_TagBlockedOrNotSupplied; break;
                    case 1: resultCode = WriteCode.WC_TagNotSupplied; break;
                    case 2: resultCode = WriteCode.WC_TagBlocked; break;
                }
            }

            else if (statusWrite[4] != 3)
            {
                switch (statusWrite[4])
                {
                    case 0: resultCode = WriteCode.WC_TagBlockedOrNotSupplied; break;
                    case 1: resultCode = WriteCode.WC_TagNotSupplied; break;
                    case 2: resultCode = WriteCode.WC_TagBlocked; break;
                }
            }

            else if (statusWrite[5] != 1)
            {
                resultCode = WriteCode.WC_ConfirmationFailed;
            }

            else
            {
                resultCode = WriteCode.WC_Success;
            }

            return resultCode;
        }

        public WriteCode WriteNewUidWithFamily(string oldAlphaUID, string newAlphaUID, bool bLock = false)
        {
            int[] statusWrite = null;
            if (listTagWithChannel == null) return WriteCode.WC_Error;
            if ((!SDK_SC_RfidReader.DeviceBase.SerialRFID.isStringValidToWrite(newAlphaUID)) || (newAlphaUID.Length > 17))
                return WriteCode.WC_Error;
            int axis = 0;
          
            // search axis of the last scan where tag was found
            foreach (DictionaryEntry entry in listTagWithChannel)
            {
                if (!entry.Key.Equals(oldAlphaUID)) continue;

                axis = int.Parse(entry.Value.ToString());
                break;
            }

            // if not found put the first axis
            if (axis == 0) axis = 1;

            try
            {
                string octNewAlpha = "2";  // add family digits
                newAlphaUID += "§00000000000000000000"; // Add EOF + 0 Paddings
                newAlphaUID = newAlphaUID.Substring(0, 17).ToUpper();  // 17 Alpha in full Memory
                octNewAlpha += SDK_SC_RfidReader.DeviceBase.SerialRFID.ConvertAlphaNumToOct(newAlphaUID, TagType.TT_SPCE2_RW);

                string octTagID = myDevice.getTagUidOct(oldAlphaUID);

                byte[] codeRead = SDK_SC_RfidReader.DeviceBase.SerialRFID.ConvertOctToUIDCodeWithCRC(octNewAlpha);
                string[] eElatchWord = SDK_SC_RfidReader.DeviceBase.SerialRFID.ConvertCodeToHexa(codeRead);

               
                if (bLock) //to do to set Lock bits
                {
                    string highVal = eElatchWord[3].Substring(0, 2);
                    string LowVal = eElatchWord[3].Substring(2);
                    int hexVal = int.Parse(highVal, NumberStyles.HexNumber);
                    int LockMask = 0x40;
                    int newVal = hexVal | LockMask;
                    eElatchWord[3] = newVal.ToString("X2") + LowVal;
                }

                int nbAxisPass = 0;
                int axisUsedForWrite = 0;
                do
                {
                    axisUsedForWrite = axis;
                    statusWrite = myDevice.WriteBlockFullMem(4, eElatchWord, octTagID, axis++);
                    if (axis > GetCurrentNumberOfAxis()) axis = 1; // we tried the last axis. Get back to the first one, to test all axises
                    ++nbAxisPass; // how many axises we tried (we want to try all axises)

                } while ((statusWrite[4] != 3) && (nbAxisPass < GetCurrentNumberOfAxis()));

                if (statusWrite[4] == 3) //Write process send completely, comfirm new UID
                {
                    string NewUidWithCrc = string.Empty;

                    for (int i = 0; i < 42; i ++)
                        NewUidWithCrc += (char) ('0' + codeRead[i]);

                    if (myDevice.StartLedOn2(axisUsedForWrite))
                        if (myDevice.ConfirmTagUIDFullMem(NewUidWithCrc, 42))
                        statusWrite[5] = 1;
                    else
                        statusWrite[5] = 0;
                }
            }
            catch (Exception exp)
            {
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            finally
            {
                myDevice.StopField();
            }


            if (statusWrite == null)
                return WriteCode.WC_TagNotDetected;


            WriteCode resultCode = WriteCode.WC_TagNotDetected;

            if (statusWrite[0] == 0)
                resultCode = WriteCode.WC_TagNotDetected;

            else if (statusWrite[1] == 0)
                resultCode = WriteCode.WC_TagNotConfirmed;

            else if (statusWrite[2] != 3)
            {
                switch (statusWrite[2])
                {
                    case 0: resultCode = WriteCode.WC_TagBlockedOrNotSupplied; break;
                    case 1: resultCode = WriteCode.WC_TagNotSupplied; break;
                    case 2: resultCode = WriteCode.WC_TagBlocked; break;
                }
            }

            else if (statusWrite[3] != 3)
            {
                switch (statusWrite[3])
                {
                    case 0: resultCode = WriteCode.WC_TagBlockedOrNotSupplied; break;
                    case 1: resultCode = WriteCode.WC_TagNotSupplied; break;
                    case 2: resultCode = WriteCode.WC_TagBlocked; break;
                }
            }

            else if (statusWrite[4] != 3)
            {
                switch (statusWrite[4])
                {
                    case 0: resultCode = WriteCode.WC_TagBlockedOrNotSupplied; break;
                    case 1: resultCode = WriteCode.WC_TagNotSupplied; break;
                    case 2: resultCode = WriteCode.WC_TagBlocked; break;
                }
            }

            else if (statusWrite[5] != 1)
            {
                resultCode = WriteCode.WC_ConfirmationFailed;
            }

            else
            {
                resultCode = WriteCode.WC_Success;
            }

            return resultCode;
        }

        public WriteCode WriteNewUidDecimal(string oldAlphaUID, string int64ToWrite, bool bLock = false)
        {

            int[] statusWrite = null;
            if (listTagWithChannel == null) return WriteCode.WC_Error;

            System.Text.RegularExpressions.Regex myRegex = new Regex(@"^[0-9]+$");
            if ((!myRegex.IsMatch(int64ToWrite)) || (int64ToWrite.Length > 12)) 
                return WriteCode.WC_Error;
                

            int axis = 0;

            // search axis of the last scan where tag was found
            foreach (DictionaryEntry entry in listTagWithChannel)
            {
                if (!entry.Key.Equals(oldAlphaUID)) continue;

                axis = int.Parse(entry.Value.ToString());
                break;
            }

            // if not found put the first axis
            if (axis == 0) axis = 1;


            try
            {
                Int64 intVal = Int64.Parse(int64ToWrite);
                // add family digits
                string valOct = Convert.ToString(intVal,8).PadLeft(14,'0');
               
                //add family 
                string octNewAlpha = "1" + valOct;
                octNewAlpha = octNewAlpha.PadRight(35, '0');

                string octTagID = myDevice.getTagUidOct(oldAlphaUID);

                byte[] codeRead = SDK_SC_RfidReader.DeviceBase.SerialRFID.ConvertOctToUIDCodeWithCRC(octNewAlpha);
                string[] eElatchWord = SDK_SC_RfidReader.DeviceBase.SerialRFID.ConvertCodeToHexa(codeRead);

            
                if (bLock) //to do to set Lock bits
                {
                    string highVal = eElatchWord[3].Substring(0, 2);
                    string LowVal = eElatchWord[3].Substring(2);
                    int hexVal = int.Parse(highVal, NumberStyles.HexNumber);
                    int LockMask = 0x40;
                    int newVal = hexVal | LockMask;
                    eElatchWord[3] = newVal.ToString("X2") + LowVal;
                }

                int nbAxisPass = 0;
                int axisUsedForWrite = 0;
                do
                {
                    axisUsedForWrite = axis;
                    statusWrite = myDevice.WriteBlockFullMem(4, eElatchWord, octTagID, axis++);
                    if (axis > GetCurrentNumberOfAxis()) axis = 1; // we tried the last axis. Get back to the first one, to test all axises
                    ++nbAxisPass; // how many axises we tried (we want to try all axises)

                } while ((statusWrite[4] != 3) && (nbAxisPass < GetCurrentNumberOfAxis()));

                if (statusWrite[4] == 3) //Write process send completely, comfirm new UID
                {
                    string NewUidWithCrc = string.Empty;

                    for (int i = 0; i < 42; i++)
                        NewUidWithCrc += (char)('0' + codeRead[i]);

                    if (myDevice.StartLedOn2(axisUsedForWrite))
                        if (myDevice.ConfirmTagUIDFullMem(NewUidWithCrc, 42))
                            statusWrite[5] = 1;
                        else
                            statusWrite[5] = 0;
                }
            }
            catch (Exception exp)
            {
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            finally
            {
                myDevice.StopField();
            }


            if (statusWrite == null)
                return WriteCode.WC_TagNotDetected;


            WriteCode resultCode = WriteCode.WC_TagNotDetected;

            if (statusWrite[0] == 0)
                resultCode = WriteCode.WC_TagNotDetected;

            else if (statusWrite[1] == 0)
                resultCode = WriteCode.WC_TagNotConfirmed;

            else if (statusWrite[2] != 3)
            {
                switch (statusWrite[2])
                {
                    case 0: resultCode = WriteCode.WC_TagBlockedOrNotSupplied; break;
                    case 1: resultCode = WriteCode.WC_TagNotSupplied; break;
                    case 2: resultCode = WriteCode.WC_TagBlocked; break;
                }
            }

            else if (statusWrite[3] != 3)
            {
                switch (statusWrite[3])
                {
                    case 0: resultCode = WriteCode.WC_TagBlockedOrNotSupplied; break;
                    case 1: resultCode = WriteCode.WC_TagNotSupplied; break;
                    case 2: resultCode = WriteCode.WC_TagBlocked; break;
                }
            }

            else if (statusWrite[4] != 3)
            {
                switch (statusWrite[4])
                {
                    case 0: resultCode = WriteCode.WC_TagBlockedOrNotSupplied; break;
                    case 1: resultCode = WriteCode.WC_TagNotSupplied; break;
                    case 2: resultCode = WriteCode.WC_TagBlocked; break;
                }
            }

            else if (statusWrite[5] != 1)
            {
                resultCode = WriteCode.WC_ConfirmationFailed;
            }

            else
            {
                resultCode = WriteCode.WC_Success;
            }

            return resultCode;
        }

        private int GetCurrentNumberOfAxis()
        {
            int nbAxisOnDevice = myDevice.DeviceBoard.getNumberMaxChannel();

            if ((nbAxisOnDevice != 0)  && (nbAxisOnDevice <=12)) return nbAxisOnDevice;

            switch (deviceType)
            {
                case DeviceType.DT_STR:
                    nbAxisOnDevice = 1; break;
                case DeviceType.DT_DSB:
                case DeviceType.DT_MSR:
                case DeviceType.DT_SAS:
                    nbAxisOnDevice = 3; break;
                case DeviceType.DT_SBR:
                    nbAxisOnDevice = 4; break;
                case DeviceType.DT_JSC:
                    nbAxisOnDevice = 9; break;
            }

            return nbAxisOnDevice;
        }

        private void ContinousLightingThreaded(int axis, int timeout)
        {
            while (Thread.CurrentThread.IsAlive)
            {
                if (myDevice.IsInScan) break;
                myDevice.LEdOnAll(axis, timeout, false);
                myDevice.StopField();
                Thread.Sleep(200);
            }

        }

        private void LightAllAxisThreaded(List<int> axisNotEmpty, int timeout)
        {
            while (Thread.CurrentThread.IsAlive)
            {
                if ((myDevice != null) && (myDevice.IsInScan)) break;
                if (deviceStatus == DeviceStatus.DS_InScan) break;
                if (deviceStatus == DeviceStatus.DS_WaitForScan) break;
                if (deviceStatus == DeviceStatus.DS_DoorClose) break;

                //still in led - put status again to put in led if door open and thread not stopped
                deviceStatus = DeviceStatus.DS_LedOn;
                bool changeChannel = (axisNotEmpty.Count > 1); // if only one axis, do not change channel while LedOnAll. Otherwise, change.

                if (!myDevice.HardwareVersion.StartsWith("2")) // Pas une JSC
                {
                    foreach (int currentAxis in axisNotEmpty)
                    {
                        if (myDevice != null)
                            myDevice.LEdOnAll(currentAxis, timeout, changeChannel);
                        if (myDevice != null)
                            myDevice.StopField();

                    }
                }
                else
                {
                    int nbAxisOnDevice = GetCurrentNumberOfAxis();
                    for (int currentAxis = 1; currentAxis <= nbAxisOnDevice; currentAxis++)
                    {
                        if (myDevice != null)
                            myDevice.LEdOnAll(currentAxis, timeout, changeChannel);
                        if (myDevice != null)
                            myDevice.StopField();
                    }
                }
            }
        }

       

        /// <summary>
        /// Used by server to answer client's request (IS_SPEC2AVAIlABLE)
        /// </summary>
        /// <returns>true if device can handle LEDs lighting (according to its firmware version), false otherwhise.</returns>
        public bool IsSpec2Available()
        {
            if (myDevice != null)
                return (!myDevice.FirmwareVersion.StartsWith("1"));
                    // firmware started with version 1.x.x and LEDs are available since firmware 2+
           return false;
        }

        /// <summary>
        /// Used by server to answer client's request (Get Firmware version)
        /// </summary>
        /// <returns>true if device can handle LEDs lighting (according to its firmware version), false otherwhise.</returns>
        public double GetfirwareVersion()
        {
            double fv = 0.0;
            if (myDevice != null)
                double.TryParse(myDevice.FirmwareVersion.Replace(",", "."), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out fv);
            return(fv); 
        }
       
    }

    /// <summary>
    /// Classe to define a device
    /// </summary>
    public class deviceClass : ICloneable
    {
        /// <summary>
        /// 
        /// </summary>
        public DeviceInfo infoDev;
        /// <summary>
        /// 
        /// </summary>
        public RFID_Device rfidDev;
        /// <summary>
        /// 
        /// </summary>
        public InventoryData previousInventory;
        /// <summary>
        /// 
        /// </summary>
        public InventoryData currentInventory;

        /// <summary>
        /// 
        /// </summary>
        public bool bComeFromKZ = false;
        /// <summary>
        /// 
        /// </summary>
        public int cptOut = 1;

        public uint inventorySequence = 0;
        public Hashtable tagList;

        //Frige or cabinet object

        public MedicalCabinet myMedicalCabinet;
        public tempInfo currentTemp = null;
        public tempInfo previousTemp = null;

        public FridgeCabinet myFridgeCabinet;

        // for mini SAS
        public clBadgeReader masterBadgerReader;
        public clBadgeReader slaveBadgerReader;

        public bool bDataCompleted = true;  // KB use to check that the device is always in scan between device RFID ready and data ready

        //network device info
        public bool netFridgeTemperatureProcessDone;
        public ConnectionStatus netConnectionStatus;
        public DeviceStatus netDeviceStatus;
        public bool bnetWaitTagMode = false;
        public bool bnetAccumulateMode = false;
        public DateTime nettimeLastScan;
        public DateTime lastDoorOpen;
        public DateTime lastProcessInventoryGmtDate;
        public bool firstDoorOpenDetected = true;
        public int netLastScanEventID;
        public bool bUseTcpNotification = false;
        public int cptPoll = 0;
        public deviceClass()
        {
            previousInventory = new InventoryData();
            currentInventory = new InventoryData();
            netConnectionStatus = ConnectionStatus.CS_Disabled;
            netDeviceStatus = DeviceStatus.DS_NotReady;
            tagList = new Hashtable();
            bDataCompleted = true;
            netFridgeTemperatureProcessDone = false;
            netLastScanEventID = -1;
        }
       
        public deviceClass(Hashtable ColumnInfo)
        {
            previousInventory = new InventoryData(ColumnInfo);
            currentInventory = new InventoryData(ColumnInfo);
            netConnectionStatus = ConnectionStatus.CS_Disabled;
            netDeviceStatus = DeviceStatus.DS_NotReady;
            tagList = new Hashtable();
            bDataCompleted = true;
            // Ie max value to not take care when class is created
            lastProcessInventoryGmtDate = DateTime.MaxValue;
            netFridgeTemperatureProcessDone = false;
            netLastScanEventID = -1;
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
   
}
