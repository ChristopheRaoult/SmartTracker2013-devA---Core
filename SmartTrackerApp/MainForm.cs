using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using Conversive.AutoUpdater;
using DotBits.Configuration;
using ErrorMessage;
using smartTracker.Properties;
using TcpIP_class;
using System.Runtime.InteropServices;
using System.Diagnostics;
using DBClass;
using DB_Class_SQLite;

using LogicNP.CryptoLicensing;



namespace smartTracker
{
    public partial class MainForm : Form 
    {      
       
        AutoUpdater _autoUpdater;
        UserMngtForm _userMngtForm = null;
        LocalDeviceForm _localDeviceForm = null;
        LiveDataForm _liveDataForm = null;
        LiveDataForDeviceForm _liveDataForDeviceForm = null;  
        ItemListForm _itemListForm = null;
        ItemHistoryForm _itemHistoryForm = null;
        ReaderHistoryForm _readerHistoryForm = null;
        ServerMngtForm _serverMngtForm = null;
        RemoteReaderForm _remoteReaderForm = null;
        DbColumnForm _dbColumnForm = null;
        UserStatForm _userStatForm = null;
        CompareInventoryReportForm _compInvForm = null;
        AlertMngmtForm _alf = null;
        Formule _fv = null;
        BoxModeConfig _bmc = null;
        DbSqlForm _dbSqlForm = null;
        LiveDataViewerForm _ldvf = null;
        WeightScalefrm _wsf = null;
        ExportFrm _expf = null;
        TempHistory _thw = null;

        private bool bUseLightLiveData = false;

        public TcpIpServer TcpServeur = null;
        private int _tcpServerPort = 6901;
        private bool _bRestart = false;
        private bool _bShowServer = false;
        private bool _bScreenSaver = true;      
        private bool _bViewerMode = false;
        private bool _bAutoUpdate = false;
       

        #region license
        public const string ValidationKey = "AMAAMACkOWrpGOTlxDOCrNsf7rMEHJpbKN7H+nW5L7w+1F6ocA4/Rju3OgjaiHMrfVANNYUDAAEAAQ==";
        public const string LicenseCode = "lgIAAcdX3ffum80BWgAnAE1hY2hpbmVJRD0jQ29tcGFueT0jTmFtZT0jTWFjaGluZSBOYW1lPSkRM7ksui/iyJSHpc4P75Q8BZfK/8YCOHTVDle/F1/JF4KiTqjFDv0umGQh+v0HCQ==";
        CryptoLicense CreateLicense()
        {
            CryptoLicense ret = new CryptoLicense(LicenseCode,ValidationKey);   
            return ret;
        }

/*
        string getMachineID()
        {
            string sebBaseString = Encryption.Boring(Encryption.InverseByBase(SystemInfo.GetSystemInfo("TestCrypto"), 10)).Substring(0, 25).ToUpper();
            string machineID = sebBaseString.Substring(0, 5);
            machineID += "-" + sebBaseString.Substring(5, 5);
            machineID += "-" + sebBaseString.Substring(10, 5);
            machineID += "-" + sebBaseString.Substring(15, 5);
            machineID += "-" + sebBaseString.Substring(20, 5);
            return machineID;
        }
*/

        public enum LicenceStatus
        {
            LsValid = 0x00,
            LsTrial = 0x01,
            LsExpired = 0x02,
            LsNearToExpired = 0x03,
            LsDateRollBack = 0x04,
        }

        readonly CryptoLicense _license = null;
        LicenceStatus TestLicence()
        {
            LicenceStatus ret = LicenceStatus.LsValid;
            if (_license.Status == (LicenseStatus.DateRollbackDetected|LicenseStatus.UsageDaysExceeded)) ret = LicenceStatus.LsDateRollBack;
            else if (_license.Status == LicenseStatus.DateRollbackDetected) ret = LicenceStatus.LsDateRollBack;
            else if (_license.IsEvaluationExpired()) ret = LicenceStatus.LsExpired;   
            else if (_license.IsEvaluationLicense()) ret = LicenceStatus.LsTrial;              
            else if (_license.HasDateExpires)
            {
                TimeSpan remain = _license.DateExpires.Subtract(DateTime.Now);
                if (remain.Days < 15) ret = LicenceStatus.LsNearToExpired;
                if (remain.Days < 0) ret = LicenceStatus.LsExpired;               
            }                 

            _license.Dispose();
            return ret;
        }

        #endregion

        private void AutoUpdate()
        {
           
            _autoUpdater = new AutoUpdater();

            /*if (DataClass.ExcelInfo.isExcel2010_64bits())
                this.autoUpdater.ConfigURL = "http://www.spacecode.com/smartTracker_2013/x64/UpdateVersion.xml";
            else if (bShowServer) // For device update 32bits oon purpose
                this.autoUpdater.ConfigURL = "http://www.spacecode.com/smartTracker_2013/x86_Server/UpdateVersion.xml";
            else
                this.autoUpdater.ConfigURL = "http://www.spacecode.com/smartTracker_2013/x86/UpdateVersion.xml";*/

            // Keep only x86 version as now compatible
            // Keep server version to put only server if required

            _autoUpdater.ConfigURL = _bShowServer ? "http://www.spacecode.com/smartTracker_2013/x86_Server/UpdateVersion.xml" : "http://www.spacecode.com/smartTracker_2013/x86/UpdateVersion.xml";


            //this.autoUpdater.DownloadForm = new Confirm(this.autoUpdater);
            _autoUpdater.ProgressForm = new UpdateDownload(_autoUpdater);

            if (_autoUpdater.UpdateRequired())
            {
                Text = ResStrings.strUpdate;
                _autoUpdater.AutoDownload = true;
                _autoUpdater.AutoRestart = true;
                _autoUpdater.updateThread();
            }
        }

