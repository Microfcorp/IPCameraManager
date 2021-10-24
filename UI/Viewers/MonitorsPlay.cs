using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IPCamera.Monitors;
using IPCamera.Settings;

namespace IPCamera.UI.Viewers
{
    public partial class MonitorsPlay : Form
    {
        Monitors.MonitorSettings monitor;
        
        int SizeSET = 1;
        public MonitorsPlay()
        {
            InitializeComponent();
        }
        public MonitorsPlay(uint ID) : this()
        {
            monitor = MonitorsController.Load()[ID];
        }

        private void MonitorsPlay_Load(object sender, EventArgs e)
        {
            SizeSET = monitor.Monitors.Length;
            for (int i = 0; i < monitor.GetMonitorsSortPosition.Length; i++)
            {
                var m = monitor.GetMonitorsSortPosition[i];
                var Player = new InjectedAutoPlayer(m.GetCamera.MonitorPlay, m.GetCamera);
                flowLayoutPanel1.Controls.Add(Player);
            }
            flowLayoutPanel1.ResizeF();
        }

        private void MonitorsPlay_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void MonitorsPlay_Resize(object sender, EventArgs e)
        {
            flowLayoutPanel1.ResizeF();
        }
    }
}
