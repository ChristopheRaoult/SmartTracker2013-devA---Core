

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Runtime.InteropServices;
using System.Xml;
using System.Net;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using System.Management;
using System.Security.Principal;

using ErrorMessage;

namespace DataClass
{
    static public class VarGlobal
    {
        public const int Nb_Max_Column_Product = 100;
    }
 
    public enum AccessType
    {
        AT_NONE = 0x00,
        AT_FINGERPRINT = 0x01,
        AT_BADGEREADER = 0x02,
    }

    #region mail
    public enum AlertType
    {
        AT_Power_Cut = 0,
        AT_Usb_Unplug = 1,
        AT_Door_Open_Too_Long = 2,
        AT_Finger_Alert = 3,
        AT_Remove_Too_Many_Items = 4,
        AT_Limit_Value_Exceed = 5,
        AT_Move_Sensor = 6,
        AT_Max_Fridge_Temp = 7,
        AT_Remove_Tag_Max_Time = 8,
        AT_DLC_Expired = 9,
        AT_Stock_Limit = 10,
        AT_Bad_Blood_Patient = 11,
    }

    public class smtpInfo
    {
        public string smtp = null;
        public int port = 25;
        public string sender = null;
        public string login = null;
        public string pwd = null;
        public bool bUseSSL = false;
        public bool bActive = false;
    }

    public class alertInfo
    {
        public AlertType type;
        public string AlertName = null;
        public string RecipientList = null;
        public string CCRecipientList = null;
        public string BCCRecipientList = null;
        public string MailSubject = null;
        public string AlertMessage = null;
        public bool bActive = false;
        public string alertData = null;
    }

    #endregion
    public enum TagEventType
    {
        TET_Removed = 0,
        TET_Added = 1,
        TET_Present = 2,
    }
    public enum ConnectionStatus
    {
        [Description("Device Connected")]    
        CS_Connected = 0,
        [Description("Device Disconnected")]    
        CS_Disconnected = 1,
        [Description("Device in Connection")]    
        CS_InConnection = 2,
        [Description("Device Disabled")]    
        CS_Disabled = 3,
    }
    public enum DeviceStatus
    {
        [Description("Device Not Ready")]    
        DS_NotReady = 0,
        [Description("Device Ready")]  
        DS_Ready = 1,
        [Description("Door Opened")]  
        DS_DoorOpen = 2,
        [Description("Device In Scan")]  
        DS_InScan = 3,
        [Description("Device In WaitTag Mode")]  
        DS_WaitTag = 4,
        [Description("Device In Error")]  
        DS_InError = 5,
        [Description("Device Flash Firmware")]  
        DS_FlashFirmware = 6,
        [Description("Door Closed")]
        DS_DoorClose = 7,
        [Description("LEDs On")]
        DS_LedOn = 8,
    }
    public enum FPStatus
    {
        FS_Ready,
        FS_Disconnected,
        FS_FingerTouch,
        FS_FingerRemove,
        FS_CaptureComplete,
        FS_UnknownUser,
    }
    public enum DeviceType
    {
            
        DT_DSB = 1,    // DSB V1
        DT_JSC = 2,    // JSC
        DT_SMC = 3,    // Smart medical cabinet
        DT_SBX = 4,    // smartBox
        DT_STR = 5,    // smartStation
        DT_SBR = 6,    // SBR        
        DT_SAS = 7,    // SAS
        DT_SFR = 8,    // Fridge Carel
        DT_MSR = 9,    // miniSAS
        DT_SBF = 10,   // blood fridge
        DT_PAD = 11,   // smartPad
        DT_CAT = 12,   // Cathrack
        DT_MCS = 13,   // medical cabinet schoff 
        DT_WAL = 14,   // Wall drawer

        DT_UNKNOWN = 0,
    }

    public enum WriteCode
    {
        [Description("Tag not detected")]
        WC_TagNotDetected = 0,
        [Description("Tag not confirmed")]
        WC_TagNotConfirmed = 1,
        [Description("Tag blocked or not supplied")]
        WC_TagBlockedOrNotSupplied = 2,
        [Description("Tag blocked")]
        WC_TagBlocked = 3,
        [Description("Tag not supplied")]
        WC_TagNotSupplied = 4,
        [Description("Writing confirmation failed")]
        WC_ConfirmationFailed = 5,
        [Description("Tag UID written")]
        WC_Success = 6,
        [Description("Unknown error")]
        WC_Error = 7,
    }

    public enum FridgeType
    {
        FT_CAREL = 1,
        FT_EVERCOM = 2,    
        FT_PT100 = 3,
        FT_FANEM = 4,
        FT_UNKNOWN = 0,
    }
    public enum AccessBagerReaderType
      {
            RT_LF = 0x00,
            RT_HF = 0x01,
      }
    public enum DoorInfo
      {
          DI_NO_DOOR = 0x00,
          DI_MASTER_DOOR = 0x01,
          DI_SLAVE_DOOR = 0x02,
      }
    public enum UserGrant
      {
          [Description("NONE")]     
          UG_NONE = 0x00,
          [Description("MASTER")]   
          UG_MASTER = 0x01,
          [Description("SLAVE")]     
          UG_SLAVE = 0x02,
          [Description("MASTER AND SLAVE")]     
          UG_MASTER_AND_SLAVE = 0x3,
      }

     [Serializable]
      public class DeviceGrant
      {
          public UserClassTemplate user;
          public string serialRFID;
          public UserGrant userGrant;

          public DeviceGrant()
          {
              userGrant = UserGrant.UG_NONE;
              user = new UserClassTemplate();
          }
      }

    [Serializable]
    public class UserClassTemplate
    {
        public int idUser;
        public string firstName;
        public string lastName;
        public string template;
        public bool[] isFingerEnrolled = new bool[10];
        public string BadgeReaderID;        
    }

    public class rfidPluggedInfo
    {
       
        public DeviceType deviceType;
        public string SerialRFID;
        public string portCom;
        public string SoftwareVersion;
        public string HardwareVersion;
    }

    public class dtColumnInfo
    {
        public int colIndex;
        public string colName;
        public Type colType;
        public int colDoSum;
    }

    [Serializable]
    public class DeviceInfo
    {   
        public int idDevice;        
        public string DeviceName;
        public DeviceType deviceType;
        public FridgeType fridgeType = FridgeType.FT_UNKNOWN;
        public string SerialRFID;
        public string SerialFPMaster;
        public string SerialFPSlave;
        public byte bLocal;
        public string IP_Server;
        public int Port_Server;
        public int enabled;
        //public string comLCD;
        public string comTempReader;
        public string comSlaveReader;
        public string comMasterReader;
        public AccessBagerReaderType accessReaderType;
     
    }

    [Serializable]
    public class InventoryData 
    {
        public int idDevice = -1;
        public int idUser = -1;
        public int IdScanEvent = -1;
        public string scanStatus = "Not Processed";
        public string serialNumberDevice = "xxxxxxxx";
        public DateTime eventDate = DateTime.UtcNow.AddDays(-1.0);
        public bool bUserScan = false;
        public AccessType accessType = AccessType.AT_NONE;
        public string userFirstName = "Manual";
        public string userLastName = "Scan";
        public string BadgeID = null;
        public DoorInfo userDoor = DoorInfo.DI_NO_DOOR;
        public int nbTagAll = 0;
        public int nbTagPresent = 0;
        public int nbTagAdded = 0;
        public int nbTagRemoved = 0;
        public ArrayList listTagAll = new ArrayList();
        public ArrayList listTagPresent = new ArrayList();
        public ArrayList listTagAdded = new ArrayList();
        public ArrayList listTagRemoved = new ArrayList();

        public DataTable dtTagAll = new DataTable();
        public DataTable dtTagPresent = new DataTable();
        public DataTable dtTagAdded = new DataTable();
        public DataTable dtTagRemove = new DataTable();

        // for fridge shelve localisation
        public Hashtable ListTagWithChannel;

        public string spareData1 = null;
        public string spareData2 = null;

        public InventoryData()
        {
            dtTagAll.Columns.Add("TagUID", typeof(string));
            dtTagAll.Columns.Add("LotID", typeof(string));
            dtTagAll.Columns.Add("Description", typeof(string));
            dtTagAll.CaseSensitive = true;

            dtTagPresent.Columns.Add("TagUID", typeof(string));
            dtTagPresent.Columns.Add("LotID", typeof(string));
            dtTagPresent.Columns.Add("Description", typeof(string));
            dtTagPresent.CaseSensitive = true;

            dtTagAdded.Columns.Add("TagUID", typeof(string));
            dtTagAdded.Columns.Add("LotID", typeof(string));
            dtTagAdded.Columns.Add("Description", typeof(string));
            dtTagAdded.CaseSensitive = true;

            dtTagRemove.Columns.Add("TagUID", typeof(string));
            dtTagRemove.Columns.Add("LotID", typeof(string));
            dtTagRemove.Columns.Add("Description", typeof(string));
            dtTagRemove.CaseSensitive = true;
        }