        public MainForm()
        {
            AssemblyResolver.HandleUnresovledAssemblies();

            InitializeComponent();             
            InitServer();

            _license = CreateLicense();
            _license.StorageMode = LicenseStorageMode.ToRegistry;
            //license.Remove();
            if (!_license.Load())
            {
                // When app runs for first time, the load will fail, so specify an evaluation code....
                _license.LicenseCode = "lgIAAcdX3ffum80BWgAnAE1hY2hpbmVJRD0jQ29tcGFueT0jTmFtZT0jTWFjaGluZSBOYW1lPSkRM7ksui/iyJSHpc4P75Q8BZfK/8YCOHTVDle/F1/JF4KiTqjFDv0umGQh+v0HCQ==";
                _license.Save();
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.ShowInTaskbar = true;
            notifyIcon.Visible = false;

            if (_liveDataForm != null)
            {
                _liveDataForm.WindowState = FormWindowState.Maximized;
            }
        }
          private void MainForm_Resize(object sender, EventArgs e)
        {
              if (_bShowServer)
              {
                  if (this.WindowState == FormWindowState.Minimized)
                  {
                      notifyIcon.Visible = true;
                      notifyIcon.ShowBalloonTip(3000);
                      this.ShowInTaskbar = true;
                  }
              }
        }
         private void userGuideToolStripMenuItem_Click(object sender, EventArgs e)
        {
             string pathDoc = Application.StartupPath + Path.DirectorySeparatorChar + "doc" + Path.DirectorySeparatorChar + "UserGuide.pdf";
            try
            {
                if (File.Exists(pathDoc))
                {
                    ProcessStartInfo pUsb = new ProcessStartInfo(pathDoc);
                    Process.Start(pUsb);
                }
            }
            catch
            {

            }

        }

        private void MainForm_Load(object sender, EventArgs e)
        {          
            string[] picList = Directory.GetFiles(Application.StartupPath, "*.png");
            foreach (string f in picList)
            {
                File.Delete(f);
            }
            bool.TryParse(ConfigurationManager.AppSettings["bShowServer"], out _bShowServer);
            bool.TryParse(ConfigurationManager.AppSettings["bAutoUpdate"], out _bAutoUpdate);
            try
            {                

                if (_bAutoUpdate | _bShowServer)
                    AutoUpdate();
            }
            catch
            {

            }

            DBClassSQLite dbTest = null;
            try
            {
               

                dbTest = new DBClassSQLite();
                if (!dbTest.isValidDBPath())
                {
                    MessageBox.Show(ResStrings.strErrorDb, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Application.Exit();
                    return;
                }

                dbTest.OpenDB();
                if (dbTest.isOpen())
                {
                    try
                    {
                        dbTest.isTableExist("tb_Version");
                        dbTest.CloseDB();
                    }
                    catch
                    {
                        dbTest.CloseDB();
                        MessageBox.Show(ResStrings.strErrorDb, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Application.Exit();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(ResStrings.strErrorDb);
                    Application.Exit();
                    return;
                }
            }
            catch
            {
                if (dbTest != null)
                {
                    if (dbTest.isOpen())
                        dbTest.CloseDB();
                }
                MessageBox.Show(ResStrings.strErrorDb, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
                return;
            }
            try
            {

                CheckConfig();
                //updateSqliteDB();
                //updateSqlServer();
                updateDB();
                bool.TryParse(ConfigurationManager.AppSettings["bShowServer"], out _bShowServer);
                bool.TryParse(ConfigurationManager.AppSettings["bScreenSaver"], out _bScreenSaver);
                bool.TryParse(ConfigurationManager.AppSettings["bViewerMode"], out  _bViewerMode);   
              
               // this.BackgroundImage = Properties.Resources.bckMDI;                
                //this.BackgroundImageLayout = ImageLayout.Center;
                Form findForm = FindForm();
                if (findForm != null)
                {
                    var controls = findForm.Controls;
                    foreach (Control control in controls)
                    {
                        if (control is MdiClient)
                        {
                            control.BackColor = Color.White;
                        }
                    }
                }
                LicenceStatus licStatus = TestLicence();

                if ((licStatus == LicenceStatus.LsDateRollBack) || (licStatus == LicenceStatus.LsNearToExpired) || (licStatus == LicenceStatus.LsTrial) || (licStatus == LicenceStatus.LsExpired))
                {
                    MngLicense mngLic = new MngLicense(_license,ValidationKey);
                    mngLic.ShowDialog();
                    licStatus = TestLicence();
                }

                if (licStatus == LicenceStatus.LsExpired) return;
                if (licStatus == LicenceStatus.LsDateRollBack) return;
                
                /*liveDataGroupModeToolStripMenuItem.Visible = bShowGroupMode;
                liveDataToolStripMenuItem.Visible = !bShowGroupMode;*/
                operationToolStripMenuItem.Enabled = true;
                managementToolStripMenuItem.Enabled = true;
                                

                if (_bViewerMode)               
                {
                    _ldvf = new LiveDataViewerForm();
                    _ldvf.MdiParent = this;
                    _ldvf.WindowState = FormWindowState.Maximized;
                    _ldvf.Show();
                }
              
                if (_bShowServer)
                {             


                    if (_serverMngtForm != null)
                        _serverMngtForm.Dispose();

                    _serverMngtForm = new ServerMngtForm(this);
                    _serverMngtForm.MdiParent = this;
                    _serverMngtForm.WindowState = FormWindowState.Maximized;
                    _serverMngtForm.Show();
                    _serverMngtForm.Focus();
                    LayoutMdi(MdiLayout.TileHorizontal);

                    if (bUseLightLiveData)
                    {
                        _liveDataForDeviceForm = new LiveDataForDeviceForm(TcpServeur);
                        _liveDataForDeviceForm.MdiParent = this;
                        _liveDataForDeviceForm.WindowState = FormWindowState.Maximized;
                        _liveDataForDeviceForm.Show();
                    }
                    else
                    {
                        _liveDataForm = new LiveDataForm(TcpServeur);
                        _liveDataForm.MdiParent = this;
                        _liveDataForm.WindowState = FormWindowState.Maximized;
                        _liveDataForm.Show();
                    }

                    this.WindowState = FormWindowState.Minimized;
                    

                  
                    //try to get world clock
                    DateTime dtworld;

                    if (tcpUtils.GetNetworkTime(out dtworld))
                    {
                        tcpUtils.setSystemTime(dtworld);

                    }
                }
            }
            catch (Exception exp)
            {
                ExceptionMessageBox.Show(exp);
            }          

        }

        private void userManagementToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
             
            if (Application.OpenForms.OfType<LiveDataForDeviceForm>().Any()) // if livedata form is opened : close it before opening local device
            {
                _liveDataForDeviceForm.Close();
                _liveDataForDeviceForm.Dispose();
                _liveDataForDeviceForm = null;
            }
            if (Application.OpenForms.OfType<LiveDataForm>().Any()) // if livedata form is opened : close it before opening local device
            {
                _liveDataForm.Close();
                _liveDataForm.Dispose();
                _liveDataForm = null;
            }

            if (Application.OpenForms.OfType<UserMngtForm>().Any()) // if usermngt form is already opened : put it back to foreground
            {
                _userMngtForm.MdiParent = this;
                _userMngtForm.WindowState = FormWindowState.Maximized;
                _userMngtForm.Show();
                return;
            }   

            _userMngtForm = new UserMngtForm();
            _userMngtForm.MdiParent = this;
            _userMngtForm.WindowState = FormWindowState.Maximized;
            _userMngtForm.Show();            
           
        }

        private void localReaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<LiveDataForDeviceForm>().Any()) // if livedata form is opened : close it before opening local device
            {
                _liveDataForDeviceForm.Close();
                _liveDataForDeviceForm.Dispose();
                _liveDataForDeviceForm = null;
            }
            if (Application.OpenForms.OfType<LiveDataForm>().Any()) // if livedata form is opened : close it before opening local device
            {
                _liveDataForm.Close();
                _liveDataForm.Dispose();
                _liveDataForm = null;
            }

            if (Application.OpenForms.OfType<LocalDeviceForm>().Any()) // if localDevice form is already opened : put it back to foreground
            {
                _localDeviceForm.MdiParent = this;
                _localDeviceForm.WindowState = FormWindowState.Maximized;
                _localDeviceForm.Show();
                return;
            }   

            _localDeviceForm = new LocalDeviceForm();
            _localDeviceForm.MdiParent = this;
            _localDeviceForm.WindowState = FormWindowState.Maximized;
            _localDeviceForm.Show();
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAll();
        }

        private void cascasdeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void tileHorizontalyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void tileVerticalyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void liveDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                if (Application.OpenForms.OfType<LiveDataForm>().Any()) // if livedata form is already opened : put it back to foreground
                {
                    _liveDataForm.MdiParent = this;
                    _liveDataForm.WindowState = FormWindowState.Maximized;
                    _liveDataForm.Show();
                    return;
                }

                _liveDataForm = new LiveDataForm(TcpServeur);
                _liveDataForm.MdiParent = this;
                _liveDataForm.WindowState = FormWindowState.Maximized;
                _liveDataForm.Show();
            }
            else if (_bShowServer)
            {
                if (bUseLightLiveData)
                {
                    if (Application.OpenForms.OfType<LiveDataForDeviceForm>().Any()) // if livedata form is already opened : put it back to foreground
                    {
                        _liveDataForDeviceForm.MdiParent = this;
                        _liveDataForDeviceForm.WindowState = FormWindowState.Maximized;
                        _liveDataForDeviceForm.Show();
                        return;
                    }

                    _liveDataForDeviceForm = new LiveDataForDeviceForm(TcpServeur);
                    _liveDataForDeviceForm.MdiParent = this;
                    _liveDataForDeviceForm.WindowState = FormWindowState.Maximized;
                    _liveDataForDeviceForm.Show();
                 
                }
                else
                {
                    if (Application.OpenForms.OfType<LiveDataForm>().Any()) // if livedata form is already opened : put it back to foreground
                    {
                        _liveDataForm.MdiParent = this;
                        _liveDataForm.WindowState = FormWindowState.Maximized;
                        _liveDataForm.Show();
                        return;
                    }

                    _liveDataForm = new LiveDataForm(TcpServeur);
                    _liveDataForm.MdiParent = this;
                    _liveDataForm.WindowState = FormWindowState.Maximized;
                    _liveDataForm.Show();
                }
               
            }
            else
            {
                if (Application.OpenForms.OfType<LiveDataForm>().Any()) // if livedata form is already opened : put it back to foreground
                {
                    _liveDataForm.MdiParent = this;
                    _liveDataForm.WindowState = FormWindowState.Maximized;
                    _liveDataForm.Show();
                    return;
                }

                _liveDataForm = new LiveDataForm(TcpServeur);
                _liveDataForm.MdiParent = this;
                _liveDataForm.WindowState = FormWindowState.Maximized;
                _liveDataForm.Show();
            }
         
        }

