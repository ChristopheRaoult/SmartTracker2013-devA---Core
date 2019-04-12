using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;

using DataClass;
namespace DBClass
{
    public interface IDBInterface
    {
        Hashtable SqlUpdate { get; }

        // TestDB                     
         bool isTableExist(string table_name);  
         int getVersion();
         bool setVersion(int version);
         bool setUpdate(int updateNumber);
         bool isTableVersionExist();
        // Columns
         ArrayList GetColumnToExport();
         Hashtable GetColumnInfo();
         dtColumnInfo[] GetdtColumnInfo();
         Hashtable GetColumnCSVInfo();
         Object GetColumn();
         bool cleanDB(bool bDeleteColum = false);
        
        // Formule
         FormuleData getFormuleInfo();
         bool setFormuleInfo(FormuleData fd);

        // DB
         void Connect();
         void Close();
         bool OpenDB();
         void CloseDB();
         bool isOpen();
         void startTranscation();
         void endTranscation();      
       
        // Fridge
         bool StoreTempFridge(string SerialRFID , tempInfo tempInfoToStore);
         tempInfo GetLastFridgeTemp(string serialRFID);
         //string[] GetFridgeTempAfter(string serialRFID, string DateTimetoParse);
         string[] GetFridgeTempAfter(string serialRFID, string DateTimetoParse, int nbItem = int.MaxValue);
         bool GetLastFridgeTempfromxHours(string serialRFID, int nbHour, out ArrayList tempChamber, out ArrayList tempBottle);

         PtTemp GetLastTempPoint(string SerialRFID, bool bFridgeOK);
         PtTemp[] GetPointFromDate(string SerialRFID, string date);
         List<PtTemp> GetPointBeforeDate(string SerialRFID, string date, int NbPt);
         PtTemp[] GetPointBetweenDate(string SerialRFID, string dateFrom, string dateTo);
         bool AddNewPtTemp(string SerialRFID, string date);
         bool AddNewPtTemp(string SerialRFID, string date, double tempBottle, double tempChamber);
         bool UpdatePtTemp(string SerialRFID, string date, double tempBottle, double tempChamber);
         bool IsPointExist(string SerialRFID, string date, out bool bNotNull);
         void UpdateLastReportDate(string dateAsString);
         string GetLastReportDate();
        
        // alert
         smtpInfo getSmtpInfo(bool onlyWhenActive);
         bool DeleteSMTP();
         bool AddSmtp(smtpInfo smtp);
         bool DeleteAlert(alertInfo alert);
         bool AddAlertInfo(alertInfo alert);
         int GetDoorOpenToLongTime();       
         double GetMaxTempFridgeValue();
         alertInfo getAlertInfo(AlertType alertType , bool bActive);
         string GetColumnNameForAlert(AlertType alertType);
         string GetColumnNameForWeight();
         bool storeAlert(AlertType alertType, DeviceInfo di, UserClassTemplate utc, string alertData);
        
        // Group
         DataTable RecoverAllGroup();
         bool DeleteTagLinkFromBox(string tagBox);
         bool DeleteGroup(string tagBox);
         bool AddGroup(string TagBox,string groupRef, string groupInfo , string criteria = null);
         int GetGroupLinkCount(string GroupTagRef);
         string GetGroupName(string TagUID);
         ArrayList GetGroupLink(string GroupTagRef);
         bool DeleteGroupLink(string  ProductTagRef);
         bool AddGroupLink(string GroupTagRef, string productTagRef);
        
        // Image
         ArrayList GetImageListName();
         bool IsImageExist(string imageName);
         bool AddImage(string imageName , byte[] imageArray);
         bool DeleteImage(string imageName);
         byte[] getImage(string imageName);
         int getNumberofLink(string ImageName);
         bool IsImageLinkExist(string LotIDorTagID);
         string getImageNameLink(string LotIDorTagID);
         bool AddImageLink(string imageName, string LotIDorTagID );
         bool DeleteallImageLink(string ImageName);
         bool DeleteImageLink(string LotIDorTagID);
        