        public InventoryData(Hashtable ColumnInfo)
        {    
            for (int i = 0; i < ColumnInfo.Count; i++)
            {
                dtTagAll.Columns.Add(ColumnInfo[i].ToString(), typeof(string));
                dtTagPresent.Columns.Add(ColumnInfo[i].ToString(), typeof(string));
                dtTagAdded.Columns.Add(ColumnInfo[i].ToString(), typeof(string));
                dtTagRemove.Columns.Add(ColumnInfo[i].ToString(), typeof(string));
            }
            dtTagAll.CaseSensitive = true;
            dtTagPresent.CaseSensitive = true;
            dtTagAdded.CaseSensitive = true;
            dtTagRemove.CaseSensitive = true;

        }       
    }

    public class UidWriteHistory
    {
        public int _idWrite;
        public string _initialUid;
        public string _writtenUid;
        public DateTime _writtenDate;
    }
    [Serializable]
    public class TagInfo
    {
        public string TagUID;
        public TagEventType eventType;
        public string Reference;
        public string[] productInfo = new string[VarGlobal.Nb_Max_Column_Product];       
    }
    [Serializable]
    public class StoredInventoryData
    {   
        public string serialNumberDevice = null;
        public int IdScanEvent = -1;
        public DateTime eventDate = DateTime.UtcNow;
        public bool bUserScan = false;
        public string userFirstName = null;
        public string userLastName = null;
        public DoorInfo userDoor = DoorInfo.DI_NO_DOOR;
        public string BadgeID = null;
        public AccessType accessType = AccessType.AT_NONE;
        public int nbTagAll = 0;
        public int nbTagPresent = 0;
        public int nbTagAdded = 0;
        public int nbTagRemoved = 0;
        public TagInfo[] TagArray;
        public Hashtable ListTagWithChannel;
        public string spareData1 = null;
        public string spareData2 = null;
    }


    [Serializable]
    public class ProductClassTemplate
    {
        public string tagUID;
        public string reference;
        public string[] productInfo = new string[VarGlobal.Nb_Max_Column_Product];        
    }

    [Serializable]
    public class TagEventClass
    {
        public string tagUID;
        public string serialRFID;
        public string DeviceName;       
        public string FirstName;
        public string LastName;
        public string eventDate;
        public string eventSortedDate;
        public string tagEventType;
        public string ProductRef;
    }

    [Serializable]
    public class tempInfo
    {
        public Dictionary<int, double> tempArray;
        public Dictionary<int, double> tempChamber;
        public Dictionary<int, double> tempBottle;
        public DateTime CreationDate;
        public DateTime lastTempAcq;
        public int nbValueTemp;
        public double lastTempValue;
        public double mean;
        public double max;
        public double min;
        public double sumTemp;
        public bool DefrostActive;
        public bool WasInDefrost;

        public tempInfo()
        {
            tempChamber = new Dictionary<int, double>();
            tempBottle = new Dictionary<int, double>();
            tempArray = new Dictionary<int, double>();
            CreationDate = DateTime.UtcNow;  // PC Fridge en temp UTC
            nbValueTemp = 0;
            max = -99.0;
            min = 99.0;
            sumTemp = 0.0;
            DefrostActive = false;
            WasInDefrost = false;
        }
    }

    public class PtTemp
    {
        public string TempAcqDate;
        public double? TempChamber;
        public double? TempBottle;
        public int bFridgeOK;
    }
    [Serializable]
    public class DataFanemInfo
    {
        public enum doorStatus
        {
            Closed = 0x00,
            Opened = 0x01,
        }

        public enum AcPowerStatus
        {
            AC_OK = 0x00,
            AC_Fault = 0x01,
        }

        public enum OnOffStatus
        {
            Off = 0x00,
            On = 0x01,
        }

        public double T0 = 0.0;
        public double T1 = 0.0;
        public double T2 = 0.0;
        public double Tconsigne = 0.0;
        public double AlarmHigh = 0.0;
        public double AlarmLow = 0.0;
        public double MaxT = 0.0;
        public double MinT = 0.0;
        public doorStatus Door = doorStatus.Closed;
        public AcPowerStatus AcPower = AcPowerStatus.AC_OK;
        public OnOffStatus Refrigeration = OnOffStatus.Off;
        public OnOffStatus FlashDrive = OnOffStatus.Off;
        public OnOffStatus Defrost = OnOffStatus.Off;
        public int Year;
        public int Month;
        public int Day;
        public int Hour;
        public int Minute;
        public int Second;
        public string Model;
        public string Serial;

        public double GetT0 { get { return T0; } }
        public double GetT1 { get { return T1; } }
        public double GetT2 { get { return T2; } }
        public double GetTconsigne { get { return Tconsigne; } }
        public double GetAlarmHigh { get { return AlarmHigh; } }
        public double GetAlarmLow { get { return AlarmLow; } }
        public double GetMaxT { get { return MaxT; } }
        public double GetMinT { get { return MinT; } }
        public doorStatus GetDoor { get { return Door; } }
        public AcPowerStatus GetAcPower { get { return AcPower; } }
        public OnOffStatus GetRefrigeration { get { return Refrigeration; } }
        public OnOffStatus GetFlashDrive { get { return FlashDrive; } }
        public OnOffStatus GetDefrost { get { return Defrost; } }

        public DateTime GetDateTime
        {
            get
            {
                try
                {
                    return new DateTime(2000 + Year, Month, Day, Hour, Minute, Second);
                }
                catch
                {
                    return DateTime.Now;
                }

            }
        }

        public string GetModel { get { return Model; } }
        public string GetSerial { get { return Serial; } }
    }


   


    static public class ExcelInfo
    {
        private const string regKey_office2010 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\14.0\Outlook";
        private const string regKey_office2013 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\15.0\Outlook";
        private const string regValue = "Bitness";

        static public bool isExcel2010_64bits()
        {
            bool ret= false;

            string strBitness = Convert.ToString(Registry.GetValue(regKey_office2010, regValue, null));
            if (!string.IsNullOrEmpty(strBitness))
            {
                if (strBitness.Equals("x64"))
                    ret = true;
            }
            else
            {
                string strBitness2 = Convert.ToString(Registry.GetValue(regKey_office2013, regValue, null));
                if (!string.IsNullOrEmpty(strBitness2))
                {
                    if (strBitness2.Equals("x64"))
                        ret = true;
                }
            }
            return ret;
        }
    }
    static public class OSInfo
    {
        #region BITS
        /// <summary>
        /// Determines if the current application is 32 or 64-bit.
        /// </summary>
        static public int Bits
        {
            get
            {
                //return IntPtr.Size * 8;  // KB not work as if X86 is forced in compilateur this return 32 on a 64 bits

                if (Directory.Exists(@"C:\Program Files (x86)"))
                    return 64;
                else
                    return 32;
            }
        }
        #endregion BITS

