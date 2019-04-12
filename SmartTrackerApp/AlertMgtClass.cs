using System;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net.Mail;
using DataClass;
using DBClass;
using System.Threading;


namespace smartTracker
{
    public static class AlertMgtClass
    {

      

        public static void treatAlert(AlertType alertType,DeviceInfo device, UserClassTemplate utc , InventoryData inv , string spareData, bool bShowWindowBox)
        {
            switch (alertType)
            {
                case AlertType.AT_Power_Cut:
                    InputData input = new InputData(AlertType.AT_Power_Cut, device, utc, spareData,bShowWindowBox); 
                    ThreadPool.QueueUserWorkItem(new WaitCallback(treatPowerCut), input);                    
                    break;
                case AlertType.AT_Usb_Unplug:
                    InputData input2 = new InputData(AlertType.AT_Usb_Unplug, device, utc, spareData,bShowWindowBox);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(treatUsbUnplug), input2);        
                  
                break;
                case AlertType.AT_Door_Open_Too_Long:
                         InputData input3 = new InputData(AlertType.AT_Door_Open_Too_Long, device, utc,spareData, bShowWindowBox);
                         ThreadPool.QueueUserWorkItem(new WaitCallback(treatDoorOpenTooLong), input3);                     
                    break;
                case AlertType.AT_Finger_Alert:
                        InputData input4 = new InputData(AlertType.AT_Finger_Alert, device, utc,spareData, bShowWindowBox);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(treatFingerAlert), input4);
                        break;
                case AlertType.AT_Remove_Too_Many_Items:
                       InputData input5 = new InputData(AlertType.AT_Remove_Too_Many_Items, device, utc,spareData, bShowWindowBox);
                       ThreadPool.QueueUserWorkItem(new WaitCallback(treatItemRemoved), input5);
                        break;
                case AlertType.AT_Limit_Value_Exceed:
                        InputData input6 = new InputData(AlertType.AT_Limit_Value_Exceed, device, utc, spareData,bShowWindowBox);
                       ThreadPool.QueueUserWorkItem(new WaitCallback(treatValueRemoved), input6);
                    break;
                case AlertType.AT_Move_Sensor:
                    InputData input7 = new InputData(AlertType.AT_Move_Sensor, device, utc, spareData, bShowWindowBox);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(treatMoveSensor), input7);                    
                    break;
                case AlertType.AT_Max_Fridge_Temp:
                    InputData input8 = new InputData(AlertType.AT_Max_Fridge_Temp, device, utc, spareData, bShowWindowBox);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(treatMaxFridgeTemp), input8);
                    break;
                case AlertType.AT_Remove_Tag_Max_Time:
                    InputData input9 = new InputData(AlertType.AT_Max_Fridge_Temp, device, utc, spareData, bShowWindowBox);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(treatMaxTimeTagRemoved), input9);
                    break;
                case AlertType.AT_Stock_Limit:
                    InputData input10 = new InputData(AlertType.AT_Stock_Limit, device, utc, spareData, bShowWindowBox);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(treatStockLimit), input10);
                    break;
                case AlertType.AT_DLC_Expired:
                     InputData input11 = new InputData(AlertType.AT_DLC_Expired, device, utc, spareData, bShowWindowBox);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(treatDLCExpired), input11);
                    break;
                case AlertType.AT_Bad_Blood_Patient:
                     InputData input12 = new InputData(AlertType.AT_Bad_Blood_Patient, device, utc, spareData, bShowWindowBox);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(treatBadBloodPatient), input12);
                    break;


            }
        }

        private static void treatMoveSensor(object o)
        {
            InputData input = o as InputData;           
            if (input.BShowWindowBox)
            {
                MessageBoxEx.Show(Properties.ResStrings.strAlertSensor, Properties.ResStrings.strAlertDetected, MessageBoxButtons.OK, MessageBoxIcon.Stop, 10000);               
            }
           
            MainDBClass db = new MainDBClass();
            if (db.OpenDB())
            {
                smtpInfo smtp = db.getSmtpInfo(true);
                alertInfo alert = db.getAlertInfo(AlertType.AT_Move_Sensor, true);
                db.CloseDB();

                if ((alert != null) && (smtp != null))
                    createAndSendMail(alert, smtp, input.Device, input.UTC, input.SpareData);
            }
            
        }

        private static void treatValueRemoved(object o)
        {
            InputData input = o as InputData;
            
            if (input.BShowWindowBox)
            {
                MessageBoxEx.Show(Properties.ResStrings.strAlertValue, Properties.ResStrings.strAlertDetected, MessageBoxButtons.OK, MessageBoxIcon.Stop, 10000);
               
            }           
            MainDBClass db = new MainDBClass();
            if (db.OpenDB())
            {
                smtpInfo smtp = db.getSmtpInfo(true);
                alertInfo alert = db.getAlertInfo(AlertType.AT_Limit_Value_Exceed, true);
                db.CloseDB();

                if ((alert != null) && (smtp != null))
                    createAndSendMail(alert, smtp, input.Device, input.UTC, input.SpareData);
            }            
        }

        private static void treatItemRemoved(object o)
        {
            InputData input = o as InputData;
            
            if (input.BShowWindowBox)
            {
                DialogResult ret = MessageBoxEx.Show(Properties.ResStrings.strAlertItems, Properties.ResStrings.strAlertDetected, MessageBoxButtons.OK, MessageBoxIcon.Stop, 10000);
              
            }
           
            MainDBClass db = new MainDBClass();
            if (db.OpenDB())
            {
                smtpInfo smtp = db.getSmtpInfo(true);
                alertInfo alert = db.getAlertInfo(AlertType.AT_Remove_Too_Many_Items, true);
                db.CloseDB();

                if ((alert != null) && (smtp != null))
                    createAndSendMail(alert, smtp, input.Device, input.UTC, input.SpareData);
            }            
        }
     

        private static void treatFingerAlert(object o)
        {
            InputData input = o as InputData;

            bool sendMail = true;
            if (input.BShowWindowBox)
            {
                DialogResult ret = MessageBoxEx.Show(Properties.ResStrings.strAlertFinger, Properties.ResStrings.strAlertDetected, MessageBoxButtons.YesNo, MessageBoxIcon.Stop, 10000);
                if (ret == DialogResult.No) sendMail = false;
            }

            if (sendMail)
            {
                MainDBClass db = new MainDBClass();
                if (db.OpenDB())
                {
                    smtpInfo smtp = db.getSmtpInfo(true);
                    alertInfo alert = db.getAlertInfo(AlertType.AT_Finger_Alert, true);
                    db.CloseDB();

                    if ((alert != null) && (smtp != null))
                        createAndSendMail(alert, smtp, input.Device, input.UTC, input.SpareData);
                }
            }
        }
     
        private static void treatPowerCut(object o)
        {
            InputData input  = o as InputData;

            bool sendMail = true;
            if (input.BShowWindowBox)
            {
                DialogResult ret = MessageBoxEx.Show(Properties.ResStrings.strAlertPower, Properties.ResStrings.strAlertDetected, MessageBoxButtons.YesNo, MessageBoxIcon.Stop, 10000);
                if (ret == DialogResult.No) sendMail = false;
            }

            if (sendMail)
            {
                MainDBClass db = new MainDBClass();
                if (db.OpenDB())
                {                  
                    smtpInfo smtp = db.getSmtpInfo(true);
                    alertInfo alert = db.getAlertInfo(AlertType.AT_Power_Cut, true);                    
                    db.CloseDB();

                    if ((alert != null) && (smtp != null))
                        createAndSendMail(alert, smtp, input.Device, null, input.SpareData);
                }
             }
        }
        private static void treatUsbUnplug(object o)
        {
            bool sendMail = true;
            InputData input = o as InputData;
            if (input.BShowWindowBox)
            {
                DialogResult ret = MessageBoxEx.Show(Properties.ResStrings.strAlertUsb, Properties.ResStrings.strAlertDetected, MessageBoxButtons.YesNo, MessageBoxIcon.Stop, 10000);
                if (ret == DialogResult.No) sendMail = false;
            }

            if (sendMail)
            {
                MainDBClass db = new MainDBClass();
                if (db.OpenDB())
                {
                    smtpInfo smtp = db.getSmtpInfo(true);
                    alertInfo alert = db.getAlertInfo(AlertType.AT_Usb_Unplug, true);
                    db.CloseDB();
                    if ((alert != null) && (smtp != null))
                        createAndSendMail(alert, smtp, input.Device, null, input.SpareData);                    
                }             
        
            }
        }
        private static void treatDoorOpenTooLong(object o)
        {
            bool sendMail = true;
            InputData input = o as InputData;
            if (input.BShowWindowBox)
            {
                DialogResult ret = MessageBoxEx.Show(Properties.ResStrings.strAlertDoor, Properties.ResStrings.strAlertDetected, MessageBoxButtons.YesNo, MessageBoxIcon.Stop, 10000);
                if (ret == DialogResult.No) sendMail = false;
            }

            if (sendMail)
            {
                MainDBClass db = new MainDBClass();
                if (db.OpenDB())
                {
                    smtpInfo smtp = db.getSmtpInfo(true);
                    alertInfo alert = db.getAlertInfo(AlertType.AT_Door_Open_Too_Long, true);
                    db.CloseDB();
                    if ((alert != null) && (smtp != null))
                        createAndSendMail(alert, smtp, input.Device, input.UTC,input.SpareData);
                }
            }
        }
        private static void treatMaxFridgeTemp(object o)
        {
            bool sendMail = true;
            InputData input = o as InputData;
            if (input.BShowWindowBox)
            {
                DialogResult ret = MessageBoxEx.Show( Properties.ResStrings.strAlertTemp, Properties.ResStrings.strAlertDetected, MessageBoxButtons.YesNo, MessageBoxIcon.Stop, 10000);
                if (ret == DialogResult.No) sendMail = false;
            }

            if (sendMail)
            {
                MainDBClass db = new MainDBClass();
                if (db.OpenDB())
                {
                    smtpInfo smtp = db.getSmtpInfo(true);
                    alertInfo alert = db.getAlertInfo(AlertType.AT_Max_Fridge_Temp, true);
                    db.CloseDB();
                    if ((alert != null) && (smtp != null))
                        createAndSendMail(alert, smtp, input.Device, input.UTC, input.SpareData);
                }
            }
        }
        private static void treatStockLimit(object o)
        {
            bool sendMail = true;
            InputData input = o as InputData;
            if (input.BShowWindowBox)
            {
                DialogResult ret = MessageBoxEx.Show(Properties.ResStrings.strAlertStock, Properties.ResStrings.strAlertDetected, MessageBoxButtons.YesNo, MessageBoxIcon.Stop, 10000);
                if (ret == DialogResult.No) sendMail = false;
            }

            if (sendMail)
            {
                MainDBClass db = new MainDBClass();
                if (db.OpenDB())
                {
                    smtpInfo smtp = db.getSmtpInfo(true);
                    alertInfo alert = db.getAlertInfo(AlertType.AT_Stock_Limit, true);
                    db.CloseDB();
                    if ((alert != null) && (smtp != null))
                        createAndSendMail(alert, smtp, input.Device, input.UTC, input.SpareData);
                }
            }
        }
        private static void treatDLCExpired(object o)
        {
            bool sendMail = true;
            InputData input = o as InputData;
            if (input.BShowWindowBox)
            {
                DialogResult ret = MessageBoxEx.Show(Properties.ResStrings.strAlertProduct, Properties.ResStrings.strAlertDetected, MessageBoxButtons.YesNo, MessageBoxIcon.Stop, 10000);
                if (ret == DialogResult.No) sendMail = false;
            }

            if (sendMail)
            {
                MainDBClass db = new MainDBClass();
                if (db.OpenDB())
                {
                    smtpInfo smtp = db.getSmtpInfo(true);
                    alertInfo alert = db.getAlertInfo(AlertType.AT_DLC_Expired, true);
                    db.CloseDB();
                    if ((alert != null) && (smtp != null))
                        createAndSendMail(alert, smtp, input.Device, input.UTC, input.SpareData);
                }
            }
        }
      

        private static void treatMaxTimeTagRemoved(object o)
        {
            bool sendMail = true;
            InputData input = o as InputData;
            if (input.BShowWindowBox)
            {
                DialogResult ret = MessageBoxEx.Show(Properties.ResStrings.strAlertTime, Properties.ResStrings.strAlertDetected, MessageBoxButtons.YesNo, MessageBoxIcon.Stop, 10000);
                if (ret == DialogResult.No) sendMail = false;
            }

            if (sendMail)
            {
                MainDBClass db = new MainDBClass();
                if (db.OpenDB())
                {
                    smtpInfo smtp = db.getSmtpInfo(true);
                    alertInfo alert = db.getAlertInfo(AlertType.AT_Remove_Tag_Max_Time, true);
                    db.CloseDB();
                    if ((alert != null) && (smtp != null))
                        createAndSendMail(alert, smtp, input.Device, input.UTC, input.SpareData);
                }
            }
        }


        private static void treatBadBloodPatient(object o)
        {
            bool sendMail = true;
            InputData input = o as InputData;
            if (input.BShowWindowBox)
            {
                DialogResult ret = MessageBoxEx.Show(Properties.ResStrings.strAlertBlood, Properties.ResStrings.strAlertDetected, MessageBoxButtons.YesNo, MessageBoxIcon.Stop, 10000);
                if (ret == DialogResult.No) sendMail = false;
            }

            if (sendMail)
            {
                MainDBClass db = new MainDBClass();
                if (db.OpenDB())
                {
                    smtpInfo smtp = db.getSmtpInfo(true);
                    alertInfo alert = db.getAlertInfo(AlertType.AT_Bad_Blood_Patient, true);
                    db.CloseDB();
                    if ((alert != null) && (smtp != null))
                        createAndSendMail(alert, smtp, input.Device, input.UTC, input.SpareData);
                }
            }
        }


        

        private static void createAndSendMail(alertInfo alert, smtpInfo smtp, DeviceInfo device, UserClassTemplate utc, string spareData)
        {
            bool mailcreated = false;
            //create the mail message
            MailMessage mail = null;
            string mailbody = null;

            mailcreated = true;
            mail = new MailMessage();
            //set the addresses
            mail.From = new MailAddress(smtp.sender);

            string[] recpt = alert.RecipientList.Split(';');
            foreach (string str in recpt)
                mail.To.Add(str.Trim());
            if (!string.IsNullOrEmpty(alert.BCCRecipientList))
            {
                string[] bcc = alert.BCCRecipientList.Split(';');
                foreach (string str in bcc)
                    mail.Bcc.Add(str.Trim());
            }
            if (!string.IsNullOrEmpty(alert.CCRecipientList))
            {
                string[] cc = alert.CCRecipientList.Split(';');
                foreach (string str in cc)
                    mail.CC.Add(str.Trim());
            }

           /* mail.Subject = alert.MailSubject + " " + device.DeviceName + " [S/N:" + device.SerialRFID + "]";
            mailbody = string.Empty;

            mailbody += "Alert type : <B>" + alert.AlertMessage + "</B><br />";
            mailbody += "Alert date : <B>" + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + "</B><br />";
            mailbody += "Alert on device : <B>" + device.DeviceName + " [s/n: " + device.SerialRFID + "]" + "</B><br />";

            switch (alert.type)
            {  
                case AlertType.AT_Door_Open_Too_Long:
                case AlertType.AT_Finger_Alert:
                    mailbody += "Alert generated  by user : <B>" + utc.firstName + " " + utc.lastName + "</B><br />";
                    break;
                case AlertType.AT_Remove_Too_Many_Items:
                case AlertType.AT_Limit_Value_Exceed:
                    mailbody += "Alert generated  by user : <B>" + utc.firstName + " " + utc.lastName + "</B><br />";
                    mailbody += "User removed <B>" + spareData + "</B><br />";
                    break;
            }     */

            string subject = alert.MailSubject;
            subject = subject.Replace("\n", "<br/>");
            if (device != null)
            {
                subject = subject.Replace("[READERNAME]", device.DeviceName);
                subject = subject.Replace("[READERSERIAL]", device.SerialRFID);
            }
            if (utc != null)
            {
                subject = subject.Replace("[USERNAME]", utc.firstName + " " + utc.lastName);
            }
            subject = subject.Replace("[DATE]", DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString());
            mail.Subject = subject;

            string body = alert.AlertMessage;            
            body = body.Replace("\n", "<br/>");
            if (device != null)
            {
                body = body.Replace("[READERNAME]", device.DeviceName);
                body = body.Replace("[READERSERIAL]", device.SerialRFID);
            }
            if (utc != null)
            {
                body = body.Replace("[USERNAME]", utc.firstName + " " + utc.lastName);
            }          

            body = body.Replace("[DATE]", DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString());          
            body += "<br />";
            switch (alert.type)
            {
                case AlertType.AT_Remove_Too_Many_Items:
                case AlertType.AT_Limit_Value_Exceed:

                    body += "<br />User removed " + spareData + "<br />";
                    break;
                case  AlertType.AT_Max_Fridge_Temp:

                    body += "<br />"+ spareData + "<br />";
                    break;

                case AlertType.AT_Remove_Tag_Max_Time:

                    string[] dt = spareData.Split(';');
                    
                    if (dt!= null)
                    {
                        int nbTag = dt.Length - 1;
                        body += "<br />All the following " + nbTag + " tag(s) have been removed from device than more " + dt[0] + " Minutes<br />";

                    for (int i = 1; i < dt.Length; i++)
                        body += " <br /> " + dt[i];
                    }
                    break;
                case AlertType.AT_Stock_Limit:

                    string[] spData = spareData.Split(';');
                    if (spData != null)
                    {
                        for (int i = 0; i < spData.Length; i+=2)
                            body += "<br />Product " + spData[i] + " reach low stock limit : " + spData[i+1] + " Left";
                    }
                    break;
                case AlertType.AT_DLC_Expired:

                   /* string[] dt2 = spareData.Split(';');
                    
                    if (dt2!= null)
                    {
                        int nbTag = dt2.Length - 1;
                        body += "<br />All the following " + nbTag + " product(s) have overtake their date of use<br />";

                    for (int i = 1; i < dt2.Length; i++)
                        body += " <br /> " + dt2[i];
                    }
                    */
                    break;

                case AlertType.AT_Bad_Blood_Patient:
                    string[] dt3 = spareData.Split(';');

                    if (dt3 != null)
                    {
                        string patient = dt3[0];
                        body += "<br />The following blood bags removed are not for patient " + patient + "<br />";

                        for (int i = 1; i < dt3.Length; i++)
                            body += " <br /> " + dt3[i];
                    }


                    break;
            }
            mailbody = body;
            /*Attachment data = new Attachment(path);
            mail.Attachments.Add(data);*/

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
                SmtpClient SmtpServer = new SmtpClient(smtp.smtp);
                SmtpServer.Port = smtp.port;
                if (smtp.bUseSSL) SmtpServer.EnableSsl = true;
                SmtpServer.Credentials = new System.Net.NetworkCredential(smtp.login, smtp.pwd);
                SmtpServer.Send(mail);

            }

        }
    }

    public class InputData
    {
        public InputData(AlertType alertType, DeviceInfo device, UserClassTemplate utc,string spareData, bool bShowWindowBox)
        {
            this.Alert = alertType;
            this.Device = device;
            this.UTC = utc;
            this.BShowWindowBox = bShowWindowBox;
            this.SpareData = spareData;
        }

        public  AlertType Alert { get; set; }
        public DeviceInfo Device { get; set; }
        public UserClassTemplate UTC{ get; set; }
        public bool BShowWindowBox { get; set; }
        public string SpareData { get; set; }

    }

    public class MessageBoxEx
    {
        public static DialogResult Show(string text, uint uTimeout)
        {
            Setup("", uTimeout);
            return MessageBox.Show(text);
        }

        public static DialogResult Show(string text, string caption, uint uTimeout)
        {
            Setup(caption, uTimeout);
            return MessageBox.Show(text, caption);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, uint uTimeout)
        {
            Setup(caption, uTimeout);
            return MessageBox.Show(text, caption, buttons);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, uint uTimeout)
        {
            Setup(caption, uTimeout);
            return MessageBox.Show(text, caption, buttons, icon);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton, uint uTimeout)
        {
            Setup(caption, uTimeout);
            return MessageBox.Show(text, caption, buttons, icon, defButton);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton, MessageBoxOptions options, uint uTimeout)
        {
            Setup(caption, uTimeout);
            return MessageBox.Show(text, caption, buttons, icon, defButton, options);
        }

        public static DialogResult Show(IWin32Window owner, string text, uint uTimeout)
        {
            Setup("", uTimeout);
            return MessageBox.Show(owner, text);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, uint uTimeout)
        {
            Setup(caption, uTimeout);
            return MessageBox.Show(owner, text, caption);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, uint uTimeout)
        {
            Setup(caption, uTimeout);
            return MessageBox.Show(owner, text, caption, buttons);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, uint uTimeout)
        {
            Setup(caption, uTimeout);
            return MessageBox.Show(owner, text, caption, buttons, icon);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton, uint uTimeout)
        {
            Setup(caption, uTimeout);
            return MessageBox.Show(owner, text, caption, buttons, icon, defButton);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton, MessageBoxOptions options, uint uTimeout)
        {
            Setup(caption, uTimeout);
            return MessageBox.Show(owner, text, caption, buttons, icon, defButton, options);
        }

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        public delegate void TimerProc(IntPtr hWnd, uint uMsg, UIntPtr nIDEvent, uint dwTime);

        public const int WH_CALLWNDPROCRET = 12;
        public const int WM_DESTROY = 0x0002;
        public const int WM_INITDIALOG = 0x0110;
        public const int WM_TIMER = 0x0113;
        public const int WM_USER = 0x400;
        public const int DM_GETDEFID = WM_USER + 0;

        [DllImport("User32.dll")]
        public static extern UIntPtr SetTimer(IntPtr hWnd, UIntPtr nIDEvent, uint uElapse, TimerProc lpTimerFunc);

        [DllImport("User32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll")]
        public static extern int UnhookWindowsHookEx(IntPtr idHook);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxLength);

        [DllImport("user32.dll")]
        public static extern int EndDialog(IntPtr hDlg, IntPtr nResult);

        [StructLayout(LayoutKind.Sequential)]
        public struct CWPRETSTRUCT
        {
            public IntPtr lResult;
            public IntPtr lParam;
            public IntPtr wParam;
            public uint message;
            public IntPtr hwnd;
        };

        private const int TimerID = 42;
        private static HookProc hookProc;
        private static TimerProc hookTimer;
        private static uint hookTimeout;
        private static string hookCaption;
        private static IntPtr hHook;

        static MessageBoxEx()
        {
            hookProc = new HookProc(MessageBoxHookProc);
            hookTimer = new TimerProc(MessageBoxTimerProc);
            hookTimeout = 0;
            hookCaption = null;
            hHook = IntPtr.Zero;
        }

        private static void Setup(string caption, uint uTimeout)
        {
            /*if (hHook != IntPtr.Zero)
                throw new NotSupportedException("multiple calls are not supported");*/

            hookTimeout = uTimeout;
            hookCaption = caption != null ? caption : "";
            hHook = SetWindowsHookEx(WH_CALLWNDPROCRET, hookProc, IntPtr.Zero, AppDomain.GetCurrentThreadId());
        }

        private static IntPtr MessageBoxHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
                return CallNextHookEx(hHook, nCode, wParam, lParam);

            CWPRETSTRUCT msg = (CWPRETSTRUCT)Marshal.PtrToStructure(lParam, typeof(CWPRETSTRUCT));
            IntPtr hook = hHook;

            if (hookCaption != null && msg.message == WM_INITDIALOG)
            {
                int nLength = GetWindowTextLength(msg.hwnd);
                StringBuilder text = new StringBuilder(nLength + 1);

                GetWindowText(msg.hwnd, text, text.Capacity);

                if (hookCaption == text.ToString())
                {
                    hookCaption = null;
                    SetTimer(msg.hwnd, (UIntPtr)TimerID, hookTimeout, hookTimer);
                    UnhookWindowsHookEx(hHook);
                    hHook = IntPtr.Zero;
                }
            }

            return CallNextHookEx(hook, nCode, wParam, lParam);
        }

        private static void MessageBoxTimerProc(IntPtr hWnd, uint uMsg, UIntPtr nIDEvent, uint dwTime)
        {
            if (nIDEvent == (UIntPtr)TimerID)
            {
                short dw = (short)SendMessage(hWnd, DM_GETDEFID, IntPtr.Zero, IntPtr.Zero);

                EndDialog(hWnd, (IntPtr)dw);
            }
        }
    }
}
