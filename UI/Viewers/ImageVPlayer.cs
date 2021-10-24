using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPCamera.UI.Viewers
{
    public partial class ImageVPlayer : UserControl
    {
        /// <summary>
        /// Возвращает или задает путь к URL
        /// </summary>
        public string ImageURL
        {
            get
            {
                return pictureBox1.ImageLocation;
            }
            set
            {
                pictureBox1.ImageLocation = value;
            }
        }
        /// <summary>
        /// Возвращет или задает количетво кадров в векунду
        /// </summary>
        public byte FPS
        {
            get
            {
                return (byte)(1000 / timer1.Interval);
            }
            set
            {
                timer1.Interval = 1000 / value;
            }
        }
        public ImageVPlayer()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.ImageLocation = ImageURL;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void ImageVPlayer_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}