        #region EDITION
        static private string s_Edition;
        /// <summary>
        /// Gets the edition of the operating system running on this computer.
        /// </summary>
        static public string Edition
        {
            get
            {
                if (s_Edition != null)
                    return s_Edition;  //***** RETURN *****//

                string edition = String.Empty;

                OperatingSystem osVersion = Environment.OSVersion;
                OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
                osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

                if (GetVersionEx(ref osVersionInfo))
                {
                    int majorVersion = osVersion.Version.Major;
                    int minorVersion = osVersion.Version.Minor;
                    byte productType = osVersionInfo.wProductType;
                    short suiteMask = osVersionInfo.wSuiteMask;

                    #region VERSION 4
                    if (majorVersion == 4)
                    {
                        if (productType == VER_NT_WORKSTATION)
                        {
                            // Windows NT 4.0 Workstation
                            edition = "Workstation";
                        }
                        else if (productType == VER_NT_SERVER)
                        {
                            if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                            {
                                // Windows NT 4.0 Server Enterprise
                                edition = "Enterprise Server";
                            }
                            else
                            {
                                // Windows NT 4.0 Server
                                edition = "Standard Server";
                            }
                        }
                    }
                    #endregion VERSION 4

                    #region VERSION 5
                    else if (majorVersion == 5)
                    {
                        if (productType == VER_NT_WORKSTATION)
                        {
                            if ((suiteMask & VER_SUITE_PERSONAL) != 0)
                            {
                                // Windows XP Home Edition
                                edition = "Home";
                            }
                            else
                            {
                                // Windows XP / Windows 2000 Professional
                                edition = "Professional";
                            }
                        }
                        else if (productType == VER_NT_SERVER)
                        {
                            if (minorVersion == 0)
                            {
                                if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                                {
                                    // Windows 2000 Datacenter Server
                                    edition = "Datacenter Server";
                                }
                                else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                                {
                                    // Windows 2000 Advanced Server
                                    edition = "Advanced Server";
                                }
                                else
                                {
                                    // Windows 2000 Server
                                    edition = "Server";
                                }
                            }
                            else
                            {
                                if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                                {
                                    // Windows Server 2003 Datacenter Edition
                                    edition = "Datacenter";
                                }
                                else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                                {
                                    // Windows Server 2003 Enterprise Edition
                                    edition = "Enterprise";
                                }
                                else if ((suiteMask & VER_SUITE_BLADE) != 0)
                                {
                                    // Windows Server 2003 Web Edition
                                    edition = "Web Edition";
                                }
                                else
                                {
                                    // Windows Server 2003 Standard Edition
                                    edition = "Standard";
                                }
                            }
                        }
                    }
                    #endregion VERSION 5

                    #region VERSION 6
                    else if (majorVersion == 6)
                    {
                        int ed;
                        if (GetProductInfo(majorVersion, minorVersion,
                            osVersionInfo.wServicePackMajor, osVersionInfo.wServicePackMinor,
                            out ed))
                        {
                            switch (ed)
                            {
                                case PRODUCT_BUSINESS:
                                    edition = "Business";
                                    break;
                                case PRODUCT_BUSINESS_N:
                                    edition = "Business N";
                                    break;
                                case PRODUCT_CLUSTER_SERVER:
                                    edition = "HPC Edition";
                                    break;
                                case PRODUCT_CLUSTER_SERVER_V:
                                    edition = "HPC Edition without Hyper-V";
                                    break;
                                case PRODUCT_DATACENTER_SERVER:
                                    edition = "Datacenter Server";
                                    break;
                                case PRODUCT_DATACENTER_SERVER_CORE:
                                    edition = "Datacenter Server (core installation)";
                                    break;
                                case PRODUCT_DATACENTER_SERVER_V:
                                    edition = "Datacenter Server without Hyper-V";
                                    break;
                                case PRODUCT_DATACENTER_SERVER_CORE_V:
                                    edition = "Datacenter Server without Hyper-V (core installation)";
                                    break;
                                case PRODUCT_EMBEDDED:
                                    edition = "Embedded";
                                    break;
                                case PRODUCT_ENTERPRISE:
                                    edition = "Enterprise";
                                    break;
                                case PRODUCT_ENTERPRISE_N:
                                    edition = "Enterprise N";
                                    break;
                                case PRODUCT_ENTERPRISE_E:
                                    edition = "Enterprise E";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER:
                                    edition = "Enterprise Server";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_CORE:
                                    edition = "Enterprise Server (core installation)";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_CORE_V:
                                    edition = "Enterprise Server without Hyper-V (core installation)";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_IA64:
                                    edition = "Enterprise Server for Itanium-based Systems";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_V:
                                    edition = "Enterprise Server without Hyper-V";
                                    break;
                                case PRODUCT_ESSENTIALBUSINESS_SERVER_MGMT:
                                    edition = "Essential Business Server MGMT";
                                    break;
                                case PRODUCT_ESSENTIALBUSINESS_SERVER_ADDL:
                                    edition = "Essential Business Server ADDL";
                                    break;
                                case PRODUCT_ESSENTIALBUSINESS_SERVER_MGMTSVC:
                                    edition = "Essential Business Server MGMTSVC";
                                    break;
                                case PRODUCT_ESSENTIALBUSINESS_SERVER_ADDLSVC:
                                    edition = "Essential Business Server ADDLSVC";
                                    break;
                                case PRODUCT_HOME_BASIC:
                                    edition = "Home Basic";
                                    break;
                                case PRODUCT_HOME_BASIC_N:
                                    edition = "Home Basic N";
                                    break;
                                case PRODUCT_HOME_BASIC_E:
                                    edition = "Home Basic E";
                                    break;
                                case PRODUCT_HOME_PREMIUM:
                                    edition = "Home Premium";
                                    break;
                                case PRODUCT_HOME_PREMIUM_N:
                                    edition = "Home Premium N";
                                    break;
                                case PRODUCT_HOME_PREMIUM_E:
                                    edition = "Home Premium E";
                                    break;
                                case PRODUCT_HOME_PREMIUM_SERVER:
                                    edition = "Home Premium Server";
                                    break;
                                case PRODUCT_HYPERV:
                                    edition = "Microsoft Hyper-V Server";
                                    break;
                                case PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT:
                                    edition = "Windows Essential Business Management Server";
                                    break;
                                case PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING:
                                    edition = "Windows Essential Business Messaging Server";
                                    break;
                                case PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY:
                                    edition = "Windows Essential Business Security Server";
                                    break;
                                case PRODUCT_PROFESSIONAL:
                                    edition = "Professional";
                                    break;
                                case PRODUCT_PROFESSIONAL_N:
                                    edition = "Professional N";
                                    break;
                                case PRODUCT_PROFESSIONAL_E:
                                    edition = "Professional E";
                                    break;
                                case PRODUCT_SB_SOLUTION_SERVER:
                                    edition = "SB Solution Server";
                                    break;
                                case PRODUCT_SB_SOLUTION_SERVER_EM:
                                    edition = "SB Solution Server EM";
                                    break;
                                case PRODUCT_SERVER_FOR_SB_SOLUTIONS:
                                    edition = "Server for SB Solutions";
                                    break;
                                case PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM:
                                    edition = "Server for SB Solutions EM";
                                    break;
                                case PRODUCT_SERVER_FOR_SMALLBUSINESS:
                                    edition = "Windows Essential Server Solutions";
                                    break;
                                case PRODUCT_SERVER_FOR_SMALLBUSINESS_V:
                                    edition = "Windows Essential Server Solutions without Hyper-V";
                                    break;
                                case PRODUCT_SERVER_FOUNDATION:
                                    edition = "Server Foundation";
                                    break;
                                case PRODUCT_SMALLBUSINESS_SERVER:
                                    edition = "Windows Small Business Server";
                                    break;
                                case PRODUCT_SMALLBUSINESS_SERVER_PREMIUM:
                                    edition = "Windows Small Business Server Premium";
                                    break;
                                case PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE:
                                    edition = "Windows Small Business Server Premium (core installation)";
                                    break;
                                case PRODUCT_SOLUTION_EMBEDDEDSERVER:
                                    edition = "Solution Embedded Server";
                                    break;
                                case PRODUCT_SOLUTION_EMBEDDEDSERVER_CORE:
                                    edition = "Solution Embedded Server (core installation)";
                                    break;
                                case PRODUCT_STANDARD_SERVER:
                                    edition = "Standard Server";
                                    break;
                                case PRODUCT_STANDARD_SERVER_CORE:
                                    edition = "Standard Server (core installation)";
                                    break;
                                case PRODUCT_STANDARD_SERVER_SOLUTIONS:
                                    edition = "Standard Server Solutions";
                                    break;
                                case PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE:
                                    edition = "Standard Server Solutions (core installation)";
                                    break;
                                case PRODUCT_STANDARD_SERVER_CORE_V:
                                    edition = "Standard Server without Hyper-V (core installation)";
                                    break;
                                case PRODUCT_STANDARD_SERVER_V:
                                    edition = "Standard Server without Hyper-V";
                                    break;
                                case PRODUCT_STARTER:
                                    edition = "Starter";
                                    break;
                                case PRODUCT_STARTER_N:
                                    edition = "Starter N";
                                    break;
                                case PRODUCT_STARTER_E:
                                    edition = "Starter E";
                                    break;
                                case PRODUCT_STORAGE_ENTERPRISE_SERVER:
                                    edition = "Enterprise Storage Server";
                                    break;
                                case PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE:
                                    edition = "Enterprise Storage Server (core installation)";
                                    break;
                                case PRODUCT_STORAGE_EXPRESS_SERVER:
                                    edition = "Express Storage Server";
                                    break;
                                case PRODUCT_STORAGE_EXPRESS_SERVER_CORE:
                                    edition = "Express Storage Server (core installation)";
                                    break;
                                case PRODUCT_STORAGE_STANDARD_SERVER:
                                    edition = "Standard Storage Server";
                                    break;
                                case PRODUCT_STORAGE_STANDARD_SERVER_CORE:
                                    edition = "Standard Storage Server (core installation)";
                                    break;
                                case PRODUCT_STORAGE_WORKGROUP_SERVER:
                                    edition = "Workgroup Storage Server";
                                    break;
                                case PRODUCT_STORAGE_WORKGROUP_SERVER_CORE:
                                    edition = "Workgroup Storage Server (core installation)";
                                    break;
                                case PRODUCT_UNDEFINED:
                                    edition = "Unknown product";
                                    break;
                                case PRODUCT_ULTIMATE:
                                    edition = "Ultimate";
                                    break;
                                case PRODUCT_ULTIMATE_N:
                                    edition = "Ultimate N";
                                    break;
                                case PRODUCT_ULTIMATE_E:
                                    edition = "Ultimate E";
                                    break;
                                case PRODUCT_WEB_SERVER:
                                    edition = "Web Server";
                                    break;
                                case PRODUCT_WEB_SERVER_CORE:
                                    edition = "Web Server (core installation)";
                                    break;
                            }
                        }
                    }
                    #endregion VERSION 6
                }

                s_Edition = edition;
                return edition;
            }
        }
        #endregion EDITION

