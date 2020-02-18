using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera
{
    public partial class VievImage : UserControl
    {
        private float fPS;

        public string URL
        {
            get;
            set;
        }

        public float FPS
        {
            get => fPS;
            set { fPS = (int)((float)(1 / value) * 1000); timer1.Interval = (int)((float)(1 / value) * 1000); }
        }
        public VievImage()
        {
            InitializeComponent();
            FPS = 15;
        }
        public VievImage(string path, float FPS = 25f) : this()
        {
            this.FPS = FPS;
            timer1.Start();
            URL = path;
            pictureBox1.ImageLocation = URL;
        }

        protected void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.ImageLocation = URL;
            //pictureBox1.Refresh();
        }
    }
}
