using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.Threading;
using System.IO;
using System.Net.Mail;
using System.Data.SqlClient;
using Microsoft.Win32;



namespace SDK_SC_Fingerprint
{
    public class VerifyFinger:IDisposable, DPFP.Capture.EventHandler
    {
        static int PROBABILITY_ONE = 0x7FFFFFFF;
        static int FarThreshold = PROBABILITY_ONE / 10000;
        //private Dictionary<string, MyLogListener> LogTab = new Dictionary<string, MyLogListener>();
        private Object logLock = new Object();

        string LogPath = null;// oldLogPath;

        private DPFP.Capture.Capture Capturer;
        private DPFP.Verification.Verification Verificator;        
        public FingerprintMode Mode = FingerprintMode.Verify;

        public TextBox textbox = null;
        private delegate void SetTextBoxDelegate(string s);

        public event NotifyHandlerDelegate NotifyEvent;
        FingerArgs notifyEvent;
        private string ReaderSerialNumber = null;
        private string FingerSerialNumber = null;

        private string serialFingerTouched = null;

        public string getSerialFingerTouched { get { return serialFingerTouched; } }

        private bool bUseEmbedDB = false;
        private bool bIsProcessed = false;

        public ArrayList UserList = new ArrayList();

       // private static Mutex mutex;
        //private string mutexName = "SDK_RFID.1mutex";


        public bool debugFP = true;
        public bool debugFPFormVisible = true;
        public int debugWindowTimeout = 5;
        public bool sendDebugMail = true;
        public bool debugOnFailedOnly = false;

        private bool errorCaptFP = false;

        private Bitmap BmpFP;

        private DateTime TimeFingerTouch;
        private DateTime TimeFingerGone;
        private int CloseFarFinger;

        private string debugFirstName;
        private string debugLastName;
        private FingerIndexValue debugFinger;

        private string pathImage;

        /*private string SenderAdress = null;
        private string LoginName = null;
        private string Password = null;
        private string SMTPServer = null;
        private int SMTPPort = 0;*/

        string SenderAdress = "alert@spacecode-rfid.com";
        string LoginName = "alert@spacecode-rfid.com";
        string Password = "rfidnet";
        string SMTPServer = "mail.spacecode-rfid.com";
        int SMTPPort = 587;
      

        private int previousMS = -1;

        private DPFP.Capture.CaptureFeedback debugFeedback = new DPFP.Capture.CaptureFeedback();

        /*private void initMutex()
        {
            try
            {
                mutex = Mutex.OpenExisting(mutexName, System.Security.AccessControl.MutexRights.FullControl);

            }
            catch
            {
                mutex = new Mutex(false, mutexName);
            }
        }*/

        public VerifyFinger(NotifyHandlerDelegate NotifyEvent,
                            TextBox textbox,
                            string ReaderSerialNumber,
                            string FingerSerialNumber,
                            bool bUseEmbedDB)

        {
            this.NotifyEvent = NotifyEvent;
            this.textbox = textbox;
            this.ReaderSerialNumber = ReaderSerialNumber;
            this.FingerSerialNumber = FingerSerialNumber;
            this.bUseEmbedDB = bUseEmbedDB;


            SetTextBox("Create Finger Object",DateTime.Now);

            //getSMTPInfo(out SenderAdress, out LoginName, out Password, out SMTPServer, out SMTPPort);
            //SetTextBox("GetSMTP : " + SMTPServer + ":" + SMTPPort.ToString(), DateTime.Now);

            //initMutex();
            Stop();
            Init(FingerSerialNumber);
            Start();           
        }

        public void Dispose()
        {
            SetTextBox("Dispose Finger Object", DateTime.Now);
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Free other state (managed objects).
                Stop();
            }
            // Free your own state (unmanaged objects).
            // Set large fields to null.
        }