        #region NAME
        static private string s_Name;
        /// <summary>
        /// Gets the name of the operating system running on this computer.
        /// </summary>
        static public string Name
        {
            get
            {
                if (s_Name != null)
                    return s_Name;  //***** RETURN *****//

                string name = "unknown";

                OperatingSystem osVersion = Environment.OSVersion;
                OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
                osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

                if (GetVersionEx(ref osVersionInfo))
                {
                    int majorVersion = osVersion.Version.Major;
                    int minorVersion = osVersion.Version.Minor;

                    switch (osVersion.Platform)
                    {
                        case PlatformID.Win32S:
                            name = "Windows 3.1";
                            break;
                        case PlatformID.WinCE:
                            name = "Windows CE";
                            break;
                        case PlatformID.Win32Windows:
                            {
                                if (majorVersion == 4)
                                {
                                    string csdVersion = osVersionInfo.szCSDVersion;
                                    switch (minorVersion)
                                    {
                                        case 0:
                                            if (csdVersion == "B" || csdVersion == "C")
                                                name = "Windows 95 OSR2";
                                            else
                                                name = "Windows 95";
                                            break;
                                        case 10:
                                            if (csdVersion == "A")
                                                name = "Windows 98 Second Edition";
                                            else
                                                name = "Windows 98";
                                            break;
                                        case 90:
                                            name = "Windows Me";
                                            break;
                                    }
                                }
                                break;
                            }

                        case PlatformID.Win32NT:
                            {
                                byte productType = osVersionInfo.wProductType;

                                switch (majorVersion)
                                {
                                    case 3:
                                        name = "Windows NT 3.51";
                                        break;
                                    case 4:
                                        switch (productType)
                                        {
                                            case 1:
                                                name = "Windows NT 4.0";
                                                break;
                                            case 3:
                                                name = "Windows NT 4.0 Server";
                                                break;
                                        }
                                        break;
                                    case 5:
                                        switch (minorVersion)
                                        {
                                            case 0:
                                                name = "Windows 2000";
                                                break;
                                            case 1:
                                                name = "Windows XP";
                                                break;
                                            case 2:
                                                name = "Windows Server 2003";
                                                break;
                                        }
                                        break;
                                    case 6:
                                        switch (minorVersion)
                                        {
                                            case 0:
                                                switch (productType)
                                                {
                                                    case 1:
                                                        name = "Windows Vista";
                                                        break;
                                                    case 3:
                                                        name = "Windows Server 2008";
                                                        break;
                                                }
                                                break;

                                            case 1:
                                                switch (productType)
                                                {
                                                    case 1:
                                                        name = "Windows 7";
                                                        break;
                                                    case 3:
                                                        name = "Windows Server 2008 R2";
                                                        break;
                                                }
                                                break;
                                            case 2:
                                                switch (productType)
                                                {
                                                    case 1:
                                                        name = "Windows 8";
                                                        break;
                                                    case 3:
                                                        name = "Windows Server 2012";
                                                        break;
                                                }
                                                break;
                                        }
                                        break;
                                }
                                break;
                            }
                    }
                }

                s_Name = name;
                return name;
            }
        }
        #endregion NAME

        #region PINVOKE
        #region GET
        #region PRODUCT INFO
        [DllImport("Kernel32.dll")]
        internal static extern bool GetProductInfo(
            int osMajorVersion,
            int osMinorVersion,
            int spMajorVersion,
            int spMinorVersion,
            out int edition);
        #endregion PRODUCT INFO

        #region VERSION
        [DllImport("kernel32.dll")]
        private static extern bool GetVersionEx(ref OSVERSIONINFOEX osVersionInfo);
        #endregion VERSION
        #endregion GET

        #region OSVERSIONINFOEX
        [StructLayout(LayoutKind.Sequential)]
        private struct OSVERSIONINFOEX
        {
            public int dwOSVersionInfoSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
            public short wServicePackMajor;
            public short wServicePackMinor;
            public short wSuiteMask;
            public byte wProductType;
            public byte wReserved;
        }
        #endregion OSVERSIONINFOEX

        #region PRODUCT
        private const int PRODUCT_UNDEFINED = 0x00000000;
        private const int PRODUCT_ULTIMATE = 0x00000001;
        private const int PRODUCT_HOME_BASIC = 0x00000002;
        private const int PRODUCT_HOME_PREMIUM = 0x00000003;
        private const int PRODUCT_ENTERPRISE = 0x00000004;
        private const int PRODUCT_HOME_BASIC_N = 0x00000005;
        private const int PRODUCT_BUSINESS = 0x00000006;
        private const int PRODUCT_STANDARD_SERVER = 0x00000007;
        private const int PRODUCT_DATACENTER_SERVER = 0x00000008;
        private const int PRODUCT_SMALLBUSINESS_SERVER = 0x00000009;
        private const int PRODUCT_ENTERPRISE_SERVER = 0x0000000A;
        private const int PRODUCT_STARTER = 0x0000000B;
        private const int PRODUCT_DATACENTER_SERVER_CORE = 0x0000000C;
        private const int PRODUCT_STANDARD_SERVER_CORE = 0x0000000D;
        private const int PRODUCT_ENTERPRISE_SERVER_CORE = 0x0000000E;
        private const int PRODUCT_ENTERPRISE_SERVER_IA64 = 0x0000000F;
        private const int PRODUCT_BUSINESS_N = 0x00000010;
        private const int PRODUCT_WEB_SERVER = 0x00000011;
        private const int PRODUCT_CLUSTER_SERVER = 0x00000012;
        private const int PRODUCT_HOME_SERVER = 0x00000013;
        private const int PRODUCT_STORAGE_EXPRESS_SERVER = 0x00000014;
        private const int PRODUCT_STORAGE_STANDARD_SERVER = 0x00000015;
        private const int PRODUCT_STORAGE_WORKGROUP_SERVER = 0x00000016;
        private const int PRODUCT_STORAGE_ENTERPRISE_SERVER = 0x00000017;
        private const int PRODUCT_SERVER_FOR_SMALLBUSINESS = 0x00000018;
        private const int PRODUCT_SMALLBUSINESS_SERVER_PREMIUM = 0x00000019;
        private const int PRODUCT_HOME_PREMIUM_N = 0x0000001A;
        private const int PRODUCT_ENTERPRISE_N = 0x0000001B;
        private const int PRODUCT_ULTIMATE_N = 0x0000001C;
        private const int PRODUCT_WEB_SERVER_CORE = 0x0000001D;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT = 0x0000001E;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY = 0x0000001F;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING = 0x00000020;
        private const int PRODUCT_SERVER_FOUNDATION = 0x00000021;
        private const int PRODUCT_HOME_PREMIUM_SERVER = 0x00000022;
        private const int PRODUCT_SERVER_FOR_SMALLBUSINESS_V = 0x00000023;
        private const int PRODUCT_STANDARD_SERVER_V = 0x00000024;
        private const int PRODUCT_DATACENTER_SERVER_V = 0x00000025;
        private const int PRODUCT_ENTERPRISE_SERVER_V = 0x00000026;
        private const int PRODUCT_DATACENTER_SERVER_CORE_V = 0x00000027;
        private const int PRODUCT_STANDARD_SERVER_CORE_V = 0x00000028;
        private const int PRODUCT_ENTERPRISE_SERVER_CORE_V = 0x00000029;
        private const int PRODUCT_HYPERV = 0x0000002A;
        private const int PRODUCT_STORAGE_EXPRESS_SERVER_CORE = 0x0000002B;
        private const int PRODUCT_STORAGE_STANDARD_SERVER_CORE = 0x0000002C;
        private const int PRODUCT_STORAGE_WORKGROUP_SERVER_CORE = 0x0000002D;
        private const int PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE = 0x0000002E;
        private const int PRODUCT_STARTER_N = 0x0000002F;
        private const int PRODUCT_PROFESSIONAL = 0x00000030;
        private const int PRODUCT_PROFESSIONAL_N = 0x00000031;
        private const int PRODUCT_SB_SOLUTION_SERVER = 0x00000032;
        private const int PRODUCT_SERVER_FOR_SB_SOLUTIONS = 0x00000033;
        private const int PRODUCT_STANDARD_SERVER_SOLUTIONS = 0x00000034;
        private const int PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE = 0x00000035;
        private const int PRODUCT_SB_SOLUTION_SERVER_EM = 0x00000036;
        private const int PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM = 0x00000037;
        private const int PRODUCT_SOLUTION_EMBEDDEDSERVER = 0x00000038;
        private const int PRODUCT_SOLUTION_EMBEDDEDSERVER_CORE = 0x00000039;
        //private const int ???? = 0x0000003A;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_MGMT = 0x0000003B;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_ADDL = 0x0000003C;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_MGMTSVC = 0x0000003D;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_ADDLSVC = 0x0000003E;
        private const int PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE = 0x0000003F;
        private const int PRODUCT_CLUSTER_SERVER_V = 0x00000040;
        private const int PRODUCT_EMBEDDED = 0x00000041;
        private const int PRODUCT_STARTER_E = 0x00000042;
        private const int PRODUCT_HOME_BASIC_E = 0x00000043;
        private const int PRODUCT_HOME_PREMIUM_E = 0x00000044;
        private const int PRODUCT_PROFESSIONAL_E = 0x00000045;
        private const int PRODUCT_ENTERPRISE_E = 0x00000046;
        private const int PRODUCT_ULTIMATE_E = 0x00000047;
        //private const int PRODUCT_UNLICENSED = 0xABCDABCD;
        #endregion PRODUCT

