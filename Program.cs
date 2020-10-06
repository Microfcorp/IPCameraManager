using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using IPCamera.ComandArgs;
using IPCamera.Network.AutoUpdate;
using IPCamera.DLL;

namespace IPCamera
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Parser p = new Parser(args);

            var vers = NetworkUpdate.GetVersionServer();
            if (vers > CurrentVersion.CurrentVersions)
                Application.Run(new Updater(vers));

            if (File.Exists("UpdateDownloading.exe"))
                File.Delete("UpdateDownloading.exe");

            if (p.FindParams("-c"))
            {
                Application.Run(new Convert());
            }
            else
            {
                Application.Run(new Main(p));
            }

        }
    }
}
