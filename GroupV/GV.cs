using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera.GroupV
{
    public partial class GV : Form
    {
        readonly Settings.Structures[] set = Settings.Structures.Load();
        readonly SortedList<string, CV.MootionDetect> Cameras = new SortedList<string, CV.MootionDetect>();
        readonly SortedList<string, ImageBox> ICameras = new SortedList<string, ImageBox>();

        public int SizeSET
        {
            get => set.Length;
        }

        public new const int Padding = 10;
        public const double SteepZoom = 0.01;
        public const int SteepVP = 1;
        public const int SteepHP = 1;

        public GV()
        {
            InitializeComponent();
            Cursor = Cursors.WaitCursor;
        }

        public void ResizeF()
        {
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.Controls.Clear();

            for (int i = 0; i < set.Length; i++)
            {
                if(!Cameras.ContainsKey(set[i].IP + i))
                    Cameras.Add(set[i].IP + i, new CV.MootionDetect(set[i].GetRTSPSecond));
                
                Cameras[set[i].IP + i].DetectPed = false;

                if (!ICameras.ContainsKey(set[i].IP + i))
                    ICameras.Add(set[i].IP + i, new ImageBox());

                ICameras[set[i].IP + i].SizeMode = PictureBoxSizeMode.Zoom;
                ICameras[set[i].IP + i].FunctionalMode = ImageBox.FunctionalModeOption.PanAndZoom;
                //ICameras[set[i].IP].Anchor = AnchorStyles.Left | AnchorStyles.Right;
                //ICameras[set[i].IP].Dock = DockStyle.None;

                int ia = i;
                Cameras[set[i].IP + i].OnMD += (l, m, h) => { if (ICameras.ContainsKey(set[ia].IP + ia)) if (!ICameras[set[ia].IP + ia].Disposing && !ICameras[set[ia].IP + ia].IsDisposed) { try { ICameras[set[ia].IP + ia].Image = m; } catch { } } };
                
                if(Orientation.OrientationSensor.GetOrientationType(flowLayoutPanel1.Size) == Orientation.OrientationType.Vertical) ICameras[set[i].IP + i].Size = new Size((flowLayoutPanel1.ClientSize.Width) - Padding, (flowLayoutPanel1.ClientSize.Height / SizeSET) - Padding);
                else if(Orientation.OrientationSensor.GetOrientationType(flowLayoutPanel1.Size) == Orientation.OrientationType.Horizontal) ICameras[set[i].IP + i].Size = new Size((flowLayoutPanel1.ClientSize.Width / SizeSET) - Padding, (flowLayoutPanel1.ClientSize.Height) - Padding);               

                ICameras[set[i].IP + i].MouseDoubleClick += (o, e) => 
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        var ib = (o as ImageBox);
                        ib.SetZoomScale(1, new Point());
                        ib.HorizontalScrollBar.Value = 0;
                        ib.VerticalScrollBar.Value = 0;
                    }
                };
                ICameras[set[i].IP + i].PreviewKeyDown += (o, e) =>
                {
                    var ib = (o as ImageBox);
                    if (e.KeyCode == Keys.Down)
                        ib.VerticalScrollBar.Value = ib.VerticalScrollBar.Maximum <= (ib.VerticalScrollBar.Value + SteepVP) ? ib.VerticalScrollBar.Maximum : ib.VerticalScrollBar.Value + SteepVP;
                    else if (e.KeyCode == Keys.Up)
                        ib.VerticalScrollBar.Value = ib.VerticalScrollBar.Minimum >= (ib.VerticalScrollBar.Value - SteepVP) ? ib.VerticalScrollBar.Minimum : ib.VerticalScrollBar.Value - SteepVP;
                    else if (e.KeyCode == Keys.Right)
                        ib.HorizontalScrollBar.Value = ib.HorizontalScrollBar.Maximum <= (ib.HorizontalScrollBar.Value + SteepHP) ? ib.HorizontalScrollBar.Maximum : ib.HorizontalScrollBar.Value + SteepHP;
                    else if (e.KeyCode == Keys.Left)
                        ib.HorizontalScrollBar.Value = ib.HorizontalScrollBar.Minimum >= (ib.HorizontalScrollBar.Value - SteepHP) ? ib.HorizontalScrollBar.Minimum : ib.HorizontalScrollBar.Value - SteepHP;
                    else if (e.KeyCode == Keys.Add)
                        ib.SetZoomScale(ib.ZoomScale + SteepZoom, new Point());
                    else if (e.KeyCode == Keys.Subtract)
                        ib.SetZoomScale(ib.ZoomScale - SteepZoom, new Point());
                };
                flowLayoutPanel1.Controls.Add(ICameras[set[i].IP + i]);
            }
            flowLayoutPanel1.ResumeLayout();
        }

        private void GV_Load(object sender, EventArgs e)
        {
            ResizeF();
            Cursor = Cursors.Default;
        }

        private void flowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            ResizeF();
        }

        private void GV_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var item in Cameras) item.Value.Dispose();
            foreach (var item in ICameras) item.Value.Dispose();
        }
    }
}
