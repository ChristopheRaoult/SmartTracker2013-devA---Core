using System;
using System.Globalization;
using System.Text;
using System.Management;
using System.Windows.Forms;
using smartTracker.Properties;

namespace smartTracker
{
    static class Encryption
    {
        static public string InverseByBase(string st, int moveBase)
        {
            StringBuilder sb = new StringBuilder();
            //st = ConvertToLetterDigit(st);
            for (int i = 0; i < st.Length; i += moveBase)
            {
                int c;
                if (i + moveBase > st.Length - 1)
                    c = st.Length - i;
                else
                    c = moveBase;
                sb.Append(InverseString(st.Substring(i, c)));
            }
            return sb.ToString();
        }
        static public string InverseString(string st)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = st.Length - 1; i >= 0; i--)
            {
                sb.Append(st[i]);
            }
            return sb.ToString();
        }
        static public string ConvertToLetterDigit(string st)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char ch in st)
            {
                if (char.IsLetterOrDigit(ch) == false)
                    sb.Append(Convert.ToInt16(ch).ToString(CultureInfo.InvariantCulture));
                else
                    sb.Append(ch);
            }
            return sb.ToString();
        }

        /// <summary>
        /// moving all characters in string insert then into new index
        /// </summary>
        /// <param name="st">string to moving characters</param>
        /// <returns>moved characters string</returns>
        static public string Boring(string st)
        {
            for (int i = 0; i < st.Length; i++)
            {
                int newPlace = i * Convert.ToUInt16(st[i]);
                newPlace = newPlace % st.Length;
                char ch = st[i];
                st = st.Remove(i, 1);
                st = st.Insert(newPlace, ch.ToString(CultureInfo.InvariantCulture));
            }
            return st;
        }

        static public string MakePassword(string st, string identifier)
        {
            if (identifier.Length != 3)
                throw new ArgumentException("Identifier must be 3 character length");

            int[] num = new int[3];
            num[0] = Convert.ToInt32(identifier[0].ToString(CultureInfo.InvariantCulture), 10);
            num[1] = Convert.ToInt32(identifier[1].ToString(CultureInfo.InvariantCulture), 10);
            num[2] = Convert.ToInt32(identifier[2].ToString(CultureInfo.InvariantCulture), 10);
            st = Boring(st);
            st = InverseByBase(st, num[0]);
            st = InverseByBase(st, num[1]);
            st = InverseByBase(st, num[2]);

            StringBuilder sb = new StringBuilder();
            foreach (char ch in st)
            {
                sb.Append(ChangeChar(ch, num));
            }
            return sb.ToString();
        }

        static private char ChangeChar(char ch, int[] enCode)
        {
            ch = char.ToUpper(ch);
            if (ch >= 'A' && ch <= 'H')
                return Convert.ToChar(Convert.ToInt16(ch) + 2 * enCode[0]);
            if (ch >= 'I' && ch <= 'P')
                return Convert.ToChar(Convert.ToInt16(ch) - enCode[2]);
            if (ch >= 'Q' && ch <= 'Z')
                return Convert.ToChar(Convert.ToInt16(ch) - enCode[1]);
            if (ch >= '0' && ch <= '4')
                return Convert.ToChar(Convert.ToInt16(ch) + 5);
            if (ch >= '5' && ch <= '9')
                return Convert.ToChar(Convert.ToInt16(ch) - 5);
            return '0';
        }
    }
    static class SystemInfo
    {
        #region -> Private Variables

        public static bool UseProcessorId;
        public static bool UseBaseBoardProduct;
        public static bool UseBaseBoardManufacturer;
        public static bool UseDiskDriveSignature;
        public static bool UseVideoControllerCaption;
        public static bool UsePhysicalMediaSerialNumber;
        public static bool UseBiosVersion;
        public static bool UseBiosManufacturer;
        public static bool UseWindowsSerialNumber;

        #endregion

        public static string GetSystemInfo(string softwareName)
        {
            UseBaseBoardManufacturer = true;
            UseBaseBoardProduct = true;
            UseBiosManufacturer = true;
            UseBiosVersion = true;
            UseDiskDriveSignature = true;
            UsePhysicalMediaSerialNumber = true;
            UseProcessorId = true;
            UseVideoControllerCaption = true;
            UseWindowsSerialNumber = true;


            if (UseProcessorId)
                softwareName += RunQuery("Processor", "ProcessorId");

            if (UseBaseBoardProduct)
                softwareName += RunQuery("BaseBoard", "Product");

            if (UseBaseBoardManufacturer)
                softwareName += RunQuery("BaseBoard", "Manufacturer");

            if (UseDiskDriveSignature)
                softwareName += RunQuery("DiskDrive", "Signature");

            if (UseVideoControllerCaption)
                softwareName += RunQuery("VideoController", "Caption");

            if (UsePhysicalMediaSerialNumber)
                softwareName += RunQuery("PhysicalMedia", "SerialNumber");

            if (UseBiosVersion)
                softwareName += RunQuery("BIOS", "Version");

            if (UseWindowsSerialNumber)
                softwareName += RunQuery("OperatingSystem", "SerialNumber");

            softwareName = RemoveUseLess(softwareName);

            if (softwareName.Length < 25)
                return GetSystemInfo(softwareName);

            // return SoftwareName.Substring(0, 25).ToUpper();
            return softwareName.ToUpper();
        }

        private static string RemoveUseLess(string st)
        {
            for (int i = st.Length - 1; i >= 0; i--)
            {
                char ch = char.ToUpper(st[i]);

                if ((ch < 'A' || ch > 'Z') &&
                    (ch < '0' || ch > '9'))
                {
                    st = st.Remove(i, 1);
                }
            }
            return st;
        }

        private static string RunQuery(string tableName, string methodName)
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("Select * from Win32_" + tableName);
            foreach (var o in mos.Get())
            {
                ManagementObject mo = (ManagementObject) o;
                try
                {
                    if (mo[methodName] != null)
                        return mo[methodName].ToString();
                    return "";
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format(ResStrings.str__Failed_Query_Windows_Info, e.Message, tableName, methodName));
                }
            }
            return "";
        }
    }
}
