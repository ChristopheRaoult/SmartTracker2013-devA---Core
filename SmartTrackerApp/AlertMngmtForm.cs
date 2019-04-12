using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net.Sockets;
using System.Net;
using System.Net.Mail;

using DataClass;
using DBClass;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class AlertMngmtForm : Form
    {

        smtpInfo smtp = new smtpInfo();
        MainDBClass db = null;
        AlertType selAlert;
        alertInfo selAlertInfo = new alertInfo();
        DeviceInfo[] DeviceArray = null;
        Control _lastEnteredControl;


        public AlertMngmtForm()
        {
            InitializeComponent();
        }

        

        private void buttonTest_Click(object sender, EventArgs e)
        {          
            try
            {
                int port = int.Parse(textBoxPort.Text.Trim());
                if (SmtpHelper.TestConnection(textBoxSmtpServer.Text, port))
                {
                
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient(textBoxSmtpServer.Text);

                    mail.From = new MailAddress(textBoxSender.Text);
                    mail.To.Add(textBoxMailTO.Text);
                    mail.Subject = Properties.ResStrings.strAlertMailSubject;
                    mail.Body = Properties.ResStrings.strAlertMailBody;

                    SmtpServer.Port = port;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(textBoxLogin.Text, textBoxPwd.Text);
                    if (checkBoxSSL.Checked) SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                    MessageBox.Show(Properties.ResStrings.strAlertOk, Properties.ResStrings.strAlertSmtpInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }  
                else
                {
                    MessageBox.Show(Properties.ResStrings.strAlertFailed, Properties.ResStrings.strAlertSmtpInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(Properties.ResStrings.strAlertError + exp.Message, Properties.ResStrings.strAlertSmtpInfo, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AlertMngmtForm_Load(object sender, EventArgs e)
        {
            UpdateCTRL();
            updateReader();
            comboBoxAlertType.SelectedIndex = -1;
        }

        private void updateReader()
        {            
            int nbReader = 0;
            DeviceInfo[] tmpDeviceArrayLocal = db.RecoverDevice(true);
            DeviceInfo[] tmpDeviceArrayNet = db.RecoverDevice(false);
            if (tmpDeviceArrayLocal != null) nbReader += tmpDeviceArrayLocal.Length;
            if (tmpDeviceArrayNet != null) nbReader += tmpDeviceArrayNet.Length;        

            checkedListBox.Items.Clear();
            if (nbReader > 0)
            {
                DeviceArray = new DeviceInfo[nbReader];
                int nIndex = 0;
                if (tmpDeviceArrayLocal != null)
                {
                    foreach (DeviceInfo dv in tmpDeviceArrayLocal)
                    {
                        DeviceArray[nIndex++] = dv;
                        string strDevice = dv.DeviceName + " (" + dv.SerialRFID + " - " + getStringType(dv.deviceType) + ")";
                        checkedListBox.Items.Add(strDevice);
                    }
                }
                if (tmpDeviceArrayNet != null)
                {
                    foreach (DeviceInfo dv in tmpDeviceArrayNet)
                    {
                        DeviceArray[nIndex++] = dv;
                        string strDevice = dv.DeviceName + " (" + dv.SerialRFID + " - " + getStringType(dv.deviceType) + ")";
                        checkedListBox.Items.Add(strDevice);
                    }
                }
            }           

        }

        private string getStringType(DeviceType devType)
        {
            string strType = ResStrings.str_Unknown_Type;
            switch (devType)
            {
                case DeviceType.DT_DSB: strType = ResStrings.str_Diamond_Smart_Box; break;
                case DeviceType.DT_JSC: strType = ResStrings.str_Jewelry_Smart_Cabinet; break;
                case DeviceType.DT_SAS: strType = ResStrings.str_Diamond_SAS; break;
                case DeviceType.DT_MSR: strType = ResStrings.str_Smart_Drawer; break;
                case DeviceType.DT_SBR: strType = ResStrings.str_Smart_Board; break;
                case DeviceType.DT_SMC: strType = ResStrings.str_Medical_Cabinet_; break;
                case DeviceType.DT_SFR: strType = ResStrings.str_Smart_Fridge; break;
                case DeviceType.DT_STR: strType = ResStrings.str_Smart_Station; break;
                case DeviceType.DT_SBF: strType = ResStrings.str_Smart_Blood_Fridge; break;
            }
            return strType;
        }

        private void UpdateCTRL()
        {
             db = new MainDBClass();
             if (db.OpenDB())
             {
                 smtp = db.getSmtpInfo(false);
                 if (smtp != null)
                 {
                     textBoxSmtpServer.Text = smtp.smtp;
                     textBoxPort.Text = smtp.port.ToString();
                     textBoxSender.Text = smtp.sender;
                     textBoxLogin.Text = smtp.login ;
                     textBoxPwd.Text =  smtp.pwd ;
                     checkBoxSSL.Checked = smtp.bUseSSL;
                     checkBoxSMTPActive.Checked = smtp.bActive;
                 }
             }
        }

        private void ResetCTRL()
        {            
            textBoxSmtpServer.Text = null;
            textBoxPort.Text = null;
            textBoxSender.Text = null;
            textBoxLogin.Text = null;
            textBoxPwd.Text = null;
            checkBoxSSL.Checked = false;
            checkBoxSMTPActive.Checked = false; 
        }

        private void AlertMngmtForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (db.isOpen()) db.CloseDB();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ResetCTRL();
        }

        private void button2_Click(object sender, EventArgs e)
        {
             try
            {               
                db.DeleteSMTP();
                ResetCTRL();
            }
             catch (Exception exp)
             {
                 ErrorMessage.ExceptionMessageBox.Show(exp);
             }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                smtp = new smtpInfo();
                smtp.smtp = textBoxSmtpServer.Text;
                smtp.port = Convert.ToInt32(textBoxPort.Text);
                smtp.sender = textBoxSender.Text;
                smtp.login =  textBoxLogin.Text;
                smtp.pwd = textBoxPwd.Text;
                smtp.bUseSSL =  checkBoxSSL.Checked ;
                smtp.bActive = checkBoxSMTPActive.Checked;
                db.DeleteSMTP();
                db.AddSmtp(smtp);
                MessageBox.Show(Properties.ResStrings.strAlertSmtpSaved, Properties.ResStrings.strAlertSmtpInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exp)
            {
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
        }

        private void comboBoxAlertType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxAlertType.SelectedIndex >= 0)
            {
                selAlert = (AlertType)comboBoxAlertType.SelectedIndex;
                switch (selAlert)
                {
                    case AlertType.AT_Max_Fridge_Temp:
                        labelAlert.Visible = true;
                        labelAlert.Text = Properties.ResStrings.strAlertLblTemp;
                        textBoxAlert.Visible = true;
                        textBoxAlert.Text = null;
                        break;
                    case AlertType.AT_Door_Open_Too_Long:
                          labelAlert.Visible = true;
                          labelAlert.Text = Properties.ResStrings.strAlertLblTime;
                        textBoxAlert.Visible = true;
                        textBoxAlert.Text = null;
                        break;

                    case AlertType.AT_Remove_Tag_Max_Time:
                        labelAlert.Visible = true;
                        labelAlert.Text = Properties.ResStrings.strAlertLblTime2;
                        textBoxAlert.Visible = true;
                        textBoxAlert.Text = null;
                        checkedListBox.Visible = true;
                        break;
                    case AlertType.AT_Stock_Limit:
                        labelAlert.Visible = true;
                        labelAlert.Text = Properties.ResStrings.strAlertLblStock;
                        textBoxAlert.Visible = true;
                        textBoxAlert.Text = null;
                        break;                    

                    default:
                        labelAlert.Visible = false;
                        textBoxAlert.Visible = false;
                        textBoxAlert.Text = null;
                        checkedListBox.Visible = false;
                        break;
                }
                getAlert( selAlert);
            }
        }

        private void getAlert(AlertType selAlert)
        {
            try
            {
                selAlertInfo = db.getAlertInfo(selAlert, false);
                if (selAlertInfo != null)
                {
                    textBoxMailToList.Text = selAlertInfo.RecipientList;
                    textBoxCCList.Text = selAlertInfo.CCRecipientList;
                    textBoxBCCList.Text = selAlertInfo.BCCRecipientList;
                    textBoxMailSubject.Text = selAlertInfo.MailSubject;
                    richTextBox.Text = selAlertInfo.AlertMessage;
                    checkBoxAlertActive.Checked = selAlertInfo.bActive;
                    switch (selAlertInfo.type)
                    {
                        case  AlertType.AT_Door_Open_Too_Long:
                        case AlertType.AT_Max_Fridge_Temp:
                        case AlertType.AT_Stock_Limit:
                            textBoxAlert.Text = selAlertInfo.alertData;
                            break;
                        case AlertType.AT_Remove_Tag_Max_Time:
                            if (selAlertInfo.alertData.Contains(';'))
                            {
                                string[] data = selAlertInfo.alertData.Split(';');
                                textBoxAlert.Text = data[0];                              
                                checkedListBox.ClearSelected();
                                if (data.Length > 2)
                                    for (int i = 1; i < data.Length; i++)
                                    {
                                        for (int loop = 0; loop < DeviceArray.Length; loop++)
                                        {
                                            if (DeviceArray[loop].SerialRFID == data[i])
                                            {
                                                checkedListBox.SetItemChecked(loop, true); ;
                                                break;
                                            }
                                        }
                                    }
                                
                            }
                            break;
                        default :
                            textBoxAlert.Text = null;
                            break;
                    }
                }
                else
                {
                    textBoxMailToList.Text = null;
                    textBoxCCList.Text = null;
                    textBoxBCCList.Text = null;
                    textBoxMailSubject.Text = null;
                    richTextBox.Text = null;
                    checkBoxAlertActive.Checked = false;
                    textBoxAlert.Text = null;
                }
              
            }
            catch (Exception exp)
            {
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
        }

        private void buttonTestAlert_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxAlertType.SelectedIndex >= 0)
                {
                    selAlert = (AlertType)comboBoxAlertType.SelectedIndex;
                    selAlertInfo = new alertInfo();
                    selAlertInfo.AlertName = selAlert.ToString();
                    selAlertInfo.RecipientList = textBoxMailToList.Text;
                    selAlertInfo.CCRecipientList = textBoxCCList.Text;
                    selAlertInfo.BCCRecipientList = textBoxBCCList.Text;
                    selAlertInfo.MailSubject = textBoxMailSubject.Text;
                    selAlertInfo.AlertMessage = richTextBox.Text;
                    selAlertInfo.bActive = checkBoxAlertActive.Checked;
                    if (selAlert == AlertType.AT_Remove_Tag_Max_Time)
                    {
                        if (checkedListBox.SelectedIndices.Count > 0)
                        {                          
                            selAlertInfo.alertData = textBoxAlert.Text;
                            for (int i = 0; i < checkedListBox.SelectedIndices.Count; i++)
                                selAlertInfo.alertData += ";" + DeviceArray[checkedListBox.SelectedIndices[i]].SerialRFID;
                        }
                    }
                    else
                    {
                        selAlertInfo.alertData = textBoxAlert.Text;
                    }

                    
                    createAndSendMail(selAlertInfo, smtp);
                }
            }
            catch (Exception exp)
            {
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
         
        }
        private  void createAndSendMail(alertInfo alert, smtpInfo smtp)
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

            string subject = alert.MailSubject;
            subject = subject.Replace("\n", "<br/>");
            mail.Subject = subject;

            string body = alert.AlertMessage;
            body = body.Replace("\n", "<br/>");
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
                MessageBox.Show(Properties.ResStrings.strAlertTestOk, Properties.ResStrings.stralertTestInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }      

        }

        private void button4_Click(object sender, EventArgs e)
        {
            addTxt("[READERNAME]");
        }

        private void textBoxMailSubject_Enter(object sender, EventArgs e)
        {
            _lastEnteredControl = (Control)sender;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            addTxt("[READERSERIAL]");
        }       

        private void button6_Click(object sender, EventArgs e)
        {
            addTxt("[USERNAME]");
        }
        private void button7_Click(object sender, EventArgs e)
        {
            addTxt("[DATE]");
        }
        private void addTxt(string str)
        {
            if (_lastEnteredControl == richTextBox)
            {
                int pos = richTextBox.SelectionStart;
                string textTOinsert = str;
                string newText = richTextBox.Text.Substring(0, pos) + textTOinsert + richTextBox.Text.Substring(pos);
                richTextBox.Text = newText;
                richTextBox.Select(pos + textTOinsert.Length, 0);
            }

            if (_lastEnteredControl == textBoxMailSubject)
            {
                int pos = textBoxMailSubject.SelectionStart;
                string textTOinsert = str;
                string newText = textBoxMailSubject.Text.Substring(0, pos) + textTOinsert + textBoxMailSubject.Text.Substring(pos);
                textBoxMailSubject.Text = newText;
                textBoxMailSubject.Select(pos + textTOinsert.Length, 0);
            }
            _lastEnteredControl.Focus();
        }

        private void buttonSaveAlert_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxAlertType.SelectedIndex >= 0)
                {
                    selAlert = (AlertType)comboBoxAlertType.SelectedIndex;
                    selAlertInfo = new alertInfo();
                    selAlertInfo.AlertName = selAlert.ToString();
                    selAlertInfo.RecipientList = textBoxMailToList.Text;
                    selAlertInfo.CCRecipientList =  textBoxCCList.Text;
                    selAlertInfo.BCCRecipientList =  textBoxBCCList.Text;
                    selAlertInfo.MailSubject = textBoxMailSubject.Text;
                    selAlertInfo.AlertMessage =  richTextBox.Text ;
                    selAlertInfo.bActive = checkBoxAlertActive.Checked;
                    if (selAlert == AlertType.AT_Remove_Tag_Max_Time)
                    {
                        if (checkedListBox.CheckedIndices.Count > 0)
                        {                          
                            selAlertInfo.alertData = textBoxAlert.Text;
                            for (int i = 0; i < checkedListBox.CheckedIndices.Count; i++)
                                selAlertInfo.alertData += ";" + DeviceArray[checkedListBox.CheckedIndices[i]].SerialRFID;
                        }
                    }
                    else
                    {
                        selAlertInfo.alertData = textBoxAlert.Text;
                    }

                    db.DeleteAlert(selAlertInfo);
                    db.AddAlertInfo(selAlertInfo);
                    MessageBox.Show(ResStrings.strAlertSaved, ResStrings.strAlertManagmentInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception exp)
            {
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
        }

        private void toolStripButtonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

       

    }
    public static class SmtpHelper
    {
        

        /// <summary>
        /// test the smtp connection by sending a HELO command
        /// </summary>
        /// <param name="smtpServerAddress"></param>
        /// <param name="port"></param>
        public static bool TestConnection(string smtpServerAddress, int port)
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(smtpServerAddress);
            IPEndPoint endPoint = new IPEndPoint(hostEntry.AddressList[0], port);
            using (Socket tcpSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                //try to connect and test the rsponse for code 220 = success
                tcpSocket.Connect(endPoint);
                if (!CheckResponse(tcpSocket, 220))
                {
                    return false;
                }

                // send HELO and test the response for code 250 = proper response
                SendData(tcpSocket, string.Format("HELO {0}\r\n", Dns.GetHostName()));
                if (!CheckResponse(tcpSocket, 250))
                {
                    return false;
                }

                // if we got here it's that we can connect to the smtp server
                return true;
            }
        }

        private static void SendData(Socket socket, string data)
        {
            byte[] dataArray = Encoding.ASCII.GetBytes(data);
            socket.Send(dataArray, 0, dataArray.Length, SocketFlags.None);
        }

        private static bool CheckResponse(Socket socket, int expectedCode)
        {
            while (socket.Available == 0)
            {
                System.Threading.Thread.Sleep(100);
            }
            byte[] responseArray = new byte[1024];
            socket.Receive(responseArray, 0, socket.Available, SocketFlags.None);
            string responseData = Encoding.ASCII.GetString(responseArray);
            int responseCode = Convert.ToInt32(responseData.Substring(0, 3));
            if (responseCode == expectedCode)
            {
                return true;
            }
            return false;
        }
    }

}
