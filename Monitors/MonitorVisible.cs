using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera.Monitors
{
    public partial class MonitorVisible : Form
    {
        public new int Padding = Settings.StaticMembers.ImageSettings.Padding;

        Monitor[] Monitors;
        Structures[] settings;
        MonitorSettings MS;
        private Point lastPoint;
        private uint SelectedCamera = 0;

        private bool IsAutoOrient { get; set; }
        public int SizeSET
        {
            get => Monitors.Length;
        }

        public MonitorVisible(MonitorSettings ms)
        {
            InitializeComponent();
            settings = Structures.Load();
            MS = ms;
            IsAutoOrient = true;
            Monitors = ms.Monitors.Where(tmp => tmp.Enable).OrderBy(tmp => tmp.Position).ToArray();

            flowLayoutPanel1.ControlAdded += (o, e) =>
            {
                (o as Control).KeyDown += Photo_KeyDown;
            };

            SizeR();
            Text = ms.Name;
        }

        private void SizeR()
        {
            flowLayoutPanel1.SuspendLayout();
            foreach (var item in Monitors)
            {
                if (!flowLayoutPanel1.Controls.ContainsKey(settings[item.Camera].IP))
                {
                    var IV = new VievImage(settings[item.Camera], Settings.StaticMembers.ImageSettings.FPS);
                    IV.SizeMode = PictureBoxSizeMode.Zoom;
                    IV.Name = settings[item.Camera].IP;

                    IV.MouseMove += (o, e) =>
                    {
                        if (e.Button != MouseButtons.Left) return;

                        var PTZ = settings[item.Camera].GetPTZController();
                        if (!PTZ.IsSuported) return;

                        var dx = e.Location.X - lastPoint.X;
                        var dy = e.Location.Y - lastPoint.Y;
                        _ = (Math.Abs(dy) > Math.Abs(dx) ? dy > 0 ? PTZ.AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.UP) : PTZ.AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.DOWN) : dx > 0 ? PTZ.AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.LEFT) : PTZ.AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.RIGHT));
                        lastPoint = e.Location;
                    };

                    (IV as Control).KeyDown += Photo_KeyDown;

                    IV.MouseClick += (o, e) => { SelectedCamera = item.Camera; };
                    IV.MouseWheel += (o, e) => { var PTZ = settings[item.Camera].GetPTZController(); PTZ.MoveToHome(); };

                    IV.DoubleClick += (o, e) =>
                    {
                        var camera = (o as VievImage);
                        if (camera.SizeMode == PictureBoxSizeMode.Zoom) camera.SizeMode = PictureBoxSizeMode.StretchImage;
                        else camera.SizeMode = PictureBoxSizeMode.Zoom;
                    };
                    flowLayoutPanel1.Controls.Add(IV);
                }
                SizeOrient(flowLayoutPanel1.Controls[settings[item.Camera].IP] as VievImage);
            }
            flowLayoutPanel1.ResumeLayout();
        }

        private void SizeOrient(VievImage panel)
        {
            if (IsAutoOrient)
            {
                if (GroupV.Orientation.OrientationSensor.GetOrientationType(flowLayoutPanel1.Size) == GroupV.Orientation.OrientationType.Vertical) panel.Size = new Size((flowLayoutPanel1.ClientSize.Width) - Padding, (flowLayoutPanel1.ClientSize.Height / SizeSET) - Padding);
                else if (GroupV.Orientation.OrientationSensor.GetOrientationType(flowLayoutPanel1.Size) == GroupV.Orientation.OrientationType.Horizontal) panel.Size = new Size((flowLayoutPanel1.ClientSize.Width / SizeSET) - Padding, (flowLayoutPanel1.ClientSize.Height) - Padding);
            }
        }

        private void Photo_KeyDown(object sender, KeyEventArgs e)
        {
            var camera = settings[SelectedCamera].GetPTZController();
            if (!camera.IsSuported) return;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    camera.AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.UP);
                    break;
                case Keys.Down:
                    camera.AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.DOWN);
                    break;
                case Keys.Left:
                    camera.AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.LEFT);
                    break;
                case Keys.Right:
                    camera.AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.RIGHT);
                    break;
            }
        }

        private void MonitorVisible_Load(object sender, EventArgs e)
        {

        }

        private void MonitorVisible_Resize(object sender, EventArgs e)
        {
            SizeR();
        }
    }
}
