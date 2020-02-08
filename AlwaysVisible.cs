using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IPCamera.DLL;

namespace IPCamera
{
    public partial class AlwaysVisible : Form
    {
        Structures structures;
        private bool isDown;

        public AlwaysVisible(uint sel)
        {
            InitializeComponent();
            structures = Structures.Load()[sel];
            MouseWheel += AlwaysVisible_MouseWheel;
        }

        private void AlwaysVisible_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta == 120) Size += new Size(10,10);
            else if (e.Delta == -120) Size -= new Size(10, 10);
        }

        private void AlwaysVisible_Load(object sender, EventArgs e)
        {
            Handle.SetWindowPos(new IntPtr(CommonFunctions.HWND_TOPMOST),
            0, 0, 0, 0,
            CommonFunctions.SWP_NOMOVE | CommonFunctions.SWP_NOSIZE);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.ImageLocation = structures.GetPhotoStream;
        }

        private int x = 0;
        private int y = 0;
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //if(e.Button == MouseButtons.Left)
            //    this.Location = PointToScreen(e.Location);

            if (isDown)
            {
                Point pos = new Point((e.Location.X - x), (e.Location.Y - y));
                Location = PointToScreen(pos);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isDown = false;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDown = true;
                x = (DesktopLocation).X;
                y = (DesktopLocation).Y;
            }
        }
    }
}
