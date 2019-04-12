using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SDK_SC_Fingerprint
{
    interface IFingerPrintClass
    {

        bool DebugFP { get; set; }
        bool DebugFPFormVisible { get; set; }
        bool SendDebugMail { get; set; }
        int DebugWindowTimeout { get; set; }
        bool DebugOnFailureOnly { get; set; }

        TextBox TextBox { get; set; }
        string[] GetPluggedFingerprint();
        void InitFingerPrint(string ReaderSerialNumber, bool bUseEmbedDB);
        bool InitFingerPrint(string ReaderSerialNumber, string FingerSerialNumber);
        void ReleaseFingerprint();
        //void LoadFingerprintTemplates(string[] templates);
        int LoadFingerprintTemplates(string[] templates);
        void ClearFingerprintTemplates();
        string EnrollUser(string FPSerialNumber, string FirstName, string LastName, string template, bool bNotEnroll);
        int IsFingerEnrolled(string template, FingerIndexValue fingerToTest);
    }
}
