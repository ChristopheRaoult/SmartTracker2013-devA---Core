using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Collections;
using LogicNP.CryptoLicensing;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class MngLicense : Form
    {
        CryptoLicense lic;
        readonly string _validationKey;
        public MngLicense(CryptoLicense lic, string validationKey)
        {
            InitializeComponent();
            this.lic = lic;
            _validationKey = validationKey;
        }

        private void mngLicense_Load(object sender, EventArgs e)
        {
          
        }

        void UpdateLicence()
        {
            //ThreadPool.QueueUserWorkItem(new WaitCallback(treatMachineID), null); 
            TreatMachineId();

            labelLicenceStatus.Text = lic.Status.ToString();
            switch (lic.Status)
            {
                case LicenseStatus.DateRollbackDetected:
                case LicenseStatus.Expired:
                case LicenseStatus.EvaluationExpired:
                    labelLicenceStatus.ForeColor = Color.Red;
                    buttonCont.Enabled = false;
                    break;
                case LicenseStatus.Valid:
                    if (lic.IsEvaluationLicense())
                    {
                        labelLicenceStatus.Text = ResStrings.str_Evaluation_Period;
                        labelLicenceStatus.ForeColor = Color.DarkOrange;


                        if (lic.RemainingExecutions < 100)
                        {
                            labelRun.Visible = true;
                            labelRun.Text = string.Format(ResStrings.str_runs_remaining, lic.RemainingExecutions, lic.MaxExecutions);
                            progressBarRun.Visible = true;
                            progressBarRun.Maximum = lic.MaxExecutions;
                            progressBarRun.Value = lic.RemainingExecutions;
                        }
                      
                        labelDays.Visible = true;                       
                        labelDays.Text = string.Format(ResStrings.str_days_remaining, lic.RemainingUsageDays, lic.MaxUsageDays);

                        progressBarDays.Visible = true; 
                        progressBarDays.Maximum = lic.MaxUsageDays;
                        progressBarDays.Value = lic.RemainingUsageDays;

                        buttonCont.Enabled = true;
                        buttonCont.Text = ResStrings.str_Continue_Evaluation;


                    }
                    else
                    {
                        labelLicenceStatus.ForeColor = Color.Green;
                        buttonCont.Enabled = true;
                        buttonCont.Text = ResStrings.str_Continue;
                    }
                    break;                  

            }

            if (lic.HasUserData)
            {
                 string userName = null;
                 string company = null;
                 try
                 {
                     Hashtable dataFields = lic.ParseUserData("#");
                     company = dataFields["Company"] as string;
                     userName = dataFields["Name"] as string;
                 }
                 catch
                 {

                 }
                 labelCompany.Text = company;
                 labelUser.Text = userName;

            }


            if (lic.HasDateExpires)
            {
                TimeSpan remain = lic.DateExpires.Subtract(DateTime.Now);
                labelExpirationDate.Text = lic.DateExpires.ToShortDateString();
                labelDaysLeft.Text = remain.Days.ToString(CultureInfo.InvariantCulture);

                if (remain.Days < 15)
                {
                    labelExpirationDate.ForeColor = Color.DarkOrange;
                    labelDaysLeft.ForeColor = Color.DarkOrange;
                }
                if (remain.Days < 5)
                {
                    labelExpirationDate.ForeColor = Color.Red;
                    labelDaysLeft.ForeColor = Color.Red;
                }
            }
            
        }
        private void TreatMachineId()
        {
            Invoke((MethodInvoker)delegate {
                try
                {
                    lblMachineName.Text = System.Net.Dns.GetHostName();
                    labelMachineID.Text = getMachineID();
                }
                catch
                {
                }
            });
        }

        string getMachineID()
        {
            string sebBaseString = Encryption.Boring(Encryption.InverseByBase(SystemInfo.GetSystemInfo("TestCrypto"), 10)).Substring(0, 25).ToUpper();
            string machineId = sebBaseString.Substring(0, 5);
            machineId += "-" + sebBaseString.Substring(5, 5);
            machineId += "-" + sebBaseString.Substring(10, 5);
            machineId += "-" + sebBaseString.Substring(15, 5);
            machineId += "-" + sebBaseString.Substring(20, 5);
            return machineId;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string command = "mailto:licencing@spacecode.com?subject=Request SmartTracker 2014 Licence ";
            command += "&body=Please fill the following information %0A%0A";
            command += "User Name : %0A";
            command += "Company Name : %0A";          
            try
            {
                command += "Machine Name : " + System.Net.Dns.GetHostName() + "%0A";               
            }
            catch
            {
            }
            command += "Machine ID : " + labelMachineID.Text;
            command += "%0ASpacecode Commercial Contact Name : %0A";
            command += "%0A%0A Thanks for using SmartTracker - Report any bugs at support@spacecode.com %0A%0A SPACECODE TEAM";
            System.Diagnostics.Process.Start(command); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Set output to clipboard.
            Clipboard.SetDataObject(labelMachineID.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool bLicOk = false;
            if (string.IsNullOrEmpty(textBoxLic.Text))
            {
                MessageBox.Show(ResStrings.str_Enter_a_License, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            CryptoLicense testLic = new CryptoLicense();
            testLic.ValidationKey = _validationKey;
            testLic.StorageMode = LicenseStorageMode.ToRegistry;
            testLic.LicenseCode = textBoxLic.Text;

            if (testLic.HasUserData)
            {
                Hashtable dataFields = testLic.ParseUserData("#");
                string licenseMachineId = dataFields["MachineID"] as string;
                string machineId = labelMachineID.Text;
                bLicOk = machineId.Equals(licenseMachineId);
            }

            if (bLicOk)
            {               
                testLic.Save();
                lic.Load();
                UpdateLicence();
                MessageBox.Show(ResStrings.str_License_Info_Saved, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                textBoxLic.Text = string.Empty;
                MessageBox.Show(ResStrings.str_Not_a_valid_license_for_this_machine, ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            testLic.Dispose();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            UpdateLicence();
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }

        private void textBoxLic_KeyDown(object sender, KeyEventArgs e)
        {
            const string strLic = "SPACECODE@123456";
            if (e.KeyCode == Keys.L && e.Modifiers == Keys.Control)
            {
                if (textBoxLic.Text != strLic) return;
                lic.Remove();
                lic.ResetCustomInfo();
                lic.ResetEvaluationInfo();
                MessageBox.Show(ResStrings.str_License_removed,ResStrings.strInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }
}
