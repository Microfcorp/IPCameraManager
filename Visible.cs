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

namespace IPCamera
{
    public partial class Visible : Form
    {             

        Process s;

        Settings.Structures Setting;

        bool IsHigh = true;

        public Visible(uint Selected)
        {
            InitializeComponent();
            Setting = Settings.Structures.Load()[Selected];
            Application.EnableVisualStyles();
            this.DoubleBuffered = true;
            BackColor = Color.Blue;
        }

        void Play()
        {          
            if (s != null && !s.HasExited) s.Kill();

            s = new Process();
            s.StartInfo.FileName = "ffplay.exe";
            s.StartInfo.Arguments = String.Format(IsHigh ? Setting.GetRTSPFirst : Setting.GetRTSPSecond)
                + " -x 640 -y 360";
            s.StartInfo.UseShellExecute = false;
            s.StartInfo.CreateNoWindow = true;
            //s.StartInfo.RedirectStandardError = true;
            s.EnableRaisingEvents = true;
            s.StartInfo.RedirectStandardError = true;
            //s.OutputDataReceived += (o, e) => Debug.WriteLine(e.Data ?? "NULL", "ffplay");
            //s.ErrorDataReceived += (o, e) => Debug.WriteLine(e.Data ?? "NULL", "ffplay");      

            s.Exited += (o, e) => { BackColor = Color.Red;};

            s.Start();
            s.BeginErrorReadLine();

            this.StopFlashing();

            s.ErrorDataReceived += (o, e) => { if (GroupV.FFPGV.IsRunFFPLAY(e.Data)) try { if (InvokeRequired) Invoke(new Action(() => {  BackColor = Color.FromArgb(255, 192, 192); })); } catch { } };                      

            Thread.Sleep(200); // you need to wait/check the process started, then...

            // child, new parent
            // make 'this' the parent of ffmpeg (presuming you are in scope of a Form or Control)
            s.MainWindowHandle.SetParent(this.Handle);
            s.MainWindowHandle.ShowWindow((int)CommonFunctions.nCmdShow.SW_HIDE);

            // window, x, y, width, height, repaint
            // move the ffplayer window to the top-left corner and set the size to 320x280
            //MoveWindow(s.MainWindowHandle, 0, 0, 320, 280, true);
            //s.MainWindowHandle.SetWindowLong((int)CommonFunctions.WindowLongFlags.GWL_STYLE, WindowStyles.WS_VISIBLE);
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
                    IsHigh = !IsHigh;
                    s.EnableRaisingEvents = false;
                    BackColor = Color.Blue;
                    Play();
                }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IsHigh = !IsHigh;
            s.EnableRaisingEvents = false;
            BackColor = Color.Blue;
            Play();
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
