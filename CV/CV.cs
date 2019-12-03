using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using FaceDetection;
using PedestrianDetection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace IPCamera.CV
{
    public partial class CV : Form
    {
        Settings.Structures set = Settings.Structures.Load();

        static bool NoDetect = false;
        Rectangle p = Settings.Structures.Load().ZoneDetect;

        MootionDetect md;
        ImageBox im = new ImageBox();
        long Detects;  
        public CV()
        {
            InitializeComponent();

            md = new MootionDetect(String.Format("rtsp://{0}:{1}@{2}:{3}/iphone/{4}", set.Name, set.Password, set.IP, set.RTSPPort, ((int)Network.RTSPStream.Second_Stream).ToString()));
            //md = new MootionDetect("C:\\Users\\Лехап\\Desktop\\Барсик\\P191202_170012_171018.264.h264_ff.mp4");
            md.OnMD += (long s, Mat image, long m) => { im.Image = image; if (s != default(double)) { Detect(s); Detects = s; } };

            im.Location = new Point(0, 0);
            im.Dock = System.Windows.Forms.DockStyle.Fill;
            im.Size = this.ClientSize;
            im.SizeMode = PictureBoxSizeMode.StretchImage;
            im.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;

            md.Zone = p;        

            im.MouseDown += (object sender, MouseEventArgs e) => { if (e.Button == MouseButtons.Right) p = new Rectangle(e.Location, new Size(0,0)); };
            im.MouseMove += (object sender, MouseEventArgs e) => 
            {              
                if(e.Button == MouseButtons.Right)
                {
                    p.Size = new Size(Math.Abs(p.X - e.X), Math.Abs(p.Y - e.Y));
                    im.CreateGraphics().DrawRectangle(new Pen(Brushes.Blue, 3f), p);
                    NoDetect = false;
                }               
            };
            im.MouseUp += (object sender, MouseEventArgs e) => {
                if (e.Button == MouseButtons.Right)
                {
                    md.Zone = p;
                    //im.CreateGraphics().Clear(Color.Gray);
                    set.ZoneDetect = p;
                    set.Save();
                }
            };
            im.Invalidated += (object sender, InvalidateEventArgs e) => im.CreateGraphics().DrawRectangle(new Pen(Brushes.Blue, 3f), p);
        }

        private void CV_Load(object sender, EventArgs e)
        {
            this.Controls.Add(im);          
        }

        private void CV_FormClosing(object sender, FormClosingEventArgs e)
        {
            md.Dispose();
            Console.WriteLine(max.Max());
        }

        private void CV_Resize(object sender, EventArgs e)
        {
            im.Size = this.ClientSize;
            //p.Location =  (new Point(this.ClientSize) - new Size(p.Location));
        }
        List<long> max = new List<long>(1);
        List<long> data = new List<long>();
        private void Detect(long s)
        {          
            max.Add(s);
            data.Add(s);

            if (data.Count > 5 / MootionDetect.allfpsc)
                data.RemoveAt(0);

            //Console.WriteLine(s + " || " + data.Average() + " || " + data.Min() + " || " + data.Max());

            if ((decimal)data.Average() >= set.ValueMD & !NoDetect)
            {
                Console.Beep(2500, 500);
                if (!backgroundWorker1.IsBusy) { backgroundWorker1.RunWorkerAsync(); }
                Console.WriteLine("Detect!!!");
                im.Image.Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofCorp\\IPCameraManager\\md_"+DateTime.Now.ToString().Replace(":","_")+".jpg");             
            }           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            set.ValueMD = numericUpDown1.Value;
            set.Save();            
            panel1.Visible = false;
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = set.ValueMD;
            panel1.Visible = true;
        }

        private void детекторДвиженияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NoDetect = !NoDetect;
        }

        private void обнаружениеЛюдейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            md.DetectPed = !md.DetectPed;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //Console.Beep(4000, 1000);
            if (MessageBox.Show("Обнаружено движение", "IPCameraManager", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No) NoDetect = true;           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = set.ValueMD = (decimal)Detects;
            set.Save();
        }
    }
}
