using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Threading;
using smartTracker.Properties;


namespace smartTracker
{
    static class Program
    {

        private const string RegKey = @"HKEY_CURRENT_USER\SOFTWARE\SmartTracker_V2";
        private const string RegValue = "Driver_Install";
        private const string RegValueInstall = "Executable_Path";
        private const string RegValueAccess = "Driver_Access";
        private const string RegValueYapi = "Driver_Yapi";
      

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
          
           

            bool owned = true;                         
            //Mutex mutex = new Mutex(true, "smartTracker_mutex", out owned);


            if (owned)
            {

                Registry.SetValue(RegKey, RegValueInstall, Application.StartupPath);
                string tmpStr = Convert.ToString(Registry.GetValue(RegKey, RegValue, null));


                if (string.IsNullOrEmpty(tmpStr))
                {
                    InstallDriver();
                    Registry.SetValue(RegKey, RegValue, "Done");
                    Registry.SetValue(RegKey, RegValueAccess, "Done");
                }
                tmpStr = Convert.ToString(Registry.GetValue(RegKey, RegValueAccess, null));
                if (string.IsNullOrEmpty(tmpStr))
                {
                    // InstallAccess();
                    Registry.SetValue(RegKey, RegValueAccess, "Done");

                }
                tmpStr = Convert.ToString(Registry.GetValue(RegKey, RegValueYapi, null));
                if (string.IsNullOrEmpty(tmpStr))
                {
                    InstallYapiDll();
                    Registry.SetValue(RegKey, RegValueYapi, "Done");
                }


                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);       
                    Application.Run(new MainForm());
                    //mutex.ReleaseMutex();
               
            }
            else
            {
                MessageBox.Show(ResStrings.str_Another_instance_of_smartTracker_is_running, ResStrings.strInfo,MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
           
        }
        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            DialogResult result = DialogResult.Abort;
            try
            {
                string info = "\r\nWhoops! Please contact the developers with the following information:\n\nApplication Error\r\n" + e.Exception.Message + e.Exception.StackTrace;

                File.AppendAllText(@"C:\temp\LogProgram.txt", info);
               /* result = MessageBox.Show(info,
                  "Application Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);*/

        
            }
            finally
            {
              /*  if (result == DialogResult.Abort)
                {
                    Application.Exit();
                }*/
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                string info = "\r\nWhoops! Please contact the developers with the following information:\n\nFatal Error\r\n" + ex.Message + ex.StackTrace;
                File.AppendAllText(@"C:\temp\LogProgram.txt", info);
               // MessageBox.Show(info,"Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            finally
            {
               // Application.Exit();
            }
        }

        private static void InstallYapiDll()
        {
            string applicationExePath = Application.StartupPath;
            string yapi32 = applicationExePath + Path.DirectorySeparatorChar + "lib" + Path.DirectorySeparatorChar + "x86" + Path.DirectorySeparatorChar + "yapi.dll";
            string yapi64 = applicationExePath + Path.DirectorySeparatorChar + "lib" + Path.DirectorySeparatorChar + "x64" + Path.DirectorySeparatorChar + "yapi.dll";
            string pathYapi = applicationExePath + Path.DirectorySeparatorChar + "yapi.dll";

            if (File.Exists(pathYapi))
            {
                File.Delete(pathYapi);
            }
            if (Directory.Exists(@"C:\Program Files (x86)"))
            {
                File.Copy(yapi64, pathYapi);
            }
            else
            {
                File.Copy(yapi32, pathYapi);
            }

        }
/*
        private static void InstallAccess()
        {
           
            string ApplicationExePath = Application.StartupPath;
            Process installXlsProcess = null;

            string XlsDriver32 = ApplicationExePath + Path.DirectorySeparatorChar + "driver" + Path.DirectorySeparatorChar + "Import Excel" + Path.DirectorySeparatorChar + "AccessDatabaseEngine_x86.exe";
            string XlsDriver64 = ApplicationExePath + Path.DirectorySeparatorChar + "driver" + Path.DirectorySeparatorChar + "Import Excel" + Path.DirectorySeparatorChar + "AccessDatabaseEngine_x64.exe";
            try
            {                
                if (File.Exists(XlsDriver32))
                {
                    ProcessStartInfo pXls = new ProcessStartInfo(XlsDriver32);
                    pXls.Arguments = "/passive";                    
                    installXlsProcess = Process.Start(pXls);
                }
            }
            catch
            {

            }

            if (installXlsProcess != null)
            {
                try
                {
                    installXlsProcess.WaitForExit();
                }
                catch
                {

                }
            }

            try
            {
                if (File.Exists(XlsDriver64))
                {
                    ProcessStartInfo pXls = new ProcessStartInfo(XlsDriver64);
                    pXls.Arguments = "/passive";                   
                    installXlsProcess = Process.Start(pXls);
                }
            }
            catch
            {

            }

            if (installXlsProcess != null)
            {
                try
                {
                    installXlsProcess.WaitForExit();
                }
                catch
                {

                }
            }            
        }
*/

        private static void InstallDriver()
        {

            //USB
            Process installUsbProcess = null;
            string applicationExePath = Application.StartupPath;
            string usbDriverexe;
            if (Directory.Exists(@"C:\Program Files (x86)"))
                usbDriverexe = applicationExePath + Path.DirectorySeparatorChar + "driver" + Path.DirectorySeparatorChar + "USB" + Path.DirectorySeparatorChar + "DPInst_x64.exe";
            else
                usbDriverexe = applicationExePath + Path.DirectorySeparatorChar + "driver" + Path.DirectorySeparatorChar + "USB" + Path.DirectorySeparatorChar + "DPInst_x86.exe";
            try
            {
                if (File.Exists(usbDriverexe))
                {
                    ProcessStartInfo pUsb = new ProcessStartInfo(usbDriverexe);
                    pUsb.Verb = "runas";
                    installUsbProcess = Process.Start(pUsb);
                }
            }
            catch
            {

            }

            if (installUsbProcess != null)
            {
                try
                {
                    installUsbProcess.WaitForExit();
                }
                catch
                {

                }
            }

            //XLS
            /*Process installXlsProcess = null;

            string XlsDriver32 = ApplicationExePath + Path.DirectorySeparatorChar + "driver" + Path.DirectorySeparatorChar + "Import Excel" + Path.DirectorySeparatorChar + "AccessDatabaseEngine_x86.exe \\passive";
            string XlsDriver64 = ApplicationExePath + Path.DirectorySeparatorChar + "driver" + Path.DirectorySeparatorChar + "Import Excel" + Path.DirectorySeparatorChar + "AccessDatabaseEngine_x64.exe \\passive";
            try
            {
               
                if (File.Exists(XlsDriver32))
                {
                    ProcessStartInfo pXls = new ProcessStartInfo(XlsDriver32);
                    pXls.Verb = "runas";
                    installXlsProcess = Process.Start(pXls);
                }
            }
            catch
            {

            }

            if (installXlsProcess != null)
            {
                try
                {
                    installXlsProcess.WaitForExit();
                }
                catch
                {

                }
            }

            try
            {
                if (File.Exists(XlsDriver64))
                {
                    ProcessStartInfo pXls = new ProcessStartInfo(XlsDriver64);
                    pXls.Verb = "runas";
                    installXlsProcess = Process.Start(pXls);
                }
            }
            catch
            {

            }

            if (installXlsProcess != null)
            {
                try
                {
                    installXlsProcess.WaitForExit();
                }
                catch
                {

                }
            }

            */

            // FP
            
            Process installFpProcess = null;           
            string fpDriverexe = applicationExePath + Path.DirectorySeparatorChar + "driver" + Path.DirectorySeparatorChar + "FingerPrint" + Path.DirectorySeparatorChar + "Setup.exe";

            try
            {
                if (File.Exists(fpDriverexe))
                {
                    ProcessStartInfo pFp = new ProcessStartInfo(fpDriverexe);
                    pFp.Verb = "runas";
                    installFpProcess = Process.Start(pFp);
                }
            }
            catch
            {

            }

            if (installFpProcess != null)
            {
                try
                {
                    installFpProcess.WaitForExit();
                }
                catch
                {

                }
            }

            // Chart for stat

            Process installChartProcess = null;
            string chartDriverexe = applicationExePath + Path.DirectorySeparatorChar + "driver" + Path.DirectorySeparatorChar + "Chart" + Path.DirectorySeparatorChar + "MSChart.exe";

            try
            {
                if (File.Exists(chartDriverexe))
                {
                    ProcessStartInfo pChart = new ProcessStartInfo(chartDriverexe);
                    pChart.Verb = "runas";
                    installChartProcess = Process.Start(pChart);
                }
            }
            catch
            {

            }

            if (installChartProcess != null)
            {
                try
                {
                    installChartProcess.WaitForExit();
                }
                catch
                {

                }
            }
        }
    }
}
