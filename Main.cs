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
using IPCamera.ComandArgs;
using IPCamera.Network.AutoUpdate;
using IPCamera.Network;
using System.Threading.Tasks;

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

        private bool PTZ
        {
            get
            {
                return checkBox1.Checked;
            }
            set
            {
                checkBox1.Checked = value;
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
                    настройкиИзображенияToolStripMenuItem.Visible = true;
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
        Thread th = new Thread(new ParameterizedThreadStart(UpdateStructures));
        AutoResetEvent evt = new AutoResetEvent(true);

        private uint _selected = 0;

        public uint Selected
        {
            get => _selected;
            set
            {
                if (value >= Structures.Load().Length) return;
                var structures = Structures.Load()[value];

                if(InvokeRequired) Invoke(new Action(() => 
                {
                    Cursor = Cursors.WaitCursor;
                    _selected = value;                   
                    IP = structures.IP;
                    UserName = structures.Name;
                    Password = structures.Password;
                    HTTPPort = structures.HTTPPort;
                    RTSPPort = structures.RTSPPort;
                    ONVIFPort = structures.ONVIFPort;
                    GetTypeCamera = structures.TypeCamera;
                    PTZ = structures.PTZ;
                }));
                else
                {
                    Cursor = Cursors.WaitCursor;
                    _selected = value;
                    IP = structures.IP;
                    UserName = structures.Name;
                    Password = structures.Password;
                    HTTPPort = structures.HTTPPort;
                    RTSPPort = structures.RTSPPort;
                    ONVIFPort = structures.ONVIFPort;
                    GetTypeCamera = structures.TypeCamera;
                    PTZ = structures.PTZ;
                }
                NM = structures.NameCamera;

                var active = structures.IsActive;
                if (!active) active = structures.IsActive;

                if (InvokeRequired) Invoke(new Action(() =>
                {
                    задатьТекущееПоложениеКакДомашнееToolStripMenuItem.Enabled = сценарииДвиженияToolStripMenuItem.Enabled = управлениеToolStripMenuItem.Enabled = остановитьToolStripMenuItem.Enabled = structures.GetPTZController().IsSuported;
                    оКамереToolStripMenuItem.Enabled = button8.Enabled = загрузкаФайловToolStripMenuItem.Enabled = логиToolStripMenuItem.Enabled = openCVToolStripMenuItem.Enabled = начатьЗаписьToolStripMenuItem1.Enabled = потокONVIFToolStripMenuItem.Enabled = определитьПоддержкуPTZToolStripMenuItem.Enabled = настройкиИзображенияToolStripMenuItem.Enabled = одиночноеToolStripMenuItem.Enabled = просмотрФотоToolStripMenuItem.Enabled = перейтиВВебинтерфейсToolStripMenuItem.Enabled = active;
                    Network.Network.TypeCurentCamera = structures.TypeCamera;
                    текущаяКамераToolStripMenuItem.Text = "Текущая камера (" + structures.NameCamera + ")" + (structures.GetPTZController().IsSuported ? " (PTZ)" : "") + " - " + (!active ? "Не доступен" : "Доступен");
                    Items.Where(tmp => tmp.Name == value.ToString()).ToList().ForEach(tmp => tmp.Checked = true);
                    Items.Where(tmp => tmp.Name != value.ToString()).ToList().ForEach(tmp => tmp.Checked = false);
                }));
                else
                {
                    задатьТекущееПоложениеКакДомашнееToolStripMenuItem.Enabled = сценарииДвиженияToolStripMenuItem.Enabled = управлениеToolStripMenuItem.Enabled = остановитьToolStripMenuItem.Enabled = structures.GetPTZController().IsSuported;
                    оКамереToolStripMenuItem.Enabled = button8.Enabled = загрузкаФайловToolStripMenuItem.Enabled = логиToolStripMenuItem.Enabled = openCVToolStripMenuItem.Enabled = начатьЗаписьToolStripMenuItem1.Enabled = потокONVIFToolStripMenuItem.Enabled = определитьПоддержкуPTZToolStripMenuItem.Enabled = настройкиИзображенияToolStripMenuItem.Enabled = одиночноеToolStripMenuItem.Enabled = просмотрФотоToolStripMenuItem.Enabled = перейтиВВебинтерфейсToolStripMenuItem.Enabled = active;
                    Network.Network.TypeCurentCamera = structures.TypeCamera;
                    текущаяКамераToolStripMenuItem.Text = "Текущая камера (" + structures.NameCamera + ")" + (structures.GetPTZController().IsSuported ? " (PTZ)" : "") + " - " + (!active ? "Не доступен" : "Доступен");
                    Items.Where(tmp => tmp.Name == value.ToString()).ToList().ForEach(tmp => tmp.Checked = true);
                    Items.Where(tmp => tmp.Name != value.ToString()).ToList().ForEach(tmp => tmp.Checked = false);
                }
                
                UpdatePotoks(structures);

                if (InvokeRequired) Invoke(new Action(() =>
                {
                    Cursor = Cursors.Default;
                }));
                else
                    Cursor = Cursors.Default;
            }
        }
        bool NoTi = false;
        public Main(Parser p)
        {
            InitializeComponent();

            //Loading ld = new Loading();
            //ld.Show();

            if (!p.FindParams("-nb")) notifyIcon1.ShowBalloonTip(500, "IPCamera Manager", "Пожалуйста подождите. Приложение запускается", ToolTipIcon.Info);

            timer3.Enabled = !p.FindParams("-ni");
            NoTi = p.FindParams("-np");

            if (p.FindParamsAndArgs("-ap", out string ipq))
            {
                AlwaysVisible cnv = new AlwaysVisible(uint.Parse(ipq));
                cnv.Show();
            }

            if (p.FindParams("-rs"))
            {
                Structures.DeleteSetting();
                PTZScenes.PTZCollection.DeleteSetting();
                Monitors.MonitorsController.DeleteSetting();
                Settings.StaticMembers.ImageSettings.FPS = 10;
                Settings.StaticMembers.ImageSettings.Padding = 8;
                Settings.StaticMembers.PTZSettings.Timeout = 500;
                Settings.StaticMembers.PTZSettings.StepTimeout = 200;
            }

            Selected = 0;
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.UseNagleAlgorithm = false;
            //ld.Close();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            th.IsBackground = true;
            UpdateStructures();

            if (Selected >= Structures.Load().Length) return;
            var structures = Structures.Load()[Selected];
            IP = structures.IP;
            UserName = structures.Name;
            Password = structures.Password;
            HTTPPort = structures.HTTPPort;
            RTSPPort = structures.RTSPPort;
            ONVIFPort = structures.ONVIFPort;           
            GetTypeCamera = structures.TypeCamera;
            PTZ = structures.PTZ;

            //Items[(int)Selected].Checked = true;
            //Selected = uint.Parse(Items[0].Name);

            SearchONVIF();
            GetMonitors();
            GetPTZS();

            toolStripTextBox1.Text = Settings.StaticMembers.ImageSettings.FPS.ToString();
            toolStripTextBox2.Text = Settings.StaticMembers.ImageSettings.Padding.ToString();
            toolStripTextBox3.Text = Settings.StaticMembers.PTZSettings.Timeout.ToString();
            toolStripTextBox4.Text = Settings.StaticMembers.PTZSettings.StepTimeout.ToString();

            Record.RecordStarting.MainForm = Handle;

            var records = Structures.Load().Where(tmp => tmp.Records.AutoLoad == Settings.Record.AutoEnabmle.ON);
            for (int i = 0; i < records.Count(); i++)
                StartRecord(i);


            if (timer3.Enabled) //Уведомления о переключении подстветки
            {
                var host = Structures.Load();
                host = host.Where(tmp => tmp.TypeCamera == Network.Network.TypeCamera.HI3510).ToArray();
                foreach (var item in host)
                {
                    if (item.IsActive)
                    {
                        var logs = Downloading.GetLogDevice(item.URLToHTTPPort, item.Name, item.Password);
                        foreach (IRCut item1 in logs.Nodes.Where(x => x is IRCut))
                            slogs.Add(item1.date, item.IP, true);
                    }
                }
            }

            if (!NoTi)
            {
                var ptzs = PTZScenes.PTZCollection.Load().Where(tmp => tmp.Trigger == PTZScenes.TriggerPTZ.Timer).ToArray();
                for (int i = 0; i < ptzs.Length; i++)
                {
                    var tm = new System.Windows.Forms.Timer
                    {
                        Interval = (int)ptzs[i].DefaultTriggerData.Timer_Timeout.TimeOfDay.TotalMilliseconds
                    };
                    var ia = i;
                    tm.Tick += (o, q) =>
                    {
                        if (ptzs[ia].Trigger == PTZScenes.TriggerPTZ.Timer)
                            PTZScenes.PTZRunning.Run(ptzs[ia], Structures.Load()[ptzs[ia].DefaultTriggerData.CameraTrigger]);
                        else
                            (o as System.Windows.Forms.Timer).Stop();

                        (o as System.Windows.Forms.Timer).Interval = (int)ptzs[ia].DefaultTriggerData.Timer_Timeout.TimeOfDay.TotalMilliseconds;
                    };
                    //tm.Start();
                }
            }

            //Console.WriteLine(Structures.Load()[2].GetONVIFController().IsMotionDetect);
            //Console.WriteLine(ONVIF.SendResponce.GetEvents(Structures.Load()[1]));
            //Console.WriteLine(ONVIF.SendResponce.GetPTZ(structures.GetONVIF, structures.Name, structures.Password));
        }
        private void ChangeSelected(object sender, EventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart((tmp) => { Selected = uint.Parse((tmp as ToolStripMenuItem).Name); }));
            t.Start(sender);
        }

        private void UpdatePotoks(Structures str)
        {
            if (!str.GetONVIFController().IsSupported) return;

            if (InvokeRequired) Invoke(new Action(() =>
            {
                первичныйToolStripMenuItem.DropDownItems.Clear();
                вторичныйToolStripMenuItem.DropDownItems.Clear();
            }));
            else
            {
                первичныйToolStripMenuItem.DropDownItems.Clear();
                вторичныйToolStripMenuItem.DropDownItems.Clear();
            }

            var pr = str.GetONVIFController().Profiles;

            for (int i = 0; i < pr.Length; i++)
            {
                var raz = " (" + pr[i].VideoEncoderConfiguration.Resolution.Width + "x" + pr[i].VideoEncoderConfiguration.Resolution.Height + ")";

                ToolStripMenuItem tm = new ToolStripMenuItem(i + " - " + pr[i].Name.Trim('"') + raz, null, StreamFSelected, i.ToString())
                {
                    Checked = str.SelectFirstProfile == i,
                };
                if (InvokeRequired) Invoke(new Action(() =>
                {
                    первичныйToolStripMenuItem.DropDownItems.Add(tm);
                }));
                else первичныйToolStripMenuItem.DropDownItems.Add(tm);

                ToolStripMenuItem tm1 = new ToolStripMenuItem(i + " - " + pr[i].Name.Trim('"') + raz, null, StreamSSelected, i.ToString())
                {
                    Checked = str.SelectSecondProfile == i,
                };
                if (InvokeRequired) Invoke(new Action(() =>
                {
                    вторичныйToolStripMenuItem.DropDownItems.Add(tm1);
                }));
                else вторичныйToolStripMenuItem.DropDownItems.Add(tm1);
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

        private void GetMonitors()
        {
            запуститьМониторToolStripMenuItem.DropDownItems.Clear();
            настроитьМониторToolStripMenuItem.DropDownItems.Clear();
            удалитьМониторToolStripMenuItem.DropDownItems.Clear();

            var Monitor = Monitors.MonitorsController.Load();

            for (int i = 0; i < Monitor.Length; i++)
            {
                ToolStripMenuItem tm = new ToolStripMenuItem(i + " - " + Monitor[i].Name, null, PlayMonitor, i.ToString());
                ToolStripMenuItem tm1 = new ToolStripMenuItem(i + " - " + Monitor[i].Name, null, EditMonitor, i.ToString());
                ToolStripMenuItem tm2 = new ToolStripMenuItem(i + " - " + Monitor[i].Name, null, RemoveMonitor, i.ToString());
                запуститьМониторToolStripMenuItem.DropDownItems.Add(tm);
                настроитьМониторToolStripMenuItem.DropDownItems.Add(tm1);
                удалитьМониторToolStripMenuItem.DropDownItems.Add(tm2);
            }
        }

        private void PlayMonitor(object sender, EventArgs e)
        {
            var s = Monitors.MonitorsController.Load();
            var mv = new Monitors.MonitorVisible(s[int.Parse((sender as ToolStripMenuItem).Name)]);
            mv.Show();
        }
        private void EditMonitor(object sender, EventArgs e)
        {
            var s = Monitors.MonitorsController.Load();
            var mv = new Monitors.MonitorEdit(s[int.Parse((sender as ToolStripMenuItem).Name)], int.Parse((sender as ToolStripMenuItem).Name));
            mv.FormClosed += (o, a) => GetMonitors();
            mv.Show();
        }
        private void RemoveMonitor(object sender, EventArgs e)
        {
            var s = Monitors.MonitorsController.Load().ToList();
            s.RemoveAt(int.Parse((sender as ToolStripMenuItem).Name));
            Monitors.MonitorsController.Save(s.ToArray());
            GetMonitors();
        }

        private void GetPTZS()
        {
            запуститьToolStripMenuItem.DropDownItems.Clear();
            настроитьToolStripMenuItem.DropDownItems.Clear();
            удалитьToolStripMenuItem1.DropDownItems.Clear();

            var Monitor = PTZScenes.PTZCollection.Load();

            for (int i = 0; i < Monitor.Length; i++)
            {
                ToolStripMenuItem tm = new ToolStripMenuItem(i + " - " + Monitor[i].Name, null, PlayPTZS, i.ToString());
                ToolStripMenuItem tm1 = new ToolStripMenuItem(i + " - " + Monitor[i].Name, null, EditPTZS, i.ToString());
                ToolStripMenuItem tm2 = new ToolStripMenuItem(i + " - " + Monitor[i].Name, null, RemovePTZS, i.ToString());
                запуститьToolStripMenuItem.DropDownItems.Add(tm);
                настроитьToolStripMenuItem.DropDownItems.Add(tm1);
                удалитьToolStripMenuItem1.DropDownItems.Add(tm2);
            }
        }

        private void PlayPTZS(object sender, EventArgs e)
        {
            var s = PTZScenes.PTZCollection.Load();
            PTZScenes.PTZRunning.Run(s[int.Parse((sender as ToolStripMenuItem).Name)], Structures.Load()[Selected]);
        }
        private void EditPTZS(object sender, EventArgs e)
        {
            var s = PTZScenes.PTZCollection.Load();
            var mv = new PTZScenes.Editor(s[int.Parse((sender as ToolStripMenuItem).Name)], int.Parse((sender as ToolStripMenuItem).Name));
            mv.FormClosed += (o, a) => GetPTZS();
            mv.Show();
        }
        private void RemovePTZS(object sender, EventArgs e)
        {
            var s = PTZScenes.PTZCollection.Load().ToList();
            s.RemoveAt(int.Parse((sender as ToolStripMenuItem).Name));
            PTZScenes.PTZCollection.Save(s.ToArray());
            GetPTZS();
        }

        private void AddAdd()
        {
            if (InvokeRequired) Invoke(new Action(() => 
            {
                ToolStripMenuItem tm = new ToolStripMenuItem("Добавить", null, добавитьToolStripMenuItem_Click);
                выборКамерыToolStripMenuItem.DropDownItems.Add(tm);
                выборКамерыToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            }));
            else
            {
                ToolStripMenuItem tm = new ToolStripMenuItem("Добавить", null, добавитьToolStripMenuItem_Click);
                выборКамерыToolStripMenuItem.DropDownItems.Add(tm);
                выборКамерыToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            }
        }

        private void UpdateStructures()
        {
            if (!th.IsAlive)
            {
                th.Start(this);
            }
            else
            {
                evt.Set();
            }
        }

        private static void UpdateStructures(object m)
        {
            var frm = m as Main;
            while (true)
            {
                frm.evt.WaitOne();

                frm.Invoke(new Action(() => frm.выборКамерыToolStripMenuItem.DropDownItems.Clear()));
                frm.Items.Clear();
                var structur = Structures.Load();
                for (int i = 0; i < structur.Length; i++)
                {
                    var dostup = structur[i].IsActive ? " (Доступен)" : " (Не доступен)";
                    ToolStripMenuItem tm = new ToolStripMenuItem(structur[i].NameCamera + " (" + structur[i].IP + ")" + dostup, null, frm.ChangeSelected, i.ToString())
                    {
                        Checked = false
                    };
                    bool isptz = structur[i].GetPTZController().IsSuported;
                    tm.BackColor = isptz ? Color.LightCyan : Color.Transparent;
                    tm.Text += isptz ? " (PTZ)" : "";
                    frm.Items.Add(tm);
                }
                frm.AddAdd();
                //frm.Items = frm.Items.OrderByDescending(tmp => tmp.Text.Contains("Доступен")).ToList();
                frm.Invoke(new Action(() => frm.выборКамерыToolStripMenuItem.DropDownItems.AddRange(frm.Items.ToArray())));
                frm.Selected = frm.Selected;
                frm.Invoke(new Action(() => frm.Items.ForEach(tmp => tmp.Checked = false)));
                frm.Invoke(new Action(() => frm.Items[(int)frm.Selected].Checked = true));
            }
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
            structures[Selected] = new Structures(IP, UserName, Password, HTTPPort, RTSPPort, ONVIFPort, Structures.Load()[Selected].SelectFirstProfile, Structures.Load()[Selected].SelectSecondProfile, PTZ, Structures.Load()[Selected].ValueMD, Structures.Load()[Selected].ZoneDetect, GetTypeCamera, Structures.Load()[Selected].Records, NM, MAC.ConvertIpToMAC(System.Net.IPAddress.Parse(IP)));
            Structures.Save(structures);

            if (!structures[Selected].IsActive) MessageBox.Show("Данная камера сейчас недоступна. Проверьте все ли данные указаны верно");

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

        private void ShowVisible(int Selected, bool ump = false)
        {
            Visible cnv = new Visible((uint)Selected, ump);
            cnv.Show();
            cnv.Handle.SetParent(panel2.Handle);
            cnv.Handle.MoveWindow((visibles.Count * cnv.Width) + 1, 1, cnv.Width, cnv.Height, true);
            var height = ((visibles.Count + 1) * cnv.Width) + 1;
            panel2.Size = new Size(Math.Max(ClientSize.Width, height), panel2.Height);
            //Console.WriteLine(Math.Max(1, ((visibles.Count - 2) * cnv.Width) + 1));
            hScrollBar1.Maximum = Math.Max(1, ((visibles.Count - 2) * cnv.Width) + 1);
            hScrollBar1.Visible = visibles.Count >= 3 ? true : false;
            cnv.FormClosing += (o, e) => { visibles.Remove(cnv); };
            cnv.Reload += (o, e) => { visibles.Add(cnv); };
            //visibles.Add(cnv);
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

        private void button5_Click(object sender, EventArgs e)
        {
            if (Network.Ping.IsOKResource(Structures.Load()[Selected].GetHTTP + "Login.htm"))
                GetTypeCamera = Network.Network.TypeCamera.HI3518;
            //else if (Network.Ping.IsOKResource(Structures.Load()[Selected].GetHTTP)) 
            //    GetTypeCamera = Network.Network.TypeCamera.HI3510;
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
            if (Records.Any()) Records.Where(tmp => tmp.Value.ID == (int)Selected).FirstOrDefault().Value.IsStop = false;
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
            if (ONVIFPORTS.Keys.Contains(comboBox1.Text))
                numericUpDown3.Value = ONVIFPORTS[comboBox1.Text];
        }

        private void определитьПоддержкуPTZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Structures.Load()[Selected].GetPTZController().IsSuported) MessageBox.Show("Камера поддерживает PTZ");
            else if (!Structures.Load()[Selected].IsActive) MessageBox.Show("Невозможно определить наличие PTZ, поскольку камера недоступна");
            else MessageBox.Show("Камера PTZ не поддерживает");
        }

        private void задатьТекущееПоложениеКакДомашнееToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Structures.Load()[Selected].GetPTZController().SetHome();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            comboBox1_SelectedIndexChanged(null, null);
        }

        private void камерыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel1.Visible = !panel1.Visible;
            panel3.Visible = panel1.Visible ? false : true;
        }

        private void проверкаОбновленийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var vers = NetworkUpdate.GetVersionServer();
            if (vers > CurrentVersion.CurrentVersions)
                MessageBox.Show("Доспуна новая версия. Перезапустите приложение для обновления", "IPCamera Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("У вас уже установлена последняя версия");
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            UpdateStructures();
            Selected = Selected;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            panel1.Visible = !panel1.Visible;
            panel3.Visible = panel1.Visible ? false : true;
        }

        private void импортToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog opg = new OpenFileDialog();
            opg.Filter = "Файл настроек (*.micset)|*.micset";
            opg.Title = "Импорт файла настроек";
            if (opg.ShowDialog() == DialogResult.OK)
            {
                Structures.ReplaceSetting(opg.FileName);
                UpdateStructures();
            }
        }

        private void экспортToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog opg = new SaveFileDialog();
            opg.Filter = "Файл настроек (*.micset)|*.micset";
            opg.Title = "Экспорт файла настроек";
            if (opg.ShowDialog() == DialogResult.OK)
            {
                Structures.CopySetting(opg.FileName);
            }
        }

        private void общаяИнформацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutCamera cnv = new AboutCamera(Selected);
            cnv.Show();
        }

        private void датаИВремяНаКамереToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var date = Structures.Load()[Selected].GetONVIFController().CameraDateTime.Parse;
            var message = "Текущее время на камере: " + date;
            MessageBox.Show(message);
        }

        private void протоколыКамерыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InfoCamera.CameraProtocols cnv = new InfoCamera.CameraProtocols(Selected);
            cnv.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (ONVIFPort == 0) return;
            var protocols = Structures.Load()[Selected].GetONVIFController().NetworkProtocols;
            HTTPPort = (uint)protocols.Where(tmp => tmp.Name == ODEV.NetworkProtocolType.HTTP).FirstOrDefault().Port.FirstOrDefault();
            RTSPPort = (uint)protocols.Where(tmp => tmp.Name == ODEV.NetworkProtocolType.RTSP).FirstOrDefault().Port.FirstOrDefault();
        }

        private void названиеХостаКамерыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var host = Structures.Load()[Selected].GetONVIFController().Hostname;
            host = "Название хоста камеры: " + host;
            MessageBox.Show(host);
        }

        private void пользователиКамерыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InfoCamera.CameraUsers cnv = new InfoCamera.CameraUsers(Selected);
            cnv.ShowDialog();
        }

        private void версияONVIFПротоколаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var host = Structures.Load()[Selected].GetONVIFController().Version.ToString();
            host = "Версия протокола ONVIF: " + host;
            MessageBox.Show(host);
        }

        private void возможностиКамерыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InfoCamera.CapCamera cnv = new InfoCamera.CapCamera(Selected);
            cnv.ShowDialog();
        }

        private void перезагрузитьКамеруToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var m = MessageBox.Show("Вы действительно хотите перезагрузить устройство " + IP + "?", "Подтвержение перезагрузки камеры", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            var ok = m == DialogResult.Yes;
            Structures.Load()[Selected].GetONVIFController().Reboot(ok);
        }

        private void настройкиИзображенияToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            CameraSettings.ONVIfImage cnv = new CameraSettings.ONVIfImage(Selected);
            cnv.Show();
        }

        SortedList<DateTime, string> slogs = new SortedList<DateTime, string>();

        private void timer3_Tick(object sender, EventArgs e)
        {
            var host = Structures.Load();
            host = host.Where(tmp => tmp.TypeCamera == Network.Network.TypeCamera.HI3510).ToArray();

            foreach (var item in host)
            {
                if (item.IsActive)
                {
                    var logs = Downloading.GetLogDevice(item.URLToHTTPPort, item.Name, item.Password);
                    foreach (IRCut item1 in logs.Nodes.Where(x => x is IRCut))
                    {
                        var one = item1.perechod == IRCut.Perehod.Day_Night ? "ИК подсветка включена (ночь)" : "ИК подстветка выключена (день)";
                        if (!slogs.ContainsKey(item1.date))
                        {
                            slogs.Add(item1.date, item.IP, true);
                            notifyIcon1.ShowBalloonTip(100, "Смена режима отображения", one, ToolTipIcon.Info);
                        }
                    }
                }
            }
        }

        private void остановитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Structures.Load()[Selected].GetPTZController().Stop();
        }

        private void добавитьМониторToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var s = Monitors.MonitorsController.Load().ToList();
            s.Add(Monitors.MonitorSettings.Null);
            Monitors.MonitorsController.Save(s.ToArray());
            GetMonitors();
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            Settings.StaticMembers.ImageSettings.FPS = uint.Parse(toolStripTextBox1.Text);
        }

        private void toolStripTextBox2_TextChanged(object sender, EventArgs e)
        {
            Settings.StaticMembers.ImageSettings.Padding = int.Parse(toolStripTextBox2.Text);
        }

        private void toolStripTextBox3_TextChanged(object sender, EventArgs e)
        {
            Settings.StaticMembers.PTZSettings.Timeout = uint.Parse(toolStripTextBox3.Text);
        }

        private void управлениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PTZMove cnv = new PTZMove(Selected);
            cnv.Show();
        }

        private void toolStripTextBox4_TextChanged(object sender, EventArgs e)
        {
            Settings.StaticMembers.PTZSettings.StepTimeout = uint.Parse(toolStripTextBox4.Text);
        }

        private void внешнегоРекодераToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OthetCoder cnv = new OthetCoder();
            cnv.Show();
        }

        private void mplayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowVisible((int)Selected, true);
        }

        string NM = "";

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TextInput ti = new TextInput("Введите имя камеры", NM);
            if(ti.ShowDialog() == DialogResult.OK)
                NM = ti.ReturnTextNoLine;
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
