using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace smartTracker
{
    class AssemblyResolver
    {
        private static readonly string ApplicationPath = Application.StartupPath;
        private const string DLLSQLite = "System.Data.SQLite.DLL";

        public static void HandleUnresovledAssemblies()
        {
            if (File.Exists(Path.Combine(ApplicationPath, DLLSQLite))) // delete SQLite.DLL if user still has one embedded with SmartTracker
            {
                try
                {
                    File.Delete(Path.Combine(ApplicationPath, DLLSQLite));
                } catch (Exception)
                {
                    // File could not be deleted for access reasons, or because path was too long (?!).    
                }
            }

            AppDomain.CurrentDomain.AssemblyResolve += currentDomain_AssemblyResolve; // raised when a DLL is not found by the application
        }

        private static Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string path = Path.Combine(ApplicationPath, "lib");

            if (args.Name.ToLower().Contains("sqlite")) // if the not-found DLL is SQLite's one
            {
                path = Path.Combine(path, IntPtr.Size == 8 ? "x64" : "x86");
                path = Path.Combine(path, DLLSQLite); // ./lib/x64/<DLLSQLite> or ./lib/x86/<DLLSQLite>

                return Assembly.LoadFile(path);
            }

            /*
            if (args.Name.ToLower().Contains("yapi"))
            {
                path = Path.Combine(path, IntPtr.Size == 8 ? "x64" : "x86");
                path = Path.Combine(path, "yapi.dll");

                return Assembly.LoadFile(path);
            }*/

            return null;
        }
    }
}
