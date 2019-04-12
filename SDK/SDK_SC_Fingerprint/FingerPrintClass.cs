using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;



namespace SDK_SC_Fingerprint
{
    public delegate void NotifyHandlerDelegate(Object sender, FingerArgs arg);
    public class FingerPrintClass : IFingerPrintClass
    {
        private VerifyFinger myFinger;
        public event NotifyHandlerDelegate NotifyEvent;

        private bool debugFP = false;
        private bool debugFPFormVisible = false;
        private int debugWindowTimeout = 5;
        private bool sendDebugMail = true;
        private bool debugOnFailedOnly = false;

        public bool DebugFP 
        { 
            set 
            { 
                debugFP = value;
               if (myFinger != null) updateProperty();
            } 
            get {  return debugFP; } 
        }
        public bool DebugFPFormVisible 
        { 
            set 
            { 
                debugFPFormVisible = value;
                if (myFinger != null) updateProperty();
            }
            get { return debugFPFormVisible; }
        }
        public bool SendDebugMail 
        { 
            set 
            {
                sendDebugMail = value;
                if (myFinger != null) updateProperty();
            }
            get { return sendDebugMail; }
        }
        public int DebugWindowTimeout 
        { 
            set 
            {
                debugWindowTimeout = value;
                 if (myFinger != null) updateProperty();
            }
            get { return debugWindowTimeout; }
        }
        public bool DebugOnFailureOnly 
        { 
            set { 
                debugOnFailedOnly = value;
                if (myFinger != null) updateProperty();
            }
            get { return debugOnFailedOnly; }
        }

        public string serialFPTouched
        {
            get
            {
                if (myFinger != null)
                    return myFinger.getSerialFingerTouched;
                else
                    return null;
            }
        }


        private TextBox textBox;
        public TextBox TextBox { get { return textBox; } set { textBox = value; } }

        public FingerPrintClass()
        {
            myFinger = null;
            textBox = null;           
        }

        public string[] GetPluggedFingerprint()
        {
            DPFP.Capture.ReadersCollection reader = new DPFP.Capture.ReadersCollection();
            string[] tmpReader = new string[reader.Count];
            for (int loop = 0; loop < reader.Count; loop++)
            {
                DPFP.Capture.ReaderDescription rd = reader[loop];
                tmpReader[loop] = rd.SerialNumber;
            }

            return tmpReader;
        }

        public void InitFingerPrint(string ReaderSerialNumber, bool bUseEmbedDB)
        {
            if (myFinger != null)
            myFinger.SetTextBox("Request InitFingerprint ", DateTime.Now);
            string serialFinger = null;
#if UseSQLITE
                if (ReaderSerialNumber != null)
                {
                    DBClassFinger dbJob = new DBClassFinger();
                    serialFinger = dbJob.GetSerialNumberFinger(ReaderSerialNumber);
                }
#endif
       myFinger = new VerifyFinger(NotifyEvent, textBox, ReaderSerialNumber, serialFinger, bUseEmbedDB);
       updateProperty();

        }

        public void updateProperty()
        {
            myFinger.debugFP = debugFP;
            myFinger.debugFPFormVisible = debugFPFormVisible;
            myFinger.debugWindowTimeout = debugWindowTimeout;
            myFinger.sendDebugMail = sendDebugMail;
            myFinger.debugOnFailedOnly = debugOnFailedOnly;
        }
        private Regex isGuid = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled);
        private bool IsGuid(string candidate, out Guid output)
        {
            bool isValid = false;
            output = Guid.Empty;
            if (candidate != null)
            {

                if (isGuid.IsMatch(candidate))
                {
                    output = new Guid(candidate);
                    isValid = true;
                }
            }
            return isValid;
        }
        public bool InitFingerPrint(string ReaderSerialNumber, string FingerSerialNumber)
        {
            if (myFinger != null)
            myFinger.SetTextBox("Request InitFingerprint ", DateTime.Now);
            string fingerSerial = null;

            if (FingerSerialNumber != null)
            {
                if (!FingerSerialNumber.Contains("{"))
                    fingerSerial = "{" + FingerSerialNumber;
                else
                    fingerSerial = FingerSerialNumber;

                if (!FingerSerialNumber.Contains("}"))
                    fingerSerial += "}";

                Guid guid;
                if (!IsGuid(fingerSerial, out guid)) return false;
            }

            if (myFinger != null) ReleaseFingerprint();
            
            myFinger = new VerifyFinger(NotifyEvent, textBox, ReaderSerialNumber, fingerSerial, false);

            myFinger.debugFP = debugFP;
            myFinger.debugFPFormVisible = debugFPFormVisible;
            myFinger.debugWindowTimeout = debugWindowTimeout;
            myFinger.sendDebugMail = sendDebugMail;
            myFinger.debugOnFailedOnly = debugOnFailedOnly;
            return true;
        }
        public void ReleaseFingerprint()
        {

            if (myFinger != null)
            {
                myFinger.SetTextBox("Release FingerPrint", DateTime.Now);
                myFinger.Dispose();

            }

        }
       /* public void LoadFingerprintTemplates(string[] templates)
        {
            if (myFinger != null)
            {
                myFinger.UserList.Clear();
                foreach (string data in templates)
                {
                    if (data != null)
                    {
                        try
                        {
                            UserClass TheUser = new UserClass();
                            BinaryFormatter bf = new BinaryFormatter();
                            MemoryStream mem = new MemoryStream(Convert.FromBase64String(data));
                            TheUser = (UserClass)bf.Deserialize(mem);
                            myFinger.UserList.Add(TheUser);
                        }
                        catch (Exception exp)
                        {
                            string err = "Error during Deserialization : " + exp.Message;
                            myFinger.SetTextBox(err);
                        }
                    }
                }
            }
        }*/