        private void saveBitmap(Bitmap bmp)
        {
            string fileName = Path.GetRandomFileName();
            fileName = Path.ChangeExtension(fileName, "jpg");
            string outputPath = Path.Combine(@"c:\temp\RfidTracking\log\img\", fileName);
            pathImage = outputPath;
            if (!File.Exists(outputPath))
            {
                FileInfo fi = new FileInfo(outputPath);
                
                string DirectoryPath = fi.DirectoryName;
                if (!Directory.Exists(DirectoryPath))
                    Directory.CreateDirectory(DirectoryPath);

                bmp.Save(outputPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }
        private void WriteToLog(string path, string message, DateTime dt)
        {
            try
            {
                StreamWriter log;
                if (!File.Exists(path))
                {
                    FileInfo fi = new FileInfo(path);
                    string DirectoryPath = fi.DirectoryName;
                    if (!Directory.Exists(DirectoryPath))
                        Directory.CreateDirectory(DirectoryPath);
                    log = new StreamWriter(path);
                }
                else
                {
                    log = File.AppendText(path);
                }

                int currentMS = dt.Millisecond;
                if (currentMS == previousMS)
                {
                    currentMS++;
                    if (currentMS > 999) currentMS = 0;
                }
                previousMS = currentMS;
                // Write to the file:          
                log.Write(dt.ToString("s") + ":" + currentMS.ToString("000") + message);
                // Close the stream:            
                log.Close();

            }
            catch 
            {
               
            }
        }

        public void SetTextBox(string prompt , DateTime dt)
        {
            try
            {
                string logmes = string.Format(" [{0}] FPN : {1} \r\n", this.ReaderSerialNumber, prompt);
                lock (logLock)
                {

                    DateTime TCurrent = dt;
                    DateTime TRef = new DateTime(TCurrent.Year,
                                                  TCurrent.Month,
                                                  TCurrent.Day,
                                                  12, 0, 0);

                    int res = DateTime.Compare(TCurrent, TRef);
                    if (res < 0)
                        LogPath = @"c:\temp\RfidTracking\log\FingerLog" + string.Format("[{0}]_{1}_AM.txt", this.ReaderSerialNumber, DateTime.Now.Date.ToString("dd_MM_yyyy"));
                    else
                        LogPath = @"c:\temp\RfidTracking\log\FingerLog" + string.Format("[{0}]_{1}_PM.txt", this.ReaderSerialNumber, DateTime.Now.Date.ToString("dd_MM_yyyy"));

                    WriteToLog(LogPath, logmes , dt);
                   // ZipAndBackupLog();
                   
                }
                if (textbox != null)
                {
                    TimeSpan time = DateTime.Now.TimeOfDay;
                    string str = (string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}: ",
                            time.Hours, time.Minutes, time.Seconds, time.Milliseconds));
                    TextBoxRefresh(str + logmes);
                }

            }
            catch 
            {
                //                MessageBox.Show("Erreur  SettextBox in finger: " + exp.Message);
            }
        }

       /* private void ZipAndBackupLog()
        {
            lock (logLock)
            {
                if (oldLogPath == null) oldLogPath = LogPath;
                if (!oldLogPath.Equals(LogPath))
                {
                    try
                    {
                        //Zip oldLogPath
                        string pathZip = oldLogPath + ".gz";
                        FileZip oZip = new FileZip(oldLogPath, pathZip, Action.Zip);
                        Thread.Sleep(1000);
                        //delete old log path
                        File.Delete(oldLogPath);
                        // update oldLogPath
                        oldLogPath = LogPath;

                        sendMailLog(pathZip);

                    }

                    catch(Exception exp)
                    {
                        MessageBox.Show("Erreur ZIP and Log : " + exp.Message);
                    }
                }
            }
        }*/
            
        
       /* private void sendMailLog(string PathLog)
        {
            try
            {
                //create the mail message
                MailMessage mail = new MailMessage();

                //set the addresses
                mail.From = new MailAddress("alert@spacecode-rfid.com");
                mail.To.Add("christophe.raoult@spacecode-rfid.com");
                mail.To.Add("eric.gout@spacecode-rfid.com");

                //set the content
                mail.Subject = "FingerPrint LOG [" + ReaderSerialNumber + "]";

                string mailbody = string.Empty;

                mailbody +=  "Finger print Log \r\n\r\n<br />";
             

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

                Attachment myAttachment = new Attachment(PathLog);
                mail.Attachments.Add(myAttachment);


                //send the message
                SmtpClient SmtpServer = new SmtpClient("mail.spacecode-rfid.com");
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("alert@spacecode-rfid.com", "rfidnet");
                SmtpServer.Send(mail);
            }
            catch
            {
             
            }
        }*/
        

        /// <summary>
        /// delegate for uptate textbox in case of cross thread.
        /// </summary>
        /// <param name="s"></param>
        private delegate void TextBoxRefreshDelegate(string s);
        /// <summary>
        /// Update textbox with case of thread involved.
        /// </summary>
        /// <param name="s"></param>
        private void TextBoxRefresh(string s)
        {
            object locker = new object();
            lock (locker)
            {
                if (textbox.InvokeRequired)
                {
                    IAsyncResult ar = textbox.BeginInvoke(new TextBoxRefreshDelegate(TextBoxRefresh), new object[] { s });
                    textbox.EndInvoke(ar);
                }
                else
                {
                    textbox.AppendText(s);
                    textbox.Refresh();
                }
            }
        }


        protected virtual void Init(string serialFinger)
        {
            try
            {
                SetTextBox("Request Init", DateTime.Now);
                Stop();
                if (serialFinger != null)
                    Capturer = new DPFP.Capture.Capture(serialFinger,DPFP.Capture.Priority.Low);  			// Create a capture operation.              
                else
                    Capturer = new DPFP.Capture.Capture(DPFP.Capture.Priority.Low); 
 
                if (null != Capturer)
                {
                    Capturer.EventHandler = this;                   
                    notifyEvent = new FingerArgs(ReaderSerialNumber, FingerArgs.FingerNotify.RN_FingerSucceedToInit, "Finger Initialized");
                    if (NotifyEvent != null)
                    {
                        SetTextBox("Finger Initialized", DateTime.Now);// Subscribe for capturing events.
                        NotifyEvent(this, notifyEvent);
                    }
                   
                }
                else
                {                   
                    notifyEvent = new FingerArgs(ReaderSerialNumber, FingerArgs.FingerNotify.RN_FingerFailedToInit, "Finger failed to Initialize");
                    if (NotifyEvent != null)
                    {
                        SetTextBox("Can't initiate capture operation!", DateTime.Now);
                        NotifyEvent(this, notifyEvent);
                    }
                }
            }
            catch
            {                
                notifyEvent = new FingerArgs(ReaderSerialNumber, FingerArgs.FingerNotify.RN_FingerFailedToInit, "Finger failed to Initialize");
                if (NotifyEvent != null)
                {
                    SetTextBox("Finger failed to Initialize", DateTime.Now);
                    NotifyEvent(this, notifyEvent);
                }                 
                
            }
            if (Mode == FingerprintMode.Verify)
            {
                Verificator = new DPFP.Verification.Verification(FarThreshold);		// Create a fingerprint template verificator FAR 0.01% > 214748
                SetTextBox("Init Verificator with FAR : " + Verificator.FARRequested.ToString(), DateTime.Now);
                if (bUseEmbedDB)
                {
                    #if UseSQLITE
                    DBClassUser dbJob = new DBClassUser(UserList);
                    dbJob.RecoverUser();
                    #endif
                }
            }         
        }
        protected void Start()
        {
            SetTextBox("Request Start", DateTime.Now);
            if (null != Capturer)
            {
                try
                {
                    Capturer.StartCapture();
                    if (Mode == FingerprintMode.Verify)
                    {
                        notifyEvent = new FingerArgs(ReaderSerialNumber, FingerArgs.FingerNotify.RN_FingerSucceedToStartCapture, "Fingerprint wait Capture");
                        if (NotifyEvent != null)
                        {
                            SetTextBox("Capture Process", DateTime.Now);
                            NotifyEvent(this, notifyEvent);
                        }
                    }
                }
                catch
                {                  
                    notifyEvent = new FingerArgs(ReaderSerialNumber, FingerArgs.FingerNotify.RN_FingerFailedToStartCapture, "Can't initiate capture!");
                    if (NotifyEvent != null)
                    {
                        SetTextBox("Can't initiate capture!", DateTime.Now);
                        NotifyEvent(this, notifyEvent);
                    }
                }
            }
        }
        protected void Stop()
        {
            SetTextBox("Request Stop", DateTime.Now);
            if (null != Capturer)
            {
                try
                {
                    Capturer.StopCapture();                    
                    notifyEvent = new FingerArgs(ReaderSerialNumber, FingerArgs.FingerNotify.RN_FingerSucceedToStopCapture ,"Fingerprint Stopped");
                    if (NotifyEvent != null)
                    {
                        SetTextBox("Fingerprint Stopped", DateTime.Now);
                        NotifyEvent(this, notifyEvent);
                    }
             
                   
                }
                catch
                {                   
                    notifyEvent = new FingerArgs(ReaderSerialNumber, FingerArgs.FingerNotify.RN_FingerFailedToStopCapture, "Fingerprint fails to stop");
                    if (NotifyEvent != null)
                    {
                        SetTextBox("Can't terminate capture!", DateTime.Now);
                        NotifyEvent(this, notifyEvent);
                    }
                }
            }
        }

        protected virtual void Process(DPFP.Sample Sample)
        {
            if (Mode == FingerprintMode.Verify)
            {
               
                bool bVerified = false;
                int fingerFind = -1;
                UserClass VerifiedUser = new UserClass();

                // Process the sample and create a feature set for the enrollment purpose.
                DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Verification);

                if (features != null)
                {
                    foreach (UserClass TheUser in UserList)
                    {
                        FingerData fingdata = new FingerData();
                        fingdata.CopyUserToFinger(TheUser);

                        //foreach (DPFP.Template Template in fingdata.Templates)
                        for (int index = 0 ; index < 10 ; index ++)
                        {
                            DPFP.Template Template = fingdata.Templates[index];
                            if (Template != null)
                            {

                                FingerIndexValue fiv = (FingerIndexValue)index;
                                // Compare the feature set with our template
                                DPFP.Verification.Verification.Result result = new DPFP.Verification.Verification.Result();
                                Verificator.Verify(features, Template, ref result);

                                SetTextBox("Verify : " + TheUser.firstName + " " +TheUser.lastName + " : " + fiv.ToString() +
                                " - FAR :" + result.FARAchieved.ToString() + " / " + Verificator.FARRequested.ToString()
                                + " - Verified : " + result.Verified.ToString(), DateTime.Now);

                                if (CloseFarFinger > result.FARAchieved)
                                    CloseFarFinger = result.FARAchieved;

                                if ((result.Verified) || (result.FARAchieved < Verificator.FARRequested))
                                {
                                    bVerified = true;
                                    VerifiedUser = TheUser;
                                    fingerFind = index;
                                    break;
                                }
                               
                            }
                        }
                        if (bVerified) break;
                    }
                    if (bVerified)
                    {
                        string strUser = VerifiedUser.firstName + ";" + VerifiedUser.lastName + ";" + fingerFind.ToString();
                        debugFirstName = VerifiedUser.firstName;
                        debugLastName = VerifiedUser.lastName;
                        debugFinger = (FingerIndexValue)fingerFind;

                        if (debugOnFailedOnly)
                            errorCaptFP = false;    
                        
                        notifyEvent = new FingerArgs(ReaderSerialNumber, FingerArgs.FingerNotify.RN_AuthentificationCompleted, strUser);
                        if (NotifyEvent != null)
                        {
                            SetTextBox("User verify : " + strUser, DateTime.Now);
                            NotifyEvent(this, notifyEvent);
                        }
                    }
                    else
                    {   
                        debugFirstName = "Unknown User";
                        debugLastName = "Unknown User";
                        debugFinger = FingerIndexValue.Unknown_Finger;   
                        notifyEvent = new FingerArgs(ReaderSerialNumber, FingerArgs.FingerNotify.RN_FingerUserUnknown, "Unknown User");
                        if (NotifyEvent != null)
                        {
                            SetTextBox("Unknown User", DateTime.Now);
                            NotifyEvent(this, notifyEvent);
                        }                                            
                    }
                }
               
            }  
        }
        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            bIsProcessed = true;
            
