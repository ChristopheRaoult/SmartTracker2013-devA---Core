using System;
using System.Collections.Generic;
using System.Text;

namespace DBClass_SQLServer
{
    public class UtilSqlServer
    {
         // Methods
        public static string ConvertConnectionString(string OldStr)
        {
            int startIndex = 0;
            startIndex = OldStr.Trim().LastIndexOf('=');
            string str = OldStr.Trim().Substring(startIndex);
            int num = str.Trim().IndexOf(';');
            if (num == 0)
            {
                num = str.Length;
            }
            str = str.Substring( 1, num - 1);
            string newValue = "";
            string str5 = str;
            int num3 = 0;
            int length = str5.Length;
            while (num3 < length)
            {
                char ch = str5[num3];
                newValue = newValue + Convert.ToString(Chr(Asc(ch.ToString()) - 0x2d));
                num3++;
            }
            return OldStr.Trim().Replace(str, newValue);
        }

        public static string EncryptPassword(string Pass)
        {
            string str2 = "";
            string str3 = Pass;
            int num = 0;
            int length = str3.Length;
            while (num < length)
            {
                char ch = str3[num];
                str2 = str2 + Convert.ToString(Chr(Asc(ch.ToString()) + 0x2d));
                num++;
            }
            return str2;
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
                str2 = str2 + Convert.ToString(Chr(Asc(ch.ToString()) - 0x2d));
                num++;
            }
            return str2;
        }

        static short Asc(string String)
        {
            return Encoding.Default.GetBytes(String)[0];
        }
        static string Chr(int CharCode)
        {
            if (CharCode > 255)
                throw new ArgumentOutOfRangeException("CharCode", CharCode, "CharCode must be between 0 and 255.");
            return Encoding.Default.GetString(new[] { (byte)CharCode });
        }
    }
    
}
