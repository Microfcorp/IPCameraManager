using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IPCamera.Network;
using IPCamera.Settings;

namespace IPCamera
{
    public partial class IR : Form
    {
        Structures structures;

        /// <summary>
        /// Режим работы ИК
        /// </summary>
        public string IRStat
        {
            get
            {
                if (radioButton1.Checked) return "auto";
                else if (radioButton2.Checked) return "open";
                else return "close";
            }
            set
            {
                if ("auto" == value) radioButton1.Checked = true;
                else if ("open" == value) radioButton2.Checked = true;
                else radioButton3.Checked = true;
            }
        }
        /// <summary>
        /// Расположение камеры
        /// </summary>
        public string AEMode
        {
            get
            {
                //aemode
                if (radioButton4.Checked) return "0";
                else if (radioButton5.Checked) return "1";
                else return "2";
            }
            set
            {
                if (value == "0") radioButton4.Checked = true;
                else if (value == "0") radioButton5.Checked = true;
                else radioButton6.Checked = true;
            }
        }
        /// <summary>
        /// Режим работы ночного видения
        /// </summary>
        public string LampMode
        {
            get
            {
                return comboBox1.SelectedIndex.ToString();
            }
            set
            {
                comboBox1.SelectedIndex = int.Parse(value);
            }
        }
        /// <summary>
        /// Яркость изображения
        /// </summary>
        public string Brightness
        {
            get
            {
                return numericUpDown2.Value.ToString();
            }
            set
            {
                numericUpDown2.Value = int.Parse(value);
            }
        }
        /// <summary>
        /// Насыщенность изображения
        /// </summary>
        public string Saturation
        {
            get
            {
                return numericUpDown3.Value.ToString();
            }
            set
            {
                numericUpDown3.Value = int.Parse(value);
            }
        }
        /// <summary>
        /// Четкость изображения
        /// </summary>
        public string Sharpness
        {
            get
            {
                return numericUpDown4.Value.ToString();
            }
            set
            {
                numericUpDown4.Value = int.Parse(value);
            }
        }
        /// <summary>
        /// Контрасность изображения
        /// </summary>
        public string Contrast
        {
            get
            {
                return numericUpDown5.Value.ToString();
            }
            set
            {
                numericUpDown5.Value = int.Parse(value);
            }
        }
        /// <summary>
        /// Поворот изображения
        /// </summary>
        public string Flip
        {
            get
            {
                return checkBox1.Checked ? "on" : "off";
            }
            set
            {
                checkBox1.Checked = value == "on" ? true : false;
            }
        }
        /// <summary>
        /// Зеркальность изображения
        /// </summary>
        public string Mirror
        {
            get
            {
                return checkBox2.Checked ? "on" : "off";
            }
            set
            {
                checkBox2.Checked = value == "on" ? true : false;
            }
        }
        /// <summary>
        /// Широкий динамический диапазон
        /// </summary>
        public string WDR
        {
            get
            {
                return checkBox4.Checked ? "on" : "off";
            }
            set
            {
                checkBox4.Checked = value == "on" ? true : false;
            }
        }
        /// <summary>
        /// Ночной режим
        /// </summary>
        public string Night
        {
            get
            {
                return checkBox3.Checked ? "on" : "off";
            }
            set
            {
                checkBox3.Checked = value == "on" ? true : false;
            }
        }
        /// <summary>
        /// Оттенок изображения
        /// </summary>
        public string Hue
        {
            get
            {
                return numericUpDown6.Value.ToString();
            }
            set
            {
                numericUpDown6.Value = int.Parse(value);
            }
        }


        public IR(uint Selected)
        {
            InitializeComponent();
            structures = Structures.Load()[Selected];
        }

