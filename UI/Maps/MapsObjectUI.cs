using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPCamera.UI.Maps
{
    public partial class MapsObjectUI : UserControl
    {
        public Image Images
        {
            get
            {
                return pictureBox1.Image;
            }
            set
            {
                pictureBox1.Image = value;
            }
        }

        public new event EventHandler Click;

        public MapsObjectUI()
        {
            InitializeComponent();
        }
        public MapsObjectUI(Image image) : this()
        {
            Images = image;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Click?.Invoke(this, new EventArgs());
        }
    }
}
