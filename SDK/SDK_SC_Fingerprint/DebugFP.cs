using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;

namespace SDK_SC_Fingerprint
{
    public partial class DebugFP : Form
    {

        private int timeout;
        private bool sendMail;
        private Bitmap BmpFP;
        private string reader;
        private DateTime TimeFingerTouch;
        private DateTime TimeFingerGone;

        private string debugFirstName;
        private string debugLastName;
        private FingerIndexValue debugFinger;
        private int FarAchieved;
        private int FarThreshold;

        private DPFP.Capture.CaptureFeedback debugFeedback;

        private string pathImage;       
        private bool FormVisible;
        private bool bIsProcessed;

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

        public DebugFP(                       
                        bool FormVisible,
                        int timeout , 
                        bool sendMail ,
                        Bitmap BmpFP,
                        string reader,
                        DateTime TimeFingerTouch,
                        DateTime TimeFingerGone,
                        string debugFirstName,
                        string debugLastName,
                        FingerIndexValue debugFinger,
                        int FarAchieved,
                        int FarThreshold,
                        DPFP.Capture.CaptureFeedback debugFeedback,
                        string pathImage,
                        bool bIsProcessed,
                        string SenderAdress,
                        string LoginName,
                        string Password,
                        string SMTPServer, 
                        int SMTPPort
                        )
        {
            InitializeComponent();

            this.FormVisible = FormVisible;
            this.timeout = timeout;
            this.sendMail = sendMail;
            timer1.Interval = timeout * 1000;

            this.BmpFP = BmpFP;
            if ( this.BmpFP != null)
                 pictureBoxFP.Image = new Bitmap(this.BmpFP, pictureBoxFP.Size);
                //pictureBoxFP.Image = cleanBitmap(this.BmpFP);

            this.SenderAdress = SenderAdress;
            this.LoginName = LoginName;
            this.Password = Password;
            this.SMTPServer = SMTPServer;
            this.SMTPPort = SMTPPort;


            this.reader = reader;
            labelReader.Text = "Reader S/N : " + this.reader;

            this.TimeFingerGone = TimeFingerGone;
            this.TimeFingerTouch = TimeFingerTouch;

            labelTouch.Text = "Touch : " + TimeFingerTouch.ToString("G") + TimeFingerTouch.ToString(":fff");
            labelGone.Text = "Gone :" + TimeFingerGone.ToString("G") + TimeFingerGone.ToString(":fff");;
            TimeSpan ts = TimeFingerGone - TimeFingerTouch;
            labelElapsed.Text = "Time Sensor :" + ts.TotalSeconds.ToString("0.000") + " sec";
            
            this.debugFirstName = debugFirstName;
            this.debugLastName = debugLastName;
            this.debugFinger = debugFinger;

            labelFirsname.Text = "First Name : " + debugFirstName;
            labelLastName.Text = "Last Name : " + debugLastName;
            labelFinger.Text = "Finger Used : " + debugFinger.ToString();

            this.FarAchieved = FarAchieved;
            this.FarThreshold = FarThreshold;
            labelFAR.Text = "FAR : " + FarAchieved.ToString() + " / " + FarThreshold.ToString();

            this.debugFeedback = debugFeedback;
            labelQuality.Text = "Quality : " + debugFeedback.ToString();

            this.pathImage = pathImage;

            this.bIsProcessed = bIsProcessed;
            if (bIsProcessed)
            {                
                labelCapture.Text = "Capture Status : OK";               
            }
            else
            {                
                labelCapture.Text = "Capture Status : not OK";                
            }

            if (!FormVisible)
            {
                this.Opacity = 0.0f;
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }
            else
                this.BackColor = Color.WhiteSmoke;
           
        }

        private Bitmap cleanBitmap(Bitmap BmpToClean)
        {

            int width;
            int height;
            Color color;
            Bitmap bmp = new Bitmap(BmpToClean, pictureBoxFP.Size);

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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (sendMail) sendMailAlert();
            Close();
        }


        private void sendMailAlert()
        {

            if (string.IsNullOrEmpty(SenderAdress)) return;
            if (string.IsNullOrEmpty(LoginName)) return;
            if (string.IsNullOrEmpty(Password)) return;
            if (string.IsNullOrEmpty(SMTPServer)) return;
            if (SMTPPort == 0) return;
            try
            {
                //create the mail message
                MailMessage mail = new MailMessage();

                //set the addresses
                mail.From = new MailAddress(SenderAdress);               

                mail.To.Add("christophe.raoult@spacecode-rfid.com");
                mail.To.Add("eric.gout@spacecode-rfid.com");

                //set the content
                mail.Subject = "FingerPrint Debug " + labelReader.Text;

                string mailbody = string.Empty;

                mailbody += labelReader.Text + "\r\n\r\n<br />";
                mailbody += labelTouch.Text + "\r\n\r\n<br />";
                mailbody += labelGone.Text + "\r\n\r\n<br />";
                mailbody += labelElapsed.Text + "\r\n\r\n<br />";
                mailbody += labelFirsname.Text + "\r\n\r\n<br />";
                mailbody += labelLastName.Text + "\r\n\r\n<br />";
                mailbody += labelFinger.Text + "\r\n\r\n<br />";
                mailbody += labelFAR.Text + "\r\n\r\n<br />";
                mailbody += labelQuality.Text + "\r\n\r\n<br />";
                mailbody += labelCapture.Text + "\r\n\r\n<br />";

                //first we create the Plain Text part
                AlternateView plainView = AlternateView.CreateAlternateViewFromString(mailbody, null, "text/plain");

                //then we create the Html part
                //to embed images, we need to use the prefix 'cid' in the img src value
                //the cid value will map to the Content-Id of a Linked resource.
                //thus <img src='cid:companylogo'> will map to a LinkedResource with a ContentId of 'companylogo'
                AlternateView htmlView;
                if (string.IsNullOrEmpty(pathImage))
                {
                    htmlView = AlternateView.CreateAlternateViewFromString(mailbody + "<br />", null, "text/html");
                }
                else
                {
                    htmlView = AlternateView.CreateAlternateViewFromString(mailbody + "<br /><img src=cid:FPbitmap>", null, "text/html");

                    //create the LinkedResource (embedded image)
                    LinkedResource logo = new LinkedResource(pathImage);
                    logo.ContentId = "FPbitmap";
                    //add the LinkedResource to the appropriate view
                    htmlView.LinkedResources.Add(logo);
                }

                //add the views
                mail.AlternateViews.Add(plainView);
                mail.AlternateViews.Add(htmlView);


                //send the message
                SmtpClient SmtpServer = new SmtpClient(SMTPServer);
                SmtpServer.Port = SMTPPort;
                SmtpServer.Credentials = new System.Net.NetworkCredential(LoginName, Password);
                
                //Valimpex
               /* SmtpClient SmtpServer = new SmtpClient("valimpex02.valimpex");
                SmtpServer.Port = 25;
                SmtpServer.Credentials = new System.Net.NetworkCredential("valimpex\\alert", "Sp@cec0de");
                */
                SmtpServer.Send(mail);
            }
            catch
            {
               
            }
            }
        }
    
}
