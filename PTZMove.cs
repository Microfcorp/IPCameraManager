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
    public partial class PTZMove : Form
    {
        Structures structures;
        public PTZMove(uint Selected)
        {
            InitializeComponent();
            structures = Structures.Load()[Selected];
            vievImage1.Start(structures, 10);
            vievImage1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void PTZMove_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (structures.GetPTZController().IsSuported)
                structures.GetPTZController().AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.UP);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (structures.GetPTZController().IsSuported)
                structures.GetPTZController().AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.LEFT);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (structures.GetPTZController().IsSuported)
                structures.GetPTZController().AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.DOWN);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (structures.GetPTZController().IsSuported)
                structures.GetPTZController().AsyncCountiniousMove(ONVIF.PTZ.PTZParameters.Vector.RIGHT);
        }
    }
}
