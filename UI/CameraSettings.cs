using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QRCoder;
using IPCamera.Network;
using System.ServiceModel.Discovery;
using System.Xml;
using System.IO;

namespace IPCamera.UI
{
    public partial class CameraSettings : Form
    {
        Structures conf;
        uint select;

        SortedList<string, int> ONVIFPORTS = new SortedList<string, int>();
        public CameraSettings()
        {
            InitializeComponent();
        }

        public CameraSettings(uint Selected) : this()
        {
            select = Selected;
            conf = Structures.Load()[Selected];
            Text = "Настройки камеры - " + conf.NameCamera;
        }
        string GetSize(ServiceReference1.Profile p)
        {
            return " (" + p.VideoEncoderConfiguration.Resolution.Width + "x" + p.VideoEncoderConfiguration.Resolution.Height + ")";
        }
        void LoadVideo()
        {
            var profiles = conf.GetONVIFController().Profiles.Select(tmp => tmp.Name.Trim('"') + " " + GetSize(tmp)).ToArray();
            comboBox3.Items.AddRange(profiles);
            comboBox4.Items.AddRange(profiles);

            comboBox3.SelectedIndex = conf.SelectFirstProfile;
            comboBox4.SelectedIndex = conf.SelectSecondProfile;
            comboBox7.SelectedIndex = (int)conf.SinglePlay;
            comboBox6.SelectedIndex = (int)conf.GroupPlay;
            comboBox8.SelectedIndex = (int)conf.MonitorPlay;
        }
        void SaveVideo()
        {
            conf.SelectFirstProfile = comboBox3.SelectedIndex;
            conf.SelectSecondProfile = comboBox4.SelectedIndex;
            conf.SinglePlay = (TypeViewers)comboBox7.SelectedIndex;
            conf.GroupPlay = (TypeViewers)comboBox6.SelectedIndex;
            conf.MonitorPlay = (TypeViewers)comboBox8.SelectedIndex;
        }
        void LoadONVIF()
        {
            textBox10.Text = conf.GetONVIFController().Version.ToString();
            dateTimePicker1.Value = conf.GetONVIFController().CameraDateTime.DateTime;
        }
        void LoadHI3510()
        {
            var Params = Downloading.GetP2P(conf.IP, conf.Name, conf.Password);
            if (Params.Count < 1) { tabControl1.TabPages.Remove(tabPage4); return; }
            textBox11.Text = Params["xqp2p_uid"];
            var QRCode = new QRCode(new QRCodeGenerator().CreateQrCode(textBox1.Text, QRCodeGenerator.ECCLevel.M));
            pictureBox1.Image = QRCode.GetGraphic(8, Color.Black, Color.White, Properties.Resources.camera, 15, 6, false);
        }
        void LoadPTZ()
        {
            checkBox1.Checked = conf.PTZ;
        }
        void SavePTZ()
        {
            conf.PTZ = checkBox1.Checked;
        }
        void LoadAbout()
        {
            textBox1.Text = conf.NameCamera;
            textBox3.Text = conf.GetONVIFController().Hostname;           
            comboBox2.SelectedIndex = (int)conf.TypeCamera;
            if (conf.TypeCamera != Network.Network.TypeCamera.HI3510) tabControl1.TabPages.Remove(tabPage4);
            else LoadHI3510();
            var devid = conf.GetONVIFController().DeviceInformation;
            textBox4.Text = devid.Model;
            textBox5.Text = devid.Version;
            textBox6.Text = devid.SerialNumber;
            textBox7.Text = devid.HardwareID;
            textBox8.Text = devid.Other;
            textBox9.Text = conf.GetMAC;         
        }

        void SaveAbout()
        {
            conf.NameCamera = textBox1.Text;
            try
            {
                conf.GetONVIFController().Hostname = textBox3.Text;
            }
            catch { Console.WriteLine("Ошибка записи Hostname"); }
            conf.TypeCamera = (Network.Network.TypeCamera)comboBox2.SelectedIndex;
        }

        void LoadNetworkSettings() //сделать автопоиск
        {
            comboBox5.Text = conf.IP;
            numericUpDown1.Value = conf.HTTPPort;
            numericUpDown2.Value = conf.ONVIFPort;
            numericUpDown3.Value = conf.RTSPPort;
            comboBox1.Text = conf.Name;
            textBox2.Text = conf.Password;
            comboBox1.Items.AddRange(conf.GetONVIFController().Users.Select(tmp => tmp.Username).ToArray());
        }

        void SaveNetworkSettings() //сделать автопоиск
        {
            conf.IP = comboBox5.Text;
            conf.HTTPPort = (uint)(numericUpDown1.Value);
            conf.ONVIFPort = (uint)(numericUpDown2.Value);
            conf.RTSPPort = (uint)(numericUpDown3.Value);
            conf.Name = comboBox1.Text;
            conf.Password = textBox2.Text;
            conf.MapsFile = Application.StartupPath + Maps.MapsFile.MapsDirPrefix + comboBox9.Text;
        }

        void SaveStructures()
        {
            var s = Structures.Load();
            s[select] = conf;
            Structures.Save(s);
        }

        void SaveSettings()
        {
            SaveNetworkSettings();
            SaveAbout();
            SavePTZ();
            SaveVideo();

            SaveStructures();
        }

        void LoadSettings()
        {
            LoadNetworkSettings(); //загрузка сетевых настроек
            LoadAbout();
            LoadPTZ();
            LoadONVIF();
            LoadVideo();
        }

