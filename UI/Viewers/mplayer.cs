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
    public partial class mplayer : UserControl
    {
        const string mplaypath = "mplayer.exe";
        const string needargs = "  -x 640 -y 360 -nocache -framedrop ";

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
        public mplayer()
        {
            InitializeComponent();
        }
        public mplayer(string path) : base()
        {
            FilePath = path;
            Startmplayer();
        }

        private void ffplayer_Load(object sender, EventArgs e)
        {
            if (AutoPlay) Startmplayer();
        }

        public void Startmplayer()
        {
            label1.Text = "Получение потока:" + Environment.NewLine + FilePath;
            // start ffplay 
            var mplaye = new Process
            {
                StartInfo =
                {
                    FileName = mplaypath,
                    Arguments = needargs + FilePath,
                    CreateNoWindow = true, 
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
            var h = Handle;
            mplaye.EnableRaisingEvents = true;
            mplaye.OutputDataReceived += (o, e) => Debug.WriteLine(e.Data ?? "NULL", "mplayer");
            mplaye.ErrorDataReceived += (o, e) => { 
                if (GroupV.FFPGV.IsRunMPLAYER(e.Data) && !IsRunOK) 
                {
                    IsRunOK = true;
                    Thread.Sleep(1000);
                    mplaye.MainWindowHandle.SetParent(h);
                    mplaye.MainWindowHandle.MoveWindow(0, 0, ClientSize.Width, ClientSize.Height, true);
                    mplaye.MainWindowHandle.SetWindowLongPtr((int)CommonFunctions.WindowLongFlags.GWL_STYLE, new IntPtr(WindowStyles.WS_VISIBLE));
                }
            };
            mplaye.Exited += (o, e) => { try { Invoke(new Action(() => { Dispose(); } )); } catch { } };
            mplaye.Start();
            mplaye.BeginErrorReadLine();

            Disposed += (o, e) => { if(!mplaye.HasExited) mplaye.Kill(); };
            Resize += (o, e) => { mplaye.MainWindowHandle.MoveWindow(0, 0, ClientSize.Width, ClientSize.Height, true); };
        }
    }
}
