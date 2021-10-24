using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using IPCamera.DLL;

namespace IPCamera.UI.Viewers
{
    public partial class ffplayer : UserControl
    {
        const string ffplaypath = "ffplay.exe";
        const string needargs = "  -x 640 -y 360 ";

        /// <summary>
        /// Файл воспроизведения
        /// </summary>
        public string FilePath
        {
            get;
            set;
        }
        /// <summary>
        /// Успешно ли получен поток
        /// </summary>
        public bool IsRunOK
        {
            get;
            private set;
        }
        /// <summary>
        /// Автоматическое воспроизведение при запуске UI
        /// </summary>
        public bool AutoPlay
        {
            get;
            private set;
        }
        public ffplayer()
        {
            InitializeComponent();
        }
        public ffplayer(string path) : base()
        {
            FilePath = path;
            StartFFPLAY();
        }

        private void ffplayer_Load(object sender, EventArgs e)
        {
            if (AutoPlay) StartFFPLAY();
        }

        public void StartFFPLAY()
        {
            label1.Text = "Получение потока:" + Environment.NewLine + FilePath;
            // start ffplay 
            var ffplay = new Process
            {
                StartInfo =
                {
                    FileName = ffplaypath,
                    Arguments = needargs + FilePath,
                    CreateNoWindow = true, 
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
            var h = Handle;
            ffplay.EnableRaisingEvents = true;
            ffplay.OutputDataReceived += (o, e) => Debug.WriteLine(e.Data ?? "NULL", "ffplay");
            ffplay.ErrorDataReceived += (o, e) => { 
                if (GroupV.FFPGV.IsRunFFPLAY(e.Data) && !IsRunOK) 
                {
                    IsRunOK = true;
                    Thread.Sleep(10);
                    ffplay.MainWindowHandle.SetParent(h);
                    ffplay.MainWindowHandle.MoveWindow(0, 0, ClientSize.Width, ClientSize.Height, true);
                    ffplay.MainWindowHandle.SetWindowLongPtr((int)CommonFunctions.WindowLongFlags.GWL_STYLE, new IntPtr(WindowStyles.WS_VISIBLE));
                }
            };
            ffplay.Exited += (o, e) => Dispose();
            ffplay.Start();
            ffplay.BeginErrorReadLine();

            Disposed += (o, e) => { if(!ffplay.HasExited) ffplay.Kill(); };
            Resize += (o, e) => { ffplay.MainWindowHandle.MoveWindow(0, 0, ClientSize.Width, ClientSize.Height, true); };
        }
    }
}
