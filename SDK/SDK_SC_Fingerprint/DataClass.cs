using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

//Fingerprint Definition
namespace SDK_SC_Fingerprint
{  
   
     public enum FingerprintMode
     {
          Enroll = 0x01,
          Verify = 0x02,
     }

     public enum FingerIndexValue
     {
            Right_Thumb = 0x00,
            Right_Index = 0x01,
            Right_Middle = 0x02,
            Right_Ring = 0x03,
            Right_Little = 0x04,
            Left_Thumb = 0x05,
            Left_Index = 0x06,
            Left_Middle = 0x07,
            Left_Ring = 0x08,
            Left_Little = 0x09,
            Unknown_Finger = 0xA,
     }
    

    [Serializable]
    public class UserClass
    {
        public const int MaxFingers = 10;
        public string firstName;
        public string lastName;
        public string[] strFingerprint = new string[MaxFingers];
        public int EnrolledFingersMask = 0;

    }
    public delegate void OnChangeHandler();
    public class FingerData
    {
        public const int MaxFingers = 10;
        // shared data
        public int EnrolledFingersMask = 0;
        public int MaxEnrollFingerCount = MaxFingers;
        public bool IsEventHandlerSucceeds = true;
        public bool IsFeatureSetMatched = false;
        public int FalseAcceptRate = 0;
        public DPFP.Template[] Templates = new DPFP.Template[MaxFingers];

        // data change notification
        public void Update() { OnChange(); }		// just fires the OnChange() event
        public event OnChangeHandler OnChange;

        //shared function
        public void CopyFingerToUser(UserClass User)
        {
            int loop = 0;
            User.EnrolledFingersMask = EnrolledFingersMask;
            foreach (DPFP.Template template in Templates)
            {
                // Get template from storage.
                if (template != null)
                {
                    System.IO.MemoryStream mem = new System.IO.MemoryStream();
                    template.Serialize(mem);
                    User.strFingerprint[loop++] = Convert.ToBase64String(mem.ToArray());
                }
                else
                    User.strFingerprint[loop++] = null;
            }
        }
        public void CopyUserToFinger(UserClass User)
        {
            int loop = 0;
            EnrolledFingersMask = User.EnrolledFingersMask;
            foreach (string str in User.strFingerprint)
            {
                if (str != null)
                {
                    System.IO.MemoryStream mem = new System.IO.MemoryStream(Convert.FromBase64String(str));
                    Templates[loop++] = new DPFP.Template(mem);

                }
                else
                    Templates[loop++] = null;
            }
        }

        public byte[] GetFingerTemplate( int fingerIndex)
        {
            return Templates[fingerIndex].Bytes;
        }
    }
    public class FingerArgs : System.EventArgs
    {
        /// <summary>
        /// Enumeration of possible notification
        /// </summary>
        public enum FingerNotify
        {

            RN_FingerTouch = 0x30,  //48
            RN_FingerGone = 0x31,
            RN_FingerprintConnect = 0x32,
            RN_FingerprintDisconnect = 0x33,
            RN_AuthentificationCompleted = 0x34,
            RN_FingerFailedToInit = 0x35,
            RN_FingerFailedToStartCapture = 0x36,
            RN_FingerFailedToStopCapture = 0x37,
            RN_FingerSucceedToInit = 0x38,
            RN_FingerSucceedToStartCapture = 0x39,
            RN_FingerSucceedToStopCapture = 0x3A,
            RN_FingerUserUnknown = 0x3B,
            RN_CaptureGood = 0x3C,
            RN_CaptureBad = 0x3D,
        }
        private string serialNumber;
        private FingerNotify rnValue;
        private string message;
        private bool master;
        /// <summary>
        /// Constructor of rfidReaderArgs
        /// </summary>
        /// <param name="RNValue">Notification to launch</param>
        /// <param name="message">Message to displayed</param>
        ///  <param name="serialNumber">SerialNumber of the  Board</param>
        public FingerArgs(string serialNumber, FingerNotify RNValue, string message, bool master = true)
        {
            this.serialNumber = serialNumber;
            rnValue = RNValue;
            this.message = message;
            this.master = master;
        }
        /// <summary>
        /// Property to retreive serial number
        /// </summary>
        public string SerialNumber
        {
            get { return serialNumber; }
        }

        /// <summary>
        /// Property to retrieve message.
        /// </summary>
        /// <returns>a string of the message.</returns>
        public string Message
        {
            get { return message; }
        }
        /// <summary>
        /// Property to retrieve notification type
        /// </summary>
        /// <returns>The enumeration ReaderNotify</returns>
        public FingerNotify RN_Value
        {
            get { return rnValue; }
        }

        public bool Master { get { return master;  }}
    }

    public static class fingerUtils
    {
        public static void AddTemplateToUser(UserClass theUser, byte[] template, int fingerIndex)
        {
           
            System.IO.Stream dataStream = new System.IO.MemoryStream(template);
            DPFP.Template templateToLoad = new DPFP.Template(dataStream);
            
            FingerData newfinger = new FingerData();
            newfinger.CopyUserToFinger(theUser);
            newfinger.Templates[fingerIndex] = templateToLoad;

            if (fingerIndex > 4)
                newfinger.EnrolledFingersMask = newfinger.EnrolledFingersMask | (0x01 << ConvertSCtoFPIndex(fingerIndex));
            else
                newfinger.EnrolledFingersMask = newfinger.EnrolledFingersMask | (0x01 << ConvertSCtoFPIndex(fingerIndex));
            newfinger.CopyFingerToUser(theUser);
        }

        public static void DelTemplateToUser(UserClass theUser, int fingerIndex)
        {  
            FingerData newfinger = new FingerData();
            newfinger.CopyUserToFinger(theUser);
            newfinger.Templates[fingerIndex] = null;

            if (fingerIndex > 4)
                newfinger.EnrolledFingersMask = newfinger.EnrolledFingersMask & (0xFE << ConvertSCtoFPIndex(fingerIndex));
            else
                newfinger.EnrolledFingersMask = newfinger.EnrolledFingersMask & (0xFE << ConvertSCtoFPIndex(fingerIndex));
            newfinger.CopyFingerToUser(theUser);
        }

        public static int ConvertSCtoFPIndex(int fingerIndex)
        {
            int val = 0;
            switch (fingerIndex)
            {
                case 0: val = 5; break;
                case 1: val = 6; break;
                case 2: val = 7; break;
                case 3: val = 8; break;
                case 4: val = 9; break;
                case 5: val = 4; break;
                case 6: val = 3; break;
                case 7: val = 2; break;
                case 8: val = 1; break;
                case 9: val = 0; break;
            }
            return val;
        }
    }
}
