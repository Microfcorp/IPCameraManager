using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPCamera.UI.Maps
{
    public partial class MapsView : Form
    {
        MapsFile file;
        public MapsView(string filep)
        {
            InitializeComponent();
            file = new MapsFile(filep);
            pictureBox1.Image = file.LoadMainImage();
            foreach (var item in file.Manifest.Objects)
            {
                MapsObjectUI pb = new MapsObjectUI
                {
                    Images = file.LoadImage(item.Image),
                    Location = item.Point + new Size(32,32),
                    Size = new Size(32, 32)
                };
                toolTip1.SetToolTip(pb, item.Name);
                pictureBox1.Controls.Add(pb);
            }
        }

        private void MapsView_Load(object sender, EventArgs e)
        {

        }
    }
}
