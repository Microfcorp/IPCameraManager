using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera
{
    public partial class ImageV : Form
    {
        public const string Photo = "http://{0}:{1}/web/auto.jpg?-usr={2}&-pwd={3}&";

        Structures structures = Structures.Load();

        public int FPSMillis
        {
            get
            {
                return (int)(((float)1 / (float)trackBar1.Value) * 1000);
            }
        }
        public ImageV()
        {
            InitializeComponent();
            _btn1xScale = button1.Location.X / (float)this.Width;
            _btn1yScale = button1.Location.Y / (float)this.Height;

            _btn1xScale1 = trackBar1.Location.X / (float)this.Width;
            _btn1yScale1 = trackBar1.Location.Y / (float)this.Height;
        }

        private void ImageV_Load(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            timer1.Interval = FPSMillis;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.ImageLocation = (String.Format(Photo, structures.IP, structures.HTTPPort, structures.Name, structures.Password));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Save(DateTime.Now.ToString().Replace(":", "-") + ".jpg");
        }

        // X, Y scaling variables for btn1
        private float _btn1xScale;
        private float _btn1yScale;
        // X, Y scaling variables for btn1
        private float _btn1xScale1;
        private float _btn1yScale1;

        private void ImageV_ResizeEnd(object sender, EventArgs e)
        {
            
        }

        private void ImageV_Resize(object sender, EventArgs e)
        {
            //button1.Location = new Point(Size.Width - (Size.Width - button1.Location.X), Size.Height - (Size.Height - button1.Location.Y));
            // adjust position based on 
            button1.Location = new Point(
                (int)(this.Width * _btn1xScale),
                (int)(this.Height * _btn1yScale));
            // adjust position based on 
            trackBar1.Location = new Point(
                (int)(this.Width * _btn1xScale1),
                (int)(this.Height * _btn1yScale1));

            button1.Size = new Size(Size.Width - 40, button1.Size.Height);
            trackBar1.Size = new Size(Size.Width - 40, trackBar1.Size.Height);
        }
    }
}