        #region VERSIONS
        private const int VER_NT_WORKSTATION = 1;
        private const int VER_NT_DOMAIN_CONTROLLER = 2;
        private const int VER_NT_SERVER = 3;
        private const int VER_SUITE_SMALLBUSINESS = 1;
        private const int VER_SUITE_ENTERPRISE = 2;
        private const int VER_SUITE_TERMINAL = 16;
        private const int VER_SUITE_DATACENTER = 128;
        private const int VER_SUITE_SINGLEUSERTS = 256;
        private const int VER_SUITE_PERSONAL = 512;
        private const int VER_SUITE_BLADE = 1024;
        #endregion VERSIONS
        #endregion PINVOKE

        #region SERVICE PACK
        /// <summary>
        /// Gets the service pack information of the operating system running on this computer.
        /// </summary>
        static public string ServicePack
        {
            get
            {
                string servicePack = String.Empty;
                OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();

                osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

                if (GetVersionEx(ref osVersionInfo))
                {
                    servicePack = osVersionInfo.szCSDVersion;
                }

                return servicePack;
            }
        }
        #endregion SERVICE PACK

        #region VERSION
        #region BUILD
        /// <summary>
        /// Gets the build version number of the operating system running on this computer.
        /// </summary>
        static public int BuildVersion
        {
            get
            {
                return Environment.OSVersion.Version.Build;
            }
        }
        #endregion BUILD

        #region FULL
        #region STRING
        /// <summary>
        /// Gets the full version string of the operating system running on this computer.
        /// </summary>
        static public string VersionString
        {
            get
            {
                return Environment.OSVersion.Version.ToString();
            }
        }
        #endregion STRING

        #region VERSION
        /// <summary>
        /// Gets the full version of the operating system running on this computer.
        /// </summary>
        static public Version Version
        {
            get
            {
                return Environment.OSVersion.Version;
            }
        }
        #endregion VERSION
        #endregion FULL

        #region MAJOR
        /// <summary>
        /// Gets the major version number of the operating system running on this computer.
        /// </summary>
        static public int MajorVersion
        {
            get
            {
                return Environment.OSVersion.Version.Major;
            }
        }
        #endregion MAJOR

        #region MINOR
        /// <summary>
        /// Gets the minor version number of the operating system running on this computer.
        /// </summary>
        static public int MinorVersion
        {
            get
            {
                return Environment.OSVersion.Version.Minor;
            }
        }
        #endregion MINOR

        #region REVISION
        /// <summary>
        /// Gets the revision version number of the operating system running on this computer.
        /// </summary>
        static public int RevisionVersion
        {
            get
            {
                return Environment.OSVersion.Version.Revision;
            }
        }
        #endregion REVISION
        #endregion VERSION
    }

     #region utile
    static public class ConvertInventory    {

