using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera
{
    public partial class ImageV : Form
    {
        

        Structures structures;

        public int FPSMillis
        {
            get
            {
                return (int)(((float)1 / (float)trackBar1.Value) * 1000);
            }
        }
        string pathtophoto;
        public ImageV(uint Selected)
        {           
            InitializeComponent();

            structures = Structures.Load()[Selected];

            _btn1xScale = button1.Location.X / (float)this.Width;
            _btn1yScale = button1.Location.Y / (float)this.Height;

            _btn1xScale1 = trackBar1.Location.X / (float)this.Width;
            _btn1yScale1 = trackBar1.Location.Y / (float)this.Height;

            pathtophoto = structures.GetPhotoStreamSecondONVIF;
        }

        private void ImageV_Load(object sender, EventArgs e)
        {
            timer1.Interval = FPSMillis;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            timer1.Interval = FPSMillis;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (structures.TypeCamera == Network.Network.TypeCamera.Other) pictureBox1.Image = Network.Downloading.GetImageWitchAutorized(pathtophoto, structures.Login, structures.Password);
            else pictureBox1.ImageLocation = structures.GetPhotoStream;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var file = DateTime.Now.ToString().Replace(":", "-") + ".jpg";
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\IP Camera\\" + structures.IP);
            pictureBox1.Image.Save(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\IP Camera\\" + structures.IP + "\\" + file);
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

        private void ImageV_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }
    }
}
