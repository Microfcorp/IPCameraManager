using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera
{
    public partial class Visible : Form
    {
        public const string RTSP = "rtsp://{0}:{1}@{2}:{3}/iphone/{4}"; //rtsp://admin:admin@192.168.1.34/iphone/11

        string stream = ((int)Network.RTSPStream.First_Stream).ToString(); //11 and 12

        Process s;

        public Visible()
        {
            InitializeComponent();
        }

        void Play()
        {
            var Setting = IPCamera.Settings.Structures.Load();

            if (s != null) s.Kill();
           
            s = Process.Start("ffplay.exe", String.Format(RTSP, Setting.Name, Setting.Password, Setting.IP, Setting.RTSPPort, stream) 
                + " -x 640 -y 360");
            s.StartInfo.UseShellExecute = false;
            s.StartInfo.CreateNoWindow = true;
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
                    stream = stream == ((int)Network.RTSPStream.First_Stream).ToString() ? ((int)Network.RTSPStream.Second_Stream).ToString() : ((int)Network.RTSPStream.First_Stream).ToString();
                    Play();
                }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            stream = stream == Network.RTSPStream.First_Stream.ToString() ? ((int)Network.RTSPStream.Second_Stream).ToString() : ((int)Network.RTSPStream.First_Stream).ToString();
            Play();
        }

        private void Visible_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (s != null) if(!s.HasExited) s.Kill();
        }
    }
}
