using IPCamera.DLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace IPCamera.GroupV
{

    public partial class FFPGV : Form
    {
        readonly Settings.Structures[] set = Settings.Structures.Load();
        readonly SortedList<string, Process> Cameras = new SortedList<string, Process>();
        readonly SortedList<string, PanelPTR> PanelCameras = new SortedList<string, PanelPTR>();
        readonly SortedList<string, int> CountRun = new SortedList<string, int>();
        //readonly List<IntPtr> Forms = new List<IntPtr>();
        readonly List<string> _IsRunFFMPEG = new List<string>();

        public int SizeSET
        {
            get => set.Length;
        }

        public new const int Padding = 10;
        public const int CountRunning = 50;

        public FFPGV()
        {
            InitializeComponent();
        }

        public static bool IsRunFFPLAY(string str)
        {
            return str != null && str.Contains("M-V:") && str.Contains("fd=") && str.Contains("aq=") && str.Contains("vq=") && str.Contains("sq=");
        }

        private Process OpenPlayer(Settings.Structures Setting)
        {
            var s = new Process();
            s.StartInfo.FileName = "ffplay.exe"; //-nodisp -vn -autoexit
            s.StartInfo.Arguments = String.Format("-noborder " + Setting.GetRTSPSecondONVIF)
                + " -x 640 -y 360";
            s.StartInfo.UseShellExecute = false;
            s.StartInfo.CreateNoWindow = true;
            //s.StartInfo.RedirectStandardOutput = true;
            //s.StartInfo.RedirectStandardInput = true;
            s.StartInfo.RedirectStandardError = true;
            s.EnableRaisingEvents = true;
            s.Start();
            s.BeginErrorReadLine();
            Thread.Sleep(200);
            s.MainWindowHandle.SetParent(this.Handle);
            s.MainWindowHandle.ShowWindow((int)CommonFunctions.nCmdShow.SW_HIDE);
            Console.WriteLine(s.MainWindowHandle.ToInt32());
            return s;
        }
        //5.91 M-V:  0.020 fd=   4 aq=    0KB vq=   69KB sq=    0B f=1/1

        private void SizeOrient(PanelPTR panel)
        {
            if (Orientation.OrientationSensor.GetOrientationType(flowLayoutPanel1.Size) == Orientation.OrientationType.Vertical) panel.Size = new Size((flowLayoutPanel1.ClientSize.Width) - Padding, (flowLayoutPanel1.ClientSize.Height / SizeSET) - Padding);
            else if (Orientation.OrientationSensor.GetOrientationType(flowLayoutPanel1.Size) == Orientation.OrientationType.Horizontal) panel.Size = new Size((flowLayoutPanel1.ClientSize.Width / SizeSET) - Padding, (flowLayoutPanel1.ClientSize.Height) - Padding);
        }

        public void ResizeF()
        {
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.Controls.Clear();

            for (int i = 0; i < set.Length; i++)
            {
                if (!Cameras.ContainsKey(set[i].IP + i))
                    Cameras.Add(set[i].IP + i, OpenPlayer(set[i]));

                CountRun.Add(set[i].IP + i, 0);
                PanelCameras.Add(set[i].IP + i, new PanelPTR());

                var ia = i;

                SizeOrient(PanelCameras[set[ia].IP + ia]);

                Cameras[set[i].IP + i].ErrorDataReceived += (o, e) =>
                {
                    if (CountRun[set[ia].IP + ia] < CountRunning)
                    {
                        CountRun[set[ia].IP + ia]++;
                        //var Camera = o as Process;
                        _IsRunFFMPEG.Add(set[ia].IP + ia);
                        PanelCameras[set[ia].IP + ia].Invoke(new Action(() =>
                        {
                            var t = Cameras[set[ia].IP + ia].Id.EnumerateProcessWindowHandles();
                            foreach (var item in t)
                            {
                                //Console.WriteLine(PanelCameras[set[ia].IP + ia].Handle);
                                //Console.WriteLine("tt");
                                //Forms.Add(item, true);
                                PanelCameras[set[ia].IP + ia].AddHandlers(item);
                                item.SetParent(PanelCameras[set[ia].IP + ia].Handle);
                                item.MoveWindow(0, 0, PanelCameras[set[ia].IP + ia].Width, PanelCameras[set[ia].IP + ia].Height, true);
                            }                                                     
                        }));
                    }
                };

                Cameras[set[i].IP + i].Exited += (o, e) => { };
                
                //Cameras[set[i].IP + i].MainWindowHandle.ShowWindow((int)CommonFunctions.nCmdShow.SW_HIDE);               

                flowLayoutPanel1.Controls.Add(PanelCameras[set[i].IP + i]);
            }
            flowLayoutPanel1.ResumeLayout();
        }

        private void FFPGV_Load(object sender, EventArgs e)
        {
            ResizeF();
        }

        private void FFPGV_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                foreach (var item in Cameras)
                {
                    if (!item.Value.HasExited) item.Value.Kill();
                    Cameras.Remove(item.Key);
                }
            }
            catch { }
        }

        private void flowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            foreach (var item in PanelCameras)
            {
                SizeOrient(item.Value);
                foreach (var item1 in item.Value.HandlersArray)
                {
                    item1.MoveWindow(0, 0, item.Value.Width, item.Value.Height, true);
                }
            }
        }
    }
}
