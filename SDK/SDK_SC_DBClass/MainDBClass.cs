using System;
using System.Collections.Generic;
using System.Text;
using DB_Class_SQLite;
using System.Collections;
using System.Data;

using DataClass;
namespace DBClass
{
    public class MainDBClass
    {
        public enum dbUsedType
        {
            db_SqlLite = 0x00,
            db_SqlServer = 0x01,
        }

        IDBInterface sqliteProvider;
        IDBInterface sqlServerProvider;
        private dbUsedType useDb = dbUsedType.db_SqlLite;
        public dbUsedType getUsedDBType { get { return useDb; } }
        public MainDBClass()
        {
            sqliteProvider = new DB_Class_SQLite.DBClassSQLite();

            string conString = string.Empty;
            int bUseSQlServer = 0;
            DBClassSQLite db1 = new DBClassSQLite();
            db1.OpenDB();
            if (db1.isTableExist("tb_SqlServer_Info"))
            {
                db1.getSqlServerInfo(out conString, out bUseSQlServer);
            }
            db1.CloseDB();

            if ((bUseSQlServer == 1) && (!string.IsNullOrEmpty(conString)))
            {    
                useDb = dbUsedType.db_SqlServer;
                sqlServerProvider = new DBClassSqlServer.DBSqlServer(DBClass_SQLServer.UtilSqlServer.ConvertConnectionString(conString));
                try
                {
                    OpenDB();
                    CloseDB();
                }
                catch
                {
                    useDb = dbUsedType.db_SqlLite;
                    ErrorMessage.ExceptionMessageBox.Show("Unable to connect to SQL Server Database - Application start with local database to check configuration", "Information");
                }               

            }
        }

        public Hashtable SqlUpdate
        {
            get
            {
                switch (useDb)
                {
                    case dbUsedType.db_SqlLite: return sqliteProvider.SqlUpdate; 
                    case dbUsedType.db_SqlServer: return sqlServerProvider.SqlUpdate; 
                }
                return null;
            }
        }

