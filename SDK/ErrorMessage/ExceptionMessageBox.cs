using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Mail;
using System.IO;

using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace ErrorMessage
{
    public partial class ExceptionMessageBox : Form
    {
        // le texte qui sera affiché
		private string _Text;

		// les détails qui seront affichés
		private string _Details;

        // le titre
        private string _Title;

        // taille originale de la form
        private Size ClosedSize = new Size(428,143);

        // taille de la form ouverte
        private Size OpenSize = new Size(428, 323);

       

        int cpt = 10;

        private ExceptionMessageBox(string text, string details, string title)
		{
			InitializeComponent();
			_Text = text;
			_Details = details;
            _Title = title;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
            cpt = 30;
			// on met la taille de la Form à celle de sa version "fermée"
            this.Size = ClosedSize;

			// on remplit les labels et textbox
			labelText.Text = _Text;
			textBoxDetails.Text = _Details;			

            // on affiche le titre
            this.Text = _Title;

        }

        /// <summary>
        /// Affiche une ErrorMessageBox avec le contenu du champ "details" dans une fenêtre pouvant être ouverte/fermée.
        /// </summary>
        /// <param name="text">Texte principal du message d'erreur.</param>
        /// <param name="details">Contenu du champ "détails" du message d'erreur.</param>
        /// <param name="title">Titre de la fenêtre du message d'erreur.</param>
		public static void Show(string text,string details,string title)
		{
           
            // on vérifie les paramètres
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }
            if (string.IsNullOrEmpty(details))
            {
                throw new ArgumentNullException("details");
            }
            if (string.IsNullOrEmpty(title))
            {
                title = "Erreur !";
            }

			// on crée une ErrorMessageBox et on l'affiche
            ExceptionMessageBox f = new ExceptionMessageBox(text, details, title);
			//f.ShowDialog();
            f.Show();
            
		}

        /// <summary>
        /// Affiche une ErrorMessageBox avec le contenu du champ "details" dans une fenêtre pouvant être ouverte/fermée.
        /// La valeur par défaut du titre de la fenêtre est "Erreur !".
        /// </summary>
        /// <param name="text">Texte principal du message d'erreur.</param>
        /// <param name="details">Contenu du champ "détails" du message d'erreur.</param>
        public static void Show(string text, string details)
        {
            Show(text, details, null);
        }

       
        /// <summary>
        /// Affiche une ErrorMessageBox avec le contenu d'une erreur : le message d'erreur principal directement affiché, et les détails de l'Exception et de InnerException dans le champ "détails".
        /// </summary>
        /// <param name="ex">L'erreur à afficher en détails.</param>
		public static void Show(Exception ex,string spareInfo = null)
		{
			// on crée le contenu des détails à partie de l'Exception
            Exception bckexp = ex;
            try
            {
                string error = getError(ex);

                StringBuilder details = new StringBuilder();
                details.AppendLine(ex.Message);
                details.Append("(in " + ex.Source);
                details.AppendLine(" at " + ex.TargetSite + ")");
                details.AppendLine(ex.StackTrace);

                // on ajoute les détails de InnerException si elle existe
                while (ex.InnerException != null)
                {                    
                    details.AppendLine("InnerException :");
                    details.AppendLine(ex.InnerException.Message);
                    details.Append("(in " + ex.InnerException.Source);
                    details.AppendLine(" at " + ex.InnerException.TargetSite + ")");
                    details.AppendLine(ex.InnerException.StackTrace);
                    details.AppendLine("\r\n");
                    ex = ex.InnerException;
                }

                error += "/r/n/r/n *************************/r/n" + details;
                writeLog(error);   
                if (spareInfo == null)
                    Show(bckexp.Message, error, "Exception");
                else
                    Show(bckexp.Message, error, "Exception in " + spareInfo);
            }
            catch
            {
               // Nothing to do unless pray to not come here :-)
            }
            
		}

        public static string getError(Exception exception)
        {
            StringBuilder error = new StringBuilder();           

            error.AppendLine("Application:       " + Application.ProductName);
            error.AppendLine("Version:           " + Application.ProductVersion);
            error.AppendLine("Date:              " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            error.AppendLine("Computer name:     " + SystemInformation.ComputerName);
            error.AppendLine("User name:         " + SystemInformation.UserName);
            error.AppendLine("OS:                " + Environment.OSVersion.ToString());
            error.AppendLine("Culture:           " + CultureInfo.CurrentCulture.Name);
            error.AppendLine("Resolution:        " + SystemInformation.PrimaryMonitorSize.ToString());
            error.AppendLine("System up time:    " + GetSystemUpTime());
            error.AppendLine("App up time:       " +
              (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString());

            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(memStatus))
            {
                error.AppendLine("Total memory:      " + memStatus.ullTotalPhys / (1024 * 1024) + "Mb");
                error.AppendLine("Available memory:  " + memStatus.ullAvailPhys / (1024 * 1024) + "Mb");
            }

            error.AppendLine("");

            error.AppendLine("Exception classes:   ");
            error.Append(GetExceptionTypeStack(exception));
            error.AppendLine("");
            error.AppendLine("Exception messages: ");
            error.Append(GetExceptionMessageStack(exception));

            error.AppendLine("");
            error.AppendLine("Stack Traces:");
            error.Append(GetExceptionCallStack(exception));
            error.AppendLine("");
            /*error.AppendLine("Loaded Modules:");

            

                Process thisProcess = Process.GetCurrentProcess();
                foreach (ProcessModule module in thisProcess.Modules)
                {
                    try
                    {
                        error.AppendLine(module.FileName + " " + module.FileVersionInfo.FileVersion);
                    }
                    catch
                    {
                    }
                }
          
            error.AppendLine(" ");*/
            return error.ToString();
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        private static TimeSpan GetSystemUpTime()
        {
            PerformanceCounter upTime = new PerformanceCounter("System", "System Up Time");
            upTime.NextValue();
            return TimeSpan.FromSeconds(upTime.NextValue());
        }

        private static string GetExceptionTypeStack(Exception e)
        {
            if (e.InnerException != null)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine(GetExceptionTypeStack(e.InnerException));
                message.AppendLine("   " + e.GetType().ToString());
                return (message.ToString());
            }
            else
            {
                return "   " + e.GetType().ToString();
            }
        }
        private static string GetExceptionMessageStack(Exception e)
        {
            if (e.InnerException != null)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine(GetExceptionMessageStack(e.InnerException));
                message.AppendLine("   " + e.Message);
                return (message.ToString());
            }
            else
            {
                return "   " + e.Message;
            }
        }
        private static string GetExceptionCallStack(Exception e)
        {
            if (e.InnerException != null)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine(GetExceptionCallStack(e.InnerException));
                message.AppendLine("--- Next Call Stack:");
                message.AppendLine(e.StackTrace);
                return (message.ToString());
            }
            else
            {
                return e.StackTrace;
            }
        }

        public static void writeLog(string error)
        {
            try
            {
                string filename = Path.GetDirectoryName(Application.ExecutablePath);
                filename += "\\BugReport.log";

                object lockObj = new object();
                lock (lockObj)
                {
                    string spare = "\r\n_______________________________________________________________\r\n\r\n";
                    File.AppendAllText(filename,error);
                    File.AppendAllText(filename,spare);    
                }
            }
            catch
            {
            }
        
        }

        public void sendError()
        {
            string SenderAdress = "alert@spacecode-rfid.com";
            string LoginName = "alert@spacecode-rfid.com";
            string Password = "rfidnet";
            string SMTPServer = "mail.spacecode-rfid.com";
            int SMTPPort = 587;
         
            try
            {
                bool mailcreated = false;
                //create the mail message
                MailMessage mail = null;
                string mailbody = null;

              
                mailcreated = true;
                mail = new MailMessage();
                //set the addresses
                mail.From = new MailAddress(SenderAdress);

                mail.To.Add("christophe.raoult@spacecode-rfid.com");
                //mail.To.Add("eric.gout@spacecode-rfid.com");

                mail.Subject = "Exception smartTracker on " + SystemInformation.ComputerName;
                mailbody = string.Empty;

                mailbody += labelText.Text + "<br /><br />";

                mailbody += "Environnement <br />";
                mailbody += "Machine name :" + SystemInformation.ComputerName + "<br />";
                mailbody += "OS name :" + OSInfo.Name + "<br />";
                mailbody += "OS Edition :" + OSInfo.Edition + "<br />";
                mailbody += "OS Bits :" + OSInfo.Bits + "<br />";
                mailbody += "OS Service Pack :" + OSInfo.ServicePack + "<br />";

                mailbody += _Title + " : <br /><br /><br />";

                string str = textBoxDetails.Text.Replace("\n", "<br />");

                mailbody += str;   



                if (mailcreated)
                {

                    //first we create the Plain Text part
                    AlternateView plainView = AlternateView.CreateAlternateViewFromString(mailbody, null, "text/plain");

                    //then we create the Html part
                    //to embed images, we need to use the prefix 'cid' in the img src value
                    //the cid value will map to the Content-Id of a Linked resource.
                    //thus <img src='cid:companylogo'> will map to a LinkedResource with a ContentId of 'companylogo'
                    AlternateView htmlView;

                    htmlView = AlternateView.CreateAlternateViewFromString(mailbody + "<br />", null, "text/html");

                    //add the views
                    mail.AlternateViews.Add(plainView);
                    mail.AlternateViews.Add(htmlView);                    


                    //send the message
                    SmtpClient SmtpServer = new SmtpClient(SMTPServer);
                    SmtpServer.Port = SMTPPort;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(LoginName, Password);
                    SmtpServer.Send(mail);               

                }
            }
            catch (Exception exp)
            {
                //ErrorMessage.ExceptionMessageBox.Show(exp);              
            }

        }

		private void buttonOK_Click(object sender, EventArgs e)
		{
            timer1.Enabled = false;
			this.Close();
		}

		private void buttonDetails_Click(object sender, EventArgs e)
		{
			// on modifie la taille de la fenêtre
			if (this.Size == ClosedSize)
				this.Size = OpenSize;
			else
				this.Size = ClosedSize;
		}

        private void buttonMail_Click(object sender, EventArgs e)
        {
            sendError();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            cpt--;
            if (cpt < 0)
            {
                sendError();
                Close();

            }
            this.Text = _Title + "(close in " + cpt + "sec)";    

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
                return IntPtr.Size * 8;
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
                                case PRODUCT_DATACENTER_SERVER:
                                    edition = "Datacenter Server";
                                    break;
                                case PRODUCT_DATACENTER_SERVER_CORE:
                                    edition = "Datacenter Server (core installation)";
                                    break;
                                case PRODUCT_ENTERPRISE:
                                    edition = "Enterprise";
                                    break;
                                case PRODUCT_ENTERPRISE_N:
                                    edition = "Enterprise N";
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
                                case PRODUCT_HOME_BASIC:
                                    edition = "Home Basic";
                                    break;
                                case PRODUCT_HOME_BASIC_N:
                                    edition = "Home Basic N";
                                    break;
                                case PRODUCT_HOME_PREMIUM:
                                    edition = "Home Premium";
                                    break;
                                case PRODUCT_HOME_PREMIUM_N:
                                    edition = "Home Premium N";
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
                                case PRODUCT_SERVER_FOR_SMALLBUSINESS:
                                    edition = "Windows Essential Server Solutions";
                                    break;
                                case PRODUCT_SERVER_FOR_SMALLBUSINESS_V:
                                    edition = "Windows Essential Server Solutions without Hyper-V";
                                    break;
                                case PRODUCT_SMALLBUSINESS_SERVER:
                                    edition = "Windows Small Business Server";
                                    break;
                                case PRODUCT_STANDARD_SERVER:
                                    edition = "Standard Server";
                                    break;
                                case PRODUCT_STANDARD_SERVER_CORE:
                                    edition = "Standard Server (core installation)";
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
                                case PRODUCT_STORAGE_ENTERPRISE_SERVER:
                                    edition = "Enterprise Storage Server";
                                    break;
                                case PRODUCT_STORAGE_EXPRESS_SERVER:
                                    edition = "Express Storage Server";
                                    break;
                                case PRODUCT_STORAGE_STANDARD_SERVER:
                                    edition = "Standard Storage Server";
                                    break;
                                case PRODUCT_STORAGE_WORKGROUP_SERVER:
                                    edition = "Workgroup Storage Server";
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
                                        /*switch (productType)
                                        {
                                            case 1:
                                                name = "Windows Vista";
                                                break;
                                            case 3:
                                                name = "Windows Server 2008";
                                                break;
                                        }*/

                                        switch (minorVersion)
                                        {
                                            case 0:
                                                name = "Windows Vista";
                                                break;
                                            case 1:
                                                name = "Windows 7";
                                                break;
                                            case 3:
                                                name = "Windows Server 2008";
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
        private const int PRODUCT_SERVER_FOR_SMALLBUSINESS_V = 0x00000023;
        private const int PRODUCT_STANDARD_SERVER_V = 0x00000024;
        private const int PRODUCT_ENTERPRISE_SERVER_V = 0x00000026;
        private const int PRODUCT_STANDARD_SERVER_CORE_V = 0x00000028;
        private const int PRODUCT_ENTERPRISE_SERVER_CORE_V = 0x00000029;
        private const int PRODUCT_HYPERV = 0x0000002A;
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
}
