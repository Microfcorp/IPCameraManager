using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using IPCamera.DLL;
using RecordDB;

namespace IPCamera
{
    public partial class OthetCoder : Form
    {
        const int CC = 50;
        int CV = 0;
        public OthetCoder(bool showaelect = true)
        {
            InitializeComponent();
            if (showaelect) выбратьЗаписиToolStripMenuItem_Click(null, null);
        }

        private void выбратьЗаписиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog opg = new OpenFileDialog()
            {
                Filter = "Все поддерживаемые|*.mp4;*.mkv|" +
                         "MP4 (Видео и Аудио)|*.mp4|" +
                         "MKV (Видео и Аудио)|*.mkv",
                Multiselect = true
            };
            if(opg.ShowDialog() == DialogResult.OK)
            {
                foreach (var item in opg.FileNames)
                {        
                    treeView1.Nodes.Add(item, Path.GetFileName(item));
                }
            }
        }

        private void Resizeplayer(Process p)
        {
            panel1.Invoke(new Action(() =>
            {
                var procecess = p.Id.EnumerateProcessWindowHandles();
                foreach (var item in procecess)
                {
                    item.SetParent(panel1.Handle);
                    item.MoveWindow(0, 0, panel1.Width, panel1.Height, true);
                }
            }));
        }

        private Process OpenPlayer(string path)
        {
            var s = new Process();
            s.StartInfo.FileName = "mplayer.exe"; //-nodisp -vn -autoexit
            s.StartInfo.Arguments = "-nofs -noquiet -identify -slave -nomouseinput -sub-fuzziness 1 -wid " + panel1.Handle + " " + path;
            s.StartInfo.UseShellExecute = false;
            s.StartInfo.CreateNoWindow = true;
            s.StartInfo.RedirectStandardInput = true;
            s.StartInfo.RedirectStandardOutput = true;
            s.EnableRaisingEvents = true;
            s.OutputDataReceived += (o, e) =>
            {
                //   Resizeplayer(o as Process); 
                try
                {
                    int percent = 0;
                    var g1 = e.Data.Split('\n').Where(tmp => tmp.Contains("ANS_PERCENT_POSITION")).FirstOrDefault();
                    if (g1 != null && g1.Count(tmp => tmp == '=') >= 1) if (int.TryParse(g1.Split('=').LastOrDefault(), out percent)) Invoke(new Action(() => hScrollBar1.Value = percent));
                }
                catch { }
            };
            s.Start();
            s.BeginOutputReadLine();
            //Thread.Sleep(200);
            s.MainWindowHandle.SetParent(this.Handle);
            //s.MainWindowHandle.ShowWindow((int)CommonFunctions.nCmdShow.SW_HIDE);
            return s;
        }

        bool SendCommand(string cmd)
        {
            try
            {
                if (p != null && p.HasExited == false)
                {
                    p.StandardInput.Write(cmd + "\n");
                    p.StandardInput.Flush();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        Process p;

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //axWindowsMediaPlayer1.URL = e.Node.Name;
            //axWindowsMediaPlayer1.Ctlcontrols.play(); 
            try
            {
                if (p != null && !p.HasExited)
                {
                    SendCommand("loadfile " + PrepareFilePath("\"" + e.Node.Name + "\""));
                }
                else
                {
                    p = OpenPlayer(PrepareFilePath("\"" + e.Node.Name + "\""));
                    //p.Exited += (o, eq) => { try { Invoke(new Action(() => e.Node.NodeFont = SystemFonts.DefaultFont)); } catch { } };
                    //e.Node.NodeFont = SystemFonts.CaptionFont;
                }
            }
            catch {  }
        }

        private string PrepareFilePath(string filePath)
        {
            string preparedPath = filePath;

            if (Environment.OSVersion.ToString().IndexOf("Windows") > -1)
            {
                preparedPath = filePath.Replace("" + System.IO.Path.DirectorySeparatorChar, "" + System.IO.Path.DirectorySeparatorChar + System.IO.Path.DirectorySeparatorChar);
            }

            return preparedPath;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            label4.Text = e.Node.Index.ToString();
            label5.Text = e.Node.Name;
            if (e.Node.Text.Length > 19)
            {
                label6.Text = e.Node.Text.Substring(1, 6).Insert(2, "-").Insert(5, "-");
                label7.Text = e.Node.Text.Substring(8, 6).Insert(2, ":").Insert(5, ":");
                label9.Text = e.Node.Text.Substring(15, 6).Insert(2, ":").Insert(5, ":");
                new Thread(() => { this.Invoke(new Action(() => { label11.Text = FFprobe.FFprobe.GetDuration(e.Node.Name) + " c"; })); }).Start();
            }
        }

        private void panel1_Resize(object sender, EventArgs e)
        {

        }

        private void OthetCoder_Load(object sender, EventArgs e)
        {
            
        }

        private void воспроизвестиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendCommand("pause");
            timer1.Enabled = !timer1.Enabled;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                SendCommand("gamma 99 1");
                SendCommand("saturation 99 1");
                SendCommand("contrast 99 1");
            }
            else
            {
                SendCommand("gamma -99");
                SendCommand("saturation -99");
                SendCommand("contrast -99");             
            }
        }

        private void покадровоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendCommand("frame_step");
            timer1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (p != null && !p.HasExited)
            {
                SendCommand("pausing_keep_force get_percent_pos");
            }
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            //воспроизвестиToolStripMenuItem_Click(null,null);
            SendCommand("set_property percent_pos " + e.NewValue);
            воспроизвестиToolStripMenuItem_Click(null, null);
        }

        private void сохранитьВЖурналToolStripMenuItem1_Click(object sender, EventArgs e)
        {           
            DBFile File = new DBFile();
            SaveFileDialog svf = new SaveFileDialog();
            svf.Filter = "BD Data|*.bd";
            if (svf.ShowDialog() == DialogResult.OK) File.FileDB = svf.FileName;
            foreach (var item in treeView1.Nodes)
            {
                var o = item as TreeNode;
                var d = o.Text.Substring(1, 6);
                DBNode n = new DBNode()
                {
                    FilePath = o.Name,
                    Camera = "Created by OtherCoder",
                    Date = new DateTime(int.Parse(d.Substring(0, 2)), int.Parse(d.Substring(2, 2)), int.Parse(d.Substring(4))),
                    RecordTime = FFprobe.FFprobe.GetDuration(o.Name)
                };
                File.AddNode(n);
            }
            File.Save();
            MessageBox.Show("Успешно");
        }

        private void открытьИзФайлаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DBFile File = new DBFile();
            OpenFileDialog svf = new OpenFileDialog();
            svf.Filter = "BD Data|*.bd";
            if (svf.ShowDialog() == DialogResult.OK) File.FileDB = svf.FileName;
            File.Load();
            foreach (var item in File.DBNodes)
            {
                treeView1.Nodes.Add(item.FilePath, Path.GetFileName(item.FilePath));
            }
            MessageBox.Show("Успешно");
        }
    }
}
