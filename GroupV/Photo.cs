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
    public partial class Photo : Form
    {
        readonly Settings.Structures[] set = Settings.Structures.Load();
        readonly SortedList<string, VievImage> PanelCameras = new SortedList<string, VievImage>();
        readonly SortedList<string, float> PanelCamerasScale = new SortedList<string, float>();
        readonly List<string> CameraIP = new List<string>();

        bool IsAutoOrient = true;
        private Point lastPoint;

        public int SizeSET
        {
            get => set.Length;
        }

        public new int Padding = Settings.StaticMembers.ImageSettings.Padding;

        public Photo()
        {
            InitializeComponent();
            ControlAdded += (o, e) =>
            {
                (o as Control).KeyDown += Photo_KeyDown;
            };
        }

        private void SizeOrient(VievImage panel)
        {
            if (IsAutoOrient)
            {
                if (Orientation.OrientationSensor.GetOrientationType(flowLayoutPanel1.Size) == Orientation.OrientationType.Vertical) panel.Size = new Size((flowLayoutPanel1.ClientSize.Width) - Padding, (flowLayoutPanel1.ClientSize.Height / SizeSET) - Padding);
                else if (Orientation.OrientationSensor.GetOrientationType(flowLayoutPanel1.Size) == Orientation.OrientationType.Horizontal) panel.Size = new Size((flowLayoutPanel1.ClientSize.Width / SizeSET) - Padding, (flowLayoutPanel1.ClientSize.Height) - Padding);
            }
        }

        int SelectedCamera = 0;

        public void ResizeF()
        {
            flowLayoutPanel1.SuspendLayout();
            //flowLayoutPanel1.Controls.Clear();

            for (int i = 0; i < set.Length; i++)
            {
                if (!PanelCameras.ContainsKey(set[i].IP + i))
                {
                    PanelCameras.Add(set[i].IP + i, new VievImage(set[i], Settings.StaticMembers.ImageSettings.FPS));
                    PanelCamerasScale.Add(set[i].IP + i, 1f);
                    PanelCameras[set[i].IP + i].SizeMode = PictureBoxSizeMode.Zoom;
                    PanelCameras[set[i].IP + i].Name = set[i].IP + i;
                    //PanelCameras[set[i].IP + i].ImageLocation = Network.Downloading.GetImageWitchAutorized(set[i].GetPhotoStreamSecondONVIF, set[i].Login, set[i].Password);
                    CameraIP.Add(set[i].IsActive ? set[i].GetPhotoStreamSecondONVIF : "", true);
                    //PanelCameras[set[i].IP + i].DragDrop += (o, e) => { Console.WriteLine(e.X); };
                    //PanelCameras[set[i].IP + i].DragEnter += (o, e) => { e.Effect = DragDropEffects.Copy; };
                    var ia = i;
                    PanelCameras[set[i].IP + i].MouseMove += (o, e) => 
                    { 
                        if (e.Button != MouseButtons.Left) return;

                        var PTZ = set[ia].GetPTZController();
                        if (!PTZ.IsSuported) return;

                        var dx = e.Location.X - lastPoint.X;
                        var dy = e.Location.Y - lastPoint.Y;
                        _ = (Math.Abs(dy) > Math.Abs(dx) ? dy > 0 ? PTZ.AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.UP) : PTZ.AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.DOWN) : dx > 0 ? PTZ.AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.LEFT) : PTZ.AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.RIGHT));
                        lastPoint = e.Location;
                    };

                    PanelCameras[set[i].IP + i].MouseClick += (o, e) => { SelectedCamera = ia; };
                    //var ia = i;
                    PanelCameras[set[i].IP + i].MouseWheel += (o, e) => { var PTZ = set[ia].GetPTZController(); PTZ.MoveToHome(); };
                    //PanelCameras[set[i].IP + i]. += (o, e) => { if (e.Button != MouseButtons.Left) return; Console.WriteLine(e.Delta); if (e.Delta > 0) PanelCamerasScale[set[ia].IP + ia]++; else if (e.Delta < 0) PanelCamerasScale[set[ia].IP + ia]--; (o as PictureBox).Scale((int)PanelCamerasScale[set[ia].IP + ia]); };
                    PanelCameras[set[i].IP + i].DoubleClick += (o, e) =>
                    {
                        var camera = (o as VievImage);
                        if(camera.SizeMode == PictureBoxSizeMode.Zoom) camera.SizeMode = PictureBoxSizeMode.StretchImage;
                        else camera.SizeMode = PictureBoxSizeMode.Zoom;
                    };
                }                              

                SizeOrient(PanelCameras[set[i].IP + i]);  
                if(!flowLayoutPanel1.Controls.ContainsKey(set[i].IP + i))
                    flowLayoutPanel1.Controls.Add(PanelCameras[set[i].IP + i]);
            }
            flowLayoutPanel1.ResumeLayout();
        }

        private void Photo_Load(object sender, EventArgs e)
        {
            ResizeF();
        }        

        private void timer1_Tick(object sender, EventArgs e)
        {
            /*foreach (var item in PanelCameras.Values)
            {
                item.ImageLocation = item.ImageLocation;
            }*/
            /*for (int i = 0; i < PanelCameras.Count; i++)
            {
                if (set[i].TypeCamera == Network.Network.TypeCamera.Other) PanelCameras.ElementAt(i).Value.Image = Network.Downloading.GetImageWitchAutorized(CameraIP[i], set[i].Login, set[i].Password);
                else PanelCameras.ElementAt(i).Value.ImageLocation = set[i].GetPhotoStream;
                PanelCameras.ElementAt(i).Value.Update();
            }*/
        }

        private void Photo_Resize(object sender, EventArgs e)
        {
            ResizeF();
        }

        private void Photo_DoubleClick(object sender, EventArgs e)
        {
            IsAutoOrient = !IsAutoOrient;
            Text = "Групповой просмотр в фото режиме - " + (IsAutoOrient ? "Автоматическая ориентация" : "Фиксированная ориентация");
        }

        private void Photo_KeyDown(object sender, KeyEventArgs e)
        {
            var camera = set[SelectedCamera].GetPTZController();
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
    }
    public abstract class move
    {

        static bool isPress = false;
        static Point startPst;
        /// <summary>
        /// Функция выполняется при нажатии на перемещаемый контрол
        /// </summary>
        /// <param name="sender">контролл</param>
        /// <param name="e">событие мышки</param>
        private static void mDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) return;//проверка что нажата левая кнопка
            isPress = true;
            startPst = e.Location;
        }
        /// <summary>
        /// Функция выполняется при отжатии перемещаемого контрола
        /// </summary>
        /// <param name="sender">контролл</param>
        /// <param name="e">событие мышки</param>
        private static void mUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) return;//проверка что нажата левая кнопка
            isPress = false;
        }
        /// <summary>
        /// Функция выполняется при перемещении контрола
        /// </summary>
        /// <param name="sender">контролл</param>
        /// <param name="e">событие мышки</param>
        private static void mMove(object sender, MouseEventArgs e)
        {
            if (isPress)
            {
                Control control = (Control)sender;
                control.Top += e.Y - startPst.Y;
                control.Left += e.X - startPst.X;
            }
        }
        /// <summary>
        /// обучает контролы передвигаться
        /// </summary>
        /// <param name="sender">контролл(это может быть кнопка, лейбл, календарик и.т.д)</param>
        public static void LearnToMove(object sender)
        {
            Control control = (Control)sender;
            control.MouseDown += new MouseEventHandler(mDown);
            control.MouseUp += new MouseEventHandler(mUp);
            control.MouseMove += new MouseEventHandler(mMove);
        }
    }
}
