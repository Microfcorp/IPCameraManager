using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using IPCamera.Settings;
using IPCamera.DLL;
using System.Threading;
using System.Diagnostics;

namespace IPCamera.UI
{
    public partial class CameraMenuModule : UserControl
    {
        public string IP
        {
            get
            {
                return Camera.IP;
            }
            set
            {
                Camera = Structures.Load().Where(tmp => tmp.IP == value).FirstOrDefault();
                GenerateButtons();
                new Thread(() => { Thread.Sleep(500); try { CameraTime = Camera.GetONVIFController().CameraDateTime.DateTime; Invoke(new Action(() => { label5.Visible = label6.Visible = true; })); } catch { }  }).Start(); 
                label1.Text = Camera.NameCamera;
                linkLabel2.Text = Camera.IP;
            }
        }

        public uint GenSelected
        {
            get
            {
                return (uint)Structures.Load().ToList().IndexOf(Camera);
            }
        }

        Record.RecordParameters Records = new Record.RecordParameters();

        public Structures Camera;

        DateTime CameraTime;

        public CameraMenuModule()
        {
            InitializeComponent();
        }

        public CameraMenuModule(string ip) : this()
        {
            IP = ip;
        }

        void GenerateButtons()
        {
            button3.Visible = button5.Visible = button4.Visible = Camera.TypeCamera == Network.Network.TypeCamera.HI3510;
            pictureBox1.Visible = button6.Visible = Camera.Records.Enamble != Settings.Record.RecEnamble.OFF;
        }

        private void CameraMenuModule_Paint(object sender, PaintEventArgs e)
        {
            LinearGradientBrush linGrBrush = new LinearGradientBrush(
            new Point(0, 0),
            new Point(Width, Height),
            Color.FromArgb(255, 0, 0, 110),   // opaque blue
            Color.FromArgb(255, 0, 165, 105));  // opaque green

            Pen pen = new Pen(linGrBrush, 10);

            e.Graphics.DrawRectangle(pen, 0, 0, Width, Height);
            //e.Graphics.DrawLine(pen, 0, 0, 600, 300);
            //e.Graphics.FillEllipse(linGrBrush, 0, 0, Width, Height);
        }

        private void LoadGradient(Graphics gr)
        {
            LinearGradientBrush linGrBrush = new LinearGradientBrush(
            new Point(0, 0),
            new Point(Width, Height),
            Color.FromArgb(255, 0, 0, 110),   // opaque blue
            Color.FromArgb(255, 0, 165, 105));  // opaque green

            Pen pen = new Pen(linGrBrush, 10);

            //gr.(linGrBrush, 0, 0, Width, Height);
            //e.Graphics.DrawLine(pen, 0, 0, 600, 300);
            //e.Graphics.FillEllipse(linGrBrush, 0, 0, Width, Height);
        }

        private void ShowVisible()
        {
            button2.Enabled = false;
            Visible cnv = new Visible(GenSelected, false);
            cnv.mShow();
            cnv.StreamOK += (o, e) =>
            {
                //Console.WriteLine("OK");
                Invoke(new Action(() => {
                    Cursor = Cursors.Default;
                    button2.BackgroundImage = Properties.Resources._54544; 
                }));
                
            };

            cnv.CloseStream += (o, e) =>
            {
                Invoke(new Action(() => {
                    button2.Enabled = true;
                }));                
            };

            cnv.Invoke(new Action(() => { cnv.Handle.SetParent(Handle); }));
            
            //cnv.Handle.MoveWindow(-100, -100, cnv.Width, cnv.Height, true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var player = new Viewers.singleplay(GenSelected);
            button2.Enabled = false;
            player.FormClosing += (o, q) => { button2.Enabled = true; };
            player.OpenToWindowManager((MainForm)ParentForm, "Поток - " + Camera.NameCamera);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            (new Download(GenSelected)).OpenToWindowManager((MainForm)ParentForm, "Загрузчик файлов");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CameraTime += new TimeSpan(0,0,1);
            label5.Text = CameraTime.ToString();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(String.Format("http://{0}", Camera.IP));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var player = new CameraSettings(GenSelected);
            button1.Enabled = false;
            player.FormClosing += (o, q) => { button1.Enabled = true; IP = IP; };
            player.OpenToWindowManager((MainForm)ParentForm, "Настройки - " + Camera.NameCamera);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            (new Convert()).OpenToWindowManager((MainForm)ParentForm, "Конвертер");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            (new LogView(GenSelected)).OpenToWindowManager((MainForm)ParentForm, "Просмотр логов");
        }

        void StartRecord()
        {
            if (!Camera.IsActive) return;

            if (Records.IsRunning)
            {
                if (MessageBox.Show("Вы действительно хотите остановить запись?", "Microf IP Camera Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Records.Close();
                }
            }
            else
            {
                Records = Record.RecordStarting.StartRecord(GenSelected, (int)GenSelected);
                Records.Exited += (o, es) => { pictureBox1.Image = Properties.Resources.norecord; };
                Disposed += (o, e) => { Records.Close(); };
                if (Records.IsRunning)
                {
                    pictureBox1.Image = Properties.Resources.record;
                    Record.RemoveFiles.RemoveOLDFiles((int)GenSelected);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            StartRecord();
        }

        private void CameraMenuModule_Load(object sender, EventArgs e)
        {
            if(Camera.Records.Enamble == Settings.Record.RecEnamble.Auto && Camera.Records.AutoLoad == Settings.Record.AutoEnabmle.ON)
            {
                StartRecord();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            (new Record.VisibleRecord(GenSelected)).OpenToWindowManager((MainForm)ParentForm, "Просмотр логов");
        }
    }
}
