using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Management;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Threading;

using DataClass;
using SDK_SC_RFID_Devices;
using SDK_SC_RfidReader;
namespace SDK_SC_Rfid_And_Scale
{
    public enum ScaleComType
    {       
        SCT_RS232 = 0x00,
        SCT_USB = 0x01,
    }
    public enum retCode
    {
        RC_succeed = 0x00,
        RC_RFID_Not_Connected = 0x01,
        RC_Scale_Not_Connected = 0x02,
        RC_Scale_Not_Stabilized = 0x03,
        RC_No_Tag_Detected = 0x04,
        RC_Too_Many_Tags_Detected = 0x05,
        RC_Error_RFID = 0x06,
        RC_Unknown = 0xFF,
    }
        
    public class RFID_And_Scale_Device : IRFID_And_Scale,IDisposable
    {
        bool disposed;

        retCode ret;
        private Sartorius_Scale scale = null;
        private RFID_Device device = null;
        ArrayList listTagID = new ArrayList();

        private bool isRfidConnected = false;
        private bool isScaleConnected = false;

        public bool IsRfidConnected { get { return isRfidConnected; } }
        public bool IsScaleConnected { get { return isScaleConnected; } }

        ScaleComType scaleType;
        public ScaleComType ScaleType { get { return scaleType; } }
        string serialRFID = string.Empty;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed ressources
                    if (device != null)
                        device.ReleaseDevice();
                    if (scale != null) 
                        scale.closePort();
                }
            }
            //dispose unmanaged ressources
            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public RFID_And_Scale_Device (ScaleComType scaleType)
        {        
            this.scaleType = scaleType;

            if (scaleType == ScaleComType.SCT_RS232)
            {
                if (!IsUserAdministrator())
                {
                    ErrorMessage.ExceptionMessageBox.Show("Scale in RS232 Mode request Administrator for autoConnection ", "Information");
                    Dispose(true);
                }
            }
            else if (scaleType == ScaleComType.SCT_USB)
            {
                ErrorMessage.ExceptionMessageBox.Show("Scale in USB Mode not allowed with this SDK Version", "Information");
                Dispose(true);
            }
            else
            {
                Dispose(true);
            }
        }

        #region Discover
        private EventWaitHandle eventEndDiscover = new AutoResetEvent(false);
        private EventWaitHandle eventEndConnection = new AutoResetEvent(false);
        private EventWaitHandle eventEndRFID = new AutoResetEvent(false);
        private EventWaitHandle eventEndScale = new AutoResetEvent(false);
        public rfidPluggedInfo[] getRFIDpluggedDevice()
        {
            try
            {
                ArrayList listdev = new ArrayList();
                //string[] ports = System.IO.Ports.SerialPort.GetPortNames();

                List<string> ports = GetDevicePortCom();
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
        private List<string> GetDevicePortCom()
        {
            List<string> comPortList = new List<string>(System.IO.Ports.SerialPort.GetPortNames());
            try
            {
                const string VID = "0403";               
                const string PID2 = "6010";
                //ArrayList comPortList = new ArrayList(System.IO.Ports.SerialPort.GetPortNames());
                    List<string> comports = new List<string>();
                    string pattern = String.Format("^VID_{0}.PID_{1}", VID, PID2);
                    Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
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

                if ((comports != null) && (comports.Count > 0))
                    return comports;
                else
                    return comPortList;

            }
            catch
            {
                return comPortList;
            }
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
        #endregion

        #region Connection
        public bool ConnectRFIDandScale(string serialRFID , string portCom)
        {
            this.serialRFID = serialRFID;
            isScaleConnected = false;
            isRfidConnected = false;
            // release previous object if not connected
            if (device != null)
            {              
                 device.ReleaseDevice();
            }
            //Create a new object 
            device = new RFID_Device();
            //subscribe the event 
            device.NotifyRFIDEvent += new NotifyHandlerRFIDDelegate(rfidDev_NotifyRFIDEvent);
            //Create a d device   
            eventEndConnection.Reset();     
            device.Create_NoFP_Device(serialRFID,portCom);            
            eventEndConnection.WaitOne();
            if (isRfidConnected && isScaleConnected) return true;
            else return false;
        }
        private bool FindAndConnectScale(string serialRFID, string comPortRFID)
        {
            isScaleConnected = false;
            bool ret = false;
            string comScale = null;
            if (GetComScale(serialRFID, comPortRFID, out comScale))
            {
                if (scale != null) scale.closePort();
                scale = new Sartorius_Scale(comScale);
                scale.NotifyWeightEvent += new NotifyHandlerWeightScaleDelegate(scale_NotifyWeightEvent);
                if (scale.IsConnected)
                {
                    scale.getWeight();
                    System.Threading.Thread.Sleep(3000);
                    if (scale.LastScaledWeight != null)
                    {
                        isScaleConnected = true;
                        ret = true;
                    }                  
                }              
            }        
            if (eventEndConnection != null)
                eventEndConnection.Set();
            return ret;
        }
        static private bool GetComScale(string serialNumber, string comPortRFID, out string comScale)
        {
            comScale = null;           
            string instancePort = null;
            string portScale = null;
            ManagementObjectSearcher MOSearcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSSerial_PortName");
            foreach (ManagementObject MOject in MOSearcher.Get())
            {
                if (MOject["InstanceName"].ToString().Contains("FTDIBUS\\VID_0403+PID_6010"))
                {
                    instancePort = MOject["InstanceName"].ToString().Substring(26, 8);
                    if (MOject["InstanceName"].ToString().Contains(instancePort + "A"))
                        portScale = MOject["PortName"].ToString().ToUpper();
                    if (MOject["InstanceName"].ToString().Contains(instancePort + "B"))
                    {
                        if (MOject["PortName"].ToString().ToUpper() == comPortRFID)
                        {
                            comScale = portScale;
                            break;
                        }
                        else
                            portScale = null;
                    }
                }
            }
            MOSearcher.Dispose();       
                
            if (string.IsNullOrEmpty(comScale))
                return false;
            else
                return true;
        }

        #endregion
        #region RFID
        private void rfidDev_NotifyRFIDEvent(object sender, SDK_SC_RfidReader.rfidReaderArgs args)
        {
            switch (args.RN_Value)
            {
                case SDK_SC_RfidReader.rfidReaderArgs.ReaderNotify.RN_FailedToConnect:
                {
                    isRfidConnected = false;
                    if (eventEndConnection != null)
                        eventEndConnection.Set();
                    break;
                }
                case SDK_SC_RfidReader.rfidReaderArgs.ReaderNotify.RN_Disconnected:
                {
                    isRfidConnected = false;
                } break;
                case SDK_SC_RfidReader.rfidReaderArgs.ReaderNotify.RN_Connected:
                {
                    isRfidConnected = true;
                    FindAndConnectScale(args.SerialNumber, device.get_RFID_Device.StrCom);
                    break;
                }
                case rfidReaderArgs.ReaderNotify.RN_ErrorDuringScan:
                case rfidReaderArgs.ReaderNotify.RN_ReaderFailToStartScan:
                    {
                        ret = retCode.RC_Error_RFID;
                        if (eventEndRFID != null)
                             eventEndRFID.Set();
                        break;
                    }
                case rfidReaderArgs.ReaderNotify.RN_ScanStarted:
                    {
                        break;
                    }
                case rfidReaderArgs.ReaderNotify.RN_ScanCompleted:
                    {
                        if (eventEndRFID != null)
                            eventEndRFID.Set();
                        break;
                    }

            }
        }
        #endregion
        #region Scale
        private void scale_NotifyWeightEvent(Object sender, string deviceSerial, string dataReceived)
        {
            if (eventEndScale != null)
                eventEndScale.Set();
        }
        #endregion

        public retCode getTagAndWeight(int timeoutScale, out string tagUID, out double scaleWeight)
        {
            tagUID = null;
            scaleWeight = 0.0;
            ret = retCode.RC_Unknown;
            if (device == null) ret = retCode.RC_RFID_Not_Connected;
            else if (scale == null) ret = retCode.RC_Scale_Not_Connected;
            else if (!isRfidConnected) ret = retCode.RC_RFID_Not_Connected;
            else if (!isScaleConnected) ret = retCode.RC_Scale_Not_Connected;
            else
            {
                device.ScanDevice();
                eventEndRFID.WaitOne();
                if (device.currentInventory.nbTagAll == 0)
                {
                    ret = retCode.RC_No_Tag_Detected;
                }
                else if (device.currentInventory.nbTagAll > 1)
                {
                    ret = retCode.RC_Too_Many_Tags_Detected;
                }
                else if (device.currentInventory.nbTagAll == 1)
                {
                    tagUID = device.currentInventory.listTagAll[0].ToString();
                    eventEndScale.Reset();
                    scale.getWeight();
                    if (eventEndScale.WaitOne(timeoutScale))
                    {
                        if (scale.LastScaledWeight != null)
                        {
                            scaleWeight = scale.LastScaledWeight.WeightValue;
                            ret = retCode.RC_succeed;
                        }
                        else
                            ret = retCode.RC_Unknown;
                    }
                    else
                    {
                        ret = retCode.RC_Scale_Not_Stabilized;
                    }
                }
            }

            return ret;            
        }

        #region utils
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
        #endregion
    }
}