        public static StoredInventoryData ConvertForStore(InventoryData invData)
        {
            if (invData == null) return null;
            StoredInventoryData storeData = new StoredInventoryData();
            try
            {
                int nIndex = 0;

                storeData.IdScanEvent = invData.IdScanEvent;
                storeData.bUserScan = invData.bUserScan;
                storeData.eventDate = invData.eventDate;
                storeData.nbTagAdded = invData.nbTagAdded;
                storeData.nbTagAll = invData.nbTagAll;
                storeData.nbTagPresent = invData.nbTagPresent;
                storeData.nbTagRemoved = invData.nbTagRemoved;
                storeData.serialNumberDevice = invData.serialNumberDevice;
                storeData.userFirstName = invData.userFirstName;
                storeData.userLastName = invData.userLastName;
                storeData.BadgeID = invData.BadgeID;
                storeData.userDoor = invData.userDoor;
                storeData.accessType = invData.accessType;
                storeData.ListTagWithChannel = invData.ListTagWithChannel;
                storeData.spareData1 = invData.spareData1;
                storeData.spareData2 = invData.spareData2;

                int nbTostore = invData.nbTagAdded + invData.nbTagPresent + invData.nbTagRemoved;
               

                if (nbTostore != 0)
                {

                    storeData.TagArray = new TagInfo[nbTostore];

                    for (int loop = 0; loop < nbTostore; loop++) storeData.TagArray[loop] = new TagInfo();

                    foreach (string uidToAdd in invData.listTagAdded)
                    {
                        storeData.TagArray[nIndex].TagUID = uidToAdd;
                        storeData.TagArray[nIndex].eventType = TagEventType.TET_Added;
                        storeData.TagArray[nIndex].Reference = "Unreferenced";
                        nIndex++;
                    }

                    foreach (string uidPresent in invData.listTagPresent)
                    {

                        storeData.TagArray[nIndex].TagUID = uidPresent;
                        storeData.TagArray[nIndex].eventType = TagEventType.TET_Present;
                        storeData.TagArray[nIndex].Reference = "Unreferenced";
                        nIndex++;
                    }

                    foreach (string uidRemove in invData.listTagRemoved)
                    {
                        storeData.TagArray[nIndex].TagUID = uidRemove;
                        storeData.TagArray[nIndex].eventType = TagEventType.TET_Removed;
                        storeData.TagArray[nIndex].Reference = "Unreferenced";
                        nIndex++;
                    }
                }

            }
            catch (Exception exp)
            {
                ErrorMessage.ExceptionMessageBox.Show(exp);
                throw exp;
            }
            return storeData;

        }
        public static StoredInventoryData ConvertForStore(InventoryData invData, Hashtable ColumnInfo)
        {
          
                StoredInventoryData storeData = new StoredInventoryData();
                try
                {
                    int nIndex = 0;

                    storeData.IdScanEvent = invData.IdScanEvent;
                    storeData.bUserScan = invData.bUserScan;
                    storeData.eventDate = invData.eventDate;
                    storeData.nbTagAdded = invData.nbTagAdded;
                    storeData.nbTagAll = invData.nbTagAll;
                    storeData.nbTagPresent = invData.nbTagPresent;
                    storeData.nbTagRemoved = invData.nbTagRemoved;
                    storeData.serialNumberDevice = invData.serialNumberDevice;
                    storeData.userFirstName = invData.userFirstName;
                    storeData.userLastName = invData.userLastName;
                    storeData.BadgeID = invData.BadgeID;
                    storeData.userDoor = invData.userDoor;
                    storeData.accessType = invData.accessType;
                    storeData.ListTagWithChannel = invData.ListTagWithChannel;
                    storeData.spareData1 = invData.spareData1;
                    storeData.spareData2 = invData.spareData2;

                    int nbTostore = invData.nbTagAdded + invData.nbTagPresent + invData.nbTagRemoved;
                    int checkNbToStore = invData.dtTagAdded.Rows.Count + invData.dtTagPresent.Rows.Count + invData.dtTagRemove.Rows.Count;

                    /*if (nbTostore != checkNbToStore)
                    {                       
                        return null;
                    }*/

                    if (nbTostore != 0)
                    {

                        storeData.TagArray = new TagInfo[nbTostore];

                        for (int loop = 0; loop < nbTostore; loop++) storeData.TagArray[loop] = new TagInfo();

                        foreach (DataRow dr in invData.dtTagAdded.Rows)
                        {
                            storeData.TagArray[nIndex].TagUID = dr[ColumnInfo[0].ToString()].ToString();
                            storeData.TagArray[nIndex].eventType = TagEventType.TET_Added;
                            if (dr[ColumnInfo[1].ToString()].ToString().Contains("Unreferenced"))
                            {
                                storeData.TagArray[nIndex].Reference = dr[ColumnInfo[1].ToString()].ToString();
                                for (int i = 0; i < ColumnInfo.Count - 2; i++)
                                    storeData.TagArray[nIndex].productInfo[i] = string.Empty;
                            }
                            else
                            {
                                storeData.TagArray[nIndex].Reference = dr[ColumnInfo[1].ToString()].ToString();
                                for (int i = 0; i < ColumnInfo.Count - 2; i++)
                                    storeData.TagArray[nIndex].productInfo[i] = dr[ColumnInfo[i + 2].ToString()].ToString();
                            }
                            nIndex++;
                        }

                        foreach (DataRow dr in invData.dtTagPresent.Rows)
                        {
                            string val = ColumnInfo[0].ToString();
                            storeData.TagArray[nIndex].TagUID = dr[ColumnInfo[0].ToString()].ToString();


                            storeData.TagArray[nIndex].eventType = TagEventType.TET_Present;
                            if (dr[ColumnInfo[1].ToString()].ToString().Contains("Unreferenced"))
                            {
                                storeData.TagArray[nIndex].Reference = dr[ColumnInfo[1].ToString()].ToString();
                                for (int i = 0; i < ColumnInfo.Count - 2; i++)
                                    storeData.TagArray[nIndex].productInfo[i] = string.Empty;
                            }
                            else
                            {
                                storeData.TagArray[nIndex].Reference = dr[ColumnInfo[1].ToString()].ToString();
                                for (int i = 0; i < ColumnInfo.Count - 2; i++)
                                    storeData.TagArray[nIndex].productInfo[i] = dr[ColumnInfo[i + 2].ToString()].ToString();
                            }
                            nIndex++;
                        }

                        foreach (DataRow dr in invData.dtTagRemove.Rows)
                        {
                            storeData.TagArray[nIndex].TagUID = dr[ColumnInfo[0].ToString()].ToString();
                            storeData.TagArray[nIndex].eventType = TagEventType.TET_Removed;
                            if (dr[ColumnInfo[1].ToString()].ToString().Contains("Unreferenced"))
                            {
                                storeData.TagArray[nIndex].Reference = dr[ColumnInfo[1].ToString()].ToString();
                                for (int i = 0; i < ColumnInfo.Count - 2; i++)
                                    storeData.TagArray[nIndex].productInfo[i] = string.Empty;
                            }
                            else
                            {
                                storeData.TagArray[nIndex].Reference = dr[ColumnInfo[1].ToString()].ToString();
                                for (int i = 0; i < ColumnInfo.Count - 2; i++)
                                    storeData.TagArray[nIndex].productInfo[i] = dr[ColumnInfo[i + 2].ToString()].ToString();
                            }
                            nIndex++;
                        }
                    }

                }
                catch (Exception exp)
                {
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                    throw exp;
                }
                return storeData;
            
        }
     
        public static InventoryData ConvertForUse(StoredInventoryData storeData, Hashtable ColumnInfo)
        {

            
                InventoryData invData;
                if (ColumnInfo != null)
                    invData = new InventoryData(ColumnInfo);
                else
                    invData = new InventoryData();

                try
                {
                    invData.IdScanEvent = storeData.IdScanEvent;
                    invData.bUserScan = storeData.bUserScan;
                    invData.eventDate = storeData.eventDate;
                    invData.nbTagAdded = storeData.nbTagAdded;
                    invData.nbTagAll = storeData.nbTagAll;
                    invData.nbTagPresent = storeData.nbTagPresent;
                    invData.nbTagRemoved = storeData.nbTagRemoved;
                    invData.serialNumberDevice = storeData.serialNumberDevice;
                    invData.userFirstName = storeData.userFirstName;
                    invData.userLastName = storeData.userLastName;
                    invData.BadgeID = storeData.BadgeID;
                    invData.userDoor = storeData.userDoor;
                    invData.accessType = storeData.accessType;
                    invData.ListTagWithChannel = storeData.ListTagWithChannel;
                    invData.spareData1 = storeData.spareData1;
                    invData.spareData2 = storeData.spareData2;

                    if (storeData.TagArray == null)
                        return invData;

                    foreach (TagInfo ti in storeData.TagArray)
                    {

                        switch (ti.eventType)
                        {
                            case TagEventType.TET_Added:

                                invData.listTagAdded.Add(ti.TagUID);
                                invData.listTagAll.Add(ti.TagUID);
                                if (ColumnInfo == null) break;
                                if (string.IsNullOrEmpty(ti.Reference))
                                {
                                    //invData.dtTagAdded.Rows.Add(ti.TagUID, "Unreferenced", " ");
                                    //invData.dtTagAll.Rows.Add(ti.TagUID, "Unreferenced", " ");
                                    object[] param = new object[ColumnInfo.Count];
                                    param[0] = ti.TagUID;
                                    param[1] = "Unreferenced";
                                    for (int i = 2; i < ColumnInfo.Count; i++)
                                        param[i] = " ";
                                    invData.dtTagAdded.Rows.Add(param);
                                    invData.dtTagAll.Rows.Add(param);
                                }
                                else
                                {
                                    //invData.dtTagAdded.Rows.Add(ti.TagUID, ti.Reference, ti.Description);
                                    //invData.dtTagAll.Rows.Add(ti.TagUID, ti.Reference, ti.Description);

                                    object[] param = new object[ColumnInfo.Count];
                                    param[0] = ti.TagUID;
                                    param[1] = ti.Reference;
                                    if (ti.productInfo == null)
                                    {
                                        for (int i = 2; i < ColumnInfo.Count; i++)
                                            param[i] = " ";
                                    }
                                    else
                                    {
                                        for (int i = 2; i < ColumnInfo.Count; i++)
                                            param[i] = ti.productInfo[i - 2];
                                    }
                                    invData.dtTagAdded.Rows.Add(param);
                                    invData.dtTagAll.Rows.Add(param);
                                }

                                break;
                            case TagEventType.TET_Present:

                                invData.listTagPresent.Add(ti.TagUID);
                                invData.listTagAll.Add(ti.TagUID);
                                if (ColumnInfo == null) break;
                                if (string.IsNullOrEmpty(ti.Reference))
                                {
                                    //invData.dtTagPresent.Rows.Add(ti.TagUID, " Unreferenced ", " ");
                                    //invData.dtTagAll.Rows.Add(ti.TagUID, " Unreferenced ", " ");
                                    object[] param = new object[ColumnInfo.Count];
                                    param[0] = ti.TagUID;
                                    param[1] = "Unreferenced";
                                    for (int i = 2; i < ColumnInfo.Count; i++)
                                        param[i] = " ";
                                    invData.dtTagPresent.Rows.Add(param);
                                    invData.dtTagAll.Rows.Add(param);

                                }
                                else
                                {
                                    //invData.dtTagPresent.Rows.Add(ti.TagUID, ti.Reference, ti.Description);
                                    //invData.dtTagAll.Rows.Add(ti.TagUID, ti.Reference, ti.Description);


                                    object[] param = new object[ColumnInfo.Count];
                                    param[0] = ti.TagUID;
                                    param[1] = ti.Reference;
                                    if (ti.productInfo == null)
                                    {
                                        for (int i = 2; i < ColumnInfo.Count; i++)
                                            param[i] = " ";
                                    }
                                    else
                                    {
                                        for (int i = 2; i < ColumnInfo.Count; i++)
                                            param[i] = ti.productInfo[i - 2];
                                    }
                                    invData.dtTagPresent.Rows.Add(param);
                                    invData.dtTagAll.Rows.Add(param);
                                }

                                break;
                            case TagEventType.TET_Removed:

                                invData.listTagRemoved.Add(ti.TagUID);
                                if (ColumnInfo == null) break;
                                if (string.IsNullOrEmpty(ti.Reference))
                                {
                                    //invData.dtTagRemove.Rows.Add(ti.TagUID, " Unreferenced ", " ");
                                    object[] param = new object[ColumnInfo.Count];
                                    param[0] = ti.TagUID;
                                    param[1] = "Unreferenced";
                                    for (int i = 2; i < ColumnInfo.Count; i++)
                                        param[i] = " ";
                                    invData.dtTagRemove.Rows.Add(param);
                                }
                                else
                                {
                                    //invData.dtTagRemove.Rows.Add(ti.TagUID, ti.Reference, ti.Description);

                                    object[] param = new object[ColumnInfo.Count];
                                    param[0] = ti.TagUID;
                                    param[1] = ti.Reference;
                                    if (ti.productInfo == null)
                                    {
                                        for (int i = 2; i < ColumnInfo.Count; i++)
                                            param[i] = " ";
                                    }
                                    else
                                    {
                                        for (int i = 2; i < ColumnInfo.Count; i++)
                                            param[i] = ti.productInfo[i - 2];
                                    }
                                    invData.dtTagRemove.Rows.Add(param);
                                }

                                break;
                        }

                    }
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    //throw new Exception();
                    ExceptionMessageBox.Show(exp);
                }
                return invData;
           
        }
    }