        // TestDB                     
        public bool isTableExist(string table_name)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.isTableExist(table_name); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.isTableExist(table_name); 
            }
            return false;
        }
        public int getVersion()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.getVersion(); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.getVersion(); 
            }
            return -1;
        }
        public bool setVersion(int version)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.setVersion(version); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.setVersion(version); 
            }
            return false;
        }
        public bool setUpdate(int updateNumber)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.setUpdate(updateNumber); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.setUpdate(updateNumber); 
            }
            return false;
        }
        public bool isTableVersionExist()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.isTableVersionExist(); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.isTableVersionExist(); 
            }
            return false;
        }

        // Columns
        public ArrayList GetColumnToExport()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetColumnToExport(); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetColumnToExport(); 
            }
            return null;
        }
        public Hashtable GetColumnInfo()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetColumnInfo(); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetColumnInfo(); 
            }
            return null;
        }
        public dtColumnInfo[] GetdtColumnInfo()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetdtColumnInfo(); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetdtColumnInfo(); 
            }
            return null;
        }
        public Hashtable GetColumnCSVInfo()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetColumnCSVInfo() ; 
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetColumnCSVInfo(); 
            }
            return null;
        }
        public Object GetColumn()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetColumn(); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetColumn(); 
            }
            return null;
        }
        public bool cleanDB(bool bDeleteColum = false)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.cleanDB(bDeleteColum); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.cleanDB(bDeleteColum); 
            }
            return false;
        }

        // Formule
        public FormuleData getFormuleInfo()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.getFormuleInfo(); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.getFormuleInfo(); 
            }
            return null;
        }
        public bool setFormuleInfo(FormuleData fd)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.setFormuleInfo(fd); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.setFormuleInfo(fd); 
            }
            return false;
        }

        // DB
        public bool OpenDB()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.OpenDB(); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.OpenDB(); 
            }
            return false;
        }
        public void CloseDB()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: sqliteProvider.CloseDB(); break;
                case dbUsedType.db_SqlServer:  sqlServerProvider.CloseDB(); break;
            }           
        }
        public bool isOpen()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.isOpen(); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.isOpen(); 
            }
            return false;
        }
        public void startTranscation()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: sqliteProvider.startTranscation(); break;
                case dbUsedType.db_SqlServer: sqlServerProvider.startTranscation(); break;
            }
            return;
        }
        public void endTranscation()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: sqliteProvider.endTranscation(); break;
                case dbUsedType.db_SqlServer: sqlServerProvider.endTranscation(); break;
            }
            return;
        }

        // Fridge
        public bool StoreTempFridge(string SerialRFID, tempInfo tempInfoToStore)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.StoreTempFridge(SerialRFID, tempInfoToStore); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.StoreTempFridge(SerialRFID, tempInfoToStore); 
            }
            return false;
        }
        public tempInfo GetLastFridgeTemp(string serialRFID)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetLastFridgeTemp(serialRFID); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetLastFridgeTemp(serialRFID); 
            }
            return null;
        }
        public string[] GetFridgeTempAfter(string serialRFID, string DateTimetoParse, int nbItem = int.MaxValue)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetFridgeTempAfter(serialRFID, DateTimetoParse, nbItem); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetFridgeTempAfter(serialRFID, DateTimetoParse,nbItem); 
            }
            return null;
        }
        public bool GetLastFridgeTempfromxHours(string serialRFID, int nbHour, out ArrayList tempChamber, out ArrayList tempBottle)
        {
            tempChamber = null;
            tempBottle = null;
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetLastFridgeTempfromxHours(serialRFID, nbHour, out tempChamber, out tempBottle);
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetLastFridgeTempfromxHours(serialRFID, nbHour, out tempChamber, out tempBottle);
            }
            return false;
        }
        //V2
        public PtTemp GetLastTempPoint(string SerialRFID, bool bFridgeOK)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetLastTempPoint(SerialRFID, bFridgeOK);
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetLastTempPoint(SerialRFID, bFridgeOK);
            }
            return null;
        }
        public PtTemp[] GetPointFromDate(string SerialRFID, string date)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetPointFromDate(SerialRFID, date);
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetPointFromDate(SerialRFID, date);
            }
            return null;
        }
        public List<PtTemp> GetPointBeforeDate(string SerialRFID, string date, int NbPt)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetPointBeforeDate(SerialRFID, date, NbPt);
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetPointBeforeDate(SerialRFID, date, NbPt);
            }
            return null;
        }
        public PtTemp[] GetPointBetweenDate(string SerialRFID, string dateFrom, string dateTo)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetPointBetweenDate(SerialRFID, dateFrom, dateTo);
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetPointBetweenDate(SerialRFID, dateFrom, dateTo);
            }
            return null;
        }
        public bool AddNewPtTemp(string SerialRFID, string date)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.AddNewPtTemp(SerialRFID, date);
                case dbUsedType.db_SqlServer: return sqlServerProvider.AddNewPtTemp(SerialRFID, date);
            }
            return false;
        }
        public bool AddNewPtTemp(string SerialRFID, string date, double tempBottle, double tempChamber)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.AddNewPtTemp(SerialRFID, date, tempBottle, tempChamber);
                case dbUsedType.db_SqlServer: return sqlServerProvider.AddNewPtTemp(SerialRFID, date, tempBottle, tempChamber);
            }
            return false;
        }
        public bool UpdatePtTemp(string SerialRFID, string date, double tempBottle, double tempChamber)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.UpdatePtTemp(SerialRFID, date, tempBottle, tempChamber);
                case dbUsedType.db_SqlServer: return sqlServerProvider.UpdatePtTemp(SerialRFID, date, tempBottle, tempChamber);
            }
            return false;
        }
        public bool IsPointExist(string SerialRFID, string date, out bool bNotNull)
        {
            bNotNull = false;
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.IsPointExist(SerialRFID, date ,out bNotNull);
                case dbUsedType.db_SqlServer: return sqlServerProvider.IsPointExist(SerialRFID, date , out bNotNull);
            }
            return false;
        }
        public void UpdateLastReportDate(string dateAsString)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: sqliteProvider.UpdateLastReportDate(dateAsString); break;
                case dbUsedType.db_SqlServer: sqlServerProvider.UpdateLastReportDate(dateAsString); break;
            }
        }
        public string GetLastReportDate()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetLastReportDate(); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetLastReportDate(); 
            }

            return String.Empty;
        }

        // alert
        public smtpInfo getSmtpInfo(bool onlyWhenActive)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.getSmtpInfo(onlyWhenActive); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.getSmtpInfo(onlyWhenActive); 
            }
            return null;
        }
        public bool DeleteSMTP()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.DeleteSMTP(); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.DeleteSMTP(); 
            }
            return false;
        }
        public bool AddSmtp(smtpInfo smtp)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.AddSmtp(smtp); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.AddSmtp(smtp); 
            }
            return false;
        }
        public bool DeleteAlert(alertInfo alert)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.DeleteAlert(alert); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.DeleteAlert(alert); 
            }
            return false;
        }
        public bool AddAlertInfo(alertInfo alert)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.AddAlertInfo(alert); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.AddAlertInfo(alert); 
            }
            return false;
        }
        public int GetDoorOpenToLongTime()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetDoorOpenToLongTime(); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetDoorOpenToLongTime(); 
            }
            return 30;
        }
        public double GetMaxTempFridgeValue()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetMaxTempFridgeValue(); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetMaxTempFridgeValue(); 
            }
            return 12.0;
        }
        public alertInfo getAlertInfo(AlertType alertType, bool bActive)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.getAlertInfo(alertType,bActive); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.getAlertInfo(alertType, bActive); 
            }
            return null;
        }
        public string GetColumnNameForAlert(AlertType alertType)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetColumnNameForAlert(alertType); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetColumnNameForAlert(alertType); 
            }
            return null;
        }

        public string GetColumnNameForWeight()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetColumnNameForWeight();
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetColumnNameForWeight();
            }
            return null;
        }

        public bool storeAlert(AlertType alertType, DeviceInfo di, UserClassTemplate utc, string alertData)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.storeAlert(alertType,di,utc,alertData); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.storeAlert(alertType, di, utc, alertData); 
            }
            return false;
        }

        // Group
        public DataTable RecoverAllGroup()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.RecoverAllGroup(); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.RecoverAllGroup(); 
            }
            return null;
        }
        public bool DeleteTagLinkFromBox(string tagBox)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.DeleteTagLinkFromBox(tagBox); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.DeleteTagLinkFromBox(tagBox); 
            }
            return false;
        }
        public bool DeleteGroup(string tagBox)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.DeleteGroup(tagBox); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.DeleteGroup(tagBox); 
            }
            return false;
        }
        public bool AddGroup(string TagBox, string groupRef, string groupInfo, string criteria = null)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.AddGroup(TagBox,groupRef,groupInfo,criteria); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.AddGroup(TagBox, groupRef, groupInfo, criteria); 
            }
            return false;
        }
        public int GetGroupLinkCount(string GroupTagRef)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetGroupLinkCount(GroupTagRef); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetGroupLinkCount(GroupTagRef); 
            }
            return 0;
        }
        public string GetGroupName(string TagUID)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetGroupName(TagUID); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetGroupName(TagUID); 
            }
            return null;
        }
        public ArrayList GetGroupLink(string GroupTagRef)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetGroupLink(GroupTagRef); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetGroupLink(GroupTagRef); 
            }
            return null;
        }
        public bool DeleteGroupLink(string ProductTagRef)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.DeleteGroupLink(ProductTagRef); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.DeleteGroupLink(ProductTagRef); 
            }
            return false;
        }
        public bool AddGroupLink(string GroupTagRef, string productTagRef)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.AddGroupLink(GroupTagRef,productTagRef); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.AddGroupLink(GroupTagRef, productTagRef); 
            }
            return false;
        }

        // Image
        public ArrayList GetImageListName()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetImageListName(); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetImageListName(); 
            }
            return null;
        }
        public bool IsImageExist(string imageName)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.IsImageExist(imageName); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.IsImageExist(imageName); 
            }
            return false;
        }
        public bool AddImage(string imageName, byte[] imageArray)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.AddImage(imageName,imageArray); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.AddImage(imageName, imageArray); 
            }
            return false;
        }
        public bool DeleteImage(string imageName)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.DeleteImage(imageName); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.DeleteImage(imageName); 
            }
            return false;
        }
        public byte[] getImage(string imageName)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.getImage(imageName); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.getImage(imageName); 
            }
            return null;
        }
        public int getNumberofLink(string ImageName)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.getNumberofLink(ImageName); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.getNumberofLink(ImageName); 
            }
            return 0;
        }
        public bool IsImageLinkExist(string LotIDorTagID)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.IsImageLinkExist(LotIDorTagID); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.IsImageLinkExist(LotIDorTagID); 
            }
            return false;
        }
        public string getImageNameLink(string LotIDorTagID)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.getImageNameLink(LotIDorTagID); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.getImageNameLink(LotIDorTagID); 
            }
            return null;
        }
        public bool AddImageLink(string imageName, string LotIDorTagID)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.AddImageLink(imageName,LotIDorTagID); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.AddImageLink(imageName, LotIDorTagID); 
            }
            return false;
        }
        public bool DeleteallImageLink(string ImageName)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.DeleteallImageLink(ImageName); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.DeleteallImageLink(ImageName); 
            }
            return false;
        }
        public bool DeleteImageLink(string LotIDorTagID)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.DeleteImageLink(LotIDorTagID); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.DeleteImageLink(LotIDorTagID); 
            }
            return false;
        }

        // User
        public UserClassTemplate[] RecoverUser()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.RecoverUser(); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.RecoverUser(); 
            }
            return null;
        }
        /*public UserClassTemplate[] RecoverAllowedUser(string serialRFID)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.RecoverAllowedUser(serialRFID); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.RecoverAllowedUser(serialRFID); 
            }
            return null;
        }*/
        public DeviceGrant[] RecoverAllowedUser(string serialRFID)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.RecoverAllowedUser(serialRFID);
                case dbUsedType.db_SqlServer: return sqlServerProvider.RecoverAllowedUser(serialRFID);
            }
            return null;
        }
        public UserClassTemplate RecoverUser(string firstName, string lastName)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.RecoverUser(firstName,lastName); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.RecoverUser(firstName, lastName); 
            }
            return null;
        }
        public bool IsUserExist(string firstName, string lastName)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.IsUserExist(firstName,lastName); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.IsUserExist(firstName, lastName); 
            }
            return false;
        }
        public bool StoreUser(UserClassTemplate UserToStore)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.StoreUser(UserToStore); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.StoreUser(UserToStore); 
            }
            return false;
        }
        public int getUserFingerAlert(string firstName, string lastName)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.getUserFingerAlert(firstName,lastName); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.getUserFingerAlert(firstName, lastName); 
            }
            return 0;
        }
        public bool UpdateUserFingerAlert(string firstName, string lastName, int fingerAlert)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.UpdateUserFingerAlert(firstName,lastName,fingerAlert); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.UpdateUserFingerAlert(firstName, lastName, fingerAlert); 
            }
            return false;
        }
        public bool UpdateUserMaxRemovedItem(string firstName, string lastName, int maxItem)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.UpdateUserMaxRemovedItem(firstName,lastName,maxItem); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.UpdateUserMaxRemovedItem(firstName, lastName, maxItem); 
            }
            return false;
        }
        public int getUserMaxRemovedItem(string firstName, string lastName)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.getUserMaxRemovedItem(firstName,lastName);
                case dbUsedType.db_SqlServer: return sqlServerProvider.getUserMaxRemovedItem(firstName, lastName); 
            }
            return 0;
        }
        public bool UpdateUserMaxRemoveValue(string firstName, string lastName, float maxRemoveValue)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.UpdateUserMaxRemoveValue(firstName,lastName,maxRemoveValue); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.UpdateUserMaxRemoveValue(firstName, lastName, maxRemoveValue); 
            }
            return false;
        }
        public double getUserMaxRemoveValue(string firstName, string lastName)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.getUserMaxRemoveValue(firstName,lastName); 
                case dbUsedType.db_SqlServer: return sqlServerProvider.getUserMaxRemoveValue(firstName, lastName); 
            }
            return 0.0;
        }
        public bool DeleteUser(string firstName, string lastName)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.DeleteUser(firstName,lastName);
                case dbUsedType.db_SqlServer: return sqlServerProvider.DeleteUser(firstName, lastName);
            }
            return false;
        }
        public bool DeleteAllUser()
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.DeleteAllUser();
                case dbUsedType.db_SqlServer: return sqlServerProvider.DeleteAllUser();
            }
            return false;
        }

        // user Grant
        public string[] recoverUserGrant(UserClassTemplate User)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.recoverUserGrant(User);
                case dbUsedType.db_SqlServer: return sqlServerProvider.recoverUserGrant(User);
            }
            return null;
        }

        public DeviceGrant[] recoverUserGrantFull(UserClassTemplate User)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.recoverUserGrantFull(User);
                case dbUsedType.db_SqlServer: return sqlServerProvider.recoverUserGrantFull(User);
            }
            return null;
        }

        public bool isGrantExist(string firstName, string lastName, string SerialRFID)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.isGrantExist(firstName,lastName,SerialRFID);
                case dbUsedType.db_SqlServer: return sqlServerProvider.isGrantExist(firstName, lastName, SerialRFID);
            }
            return false;
        }
        public bool StoreGrant(UserClassTemplate UserToStore, string SerialRFID, string BelongTo,UserGrant userGrant)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.StoreGrant(UserToStore, SerialRFID, BelongTo, userGrant);
                case dbUsedType.db_SqlServer: return sqlServerProvider.StoreGrant(UserToStore, SerialRFID, BelongTo, userGrant);
            }
            return false;
        }
        public bool DeleteUserGrant(UserClassTemplate UserToStore, string SerialRFID)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.DeleteUserGrant(UserToStore,SerialRFID);
                case dbUsedType.db_SqlServer: return sqlServerProvider.DeleteUserGrant(UserToStore, SerialRFID);
            }
            return false;
        }
        public bool DeleteUserGrant(string firstName, string lastName, string SerialRFID)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.DeleteUserGrant(firstName,lastName,SerialRFID);
                case dbUsedType.db_SqlServer: return sqlServerProvider.DeleteUserGrant(firstName, lastName, SerialRFID);
            }
            return false;
        }
        public bool DeleteUserGrant(string BelongTo)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.DeleteUserGrant(BelongTo);
                case dbUsedType.db_SqlServer: return sqlServerProvider.DeleteUserGrant(BelongTo);
            }
            return false;
        }

        // Device
        public ArrayList RecoverDistinctSerialRFID()
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.RecoverDistinctSerialRFID();
                case dbUsedType.db_SqlServer: return sqlServerProvider.RecoverDistinctSerialRFID();
            }
            return null;
        }
        public DeviceInfo[] RecoverAllDevice()
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.RecoverAllDevice();
                case dbUsedType.db_SqlServer: return sqlServerProvider.RecoverAllDevice(Environment.MachineName.ToString());
            }
            return null;
        }
        public DeviceInfo[] RecoverDevice(bool bLocal)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.RecoverDevice(bLocal);
                case dbUsedType.db_SqlServer: return sqlServerProvider.RecoverDevice(bLocal,Environment.MachineName.ToString());
            }
            return null;
        }
        public DeviceInfo RecoverDevice(string SerialRFID)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.RecoverDevice(SerialRFID);
                case dbUsedType.db_SqlServer: return sqlServerProvider.RecoverDevice(SerialRFID);
            }
            return null;
        }
        public bool IsDeviceExist(string SerialRFID)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.IsDeviceExist(SerialRFID);
                case dbUsedType.db_SqlServer: return sqlServerProvider.IsDeviceExist(SerialRFID,Environment.MachineName.ToString());
            }
            return false;
        }
        public bool StoreDevice(DeviceInfo DeviceToStore, bool bNetwork)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.StoreDevice(DeviceToStore,bNetwork);
                case dbUsedType.db_SqlServer: return sqlServerProvider.StoreDevice(DeviceToStore, bNetwork,Environment.MachineName.ToString());
            }
            return false;
        }
        public bool UpdateNetworkDevice(string serialRFID, string IP_Remote, int port_Remote)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.UpdateNetworkDevice(serialRFID,IP_Remote,port_Remote);
                case dbUsedType.db_SqlServer: return sqlServerProvider.UpdateNetworkDevice(serialRFID, IP_Remote, port_Remote);
            }
            return false;
        }
        public bool DeleteDevice(DeviceInfo DeviceToStore)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.DeleteDevice(DeviceToStore);
                case dbUsedType.db_SqlServer: return sqlServerProvider.DeleteDevice(DeviceToStore);
            }
            return false;
        }

        // Product
        public DataTable RecoverAllProduct()
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.RecoverAllProduct();
                case dbUsedType.db_SqlServer: return sqlServerProvider.RecoverAllProduct();
            }
            return null;
        }
        public string getProductTagID(string LotID)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.getProductTagID(LotID);
                case dbUsedType.db_SqlServer: return sqlServerProvider.getProductTagID(LotID);
            }
            return null;
        }
        public string[] RecoverProductInfo(string TagUID)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.RecoverProductInfo(TagUID);
                case dbUsedType.db_SqlServer: return sqlServerProvider.RecoverProductInfo(TagUID);
            }
            return null;
        }
        public ArrayList RecoverProductHistory(string TagUID)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.RecoverProductHistory(TagUID);
                case dbUsedType.db_SqlServer: return sqlServerProvider.RecoverProductHistory(TagUID);
            }
            return null;
        }
        public bool IsProductExist(string tagUID)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.IsProductExist(tagUID);
                case dbUsedType.db_SqlServer: return sqlServerProvider.IsProductExist(tagUID);
            }
            return false;
        }
        public bool StoreProduct(ProductClassTemplate ProductToStore)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.StoreProduct(ProductToStore);
                case dbUsedType.db_SqlServer: return sqlServerProvider.StoreProduct(ProductToStore);
            }
            return false;
        }
        public bool DeleteProduct(string tagUID)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.DeleteProduct(tagUID);
                case dbUsedType.db_SqlServer: return sqlServerProvider.DeleteProduct(tagUID);
            }
            return false;
        }

        public bool AddUidHistory(string _initialUid, string _writtenUid)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.AddUidHistory(_initialUid.Trim(), _writtenUid.Trim());
                case dbUsedType.db_SqlServer: return sqlServerProvider.AddUidHistory(_initialUid.Trim(), _writtenUid.Trim());
            }
            return false;
        }
         public string GetInitialUID(string tagUID)             
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetInitialUID(tagUID.Trim());
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetInitialUID(tagUID.Trim());
            }
            return null;
        }
        public List<UidWriteHistory> GetUidHistory(string _initialUid)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetUidHistory(_initialUid.Trim());
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetUidHistory(_initialUid.Trim());
            }
            return null;
        }

        // Inventory
        public bool IsInventoryExist(InventoryData IDtoStore)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.IsInventoryExist(IDtoStore);
                case dbUsedType.db_SqlServer: return sqlServerProvider.IsInventoryExist(IDtoStore);
            }
            return false;
        }
        //public bool StoreInventory(InventoryData IDtoStore)
       public bool StoreInventory(InventoryData IDtoStore, bool bStoreforDevice = false)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.StoreInventory(IDtoStore,bStoreforDevice);
                case dbUsedType.db_SqlServer: return sqlServerProvider.StoreInventory(IDtoStore);
            }
            return false;
        }
        public int getRowInsertIndex()
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.getRowInsertIndex();
                case dbUsedType.db_SqlServer: return sqlServerProvider.getRowInsertIndex();
            }
            return -1;
        }
        public bool storeTagEvent(InventoryData IDtoStore, int IdScan)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.storeTagEvent(IDtoStore,IdScan);
                case dbUsedType.db_SqlServer: return sqlServerProvider.storeTagEvent(IDtoStore, IdScan);
            }
            return false;
        }
        public InventoryData[] GetInventory(DeviceInfo di, UserClassTemplate uct, int nbData = 100)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetInventory(di,uct,nbData);
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetInventory(di, uct, nbData);
            }
            return null;
        }
        public string[] GetInventoryBefore(string SerialRFID, string DateTimetoParse)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetInventoryBefore(SerialRFID,DateTimetoParse);
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetInventoryBefore(SerialRFID, DateTimetoParse);
            }
            return null;
        }
        public string[] GetInventoryAfter(string SerialRFID, string DateTimetoParse)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetInventoryAfter(SerialRFID,DateTimetoParse);
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetInventoryAfter(SerialRFID, DateTimetoParse);
            }
            return null;
        }
        public string[] GetInventory(string SerialRFID, string DateTimetoParse)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetInventory(SerialRFID,DateTimetoParse);
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetInventory(SerialRFID, DateTimetoParse);
            }
            return null;
        }
        public string[] GetInventoryFromData(string SerialRFID, string spareData1, string spareData2)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetInventoryFromData(SerialRFID, spareData1, spareData2);
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetInventoryFromData(SerialRFID, spareData1, spareData2);
            }
            return null;
        }
        public InventoryData GetLastScan(string serialRFID)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetLastScan(serialRFID);
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetLastScan(serialRFID);
            }
            return null;
        }
        public InventoryData GetLastScanFromID(string serialRFID, int IdEvent)
        {
            switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetLastScanFromID(serialRFID, IdEvent);
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetLastScanFromID(serialRFID, IdEvent);
            }
            return null;
        }
        public TagEventClass[] GetTagEvent(string tagUID)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetTagEvent(tagUID);
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetTagEvent(tagUID);
            }
            return null;
        }
        public TagEventClass GetLastTagEvent(string tagUID)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetLastTagEvent(tagUID);
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetLastTagEvent(tagUID);
            }
            return null;
        }
        public bool DeleteOldInventory(string serialRFID, int NbToKeep)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.DeleteOldInventory(serialRFID,NbToKeep);
                case dbUsedType.db_SqlServer: return sqlServerProvider.DeleteOldInventory(serialRFID, NbToKeep);
            }
            return false;
        }

        // import
        public DataSet ImportCSVToDS(string filename)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.ImportCSVToDS(filename);
                case dbUsedType.db_SqlServer: return sqlServerProvider.ImportCSVToDS(filename);
            }
            return null;
        }
        public DataSet ImportExcelToDS(string filename)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.ImportExcelToDS(filename);
                case dbUsedType.db_SqlServer: return sqlServerProvider.ImportExcelToDS(filename);
            }
            return null;
        }
        public DataSet ImportExcelToDS(string filename, string sheetName)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.ImportExcelToDS(filename,sheetName);
                case dbUsedType.db_SqlServer: return sqlServerProvider.ImportExcelToDS(filename, sheetName);
            }
            return null;
        }


        // stat
        public int GetInventoryCount(string serialRFID = null, string firstName = null, string lastName = null, string dateFrom = null, string dateTo = null)
        {
             switch (useDb)
            {
                case dbUsedType.db_SqlLite: return sqliteProvider.GetInventoryCount(serialRFID,firstName,lastName,dateFrom,dateTo);
                case dbUsedType.db_SqlServer: return sqlServerProvider.GetInventoryCount(serialRFID, firstName, lastName, dateFrom, dateTo);
            }
            return 0;
        }

        
       
    }
}
