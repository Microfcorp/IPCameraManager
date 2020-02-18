using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using IPCamera.DLL;

namespace IPCamera
{
    public partial class Main : Form
    {
        private string IP
        {
            get
            {
                return comboBox1.Text;
            }
            set
            {
                comboBox1.Text = value;
            }
        }
        private string UserName
        {
            get
            {
                return textBox2.Text;
            }
            set
            {
                textBox2.Text = value;
            }
        }
        private string Password
        {
            get
            {
                return textBox3.Text;
            }
            set
            {
                textBox3.Text = value;
            }
        }
        private uint HTTPPort
        {
            get
            {
                return (uint)numericUpDown1.Value;
            }
            set
            {
                numericUpDown1.Value = value;
            }
        }
        private uint RTSPPort
        {
            get
            {
                return (uint)numericUpDown2.Value;
            }
            set
            {
                numericUpDown2.Value = value;
            }
        }
        private uint ONVIFPort
        {
            get
            {
                return (uint)numericUpDown3.Value;
            }
            set
            {
                numericUpDown3.Value = value;
            }
        }

        public Network.Network.TypeCamera GetTypeCamera
        {
            get
            {
                if (radioButton1.Checked) return Network.Network.TypeCamera.HI3510;
                else if (radioButton2.Checked) return Network.Network.TypeCamera.HI3518;
                else return Network.Network.TypeCamera.Other;
            }
            set
            {
                if (value == Network.Network.TypeCamera.HI3510)
                {
                    radioButton1.Checked = true;
                    загрузкаФайловToolStripMenuItem.Visible = true;
                    логиToolStripMenuItem.Visible = true;
                    настройкиToolStripMenuItem.Visible = true;
                }
                else
                {
                    if (value == Network.Network.TypeCamera.Other) radioButton3.Checked = true;
                    if (value == Network.Network.TypeCamera.HI3518) radioButton2.Checked = true;
                    загрузкаФайловToolStripMenuItem.Visible = false;
                    логиToolStripMenuItem.Visible = false;
                    настройкиИзображенияToolStripMenuItem.Visible = false;
                }

            }
        }

        List<ToolStripMenuItem> Items = new List<ToolStripMenuItem>();
        SortedList<string, int> ONVIFPORTS = new SortedList<string, int>();

        private uint _selected = 0;

        public uint Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (value >= Structures.Load().Length) return;
                var structures = Structures.Load()[value];
                IP = structures.IP;
                UserName = structures.Name;
                Password = structures.Password;
                HTTPPort = structures.HTTPPort;
                RTSPPort = structures.RTSPPort;
                ONVIFPort = structures.ONVIFPort;
                GetTypeCamera = structures.TypeCamera;

