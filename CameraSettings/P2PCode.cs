using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IPCamera.Network;
using QRCoder;

namespace IPCamera.CameraSettings
{   
    public partial class P2PCode : Form
    {
        Structures structures;
        public P2PCode(uint Selected)
        {
            InitializeComponent();
            structures = Structures.Load()[Selected];
        }

        private void UpdateSettings()
        {
            var Params = Downloading.GetP2P(structures.IP, structures.Name, structures.Password);
            textBox1.Text = Params["xqp2p_uid"];
            var QRCode = new QRCode(new QRCodeGenerator().CreateQrCode(textBox1.Text, QRCodeGenerator.ECCLevel.M));
            pictureBox1.Image = QRCode.GetGraphic(8, Color.Black, Color.White, Properties.Resources.camera, 15, 6, false);
        }

        private void P2PCode_Load(object sender, EventArgs e)
        {
            UpdateSettings();
        }

        private void сохранитьВФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog svf = new SaveFileDialog();
            svf.Filter = "Изображение (*.bmp)|*.bmp";
            svf.Title = "Сохранить QR-код P2P подключения";
            svf.FileName = "P2P QRCode - " + structures.IP;
            if (svf.ShowDialog() == DialogResult.OK)
                pictureBox1.Image.Save(svf.FileName);
        }
    }
}