        private void itemListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_itemListForm != null)
            {
                _itemListForm.Close();
                _itemListForm.Dispose();
                _itemListForm = null;
            }

            _itemListForm = new ItemListForm(_liveDataForm,null);
            _itemListForm.MdiParent = this;
            _itemListForm.WindowState = FormWindowState.Maximized;
            _itemListForm.Show();
        }      
       

        public void CreateItemHistoryForm(string tagUid, string lotId)
        {
            if (_itemHistoryForm != null)
            {
                _itemHistoryForm.Close();
                _itemHistoryForm.Dispose();
                _itemHistoryForm = null;
            }

            _itemHistoryForm = new ItemHistoryForm(tagUid,lotId);
            _itemHistoryForm.MdiParent = this;
            _itemHistoryForm.WindowState = FormWindowState.Maximized;
            _itemHistoryForm.Show();
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAll();
            _autoUpdater = new AutoUpdater();


            // Keep Only X86 version  now compatible
            /*if (DataClass.ExcelInfo.isExcel2010_64bits())
                this.autoUpdater.ConfigURL = "http://www.spacecode.com/smartTracker_2013/x64/UpdateVersion.xml";
            else
                this.autoUpdater.ConfigURL = "http://www.spacecode.com/smartTracker_2013/x86/UpdateVersion.xml";*/


            if (_bShowServer) // For device update 32bits on purpose
                _autoUpdater.ConfigURL = "http://www.spacecode.com/smartTracker_2013/x86_Server/UpdateVersion.xml";
            else
                _autoUpdater.ConfigURL = "http://www.spacecode.com/smartTracker_2013/x86/UpdateVersion.xml";

            _autoUpdater.DownloadForm = new Confirm(_autoUpdater);
            _autoUpdater.ProgressForm = new UpdateDownload(_autoUpdater);
            _autoUpdater.AutoRestart = true;
            _autoUpdater.TryUpdate();
        }

        private void CloseAll()
        {
    
            if (_liveDataForm != null)
            {
                _liveDataForm.Close();               
                _liveDataForm = null;
            }
           

            if (_localDeviceForm != null)
            {
                _localDeviceForm.Close();                
                _localDeviceForm = null;
            }
           
            if (_userMngtForm != null)
            {
                _userMngtForm.Close();              
                _userMngtForm = null;
            }
          
            if (_wsf != null)
            {
                _wsf.Close();
                _wsf = null;
            }
           
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutUs aboutUs = new AboutUs();
            aboutUs.TopLevel = true;
            aboutUs.WindowState = FormWindowState.Normal;
            aboutUs.Show();
        }
       

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            CloseAll();
            if (TcpServeur != null)
                TcpServeur.StopSocket();
            TcpServeur = null;
           if (_bRestart)
            {
                Environment.ExitCode = 2; //the surrounding AppStarter must look for this to restart the app.
                Application.Exit();
            }
          
           
        }

        private void remoteReaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_liveDataForm != null)
            {
                _liveDataForm.Close();
                _liveDataForm.Dispose();
                _liveDataForm = null;
            }

            if (_remoteReaderForm != null)
                _remoteReaderForm.Dispose();

            _remoteReaderForm = new RemoteReaderForm(TcpServeur);
            _remoteReaderForm.MdiParent = this;
            _remoteReaderForm.WindowState = FormWindowState.Maximized;
            _remoteReaderForm.Show();      
        }

        private void serverManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_serverMngtForm != null)
                _serverMngtForm.Dispose();

            _serverMngtForm = new ServerMngtForm(this);
            _serverMngtForm.MdiParent = this;
            _serverMngtForm.WindowState = FormWindowState.Maximized;
            _serverMngtForm.Show();          
        }

        public void InitServer()
        {
            if (TcpServeur != null)
                TcpServeur.StopSocket();


            int.TryParse(ConfigurationManager.AppSettings["serverPort"], out _tcpServerPort);
            TcpServeur = new TcpIpServer(_tcpServerPort);   
            TcpServeur.StartServer();

        }

        private void timerRestart_Tick(object sender, EventArgs e)
        {
            if (_liveDataForm != null)
            {
                if (_liveDataForm.BRestart)
                {
                    _bRestart = true;
                    Close();                   
                }
            }
        }


        private void CheckConfig()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (ConfigurationManager.AppSettings["Desc_serverPort"] == null)
                config.AppSettings.Settings.Add("Desc_serverPort", ResStrings.strAppTcpPort);
            if (ConfigurationManager.AppSettings["serverPort"] == null)
                config.AppSettings.Settings.Add("serverPort", "6901");

            if (ConfigurationManager.AppSettings["Desc_NbRecordToKeep"] == null)
                config.AppSettings.Settings.Add("Desc_NbRecordToKeep", ResStrings.strAppInv);
            if (ConfigurationManager.AppSettings["NbRecordToKeep"] == null)
                config.AppSettings.Settings.Add("NbRecordToKeep", "150");

            if (ConfigurationManager.AppSettings["Desc_bStoreEventTag"] == null)
                config.AppSettings.Settings.Add("Desc_bStoreEventTag", ResStrings.strAppHistory);
            if (ConfigurationManager.AppSettings["bStoreEventTag"] == null)
                config.AppSettings.Settings.Add("bStoreEventTag", "True");

            if (ConfigurationManager.AppSettings["Desc_TimeZoneOffset"] == null)
                config.AppSettings.Settings.Add("Desc_TimeZoneOffset", ResStrings.strAppGmt);
            if (ConfigurationManager.AppSettings["TimeZoneOffset"] == null)
                config.AppSettings.Settings.Add("TimeZoneOffset", "1.0");

            if (ConfigurationManager.AppSettings["Desc_bShowServer"] == null)
                config.AppSettings.Settings.Add("Desc_bShowServer", ResStrings.tcpAppServer);
            if (ConfigurationManager.AppSettings["bShowServer"] == null)
                config.AppSettings.Settings.Add("bShowServer", "False");

            if (ConfigurationManager.AppSettings["Desc_bCompareOnLotID"] == null)
                config.AppSettings.Settings.Add("Desc_bCompareOnLotID", ResStrings.strAppLotID);
            if (ConfigurationManager.AppSettings["bCompareOnLotID"] == null)
                config.AppSettings.Settings.Add("bCompareOnLotID", "True");

            if (ConfigurationManager.AppSettings["Desc_bScreenSaver"] == null)
                config.AppSettings.Settings.Add("Desc_bScreenSaver", ResStrings.tcpAppScreenSaver);
            if (ConfigurationManager.AppSettings["bScreenSaver"] == null)
                config.AppSettings.Settings.Add("bScreenSaver", "True");

            if (ConfigurationManager.AppSettings["Desc_bShowImage"] == null)
                config.AppSettings.Settings.Add("Desc_bShowImage", ResStrings.strAppImg);
            if (ConfigurationManager.AppSettings["bShowImage"] == null)
                config.AppSettings.Settings.Add("bShowImage", "False");

            if (ConfigurationManager.AppSettings["Desc_bShowGroupMode"] == null)
                config.AppSettings.Settings.Add("Desc_bShowGroupMode", ResStrings.strAppGrp);
            if (ConfigurationManager.AppSettings["bShowGroupMode"] == null)
                config.AppSettings.Settings.Add("bShowGroupMode", "False");

            if (ConfigurationManager.AppSettings["Desc_bShowAlert"] == null)
                config.AppSettings.Settings.Add("Desc_bShowAlert", ResStrings.strAppAlert);
            if (ConfigurationManager.AppSettings["bShowAlert"] == null)
                config.AppSettings.Settings.Add("bShowAlert", "False");

            if (ConfigurationManager.AppSettings["Desc_bUseAlarm"] == null)
                config.AppSettings.Settings.Add("Desc_bUseAlarm", ResStrings.strAppAlarm);
            if (ConfigurationManager.AppSettings["bUseAlarm"] == null)
                config.AppSettings.Settings.Add("bUseAlarm", "False");

            if (ConfigurationManager.AppSettings["Desc_timeTcpToRefresh"] == null)
                config.AppSettings.Settings.Add("Desc_timeTcpToRefresh", ResStrings.strAppTcp);
            if (ConfigurationManager.AppSettings["timeTcpToRefresh"] == null)
                config.AppSettings.Settings.Add("timeTcpToRefresh", "3000");           

            if (ConfigurationManager.AppSettings["Desc_pathExcelSheet"] == null)
                config.AppSettings.Settings.Add("Desc_pathExcelSheet", ResStrings.strAppMacro);
            if (ConfigurationManager.AppSettings["pathExcelSheet"] == null)
                config.AppSettings.Settings.Add("pathExcelSheet", "");

            if (ConfigurationManager.AppSettings["Desc_MacroName"] == null)
                config.AppSettings.Settings.Add("Desc_MacroName", ResStrings.strAppMacroName);
            if (ConfigurationManager.AppSettings["MacroName"] == null)
                config.AppSettings.Settings.Add("MacroName", "");

            if (ConfigurationManager.AppSettings["Desc_bViewerMode"] == null)
                config.AppSettings.Settings.Add("Desc_bViewerMode", ResStrings.strAppViewer);
            if (ConfigurationManager.AppSettings["bViewerMode"] == null)
                config.AppSettings.Settings.Add("bViewerMode", "False");


            if (ConfigurationManager.AppSettings["Desc_bAutoUpdate"] == null)
                config.AppSettings.Settings.Add("Desc_ bAutoUpdate", ResStrings.strAppUpdate);
            if (ConfigurationManager.AppSettings["bAutoUpdate"] == null)
                config.AppSettings.Settings.Add("bAutoUpdate", "False");

            if (ConfigurationManager.AppSettings["Desc_bInterruptScanWithFP"] == null)
                config.AppSettings.Settings.Add("Desc_bInterruptScanWithFP", ResStrings.strAppbInterupt);
            if (ConfigurationManager.AppSettings["bInterruptScanWithFP"] == null)
                config.AppSettings.Settings.Add("bInterruptScanWithFP", "False");

             if (ConfigurationManager.AppSettings["Desc_bUseCsvExchange"] == null)
                config.AppSettings.Settings.Add("Desc_bUseCsvExchange", "If true use CSV file exchange data");
            if (ConfigurationManager.AppSettings["bUseCsvExchange"] == null)
                config.AppSettings.Settings.Add("bUseCsvExchange", "False");

             if (ConfigurationManager.AppSettings["Desc_pathCsvExchange"] == null)
                config.AppSettings.Settings.Add("Desc_pathCsvExchange", "Path of shared folder to CSV  exchange");
            if (ConfigurationManager.AppSettings["pathCsvExchange"] == null)
                config.AppSettings.Settings.Add("pathCsvExchange", "");

            if (ConfigurationManager.AppSettings["Desc_CsvMachineId"] == null)
                config.AppSettings.Settings.Add("Desc_CsvMachineId", "Machine ID (4 digits)");
            if (ConfigurationManager.AppSettings["CsvMachineId"] == null)
                config.AppSettings.Settings.Add("CsvMachineId", "0000");

            if (ConfigurationManager.AppSettings["Desc_TcpUnlockingAuthorization"] == null)
                config.AppSettings.Settings.Add("Desc_TcpUnlockingAuthorization", "Put Authorizing string to unlock door with TCP command");
            if (ConfigurationManager.AppSettings["TcpUnlockingAuthorization"] == null)
                config.AppSettings.Settings.Add("TcpUnlockingAuthorization", "Put Crypted String Here");


            if (ConfigurationManager.AppSettings["Desc_Rfid_Printer_Serial"] == null)
                config.AppSettings.Settings.Add("Desc_Rfid_Printer_Serial", "Serial number of RFID printer board");
            if (ConfigurationManager.AppSettings["Rfid_Printer_Serial"] == null)
                config.AppSettings.Settings.Add("Rfid_Printer_Serial", "Not configured");

            if (ConfigurationManager.AppSettings["Desc_PrinterIP"] == null)
                config.AppSettings.Settings.Add("Desc_PrinterIP", "IP of the printer");
            if (ConfigurationManager.AppSettings["PrinterIP"] == null)
                config.AppSettings.Settings.Add("PrinterIP", "xxx.xxx.xxx.xxx");

            if (ConfigurationManager.AppSettings["Desc_Template"] == null)
                config.AppSettings.Settings.Add("Desc_Template", "Name of the template for Blood");
            if (ConfigurationManager.AppSettings["Template"] == null)
                config.AppSettings.Settings.Add("Template", "Not configured");
        
            config.Save(ConfigurationSaveMode.Modified, true);
            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");
              
                    
            
        }

        private void updateDB()
        {
            MainDBClass db = new MainDBClass();

            db.OpenDB();

            if (!db.isTableVersionExist())
                db.setUpdate(1);

            //Get Version
            int version = db.getVersion();

            if (version < db.SqlUpdate.Count)
            {
                for (int i = (version + 1); i <= db.SqlUpdate.Count; i++)
                    db.setUpdate(i);
            }
            db.CloseDB();
        }

        /*private void updateSqliteDB()
        {
            MainDBClass db = new MainDBClass();
            db.OpenDB();

            if (!db.isTableExist("tb_Version"))
                db.setUpdate(1);
            
            //Get Version
            int version = db.getVersion();

            if (version < db.sqlUpdate.Count)
            {
                for (int i = (version + 1) ; i <= db.sqlUpdate.Count ; i++)
                    db.setUpdate(i);
            }  
            db.CloseDB();
            
        }

        private void updateSqlServer()
        {
            string conString = string.Empty;
            int bUseSQlServer = 0;
            MainDBClass db1 = new MainDBClass();
            db1.OpenDB();
            db1.getSqlServerInfo(out conString, out bUseSQlServer);
            db1.CloseDB();

            if ((bUseSQlServer == 1) && (!string.IsNullOrEmpty(conString)))
            {
                DBClassSqlServer.DBSqlServer db = new DBClassSqlServer.DBSqlServer(DBClass_SQLServer.UtilSqlServer.ConvertConnectionString(conString));
                db.OpenDB();
                if (db.conState == ConnectionState.Open)
                {
                    if (!db.isTableExist("tbo_Db_Info"))
                        db.setUpdate(1);

                    //Get Version
                    int version = db.getVersion();

                    if (version < db.sqlUpdate.Count)
                    {
                        for (int i = (version + 1); i <= db.sqlUpdate.Count; i++)
                            db.setUpdate(i);
                    }

                    db.CloseDB();
                }
            }
        }*/
      

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (BackgroundImage != null)
            {
                pevent.Graphics.InterpolationMode =
                InterpolationMode.Low;
                //pevent.Graphics.Clear(this.BackColor);
                pevent.Graphics.DrawImage(BackgroundImage, 0, 0,
                ClientRectangle.Width, ClientRectangle.Height);
            }
            else
            {
                base.OnPaintBackground(pevent);
            }
        }
       

        private void testRFIDToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (_liveDataForm != null)
            {
                _liveDataForm.Close();
                _liveDataForm.Dispose();
                _liveDataForm = null;
            }

            PwdForm pwd = new PwdForm();
            pwd.Show();   
        }

        private void applicationSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAll();
            /*string appConfigPath = System.Reflection.Assembly.GetExecutingAssembly().Location + ".config";

            DynamicConfigDialog.AppConfigDialog dialog = new DynamicConfigDialog.AppConfigDialog(appConfigPath);
            dialog.ShowDialog();*/

            ConfigEditor c = new ConfigEditor();
            c.MdiParent = this;
            c.WindowState = FormWindowState.Maximized;
            c.Show();         
          
           
        }

        private void databaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void spacecodeSiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.spacecode.com");
        }        

        private void activityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_userStatForm != null)
                _userStatForm.Dispose();

            _userStatForm = new UserStatForm();
            _userStatForm.MdiParent = this;
            _userStatForm.WindowState = FormWindowState.Maximized;
            _userStatForm.Show();
        }

        private void compareInventoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_compInvForm != null)
            {
                _compInvForm.Close();
                _compInvForm.Dispose();
                _compInvForm = null;
            }

            _compInvForm = new CompareInventoryReportForm();
            _compInvForm.MdiParent = this;
            _compInvForm.WindowState = FormWindowState.Maximized;
            _compInvForm.Show();
        }

        private void rEADERToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_readerHistoryForm != null)
            {
                _readerHistoryForm.Close();
                _readerHistoryForm.Dispose();
                _readerHistoryForm = null;
            }

            _readerHistoryForm = new ReaderHistoryForm();
            _readerHistoryForm.MdiParent = this;
            _readerHistoryForm.WindowState = FormWindowState.Maximized;
            _readerHistoryForm.Show();
        }

        private void productToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateItemHistoryForm(null, null);
        }

        private void liveDataGroupModeToolStripMenuItem_Click(object sender, EventArgs e)
        {            

            /*if (liveDataGroupForm != null)
            {
                liveDataGroupForm.Close();
                liveDataGroupForm.Dispose();
                liveDataGroupForm = null;
            }

            liveDataGroupForm = new LiveDataGroupForm(tcpServeur);
            liveDataGroupForm.MdiParent = this;
            liveDataGroupForm.WindowState = FormWindowState.Maximized;
            liveDataGroupForm.Show();*/
            if (_bmc != null)
            {
                _bmc.Close();
                _bmc.Dispose();
                _bmc = null;
            }

            _bmc = new BoxModeConfig(_liveDataForm);
            _bmc.MdiParent = this;
            _bmc.WindowState = FormWindowState.Maximized;
            _bmc.Show();

        }

        private void alertsAndMailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_alf != null)
            {
                _alf.Close();
                _alf.Dispose();
                _alf = null;
            }

            _alf = new AlertMngmtForm();
            _alf.MdiParent = this;
            _alf.WindowState = FormWindowState.Maximized;
            _alf.Show();
        }

        private void licenseInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAll();
            operationToolStripMenuItem.Enabled = false;
            managementToolStripMenuItem.Enabled = false;
            MngLicense mngLic = new MngLicense(_license, ValidationKey);
            mngLic.ShowDialog();

            LicenceStatus licStatus = TestLicence();
            if (licStatus != LicenceStatus.LsExpired)
            {
                //liveDataGroupModeToolStripMenuItem.Visible = bShowGroupMode;
                //liveDataToolStripMenuItem.Visible = !bShowGroupMode;
                operationToolStripMenuItem.Enabled = true;
                managementToolStripMenuItem.Enabled = true;
                
            }
        }

        private void formulaConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_fv != null)
            {
                _fv.Close();
                _fv.Dispose();
                _fv = null;
            }
            _fv = new Formule(_liveDataForm);
            _fv.MdiParent = this;
            _fv.WindowState = FormWindowState.Normal;
            _fv.Show();

        }

        private void configureColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_liveDataForm != null)
            {
                _liveDataForm.Close();
                _liveDataForm.Dispose();
                _liveDataForm = null;
            }

            if (_dbColumnForm != null)
                _dbColumnForm.Dispose();

            _dbColumnForm = new DbColumnForm();
            _dbColumnForm.MdiParent = this;
            _dbColumnForm.WindowState = FormWindowState.Maximized;
            _dbColumnForm.Show();
        }

        private void configurationSQLDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_liveDataForm != null)
            {
                _liveDataForm.Close();
                _liveDataForm.Dispose();
                _liveDataForm = null;
            }

            if (_dbSqlForm != null)
                _dbSqlForm.Dispose();

            _dbSqlForm = new DbSqlForm();
            _dbSqlForm.MdiParent = this;
            _dbSqlForm.WindowState = FormWindowState.Maximized;
            _dbSqlForm.Show();
        }

        private void viewerModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_ldvf != null)
            {
                _ldvf.Close();
                _ldvf.Dispose();
                _ldvf = null;
            }
            _ldvf = new LiveDataViewerForm();
            _ldvf.MdiParent = this;
            _ldvf.WindowState = FormWindowState.Maximized;
            _ldvf.Show();
        }
        private void tEMPERATUREHISTORYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<TempHistory>().Any()) // if usermngt form is already opened : put it back to foreground
            {
                _thw.MdiParent = this;
                _thw.WindowState = FormWindowState.Maximized;
                _thw.Show();
                return;
            }

            _thw = new TempHistory();
            _thw.MdiParent = this;
            _thw.WindowState = FormWindowState.Maximized;
            _thw.Show();   
          
        }

        private void weightScalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_wsf != null)
            {
                _wsf.Close();
                _wsf.Dispose();
                _wsf = null;
            }
            _wsf = new WeightScalefrm();
            _wsf.MdiParent = this;
            _wsf.WindowState = FormWindowState.Maximized;
            _wsf.Show();
        }

        private void exportScanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_expf != null)
            {
                _expf.Close();
                _expf.Dispose();
                _expf = null;
            }
            _expf = new ExportFrm();
            _expf.MdiParent = this;
            _expf.WindowState = FormWindowState.Maximized;
            _expf.Show();
        } 

        // Detect Clef USB for check if ResetDeviceNetwork.bat file exist on launch it

        private const int WmDevicechange = 0x0219;
        private const int DbtDevicearrival = 0x8000;