    static public class deviceUtils
    {
        public static bool  getDeviceInfo(string serialNumber, out DeviceInfo dev)
        {
            string pathXML = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "deviceInfo.xml";
            return readDeviceFromXml(pathXML, serialNumber, out dev);
        }

        static bool getDeviceFromServer(string serialNumber, out string strXML)
        {
            strXML = null;
            bool ret = false;
            try
            {               
                
                string url = "http://www.spacecode.com/deviceInfo/getDeviceInfo.php?serial=" + serialNumber;
                
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();

                if (request.HaveResponse == true)
                {
                    Stream responseStream = webResponse.GetResponseStream();
                    StreamReader responseReader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
                    strXML = responseReader.ReadToEnd();
                    ret = true;
                }
                return ret;
            }
            catch
            {
                return false;
            }
        }

        static void saveDeviceToXml(string path, DeviceInfo dev)
        {
            System.Xml.XmlDocument pDoc = new System.Xml.XmlDocument();

            if (File.Exists(path))
            {
                pDoc.Load(path);

                XmlElement root = pDoc.DocumentElement;

                XmlElement device = pDoc.CreateElement("Device");
                device.SetAttribute("serialNumber", dev.SerialRFID);

                XmlElement deviceType = pDoc.CreateElement("DeviceType");
                deviceType.InnerText = dev.deviceType.ToString();

                XmlElement fpMaster = pDoc.CreateElement("SerialFPMaster");
                fpMaster.InnerText = dev.SerialFPMaster;

                XmlElement fpSlave = pDoc.CreateElement("SerialFPSlave");
                fpSlave.InnerText = dev.SerialFPSlave;

                device.AppendChild(deviceType);
                device.AppendChild(fpMaster);
                device.AppendChild(fpSlave);
                root.AppendChild(device);

                pDoc.Save(path);

            }
            else
            {
                XmlDeclaration dec = pDoc.CreateXmlDeclaration("1.0", null, null);
                pDoc.AppendChild(dec);

                XmlElement root = pDoc.CreateElement("Devices");                
                pDoc.AppendChild(root);

                XmlElement device = pDoc.CreateElement("Device");
                device.SetAttribute("serialNumber", dev.SerialRFID);

                XmlElement deviceType = pDoc.CreateElement("DeviceType");
                deviceType.InnerText = dev.deviceType.ToString();

                XmlElement fpMaster = pDoc.CreateElement("SerialFPMaster");
                fpMaster.InnerText = dev.SerialFPMaster;

                XmlElement fpSlave = pDoc.CreateElement("SerialFPSlave");
                fpSlave.InnerText = dev.SerialFPSlave;

                device.AppendChild(deviceType);
                device.AppendChild(fpMaster);
                device.AppendChild(fpSlave);
                root.AppendChild(device);

                pDoc.Save(path);

            }   
        }

        static bool readDeviceFromXml(string path, string serialNumber, out DeviceInfo dev)
        {
            dev = null;

            if (!File.Exists(path)) return false;
            
            XmlDocument pDoc = new XmlDocument();
            pDoc.Load(path);
            XmlElement root = pDoc.DocumentElement;

            if (root == null) return false;
            XmlNode device = root.SelectSingleNode("Device[@serialNumber='" + serialNumber + "']");

            if (device == null) return false;
            
            dev = new DeviceInfo();
            dev.SerialRFID = serialNumber;

            XmlNode deviceTypeNode = device.SelectSingleNode("DeviceType");
            XmlNode serialFPMasterNode = device.SelectSingleNode("SerialFPMaster");
            XmlNode serialFPSlaveNode = device.SelectSingleNode("SerialFPSlave");
            XmlNode badgeUsbSerial = device.SelectSingleNode("BadgeUsbSerial");

            if (deviceTypeNode == null || serialFPMasterNode == null || serialFPSlaveNode == null ||
                badgeUsbSerial == null)
                return false;

            dev.deviceType = (DeviceType)Enum.Parse(typeof(DeviceType), deviceTypeNode.InnerText);

            if (dev.deviceType == DeviceType.DT_MSR)
            {
                XmlNode badgeType = device.SelectSingleNode("BadgeType");

                if (badgeType == null) return false;
                dev.accessReaderType = (AccessBagerReaderType)Enum.Parse(typeof(AccessBagerReaderType), badgeType.InnerText);

                string comMaster, comSlave;
                if (GetComBadge(badgeUsbSerial.InnerText, out comMaster, out comSlave))
                {
                    dev.comMasterReader = comMaster;                            
                    dev.comSlaveReader = comSlave;
                }
            }

            dev.SerialFPMaster = serialFPMasterNode.InnerText;
            dev.SerialFPSlave = serialFPSlaveNode.InnerText;
            return true;
        }

        static private bool GetComBadge (string serial, out string comMaster, out string comSlave)
        {
            comMaster = null;
            comSlave = null;

            try
            {
                ManagementObjectSearcher moSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");

                foreach (ManagementObject mosObject in moSearcher.Get())
                {
                    string portName = string.Empty;
                    try
                    {
                        portName = mosObject["Name"].ToString().ToUpper();
                    }
                    catch
                    {
                        // no port name but anyway - 
                    }

                    if (portName.Contains("(COM")) // msObject["Name"] looks like "Usb Serial Device (COM1)"
                    {
                        
                            int startIndex = portName.IndexOf("(COM", StringComparison.OrdinalIgnoreCase) + 1;
                            int endIndex = portName.IndexOf(")", startIndex, StringComparison.Ordinal);

                            if (mosObject["DeviceID"].ToString().Contains(serial + "A")) // "DeviceID" contains e.g "FTU7PL54" (badge reader usb serial)
                                comMaster = portName.Substring(startIndex, endIndex - startIndex); // substring to only get e.g "COM1"

                            else if (mosObject["DeviceID"].ToString().Contains(serial + "B"))
                                comSlave = portName.Substring(startIndex, endIndex - startIndex);
                        
                    }
                }

                if ((string.IsNullOrEmpty(comMaster)) || string.IsNullOrEmpty(comSlave))
                    return false;

                return true;
            } catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }

            return false;
        }

