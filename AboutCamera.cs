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
    public partial class AboutCamera : Form
    {
        public AboutCamera(uint Selected)
        {
            InitializeComponent();

            var str = Structures.Load()[Selected];
            var di = str.GetONVIFController().DeviceInformation;
            textBox1.Text = di.Model;
            textBox2.Text = di.Version;
            textBox3.Text = di.SerialNumber;
            textBox4.Text = di.HardwareID;
            textBox5.Text = di.Other;
        }

        private void AboutCamera_Load(object sender, EventArgs e)
        {

        }

        private void проверитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var g = new SortedList<string, string>() { };
            g.Add("webMac", textBox3.Text + "**");
            //Console.WriteLine(Network.Downloading.SendRequest("https://www.xmeye.net/cloud_device_status_null", g));
        }
    }
}