        public void ClearFingerprintTemplates()
        {
            if (myFinger != null)
            myFinger.UserList.Clear();
        }

        public int LoadFingerprintTemplates(string[] templates)
        {
            if (myFinger == null) return -1;

            myFinger.SetTextBox("Request LoadFingerPrint :" + templates.Length, DateTime.Now);
            myFinger.UserList.Clear();
            foreach (string data in templates)
            {
                if (data != null)
                {
                    try
                    {
                        UserClass TheUser = new UserClass();
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream mem = new MemoryStream(Convert.FromBase64String(data));
                        TheUser = (UserClass)bf.Deserialize(mem);
                        myFinger.UserList.Add(TheUser);                           
                             
                        int enrolledFinger = 0;
                        for (int j = 0; j < TheUser.strFingerprint.Length; j++)
                            if (TheUser.strFingerprint[j] != null) enrolledFinger++;

                        myFinger.SetTextBox("Find :" +
                                            TheUser.firstName + " " + TheUser.lastName +
                                            " with " + enrolledFinger.ToString() + " enrolled finger(s) ", DateTime.Now);

                        for (int i = 0; i < TheUser.strFingerprint.Length ; i++)
                        {
                            if (TheUser.strFingerprint[i] != null)
                            {
                                FingerIndexValue fiv = (FingerIndexValue)i;
                                myFinger.SetTextBox("Load :" +
                                                    TheUser.firstName + " " +
                                                    TheUser.lastName + " : " +
                                                    fiv.ToString(),DateTime.Now);    
                            }

                        }
                           
                    }
                    catch (Exception exp)
                    {
                        string err = "Error during Deserialization : " + exp.Message;
                        myFinger.SetTextBox(err + "\r\nFailed template : " + data, DateTime.Now);
                            
                    }
                }

                else
                    myFinger.SetTextBox("try to Load null template", DateTime.Now);

            }

            myFinger.SetTextBox(myFinger.UserList.Count + " Templates loaded", DateTime.Now);
            return myFinger.UserList.Count;
        }


        public string EnrollUser(string FPSerialNumber, string FirstName, string LastName, string template, bool bNotEnroll)
        {

            if (myFinger != null)
                ReleaseFingerprint();
          //   myFinger.SetTextBox("Request Enroll User :", DateTime.Now);
            string fingerSerial = null;            

            if (FPSerialNumber != null)
            {
                if (!FPSerialNumber.Contains("{"))
                    fingerSerial = "{" + FPSerialNumber;
                else
                    fingerSerial = FPSerialNumber;

                if (!FPSerialNumber.Contains("}"))
                    fingerSerial += "}";

                Guid guid;
                if (!IsGuid(fingerSerial, out guid)) return null;
            }

            FingerData newfinger = new FingerData();
            UserClass TheUser = new UserClass();

            if (!String.IsNullOrEmpty(template))
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream mem = new MemoryStream(Convert.FromBase64String(template));
                TheUser = (UserClass)bf.Deserialize(mem);
            }
            else
            {
                TheUser.firstName = FirstName;
                TheUser.lastName = LastName;
            }

            if (!bNotEnroll)                            
            {
                newfinger.CopyUserToFinger(TheUser);
                EnrollmentProcess Enroller = new EnrollmentProcess(fingerSerial, newfinger);
                Enroller.ShowDialog();	// process enrollment
                newfinger.CopyFingerToUser(TheUser);
           
            }
            
            BinaryFormatter bf2 = new BinaryFormatter();
            MemoryStream mem2 = new MemoryStream();
            bf2.Serialize(mem2, TheUser);
            return (Convert.ToBase64String(mem2.ToArray()));

        }
        /// <summary>
        /// Function to test if a finger is enroll in a binary string
        /// </summary>
        /// <param name="template">template under test</param>
        /// <param name="fingerToTest">finger to test</param>
        /// <returns>-1 if error , 0 if finger not enroll otherwise 1</returns>

        public int IsFingerEnrolled(string template, FingerIndexValue fingerToTest)
        {
            try
            {

                UserClass TheUser = new UserClass();
                if (template != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream mem = new MemoryStream(Convert.FromBase64String(template));
                    TheUser = (UserClass)bf.Deserialize(mem);

                    if (TheUser.strFingerprint[(int)fingerToTest] != null)
                        return 1;
                    else
                        return 0;
                }
                else
                    return -1;

            }
            catch 
            {
                return -1;
            }
        }

       public void renewFP(bool bFullRenew)
       {
           if (myFinger != null)
               myFinger.renewFP(bFullRenew);
       }
    }    
}
