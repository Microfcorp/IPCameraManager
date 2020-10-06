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
    public partial class ImagePlayer : Form
    {
        public ImagePlayer()
        {
            InitializeComponent();
        }
        public ImagePlayer(Image img, string url = "") : this()
        {
            if (url == "") получитьURLToolStripMenuItem.Enabled = false;
            pictureBox1.Image = img;

            получитьURLToolStripMenuItem.Click += (o, e) => MessageBox.Show(url);
        }

        private void ImagePlayer_Load(object sender, EventArgs e)
        {

        }
    }
}