        void LoadMapsFiles()
        {
            comboBox9.Items.Clear();
            comboBox9.Items.AddRange(Maps.MapsFile.GetMapsFileName());
            comboBox9.Text = Path.GetFileName(conf.MapsFile);
        }

        private void CameraSettings_Load(object sender, EventArgs e)
        {
            SearchONVIF();
            LoadSettings();
            LoadMapsFiles();
        }

        private void CameraSettings_FormClosing(object sender, FormClosingEventArgs e)
        {           
            var d = MessageBox.Show("Сохранить настройки?", "Microf IP Camera Manager", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (d == DialogResult.Yes)
            {
                SaveSettings();
            }
            else if (d == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveNetworkSettings();
            LoadSettings();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            conf.GetPTZController().Stop();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            conf.GetPTZController().SetHome();
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            SaveFileDialog svf = new SaveFileDialog();
            svf.Filter = "Изображение (*.bmp)|*.bmp";
            svf.Title = "Сохранить QR-код P2P подключения";
            svf.FileName = "P2P QRCode - " + conf.NameCamera;
            if (svf.ShowDialog() == DialogResult.OK)
                pictureBox1.Image.Save(svf.FileName);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            (new IPCamera.CameraSettings.Email(select)).OpenToWindowManager((MainForm)Application.OpenForms[0]);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            (new IR(select)).OpenToWindowManager((MainForm)Application.OpenForms[0]);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            (new IPCamera.CameraSettings.OSD(select)).OpenToWindowManager((MainForm)Application.OpenForms[0]);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            conf.GetONVIFController().SetCameraTime(ODEV.SetDateTimeType.Manual, conf.GetONVIFController().CameraDateTime.TZ, DateTime.Now);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            conf.GetONVIFController().SetCameraTime(ODEV.SetDateTimeType.NTP, null, DateTime.Now);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            (new InfoCamera.CameraProtocols(select)).OpenToWindowManager((MainForm)Application.OpenForms[0]);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            (new IPCamera.CameraSettings.ONVIfImage(select)).OpenToWindowManager((MainForm)Application.OpenForms[0]);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            (new InfoCamera.CapCamera(select)).OpenToWindowManager((MainForm)Application.OpenForms[0]);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            conf.GetONVIFController().Reboot(true);
            MessageBox.Show("Запрос на перезагрузку отправлен");
        }

        private void button13_Click(object sender, EventArgs e)
        {
            (new Record.ManagerRecords(select)).OpenToWindowManager((MainForm)Application.OpenForms[0]);
        }

        private void SearchONVIF()
        {
            if (backgroundWorker1.IsBusy) backgroundWorker1.CancelAsync();
            backgroundWorker1.RunWorkerAsync();
        }

        void _SearchONVIF()
        {
            var endPoint = new UdpDiscoveryEndpoint(DiscoveryVersion.WSDiscoveryApril2005);

            var discoveryClient = new DiscoveryClient(endPoint);

            discoveryClient.FindProgressChanged += discoveryClient_FindProgressChanged;

            FindCriteria findCriteria = new FindCriteria
            {
                Duration = new TimeSpan(0, 0, 10),
                MaxResults = int.MaxValue
            };
            // Edit: optionally specify contract type, ONVIF v1.0
            findCriteria.ContractTypeNames.Add(new XmlQualifiedName("NetworkVideoTransmitter",
                "http://www.onvif.org/ver10/network/wsdl"));

            discoveryClient.FindAsync(findCriteria);
            comboBox1.Invoke(new Action(() => { comboBox1.Items.Clear(); }));
        }

        void discoveryClient_FindProgressChanged(object sender, FindProgressChangedEventArgs e)
        {
            try
            {
                for (int i = 0; i < e.EndpointDiscoveryMetadata.ListenUris.Count; i++)
                {
                    comboBox5.Invoke(new Action(() => { if (!comboBox5.Items.Contains(e.EndpointDiscoveryMetadata.ListenUris[i].Host)) comboBox5.Items.Add(e.EndpointDiscoveryMetadata.ListenUris[i].Host); }));                  
                    ONVIFPORTS.Add(e.EndpointDiscoveryMetadata.ListenUris[i].Host, e.EndpointDiscoveryMetadata.ListenUris[i].Port, true);
                }
            }
            catch { }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            _SearchONVIF();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SearchONVIF();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ONVIFPORTS.ContainsKey(comboBox5.Text)) 
                numericUpDown2.Value = ONVIFPORTS[comboBox5.Text];
        }

        private void button15_Click(object sender, EventArgs e)
        {
            var protocols = conf.GetONVIFController().NetworkProtocols;
            numericUpDown2.Value = ONVIFPORTS[comboBox5.Text];
            numericUpDown1.Value = (uint)protocols.Where(tmp => tmp.Name == ODEV.NetworkProtocolType.HTTP).FirstOrDefault().Port.FirstOrDefault();
            numericUpDown3.Value = (uint)protocols.Where(tmp => tmp.Name == ODEV.NetworkProtocolType.RTSP).FirstOrDefault().Port.FirstOrDefault();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Maps.MapsFile.NewMapsFile(comboBox5.Text);
            LoadMapsFiles();
        }

        private void button16_Click_1(object sender, EventArgs e)
        {
            Maps.MapsFile.DeleteMapsFile(Application.StartupPath + Maps.MapsFile.MapsDirPrefix + comboBox9.Text);
            conf.MapsFile = null;
            LoadMapsFiles();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            new Maps.MapsEdit(Application.StartupPath + Maps.MapsFile.MapsDirPrefix + comboBox9.Text).Show();
        }
    }
}
