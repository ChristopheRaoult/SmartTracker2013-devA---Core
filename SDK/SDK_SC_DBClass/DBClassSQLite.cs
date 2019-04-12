using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data.OleDb; //For import xls
using System.Data;

using System.Configuration;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using Microsoft.Win32;
using System.Text.RegularExpressions;

using System.Xml;
using System.Xml.Xsl;

using DataClass;
using ExcelLibrary.SpreadSheet;


namespace DB_Class_SQLite
{
    public class DBClassSQLite : DBClass.IDBInterface
    {
        private const string dbDefaultEmptyName = "db_ST2013_Empty.db3";
        private const string dbEmptyName = "db_ST2013.db3";
        private const string regKey = @"HKEY_CURRENT_USER\SOFTWARE\SmartTracker_V2";
        private const string regValue = "DB_Path_SQLITE";
        private string _InitialDbPath = null;
        private string ConnectionString;
        private SQLiteConnection myConn = null;

        SQLiteTransaction trans;

        public Hashtable SqlUpdate { get{return sqlUpdate;} }

        #region Update
        public readonly Hashtable sqlUpdate = new Hashtable
        {
              { 1 , "CREATE TABLE IF NOT EXISTS [tb_Version] (Version INTEGER NOT NULL, PRIMARY KEY ([version]))" },
              { 2 , "DROP TABLE IF EXISTS [tb_Product_Def]"},
              { 3 , "CREATE TABLE [tb_Product_Def] ([TagUID] VARCHAR(20)  PRIMARY KEY NULL,[ProductRef] VARCHAR(50)  NULL,[ProductInfo1] VARCHAR(255)  NULL,[ProductInfo2] VARCHAR(255)  NULL,[ProductInfo3] VARCHAR(255)  NULL,[ProductInfo4] VARCHAR(255)  NULL,[ProductInfo5] VARCHAR(255)  NULL,[ProductInfo6] VARCHAR(255)  NULL,[ProductInfo7] VARCHAR(255)  NULL,[ProductInfo8] VARCHAR(255)  NULL,[ProductInfo9] VARCHAR(255)  NULL,[ProductInfo10] VARCHAR(255)  NULL) "},
              { 4 , "DELETE FROM [tb_Tag_Event]"},
              { 5 , "DELETE FROM [tb_Scan_Event]"},
              { 6 , "CREATE TABLE IF NOT EXISTS [tb_Column_Info] ([IdColumn] INTEGER ,[ColumnName] VARCHAR(25)  NULL,[ColumnCSVName] VARCHAR(25)  NULL,[ColumnType] VARCHAR(25)  NULL,PRIMARY KEY ([IdColumn])) "},
              { 7 , "INSERT INTO [tb_Column_Info] ([IdColumn],[ColumnName],[ColumnCSVName],[ColumnType]) VALUES ('0','TagUID','Sr.No.','STRING')"},
              { 8 , "INSERT INTO [tb_Column_Info] ([IdColumn],[ColumnName],[ColumnCSVName],[ColumnType]) VALUES ('1','LotID','Stone ID','STRING')"},
              { 9 , "INSERT INTO [tb_Column_Info] ([IdColumn],[ColumnName],[ColumnCSVName],[ColumnType]) VALUES ('2','Shape','Shape','STRING')"},
              { 10 , "INSERT INTO [tb_Column_Info] ([IdColumn],[ColumnName],[ColumnCSVName],[ColumnType]) VALUES ('3','Weight','Weight','STRING')"},
              { 11 , "INSERT INTO [tb_Column_Info] ([IdColumn],[ColumnName],[ColumnCSVName],[ColumnType]) VALUES ('4','Colour','Colour','STRING')"},
              { 12 , "ALTER TABLE [tb_Column_Info] RENAME TO [tmp_Column_Info]"},
              { 13 , "CREATE TABLE IF NOT EXISTS [tb_Column_Info] ([IdColumn] INTEGER ,[ColumnName] VARCHAR(25)  NULL,[ColumnCSVName] VARCHAR(25)  NULL,[ColumnType] VARCHAR(25)  NULL,PRIMARY KEY ([IdColumn])) "},
              { 14 , "INSERT INTO [tb_Column_Info] ([IdColumn],[ColumnName],[ColumnCSVName],[ColumnType]) SELECT [IdColumn],[ColumnName],[ColumnCSVName],[ColumnType] FROM [tmp_Column_Info]"},
              { 15 , "DROP TABLE IF EXISTS [tmp_Column_Info]"},
              { 16 , "CREATE TABLE IF NOT EXISTS [tb_Product_LotID] ([ProductRef] VARCHAR(50) PRIMARY KEY NULL,[ProductInfo1] VARCHAR(255)  NULL,[ProductInfo2] VARCHAR(255)  NULL,[ProductInfo3] VARCHAR(255)  NULL,[ProductInfo4] VARCHAR(255)  NULL,[ProductInfo5] VARCHAR(255)  NULL,[ProductInfo6] VARCHAR(255)  NULL,[ProductInfo7] VARCHAR(255)  NULL,[ProductInfo8] VARCHAR(255)  NULL,[ProductInfo9] VARCHAR(255)  NULL,[ProductInfo10] VARCHAR(255)  NULL) "},
              { 17 , "CREATE TABLE IF NOT EXISTS [tb_Image] (ImageID INTEGER PRIMARY KEY NULL,[ImageName] VARCHAR(50) NULL,[ImageByteArray] BLOB NULL)"},
              { 18 , "CREATE TABLE IF NOT EXISTS [tb_Image_Link] ([LotIDorTagID] VARCHAR(50) PRIMARY KEY ,ImageName VARCHAR(50) NOT NULL)"},
              { 19 , "DROP TABLE IF EXISTS [tb_Product_LotID]"},
              { 20 , "CREATE TABLE IF NOT EXISTS [tb_Group_Def] ([TagGroup] VARCHAR(20)  PRIMARY KEY NULL,[GroupRef] VARCHAR(50)  NULL,[GroupInfo] VARCHAR(255)  NULL)"},
              { 21 , "CREATE TABLE IF NOT EXISTS [tb_Group_Link] ([IdLink] INTEGER  PRIMARY KEY NULL,[GroupTagRef] VARCHAR(50)  NULL,[ProductTagRef] VARCHAR(50)  NULL)"},
              { 22 , "CREATE TABLE IF NOT EXISTS [tb_Mail_Smtp] ([MailID] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL, [Smtp] VARCHAR(20)  NULL,[Port] INTEGER  NULL,[Sender] VARCHAR(100)  NULL,[Login] VARCHAR(100)  NULL,[Pwd] VARCHAR(100)  NULL)"},
              { 23 , "CREATE TABLE IF NOT EXISTS [tb_Alert_List] ([AlertListID] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL, [AlertName] VARCHAR(50) NULL, [RecipientList] VARCHAR(1024) NULL,[AlertMessage] VARCHAR(2048) NULL ,[MailSubject] VARCHAR(512) NULL , [CCRecipientList] VARCHAR(512) NULL,[BCCRecipientList] VARCHAR(512) NULL)"},
              { 24 , "CREATE TABLE IF NOT EXISTS [tb_Alert_History] ([AlertListID] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,[AlertDate] DATETIME NULL, [SortedAlertDate] VARCHAR NULL, [AlertName] VARCHAR(50) NULL, [SerialRFID] VARCHAR(50) NULL,[FirstName] VARCHAR(50) NULL ,[LastName] VARCHAR(50) NULL,[AlertData] VARCHAR(50) NULL )"}, 
              { 25 , "ALTER TABLE [tb_User_Def] ADD FingerAlert INTEGER DEFAULT -1"},
              { 26 , "ALTER TABLE [tb_User_Def] ADD MaxRemoveItem INTEGER DEFAULT 0"},
              { 27 , "ALTER TABLE [tb_User_Def] ADD MaxRemoveValue FLOAT DEFAULT 0.0"},
              { 28 , "ALTER TABLE [tb_Column_Info] ADD AlertLink VARCHAR(50) DEFAULT 'NONE'"},
              { 29 , "ALTER TABLE [tb_Alert_List] ADD bActive INTEGER DEFAULT 1"},
              { 30,  "INSERT INTO [tb_Alert_List] ([AlertName],bActive) VALUES ('AT_Power_Cut',0)" },
              { 31,  "INSERT INTO [tb_Alert_List] ([AlertName],bActive) VALUES ('AT_Usb_Unplug',0)" },
              { 32,  "INSERT INTO [tb_Alert_List] ([AlertName],bActive) VALUES ('AT_Door_Open_Too_Long',0)" },
              { 33,  "INSERT INTO [tb_Alert_List] ([AlertName],bActive) VALUES ('AT_Finger_Alert',0)" },
              { 34,  "INSERT INTO [tb_Alert_List] ([AlertName],bActive) VALUES ('AT_Remove_Too_Many_Items',0)" },
              { 35,  "INSERT INTO [tb_Alert_List] ([AlertName],bActive) VALUES ('AT_Limit_Value_Exceed',0)" },  
              { 36,  "ALTER TABLE [tb_Mail_Smtp] ADD bUseSSL INTEGER DEFAULT 0"}, 
              { 37,  "INSERT INTO [tb_Alert_List] ([AlertName],bActive) VALUES ('AT_Move_Sensor',0)" }, 
              { 38,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo11] VARCHAR(255)  NULL"}, 
              { 39,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo12] VARCHAR(255)  NULL"}, 
              { 40,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo13] VARCHAR(255)  NULL"}, 
              { 41,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo14] VARCHAR(255)  NULL"}, 
              { 42,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo15] VARCHAR(255)  NULL"}, 
              { 43,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo16] VARCHAR(255)  NULL"}, 
              { 44,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo17] VARCHAR(255)  NULL"}, 
              { 45,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo18] VARCHAR(255)  NULL"}, 
              { 46,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo19] VARCHAR(255)  NULL"}, 
              { 47,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo20] VARCHAR(255)  NULL"}, 
              { 48,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo21] VARCHAR(255)  NULL"}, 
              { 49,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo22] VARCHAR(255)  NULL"}, 
              { 50,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo23] VARCHAR(255)  NULL"}, 
              { 51,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo24] VARCHAR(255)  NULL"}, 
              { 52,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo25] VARCHAR(255)  NULL"}, 
              { 53,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo26] VARCHAR(255)  NULL"}, 
              { 54,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo27] VARCHAR(255)  NULL"}, 
              { 55,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo28] VARCHAR(255)  NULL"}, 
              { 56,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo29] VARCHAR(255)  NULL"}, 
              { 57,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo30] VARCHAR(255)  NULL"},
              { 58,  "ALTER TABLE [tb_Mail_Smtp] ADD bActive INTEGER DEFAULT 0"}, 
              { 59 , "DELETE FROM [tb_Mail_Smtp]"},
              { 60 , "INSERT INTO [tb_Mail_Smtp] (Smtp,Port,Sender,Login,Pwd,bUseSSL,bActive) VALUES('smtp.gmail.com',587,'spacecode.rfid@gmail.com','spacecode.rfid@gmail.com','rfid123456',1,0)"},
              { 61 , "DELETE FROM [tb_Alert_List]"},
              { 62,  "INSERT INTO [tb_Alert_List] ([AlertName],[AlertMessage],[MailSubject],bActive) VALUES ('AT_Power_Cut','A power cut alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE]','POWER CUT ALERT ON [READERNAME]',0)" },
              { 63,  "INSERT INTO [tb_Alert_List] ([AlertName],[AlertMessage],[MailSubject],bActive) VALUES ('AT_Usb_Unplug','A usb cable unplug alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE]','USB CABLE UNPLUG ALERT ON [READERNAME]',0)" },
              { 64,  "INSERT INTO [tb_Alert_List] ([AlertName],[AlertMessage],[MailSubject],bActive) VALUES ('AT_Door_Open_Too_Long','A door opened too long alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE] from user [USERNAME]','DOOR OPEN TOO LONG ALERT ON [READERNAME]',0)" },
              { 65,  "INSERT INTO [tb_Alert_List] ([AlertName],[AlertMessage],[MailSubject],bActive) VALUES ('AT_Finger_Alert','A finger alert alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE] from user [USERNAME]','FINGER ALERT ALERT ON [READERNAME]',0)" },
              { 66,  "INSERT INTO [tb_Alert_List] ([AlertName],[AlertMessage],[MailSubject],bActive) VALUES ('AT_Remove_Too_Many_Items','A remove too many items in number alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE] from user [USERNAME]','REMOVE TOO MANY ITEMS ALERT ON [READERNAME]',0)" },
              { 67,  "INSERT INTO [tb_Alert_List] ([AlertName],[AlertMessage],[MailSubject],bActive) VALUES ('AT_Limit_Value_Exceed','A remove too many items in value alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE] from user [USERNAME]','REMOVE TOO MANY VALUE ALERT ON [READERNAME]',0)" },
              { 68,  "INSERT INTO [tb_Alert_List] ([AlertName],[AlertMessage],[MailSubject],bActive) VALUES ('AT_Move_Sensor','A move sensor alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE]','MOVE SENSOR ALERT ON [READERNAME]',0)" }, 
              { 69,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo31] VARCHAR(255)  NULL"},
              { 70,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo32] VARCHAR(255)  NULL"},
              { 71,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo33] VARCHAR(255)  NULL"},
              { 72,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo34] VARCHAR(255)  NULL"},
              { 73,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo35] VARCHAR(255)  NULL"},
              { 74,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo36] VARCHAR(255)  NULL"},
              { 75,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo37] VARCHAR(255)  NULL"},
              { 76,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo38] VARCHAR(255)  NULL"},
              { 77,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo39] VARCHAR(255)  NULL"},
              { 78,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo40] VARCHAR(255)  NULL"},
              { 79,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo41] VARCHAR(255)  NULL"},
              { 80,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo42] VARCHAR(255)  NULL"},
              { 81,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo43] VARCHAR(255)  NULL"},
              { 82,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo44] VARCHAR(255)  NULL"},
              { 83,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo45] VARCHAR(255)  NULL"},
              { 84,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo46] VARCHAR(255)  NULL"},
              { 85,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo47] VARCHAR(255)  NULL"},
              { 86,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo48] VARCHAR(255)  NULL"},
              { 87,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo49] VARCHAR(255)  NULL"},
              { 88,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo50] VARCHAR(255)  NULL"}, 
              { 89,  "ALTER TABLE [tb_Column_Info] ADD [ColumnToExport] INTEGER DEFAULT 1"}, 
              { 90,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo51] VARCHAR(255)  NULL"},
              { 91,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo52] VARCHAR(255)  NULL"},
              { 92,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo53] VARCHAR(255)  NULL"},
              { 93,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo54] VARCHAR(255)  NULL"},
              { 94,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo55] VARCHAR(255)  NULL"},
              { 95,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo56] VARCHAR(255)  NULL"},
              { 96,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo57] VARCHAR(255)  NULL"},
              { 97,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo58] VARCHAR(255)  NULL"},
              { 98,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo59] VARCHAR(255)  NULL"},
              { 99,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo60] VARCHAR(255)  NULL"},
              { 100,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo61] VARCHAR(255)  NULL"}, 
              { 101,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo62] VARCHAR(255)  NULL"},
              { 102,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo63] VARCHAR(255)  NULL"},
              { 103,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo64] VARCHAR(255)  NULL"},
              { 104,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo65] VARCHAR(255)  NULL"},
              { 105,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo66] VARCHAR(255)  NULL"},
              { 106,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo67] VARCHAR(255)  NULL"},
              { 107,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo68] VARCHAR(255)  NULL"},
              { 108,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo69] VARCHAR(255)  NULL"},
              { 109,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo70] VARCHAR(255)  NULL"},
              { 110,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo71] VARCHAR(255)  NULL"},
              { 111,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo72] VARCHAR(255)  NULL"}, 
              { 112,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo73] VARCHAR(255)  NULL"},
              { 113,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo74] VARCHAR(255)  NULL"},
              { 114,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo75] VARCHAR(255)  NULL"},
              { 115,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo76] VARCHAR(255)  NULL"},
              { 116,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo77] VARCHAR(255)  NULL"},
              { 117,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo78] VARCHAR(255)  NULL"},
              { 118,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo79] VARCHAR(255)  NULL"},
              { 119,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo80] VARCHAR(255)  NULL"},
              { 120,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo81] VARCHAR(255)  NULL"},
              { 121,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo82] VARCHAR(255)  NULL"},
              { 122,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo83] VARCHAR(255)  NULL"}, 
              { 123,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo84] VARCHAR(255)  NULL"},
              { 124,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo85] VARCHAR(255)  NULL"},
              { 125,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo86] VARCHAR(255)  NULL"},
              { 126,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo87] VARCHAR(255)  NULL"},
              { 127,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo88] VARCHAR(255)  NULL"},
              { 128,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo89] VARCHAR(255)  NULL"},
              { 129,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo90] VARCHAR(255)  NULL"},
              { 130,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo91] VARCHAR(255)  NULL"},
              { 131,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo92] VARCHAR(255)  NULL"},
              { 132,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo93] VARCHAR(255)  NULL"},
              { 133,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo94] VARCHAR(255)  NULL"}, 
              { 134,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo95] VARCHAR(255)  NULL"},
              { 135,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo96] VARCHAR(255)  NULL"},
              { 136,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo97] VARCHAR(255)  NULL"},
              { 137,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo98] VARCHAR(255)  NULL"},
              { 138,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo99] VARCHAR(255)  NULL"},
              { 139,  "ALTER TABLE [tb_Product_Def] ADD [ProductInfo100] VARCHAR(255)  NULL"},
              { 140,  "ALTER TABLE [tb_Device_Def] ADD [comTempReader] VARCHAR(10)  NULL"},   
              { 141,  "CREATE TABLE [tb_Fridge_Temp] ([IdTemp] INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL , [SerialRFID] VARCHAR(20) NOT NULL , [Event_Date] DATETIME, [Sorted_Event_Date] VARCHAR(50), [Mean_Value] DOUBLE, [Max_Value] DOUBLE, [Min_Value] DOUBLE, [Nb_Point] INTEGER, [TempStream] MEMO)"},
              { 142,  "CREATE TABLE [tb_Product_History] ([EnterDate] DATETIME,[SortedEnterDate] VARCHAR, [TagUID] VARCHAR(20)  NULL,[ProductRef] VARCHAR(50)  NULL,[ProductInfo1] VARCHAR(255)  NULL,[ProductInfo2] VARCHAR(255)  NULL,[ProductInfo3] VARCHAR(255)  NULL,[ProductInfo4] VARCHAR(255)  NULL,[ProductInfo5] VARCHAR(255)  NULL,[ProductInfo6] VARCHAR(255)  NULL,[ProductInfo7] VARCHAR(255)  NULL,[ProductInfo8] VARCHAR(255)  NULL,[ProductInfo9] VARCHAR(255)  NULL,[ProductInfo10] VARCHAR(255)  NULL, [ProductInfo11] VARCHAR(255)  NULL, [ProductInfo12] VARCHAR(255)  NULL, [ProductInfo13] VARCHAR(255)  NULL, [ProductInfo14] VARCHAR(255)  NULL, [ProductInfo15] VARCHAR(255)  NULL, [ProductInfo16] VARCHAR(255)  NULL, [ProductInfo17] VARCHAR(255)  NULL, [ProductInfo18] VARCHAR(255)  NULL, [ProductInfo19] VARCHAR(255)  NULL, [ProductInfo20] VARCHAR(255)  NULL, [ProductInfo21] VARCHAR(255)  NULL, [ProductInfo22] VARCHAR(255)  NULL, [ProductInfo23] VARCHAR(255)  NULL, [ProductInfo24] VARCHAR(255)  NULL, [ProductInfo25] VARCHAR(255)  NULL, [ProductInfo26] VARCHAR(255)  NULL, [ProductInfo27] VARCHAR(255)  NULL, [ProductInfo28] VARCHAR(255)  NULL, [ProductInfo29] VARCHAR(255)  NULL, [ProductInfo30] VARCHAR(255)  NULL, [ProductInfo31] VARCHAR(255)  NULL, [ProductInfo32] VARCHAR(255)  NULL, [ProductInfo33] VARCHAR(255)  NULL, [ProductInfo34] VARCHAR(255)  NULL, [ProductInfo35] VARCHAR(255)  NULL, [ProductInfo36] VARCHAR(255)  NULL, [ProductInfo37] VARCHAR(255)  NULL, [ProductInfo38] VARCHAR(255)  NULL, [ProductInfo39] VARCHAR(255)  NULL, [ProductInfo40] VARCHAR(255)  NULL, [ProductInfo41] VARCHAR(255)  NULL, [ProductInfo42] VARCHAR(255)  NULL, [ProductInfo43] VARCHAR(255)  NULL, [ProductInfo44] VARCHAR(255)  NULL, [ProductInfo45] VARCHAR(255)  NULL, [ProductInfo46] VARCHAR(255)  NULL, [ProductInfo47] VARCHAR(255)  NULL, [ProductInfo48] VARCHAR(255)  NULL, [ProductInfo49] VARCHAR(255)  NULL, [ProductInfo50] VARCHAR(255)  NULL, [ProductInfo51] VARCHAR(255)  NULL, [ProductInfo52] VARCHAR(255)  NULL, [ProductInfo53] VARCHAR(255)  NULL, [ProductInfo54] VARCHAR(255)  NULL, [ProductInfo55] VARCHAR(255)  NULL, [ProductInfo56] VARCHAR(255)  NULL, [ProductInfo57] VARCHAR(255)  NULL, [ProductInfo58] VARCHAR(255)  NULL, [ProductInfo59] VARCHAR(255)  NULL, [ProductInfo60] VARCHAR(255)  NULL, [ProductInfo61] VARCHAR(255)  NULL, [ProductInfo62] VARCHAR(255)  NULL, [ProductInfo63] VARCHAR(255)  NULL, [ProductInfo64] VARCHAR(255)  NULL, [ProductInfo65] VARCHAR(255)  NULL, [ProductInfo66] VARCHAR(255)  NULL, [ProductInfo67] VARCHAR(255)  NULL, [ProductInfo68] VARCHAR(255)  NULL, [ProductInfo69] VARCHAR(255)  NULL, [ProductInfo70] VARCHAR(255)  NULL, [ProductInfo71] VARCHAR(255)  NULL, [ProductInfo72] VARCHAR(255)  NULL, [ProductInfo73] VARCHAR(255)  NULL, [ProductInfo74] VARCHAR(255)  NULL, [ProductInfo75] VARCHAR(255)  NULL, [ProductInfo76] VARCHAR(255)  NULL, [ProductInfo77] VARCHAR(255)  NULL, [ProductInfo78] VARCHAR(255)  NULL, [ProductInfo79] VARCHAR(255)  NULL, [ProductInfo80] VARCHAR(255)  NULL, [ProductInfo81] VARCHAR(255)  NULL, [ProductInfo82] VARCHAR(255)  NULL, [ProductInfo83] VARCHAR(255)  NULL, [ProductInfo84] VARCHAR(255)  NULL, [ProductInfo85] VARCHAR(255)  NULL, [ProductInfo86] VARCHAR(255)  NULL, [ProductInfo87] VARCHAR(255)  NULL, [ProductInfo88] VARCHAR(255)  NULL, [ProductInfo89] VARCHAR(255)  NULL, [ProductInfo90] VARCHAR(255)  NULL, [ProductInfo91] VARCHAR(255)  NULL, [ProductInfo92] VARCHAR(255)  NULL, [ProductInfo93] VARCHAR(255)  NULL, [ProductInfo94] VARCHAR(255)  NULL, [ProductInfo95] VARCHAR(255)  NULL, [ProductInfo96] VARCHAR(255)  NULL, [ProductInfo97] VARCHAR(255)  NULL, [ProductInfo98] VARCHAR(255)  NULL, [ProductInfo99] VARCHAR(255)  NULL, [ProductInfo100] VARCHAR(255)  NULL)"},
              { 143,  "ALTER TABLE [tb_Alert_List] ADD [AlertData] VARCHAR(255)  NULL"},   
              { 144,  "INSERT INTO [tb_Alert_List] ([AlertName],[AlertMessage],[MailSubject],[bActive],[AlertData]) VALUES ('AT_Max_Fridge_Temp','A max temperature alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE]','MAX TEMPERATURE ALERT ON [READERNAME]',0,'10')" }, 
              { 145,  "UPDATE [tb_Alert_List] SET [AlertData]='30' WHERE [AlertName]='AT_Door_Open_Too_Long'" }, 
              { 146,  "INSERT INTO [tb_Alert_List] ([AlertName],[AlertMessage],[MailSubject],[bActive]) VALUES ('AT_Remove_Tag_Max_Time','A remove tag max allowed time alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE]','MAX TEMPERATURE ALERT ON [READERNAME]',0)" }, 
              { 147,  "ALTER TABLE [tb_Device_Def] ADD [comMasterReader] VARCHAR(10)  NULL"},
              { 148,  "ALTER TABLE [tb_Device_Def] ADD [comSlaveReader] VARCHAR(10)  NULL"},  
              { 149,  "ALTER TABLE [tb_Column_Info] ADD [DoSum] INTEGER DEFAULT 0"}, 
              { 150,  "CREATE TABLE [tb_Formula] ([IdFormule] INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL , [Title] VARCHAR(255)  , [Formulae] VARCHAR(255), [Enable] INTEGER DEFAULT 0 )"}, 
              { 151,  "ALTER TABLE [tb_Group_Def] ADD [Criteria] VARCHAR(255)  NULL"},  
              { 152,  "INSERT INTO [tb_Alert_List] ([AlertName],[AlertMessage],[MailSubject],bActive) VALUES ('AT_Stock_Limit','A limit stock alert has occurs on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE]','STOCK LIMIT ALERT ON [READERNAME]',0)" }, 
              { 153,  "ALTER TABLE [tb_Device_Def] ADD [accessReaderType] INTEGER DEFAULT 1"}, 
              { 154,  "ALTER TABLE [tb_Device_Def] ADD [fridgeType] INTEGER DEFAULT 0"}, 
              { 155,  "INSERT INTO [tb_Alert_List] ([AlertName],[AlertMessage],[MailSubject],bActive) VALUES ('AT_Bad_Blood_Patient','Bad blood bags removed for patient alert has occurs on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE]','BAD BLOOD BAG REMOVED FOR PATIENT ALERT ON [READERNAME]',0)" }, 
              { 156 , "CREATE TABLE IF NOT EXISTS [tb_SqlServer_Info] ([ConnectionString] VARCHAR(1024)  NULL,[bUseSqlServer] INTEGER DEFAULT 0) "},
              { 157,  "ALTER TABLE [tb_User_Grant] ADD [DoorGranted] INTEGER DEFAULT 3"}, 
              { 158 , "CREATE TABLE IF NOT EXISTS [tb_Export_Info] ([exportType] INTEGER ,[spareData1] VARCHAR(1024)  NULL, [spareData2] VARCHAR(1024)  NULL, bActive INTEGER DEFAULT 0) "},
              { 159,  "ALTER TABLE [tb_Scan_Event] ADD [SpareData1] VARCHAR(1024)  NULL"}, 
              { 160,  "ALTER TABLE [tb_Scan_Event] ADD [SpareData2] VARCHAR(1024)  NULL"}, 
              { 161 , "CREATE TABLE [tbo_TempFridge] ([IdTemp] INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL ,[TemperatureDate] VARCHAR(20)  NOT NULL,[SerialRFID] VARCHAR(25) NULL,[TemperatureBottle] FLOAT,[TemperatureChamber] FLOAT,[bFridgeOK] INTEGER DEFAULT 0)"},
              { 162,  "CREATE TABLE [tbo_Daily_TempReport_History] ([ReportDate] VARCHAR(20) NOT NULL)"},   
              { 163 , "CREATE TABLE [tb_UidHistory] ([IdWrite] INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL, [InitialUid] VARCHAR(20) , [WrittenUid] VARCHAR(20) , [WrittenDate] VARCHAR(20))"} 
        };

