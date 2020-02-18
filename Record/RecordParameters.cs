using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using IPCamera.DLL;

namespace IPCamera.Record
{
    public class RecordParameters
    {
        /// <summary>
        /// Ведется ли сейчас запись
        /// </summary>
        public bool IsRunning
        {
            get;
            set;
        }
        /// <summary>
        /// Остановлена ли запись вручную
        /// </summary>
        public bool IsStop
        {
            get;
            set;
        }
        /// <summary>
        /// Дискрептор записи
        /// </summary>
        public IntPtr Handler
        {
            get;
            set;
        }
        /// <summary>
        /// Дискрептор записи
        /// </summary>
        public Process Process
        {
            get;
            set;
        }
        /// <summary>
        /// ID записи
        /// </summary>
        public int ID
        {
            get;
            set;
        }

        public event EventHandler Exited;

        internal void Exit(object o, EventArgs e)
        {
            Exited?.Invoke(this,e);
        }
        /// <summary>
        /// Закрывает окно с FFMPEG
        /// </summary>
        public void Close()
        {
            //Process.StandardInput.AutoFlush = true;
            //Process.StandardInput.WriteLine("\x3");
            //Process.StandardInput.Close();
            //Process.MainWindowHandle.GetProcessId().GenerateConsoleCtrlEvent(CommonFunctions.ConsoleCtrlEvent.CTRL_C);
            if (Consoles.AttachConsole((uint)Process.Id))
            {
                Consoles.SetConsoleCtrlHandler(null, true);
                try
                {
                    if (!Consoles.GenerateConsoleCtrlEvent(Consoles.CTRL_C_EVENT, 0))
                        return;
                    Process.WaitForExit();
                }
                finally
                {
                    Consoles.FreeConsole();
                    Consoles.SetConsoleCtrlHandler(null, false);
                }
                return;
            }
        }
    }
}