                Items.Where(tmp => tmp.Name == value.ToString()).ToList().ForEach(tmp => tmp.Checked = true);
                Items.Where(tmp => tmp.Name != value.ToString()).ToList().ForEach(tmp => tmp.Checked = false);
                Network.Network.TypeCurentCamera = Structures.Load()[value].TypeCamera;
                текущаяКамераToolStripMenuItem.Text = "Текущая камера (" + structures.IP + ") - " + (Items.Count > 0 ? (Items.Where(tmp => tmp.Text.Contains(structures.IP)).ToArray().First().Text.Contains("Не доступен") ? "Не доступен" : "Доступен") : "");
                UpdatePotoks(structures);
            }
        }

        public Main()
        {
            InitializeComponent();
            Selected = 0;
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.UseNagleAlgorithm = false;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            UpdateStructures();
            if (Selected >= Structures.Load().Length) return;
            var structures = Structures.Load()[Selected];
            IP = structures.IP;
            UserName = structures.Name;
            Password = structures.Password;
            HTTPPort = structures.HTTPPort;
            RTSPPort = structures.RTSPPort;
            ONVIFPort = structures.ONVIFPort;
            Items[(int)Selected].Checked = true;
            GetTypeCamera = structures.TypeCamera;

            Selected = uint.Parse(Items[0].Name);

            SearchONVIF();
            Record.RecordStarting.MainForm = Handle;

            var records = Structures.Load().Where(tmp => tmp.Records.AutoLoad == Settings.Record.AutoEnabmle.ON);
            for (int i = 0; i < records.Count(); i++)
                StartRecord(i);

            //Console.WriteLine(ONVIF.SendResponce.GetVideoResolution(structures.GetONVIF, structures.Name, structures.Password));
           // Console.WriteLine(ONVIF.SendResponce.GetPTZ(structures.GetONVIF, structures.Name, structures.Password));
        }
        private void ChangeSelected(object sender, EventArgs e)
        {
            Selected = uint.Parse((sender as ToolStripMenuItem).Name);            
        }

        private void UpdatePotoks(Structures str)
        {
            //if (!str.GetONVIFController.IsSupported) return;

            первичныйToolStripMenuItem.DropDownItems.Clear();
            вторичныйToolStripMenuItem.DropDownItems.Clear();          
            
            var pr = ONVIF.SendResponce.GetProfiles(str);

            for (int i = 0; i < pr.Length; i++)
            {
                var raz = " (" + pr[i].VideoEncoderConfiguration.Resolution.Width + "x" + pr[i].VideoEncoderConfiguration.Resolution.Height + ")";
                ToolStripMenuItem tm = new ToolStripMenuItem(i + " - " + pr[i].Name.Trim('"') + raz, null, StreamFSelected, i.ToString())
                {
                    Checked = str.SelectFirstProfile == i,
                };
                первичныйToolStripMenuItem.DropDownItems.Add(tm);

                ToolStripMenuItem tm1 = new ToolStripMenuItem(i + " - " + pr[i].Name.Trim('"') + raz, null, StreamSSelected, i.ToString())
                {
                    Checked = str.SelectSecondProfile == i,
                };
                вторичныйToolStripMenuItem.DropDownItems.Add(tm1);
            }            
        }
        private void StreamFSelected(object sender, EventArgs e)
        {
            var structur = Structures.Load();
            structur[Selected].SelectFirstProfile = int.Parse((sender as ToolStripMenuItem).Name);
            Structures.Save(structur);
            foreach (var tmp in первичныйToolStripMenuItem.DropDownItems)
                (tmp as ToolStripMenuItem).Checked = false;
            (sender as ToolStripMenuItem).Checked = true;           
        }
        private void StreamSSelected(object sender, EventArgs e)
        {
            var structur = Structures.Load();
            structur[Selected].SelectSecondProfile = int.Parse((sender as ToolStripMenuItem).Name);
            Structures.Save(structur);
            foreach (var tmp in вторичныйToolStripMenuItem.DropDownItems)
                (tmp as ToolStripMenuItem).Checked = false;
            (sender as ToolStripMenuItem).Checked = true;
        }

        private void AddAdd()
        {
            ToolStripMenuItem tm = new ToolStripMenuItem("Добавить", null, добавитьToolStripMenuItem_Click);
            выборКамерыToolStripMenuItem.DropDownItems.Add(tm);
            выборКамерыToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
        }
        private void UpdateStructures()
        {
            выборКамерыToolStripMenuItem.DropDownItems.Clear();
            Items.Clear();
            var structur = Structures.Load();
            for (int i = 0; i < structur.Length; i++)
            {
                var dostup = structur[i].IsActive ? " (Доступен)" : " (Не доступен)";
                ToolStripMenuItem tm = new ToolStripMenuItem(structur[i].IP + dostup, null, ChangeSelected, i.ToString());
                tm.Checked = false;
                bool isptz = (new ONVIF.PTZ.PTZController(structur[i]).IsSuported);
                tm.BackColor = isptz ? Color.LightCyan : Color.Transparent;
                tm.Text += isptz ? " (PTZ)" : "";
                Items.Add(tm);               
            }           
            AddAdd();
            Items = Items.OrderByDescending(tmp => tmp.Text.Contains("Доступен")).ToList();            
            выборКамерыToolStripMenuItem.DropDownItems.AddRange(Items.ToArray());
            Selected = Selected;
            Items.ForEach(tmp => tmp.Checked = false);
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Visible = !panel1.Visible;
            panel3.Visible = panel1.Visible ? false : true;
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
                Duration = TimeSpan.MaxValue,
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
                    comboBox1.Invoke(new Action(() => { if (!comboBox1.Items.Contains(e.EndpointDiscoveryMetadata.ListenUris[i].Host)) comboBox1.Items.Add(e.EndpointDiscoveryMetadata.ListenUris[i].Host); }));
                    ONVIFPORTS.Add(e.EndpointDiscoveryMetadata.ListenUris[i].Host, e.EndpointDiscoveryMetadata.ListenUris[i].Port, true);
                }
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel3.Visible = true;
            var structures = Structures.Load();
            structures[Selected] = new Structures(IP, UserName, Password, HTTPPort, RTSPPort, ONVIFPort, Structures.Load()[Selected].SelectFirstProfile, Structures.Load()[Selected].SelectSecondProfile, Structures.Load()[Selected].ValueMD, Structures.Load()[Selected].ZoneDetect, GetTypeCamera, Structures.Load()[Selected].Records);
            Structures.Save(structures);
            UpdateStructures();
            Selected = Selected;
        }

        private void конвертированиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Convert cnv = new Convert();
            cnv.ShowDialog();
        }

        private void просмотрToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void загрузкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Download cnv = new Download(Selected);
            cnv.ShowDialog();
        }

        private void просмотрФотоToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ImageV cnv = new ImageV(Selected);
            cnv.ShowDialog();
        }

        private void логиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogView cnv = new LogView(Selected);
            cnv.ShowDialog();
        }

        private void создатьM3UToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //#EXTM3U
            //
            //#EXTINF:123, Исполнитель - Композиция
            //C:\Documents and Settings\Я\Моя музыка\Песня.mp3

            var structures = Structures.Load()[Selected];

            var m3u = "#EXTM3U" + Environment.NewLine;
            m3u += Environment.NewLine;
            m3u += "#EXTINF:-1, IPCamera - Second is " + structures.IP + Environment.NewLine;
            m3u += structures.GetRTSPSecondONVIF + Environment.NewLine;
            m3u += Environment.NewLine;
            m3u += "#EXTINF:-1, IPCamera - First is " + structures.IP + Environment.NewLine;
            m3u += structures.GetRTSPFirstONVIF;

            SaveFileDialog svf = new SaveFileDialog
            {
                Filter = "M3U Файл|*.m3u",
                FileName = "IP Camera (" + structures.IP + ")"
            };
            if (svf.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(svf.FileName, m3u);
            }
        }

        private void openCVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CV.CV cnv = new CV.CV(Selected);
            cnv.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Settings.Structures.DeleteSetting();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SearchONVIF();
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var s = Structures.Load().ToList();
            s.Add(Structures.Null);
            Structures.Save(s.ToArray());
            UpdateStructures();
            Selected = 0;
        }

        private void одиночноеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowVisible((int)Selected);
        }

        private void групповоеToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var s = Structures.Load().ToList();
            s.RemoveAt((int)Selected);
            Structures.Save(s.ToArray());
            UpdateStructures();
            Selected = 0;
        }

        private void текущаяКамераToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void дляВсехКамерToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var structuress = Structures.Load();

            var m3u = "#EXTM3U" + Environment.NewLine;

            foreach (var structures in structuress)
            {
                m3u += Environment.NewLine;
                m3u += "#EXTINF:-1, IPCamera - Second is " + structures.IP + Environment.NewLine;
                m3u += structures.GetRTSPSecondONVIF + Environment.NewLine;
                m3u += Environment.NewLine;
                m3u += "#EXTINF:-1, IPCamera - First is " + structures.IP + Environment.NewLine;
                m3u += structures.GetRTSPFirstONVIF;
                m3u += Environment.NewLine;
            }
            SaveFileDialog svf = new SaveFileDialog
            {
                Filter = "M3U Файл|*.m3u",
                FileName = "IP Cameras"
            };
            if (svf.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(svf.FileName, m3u);
            }
        }

        private void перейтиВВебинтерфейсToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(String.Format("http://{0}", IP));
        }

        List<Visible> visibles = new List<Visible>();

        private void ShowVisible(int Selected)
        {
            Visible cnv = new Visible((uint)Selected);
            cnv.Show();
            cnv.Handle.SetParent(panel2.Handle);
            cnv.Handle.MoveWindow((visibles.Count * cnv.Width) + 1, 1, cnv.Width, cnv.Height, true);
            var height = ((visibles.Count + 1) * cnv.Width) + 1;
            panel2.Size = new Size(Math.Max(ClientSize.Width, height), panel2.Height);
            //Console.WriteLine(Math.Max(1, ((visibles.Count - 2) * cnv.Width) + 1));
            hScrollBar1.Maximum = Math.Max(1, ((visibles.Count - 2) * cnv.Width) + 1);
            hScrollBar1.Visible = visibles.Count >= 3 ? true : false;
            cnv.FormClosing += (o, e) => { visibles.Remove(cnv); };
            visibles.Add(cnv);
        }

        private void всеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Structures.Load().Length; i++)
            {
                ShowVisible(i);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            _SearchONVIF();
        }

        private void hScrollBar1_Scroll_1(object sender, ScrollEventArgs e)
        {
            panel2.Location = new Point(-hScrollBar1.Value + 12, panel2.Location.Y);
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                foreach (var item in visibles)
                    item.Close();
            }
            catch { }
            try
            {
                foreach (var item in Records)
                    item.Value.Close();
            }
            catch { }
        }

        private void openCVToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GroupV.GV cnv = new GroupV.GV();
            cnv.Show();
        }

        private void fFplayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GroupV.FFPGV cnv = new GroupV.FFPGV();
            cnv.Show();
        }

        private void настройкиИзображенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            groupBox1.Visible = !groupBox1.Visible;            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
        }

        private void фотоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GroupV.Photo cnv = new GroupV.Photo();
            cnv.Show();
        }

        private void менеджерЗаписиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Record.ManagerRecords cnv = new Record.ManagerRecords(Selected);
            cnv.Show();
        }
        private void StartRecord(int ID)
        {
            if (Structures.Load()[ID].Records.Enamble == Settings.Record.RecEnamble.OFF || !Structures.Load()[ID].IsActive) return;
            if (Structures.Load()[ID].Records.Enamble == Settings.Record.RecEnamble.OFF && Structures.Load()[ID].IsActive) StopRecord(ID, false);

            if (!Records.ContainsKey(ID))
                Records.Add(ID, Record.RecordStarting.StartRecord((uint)ID, ID));

            if (Records[ID].IsStop) return;

            if (!Records[ID].IsRunning) 
            { 
                Records[ID] = Record.RecordStarting.StartRecord((uint)ID, ID);                
            }

            Records[ID].Exited += (o, es) => { Items[(o as Record.RecordParameters).ID].Image = null; };
            if (Records[ID].IsRunning)
            {
                Items.Where(tmp => tmp.Name == ID.ToString()).ToArray()[0].Image = Properties.Resources.record;
                Record.RemoveFiles.RemoveOLDFiles(ID);
            }
        }
        private void StopRecord(int ID, bool IsNonEnable = true)
        {
            if (!Records.ContainsKey(ID)) return;

            if (Records[ID].IsRunning)
            {
                Items.Where(tmp => tmp.Name == ID.ToString()).ToArray()[0].Image = null;
                Records[ID].Close();
                if (IsNonEnable) Records[ID].IsStop = true;
            }
        }
        private SortedList<int, Record.RecordParameters> Records = new SortedList<int, Record.RecordParameters>();
        private void timer1_Tick(object sender, EventArgs e)
        {
            var structuress = Structures.Load();

            for (int i = 0; i < structuress.Length; i++)
            {
                if (structuress[i].Records.Enamble != Settings.Record.RecEnamble.OFF)
                {
                    StartRecord(i);
                }
            }
        }

        private void начатьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Records.Values.ToList().ForEach(tmp => tmp.IsStop = false);
            timer1_Tick(null, null);
            timer1.Start();
        }

        private void остановитьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var structuress = Structures.Load();

            for (int i = 0; i < structuress.Length; i++)
            {
                StopRecord(i, true);
            }
            MessageBox.Show("Все записи остановлены");
        }

        private void просмотрЗаписиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Network.Ping.IsOKServer(Structures.Load()[Selected].GetHTTP + "Login.htm"))
                GetTypeCamera = Network.Network.TypeCamera.HI3518;
            else GetTypeCamera = Network.Network.TypeCamera.HI3510;
        }

        private void поверхВсехОконToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AlwaysVisible cnv = new AlwaysVisible(Selected);
            cnv.Show();
        }

        private void настройкиИзображенияToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            IR cnv = new IR(Selected);
            cnv.Show();
        }

        private void найстройкаOSDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CameraSettings.OSD cnv = new CameraSettings.OSD(Selected);
            cnv.Show();
        }

        private void получитьP2PКодToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CameraSettings.P2PCode cnv = new CameraSettings.P2PCode(Selected);
            cnv.ShowDialog();
        }

        private void emailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CameraSettings.Email cnv = new CameraSettings.Email(Selected);
            cnv.ShowDialog();
        }

        private void начатьЗаписьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if(Records.Any()) Records.Where(tmp => tmp.Value.ID == (int)Selected).FirstOrDefault().Value.IsStop = false;
            StartRecord((int)Selected);
        }

        private void остановитьЗаписьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            StopRecord((int)Selected, true);
        }

        private void просмотрЗаписейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Record.VisibleRecord cnv = new Record.VisibleRecord(Selected);
            cnv.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ONVIFPORTS.ContainsKey(comboBox1.Text))
                numericUpDown3.Value = ONVIFPORTS[comboBox1.Text];
        }

        private void определитьПоддержкуPTZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Structures.Load()[Selected].GetPTZController.IsSuported) MessageBox.Show("Камера поддерживает PTZ");
            else MessageBox.Show("Камера PTZ не поддерживает");
        }

        private void задатьТекущееПоложениеКакДомашнееToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Structures.Load()[Selected].GetPTZController.SetHome();
        }
    }
    public static class ExtensionMethods
    {
        public static decimal Map(this decimal value, decimal fromSource, decimal toSource, decimal fromTarget, decimal toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
        public static Image Crop(this Image image, Rectangle selection)
        {
            if (!(image is Bitmap bmp))
                throw new ArgumentException("No valid bitmap");
            Bitmap cropBmp = bmp.Clone(selection, bmp.PixelFormat);
            image.Dispose();
            return cropBmp;
        }
        public static Image Resize(this Image image, Size selection)
        {   
            image = new Bitmap(image, selection.Width, selection.Height);
            return image;
        }
        /// <summary>
        /// Добавляет элемент в коллекцию, если его еще там нет
        /// </summary>
        /// <typeparam name="T">Тип коллекции</typeparam>
        /// <param name="list">Коллекция</param>
        /// <param name="item">Элемент</param>
        /// <param name="Search">Производить поиск</param>
        public static void Add<T>(this List<T> list, T item, bool Search = true)
        {
            if (Search)
            {
                if (!list.Contains(item)) list.Add(item);
            }
            else list.Add(item); ;
        }
        /// <summary>
        /// Добавляет элемент в коллекцию, если его еще там нет
        /// </summary>
        /// <typeparam name="T">Тип коллекции</typeparam>
        /// <param name="list">Коллекция</param>
        /// <param name="item">Элемент</param>
        /// <param name="Search">Производить поиск</param>
        public static void Add<T, U>(this SortedList<T, U> list, T key, U item, bool Search = true)
        {
            if (Search)
            {
                if (!list.ContainsKey(key)) list.Add(key, item);
            }
            else list.Add(key, item); ;
        }
    }
}
