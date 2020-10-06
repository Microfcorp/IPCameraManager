using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera.CameraSettings
{
    public partial class ONVIfImage : Form
    {
        Structures structures;
        private string pathtophoto;
        OIMG.ImagingSettings20 setimg;

        public ONVIfImage(uint Selected)
        {
            InitializeComponent();
            structures = Structures.Load()[Selected];
            setimg = structures.GetONVIFController().GetImagingSettings((uint)structures.SelectFirstProfile);
            vievImage1.Start(structures, 20);
        }

        private void ONVIfImage_Load(object sender, EventArgs e)
        {
            if (setimg == null)
            {
                Close();
                return;
            }
            numericUpDown2.Value = (decimal)setimg.Brightness;
            numericUpDown3.Value = (decimal)setimg.ColorSaturation;
            numericUpDown5.Value = (decimal)setimg.Contrast;
            numericUpDown4.Value = (decimal)setimg.Sharpness;
            radioButton4.Checked = setimg.BacklightCompensation.Mode == OIMG.BacklightCompensationMode.ON;
            radioButton5.Checked = setimg.BacklightCompensation.Mode == OIMG.BacklightCompensationMode.OFF;
            radioButton1.Checked = setimg.IrCutFilter == OIMG.IrCutFilterMode.AUTO;
            radioButton2.Checked = setimg.IrCutFilter == OIMG.IrCutFilterMode.ON;
            radioButton3.Checked = setimg.IrCutFilter == OIMG.IrCutFilterMode.OFF;
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
