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

namespace IPCamera.CameraSettings
{
    public partial class OSD : Form
    {
        Structures structures;
        public string PrintTime
        {
            get
            {
                return checkBox1.Checked ? "1" : "0";
            }
            set
            {
                checkBox1.Checked = value == "1" ? true : false;
            }
        }
        public string PrintName
        {
            get
            {
                return checkBox2.Checked ? "1" : "0";
            }
            set
            {
                checkBox2.Checked = value == "1" ? true : false;
            }
        }
        public string CName
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }

        public OSD(uint Selected)
        {
            structures = Structures.Load()[Selected];           
            InitializeComponent();                    
        }

        private void UpdateSettings()
        {
            var Params = Downloading.GetOSDParams(structures.IP, structures.Name, structures.Password);
            PrintTime = Params["show_0"];
            PrintName = Params["show_1"];
            CName = Params["name_1"];
        }

        private void Save()
        {
            SortedList<string, string> sending = new SortedList<string, string>();
            sending.Add("cmd", "setoverlayattr");
            sending.Add("cururl", "http://" + structures.URLToHTTPPort + "web/osd.html");
            sending.Add("-region", "0");
            sending.Add("-show", PrintTime);
            sending.Add("-place", "0");
            var resp = (Downloading.SendRequest(DownloadingPaths.ToPath(structures.IP) + DownloadingPaths.DeviceParam, structures.Name, structures.Password, sending));
            sending.Clear();
            sending.Add("cmd", "setoverlayattr");
            sending.Add("-region", "1");
            sending.Add("-show", PrintName);
            sending.Add("-name", CName);
            sending.Add("-place", "0");
            resp = (Downloading.SendRequest(DownloadingPaths.ToPath(structures.IP) + DownloadingPaths.DeviceParam, structures.Name, structures.Password, sending));

            UpdateSettings();
        }

        private void OSD_Load(object sender, EventArgs e)
        {
            UpdateSettings();
            vievImage1.URL = structures.GetPhotoStream;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Save();
        }
    }
}
