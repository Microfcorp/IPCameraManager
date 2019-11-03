using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera
{
    public partial class Main : Form
    {
        private string IP
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }
        private string UserName
        {
            get
            {
                return textBox2.Text;
            }
            set
            {
                textBox2.Text = value;
            }
        }
        private string Password
        {
            get
            {
                return textBox3.Text;
            }
            set
            {
                textBox3.Text = value;
            }
        }
        private uint HTTPPort
        {
            get
            {
                return (uint)numericUpDown1.Value;
            }
            set
            {
                numericUpDown1.Value = value;
            }
        }
        private uint RTSPPort
        {
            get
            {
                return (uint)numericUpDown2.Value;
            }
            set
            {
                numericUpDown2.Value = value;
            }
        }

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            var structures = Structures.Load();
            IP = structures.IP;
            UserName = structures.Name;
            Password = structures.Password;
            HTTPPort = structures.HTTPPort;
            RTSPPort = structures.RTSPPort;
        }
        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Visible = !panel1.Visible;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            var structures = new Structures(IP, UserName, Password, HTTPPort, RTSPPort);
            structures.Save();
        }

        private void конвертированиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Convert cnv = new Convert();
            cnv.ShowDialog();
        }

        private void просмотрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visible cnv = new Visible();
            cnv.ShowDialog();
        }

        private void загрузкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Download cnv = new Download();
            cnv.ShowDialog();
        }

        private void просмотрФотоToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ImageV cnv = new ImageV();
            cnv.ShowDialog();
        }

        private void логиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogView cnv = new LogView();
            cnv.ShowDialog();
        }

        private void создатьM3UToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //#EXTM3U
            //
            //#EXTINF:123, Исполнитель - Композиция
            //C:\Documents and Settings\Я\Моя музыка\Песня.mp3

            var structures = Structures.Load();

            var m3u = "#EXTM3U" + Environment.NewLine;
            m3u += Environment.NewLine;
            m3u += "#EXTINF:-1, IPCamera - Second is " +IP + Environment.NewLine;
            m3u += String.Format("rtsp://{0}:{1}@{2}:{3}/iphone/{4}", structures.Name, structures.Password, structures.IP, structures.RTSPPort, "11") + Environment.NewLine;
            m3u += Environment.NewLine;
            m3u += "#EXTINF:-1, IPCamera - First is " + IP + Environment.NewLine;
            m3u += String.Format("rtsp://{0}:{1}@{2}:{3}/iphone/{4}", structures.Name, structures.Password, structures.IP, structures.RTSPPort, "12");

            SaveFileDialog svf = new SaveFileDialog();
            svf.Filter = "M3U Файл|*.m3u";
            svf.FileName = "IP Camera";
            if(svf.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(svf.FileName, m3u);
            }
        }
    }
}
