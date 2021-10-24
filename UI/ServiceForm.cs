using IPCamera.Settings;
using IPCamera.Settings.StaticMembers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPCamera.UI
{
    public partial class ServiceForm : Form
    {
        public ServiceForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var s = Structures.Load().ToList();
            s.Add(Structures.Null);
            Structures.Save(s.ToArray());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var s = Monitors.MonitorsController.Load().ToList();
            s.Add(Monitors.MonitorSettings.Null);
            Monitors.MonitorsController.Save(s.ToArray());
        }

        private void ServiceForm_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = ImageSettings.FPS;
            numericUpDown2.Value = ImageSettings.Padding;
            numericUpDown3.Value = PTZSettings.StepTimeout;
            numericUpDown4.Value = PTZSettings.Timeout;
            checkBox1.Checked = PTZSettings.PTZKeys;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            ImageSettings.FPS = (uint)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            ImageSettings.Padding = (int)numericUpDown2.Value;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            PTZSettings.Timeout = (uint)numericUpDown4.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            PTZSettings.StepTimeout = (uint)numericUpDown3.Value;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var structuress = Structures.Load();

            var m3u = "#EXTM3U" + Environment.NewLine;

            foreach (var structures in structuress)
            {
                m3u += Environment.NewLine;
                m3u += "#EXTINF:-1, IPCamera - First is " + structures.IP + Environment.NewLine;
                m3u += structures.GetRTSPFirstONVIF;
                m3u += Environment.NewLine;
                m3u += "#EXTINF:-1, IPCamera - Second is " + structures.IP + Environment.NewLine;
                m3u += structures.GetRTSPSecondONVIF + Environment.NewLine;
                m3u += Environment.NewLine;             
            }
            SaveFileDialog svf = new SaveFileDialog
            {
                Filter = "M3U Файл|*.m3u",
                FileName = "IP Cameras"
            };
            if (svf.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(svf.FileName, m3u);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            PTZSettings.PTZKeys = checkBox1.Checked;
        }
    }
}
