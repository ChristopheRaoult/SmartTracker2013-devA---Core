using System;
using System.Collections.Generic;
using System.Text;

using DataClass;
using SDK_SC_Fingerprint;
using SDK_SC_RfidReader;

namespace SDK_SC_RFID_Devices
{
    interface IRFID_Device
    {
        rfidPluggedInfo[] getRFIDpluggedDevice(bool bsearchCom2Plus);

        bool Create_NoFP_Device(string serialNumberRFID);
        bool Create_NoFP_Device(string serialNumberRFID,string portCom);

        bool Create_1FP_Device(string serialNumberRFID,string serialNumberFP_Master,bool bLoadTemplateFromDB);
        bool Create_1FP_Device(string serialNumberRFID,string portCom,string serialNumberFP_Master, bool bLoadTemplateFromDB);        

        bool Create_2FP_Device(string serialNumberRFID,string serialNumberFP_Master, string serialNumberFP_Slave, bool bLoadTemplateFromDB);
        bool Create_2FP_Device(string serialNumberRFID,string portCom,string serialNumberFP_Master, string serialNumberFP_Slave, bool bLoadTemplateFromDB);

        bool ScanDevice(bool bUseKR = true, bool bUnlockAllTag = true);
        bool ScanDeviceMono();
        bool StopScan();
        bool EnableWaitMode();
        bool DisableWaitMode();
        void RenewFP(bool renewboth, bool bLoadTemplateFromDB);
        int LoadFPTemplateFromDB(FingerPrintClass theFP,UserGrant userGrant);
        int LoadFPTemplate(string[] templates, FingerPrintClass theFP);
        string EnrollUser(string FPSerialNumber, string FirstName, string LastName, string template);
       
        bool UnLock();
        bool Lock();
        bool SetLight(ushort power);

        void ReleaseDevice();


        int TimeBeforeCloseLock { get; set; }
        int TimeDoorOpenTooLong { get; set; }

        string SerialNumberRFID { get; }
        //string DeviceTypeReader { get; }
        ConnectionStatus ConnectionStatus { get;}
        DeviceStatus DeviceStatus { get;}
        FPStatus FPStatusMaster { get ;}
        FPStatus FPStatusSlave { get ; }

        ushort LightInIdle { get; set; }
        ushort LightInScan { get; set; }
        ushort LightDoorOpen { get; set; }

        RfidReader get_RFID_Device { get ; }
        FingerPrintClass get_FP_Master { get;  }
        FingerPrintClass get_FP_Slave { get;  }

        InventoryData LastScanResult { get; }
        InventoryData setPreviousScan { set; }

        bool InterruptScanWithFP { get; set;}
    }
}