/*
        private const int DbtDeviceremovecomplete = 0x8004;
*/
        private const int DbtDevtypVolume = 0x2;

            protected override void WndProc(ref Message m)
		    {
			    // le message est de type DEVICECHANGE, ce qui nous interesse
			    if (m.Msg == WmDevicechange)
			    {
				    // le "sous-message" dit que le device vient d'etre pluggé
				    if (m.WParam.ToInt32() == DbtDevicearrival)
				    {
					    // device plugged

					    // on créé une structure depuis un pointeur a l'aide du Marshalling
					    // cette structure est generique mais on peut "l'interroger" comme une structure DEV_BROADCAST_HDR
					    DevBroadcastHdr hdr = (DevBroadcastHdr)Marshal.PtrToStructure(m.LParam, typeof(DevBroadcastHdr));

					    // ok, le device pluggé est un volume (aussi appelé 'périphérique de stockage de masse')...
					    if (hdr.DbchDevicetype == DbtDevtypVolume)
					    {
						    // ... et donc on recréé une structure, a partir du même pointeur de structure "générique",
						    // une structure un poil plus spécifique
						    DevBroadcastVolume vol = (DevBroadcastVolume)Marshal.PtrToStructure(m.LParam, typeof(DevBroadcastVolume));
						    // le champs dbcv_unitmask contient la ou les lettres de lecteur du ou des devices qui viennent d'etre pluggé
						    // MSDN nous dit que si le bit 0 est à 1 alors le lecteur est a:, si le bit 1 est à 1 alors le lecteur est b:
						    // et ainsi de suite
						    uint mask = vol.DbcvUnitmask;
						    // recupèration des lettres de lecteurs
						    char[] letters = MaskDepioteur(mask);

						    // mise à jour de l'IHM pour notifier nos petits yeux tout content :)
						   string filePath = letters[0] + ":\\ResetDeviceNetwork.bat";
                            if (File.Exists(filePath))
                            {
                                ProcessStartInfo psi = new ProcessStartInfo();
                                psi.FileName = filePath;
                                Process p = new Process();
                                p.StartInfo = psi;
                                p.Start();
                            }
					    }
				    }				    
			    }

			    // laissons notre fenêtre faire tout de même son boulot
			    base.WndProc(ref m);
		    }

		    // fonction d'extraction des lettres de lecteur
		    public static char[] MaskDepioteur(uint mask)
		    {
			    int cnt = 0;
			    uint tempMask = mask;

			    // on compte le nombre de bits à 1
			    for (int i = 0; i < 32; i++)
			    {
				    if ((tempMask & 1) == 1)
					    cnt++;
				    tempMask >>= 1;
				    if (tempMask == 0)
					    break;
			    }

			    // on instancie le bon nombre d'elements
			    char[] result = new char[cnt];
			    cnt = 0;
			    // on refait mais ce coup ci on attribut
			    for (int i = 0; i < 32; i++)
			    {
				    if ((mask & 1) == 1)
					    result[cnt++] = (char)('a' + i);
				    mask >>= 1;
				    if (mask == 0)
					    break;
			    }

			    return (result);
		    }
	    

	    // structure générique
	    public struct DevBroadcastHdr
	    {
		    public uint DbchSize;
		    public uint DbchDevicetype;
		    public uint DbchReserved;
	    }

	    // structure spécifique
	    // notez qu'elle a strictement le même tronche que la générique mais
	    // avec des trucs en plus
	    public struct DevBroadcastVolume
	    {
		    public uint DbcvSize;
		    public uint DbcvDevicetype;
		    public uint DbcvReserved;
		    public uint DbcvUnitmask;
		    public ushort DbcvFlags;
	    }

      

       

       

         
    }
}
