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
        readonly SortedList<string, PictureBox> PanelCameras = new SortedList<string, PictureBox>();
        readonly SortedList<string, float> PanelCamerasScale = new SortedList<string, float>();

        bool IsAutoOrient = true;

        public int SizeSET
        {
            get => set.Length;
        }

        public new const int Padding = 10;
        public const int CountRunning = 50;

        public Photo()
        {
            InitializeComponent();
        }

        private void SizeOrient(PictureBox panel)
        {
            if (IsAutoOrient)
            {
                if (Orientation.OrientationSensor.GetOrientationType(flowLayoutPanel1.Size) == Orientation.OrientationType.Vertical) panel.Size = new Size((flowLayoutPanel1.ClientSize.Width) - Padding, (flowLayoutPanel1.ClientSize.Height / SizeSET) - Padding);
                else if (Orientation.OrientationSensor.GetOrientationType(flowLayoutPanel1.Size) == Orientation.OrientationType.Horizontal) panel.Size = new Size((flowLayoutPanel1.ClientSize.Width / SizeSET) - Padding, (flowLayoutPanel1.ClientSize.Height) - Padding);
            }
        }

        [Obsolete]
        public void ResizeF()
        {
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.Controls.Clear();

            for (int i = 0; i < set.Length; i++)
            {
                if (!PanelCameras.ContainsKey(set[i].IP + i))
                {
                    PanelCameras.Add(set[i].IP + i, new PictureBox());
                    PanelCamerasScale.Add(set[i].IP + i, 1f);
                    PanelCameras[set[i].IP + i].SizeMode = PictureBoxSizeMode.Zoom;
                    PanelCameras[set[i].IP + i].ImageLocation = set[i].GetPhotoStream;
                    //PanelCameras[set[i].IP + i].DragDrop += (o, e) => { Console.WriteLine(e.X); };
                    //PanelCameras[set[i].IP + i].DragEnter += (o, e) => { e.Effect = DragDropEffects.Copy; };
                    //PanelCameras[set[i].IP + i].MouseMove += (o, e) => { if (e.Button != MouseButtons.Left) return; (o as PictureBox).DoDragDrop(o, DragDropEffects.All); };
                    //PanelCameras[set[i].IP + i].MouseClick += (o, e) => { (o as PictureBox).Focus(); };
                    //var ia = i;
                    //PanelCameras[set[i].IP + i].MouseWheel += (o, e) => { Console.WriteLine(e.Delta / SystemInformation.MouseWheelScrollDelta); if (e.Delta / SystemInformation.MouseWheelScrollDelta > 0) PanelCamerasScale[set[ia].IP + ia]++; else if (e.Delta / SystemInformation.MouseWheelScrollDelta < 0) PanelCamerasScale[set[ia].IP + ia]--; var s = new Point((int)PanelCamerasScale[set[ia].IP + ia], (int)PanelCamerasScale[set[ia].IP + ia]); (o as PictureBox).Image.Resize(new Size(s)); };
                    //PanelCameras[set[i].IP + i]. += (o, e) => { if (e.Button != MouseButtons.Left) return; Console.WriteLine(e.Delta); if (e.Delta > 0) PanelCamerasScale[set[ia].IP + ia]++; else if (e.Delta < 0) PanelCamerasScale[set[ia].IP + ia]--; (o as PictureBox).Scale((int)PanelCamerasScale[set[ia].IP + ia]); };
                    PanelCameras[set[i].IP + i].DoubleClick += (o, e) =>
                    {
                        var camera = o as PictureBox;
                        if(camera.SizeMode == PictureBoxSizeMode.Zoom) camera.SizeMode = PictureBoxSizeMode.StretchImage;
                        else camera.SizeMode = PictureBoxSizeMode.Zoom;
                    };
                }                              

                SizeOrient(PanelCameras[set[i].IP + i]);              
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
            foreach (var item in PanelCameras.Values)
            {
                item.ImageLocation = item.ImageLocation;
            }
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
