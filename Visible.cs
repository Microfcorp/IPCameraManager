using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using IPCamera.DLL;
using IPCamera.Settings;

namespace IPCamera
{
    public partial class Visible : Form
    {             

        Process s;

        Settings.Structures Setting;
        uint Selected;

        bool IsHigh = true;

        /// <summary>
        /// Возникает при перезагрузке потока
        /// </summary>
        public event EventHandler Reload;

        bool ump;

        public delegate void StreamData(object objects, string stream);

        public event StreamData StreamOK;

        public event StreamData CloseStream;

        public Visible(uint Selected, bool ump)
        {
            InitializeComponent();
            Setting = Settings.Structures.Load()[Selected];
            this.Selected = Selected;
            Application.EnableVisualStyles();
            this.DoubleBuffered = true;
            BackColor = Color.Blue;
            this.ump = ump;
        }

        public void mShow()
        {
            Show();
            Visible = false;
        }

        void Play()
        {
            if (s != null && !s.HasExited) s.Kill();
            Setting = Settings.Structures.Load()[Selected];
            Thread th = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        if (!s.HasExited && s.Id.IsFocusWindow() && Setting.GetPTZController().IsSuported)
                            if (Keys.Left.GetAsyncKeyState() != 0)
                                Setting.GetPTZController().CountiniousMove(ONVIF.PTZ.PTZParameters.Vector.LEFT);
                            else if (Keys.Right.GetAsyncKeyState() != 0)
                                Setting.GetPTZController().CountiniousMove(ONVIF.PTZ.PTZParameters.Vector.RIGHT);
                            else if (Keys.Up.GetAsyncKeyState() != 0)
                                Setting.GetPTZController().CountiniousMove(ONVIF.PTZ.PTZParameters.Vector.UP);
                            else if (Keys.Down.GetAsyncKeyState() != 0)
                                Setting.GetPTZController().CountiniousMove(ONVIF.PTZ.PTZParameters.Vector.DOWN);
                        Thread.Sleep(100);
                    }
                    catch { };
                    Thread.Sleep(100);
                }
            });

            if (!ump)
            {               
                s = new Process();
                s.StartInfo.FileName = "ffplay.exe";
                var strem = String.Format(IsHigh ? Setting.GetRTSPFirstONVIF : Setting.GetRTSPSecondONVIF);
                s.StartInfo.Arguments = strem
                    + " -x 640 -y 360";
                s.StartInfo.UseShellExecute = false;
                s.StartInfo.CreateNoWindow = true;
                //s.StartInfo.RedirectStandardError = true;
                s.EnableRaisingEvents = true;
                s.StartInfo.RedirectStandardError = true;
                //s.OutputDataReceived += (o, e) => Debug.WriteLine(e.Data ?? "NULL", "ffplay");
                //s.ErrorDataReceived += (o, e) => Debug.WriteLine(e.Data ?? "NULL", "ffplay");      

                s.Exited += (o, e) => { CloseStream?.Invoke(this, strem); BackColor = Color.Red; try { th.Abort(); } catch { } };

                

                s.Start();
                s.BeginErrorReadLine();

                this.StopFlashing();

                s.ErrorDataReceived += (o, e) => { if (GroupV.FFPGV.IsRunFFPLAY(e.Data)) try { if (InvokeRequired) Invoke(new Action(() => { StreamOK?.Invoke(this, strem); BackColor = Color.FromArgb(255, 192, 192); })); } catch { } };

                if (Setting.GetPTZController().IsSuported) th.Start();

                Thread.Sleep(200); // you need to wait/check the process started, then...

                // child, new parent
                // make 'this' the parent of ffmpeg (presuming you are in scope of a Form or Control)
                if(!s.HasExited) s.MainWindowHandle.SetParent(this.Handle);
                if (!s.HasExited) s.MainWindowHandle.ShowWindow((int)CommonFunctions.nCmdShow.SW_HIDE);

                FormClosing += (o, e) => th.Abort();

                // window, x, y, width, height, repaint
                // move the ffplayer window to the top-left corner and set the size to 320x280
                //MoveWindow(s.MainWindowHandle, 0, 0, 320, 280, true);
                //s.MainWindowHandle.SetWindowLong((int)CommonFunctions.WindowLongFlags.GWL_STYLE, WindowStyles.WS_VISIBLE);
                Reload?.Invoke(this, new EventArgs());
            }
            else
            {
                s = new Process();
                s.StartInfo.FileName = "mplayer.exe";
                s.StartInfo.Arguments = String.Format(IsHigh ? Setting.GetRTSPFirstONVIF : Setting.GetRTSPSecondONVIF)
                    + " -dumpfile file.txt -mc 10 -nofs -x 640 -y 360";
                s.StartInfo.UseShellExecute = false;
                s.StartInfo.CreateNoWindow = true;
                //s.StartInfo.RedirectStandardError = true;
                s.EnableRaisingEvents = true;
                s.StartInfo.RedirectStandardOutput = true;
                s.Start();
                s.BeginOutputReadLine();
                this.StopFlashing();

                if (Setting.GetPTZController().IsSuported) th.Start();

                s.OutputDataReceived += (o, e) => { if (GroupV.FFPGV.IsRunFFPLAY(e.Data)) try { if (InvokeRequired) Invoke(new Action(() => { BackColor = Color.FromArgb(255, 192, 192); })); } catch { } };

                s.Exited += (o, e) => { BackColor = Color.Red; th.Abort(); };
            }
        }

        private void Visible_Load(object sender, EventArgs e)
        {
            Play();
        }

        private void Visible_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
                if (e.KeyCode == Keys.S)
                {
                    ChangeRes();
                }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChangeRes();
        }

        private void ChangeRes()
        {
            IsHigh = !IsHigh;
            s.EnableRaisingEvents = false;
            BackColor = Color.Blue;
            Play();
            button1.Text = "Переключить поток ("+(IsHigh?"Первичный":"Вторичный")+")";
        }

        private void Visible_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (s != null && !s.HasExited) s.Kill();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (s != null && !s.HasExited) s.Kill();
            Play();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (s != null && !s.HasExited) s.Kill();
            Close();
        }
    }
}
