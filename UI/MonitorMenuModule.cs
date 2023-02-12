using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IPCamera.Monitors;
using System.Drawing.Drawing2D;

namespace IPCamera.UI
{
    public partial class MonitorMenuModule : UserControl, IUI
    {
        public Monitors.MonitorSettings monitorset;
        uint id;
        public MonitorMenuModule()
        {
            InitializeComponent();
        }
        public MonitorMenuModule(uint Mid) : this()
        {
            id = Mid;
            UpdateForm();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var MS = new MonitorSettings(id);
            MS.OnSave += (a, b) => { UpdateForm(); };
            MS.FormClosed += (a, b) => Enabled = true;
            MS.OpenToWindowManager((MainForm)ParentForm).PointToCenter(ParentForm);
            Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            (new Viewers.MonitorsPlay(id)).OpenToWindowManager((MainForm)ParentForm, "Монитор - " + monitorset.Name);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var s = Monitors.MonitorsController.Load().ToList();
            s.RemoveAt((int)id);
            Monitors.MonitorsController.Save(s.ToArray());
            Dispose();
        }

        private void MonitorMenuModule_Paint(object sender, PaintEventArgs e)
        {
            LinearGradientBrush linGrBrush = new LinearGradientBrush(
            new Point(0, 0),
            new Point(Width, Height),
            Color.FromArgb(255, 0, 0, 110),   // opaque blue
            Color.FromArgb(255, 0, 105, 105));  // opaque green

            Pen pen = new Pen(linGrBrush, 10);

            e.Graphics.DrawRectangle(pen, 0, 0, Width, Height);
            //e.Graphics.DrawLine(pen, 0, 0, 600, 300);
            //e.Graphics.FillEllipse(linGrBrush, 0, 0, Width, Height);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string tmp = "Данный монитор содержит следующие камеры:" + Environment.NewLine;
            foreach (var item in monitorset.GetMonitorsSortPosition)
            {
                tmp += item.GetCamera.NameCamera + Environment.NewLine;
            }
            MessageBox.Show(tmp);
        }

        public void UpdateForm()
        {
            monitorset = MonitorsController.Load()[id];
            label1.Text = monitorset.Name;
            linkLabel2.Text = monitorset.Monitors.Length.ToString();
        }
    }
}
