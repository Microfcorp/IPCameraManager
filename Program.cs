using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using IPCamera.ComandArgs;

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

            if (p.FindParams("-c"))
            {
               // Application.Run(new Convert(args));
            }
            else
            {
                Application.Run(new Main());
            }
        }
    }
}