        private void IR_Load(object sender, EventArgs e)
        {

            //Чтение
            //var lancode="1"; var display_mode="0"; var brightness="64"; var saturation="0"; var sharpness="64"; var contrast="24"; 
            //var hue="50"; var wdr="off"; var night="on"; var shutter="65535"; var flip="off"; var mirror="off"; var gc="30"; var ae="4"; 
            //var targety="64"; var noise="1"; var gamma="1"; var aemode="0"; var imgmode="0"; 
            //var name0="admin"; var password0="admin"; var authLevel0="15"; var imagesize="1080P"; var audioflag="1"; 
            //var model="C9F0SgZ3N0P8L0"; var hardVersion="V1.0.0.1"; var softVersion="V13.1.37.6.3-20190708"; var webVersion="V1.0.1"; 
            //var name="IPCAM"; var startdate="2020-01-03 20:15:42"; var upnpstatus="off"; var facddnsstatus="off"; 
            //var th3ddnsstatus="off"; var platformstatus="0"; var sdstatus="Ready"; var sdfreespace="6837312"; 
            //var sdtotalspace="31252928"; var videomode="41"; var vinorm="P"; var profile="1"; var maxchn="2"; var saradc_switch_value="500"; 
            //var saradc_b2c_switch_value="300"; var infraredstat="auto"; var rtmpport="1935"; var lamp_mode_flag="2"; var lamp_mode="0"; var lamp_timeout="30";
            //Запись
            //
            //cmd: setinfrared
            //cururl: http://192.168.1.34/web/display.html
            //   -infraredstat: auto
            //    cmd: setircutattr
            //    - saradc_switch_value: 500
            //cmd: setlampattrex
            //- lamp_mode: 0
            //cmd: setimageattr
            //- brightness: 64
            // - contrast: 24
            //  - saturation: 0
            //   - sharpness: 64
            //    - mirror: off
            //    - flip: off
            //     - shutter: 65535
            //       - night: on
            //        - wdr: off
            //         - noise: on
            //          - gc: 30
            //           - ae: 4
            //            - targety: 64
            //             - aemode: 0
            //              - image_type: 0
            //               - imgmode: 0

            UpdateParams();
        }

        string dm;

        private void UpdateParams()
        {
            var Params = Downloading.GetImageParams(structures.IP, structures.Name, structures.Password);
            numericUpDown1.Value = decimal.Parse(Params["saradc_switch_value"]);
            IRStat = Params["infraredstat"];
            AEMode = Params["aemode"];
            LampMode = Params["lamp_mode"];
            Brightness = Params["brightness"];
            Saturation = Params["saturation"];
            Contrast = Params["contrast"];
            Sharpness = Params["sharpness"];
            WDR = Params["wdr"];
            Flip = Params["flip"];
            Mirror = Params["mirror"];
            Night = Params["night"];
            Hue = Params["hue"];
            dm = Params["display_mode"];
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.ImageLocation = structures.GetPhotoStream;
        }

        private void Save()
        {
            SortedList<string, string> sending = new SortedList<string, string>();
            sending.Add("cmd", "setinfrared");
            sending.Add("cururl", "http://" + structures.URLToHTTPPort + "web/display.html");
            sending.Add("-infraredstat", IRStat);
            var resp = (Downloading.SendRequest(DownloadingPaths.ToPath(structures.IP) + DownloadingPaths.DeviceParam, structures.Name, structures.Password, sending));
            sending.Clear();
            sending.Add("cmd", "setircutattr");
            sending.Add("-saradc_switch_value", numericUpDown1.Value.ToString());
            resp = (Downloading.SendRequest(DownloadingPaths.ToPath(structures.IP) + DownloadingPaths.DeviceParam, structures.Name, structures.Password, sending));
            sending.Clear();
            sending.Add("cmd", "setlampattrex");
            sending.Add("-lamp_mode", LampMode);
            resp = (Downloading.SendRequest(DownloadingPaths.ToPath(structures.IP) + DownloadingPaths.DeviceParam, structures.Name, structures.Password, sending));
            sending.Clear();
            sending.Add("cmd", "setimageattr");           
            sending.Add("-brightness", Brightness); 
            sending.Add("-contrast", Contrast); 
            sending.Add("-saturation", Saturation); 
            sending.Add("-sharpness", Sharpness); 
            sending.Add("-mirror", Mirror); 
            sending.Add("-flip", Flip); 
            sending.Add("-shutter", "65535"); 
            sending.Add("-night", Night); 
            sending.Add("-wdr", WDR); 
            sending.Add("-noise", "on"); 
            sending.Add("-gc", "30"); 
            sending.Add("-ae", "4"); 
            sending.Add("-targety", "64"); 
            sending.Add("-aemode", AEMode); 
            sending.Add("-image_type", dm); 
            sending.Add("-imgmode", "0"); 
            resp = (Downloading.SendRequest(DownloadingPaths.ToPath(structures.IP) + DownloadingPaths.DeviceParam, structures.Name, structures.Password, sending));
            //MessageBox.Show(resp);
            UpdateParams();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