        #endregion

        public DBClassSQLite()
        {   
            this._InitialDbPath = Convert.ToString(Registry.GetValue(regKey, regValue, null));

            if (string.IsNullOrEmpty(this._InitialDbPath))
            {

                ProcessDBPath();
            }
            else
            {   
               if (!isValidDBPath()) ProcessDBPath();
                
            }

            this._InitialDbPath = Convert.ToString(Registry.GetValue(regKey, regValue, null));
            SQLiteConnectionStringBuilder SQLCSB = new SQLiteConnectionStringBuilder();
            SQLCSB.DataSource = this._InitialDbPath;
            ConnectionString = SQLCSB.ToString();           
        }
        public DBClassSQLite(string sqliteDb)
        {
            SQLiteConnectionStringBuilder connectionSb = new SQLiteConnectionStringBuilder();
            connectionSb.DataSource = sqliteDb;
            connectionSb.Version = 3;
            ConnectionString = connectionSb.ToString();
        }
        #region TestDB
        public bool isValidDBPath()
        {
            this._InitialDbPath = Convert.ToString(Registry.GetValue(regKey, regValue, null));
            if (string.IsNullOrEmpty(this._InitialDbPath))
            {
                ProcessDBPath();
                this._InitialDbPath = Convert.ToString(Registry.GetValue(regKey, regValue, null));
            }
            if (!Path.GetExtension(this._InitialDbPath).Equals(".db3")) return false;
            else if (!File.Exists(this._InitialDbPath))  return false;
            else return true;
        }
        private void ProcessDBPath()
        {
            try
            {
                Registry.SetValue(regKey, regValue, "");
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = Application.StartupPath + "\\DB"; ;
                openFileDialog.Filter = "Fichiers SQLITE 3 (*.db3)|*.db3";
                openFileDialog.Title = "Open the SQLITE Database";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string FileName = openFileDialog.FileName;

                    if (FileName.Contains(dbDefaultEmptyName))
                    {
                        // base empty - so copy to db with no empty in name and set rregistry
                        string newpath = FileName.Replace(dbDefaultEmptyName, dbEmptyName);
                        File.Copy(FileName, newpath);
                        Registry.SetValue(regKey, regValue, newpath);
                    }
                    else
                    {
                        // not empty db - set registry
                        Registry.SetValue(regKey, regValue, FileName);
                    }
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

        }
        public bool isTableExist(string table_name)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT name FROM sqlite_master ";
                sqlQuery += "WHERE name='" + table_name + "' ";
               
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();
                ret = rd.HasRows;
                rd.Close();
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }
        public bool isTableVersionExist()
        {
            return isTableExist("tb_Version");
        }
        public int getVersion()
        {
            int ret = -1;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT version FROM tb_Version ";               

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    rd.Read();
                    ret = int.Parse(rd["Version"].ToString());
                }
                rd.Close();
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }
        public bool setVersion(int version)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "DELETE FROM tb_Version ";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.ExecuteNonQuery();