        public static bool IsUserAdministrator()
        {
            //bool value to hold our return value
            bool isAdmin;
            try
            {
                //get the currently logged in user
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

    }
    
   
    static public class ImageUtils
    {
       static public void ResizeImage(string OriginalFile, string NewFile, int NewWidth, int MaxHeight, bool OnlyResizeIfWider)
        {
            System.Drawing.Image FullsizeImage = System.Drawing.Image.FromFile(OriginalFile);

            // Prevent using images internal thumbnail
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            if (OnlyResizeIfWider)
            {
                if (FullsizeImage.Width <= NewWidth)
                {
                    NewWidth = FullsizeImage.Width;
                }
            }

            int NewHeight = FullsizeImage.Height * NewWidth / FullsizeImage.Width;
            if (NewHeight > MaxHeight)
            {
                // Resize with height instead
                NewWidth = FullsizeImage.Width * MaxHeight / FullsizeImage.Height;
                NewHeight = MaxHeight;
            }

            System.Drawing.Image NewImage = FullsizeImage.GetThumbnailImage(NewWidth, NewHeight, null, IntPtr.Zero);

            // Clear handle to original file so that we can overwrite it if necessary
            FullsizeImage.Dispose();

            // Save resized picture
            NewImage.Save(NewFile);
        }

        static public System.Drawing.Image ResizeImage(System.Drawing.Image FullsizeImage, int NewHeight)
        {

            if (FullsizeImage.Height == NewHeight) return FullsizeImage;

            // Prevent using images internal thumbnail
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
           
            int NewWidth = FullsizeImage.Width * NewHeight / FullsizeImage.Height;           

            System.Drawing.Image NewImage = FullsizeImage.GetThumbnailImage(NewWidth, NewHeight, null, IntPtr.Zero);

            // Clear handle to original file so that we can overwrite it if necessary
            FullsizeImage.Dispose();

            return NewImage;
        }
        static public byte[] imageToByteArray(System.Drawing.Image imageIn,System.Drawing.Imaging.ImageFormat format)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                imageIn.Save(ms, format);
                return ms.ToArray();
            }
            catch
            {
                return null;
            }
        }

        static public Image byteArrayToImage(byte[] byteArrayIn)
        {
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn);
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
            catch
            {
                return null;
            }
        }

    }
        #endregion

    public static class DateTimeExtensions
    {
        public static System.DateTime getstartweek(DateTime dt)
        {           
            System.DayOfWeek dmon = System.DayOfWeek.Monday;
            int span = dt.DayOfWeek - dmon;
            dt = dt.AddDays(-span);
            return dt;
        }
    }

    public class FormuleData
    {
        public string Title;
        public string Formule;
        public int Enable;

        public FormuleData()
        {
            Title = string.Empty;
            Formule = string.Empty;
            Enable = 0;
        }
    }
    // Get Enum Text
    class Desc : Attribute
    {
        public string Text;
        public Desc(string text)
        {
            Text = text;
        }       
    }
    public static class getEnumDesc
    {
        public static string GetDescription(Enum en)
        {

            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(Desc), false);
                if (attrs != null && attrs.Length > 0)
                    return ((Desc)attrs[0]).Text;
            }
            return en.ToString();
        }
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi= value.GetType().GetField(value.ToString()); 
            DescriptionAttribute[] attributes = 
            (DescriptionAttribute[])fi.GetCustomAttributes
            (typeof(DescriptionAttribute), false);
            return (attributes.Length>0)?attributes[0].Description:value.ToString();
        }
        
    }

    /*public static class WriteOpUtils
    {
        const int nbBits = 134;
        public static readonly char[] alphaNumericTable = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '[', '#', '@', 'b', '>', '?', '_', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', '&', '.', ']', '(', ' ', '\\', '^', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', '-', '$', '*', ')', ';', 'c', '`', '/', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '<', ',', '%', 'a', '\"', '§' };

        public static int GetAlphaIndex(char val)
        {
            return Array.IndexOf(alphaNumericTable, val);
        }

        public static string ConvertAlphaNumToOct(string alphaNumValue)
        {
            string octString = string.Empty;
            if (alphaNumValue.Length != 10) return null;

            for (int i = 0; i < alphaNumValue.Length; i++)
            {
                octString += Convert.ToString(GetAlphaIndex((char)alphaNumValue[i]), 8).PadLeft(2, '0'); ;
            }
            return octString;
        }

        public static byte[] ConvertOctToUIDCodeWithCRC(string octString)
        {
            int chunkDigitIndex = 0;
            int crcIndex = 0;
            int digitIndex;
            byte[] CodeRead = new byte[45];
            int nbCRCCompute = 0;

            for (digitIndex = 0; digitIndex < CodeRead.Length; digitIndex++)
            {
                if (chunkDigitIndex < 5)
                {
                    if (digitIndex < octString.Length + nbCRCCompute)
                        CodeRead[digitIndex] = Convert.ToByte(octString[digitIndex - nbCRCCompute].ToString());
                    else
                        CodeRead[digitIndex] = 0;
                    chunkDigitIndex++;
                }
                else
                {
                    chunkDigitIndex = 0;
                    int crc = WriteOpUtils.ComputeCrc(CodeRead, crcIndex);
                    CodeRead[digitIndex] = (byte)crc;
                    crcIndex += 6;
                    nbCRCCompute++;
                }
            }
            return CodeRead;
        }
        public static string[] ConvertCodeToHexa(byte[] CodeRead)
        {
            string HexaCode;
            byte[] Code135 = new byte[160];

            for (int bcl = 0; bcl < 135; bcl++) Code135[bcl] = 0x00;

            int row = 31;
            string tmpcode;
            int[] bitShift = { 0, 32, 64, 96, 128, 160 };
            int rowBitshift = 0;
            for (int loop = 0; loop < 45; loop++)
            {
                tmpcode = Convert.ToString(CodeRead[loop], 2).PadLeft(3, '0');
                for (int bcl = 2; bcl >= 0; bcl--)
                {
                    if (tmpcode[bcl] == '1') Code135[row--] = 1;
                    else Code135[row--] = 0;

                    if (row == bitShift[rowBitshift] - 1)
                    {
                        rowBitshift++;
                        row = bitShift[rowBitshift + 1] - 1;
                    }

                }
            }

            HexaCode = CodeToHexa(Code135, 33, CodeRead);

            string[] EElatchWord = new string[4];
            EElatchWord[0] = HexaCode.Substring(0, 8);
            EElatchWord[1] = HexaCode.Substring(9, 8);
            EElatchWord[2] = HexaCode.Substring(18, 8);
            EElatchWord[3] = HexaCode.Substring(27, 8);

            return EElatchWord;
        }

        public static string CodeToHexa(byte[] bytetoconvert, int nbdigit, byte[] CodeRead)
        {
            StringBuilder tmp = new StringBuilder();
            byte theshift = 0x00;
            byte HexValue = 0x00;

            for (int bcl1 = 1; bcl1 <= nbdigit; bcl1++)
            {
                HexValue = 0;
                for (int bcl2 = 0; bcl2 < 4; bcl2++)
                {
                    theshift = RollLeft(bytetoconvert, 0);
                    HexValue <<= 1;
                    if (theshift == 0x01) HexValue += 1;

                }
                tmp.Append(HexValue.ToString("X1"));
                if ((bcl1 % 8) == 0) tmp.Append("-");

            }

            HexValue = 0;
            tmp.Append("000000");
            if (CodeRead[42] > 3) HexValue += 1;
            HexValue += (byte)(2 * CodeRead[43]);
            HexValue += (byte)(4 * CodeRead[44]);
            tmp.Append(HexValue.ToString("X1"));

            return tmp.ToString();
        }

        public static byte StringOctToByte(string s)
        {
            byte ret = 0;
            for (int p = 0; p < 3; p++)
            {
                ret <<= 1;
                if (s[p] == '1') ret += 1;
            }
            return ret;
        }
        public static byte ComputeCrc(byte[] Tab, int rg)
        {
            return ((byte)(7 - ((Tab[rg] + Tab[rg + 1] + Tab[rg + 2] + Tab[rg + 3] + Tab[rg + 4]) & 0x07)));
        }
        public static byte RollRight(byte[] bytetoroll, byte inByte)
        {
            byte ret = bytetoroll[nbBits];
            for (int bcl = nbBits; bcl > 0; bcl--)
            {
                bytetoroll[bcl] = bytetoroll[bcl - 1];
            }
            bytetoroll[0] = inByte;
            return ret;
        }
        public static byte RollLeft(byte[] bytetoroll, byte inByte)
        {
            byte ret = bytetoroll[0];
            for (int bcl = 0; bcl < nbBits; bcl++)
            {
                bytetoroll[bcl] = bytetoroll[bcl + 1];
            }
            bytetoroll[nbBits] = inByte;
            return ret;
        }
    }*/

    
}
