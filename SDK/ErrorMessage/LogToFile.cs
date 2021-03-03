using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorMessage
{
    public static class LogToFile
    {
        public static string GetTempPath()
        {
            string path = System.Environment.GetEnvironmentVariable("TEMP");
            if (!path.EndsWith("\\")) path += "\\SPACECODE\\";
            else
                path += "SPACECODE\\";
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            return path;
        }

        static object lockMethod = new object();
        public static void LogMessageToFile(string msg)
        {
            lock (lockMethod)
            {
                string filename = string.Format("SmartTrakerLog_{0}.txt", DateTime.Now.ToString("yyMMdd"));

                System.IO.StreamWriter sw = System.IO.File.AppendText(
                GetTempPath() + filename);
                try
                {
                    string logLine = System.String.Format(
                        "{0:G}: {1}", System.DateTime.Now, msg);
                    sw.WriteLine(logLine);
                }
                finally
                {
                    sw.Close();
                }
            }
        }
    }
}