            notifyEvent = new FingerArgs(ReaderSerialNumber, FingerArgs.FingerNotify.RN_CaptureGood, "The finger was successively capture by the fingerprint reader.");
            if (NotifyEvent != null)
            {
                SetTextBox("The finger was successively capture by the fingerprint reader.", DateTime.Now);
                NotifyEvent(this, notifyEvent);
            }

            saveBitmap(new Bitmap(ConvertSampleToBitmap(Sample)));
            BmpFP = new Bitmap(ConvertSampleToBitmap(Sample));
            Process(Sample);
        }
        public void OnFingerGone(object Capture, string ReaderSerialNumber)
        {
            TimeSpan ts = TimeSpan.MinValue;
            notifyEvent = new FingerArgs(ReaderSerialNumber, FingerArgs.FingerNotify.RN_FingerGone, "The finger was removed from the fingerprint reader.");
            if (NotifyEvent != null)
            {
                SetTextBox("The finger was removed from the fingerprint reader.", DateTime.Now);
                NotifyEvent(this, notifyEvent);
            }           

            try
            {
                TimeFingerGone = DateTime.Now;
                ts = TimeFingerGone - TimeFingerTouch;                
                if ((debugFP) && (ts.TotalSeconds < 10.0) && (errorCaptFP))
                {
                    DebugFP wf = new DebugFP(debugFPFormVisible,
                                             debugWindowTimeout,
                                             sendDebugMail,
                                             BmpFP,
                                             this.ReaderSerialNumber,
                                             TimeFingerTouch,
                                             TimeFingerGone,
                                             debugFirstName,
                                             debugLastName,
                                             debugFinger,
                                             CloseFarFinger,
                                             FarThreshold,
                                             debugFeedback,
                                             pathImage,
                                             bIsProcessed,
                                             SenderAdress,
                                             LoginName,
                                             Password,
                                             SMTPServer,
                                             SMTPPort);
                    wf.Show();

                    errorCaptFP = false;
                }
            }
            catch(Exception exp)
            {
                SetTextBox("FP exception : " + exp.Message, DateTime.Now);
                //MessageBox.Show("FP exception : " + exp.Message);
            }
            

            if (!bIsProcessed)
            {

                //renewFP();
                //Stop();
                //Init(FingerSerialNumber);
                //Start();
                //System.Threading.Thread.Sleep(500);
                notifyEvent = new FingerArgs(ReaderSerialNumber, FingerArgs.FingerNotify.RN_CaptureBad, "The finger was not successively capture by the fingerprint reader.");
                if (NotifyEvent != null)
                {
                    SetTextBox("The finger was not successively capture by the fingerprint reader.", DateTime.Now);
                    NotifyEvent(this, notifyEvent);
                }
            }          
        }
        public void OnFingerTouch(object Capture, string ReaderSerialNumber)
        {
            bIsProcessed = false;
            serialFingerTouched = ReaderSerialNumber;
            notifyEvent = new FingerArgs(ReaderSerialNumber, FingerArgs.FingerNotify.RN_FingerTouch, "The fingerprint reader was touched.");
            if (NotifyEvent != null)
            {
                SetTextBox("The fingerprint reader was touched.", DateTime.Now);
                NotifyEvent(this, notifyEvent);
            }
          

            TimeFingerTouch = DateTime.Now;
            CloseFarFinger = int.MaxValue;
            debugFeedback = DPFP.Capture.CaptureFeedback.None;
            BmpFP = null;            
            TimeFingerGone = TimeFingerTouch;   
            debugFirstName = string.Empty;
            debugLastName = string.Empty;
            debugFinger = FingerIndexValue.Unknown_Finger;
            pathImage = string.Empty;

            errorCaptFP = true;

       
        }
        public void OnReaderConnect(object Capture, string ReaderSerialNumber)
        {        
           /*Init(FingerSerialNumber);
           Start();   */
           TimeFingerTouch = DateTime.Now;
           notifyEvent = new FingerArgs(ReaderSerialNumber, FingerArgs.FingerNotify.RN_FingerprintConnect, "The fingerprint reader was connected.");
           if (NotifyEvent != null)
           {
               SetTextBox("The fingerprint reader was connected.", DateTime.Now);
               NotifyEvent(this, notifyEvent);
           }
          
        }
        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {
           // Stop();
            
            notifyEvent = new FingerArgs(ReaderSerialNumber, FingerArgs.FingerNotify.RN_FingerprintDisconnect, "The fingerprint reader was disconnected.");
            if (NotifyEvent != null)
            {
                SetTextBox("The fingerprint reader was disconnected.", DateTime.Now);
                NotifyEvent(this, notifyEvent);
            }
          
        }
        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback)
        {
           
        }

        private Bitmap cleanBitmap(Bitmap BmpToClean)
        {
            int width;
            int height;
            Color color;
            Bitmap bmp = new Bitmap(BmpToClean);

            width = bmp.Width;
            height = bmp.Height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    color = bmp.GetPixel(x, y);
                    if (color.R > 105)
                        bmp.SetPixel(x, y, System.Drawing.Color.Black);
                    else bmp.SetPixel(x, y, System.Drawing.Color.AliceBlue);
                }
            }
            return bmp;
        }

        protected Bitmap ConvertSampleToBitmap(DPFP.Sample Sample)
        {
            DPFP.Capture.SampleConversion Convertor = new DPFP.Capture.SampleConversion();	// Create a sample convertor.
            Bitmap bitmap = null;												            // TODO: the size doesn't matter
            Convertor.ConvertToPicture(Sample, ref bitmap);									// TODO: return bitmap as a result
            return bitmap;
        }
        protected DPFP.FeatureSet ExtractFeatures(DPFP.Sample Sample, DPFP.Processing.DataPurpose Purpose)
        {
            DPFP.Processing.FeatureExtraction Extractor = new DPFP.Processing.FeatureExtraction();	// Create a feature extractor
            DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet features = new DPFP.FeatureSet();
            Extractor.CreateFeatureSet(Sample, Purpose, ref feedback, ref features);			// TODO: return features as a result?
            debugFeedback = feedback;
            if (feedback == DPFP.Capture.CaptureFeedback.Good)
            {
                notifyEvent = new FingerArgs(ReaderSerialNumber, FingerArgs.FingerNotify.RN_CaptureGood, "Capture is Good");
                if (NotifyEvent != null) NotifyEvent(this, notifyEvent);
                return features;
            }
            else
            {
                notifyEvent = new FingerArgs(ReaderSerialNumber, FingerArgs.FingerNotify.RN_CaptureBad, feedback.ToString());
                if (NotifyEvent != null) NotifyEvent(this, notifyEvent);    
                return null;
            }
        }

        

        public static string DencryptPassword(string Pass)
        {
            string str2 = "";
            string str3 = Pass;
            int num = 0;
            int length = str3.Length;
            while (num < length)
            {
                char ch = str3[num];
                char[] chrBuffer = { Convert.ToChar(ch) };
                byte[] bytBuffer = Encoding.Default.GetBytes(chrBuffer);

                int chtoint = bytBuffer[0] - 0x2d;
                str2 = str2 + Chr(chtoint)[0];
                num++;
            }
            return str2;
        }
        public static string Chr(int p_intByte)
        {

            byte[] bytBuffer = BitConverter.GetBytes(p_intByte);

            return Encoding.Unicode.GetString(bytBuffer);

        }
        public  bool renewFP(bool bFullRenew)
        {
            bool ret = false;           
            string path = Application.StartupPath + Path.DirectorySeparatorChar + "driver" + Path.DirectorySeparatorChar + "FingerPrint"
                         + Path.DirectorySeparatorChar + "devcon" + Path.DirectorySeparatorChar + "x86" + Path.DirectorySeparatorChar;
                         
            try
            {
                if (DataClass.OSInfo.Bits == 64)
                {
                    path = Application.StartupPath + Path.DirectorySeparatorChar + "driver" + Path.DirectorySeparatorChar + "FingerPrint"
                    + Path.DirectorySeparatorChar + "devcon" + Path.DirectorySeparatorChar + "x64" + Path.DirectorySeparatorChar;
                       
                }

                /*if (File.Exists(path + "removeFP.bat"))
                {
                  
                    Thread.Sleep(2000);
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo.FileName = path + "removeFP.bat";
                    proc.StartInfo.WorkingDirectory = path;
                    proc.Start();
                    SetTextBox("Renew FP launched.", DateTime.Now);
                    proc.WaitForExit();                    
                    ExecuteCommandSync("net stop dphost"); 
                    ExecuteCommandSync("net start dphost");
                    ret = true;
                }*/
                if (bFullRenew)
                {
                    if (File.Exists(path + "devcon.exe"))
                    {
                        removeFP(path);
                        SearchFP(path);
                     }
                }
                ExecuteCommandSync("net stop dphost");
                ExecuteCommandSync("net start dphost");

            }
            catch (Exception exp)
            {
                SetTextBox("FP renew exception : " + exp.Message, DateTime.Now);
              
            }    
            /*ExecuteCommandSync("net stop dphost");
            Thread.Sleep(2000);
            ExecuteCommandSync("net start dphost");*/

            return ret;
        }

        public void removeFP(string path)
        {
             System.Diagnostics.Process proc = new System.Diagnostics.Process();
             proc.StartInfo.FileName = path + "devcon.exe";
             proc.StartInfo.Arguments = "remove usb*VID_05BA*";
             //proc.StartInfo.RedirectStandardError = true;
             //proc.StartInfo.RedirectStandardOutput = true;
             //proc.StartInfo.UseShellExecute = false;
             proc.Start();
             proc.WaitForExit(60000);
        }
        public void SearchFP(string path)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = path + "devcon.exe";
            proc.StartInfo.Arguments = "rescan";
           // proc.StartInfo.RedirectStandardError = true;
           // proc.StartInfo.RedirectStandardOutput = true;
            //proc.StartInfo.UseShellExecute = false;
            proc.Start();
            proc.WaitForExit(60000);
        }

        /// <summary>
        /// Executes a shell command synchronously.
        /// </summary>
        /// <param name="command">string command</param>
        /// <returns>string, as output of the command.</returns>
        public void ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                procStartInfo.CreateNoWindow = false;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
               // proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                proc.Start();
                proc.WaitForExit(10000);
                
            }
            catch (Exception objException)
            {
                // Log the exception
                SetTextBox("FP renew exception : " + objException.Message, DateTime.Now);
            }
        }
    }
}