        // User
         UserClassTemplate[] RecoverUser();
         //UserClassTemplate[] RecoverAllowedUser(string serialRFID );
         DeviceGrant[] RecoverAllowedUser(string serialRFID);
         UserClassTemplate RecoverUser(string firstName, string lastName);
         bool IsUserExist(string firstName, string lastName);
         bool StoreUser(UserClassTemplate UserToStore);
         int getUserFingerAlert(string firstName, string lastName);
         bool UpdateUserFingerAlert(string firstName, string lastName , int fingerAlert);
         bool UpdateUserMaxRemovedItem(string firstName, string lastName, int maxItem);
         int getUserMaxRemovedItem(string firstName, string lastName);
         bool UpdateUserMaxRemoveValue(string firstName, string lastName, float maxRemoveValue);
         double getUserMaxRemoveValue(string firstName, string lastName);
         bool DeleteUser(string firstName, string lastName);        
         bool DeleteAllUser();
        
       // user Grant
         string[] recoverUserGrant(UserClassTemplate User);
         DeviceGrant[] recoverUserGrantFull(UserClassTemplate User);
         bool isGrantExist(string firstName, string lastName, string SerialRFID);
         bool StoreGrant(UserClassTemplate UserToStore, string SerialRFID,string BelongTo,UserGrant userGrant);
         bool DeleteUserGrant(UserClassTemplate UserToStore, string SerialRFID);
         bool DeleteUserGrant(string firstName, string lastName, string SerialRFID);
         bool DeleteUserGrant(string BelongTo);
        
        // Device
         ArrayList RecoverDistinctSerialRFID();
         DeviceInfo[] RecoverAllDevice(string machineName = null);
         List<DeviceInfo> GetAllDevices();   
         DeviceInfo[] RecoverDevice(bool bLocal,string machine = null);
         DeviceInfo RecoverDevice(string SerialRFID);
         bool IsDeviceExist(string SerialRFID, string machineName = null);
         bool StoreDevice(DeviceInfo DeviceToStore, bool bNetwork, string machineName = null);
         bool UpdateNetworkDevice(string serialRFID, string IP_Remote, int port_Remote);
         bool DeleteDevice(DeviceInfo DeviceToStore);
        
        // Product
         DataTable RecoverAllProduct();
         string getProductTagID(string LotID);
         string[] RecoverProductInfo(string TagUID);
         ArrayList RecoverProductHistory(string tagUid);
         bool IsProductExist(string tagUID);
         bool StoreProduct(ProductClassTemplate ProductToStore);
         bool DeleteProduct(string tagUID);

         bool AddUidHistory(string _initialUid, string _writtenUid);
         string GetInitialUID(string tagUID);
         List<UidWriteHistory> GetUidHistory(string _initialUid);
        
        // Inventory
         bool IsInventoryExist(InventoryData IDtoStore);
         //bool StoreInventory(InventoryData IDtoStore);
         bool StoreInventory(InventoryData IDtoStore, bool bStoreforDevice = false);
         int getRowInsertIndex();
         bool storeTagEvent(InventoryData IDtoStore, int IdScan);
         InventoryData[] GetInventory(DeviceInfo di, UserClassTemplate uct , int nbData = 100);
         string[] GetInventoryBefore(string SerialRFID, string DateTimetoParse);
         string[] GetInventoryAfter(string SerialRFID, string DateTimetoParse);
         string[] GetInventory(string SerialRFID, string DateTimetoParse);
         string[] GetInventoryFromData(string SerialRFID, string spareData1, string spareData2);
         InventoryData GetLastScan(string serialRFID);
         InventoryData GetLastScanFromID(string serialRFID, int IdEvent);
         TagEventClass[] GetTagEvent(string tagUID);
         TagEventClass GetLastTagEvent(string tagUID);
         bool DeleteOldInventory(string serialRFID, int NbToKeep);
        
        // import
         DataSet ImportCSVToDS(string filename);
         DataSet ImportExcelToDS(string filename);
         DataSet ImportExcelToDS(string filename, string sheetName);       
       
         
        // stat
         int GetInventoryCount(string serialRFID = null, string firstName = null, string lastName = null, string dateFrom = null, string dateTo = null);
        

    }
}
