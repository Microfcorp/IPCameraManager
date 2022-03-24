using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using IPCamera.DLL;
using IPCamera.Settings;

namespace IPCamera.UI.Viewers
{
    public partial class singleplay : Form
    {
        Structures conf;
        //ffplayer ffplayer1 = new ffplayer();
        public singleplay()
        {
            InitializeComponent();
        }
        public singleplay(uint Selected)
        {
            InitializeComponent();
            conf = Structures.Load()[Selected];
            var Player = new InjectedAutoPlayer(conf.SinglePlay, conf, TypePlayer.HD);
            Player.Dock = DockStyle.Fill;
            Controls.Add(Player);
            triggerLabel1.Visible = conf.PTZ && conf.SinglePlay != TypeViewers.FFPLAY;
        }


        private void ffplay_Load(object sender, EventArgs e)
        {
            if (Settings.StaticMembers.PTZSettings.PTZKeys && conf.SinglePlay != TypeViewers.FFPLAY)
            {
                Thread th = new Thread(() =>
                {
                    while (true)
                    {
                        try
                        {
                            if (Process.GetCurrentProcess().Id.IsFocusWindow() && conf.GetPTZController().IsSuported)
                                if (Keys.Left.GetAsyncKeyState() != 0)
                                    conf.GetPTZController().CountiniousMove(ONVIF.PTZ.PTZParameters.Vector.LEFT);
                                else if (Keys.Right.GetAsyncKeyState() != 0)
                                    conf.GetPTZController().CountiniousMove(ONVIF.PTZ.PTZParameters.Vector.RIGHT);
                                else if (Keys.Up.GetAsyncKeyState() != 0)
                                    conf.GetPTZController().CountiniousMove(ONVIF.PTZ.PTZParameters.Vector.UP);
                                else if (Keys.Down.GetAsyncKeyState() != 0)
                                    conf.GetPTZController().CountiniousMove(ONVIF.PTZ.PTZParameters.Vector.DOWN);
                            Thread.Sleep(100);
                        }
                        catch { };
                        Thread.Sleep(300);
                    }
                });
                th.Start();
                FormClosing += (o, q) => { th.Abort(); };
            }            
        }

        private void ffplay_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            panel1.SetBackgroundTransparent();           
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (conf.GetPTZController().IsSuported)
                conf.GetPTZController().AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.UP);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (conf.GetPTZController().IsSuported)
                conf.GetPTZController().AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.RIGHT);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (conf.GetPTZController().IsSuported)
                conf.GetPTZController().AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.DOWN);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (conf.GetPTZController().IsSuported)
                conf.GetPTZController().AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.LEFT);
        }

        private void triggerLabel1_Click(object sender, EventArgs e)
        {
            
        }

        private void triggerLabel1_ClickBool(object sender, EventArgs e)
        {
            panel1.Visible = !panel1.Visible;
        }

        private void singleplay_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
    }
}