                sqlQuery = null;
                sqlQuery += "INSERT INTO tb_Version(Version) VALUES (" + version + ")";
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.ExecuteNonQuery();
                cmd.Dispose();
                ret = true;
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
            
        }
        public bool setUpdate(int updateNumber)
        {
             bool ret = false;
             try
             {
                string sqlQuery = null;
                sqlQuery += sqlUpdate[updateNumber];
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();
                setVersion(updateNumber);
                ret = true;
             }
             catch (Exception exp)
             {
                 // On affiche l'erreur.
                 ErrorMessage.ExceptionMessageBox.Show(exp);
             }
             return ret;

        }
        public ArrayList GetColumnToExport()
        {
            ArrayList ColumnToExport = null;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Column_Info WHERE ColumnToExport=1";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    ColumnToExport = new ArrayList();
                    while (rd.Read())
                    {                       
                        string colName = rd["ColumnName"].ToString();
                        ColumnToExport.Add(colName);
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ColumnToExport;
        }
        public Hashtable GetColumnInfo()
        {
            Hashtable ColumnInfo = null;

             try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Column_Info ";               

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    ColumnInfo = new Hashtable();                   
                    while (rd.Read())
                    {
                        int Index = int.Parse(rd["IdColumn"].ToString());
                        string colName = rd["ColumnName"].ToString();
                        ColumnInfo.Add(Index, colName);
                    }
                    rd.Close();
                    cmd.Dispose();
                }

            }
             catch (Exception exp)
             {
                 // On affiche l'erreur.
                 ErrorMessage.ExceptionMessageBox.Show(exp);
             }
            return ColumnInfo;
        }
        public dtColumnInfo[] GetdtColumnInfo()
        {
            dtColumnInfo[] colArray = null;

            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Column_Info ";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                ArrayList listtmp = new ArrayList();

                if (rd.HasRows)
                {

                    while (rd.Read())
                    {
                        dtColumnInfo colInfo = new dtColumnInfo();
                        colInfo.colIndex = int.Parse(rd["IdColumn"].ToString());
                        colInfo.colName = rd["ColumnName"].ToString();

                        switch (rd["ColumnType"].ToString().ToUpper())
                        {
                            case "STRING": colInfo.colType = typeof(string); break;
                            case "INT": colInfo.colType = typeof(int); break;
                            case "UINT64": colInfo.colType = typeof(UInt64); break;
                            case "DOUBLE": colInfo.colType = typeof(double); break;
                            case "DATETIME": colInfo.colType = typeof(DateTime); break;
                            case "IMAGE": colInfo.colType = typeof(byte[]); break;

                            default: colInfo.colType = typeof(string); break;
                        }
                        colInfo.colDoSum = int.Parse(rd["DoSum"].ToString());
                        listtmp.Add(colInfo);
                    }

                    if (listtmp.Count > 0)
                    {
                        colArray = new dtColumnInfo[listtmp.Count];
                        for (int i = 0; i < listtmp.Count; i++)
                            colArray[i] = (dtColumnInfo)listtmp[i];
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }


            return colArray;
        }
        
        public Hashtable GetColumnCSVInfo()
        {
            Hashtable ColumnCSVInfo = null;

            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Column_Info ";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    ColumnCSVInfo = new Hashtable();
                    while (rd.Read())
                    {
                        int Index = int.Parse(rd["IdColumn"].ToString());
                        string colName = rd["ColumnCSVName"].ToString();
                        ColumnCSVInfo.Add(Index, colName);
                    }
                    rd.Close();
                    cmd.Dispose();
                }

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ColumnCSVInfo;
        }

        public Object GetColumn()
        {
            SQLiteDataAdapter da = null;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                {
                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tb_Column_Info;";
                    da = new SQLiteDataAdapter(sqlQuery, myConn);
                   
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return da;
        }

        public bool cleanDB(bool bDeleteColum = false)
        {
            bool ret = false;
            string sqlQuery;
           SQLiteCommand cmd;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                if (bDeleteColum)
                {
                    sqlQuery = null;
                    sqlQuery += "DELETE FROM tb_Column_Info ";

                    cmd = myConn.CreateCommand();
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();
                }

                sqlQuery = null;
                sqlQuery += "DELETE FROM tb_Product_Def ";

                cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.ExecuteNonQuery();

                sqlQuery = null;
                sqlQuery += "DELETE FROM tb_Tag_Event ";

                cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.ExecuteNonQuery();

                sqlQuery = null;
                sqlQuery += "DELETE FROM tb_Scan_Event ";

                cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }



        #region Formule

        public FormuleData getFormuleInfo()
        {
            FormuleData retFormule = null;
            
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Formula;";
                   

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    rd.Read();
                    retFormule = new FormuleData();

                    retFormule.Title = (string)rd["Title"];
                    retFormule.Formule = (string)rd["Formulae"];
                    retFormule.Enable = Convert.ToInt32(rd["Enable"].ToString());
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }            
            return retFormule;
        }

        public bool setFormuleInfo(FormuleData fd)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "DELETE FROM tb_Formula;";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.ExecuteNonQuery();


                sqlQuery = string.Empty;
                
                sqlQuery += "INSERT INTO tb_Formula(Title,Formulae,Enable) VALUES('";
                sqlQuery += fd.Title + "','";
                sqlQuery += fd.Formule + "',";              
                sqlQuery += fd.Enable.ToString() + ");";             

                
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }

        #endregion
        #endregion
        ~DBClassSQLite()
        {
            CloseDB();
        }

        public bool OpenDB()
        {
            bool ret = false;
            try
            {
                myConn = new SQLiteConnection(ConnectionString);
                myConn.Open();                
                ret = true;
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);

            }
            return ret;
        }

        private void ExecuteUpdateScript()
        {
            int currentVersion = getVersion();
            if (currentVersion < SqlUpdate.Count)
            {
                for (int i = (currentVersion + 1); i < SqlUpdate.Count + 1; ++i)
                    setUpdate(i);
            }
        }

        public void Connect()
        {
            if (myConn != null && myConn.State == ConnectionState.Open) return;
            myConn = new SQLiteConnection(ConnectionString);
            myConn.Open();

            // check if updates are available and execute corresponding script
            ExecuteUpdateScript();
        }

        public void Close()
        {
            if (myConn == null || myConn.State == System.Data.ConnectionState.Closed) return;
            myConn.Close();
        }

        public void CloseDB()
        {
            try
            {
                if (myConn != null)
                {
                    if (myConn.State != System.Data.ConnectionState.Closed)
                        myConn.Close();
                }
                myConn = null;
                GC.Collect();
            }
            catch
            {

            }
        }
        public bool isOpen()
        {
            if (myConn.State == System.Data.ConnectionState.Open) return true;
            else return false;
        }
        public void startTranscation()
        {
            try
            {
                trans = myConn.BeginTransaction();
            }
            catch
            {
            }
        }
        public void endTranscation()
        {
            try{
            trans.Commit();
            }
            catch
            {
            }
        }
        #region SQL
        public bool AddSqlServerInfo(string ConnectionString, int bUseSqlServer)
        {
            bool ret = false;
            try
            {
                if (myConn == null) return false;
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "DELETE FROM tb_SqlServer_Info ";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();

                sqlQuery = string.Empty;
                sqlQuery += "INSERT INTO [tb_SqlServer_Info] (ConnectionString,bUseSqlServer) VALUES('";
                sqlQuery += ConnectionString + "',";
                sqlQuery += bUseSqlServer + ");";

                SQLiteCommand cmd2 = myConn.CreateCommand();
                cmd2.CommandText = sqlQuery;
                cmd2.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd2.ExecuteNonQuery();
                if (rowAffected > 0) ret = true;
                cmd.Dispose();
                cmd2.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }
        public bool getSqlServerInfo(out string ConnectionString, out int bUseSqlServer)
        {
            bool ret = false;
            ConnectionString = null;
            bUseSqlServer = 0;
            try
            {
                if (myConn == null) return false;
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM [tb_SqlServer_Info]";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    rd.Read();
                    {
                        ConnectionString = rd.IsDBNull(rd.GetOrdinal("ConnectionString")) ? string.Empty : (string)rd["ConnectionString"];
                        bUseSqlServer = Convert.ToInt32(Convert.ToInt32(rd["bUseSqlServer"].ToString()));
                    }
                    rd.Close();
                    cmd.Dispose();
                }

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }
        public bool AddExportInfo(int exportType , string spareData1 ,string spareData2, int bActive )
        {
            bool ret = false;
            try
            {
                if (myConn == null) return false;
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "DELETE FROM [tb_Export_Info] ";
                sqlQuery += "WHERE exportType=" + exportType.ToString();

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();

                sqlQuery = string.Empty;
                sqlQuery += "INSERT INTO [tb_Export_Info](exportType,spareData1,spareData2,bActive) VALUES(";
                sqlQuery += exportType.ToString() + ",'";
                sqlQuery += spareData1 + "','";
                sqlQuery += spareData2 + "',";
                sqlQuery += bActive + ");";

                SQLiteCommand cmd2 = myConn.CreateCommand();
                cmd2.CommandText = sqlQuery;
                cmd2.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd2.ExecuteNonQuery();
                if (rowAffected > 0) ret = true;
                cmd.Dispose();
                cmd2.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }
        public bool getExportInfo(int exportType, out string spareData1, out string spareData2, out int bActive)
        {
            bool ret = false;
            spareData1 = null;
            spareData2 = null;
            bActive = 0;
           
            try
            {
                if (myConn == null) return false;
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT spareData1,spareData2,bActive FROM [tb_Export_Info] ";
                sqlQuery += "WHERE exportType=" + exportType + ";";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    rd.Read();
                    {
                        spareData1 = rd.IsDBNull(rd.GetOrdinal("spareData1")) ? string.Empty : (string)rd["spareData1"];
                        spareData2 = rd.IsDBNull(rd.GetOrdinal("spareData2")) ? string.Empty : (string)rd["spareData2"];
                        bActive = Convert.ToInt32(Convert.ToInt32(rd["bActive"].ToString()));
                    }
                    rd.Close();
                    cmd.Dispose();
                    ret = true;
                }

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }

        public bool setSqlExportOnOff(int exportType, bool bOnOff)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();            

                string sqlQuery = null;
                sqlQuery += "UPDATE [tb_Export_Info] ";
                if (bOnOff) sqlQuery += "SET bActive=1 ";
                else sqlQuery += "SET bActive=0 ";
                sqlQuery += "WHERE exportType=" + exportType + ";";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }

        #endregion
        #region Fridge

        public bool StoreTempFridge(string SerialRFID , tempInfo tempInfoToStore)
        {
            bool ret = false;

            try
            {    
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream mem = new MemoryStream();
                bf.Serialize(mem, tempInfoToStore);
                string idStream = Convert.ToBase64String(mem.ToArray());
                if (myConn == null) return false;
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;

                sqlQuery += "INSERT INTO tb_Fridge_Temp(SerialRFID,Event_Date,Sorted_Event_Date,Mean_Value,Max_Value,Min_Value,Nb_Point,TempStream) VALUES('";
                sqlQuery += SerialRFID + "','";
                sqlQuery += tempInfoToStore.CreationDate.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + "','";
                sqlQuery += tempInfoToStore.CreationDate.ToString("s") + "','";
                sqlQuery += tempInfoToStore.mean.ToString("0.00",System.Globalization.CultureInfo.InvariantCulture) + "','";
                sqlQuery += tempInfoToStore.max.ToString("0.00",System.Globalization.CultureInfo.InvariantCulture) + "','";
                sqlQuery += tempInfoToStore.min.ToString("0.00",System.Globalization.CultureInfo.InvariantCulture) + "','";
                sqlQuery += tempInfoToStore.nbValueTemp + "','";
                sqlQuery += idStream + "');";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;

        }
        public tempInfo GetLastFridgeTemp(string serialRFID)
        {
            tempInfo retData = null;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();  

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Fridge_Temp ";
                sqlQuery += "WHERE SerialRFID='" + serialRFID + "' ";
                sqlQuery += "ORDER BY IdTemp DESC LIMIT 1";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    rd.Read();
                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream mem = new MemoryStream(Convert.FromBase64String((string)rd["TempStream"]));
                    retData = (tempInfo)bf.Deserialize(mem); 
                    rd.Close();
                    cmd.Dispose();
                }

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.

                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return retData;
        }
        public bool GetLastFridgeTempfromxHours(string serialRFID, int nbHour, out ArrayList tempChamber, out ArrayList tempBottle)
        {
            bool ret = false;
            tempInfo retData = null;
            tempChamber = null;
            tempBottle = null;
          
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tbo_Fridge_Temp ";
                sqlQuery += "WHERE SerialRFID='" + serialRFID + "' ";
                sqlQuery += "ORDER BY IdTemp DESC LIMIT " + nbHour.ToString();

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    tempChamber = new ArrayList();
                    tempBottle = new ArrayList();
                    ArrayList listClasstemp = new ArrayList();
                    while (rd.Read())
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream mem = new MemoryStream(Convert.FromBase64String((string)rd["TempStream"].ToString().Trim()));
                        retData = (tempInfo)bf.Deserialize(mem);
                        listClasstemp.Insert(0, retData);
                    }

                    foreach (tempInfo ti in listClasstemp)
                    {
                        if (ti.tempChamber != null)
                            foreach (KeyValuePair<int, double> pair in ti.tempChamber)
                            {
                                tempChamber.Insert(0, pair.Value);
                            }
                        if (ti.tempBottle != null)
                            foreach (KeyValuePair<int, double> pair in ti.tempBottle)
                            {
                                tempBottle.Insert(0, pair.Value);
                            }
                    }
                }
                ret = true;
                if (rd != null) rd.Close();
                if (cmd != null) cmd.Dispose();

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }            
            return ret;
        }
        /*public string[] GetFridgeTempAfter(string serialRFID, string DateTimetoParse)
        {
            ArrayList retData = new ArrayList(); ;
            string[] strData = null;
            try
            {
                //DateTime dt = DateTime.Parse(DateTimetoParse);
                DateTime dt = DateTime.ParseExact(DateTimetoParse, "yyyy-MM-dd HH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);

                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT TempStream FROM tb_Fridge_Temp WHERE 1=1 ";
                sqlQuery += "AND SerialRFID='" + serialRFID + "' ";
                sqlQuery += "AND Sorted_Event_Date>'" + dt.ToString("s") + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        retData.Add(rd[0].ToString());
                    }
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();

                }

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.

                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            if (retData.Count > 0)
            {
                strData = new string[retData.Count];
                int nIndex = 0;
                foreach (string dt in retData)
                    strData[nIndex++] = dt;
            }
            return strData;
        }*/

        public string[] GetFridgeTempAfter(string serialRFID, string DateTimetoParse, int nbItem = int.MaxValue)
        {
            ArrayList retData = new ArrayList(); ;
            string[] strData = null;

            try
            {
                //DateTime dt = DateTime.Parse(DateTimetoParse);
                DateTime dt = DateTime.ParseExact(DateTimetoParse, "yyyy-MM-dd HH:mm:ssZ",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.AssumeUniversal |
                    System.Globalization.DateTimeStyles.AdjustToUniversal);
                if (myConn == null) return null;
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT TempStream FROM tb_Fridge_Temp WHERE 1=1 ";
                sqlQuery += "AND SerialRFID='" + serialRFID + "' ";
                sqlQuery += "AND Sorted_Event_Date>'" + dt.ToString("s") + "' ";
                sqlQuery += "ORDER BY Sorted_Event_Date ASC";
                if (nbItem != int.MaxValue) sqlQuery += " LIMIT " + nbItem.ToString();

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        retData.Add(rd[0].ToString().Trim().ToString());
                    }
                }
                if (rd != null) rd.Close();
                if (cmd != null) cmd.Dispose();


            }
            catch (NullReferenceException nra)
            {

                return null;
            }
            catch (Exception exp)
            {
               // On affiche l'erreur.
               ErrorMessage.ExceptionMessageBox.Show(exp);
               return null;
            }           
            if (retData.Count > 0)
            {
                strData = new string[retData.Count];
                int nIndex = 0;
                foreach (string dt in retData)
                    strData[nIndex++] = dt;
            }
            return strData;
        }

        //  temp v2
        public PtTemp GetLastTempPoint(string SerialRFID, bool bFridgeOK)
        {
            PtTemp lastTemp = null;

            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                if (!bFridgeOK)
                {
                    sqlQuery += "SELECT * FROM tbo_TempFridge ";
                    sqlQuery += "WHERE 1=1 ";
                    sqlQuery += "AND SerialRFID='" + SerialRFID + "' ";
                    sqlQuery += "ORDER BY TemperatureDate DESC LIMIT 1";
                }
                else
                {
                    sqlQuery += "SELECT * FROM tbo_TempFridge ";
                    sqlQuery += "WHERE bFridgeOK=1 ";
                    sqlQuery += "AND SerialRFID='" + SerialRFID + "' ";
                    sqlQuery += "ORDER BY TemperatureDate DESC LIMIT 1";
                }
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    rd.Read();
                    lastTemp = new PtTemp();
                    lastTemp.TempAcqDate = rd["TemperatureDate"].ToString();
                    lastTemp.TempBottle = rd.IsDBNull(rd.GetOrdinal("TemperatureBottle")) ? null : (double?)rd["TemperatureBottle"];
                    lastTemp.TempChamber = rd.IsDBNull(rd.GetOrdinal("TemperatureChamber")) ? null : (double?)rd["TemperatureChamber"];
                    lastTemp.bFridgeOK = Convert.ToInt32(rd["bFridgeOK"].ToString());
                }
                if (rd != null) rd.Close();
                if (cmd != null) cmd.Dispose();
            }

            catch (Exception exp)
            {
                // On affiche l'erreur.

                ErrorMessage.ExceptionMessageBox.Show(exp);
            }          


            return lastTemp;
        }
        public PtTemp[] GetPointFromDate(string SerialRFID, string date)
        {
            ArrayList retData = new ArrayList(); ;
            PtTemp[] PtData = null;
           
            try
            {

                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tbo_TempFridge WHERE 1=1 ";
                sqlQuery += "AND SerialRFID='" + SerialRFID + "' ";
                sqlQuery += "AND TemperatureDate>'" + date + "' ORDER BY TemperatureDate ASC ;";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        PtTemp lastTemp = new PtTemp();
                        lastTemp.TempAcqDate = rd["TemperatureDate"].ToString();
                        lastTemp.TempBottle = rd.IsDBNull(rd.GetOrdinal("TemperatureBottle")) ? null : (double?)rd["TemperatureBottle"];
                        lastTemp.TempChamber = rd.IsDBNull(rd.GetOrdinal("TemperatureChamber")) ? null : (double?)rd["TemperatureChamber"];
                        lastTemp.bFridgeOK = Convert.ToInt32(rd["bFridgeOK"].ToString());
                        retData.Add(lastTemp);
                    }
                }
                if (rd != null) rd.Close();
                if (cmd != null) cmd.Dispose();


            }
            catch (Exception exp)
            {
                // On affiche l'erreur.

                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
           
            if (retData.Count > 0)
            {
                PtData = new PtTemp[retData.Count];
                int nIndex = 0;
                foreach (PtTemp data in retData)
                    PtData[nIndex++] = data;
            }
            return PtData;
        }
        public List<PtTemp> GetPointBeforeDate(string SerialRFID, string date, int NbPt)
        {
            List<PtTemp> result = new List<PtTemp>();           

            try
            {

                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tbo_TempFridge WHERE 1=1 ";
                sqlQuery += "AND SerialRFID='" + SerialRFID + "' ";
                sqlQuery += "AND TemperatureDate<'" + date + "' ORDER BY TemperatureDate DESC LIMIT " + NbPt.ToString();


                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        PtTemp lastTemp = new PtTemp();
                        lastTemp.TempAcqDate = rd["TemperatureDate"].ToString();
                        lastTemp.TempBottle = rd.IsDBNull(rd.GetOrdinal("TemperatureBottle")) ? null : (double?)rd["TemperatureBottle"];
                        lastTemp.TempChamber = rd.IsDBNull(rd.GetOrdinal("TemperatureChamber")) ? null : (double?)rd["TemperatureChamber"];
                        lastTemp.bFridgeOK = Convert.ToInt32(rd["bFridgeOK"].ToString());
                        result.Add(lastTemp);
                    }
                }
                if (rd != null) rd.Close();
                if (cmd != null) cmd.Dispose();

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.

                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
           

            return result;
        }

        public PtTemp[] GetPointBetweenDate(string SerialRFID, string dateFrom, string dateTo)
        {
            ArrayList retData = new ArrayList(); ;
            PtTemp[] PtData = null;
           
            try
            {

                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tbo_TempFridge WHERE 1=1 ";
                sqlQuery += "AND SerialRFID='" + SerialRFID + "' ";
                sqlQuery += "AND TemperatureDate>'" + dateFrom + "' ";
                sqlQuery += "AND TemperatureDate<'" + dateTo + "' ORDER BY TemperatureDate ASC ;";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader rd = cmd.ExecuteReader();

                rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        PtTemp lastTemp = new PtTemp();
                        lastTemp.TempAcqDate = rd["TemperatureDate"].ToString();
                        lastTemp.TempBottle = rd.IsDBNull(rd.GetOrdinal("TemperatureBottle")) ? null : (double?)rd["TemperatureBottle"];
                        lastTemp.TempChamber = rd.IsDBNull(rd.GetOrdinal("TemperatureChamber")) ? null : (double?)rd["TemperatureChamber"];
                        lastTemp.bFridgeOK = Convert.ToInt32(rd["bFridgeOK"].ToString());
                        retData.Add(lastTemp);
                    }
                }
                if (rd != null) rd.Close();
                if (cmd != null) cmd.Dispose();

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.

                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
          
            if (retData.Count > 0)
            {
                PtData = new PtTemp[retData.Count];
                int nIndex = 0;
                foreach (PtTemp data in retData)
                    PtData[nIndex++] = data;
            }
            return PtData;
        }
        public bool AddNewPtTemp(string SerialRFID, string date)
        {
            bool ret = false;
            
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;

                sqlQuery += "INSERT INTO tbo_TempFridge(TemperatureDate,SerialRFID) VALUES('";
                sqlQuery += date + "','";
                sqlQuery += SerialRFID + "')";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;             
                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;            
                if (cmd != null) cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            finally
            {
           
            }
            return ret;
        }
        public bool AddNewPtTemp(string SerialRFID, string date, double tempBottle, double tempChamber)
        {
            bool ret = false;
            
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;

                sqlQuery += "INSERT INTO tbo_TempFridge(TemperatureDate,SerialRFID,TemperatureBottle,TemperatureChamber,bFridgeOK) VALUES('";
                sqlQuery += date + "','";
                sqlQuery += SerialRFID + "',";
                sqlQuery += tempBottle.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ",";
                sqlQuery += tempChamber.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ",1)";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                if (cmd != null) cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
          
            return ret;
        }
        public bool UpdatePtTemp(string SerialRFID, string date, double tempBottle, double tempChamber)
        {
            bool ret = false;
            
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;

                sqlQuery += "UPDATE tbo_TempFridge ";
                sqlQuery += "SET TemperatureBottle=" + tempBottle.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ",";
                sqlQuery += "TemperatureChamber=" + tempChamber.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ",";
                sqlQuery += "bFridgeOK=1 ";
                sqlQuery += "WHERE TemperatureDate='" + date + "' ";
                sqlQuery += "AND SerialRFID='" + SerialRFID + "';";
                

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                int rowAffected = cmd.ExecuteNonQuery();
                            

                if (rowAffected > 0) ret = true;
                if (cmd != null) cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
         
            return ret;
        }
        public bool IsPointExist(string SerialRFID, string date, out bool bValueNull)
        {
            bool ret = false;
            bValueNull = false;
            try
            {

                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tbo_TempFridge WHERE 1=1 ";
                sqlQuery += "AND TemperatureDate='" + date + "' ";
                sqlQuery += "AND SerialRFID='" + SerialRFID + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader rd = cmd.ExecuteReader();              

                if (rd.HasRows)
                {
                    rd.Read();
                    bValueNull = !Convert.ToBoolean(Convert.ToInt32(rd["bFridgeOK"].ToString()));
                    ret = true;
                }
                if (rd != null) rd.Close();
                if (cmd != null) cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.

                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
           
            return ret;
        }

        public string GetLastReportDate()
        {
        
            string lastReportDate = String.Empty;

            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = String.Format("SELECT ReportDate FROM tbo_Daily_TempReport_History;");

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader rd = cmd.ExecuteReader();                

                if (!rd.HasRows) return lastReportDate; // str empty
                rd.Read();
                lastReportDate = rd["ReportDate"].ToString();

                if (rd != null) rd.Close();
                if (cmd != null) cmd.Dispose();
            }
            catch (Exception exp)
            {
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }            

            return lastReportDate;
        }
        public void UpdateLastReportDate(string dateAsString)
        {
            

            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = String.Format("UPDATE tbo_Daily_TempReport_History set ReportDate = '{0}';", dateAsString);

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
             

                if (cmd.ExecuteNonQuery() == 0) // no date have been inserted yet : let's create a new one
                {
                    sqlQuery = String.Format("INSERT INTO tbo_Daily_TempReport_History VALUES('{0}');", dateAsString);

                    cmd.Transaction = trans;         
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
         
                if (cmd != null) cmd.Dispose();
            }
            catch (Exception exp)
            {
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
           
        }



        #endregion
        #region alert
        public smtpInfo getSmtpInfo(bool onlyWhenActive)
        {
            smtpInfo smtp = null;
            try
            {

             if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Mail_Smtp";
                if (onlyWhenActive)
                    sqlQuery += " WHERE bActive=1";
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    rd.Read();
                    {
                        smtp = new smtpInfo();
                        smtp.smtp = rd["Smtp"].ToString();
                        smtp.port = Convert.ToInt32(rd["Port"].ToString());
                        smtp.sender = rd["Sender"].ToString();
                        smtp.login = rd["Login"].ToString();
                        smtp.pwd = rd["Pwd"].ToString();
                        smtp.bUseSSL = Convert.ToBoolean(Convert.ToInt32(rd["bUseSSL"].ToString()));
                        smtp.bActive = Convert.ToBoolean(Convert.ToInt32(rd["bActive"].ToString()));
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return smtp;
        }

        public bool DeleteSMTP()
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "DELETE FROM tb_Mail_Smtp ";  
        

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }

        public bool AddSmtp(smtpInfo smtp)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
               
                string sqlQuery = null;
                sqlQuery += "INSERT INTO [tb_Mail_Smtp] (Smtp,Port,Sender,Login,Pwd,bUseSSL,bActive) VALUES('";
                sqlQuery += smtp.smtp  + "',";
                sqlQuery += smtp.port + ",'";
                sqlQuery += smtp.sender + "','";
                sqlQuery += smtp.login + "','";
                sqlQuery += smtp.pwd + "',";
                sqlQuery += Convert.ToInt32(smtp.bUseSSL) + ",";
                sqlQuery += Convert.ToInt32(smtp.bActive) + ");";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }

        public bool DeleteAlert(alertInfo alert)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "DELETE FROM tb_Alert_List ";
                sqlQuery += "WHERE AlertName='" + alert.AlertName + "'"; ;

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }

        public bool AddAlertInfo(alertInfo alert)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                if (alert.alertData == null)
                {
                    sqlQuery += "INSERT INTO [tb_Alert_List] (AlertName,RecipientList,CCRecipientList,BCCRecipientList,MailSubject,AlertMessage,bActive) VALUES ('";
                    sqlQuery += alert.AlertName + "','";
                    sqlQuery += alert.RecipientList + "','";
                    sqlQuery += alert.CCRecipientList + "','";
                    sqlQuery += alert.BCCRecipientList + "','";
                    sqlQuery += alert.MailSubject + "','";
                    sqlQuery += alert.AlertMessage + "',";
                    sqlQuery += Convert.ToInt32(alert.bActive) + ");";
                }
                else
                {
                    sqlQuery += "INSERT INTO [tb_Alert_List] (AlertName,RecipientList,CCRecipientList,BCCRecipientList,MailSubject,AlertMessage,bActive,AlertData) VALUES ('";
                    sqlQuery += alert.AlertName + "','";
                    sqlQuery += alert.RecipientList + "','";
                    sqlQuery += alert.CCRecipientList + "','";
                    sqlQuery += alert.BCCRecipientList + "','";
                    sqlQuery += alert.MailSubject + "','";
                    sqlQuery += alert.AlertMessage + "',";
                    sqlQuery += Convert.ToInt32(alert.bActive) + ",'";
                    sqlQuery += alert.alertData +  "');";
                }

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }

        public int GetDoorOpenToLongTime()
        {
            int retData = 30; // default Value
            try
            {

                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT AlertData FROM tb_Alert_List ";
                sqlQuery += "WHERE AlertName='AT_Door_Open_Too_Long'";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    rd.Read();
                    {
                        if (!int.TryParse(rd[0].ToString(), out retData))
                            retData = 30;
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return retData;
        }
        public double GetMaxTempFridgeValue()
        {
            double retData = 12.0; // default Value
            try
            {

                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT AlertData FROM tb_Alert_List ";
                sqlQuery += "WHERE AlertName='AT_Max_Fridge_Temp'";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    rd.Read();
                    {
                        string val = rd[0].ToString();
                        System.Globalization.CultureInfo culture;

                        if (val.Contains("."))
                        {
                            // Utilisation de InvariantCulture si présence du . comme séparateur décimal. 
                            culture = System.Globalization.CultureInfo.InvariantCulture;
                        }
                        else
                        {
                            // Utilisation de CurrentCulture sinon (utilisation de , comme séparateur décimal).
                            culture = System.Globalization.CultureInfo.CurrentCulture;
                        }

                        // Conversion de la chaîne en double, en utilisant la culture correspondante.            
                        double.TryParse(val, System.Globalization.NumberStyles.Number, culture, out retData);
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return retData;
        }

        public alertInfo getAlertInfo(AlertType alertType , bool bActive)
        {
            alertInfo alert = null;
            try
            {

                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Alert_List ";
                sqlQuery += "WHERE AlertName='" + alertType.ToString();

                if (bActive) sqlQuery += "' AND bActive=1";
                else sqlQuery += "'";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    rd.Read();
                    {
                        alert = new alertInfo();
                        alert.type = alertType;
                        alert.AlertName = rd["AlertName"].ToString();
                        alert.RecipientList = rd.IsDBNull(rd.GetOrdinal("RecipientList")) ? string.Empty : (string)rd["RecipientList"];
                        alert.CCRecipientList = rd.IsDBNull(rd.GetOrdinal("CCRecipientList")) ? string.Empty : (string)rd["CCRecipientList"];
                        alert.BCCRecipientList = rd.IsDBNull(rd.GetOrdinal("BCCRecipientList")) ? string.Empty : (string)rd["BCCRecipientList"];
                        alert.MailSubject = rd.IsDBNull(rd.GetOrdinal("MailSubject")) ? string.Empty : (string)rd["MailSubject"];
                        alert.AlertMessage = rd.IsDBNull(rd.GetOrdinal("AlertMessage")) ? string.Empty : (string)rd["AlertMessage"];
                        alert.bActive = Convert.ToBoolean(Convert.ToInt32(rd["bActive"].ToString()));
                        alert.alertData = rd.IsDBNull(rd.GetOrdinal("AlertData")) ? string.Empty : (string)rd["AlertData"];
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return alert;
        }
        public string GetColumnNameForAlert(AlertType alertType)
        {
            string columnName = null;
            try
            {

                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT ColumnName FROM tb_Column_Info ";
                sqlQuery += "WHERE AlertLink='" + alertType.ToString() + "'";
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    rd.Read();
                    {
                        columnName = rd.IsDBNull(rd.GetOrdinal("ColumnName")) ? string.Empty : (string)rd["ColumnName"];                        
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return columnName;
        }
        public string GetColumnNameForWeight()
        {
            string columnName = null;
            try
            {

                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT ColumnName FROM tb_Column_Info ";
                sqlQuery += "WHERE AlertLink='WEIGHT'";
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    rd.Read();
                    {
                        columnName = rd.IsDBNull(rd.GetOrdinal("ColumnName")) ? string.Empty : (string)rd["ColumnName"];
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return columnName;
        }

        public bool storeAlert(AlertType alertType, DeviceInfo di, UserClassTemplate utc, string alertData)
        {
             bool ret = false;
             try
             {
                 if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                 DateTime gmtDate = DateTime.Now.ToUniversalTime();
                 string sqlQuery = null;
                 sqlQuery += "INSERT INTO tb_Alert_History(AlertDate,SortedAlertDate,AlertName,SerialRFID,FirstName,LastName,AlertData) VALUES('";
                 sqlQuery += gmtDate.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + "','";
                 sqlQuery += gmtDate.ToString("s") + "','";
                 sqlQuery += alertType.ToString() + "','";
                 sqlQuery += di.SerialRFID;
                 if (utc != null)
                 {
                     sqlQuery += "','";
                     sqlQuery += utc.firstName + "','";
                     sqlQuery += utc.lastName;
                 }
                 else
                 {
                     sqlQuery += "',' ',' ";
                 }
                 if (!string.IsNullOrEmpty(alertData))
                     sqlQuery += "','" + alertData;
                 else
                     sqlQuery += "',' ";
                 sqlQuery += "');";

                 SQLiteCommand cmd = myConn.CreateCommand();
                 cmd.CommandText = sqlQuery;
                 cmd.CommandType = System.Data.CommandType.Text;
                 int rowAffected = cmd.ExecuteNonQuery();

                 if (rowAffected > 0) ret = true;
                 cmd.Dispose();
             }

             catch (Exception exp)
             {
                 // On affiche l'erreur.
                 ErrorMessage.ExceptionMessageBox.Show(exp);
             }
            return ret;
        }
        #endregion
        #region Group

        public DataTable RecoverAllGroup()
        {
            DataTable dtGet = null;
            try
            {
                if (myConn == null) return null;
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                {
                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tb_Group_Def;";
                    SQLiteDataAdapter da = new SQLiteDataAdapter(sqlQuery, myConn);
                    dtGet = new DataTable();
                    da.Fill(dtGet);
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return dtGet;
        }

        public bool DeleteTagLinkFromBox(string tagBox)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "DELETE FROM tb_Group_Link ";
                sqlQuery += "WHERE GroupTagRef=@tagBox;";

                SQLiteCommand cmd2 = myConn.CreateCommand();
                cmd2.CommandText = sqlQuery;
                cmd2.CommandType = System.Data.CommandType.Text;
                cmd2.Parameters.Add(new SQLiteParameter("@tagBox", tagBox));
                cmd2.ExecuteNonQuery();              

                int rowAffected = cmd2.ExecuteNonQuery();
                if (rowAffected > 0) ret = true;
            
                cmd2.Dispose();

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }
        public bool DeleteGroup(string tagBox)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "DELETE FROM tb_Group_Link ";
                sqlQuery += "WHERE GroupTagRef=@tagBox;";

                SQLiteCommand cmd2 = myConn.CreateCommand();
                cmd2.CommandText = sqlQuery;
                cmd2.CommandType = System.Data.CommandType.Text;
                cmd2.Parameters.Add(new SQLiteParameter("@tagBox", tagBox));
                cmd2.ExecuteNonQuery();

              
                sqlQuery += "DELETE FROM tb_Group_Def ";
                sqlQuery += "WHERE TagGroup=@tagBox;";


                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@tagBox", tagBox));

                int rowAffected = cmd.ExecuteNonQuery();
              

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
                cmd2.Dispose();
                
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }
        public bool AddGroup(string TagBox,string groupRef, string groupInfo , string criteria = null)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                if (string.IsNullOrEmpty(criteria))
                {
                    sqlQuery += "INSERT INTO tb_Group_Def(TagGroup,GroupRef,GroupInfo) VALUES(";
                    sqlQuery += "@tagBox,'";
                    sqlQuery += groupRef + "','";
                    sqlQuery += groupInfo + "');";
                }
                else
                {
                    sqlQuery += "INSERT INTO tb_Group_Def(TagGroup,GroupRef,GroupInfo,Criteria) VALUES(";
                    sqlQuery += "@tagBox,'";
                    sqlQuery += groupRef + "','";
                    sqlQuery += groupInfo + "','"; 
                    sqlQuery += criteria + "');";
                }

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@tagBox", TagBox));

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }

        public int GetGroupLinkCount(string GroupTagRef)
        {
            int ret = 0;

            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT COUNT(ProductTagRef) FROM tb_Group_Link ";
                sqlQuery += "WHERE GroupTagRef=@GroupTagRef;";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@GroupTagRef", GroupTagRef));

                ret = Convert.ToInt32(cmd.ExecuteScalar());    
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;

        }
        public string GetGroupName(string tagUID)
        {
            string groupName = null;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT TGD.GroupRef ";
                sqlQuery += "FROM (tb_Group_Def AS TGD ";
                sqlQuery += "INNER JOIN tb_Group_Link AS TGL ";
                sqlQuery += "ON TGL.GroupTagRef=TGD.TagGroup) ";               
                sqlQuery += "WHERE TGL.ProductTagRef=@tagUID;";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@tagUID", tagUID));

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    rd.Read();
                    {
                        groupName = (string)rd["GroupRef"];
                    }

                }
                rd.Close();
                cmd.Dispose();

            }
            
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return groupName;
        }

        public ArrayList GetGroupLink(string GroupTagRef)
        {
            ArrayList link = null;

            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT ProductTagRef FROM tb_Group_Link ";
                sqlQuery += "WHERE GroupTagRef=@GroupTagRef;";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@GroupTagRef", GroupTagRef));

                SQLiteDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    link = new ArrayList();
                    while (rd.Read())
                    {
                        link.Add(rd["ProductTagRef"].ToString());
                    }
                }
                rd.Close();
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return link;

        }
        public bool DeleteGroupLink(string  ProductTagRef)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();


                string sqlQuery = null;
                sqlQuery += "DELETE FROM tb_Group_Link ";
                sqlQuery += "WHERE ProductTagRef=@ProductTagRef;";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@ProductTagRef", ProductTagRef));

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }

        public bool AddGroupLink(string GroupTagRef, string productTagRef)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "INSERT INTO tb_Group_Link(GroupTagRef,ProductTagRef) VALUES(";
                sqlQuery += "@GroupTagRef,";
                sqlQuery += "@ProductTagRef);";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@ProductTagRef", productTagRef));
                cmd.Parameters.Add(new SQLiteParameter("@GroupTagRef", GroupTagRef));

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }

        #endregion
        #region Image

        public ArrayList GetImageListName()
        {
            ArrayList img = null;

            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT ImageName FROM tb_Image;";
          
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    img = new ArrayList();
                    while (rd.Read())
                    {
                        img.Add(rd["ImageName"].ToString());
                    }
                }
                rd.Close();
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return img;

        }

        public bool IsImageExist(string imageName)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Image ";
                sqlQuery += "WHERE ImageName='" + imageName + "';";
               
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();
                ret = rd.HasRows;
                rd.Close();
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }

        public bool AddImage(string imageName , byte[] imageArray)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

               
                string sqlQuery = null;
                sqlQuery += "INSERT INTO tb_Image(ImageName,ImageByteArray) VALUES('";
                sqlQuery += imageName + "',@byteArray);";            

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add("@byteArray", DbType.Binary).Value = imageArray;

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }

        public bool DeleteImage(string imageName)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "DELETE FROM tb_Image ";
                sqlQuery += "WHERE ImageName='" + imageName + "';";               

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }

        public byte[] getImage(string imageName)
        {

            byte[] image  = null;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();


                string sqlQuery = null;
                sqlQuery += "SELECT ImageByteArray FROM tb_Image ";
                sqlQuery += "WHERE ImageName='" + imageName + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        image = GetBytes(reader);
                    }
                }

                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return image;
        }

        static byte[] GetBytes(SQLiteDataReader reader)
        {
            const int CHUNK_SIZE = 2 * 1024;
            byte[] buffer = new byte[CHUNK_SIZE];
            long bytesRead;
            long fieldOffset = 0;
            using (MemoryStream stream = new MemoryStream())
            {
                while ((bytesRead = reader.GetBytes(0, fieldOffset, buffer, 0, buffer.Length)) > 0)
                {
                    byte[] actualRead = new byte[bytesRead];
                    Buffer.BlockCopy(buffer, 0, actualRead, 0, (int)bytesRead);
                    stream.Write(actualRead, 0, actualRead.Length);
                    fieldOffset += bytesRead;
                }
                return stream.ToArray();
            }
        }


        public int getNumberofLink(string ImageName)
        {
            int ret = 0;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT COUNT(LotIDorTagID) FROM tb_Image_Link ";
                sqlQuery += "WHERE ImageName='" + ImageName + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                ret = Convert.ToInt32(cmd.ExecuteScalar());
             
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }
        

        public bool IsImageLinkExist(string LotIDorTagID)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Image_Link ";
                sqlQuery += "WHERE LotIDorTagID='" + LotIDorTagID + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();
                ret = rd.HasRows;
                rd.Close();
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }

        public string getImageNameLink(string LotIDorTagID)
        {
            string imageName = null;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Image_Link ";
                sqlQuery += "WHERE LotIDorTagID='" + LotIDorTagID + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    rd.Read();
                    {
                        imageName = (string) rd["ImageName"];
                    }
                   
                }
                rd.Close();
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return imageName;
        }

        public bool AddImageLink(string imageName, string LotIDorTagID )
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();


                string sqlQuery = null;
                sqlQuery += "INSERT INTO tb_Image_Link (LotIDorTagID,ImageName) VALUES('";
                sqlQuery += LotIDorTagID + "','" + imageName +"');";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;               

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }

        public bool DeleteallImageLink(string ImageName)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "DELETE FROM tb_Image_Link ";
                sqlQuery += "WHERE ImageName='" + ImageName + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd.ExecuteNonQuery();
              
                cmd.Dispose();
                  ret = true;

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }
        

        public bool DeleteImageLink(string LotIDorTagID)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "DELETE FROM tb_Image_Link ";
                sqlQuery += "WHERE LotIDorTagID='" + LotIDorTagID + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }





        #endregion
        #region User

        public UserClassTemplate[] RecoverUser()
        {
            UserClassTemplate[] userArray = null;
            ArrayList listUser = new ArrayList();

            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT FPTemplate FROM tb_User_Def";
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream mem = new MemoryStream(Convert.FromBase64String(rd[0].ToString()));
                        UserClassTemplate TheUser = new UserClassTemplate();
                        TheUser = (UserClassTemplate)bf.Deserialize(mem);
                        listUser.Add(TheUser);

                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            if (listUser.Count > 0)
            {
                userArray = new UserClassTemplate[listUser.Count];
                listUser.CopyTo(userArray);
            }
            return userArray;
        }

        /*public UserClassTemplate[] RecoverAllowedUser(string serialRFID )
        {
            UserClassTemplate[] userArray = null;
            ArrayList listUser = new ArrayList();

            try
            {
                if (myConn == null) return null;
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT TUD.FPTemplate ";
                sqlQuery += "FROM (tb_User_Def AS TUD ";
                sqlQuery += "INNER JOIN tb_User_Grant AS TUG ";
                sqlQuery += "ON TUG.FirstName=TUD.FirstName ";
                sqlQuery += "AND TUG.LastName=TUD.LastName) ";
                sqlQuery += "WHERE TUG.SerialRFID='" + serialRFID + "';";
                
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;               
                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream mem = new MemoryStream(Convert.FromBase64String(rd[0].ToString()));
                        UserClassTemplate TheUser = new UserClassTemplate();
                        TheUser = (UserClassTemplate)bf.Deserialize(mem);
                        listUser.Add(TheUser);
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            if (listUser.Count > 0)
            {
                userArray = new UserClassTemplate[listUser.Count];
                listUser.CopyTo(userArray);
            }
            return userArray;
        }*/
        public DeviceGrant[] RecoverAllowedUser(string serialRFID)
        {
            DeviceGrant[] userArray = null;
            ArrayList listUser = new ArrayList();

            try
            {
                if (myConn == null) return null;
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT TUD.FPTemplate,TUG.DoorGranted ";
                sqlQuery += "FROM (tb_User_Def AS TUD ";
                sqlQuery += "INNER JOIN tb_User_Grant AS TUG ";
                sqlQuery += "ON TUG.FirstName=TUD.FirstName ";
                sqlQuery += "AND TUG.LastName=TUD.LastName) ";
                sqlQuery += "WHERE TUG.SerialRFID='" + serialRFID + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream mem = new MemoryStream(Convert.FromBase64String(rd[0].ToString()));
                        UserClassTemplate TheUser = new UserClassTemplate();
                        TheUser = (UserClassTemplate)bf.Deserialize(mem);

                        DeviceGrant newuser = new DeviceGrant();
                        newuser.user = TheUser;
                        int reg = int.Parse(rd["DoorGranted"].ToString());
                        newuser.userGrant = (UserGrant)reg;
                        newuser.serialRFID = serialRFID;
                        listUser.Add(newuser);
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            if (listUser.Count > 0)
            {
                userArray = new DeviceGrant[listUser.Count];
                listUser.CopyTo(userArray);
            }
            return userArray;
        }

        public UserClassTemplate RecoverUser(string firstName, string lastName)
        {

            UserClassTemplate user = null;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT FPTemplate FROM tb_User_Def ";
                sqlQuery += "WHERE FirstName='" + firstName + "' ";
                sqlQuery += "AND LastName='" + lastName + "';";
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;                

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream mem = new MemoryStream(Convert.FromBase64String(rd[0].ToString()));
                        user = new UserClassTemplate();
                        user = (UserClassTemplate)bf.Deserialize(mem);

                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return user;
        }
        public bool IsUserExist(string firstName, string lastName)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_User_Def ";
                sqlQuery += "WHERE FirstName='" + firstName + "' ";
                sqlQuery += "AND LastName='" + lastName + "';";
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;              

                SQLiteDataReader rd = cmd.ExecuteReader();
                ret = rd.HasRows;
                rd.Close();
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }
        public bool StoreUser(UserClassTemplate UserToStore)
        {

            bool ret = false;

            if (IsUserExist(UserToStore.firstName, UserToStore.lastName))
            {
                ret = UpdateUser(UserToStore);
            }
            else
            {
                ret = AddUser(UserToStore);
            }
            return ret;
        }
        private bool AddUser(UserClassTemplate UserToStore)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream mem = new MemoryStream();
                bf.Serialize(mem, UserToStore);
                string str = Convert.ToBase64String(mem.ToArray());

                string sqlQuery = null;
                sqlQuery += "INSERT INTO tb_User_Def(FirstName,LastName,FPTemplate) VALUES('";
                sqlQuery += UserToStore.firstName + "','";
                sqlQuery += UserToStore.lastName + "','";
                sqlQuery += str + "');";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;               

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }

        private bool UpdateUser(UserClassTemplate UserToStore)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream mem = new MemoryStream();
                bf.Serialize(mem, UserToStore);
                string str = Convert.ToBase64String(mem.ToArray());

                string sqlQuery = null;
                sqlQuery += "UPDATE tb_User_Def ";
                sqlQuery += "SET FPTemplate='" + str + "' ";
                sqlQuery += "WHERE FirstName='" + UserToStore.firstName + "' ";
                sqlQuery += "AND LastName='" + UserToStore.lastName + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;  

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }

        public int getUserFingerAlert(string firstName, string lastName)
        {
            int ret = -1;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                string sqlQuery = null;

                sqlQuery += "SELECT FingerAlert FROM tb_User_Def ";
                sqlQuery += "WHERE FirstName='" + firstName + "' ";
                sqlQuery += "AND LastName='" + lastName + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                ret = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Dispose();
             }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }

        public bool UpdateUserFingerAlert(string firstName, string lastName , int fingerAlert)
        {
              bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                string sqlQuery = null;
                sqlQuery += "UPDATE tb_User_Def ";
                sqlQuery += "SET FingerAlert=" + fingerAlert + " ";
                sqlQuery += "WHERE FirstName='" + firstName + "' ";
                sqlQuery += "AND LastName='" + lastName + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }
     

        public bool UpdateUserMaxRemovedItem(string firstName, string lastName, int maxItem)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                string sqlQuery = null;
                sqlQuery += "UPDATE tb_User_Def ";
                sqlQuery += "SET MaxRemoveItem=" + maxItem + " ";
                sqlQuery += "WHERE FirstName='" + firstName + "' ";
                sqlQuery += "AND LastName='" + lastName + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }
        public int getUserMaxRemovedItem(string firstName, string lastName)
        {
            int ret = -1;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                string sqlQuery = null;

                sqlQuery += "SELECT MaxRemoveItem FROM tb_User_Def ";
                sqlQuery += "WHERE FirstName='" + firstName + "' ";
                sqlQuery += "AND LastName='" + lastName + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                ret = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }

        public bool UpdateUserMaxRemoveValue(string firstName, string lastName, float maxRemoveValue)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                string sqlQuery = null;
                sqlQuery += "UPDATE tb_User_Def ";
                sqlQuery += "SET MaxRemoveValue=" + maxRemoveValue + " ";
                sqlQuery += "WHERE FirstName='" + firstName + "' ";
                sqlQuery += "AND LastName='" + lastName + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }

        public double getUserMaxRemoveValue(string firstName, string lastName)
        {
            double ret = -1.0;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                string sqlQuery = null;

                sqlQuery += "SELECT MaxRemoveValue FROM tb_User_Def ";
                sqlQuery += "WHERE FirstName='" + firstName + "' ";
                sqlQuery += "AND LastName='" + lastName + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                ret = Convert.ToDouble(cmd.ExecuteScalar());
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }

        public bool DeleteUser(string firstName, string lastName)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "DELETE FROM tb_User_Def ";
                sqlQuery += "WHERE FirstName='" + firstName + "' ";
                sqlQuery += "AND LastName='" + lastName + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;               

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }
        public bool DeleteAllUser()
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "DELETE FROM tb_User_Def;";


                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;               

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }

        #endregion
        #region user Grant

        public string[] recoverUserGrant(UserClassTemplate User)
        {
            string[] userGrantArray = null;
            ArrayList listUserGrant = new ArrayList();

            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT SerialRFID FROM tb_User_Grant ";
                sqlQuery += "WHERE FirstName='" + User.firstName + "' ";
                sqlQuery += "AND LastName='" + User.lastName + "';";               

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        listUserGrant.Add(rd["SerialRFID"]);
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            if (listUserGrant.Count > 0)
            {
                userGrantArray = new string[listUserGrant.Count];
                listUserGrant.CopyTo(userGrantArray);
            }
            return userGrantArray;
        }

        public DeviceGrant[] recoverUserGrantFull(UserClassTemplate User)
        {
            DeviceGrant[] userGrantArray = null;
            ArrayList listUserGrant = new ArrayList();

            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT SerialRFID,DoorGranted FROM tb_User_Grant ";
                sqlQuery += "WHERE FirstName='" + User.firstName + "' ";
                sqlQuery += "AND LastName='" + User.lastName + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        DeviceGrant dg = new DeviceGrant();
                        dg.user = User;
                        dg.serialRFID = (string)rd["SerialRFID"];
                        int reg = int.Parse(rd["DoorGranted"].ToString());                         
                        dg.userGrant = (UserGrant)reg;
                        listUserGrant.Add(dg);
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            if (listUserGrant.Count > 0)
            {
                userGrantArray = new DeviceGrant[listUserGrant.Count];
                listUserGrant.CopyTo(userGrantArray);
            }
            return userGrantArray;
        }

        public bool isGrantExist(string firstName, string lastName, string SerialRFID)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_User_Grant ";
                sqlQuery += "WHERE FirstName='" + firstName + "' ";
                sqlQuery += "AND LastName='" + lastName + "' ";
                sqlQuery += "AND SerialRFID='" + SerialRFID + "';";
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader rd = cmd.ExecuteReader();
                ret = rd.HasRows;
                rd.Close();
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }
        public bool StoreGrant(UserClassTemplate UserToStore, string SerialRFID,string BelongTo,UserGrant userGrant)
        {
            bool ret = false;

            if (!isGrantExist(UserToStore.firstName, UserToStore.lastName, SerialRFID))
            {
                ret = AddUserGrant(UserToStore, SerialRFID, BelongTo, userGrant);
            }
            else
            {
                DeleteUserGrant(UserToStore.firstName, UserToStore.lastName, SerialRFID);
                ret = AddUserGrant(UserToStore, SerialRFID, BelongTo, userGrant);
            }
            return ret;
        }
        private bool AddUserGrant(UserClassTemplate UserToStore, string SerialRFID, string BelongTo, UserGrant userGrant)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                if (BelongTo == null)
                {
                    sqlQuery += "INSERT INTO tb_User_Grant(FirstName,LastName,SerialRFID,DoorGranted) VALUES('";
                    sqlQuery += UserToStore.firstName + "','";
                    sqlQuery += UserToStore.lastName + "','";
                    sqlQuery += SerialRFID + "',";
                    sqlQuery += (int)userGrant + ")";
                }
                else
                {
                    sqlQuery += "INSERT INTO tb_User_Grant(FirstName,LastName,SerialRFID,BelongTo,DoorGranted) VALUES('";
                    sqlQuery += UserToStore.firstName + "','";
                    sqlQuery += UserToStore.lastName + "','";
                    sqlQuery += SerialRFID + "','";
                    sqlQuery += BelongTo + "',";
                    sqlQuery += (int)userGrant + ")";
                }

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }
        public bool DeleteUserGrant(UserClassTemplate UserToStore, string SerialRFID)
        {
            bool ret = false;

            if (isGrantExist(UserToStore.firstName, UserToStore.lastName, SerialRFID))
            {
                try
                {
                    if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "DELETE FROM tb_User_Grant ";
                    sqlQuery += "WHERE FirstName='" + UserToStore.firstName + "' ";
                    sqlQuery += "AND LastName='" + UserToStore.lastName + "' ";
                    sqlQuery += "AND SerialRFID='" + SerialRFID + "';";
            

                    SQLiteCommand cmd = myConn.CreateCommand();
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                    cmd.Dispose();
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
            }
            return ret;
        }
        public bool DeleteUserGrant(string firstName, string lastName, string SerialRFID)
        {
            bool ret = false;

            
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "DELETE FROM tb_User_Grant ";
                sqlQuery += "WHERE FirstName='" + firstName + "' ";
                sqlQuery += "AND LastName='" + lastName + "'";

                if (string.IsNullOrEmpty(SerialRFID))
                    sqlQuery += ";";
                else
                    sqlQuery += " AND SerialRFID='" + SerialRFID + "';";


                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
           
            return ret;
        }
        public bool DeleteUserGrant(string BelongTo)
        {
            bool ret = false;
           
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "DELETE FROM tb_User_Grant ";
                sqlQuery += "WHERE BelongTo='" + BelongTo + "';";


                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
           
            return ret;
        }



        #endregion
        #region Device

        public ArrayList RecoverDistinctSerialRFID()
        {
            return null;
        }


        public DeviceInfo[] RecoverAllDevice(string machineName = null)
        {
            DeviceInfo[] deviceArray = null;
            ArrayList listDevice = new ArrayList();

            try
            {
                if (myConn == null) return null;
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Device_Def ";
                if (!string.IsNullOrEmpty(machineName))
                {
                    sqlQuery += "WHERE BelongTo='" + machineName + "';";
                }

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;               

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        DeviceInfo dev = new DeviceInfo();
                        dev.DeviceName = (string)rd["DeviceName"];
                        dev.deviceType = (DeviceType)int.Parse(rd["IdDeviceType"].ToString()); 
                        dev.SerialRFID = (string)rd["SerialRFID"];
                        
                        dev.SerialFPMaster = rd.IsDBNull(rd.GetOrdinal("SerialFPMaster")) ? string.Empty : (string)rd["SerialFPMaster"];
                        dev.SerialFPSlave = rd.IsDBNull(rd.GetOrdinal("SerialFPSlave")) ? string.Empty : (string)rd["SerialFPSlave"];
                        dev.bLocal = byte.Parse(rd["bLocal"].ToString());
                        dev.enabled = int.Parse(rd["Enabled"].ToString());
                        //dev.comLCD = rd.IsDBNull(rd.GetOrdinal("comLCD")) ? string.Empty : (string)rd["comLCD"];
                        dev.comMasterReader = rd.IsDBNull(rd.GetOrdinal("comMasterReader")) ? string.Empty : (string)rd["comMasterReader"];
                        dev.comSlaveReader = rd.IsDBNull(rd.GetOrdinal("comSlaveReader")) ? string.Empty : (string)rd["comSlaveReader"];
                        dev.comTempReader = rd.IsDBNull(rd.GetOrdinal("comTempReader")) ? string.Empty : (string)rd["comTempReader"];
                        dev.accessReaderType = (AccessBagerReaderType)int.Parse(rd["accessReaderType"].ToString());
                        dev.fridgeType = (FridgeType)int.Parse(rd["fridgeType"].ToString());
                        listDevice.Add(dev);
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            if (listDevice.Count > 0)
            {
                deviceArray = new DeviceInfo[listDevice.Count];
                listDevice.CopyTo(deviceArray);
            }
            return deviceArray;
        }

        public List<DeviceInfo> GetAllDevices()
        {
            List<DeviceInfo> devices = new List<DeviceInfo>();

            try
            {
                if (myConn == null) return devices;
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Device_Def;";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (!rd.HasRows) return devices;

                while (rd.Read())
                {
                    DeviceInfo dev = new DeviceInfo();
                    dev.DeviceName = (string)rd["DeviceName"];
                    dev.deviceType = (DeviceType)int.Parse(rd["IdDeviceType"].ToString());
                    dev.SerialRFID = (string)rd["SerialRFID"];

                    dev.SerialFPMaster = rd.IsDBNull(rd.GetOrdinal("SerialFPMaster")) ? string.Empty : (string)rd["SerialFPMaster"];
                    dev.SerialFPSlave = rd.IsDBNull(rd.GetOrdinal("SerialFPSlave")) ? string.Empty : (string)rd["SerialFPSlave"];
                    dev.bLocal = byte.Parse(rd["bLocal"].ToString());
                    dev.enabled = int.Parse(rd["Enabled"].ToString());

                    dev.comMasterReader = rd.IsDBNull(rd.GetOrdinal("comMasterReader")) ? string.Empty : (string)rd["comMasterReader"];
                    dev.comSlaveReader = rd.IsDBNull(rd.GetOrdinal("comSlaveReader")) ? string.Empty : (string)rd["comSlaveReader"];
                    dev.comTempReader = rd.IsDBNull(rd.GetOrdinal("comTempReader")) ? string.Empty : (string)rd["comTempReader"];
                    dev.accessReaderType = (AccessBagerReaderType)int.Parse(rd["accessReaderType"].ToString());
                    dev.fridgeType = (FridgeType)int.Parse(rd["fridgeType"].ToString());
                    devices.Add(dev);
                }
                rd.Close();
                cmd.Dispose();
            }
            catch (Exception)
            {
            }

            return devices;
        }


        public DeviceInfo[] RecoverDevice(bool bLocal, string machineName = null)
        {
            DeviceInfo[] deviceArray = null;
            ArrayList listDevice = new ArrayList();

            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Device_Def ";
                if (bLocal)
                    sqlQuery += "WHERE bLocal=1";
                else
                    sqlQuery += "WHERE bLocal=0 ";
                if (!string.IsNullOrEmpty(machineName))
                {
                    sqlQuery += "AND BelongTo='" + machineName + "';";
                }

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;              

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        DeviceInfo dev = new DeviceInfo();
                        dev.DeviceName = (string)rd["DeviceName"];
                        dev.deviceType = (DeviceType)int.Parse(rd["IdDeviceType"].ToString()); 
                        dev.SerialRFID = (string)rd["SerialRFID"];
                       
                        dev.SerialFPMaster = rd.IsDBNull(rd.GetOrdinal("SerialFPMaster")) ? string.Empty : (string)rd["SerialFPMaster"];
                        dev.SerialFPSlave = rd.IsDBNull(rd.GetOrdinal("SerialFPSlave")) ? string.Empty : (string)rd["SerialFPSlave"];
                        dev.IP_Server = rd.IsDBNull(rd.GetOrdinal("IP_Server")) ? string.Empty : (string)rd["IP_Server"];
                        dev.Port_Server = rd.IsDBNull(rd.GetOrdinal("Port_Server")) ? 0 : int.Parse(rd["Port_Server"].ToString());
                        dev.bLocal = byte.Parse(rd["bLocal"].ToString());
                        dev.enabled = int.Parse(rd["Enabled"].ToString());
                        //dev.comLCD = rd.IsDBNull(rd.GetOrdinal("comLCD")) ? string.Empty : (string)rd["comLCD"];
                        dev.comMasterReader = rd.IsDBNull(rd.GetOrdinal("comMasterReader")) ? string.Empty : (string)rd["comMasterReader"];
                        dev.comSlaveReader = rd.IsDBNull(rd.GetOrdinal("comSlaveReader")) ? string.Empty : (string)rd["comSlaveReader"];
                        dev.comTempReader = rd.IsDBNull(rd.GetOrdinal("comTempReader")) ? string.Empty : (string)rd["comTempReader"];
                        dev.accessReaderType = (AccessBagerReaderType)int.Parse(rd["accessReaderType"].ToString());
                        dev.fridgeType = (FridgeType)int.Parse(rd["fridgeType"].ToString());
                        listDevice.Add(dev);
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            if (listDevice.Count > 0)
            {
                deviceArray = new DeviceInfo[listDevice.Count];
                listDevice.CopyTo(deviceArray);
            }
            return deviceArray;
        }
        public DeviceInfo RecoverDevice(string SerialRFID)
        {

            DeviceInfo dev = null;
            try
            {
                
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Device_Def ";
                sqlQuery += "WHERE SerialRFID='" + SerialRFID + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;              

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        dev = new DeviceInfo();
                        dev.DeviceName = (string)rd["DeviceName"];
                        dev.deviceType = (DeviceType)int.Parse(rd["IdDeviceType"].ToString()); ;
                        dev.SerialRFID = (string)rd["SerialRFID"];
                      
                        dev.SerialFPMaster = rd.IsDBNull(rd.GetOrdinal("SerialFPMaster")) ? string.Empty : (string)rd["SerialFPMaster"];
                        dev.SerialFPSlave = rd.IsDBNull(rd.GetOrdinal("SerialFPSlave")) ? string.Empty : (string)rd["SerialFPSlave"];
                        dev.bLocal = byte.Parse(rd["bLocal"].ToString());
                        dev.IP_Server = rd.IsDBNull(rd.GetOrdinal("IP_server")) ? string.Empty : (string)rd["IP_server"];
                        dev.Port_Server = rd.IsDBNull(rd.GetOrdinal("Port_Server")) ? 0 : int.Parse(rd["Port_Server"].ToString());
                        dev.enabled = int.Parse(rd["Enabled"].ToString());
                        //dev.comLCD = rd.IsDBNull(rd.GetOrdinal("comLCD")) ? string.Empty : (string)rd["comLCD"];
                        dev.comMasterReader = rd.IsDBNull(rd.GetOrdinal("comMasterReader")) ? string.Empty : (string)rd["comMasterReader"];
                        dev.comSlaveReader = rd.IsDBNull(rd.GetOrdinal("comSlaveReader")) ? string.Empty : (string)rd["comSlaveReader"];
                        dev.comTempReader = rd.IsDBNull(rd.GetOrdinal("comTempReader")) ? string.Empty : (string)rd["comTempReader"];
                        dev.accessReaderType = (AccessBagerReaderType)int.Parse(rd["accessReaderType"].ToString());
                        dev.fridgeType = (FridgeType)int.Parse(rd["fridgeType"].ToString());
                       }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return dev;
        }
        public bool IsDeviceExist(string SerialRFID, string machineName = null)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Device_Def "; ;
                sqlQuery += "WHERE SerialRFID='" + SerialRFID + "';";
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;              

                SQLiteDataReader rd = cmd.ExecuteReader();
                ret = rd.HasRows;
                rd.Close();
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }
        public bool StoreDevice(DeviceInfo DeviceToStore, bool bNetwork, string machineName = null)
        {

            bool ret = false;

            if (IsDeviceExist(DeviceToStore.SerialRFID))
            {
                ret = UpdateDevice(DeviceToStore, bNetwork);
            }
            else
            {
                ret = AddDevice(DeviceToStore, bNetwork);
            }
            return ret;
        }
        private bool AddDevice(DeviceInfo DeviceToStore, bool bNetwork)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                SQLiteCommand cmd = myConn.CreateCommand();
               
                string sqlQuery = null;
                sqlQuery += "INSERT INTO tb_Device_Def(DeviceName,IdDeviceType,SerialRFID,bLocal,Enabled) VALUES('";
                sqlQuery += DeviceToStore.DeviceName + "',";
                sqlQuery += (int)DeviceToStore.deviceType + ",'";
                sqlQuery += DeviceToStore.SerialRFID + "',";
                sqlQuery += "1,";
                sqlQuery += (int)DeviceToStore.enabled + " );";
                
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();

                if (DeviceToStore.fridgeType != FridgeType.FT_UNKNOWN)
                {
                    sqlQuery = null;
                    sqlQuery += "UPDATE tb_Device_Def SET";
                    sqlQuery += " fridgeType=" + (int) DeviceToStore.fridgeType;
                    sqlQuery += " WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();
                }

                if (!string.IsNullOrEmpty(DeviceToStore.SerialFPMaster))
                {
                    sqlQuery = null;
                    sqlQuery += "UPDATE tb_Device_Def SET";
                    sqlQuery += " SerialFPMaster='" + DeviceToStore.SerialFPMaster + "'"; ; 
                    sqlQuery += " WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                if (!string.IsNullOrEmpty(DeviceToStore.SerialFPSlave))
                {
                    sqlQuery = null;
                    sqlQuery += "UPDATE tb_Device_Def SET";
                    sqlQuery += " SerialFPSlave='" + DeviceToStore.SerialFPSlave + "'";
                    sqlQuery += " WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                if (!string.IsNullOrEmpty(DeviceToStore.comMasterReader))
                {
                    sqlQuery = null;
                    sqlQuery += "UPDATE tb_Device_Def SET";
                    sqlQuery += " comMasterReader='" + DeviceToStore.comMasterReader + "'";
                    sqlQuery += " WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();


                    // update reader type
                    sqlQuery = null;
                    sqlQuery += "UPDATE tb_Device_Def SET";
                    sqlQuery += " accessReaderType=" + (int)DeviceToStore.accessReaderType;
                    sqlQuery += " WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();

                }
                if (!string.IsNullOrEmpty(DeviceToStore.comSlaveReader))
                {
                    sqlQuery = null;
                    sqlQuery += "UPDATE tb_Device_Def SET";
                    sqlQuery += " comSlaveReader='" + DeviceToStore.comSlaveReader + "'";
                    sqlQuery += " WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();
                   
                    // no need to update reader as master choose th type

                }
                if (!string.IsNullOrEmpty(DeviceToStore.comTempReader))
                {
                    sqlQuery = null;
                    sqlQuery += "UPDATE tb_Device_Def SET";
                    sqlQuery += " comTempReader='" + DeviceToStore.comTempReader + "'";
                    sqlQuery += " WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();                   
                }            
               

                if (bNetwork)
                {
                    sqlQuery = null;
                    sqlQuery += "UPDATE tb_Device_Def SET";
                    sqlQuery += " IP_Server ='" + DeviceToStore.IP_Server + "',";
                    sqlQuery += " Port_Server=" + DeviceToStore.Port_Server + ",";
                    sqlQuery += " bLocal=0 ";
                    sqlQuery += " WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();
                }              

                 ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }

        private bool UpdateDevice(DeviceInfo DeviceToStore, bool bNetwork, string machineName = null)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "UPDATE tb_Device_Def ";
                sqlQuery += "SET DeviceName='" + DeviceToStore.DeviceName + "',";
                sqlQuery += " IdDeviceType=" + (int)DeviceToStore.deviceType + ",";
                if (!string.IsNullOrEmpty(DeviceToStore.SerialFPMaster))
                    sqlQuery += " SerialFPMaster='" + DeviceToStore.SerialFPMaster + "',";
                if (!string.IsNullOrEmpty(DeviceToStore.SerialFPSlave))
                    sqlQuery += " SerialFPSlave='" + DeviceToStore.SerialFPSlave + "',";
                if (!string.IsNullOrEmpty(DeviceToStore.comMasterReader))
                {
                    sqlQuery += " comMasterReader='" + DeviceToStore.comMasterReader + "',";
                    sqlQuery += " accessReaderType=" + (int)DeviceToStore.accessReaderType +",";
                }
                if (!string.IsNullOrEmpty(DeviceToStore.comSlaveReader))
                    sqlQuery += " comSlaveReader='" + DeviceToStore.comSlaveReader + "',";
                if (!string.IsNullOrEmpty(DeviceToStore.comTempReader))
                {
                    sqlQuery += " comTempReader='" + DeviceToStore.comTempReader + "',";
                }
                if (DeviceToStore.fridgeType != FridgeType.FT_UNKNOWN)
                {
                    sqlQuery += " fridgeType=" + (int)DeviceToStore.fridgeType + ","; 
                }
                if (bNetwork)
                {
                    sqlQuery += " IP_Server ='" + DeviceToStore.IP_Server + "',";
                    sqlQuery += " Port_Server=" + DeviceToStore.Port_Server + ",";
                    sqlQuery += " bLocal=0,";
                }
                else
                    sqlQuery += " bLocal=" + DeviceToStore.bLocal.ToString() + ",";
                sqlQuery += " Enabled=" + (int)DeviceToStore.enabled + " ";
                sqlQuery += "WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";


                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

              

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }

        public bool UpdateNetworkDevice(string serialRFID, string IP_Remote, int port_Remote)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "UPDATE tb_Device_Def ";
                if (string.IsNullOrEmpty(IP_Remote))
                    sqlQuery += "SET IP_Server ='Null',";
                else
                    sqlQuery += "SET IP_Server ='" + IP_Remote + "',";
                sqlQuery += " Port_Server=" + port_Remote + " ";
                sqlQuery += "WHERE SerialRFID='" + serialRFID + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }

        public bool DeleteDevice(DeviceInfo DeviceToStore)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "DELETE FROM tb_Device_Def ";
                sqlQuery += "WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;              

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }

        #endregion
        #region TagReference

        public DataTable RecoverAllProduct()
        {
            DataTable dtGet = null;
            try
            {
                if (myConn == null) return null;
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                
                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Product_Def;";  
                SQLiteDataAdapter da = new SQLiteDataAdapter(sqlQuery,myConn);                  
                dtGet = new DataTable();
                da.Fill(dtGet); 
                
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }          
            return dtGet;
        }
        public string getProductTagID(string LotID)
        {
            string retString = null;
             lock (locker)
             {
                 try
                 {
                     if (myConn.State != System.Data.ConnectionState.Open) OpenDB();                    

                     string sqlQuery = null;
                     sqlQuery += "SELECT * FROM tb_Product_Def ";
                     sqlQuery += "WHERE ProductRef='" + LotID + "';";

                     SQLiteCommand cmd = myConn.CreateCommand();
                     cmd.CommandText = sqlQuery;
                     cmd.CommandType = System.Data.CommandType.Text;

                     SQLiteDataReader rd = cmd.ExecuteReader();

                     if (rd.HasRows)
                     {
                         rd.Read();                       
                       
                         retString = (string)rd["TagUID"]; 

                         rd.Close();
                         cmd.Dispose();
                     }
                 }
                 catch (Exception exp)
                 {
                     // On affiche l'erreur.
                     ErrorMessage.ExceptionMessageBox.Show(exp);
                 }
             }
             return retString;
        }
        private object locker = new object();
        public string[] RecoverProductInfo(string tagUID)
        {
            string[] retString = null;
            lock (locker)
            {
                try
                {
                    if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                                      

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tb_Product_Def ";
                    sqlQuery += "WHERE TagUID=@tagUID;";                    

                    SQLiteCommand cmd = myConn.CreateCommand();
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add(new SQLiteParameter("@tagUID", tagUID));                

                    SQLiteDataReader rd = cmd.ExecuteReader();                  

                    if (rd.HasRows)
                    {
                        rd.Read();                      


                        retString = new string[VarGlobal.Nb_Max_Column_Product + 2];
                        retString[0] = rd["TagUID"].ToString();
                        retString[1] = rd["ProductRef"].ToString();

                        for (int loop = 1; loop <= VarGlobal.Nb_Max_Column_Product; loop++)
                        {
                            string productRow = string.Format("ProductInfo{0}", loop);
                            retString[loop + 1] = rd[productRow].ToString();
                        }

                        rd.Close();
                        cmd.Dispose();
                    }
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
            }

            return retString;
        }
        public ArrayList RecoverProductHistory(string tagUID)
        {
            ArrayList prodHistory = null;
            string[] retString = null;
            lock (locker)
            {
                try
                {
                    if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tb_Product_History ";
                    sqlQuery += "WHERE TagUID=@tagUID ";
                    sqlQuery += "ORDER BY SortedEnterDate DESC;";

                    SQLiteCommand cmd = myConn.CreateCommand();
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add(new SQLiteParameter("@tagUID", tagUID));

                    SQLiteDataReader rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        prodHistory = new ArrayList();
                        while (rd.Read())
                        {
                            retString = new string[VarGlobal.Nb_Max_Column_Product + 4];
                            retString[0] = rd["EnterDate"].ToString();
                            retString[1] = rd["SortedEnterDate"].ToString();
                            retString[2] = rd["TagUID"].ToString();
                            retString[3] = rd["ProductRef"].ToString();

                            for (int loop = 1; loop <= VarGlobal.Nb_Max_Column_Product; loop++)
                            {
                                string productRow = string.Format("ProductInfo{0}", loop);
                                retString[loop + 3] = rd[productRow].ToString();
                            }
                            prodHistory.Add(retString);
                        }

                        rd.Close();
                        cmd.Dispose();
                    }
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
            }

            return prodHistory;
        }
        public bool IsProductExist(string tagUID)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Product_Def ";
                sqlQuery += "WHERE TagUID=@tagUID;";
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@tagUID", tagUID));

             
                SQLiteDataReader rd = cmd.ExecuteReader();
                ret = rd.HasRows;
                rd.Close();
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }
        public bool StoreProduct(ProductClassTemplate ProductToStore)
        {

            bool ret = false;

           /* System.Text.RegularExpressions.Regex myRegex = new Regex(@"[0-7]{10}");
            if (!myRegex.IsMatch(ProductToStore.tagUID)) return false;*/


            if (IsProductExist(ProductToStore.tagUID))
            {
                ret = UpdateProduct(ProductToStore);
            }
            else
            {
                ret = AddProduct(ProductToStore);
            }
            AddProductHistory(ProductToStore);
            return ret;
        }
        private bool AddProductHistory(ProductClassTemplate ProductToStore)
        {
            bool ret = false;
            try
            {
                DateTime nowDate = DateTime.Now;
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;

                sqlQuery += "INSERT INTO tb_Product_History(EnterDate,SortedEnterDate,TagUID,ProductRef";
                for (int loop = 1; loop <= VarGlobal.Nb_Max_Column_Product; loop++)
                {
                    sqlQuery += ",ProductInfo" + loop.ToString();
                }
                sqlQuery += ") VALUES('";
                sqlQuery += nowDate.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + "','";
                sqlQuery += nowDate.ToString("s") + "',";
                sqlQuery += "@tagUID,'";
                sqlQuery += ProductToStore.reference;
                for (int loop = 0; loop < VarGlobal.Nb_Max_Column_Product; loop++)
                {

                    sqlQuery += "','" + ProductToStore.productInfo[loop];
                }
                sqlQuery += "');";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@tagUID", ProductToStore.tagUID));



                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }
        private bool AddProduct(ProductClassTemplate ProductToStore)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;              

                sqlQuery += "INSERT INTO tb_Product_Def(TagUID,ProductRef";
                for (int loop = 1; loop <= VarGlobal.Nb_Max_Column_Product; loop++)
                {
                    sqlQuery += ",ProductInfo" + loop.ToString();
                }
                sqlQuery += ") VALUES(";
                sqlQuery += "@tagUID,'";
                sqlQuery += ProductToStore.reference;
                for (int loop = 0; loop < VarGlobal.Nb_Max_Column_Product; loop++)
                {

                    sqlQuery += "','" + ProductToStore.productInfo[loop];
                }
                sqlQuery += "');";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@tagUID", ProductToStore.tagUID));

        

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }
        private bool UpdateProduct(ProductClassTemplate ProductToStore)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                Hashtable ColumnInfo = GetColumnInfo();
                string sqlQuery = null;
                sqlQuery += "UPDATE tb_Product_Def ";
                sqlQuery += "SET ProductRef='" + ProductToStore.reference + "'";
                for (int i = 1; i < ColumnInfo.Count-1; i++)
                {
                    sqlQuery += ",ProductInfo" + i.ToString() + "='" + ProductToStore.productInfo[i-1] + "'";
                }
                //sqlQuery += "ProductInfo1='" + ProductToStore.description + "' ";
                sqlQuery += " WHERE TagUID=@tagUID;";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@tagUID", ProductToStore.tagUID));
               

                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }
        public bool DeleteProduct(string tagUID)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                if (IsProductExist(tagUID))
                {

                    string sqlQuery = null;
                    sqlQuery += "DELETE FROM tb_Product_Def ";
                    sqlQuery += "WHERE TagUID=@tagUID;";


                    SQLiteCommand cmd = myConn.CreateCommand();
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add(new SQLiteParameter("@tagUID", tagUID));

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                    cmd.Dispose();
                }
                else
                ret = false;
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }

        public bool AddUidHistory(string _initialUid, string _writtenUid)
        {
             bool ret = false;
             try
             {
                 if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                 string sqlQuery = null;
                 sqlQuery += "INSERT INTO tb_UidHistory (InitialUid,WrittenUid,WrittenDate) VALUES (@InitialUid,@WrittenUid,@WrittenDate)";
                 SQLiteCommand cmd = myConn.CreateCommand();
                 cmd.CommandText = sqlQuery;
                 cmd.CommandType = System.Data.CommandType.Text;
                 cmd.Parameters.Add(new SQLiteParameter("@InitialUid", _initialUid));
                 cmd.Parameters.Add(new SQLiteParameter("@WrittenUid", _writtenUid));
                 cmd.Parameters.Add(new SQLiteParameter("@WrittenDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

                 int rowAffected = cmd.ExecuteNonQuery();
                 if (rowAffected > 0) ret = true;
                 cmd.Dispose();
             }
             catch (Exception exp)
             {
                 // On affiche l'erreur.
                 ErrorMessage.ExceptionMessageBox.Show(exp);
             }
             return ret;
        }
        public string GetInitialUID(string tagUID)
        {
            string _initialUID = null;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                string sqlQuery = null;

                sqlQuery += "SELECT * FROM  tb_UidHistory ";
                sqlQuery += "WHERE WrittenUid=@WrittenUid ";
                sqlQuery += "ORDER BY IdWrite DESC;";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@WrittenUid", tagUID));
                SQLiteDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    rd.Read();
                    _initialUID = rd[1].ToString();     
                }

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return _initialUID;
        }
        public List<UidWriteHistory> GetUidHistory(string _initialUid)
        {
            List<UidWriteHistory> listWrittenUid = null;

            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                string sqlQuery = null;

                sqlQuery += "SELECT * FROM  tb_UidHistory ";
                sqlQuery += "WHERE InitialUid=@InitialUid ";
                sqlQuery += "ORDER BY IdWrite DESC;";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@InitialUid", _initialUid));
                 SQLiteDataReader rd = cmd.ExecuteReader();
                 if (rd.HasRows)
                 {
                     listWrittenUid = new List<UidWriteHistory>();
                     while (rd.Read())
                     {
                         UidWriteHistory tmpUid = new UidWriteHistory();
                         tmpUid._idWrite = Convert.ToInt32(rd["IdWrite"].ToString());
                         tmpUid._initialUid = rd["InitialUid"].ToString();
                         tmpUid._writtenUid = rd["WrittenUid"].ToString();
                         tmpUid._writtenDate = DateTime.ParseExact(rd["WrittenDate"].ToString(), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal);
                         listWrittenUid.Add(tmpUid);
                     }
                 }

                }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }            

            return listWrittenUid;

        }

        #endregion
        #region Inventory

        public bool IsInventoryExist(InventoryData IDtoStore)
        {
            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                DateTime gmtDate = IDtoStore.eventDate.ToUniversalTime();
                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Scan_Event ";
                sqlQuery += "WHERE SerialRFID='" + IDtoStore.serialNumberDevice;
                sqlQuery += "' AND SortedEventDate='" + gmtDate.ToString("s") + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                int rowAffected = cmd.ExecuteNonQuery();

                SQLiteDataReader rd = cmd.ExecuteReader();
                ret = rd.HasRows;
                rd.Close();
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }

        public bool StoreInventory(InventoryData IDtoStore , bool bStoreforDevice = false)
        {
            bool ret = false;

            try
            {
                Hashtable ColumnInfo = GetColumnInfo();

                DateTime gmtDate = IDtoStore.eventDate.ToUniversalTime();
                IDtoStore.eventDate = gmtDate;

                /*int nbTostore = IDtoStore.nbTagAdded + IDtoStore.nbTagPresent + IDtoStore.nbTagRemoved;
                int checkNbToStore = IDtoStore.dtTagAdded.Rows.Count + IDtoStore.dtTagPresent.Rows.Count + IDtoStore.dtTagRemove.Rows.Count;

                if (nbTostore != checkNbToStore)
                {
                    string info = string.Format("Added : {0}/{1} , Present : {2}/{3} , Removed : {4}/{5}", IDtoStore.nbTagAdded, IDtoStore.dtTagAdded.Rows.Count, IDtoStore.nbTagPresent, IDtoStore.dtTagPresent.Rows.Count, IDtoStore.nbTagRemoved, IDtoStore.dtTagRemove.Rows.Count);
                    ErrorMessage.ExceptionMessageBox.Show("Error Before Convert for Store", info, "Info in  store inventory DBClassSQlite");
                    return false;
                }*/
                StoredInventoryData siv = null;
                if (bStoreforDevice)
                {
                    siv = ConvertInventory.ConvertForStore(IDtoStore);
                }
                else
                {
                    siv = ConvertInventory.ConvertForStore(IDtoStore, ColumnInfo);
                }

                if (siv == null)
                {
                    string info = string.Format("Added : {0}/{1} , Present : {2}/{3} , Removed : {4}/{5}", IDtoStore.nbTagAdded, IDtoStore.dtTagAdded.Rows.Count, IDtoStore.nbTagPresent, IDtoStore.dtTagPresent.Rows.Count, IDtoStore.nbTagRemoved, IDtoStore.dtTagRemove.Rows.Count);
                    ErrorMessage.ExceptionMessageBox.Show("Error BAfter Convert for Store", info, "Info in  store inventory DBClassSQlite");
                    return false;
                }
                if (siv.serialNumberDevice.Equals("xxxxxxxx")) 
                    return false;
              

                /*BinaryFormatter bf = new BinaryFormatter();
                MemoryStream mem = new MemoryStream();
                //bf.Serialize(mem, IDtoStore);
                bf.Serialize(mem, siv);
                string idStream = Convert.ToBase64String(mem.ToArray());*/


                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;               
               

               /* sqlQuery += "INSERT INTO tb_Scan_Event(SerialRFID,bUserScan,FirstName,LastName,EventDate,SortedEventDate,NbTagAll,nbTagInReader,NbTagAdded,NbTagRemoved,InventoryStream) VALUES('";
                sqlQuery += IDtoStore.serialNumberDevice + "',";
                sqlQuery += Convert.ToByte(IDtoStore.bUserScan).ToString() + ",'";
                sqlQuery += IDtoStore.userFirstName + "','";
                sqlQuery += IDtoStore.userLastName + "','";
                sqlQuery += gmtDate.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + "','";
                sqlQuery += gmtDate.ToString("s") + "',";
                sqlQuery += IDtoStore.nbTagAll.ToString() + ",";
                sqlQuery += IDtoStore.nbTagPresent.ToString() + ",";
                sqlQuery += IDtoStore.nbTagAdded.ToString() + ",";
                sqlQuery += IDtoStore.nbTagRemoved.ToString() + ",'";
                sqlQuery += idStream + "');";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                int rowAffected = cmd.ExecuteNonQuery();

                if (rowAffected > 0) ret = true;
                cmd.Dispose();*/

                sqlQuery += "INSERT INTO tb_Scan_Event(SerialRFID,bUserScan,FirstName,LastName,EventDate,SortedEventDate,NbTagAll,nbTagInReader,NbTagAdded,NbTagRemoved,SpareData1,SpareData2) VALUES('";
                sqlQuery += IDtoStore.serialNumberDevice + "',";
                sqlQuery += Convert.ToByte(IDtoStore.bUserScan).ToString() + ",'";
                sqlQuery += IDtoStore.userFirstName + "','";
                sqlQuery += IDtoStore.userLastName + "','";
                sqlQuery += gmtDate.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + "','";
                sqlQuery += gmtDate.ToString("s") + "',";
                sqlQuery += IDtoStore.nbTagAll.ToString() + ",";
                sqlQuery += IDtoStore.nbTagPresent.ToString() + ",";
                sqlQuery += IDtoStore.nbTagAdded.ToString() + ",";
                sqlQuery += IDtoStore.nbTagRemoved.ToString() + ",";
                if (string.IsNullOrEmpty(IDtoStore.spareData1))
                    sqlQuery += "' ',";
                else
                    sqlQuery += "'" + IDtoStore.spareData1 + "',";
                if (string.IsNullOrEmpty(IDtoStore.spareData2))
                    sqlQuery += "' ');";
                else
                    sqlQuery += "'" + IDtoStore.spareData2 + "');";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();

                sqlQuery = @"SELECT last_insert_rowid()";
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                int lastId = Convert.ToInt32(cmd.ExecuteScalar());

                siv.IdScanEvent = lastId;
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream mem = new MemoryStream();
                //bf.Serialize(mem, IDtoStore);
                bf.Serialize(mem, siv);
                string idStream = Convert.ToBase64String(mem.ToArray());

                sqlQuery = "UPDATE tb_Scan_Event SET InventoryStream='" + idStream + "' ";
                sqlQuery += "WHERE IdScanEvent=" + lastId + ";";             

                
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                int rowAffected = cmd.ExecuteNonQuery(); 

                if (rowAffected > 0) ret = true;
                cmd.Dispose();

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);

            }

            return ret;

        }
        public InventoryData[] GetInventoryFromSpareData(DeviceInfo di, string spareData1, string spareData2)
        {
            InventoryData[] inventoryArray = null;
            ArrayList listInventory = new ArrayList();

            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                Hashtable ColumnInfo = GetColumnInfo();

                string sqlQuery = null;
                sqlQuery += "SELECT InventoryStream,SortedEventDate FROM tb_Scan_Event WHERE 1=1 ";

                sqlQuery += "AND SpareData1='" + spareData1 + "' ";
                sqlQuery += "AND SpareData2='" + spareData2 + "' ";
                if (di != null)
                    sqlQuery += "AND SerialRFID='" + di.SerialRFID + "' ";
                sqlQuery += "ORDER BY SortedEventDate DESC;";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream mem = new MemoryStream(Convert.FromBase64String(rd[0].ToString()));
                        //InventoryData dt = new InventoryData();
                        //dt = (InventoryData)bf.Deserialize(mem);

                        StoredInventoryData siv = new StoredInventoryData();
                        siv = (StoredInventoryData)bf.Deserialize(mem);
                        InventoryData dt = ConvertInventory.ConvertForUse(siv, ColumnInfo);

                        DateTime utcDate = dt.eventDate;
                        dt.eventDate = utcDate.ToLocalTime();

                        listInventory.Add(dt);
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            if (listInventory.Count > 0)
            {
                inventoryArray = new InventoryData[listInventory.Count];
                listInventory.CopyTo(inventoryArray);
            }
            return inventoryArray;
        }


        public int getRowInsertIndex()
        {
            int rowInserted = -1;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT last_insert_rowid();";
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                rowInserted = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Dispose();
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return rowInserted;
        }
        public bool storeTagEvent(InventoryData IDtoStore, int IdScan)
        {
            bool ret = false;
            try
            {
                DeviceInfo di = RecoverDevice(IDtoStore.serialNumberDevice);
                if (di == null) return false;

                if ((di.deviceType == DeviceType.DT_SBR) || (di.deviceType == DeviceType.DT_STR))
                {
                    trans = myConn.BeginTransaction();
                    foreach (string TagAdded in IDtoStore.listTagAll)
                        storeTag(IdScan, TagAdded, TagEventType.TET_Present);
                    trans.Commit();
                }
                else
                {
                    trans = myConn.BeginTransaction();
                    foreach (string TagAdded in IDtoStore.listTagAdded)
                        storeTag(IdScan, TagAdded, TagEventType.TET_Added);
                    trans.Commit();
                    trans = myConn.BeginTransaction();
                    foreach (string TagRemove in IDtoStore.listTagRemoved)
                        storeTag(IdScan, TagRemove, TagEventType.TET_Removed);
                    trans.Commit();
                }
               

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }
        private bool storeTag(int idScan, string UID, TagEventType TagEventType)
        {
            bool ret = false;
            if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

            string sqlQuery = null;
            sqlQuery += "INSERT INTO tb_Tag_Event (idScanEvent,TagUID,IdTagEventType) VALUES(";
            sqlQuery += idScan + ",";
            sqlQuery += "@UID,";
            sqlQuery += (int)TagEventType + ");";

            SQLiteCommand cmd = myConn.CreateCommand();
            cmd.CommandText = sqlQuery;
            cmd.Parameters.Add(new SQLiteParameter("@UID", UID));
            cmd.CommandType = System.Data.CommandType.Text;
            int rowAffected = cmd.ExecuteNonQuery();

            if (rowAffected > 0) ret = true;
            cmd.Dispose();

            return ret;
        }
        public InventoryData[] GetInventory(DeviceInfo di, UserClassTemplate uct , int nbData = 100)
        {
            InventoryData[] inventoryArray = null;
            ArrayList listInventory = new ArrayList();

            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                Hashtable ColumnInfo = GetColumnInfo();

                string sqlQuery = null;
                sqlQuery += "SELECT InventoryStream,SortedEventDate FROM tb_Scan_Event WHERE 1=1 ";
                if (uct != null)
                {
                    sqlQuery += "AND FirstName='" + uct.firstName + "' ";
                    sqlQuery += "AND LastName='" + uct.lastName + "' ";
                }
                if (di != null)
                    sqlQuery += "AND SerialRFID='" + di.SerialRFID + "' ";
                sqlQuery += "ORDER BY SortedEventDate DESC LIMIT " + nbData + ";";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        if (rd[0] == DBNull.Value)  continue;
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream mem = new MemoryStream(Convert.FromBase64String(rd[0].ToString()));
                        //InventoryData dt = new InventoryData();
                        //dt = (InventoryData)bf.Deserialize(mem);

                        StoredInventoryData siv = new StoredInventoryData();
                        siv = (StoredInventoryData)bf.Deserialize(mem);
                        InventoryData dt = ConvertInventory.ConvertForUse(siv, ColumnInfo);

                        DateTime utcDate = dt.eventDate;
                        dt.eventDate = utcDate.ToLocalTime();

                        listInventory.Add(dt);
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            if (listInventory.Count > 0)
            {
                inventoryArray = new InventoryData[listInventory.Count];
                listInventory.CopyTo(inventoryArray);
            }
            return inventoryArray;
        }
        public string[] GetInventoryBefore(string SerialRFID, string DateTimetoParse)
        {
            ArrayList listInventory = new ArrayList();
            string[] strData = null;
            try
            {

                //DateTime dt = DateTime.Parse(DateTimetoParse);
                DateTime dt = DateTime.ParseExact(DateTimetoParse, "yyyy-MM-dd HH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);

                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT InventoryStream,SortedEventDate FROM tb_Scan_Event WHERE 1=1 ";
                sqlQuery += "AND SerialRFID='" + SerialRFID + "' ";
                sqlQuery += "AND SortedEventDate<'" + dt.ToUniversalTime().ToString("s") + "' ";
                sqlQuery += "ORDER BY SortedEventDate DESC";
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        if (rd[0] == DBNull.Value) continue;
                        listInventory.Add(rd[0].ToString());
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            if (listInventory.Count > 0)
            {
                strData = new string[listInventory.Count];
                int nIndex = 0;
                foreach (string dt in listInventory)
                    strData[nIndex++] = dt;
            }
            return strData;
        }
        public string[] GetInventoryAfter(string SerialRFID, string DateTimetoParse)
        {
            ArrayList listInventory = new ArrayList();
            string[] strData = null;
            try
            {

                //DateTime dt = DateTime.Parse(DateTimetoParse);
                DateTime dt = DateTime.ParseExact(DateTimetoParse, "yyyy-MM-dd HH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);

                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT InventoryStream,SortedEventDate FROM tb_Scan_Event WHERE 1=1 ";
                sqlQuery += "AND SerialRFID='" + SerialRFID + "' ";
                sqlQuery += "AND SortedEventDate>'" + dt.ToUniversalTime().ToString("s") + "' ";
                sqlQuery += "ORDER BY SortedEventDate ASC";
                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        if (rd[0] == DBNull.Value) continue;
                        listInventory.Add(rd[0].ToString());
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            if (listInventory.Count > 0)
            {
                strData = new string[listInventory.Count];
                int nIndex = 0;
                foreach (string dt in listInventory)
                    strData[nIndex++] = dt;
            }
            return strData;
        }
        public string[] GetInventory(string SerialRFID, string DateTimetoParse)
        {

            ArrayList listInventory = new ArrayList();   
            string[] strData = null;
            try
            {

                //DateTime dt = DateTime.Parse(DateTimetoParse);
                DateTime dt = DateTime.ParseExact(DateTimetoParse, "yyyy-MM-dd HH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.AssumeUniversal );
                
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT InventoryStream,SortedEventDate FROM tb_Scan_Event WHERE 1=1 ";
                sqlQuery += "AND SerialRFID='" + SerialRFID + "' ";
                sqlQuery += "AND SortedEventDate>'" + dt.ToUniversalTime().ToString("s") + "';";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        if (rd[0] == DBNull.Value) continue;
                        listInventory.Add(rd[0].ToString());
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            if (listInventory.Count > 0)
            {
                strData = new string[listInventory.Count];
                int nIndex = 0;
                foreach (string dt in listInventory)
                    strData[nIndex++] = dt;
            }
            return strData;
        }
        public string[] GetInventoryFromData(string SerialRFID, string spareData1, string spareData2)
        {

            ArrayList listInventory = new ArrayList();
            string[] strData = null;
            try
            {

                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;
                sqlQuery += "SELECT InventoryStream,SortedEventDate FROM tb_Scan_Event WHERE 1=1 ";

                sqlQuery += "AND SpareData1='" + spareData1 + "' ";
                sqlQuery += "AND SpareData2='" + spareData2 + "' ";
                sqlQuery += "AND SerialRFID='" + SerialRFID + "';";


                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        if (rd[0] == DBNull.Value) continue;
                        listInventory.Add(rd[0].ToString());
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            if (listInventory.Count > 0)
            {
                strData = new string[listInventory.Count];
                int nIndex = 0;
                foreach (string dt in listInventory)
                    strData[nIndex++] = dt;
            }
            return strData;
        }
        public InventoryData GetLastScan(string serialRFID)
        {
            InventoryData retData = null;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                Hashtable ColumnInfo = GetColumnInfo();


                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Scan_Event ";
                sqlQuery += "WHERE SerialRFID='" + serialRFID + "' ";
                sqlQuery += "ORDER BY SortedEventDate DESC LIMIT 1";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    rd.Read();
                    if (rd["InventoryStream"] == DBNull.Value) return null;
                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream mem = new MemoryStream(Convert.FromBase64String((string)rd["InventoryStream"]));
                    //retData = new InventoryData();
                    //retData = (InventoryData)bf.Deserialize(mem);
                    StoredInventoryData siv = new StoredInventoryData();
                    siv = (StoredInventoryData)bf.Deserialize(mem);
                    retData = ConvertInventory.ConvertForUse(siv, ColumnInfo);

                    DateTime utcDate = retData.eventDate;
                    retData.eventDate = utcDate.ToLocalTime();

                    rd.Close();
                    cmd.Dispose();
                }

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.

                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return retData;
        }
        public InventoryData GetLastScanFromID(string serialRFID, int IdEvent)
        {
            InventoryData retData = null;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                Hashtable ColumnInfo = GetColumnInfo();


                string sqlQuery = null;
                sqlQuery += "SELECT * FROM tb_Scan_Event ";
                sqlQuery += "WHERE SerialRFID='" + serialRFID + "' ";
                sqlQuery += "AND IdScanEvent=" + IdEvent + " ;";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    rd.Read();
                    if (rd["InventoryStream"] == DBNull.Value) return null;
                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream mem = new MemoryStream(Convert.FromBase64String((string)rd["InventoryStream"]));
                    //retData = new InventoryData();
                    //retData = (InventoryData)bf.Deserialize(mem);
                    StoredInventoryData siv = new StoredInventoryData();
                    siv = (StoredInventoryData)bf.Deserialize(mem);
                    retData = ConvertInventory.ConvertForUse(siv, ColumnInfo);

                    DateTime utcDate = retData.eventDate;
                    retData.eventDate = utcDate.ToLocalTime();

                    rd.Close();
                    cmd.Dispose();
                }

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.

                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return retData;
        }
        public TagEventClass[] GetTagEvent(string tagUID)
        {
            TagEventClass[] tagEventArray = null;
            ArrayList listTagEvent = new ArrayList();

            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                string sqlQuery = null;
                sqlQuery += "SELECT TTE.*,TSE.SerialRFID,TDD.DeviceName,TSE.SortedEventDate,TSE.bUserScan,TSE.FirstName,TSE.LastName ";
                sqlQuery += "FROM (( tb_Tag_Event AS TTE ";
                sqlQuery += "INNER JOIN tb_Scan_Event AS TSE ON TTE.IdScanEvent=TSE.IdScanEvent) ";
                sqlQuery += "INNER JOIN tb_Device_Def AS TDD ON TSE.SerialRFID=TDD.SerialRFID) ";
                sqlQuery += "WHERE TTE.TagUID=@tagUID ";
                sqlQuery += "ORDER BY TSE.SortedEventDate DESC";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@tagUID", tagUID));

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        TagEventClass tec = new TagEventClass(); 
                        DateTime TmpUtc =   DateTime.ParseExact((string)rd["SortedEventDate"], "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);                        
                        DateTime TmpLocal = TmpUtc.ToLocalTime();
                        tec.eventSortedDate = TmpLocal.ToString("s");
                        tec.eventDate = (string)TmpLocal.ToString("G");
                        tec.serialRFID = (string)rd["SerialRFID"];

                        int eventType = int.Parse(rd["IdTagEventType"].ToString());
                        switch ((TagEventType)eventType)
                        {
                            case TagEventType.TET_Removed: tec.tagEventType = "Removed"; break;
                            case TagEventType.TET_Added: tec.tagEventType = "Added"; break;
                            case TagEventType.TET_Present: tec.tagEventType = "Present"; break;
                        }

                        tec.tagUID = (string)rd["TagUID"];
                        tec.DeviceName = (string)rd["DeviceName"];
                        tec.FirstName = (string)rd["FirstName"];
                        tec.LastName = (string)rd["LastName"];
                        listTagEvent.Add(tec);
                    }
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.

                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            if (listTagEvent.Count > 0)
            {
                tagEventArray = new TagEventClass[listTagEvent.Count];
                listTagEvent.CopyTo(tagEventArray);
            }
            return tagEventArray;
        }
        public TagEventClass GetLastTagEvent(string tagUID)
        {
            TagEventClass tec = null;
            ArrayList listTagEvent = new ArrayList();

            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                string sqlQuery = null;
                sqlQuery += "SELECT TTE.*,TSE.SerialRFID,TDD.DeviceName,TSE.SortedEventDate,TSE.bUserScan,TSE.FirstName,TSE.LastName ";
                sqlQuery += "FROM (( tb_Tag_Event AS TTE ";
                sqlQuery += "INNER JOIN tb_Scan_Event AS TSE ON TTE.IdScanEvent=TSE.IdScanEvent) ";
                sqlQuery += "INNER JOIN tb_Device_Def AS TDD ON TSE.SerialRFID=TDD.SerialRFID) ";
                sqlQuery += "WHERE TTE.TagUID=@tagUID ";
                sqlQuery += "ORDER BY TSE.SortedEventDate DESC LIMIT 1";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add(new SQLiteParameter("@tagUID", tagUID));

                SQLiteDataReader rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    rd.Read();
                    
                        tec = new TagEventClass();
                        DateTime TmpUtc = DateTime.ParseExact((string)rd["SortedEventDate"], "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);
                        DateTime TmpLocal = TmpUtc.ToLocalTime();
                        tec.eventSortedDate = TmpLocal.ToString("s");
                        tec.eventDate = (string)TmpLocal.ToString("G");
                        tec.serialRFID = (string)rd["SerialRFID"];

                        int eventType = int.Parse(rd["IdTagEventType"].ToString());
                        switch ((TagEventType)eventType)
                        {
                            case TagEventType.TET_Removed: tec.tagEventType = "Removed"; break;
                            case TagEventType.TET_Added: tec.tagEventType = "Added"; break;
                            case TagEventType.TET_Present: tec.tagEventType = "Present"; break;
                        }

                        tec.tagUID = (string)rd["TagUID"];
                        tec.DeviceName = (string)rd["DeviceName"];
                        tec.FirstName = (string)rd["FirstName"];
                        tec.LastName = (string)rd["LastName"];
                        listTagEvent.Add(tec);
                    
                    rd.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.

                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return tec;
        }
        public bool DeleteOldInventory(string serialRFID, int NbToKeep)
        {

            bool ret = false;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();

                string sqlQuery = null;

                sqlQuery += "UPDATE tb_Scan_Event SET InventoryStream=' ' ";
                sqlQuery += "WHERE SerialRFID='" + serialRFID + "' ";
                sqlQuery += "AND IdScanEvent NOT IN ";
                sqlQuery += "(SELECT IdScanEvent FROM tb_Scan_Event ";
                sqlQuery += "WHERE SerialRFID='" + serialRFID + "' ";
                sqlQuery += "ORDER BY SortedEventDate DESC LIMIT " + NbToKeep.ToString() +")";

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();

                sqlQuery = null;

                sqlQuery += "DELETE FROM  tb_Scan_Event  ";
                sqlQuery += "WHERE SerialRFID='" + serialRFID + "' ";
                sqlQuery += "AND IdScanEvent NOT IN ";
                sqlQuery += "(SELECT IdScanEvent FROM tb_Scan_Event ";
                sqlQuery += "WHERE SerialRFID='" + serialRFID + "' ";
                sqlQuery += "ORDER BY IdScanEvent DESC LIMIT 2000) ";

                cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();

                ret = true;

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ret;
        }


        #endregion
        #region import

        public DataSet ImportCSVToDS(string filename)
        {
            DataTable dt = null;
            DataSet ds = null;

            try
            {
                Hashtable ColumnInfo = GetColumnInfo();
                Hashtable ColumnCSVInfo = GetColumnCSVInfo();

                string CSVFilePathName = filename;
                string[] Lines = File.ReadAllLines(CSVFilePathName);
                string[] Fields;
                Fields = Lines[0].Split(new char[] { ',' });
                int Cols = Fields.GetLength(0);
                dt = new DataTable();
                //1st row must be column names; force lower case to ensure matching later on.
                for (int i = 0; i < Cols; i++)
                {
                    for (int loop = 0; loop < ColumnInfo.Count; loop++)
                    {
                        if (Fields[i].Equals(ColumnCSVInfo[loop])) Fields[i] = ColumnInfo[loop].ToString();
                    }

                    dt.Columns.Add(Fields[i], typeof(string));
                }
                DataRow Row;
                for (int i = 1; i < Lines.GetLength(0); i++)
                {
                    Fields = Lines[i].Split(new char[] { ',' });
                    Row = dt.NewRow();
                    for (int f = 0; f < Cols; f++)
                    {
                        if (Fields[f].Contains("\""))
                        {
                            int start = Fields[f].IndexOf("\"") + 1;
                            int end = Fields[f].LastIndexOf("\"");
                            int lenght = end - start;
                            Fields[f] = Fields[f].Substring(start, lenght);
                        }
                        Row[f] = Fields[f];
                    }
                    dt.Rows.Add(Row);
                }

                ds = new DataSet();
                ds.Tables.Add(dt);
            }
            catch (IOException exp)
            {
                MessageBox.Show("The File is already opened by another application \r\n Please close this application before try to import!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ds;

        }
        public DataSet ImportExcelToDS(string filename)
        {
            OleDbConnection objConn;
            DataTable dt = null;
            DataSet ds = null;
            string sheetName = "Import";
            String[] excelSheets = null;
            try
            {
                string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                                 "Data Source=" + filename + ";" +
                                 "Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\"";               
               
                
                    objConn = new OleDbConnection(strConn);
                    objConn.Open();
                    dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new Object[] { null, null, null, "TABLE" });

                    if (dt != null)
                    {


                        excelSheets = new String[dt.Rows.Count];
                        int i = 0;

                        // Add the sheet name to the string array.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[i] = row["TABLE_NAME"].ToString();
                            i++;
                        }
                    }

                    sheetName = excelSheets[0];
                    objConn.Close();
               // }

                
                OleDbDataAdapter da;
                ds = new DataSet();
                if (sheetName.Contains("$"))
                    da = new OleDbDataAdapter("SELECT * FROM [" + sheetName +"]", strConn);
                else
                    da = new OleDbDataAdapter("SELECT * FROM [" + sheetName +"$]", strConn);
                da.Fill(ds);
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ds;
        }
        public DataSet ImportExcelToDS(string filename, string sheetName)
        {           
            DataSet ds = null;           
            try
            {
                string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                                 "Data Source=" + filename + ";" +
                                 "Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\"";

              
                OleDbDataAdapter da;
                ds = new DataSet();
                if (sheetName.Contains("$"))
                    da = new OleDbDataAdapter("SELECT * FROM [" + sheetName + "]", strConn);
                else
                    da = new OleDbDataAdapter("SELECT * FROM [" + sheetName + "$]", strConn);
                da.Fill(ds);
            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            return ds;
        }
        #endregion
        #region export

       
        //KB use ole DB impose access driver
        public static void Export(DataTable dt, string filepath, string tablename)
        {
            string connString;
            if (!DataClass.ExcelInfo.isExcel2010_64bits())
            {
               //  OLEDB 4.0 pas inscrit en 64 bits
                connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                      filepath + ";Extended Properties=Excel 8.0;";
            }
            else
            {
                connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        filepath + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES\"";
            }
            try
            {
                using (OleDbConnection con = new OleDbConnection(connString))
                {
                    con.Open();
                    StringBuilder strSQL = new StringBuilder();
                    strSQL.Append("CREATE TABLE ").Append("[" + tablename + "]");
                    strSQL.Append("(");
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {                       
                        strSQL.Append("[" + dt.Columns[i].ColumnName.Replace(".", "#") + "] text,");
                    }
                    strSQL = strSQL.Remove(strSQL.Length - 1, 1);
                    strSQL.Append(")");

                    OleDbCommand cmd = new OleDbCommand(strSQL.ToString(), con);
                    cmd.ExecuteNonQuery();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strSQL.Length = 0;
                        StringBuilder strfield = new StringBuilder();
                        StringBuilder strvalue = new StringBuilder();
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            strfield.Append("[" + dt.Columns[j].ColumnName.Replace(".", "#") + "]");
                            strvalue.Append("'" + dt.Rows[i][j].ToString() + "'");
                            if (j != dt.Columns.Count - 1)
                            {
                                strfield.Append(",");
                                strvalue.Append(",");
                            }
                            else
                            {
                            }
                        }
                        cmd.CommandText = strSQL.Append(" insert into [" + tablename + "]( ")
                            .Append(strfield.ToString())
                            .Append(") values (").Append(strvalue).Append(")").ToString();
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
               
            }
            catch (Exception ex)
            {
                ErrorMessage.ExceptionMessageBox.Show(ex);
            }
        }
     

        public static void ExportToTemplate(DataTable dt, string filepath, string tablename)
        {
            //excel 2003
            //string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
            //     filepath + ";Extended Properties=Excel 8.0;";
            //Excel 2007



            string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                   filepath + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES\"";
            try
            {
                using (OleDbConnection con = new OleDbConnection(connString))
                {
                    con.Open();
                    StringBuilder strSQL = new StringBuilder();
                    OleDbCommand cmd = new OleDbCommand(strSQL.ToString(), con);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strSQL.Length = 0;
                        StringBuilder strfield = new StringBuilder();
                        StringBuilder strvalue = new StringBuilder();
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            strfield.Append("[" + dt.Columns[j].ColumnName + "]");
                            strvalue.Append("'" + dt.Rows[i][j].ToString() + "'");
                            if (j != dt.Columns.Count - 1)
                            {
                                strfield.Append(",");
                                strvalue.Append(",");
                            }
                            else
                            {
                            }
                        }
                        cmd.CommandText = strSQL.Append(" insert into [" + tablename + "]( ")
                            .Append(strfield.ToString())
                            .Append(") values (").Append(strvalue).Append(")").ToString();
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }

            }
            catch (Exception ex)
            {
                ErrorMessage.ExceptionMessageBox.Show(ex);
            }
        }   
       
        #endregion  
        #region stat

        public int GetInventoryCount(string serialRFID = null, string firstName = null, string lastName = null, string dateFrom = null, string dateTo = null)
        {
            int ret = -1;
            try
            {
                if (myConn.State != System.Data.ConnectionState.Open) OpenDB();
                string sqlQuery = null;

                sqlQuery += "SELECT COUNT(*) FROM tb_Scan_Event WHERE 1=1 ";

                if (!string.IsNullOrEmpty(serialRFID))
                    sqlQuery += "AND [SerialRFID]='" + serialRFID + "' ";

                if ((!string.IsNullOrEmpty(firstName)) && (!string.IsNullOrEmpty(lastName)))
                {
                    sqlQuery += "AND [FirstName]='" + firstName + "' ";
                    sqlQuery += "AND [LastName]='" + lastName + "' ";
                }

                if (dateFrom != null)
                {
                    DateTime dtFrom = DateTime.ParseExact(dateFrom, "yyyy-MM-dd HH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);
                    sqlQuery += "AND [SortedEventDate]>'" + dtFrom.ToUniversalTime().ToString("s") + "' ";
                }

                if (dateTo != null)
                {
                    DateTime dtTo= DateTime.ParseExact(dateTo, "yyyy-MM-dd HH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);
                    sqlQuery += "AND [SortedEventDate]<'" + dtTo.ToUniversalTime().ToString("s")  + "' ";
                }

                SQLiteCommand cmd = myConn.CreateCommand();
                cmd.CommandText = sqlQuery;
                cmd.CommandType = System.Data.CommandType.Text;

                ret = Convert.ToInt32(cmd.ExecuteScalar());

                cmd.Dispose();

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }

            return ret;
        }
        #endregion



    }
}
