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
using HtmlAgilityPack;
using System.Net;
using System.IO;

namespace IPCamera
{
    public partial class Download : Form
    {
        Structures setting = Structures.Load();
        public Download()
        {
            InitializeComponent();
        }

        FileTreeNode[] GetServer(string startpath = "")
        {
            Log = "Идет загрузка...";

            var web = (Downloading.GetHTML(DownloadingPaths.ToPath(setting.URLToHTTPPort) + DownloadingPaths.SD + startpath, setting.Name, setting.Password));

            var list = new List<FileTreeNode>();

            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(web);

            var table = html.DocumentNode.SelectSingleNode("/html/body/pre/table");
            var tabletr = table.ChildNodes.Where(x => x.Name == "tr").ToList();

            tabletr.RemoveRange(0, 3);

            foreach (var item in tabletr)
            {
                if (item.ChildNodes.Count > 2)
                {
                    var path = item.ChildNodes[0].ChildNodes[0].InnerHtml.Replace("&nbsp;", "");
                    var date = item.ChildNodes[1].ChildNodes[0].InnerHtml.Replace("&nbsp;", "");
                    var size = item.ChildNodes[2].ChildNodes[0].InnerHtml.Replace("&nbsp;", "");

                    //Console.WriteLine(path);
                    //Console.WriteLine(date);
                    //Console.WriteLine(type);
                    //Console.WriteLine("----------");

                    FileTreeNode tr;

                    if (size == "[DIRECTORY]")
                    {
                        tr = new FileTreeNode(path, GetServer(startpath + "" + path), FileTreeNode.TypeNode.Directory, startpath + "" + path);
                        tr.SelectedImageKey = "f";
                        tr.ImageKey = "f";
                    }
                    else
                    {
                        tr = new FileTreeNode(path, FileTreeNode.TypeNode.File, startpath + "" + path);
                        tr.SelectedImageKey = "v";
                        tr.ImageKey = "v";
                    }

                    list.Add(tr);
                }
            }

            return list.ToArray();
        }

        private string formatFileSize(int size)
        {
            var a = new string[] { "KB", "MB", "GB", "TB", "PB" };
            int pos = 0;
            while (size >= 1024) {
                size /= 1024;
                pos++;
            }
            return Math.Round((double)size, 2) + " " + a[pos];
        }

        void UpdateServer(string startpath = "")
        {
            Log = "Идет загрузка...";
            treeView1.Nodes.Clear();

            var DeviceInfo = Downloading.GetDeviceParams(setting.URLToHTTPPort, setting.Name, setting.Password);
            label1.Text = "Свободно места на SD (" + formatFileSize(int.Parse(DeviceInfo["sdfreespace"])) + ") из " + formatFileSize(int.Parse(DeviceInfo["sdtotalspace"]));
            progressBar2.Maximum = int.Parse(DeviceInfo["sdtotalspace"]);
            progressBar2.Value = int.Parse(DeviceInfo["sdtotalspace"]) - int.Parse(DeviceInfo["sdfreespace"]);
            label2.Text = (DeviceInfo["sdstatus"] == "Ready") ? "Ошибка SD карты" : "SD карта работает исправно";
            label3.Text = "Камера запущенна: " + DeviceInfo["startdate"];
            label6.Text = "Камера работает: " + (DateTime.Now - DateTime.Parse(DeviceInfo["startdate"])).ToString().Split('.')[0];

            checkBox1.Checked = DeviceInfo["planrec_enable"].Contains("1");
            numericUpDown1.Value = int.Parse(DeviceInfo["planrec_time"]);
            comboBox1.SelectedIndex = DeviceInfo["planrec_chn"] == "11" ? 1 : 0;

            var web = (Downloading.GetHTML(DownloadingPaths.ToPath(setting.URLToHTTPPort) + DownloadingPaths.SD + startpath, setting.Name, setting.Password));

            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(web);

            var table = html.DocumentNode.SelectSingleNode("/html/body/pre/table");
            var tabletr = table.ChildNodes.Where(x => x.Name == "tr").ToList();

            tabletr.RemoveRange(0, 3);

            foreach (var item in tabletr)
            {
                if (item.ChildNodes.Count > 2)
                {
                    var path = item.ChildNodes[0].ChildNodes[0].InnerHtml.Replace("&nbsp;", "");
                    var date = item.ChildNodes[1].ChildNodes[0].InnerHtml.Replace("&nbsp;", "");
                    var size = item.ChildNodes[2].ChildNodes[0].InnerHtml.Replace("&nbsp;", "");

                    //Console.WriteLine(path);
                    //Console.WriteLine(date);
                    //Console.WriteLine(type);
                    //Console.WriteLine("----------");

                    FileTreeNode tr = new FileTreeNode(path, GetServer(startpath + "" + path), FileTreeNode.TypeNode.Directory, startpath + "" + path);
                    treeView1.Nodes.Add(tr);
                }
            }

            backgroundWorker1.RunWorkerAsync(DateTime.Now - DateTime.Parse(DeviceInfo["startdate"]));
            Log = "Загрузка завершена";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateServer();
        }

        private string Log
        {
            get
            {
                return toolStripStatusLabel2.Text;
            }
            set
            {
                toolStripStatusLabel2.Text = value;
            }
        }

        public int PercentsDownload
        {
            get
            {
                return (progressBar1.Value / progressBar1.Maximum) * 100;
            }
        }

        private void ParseChild(FileTreeNode fs, string pathdown)
        {
            if (fs.typeNode == FileTreeNode.TypeNode.File & fs.Checked)
            {
                //progressBar1.Maximum += 100;
                //Directory.CreateDirectory(pathdown + fs.URL);
                BtnDownload(DownloadingPaths.ToPath(setting.URLToHTTPPort) + DownloadingPaths.SD + fs.URL, pathdown + "\\" + fs.URL.Split('/').Where(x => x.Contains(".")).ToArray()[0]);
            }
            else if (fs.typeNode == FileTreeNode.TypeNode.Directory)
            {
                foreach (var item in fs.Nodes)
                {
                    var nods = item as FileTreeNode;
                    ParseChild(nods, pathdown);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Maximum = 100;
            progressBar1.Style = ProgressBarStyle.Blocks;

            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.Description = "Папка для загрузки файлов";

            if (fd.ShowDialog() == DialogResult.OK)
            {
                foreach (var item in treeView1.Nodes)
                {
                    var nods = item as FileTreeNode;
                    ParseChild(nods, fd.SelectedPath);
                }
            }
        }

        private void Download_Load(object sender, EventArgs e)
        {
            timer1.Start();
            treeView1.ImageList = new ImageList();
            treeView1.ImageList.Images.Add("f", Properties.Resources.of);
            treeView1.ImageList.Images.Add("v", Properties.Resources.ov);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            UpdateServer();
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            foreach (TreeNode child in e.Node.Nodes)
            {
                child.Checked = e.Node.Checked;
            }
        }

        private void BtnDownload(string path, string savepath)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) => { if (savepath.Contains(".264")) File.Move(savepath, savepath + ".h264"); };
                wc.DownloadFileAsync(
                    // Param1 = Link of file
                    new System.Uri(path),
                    // Param2 = Path to save
                    savepath
                );
            }
        }
        // Event to track the progress
        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //progressBar1.Value = e.ProgressPercentage;
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.Value = progressBar1.Maximum;

            Log = "Идет скачивание файлов... " /*+ progressBar1.Value + "%"*/;

            if (e.ProgressPercentage == 100)
            {
                Log = "Скачивание файлов завершено";
                progressBar1.Style = ProgressBarStyle.Blocks;
                progressBar1.Value = progressBar1.Minimum;
                //progressBar1.Style = ProgressBarStyle.Marquee;
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            SortedList<string, string> sending = new SortedList<string, string>();
            sending.Add("cmd", "setplanrecattr");
            sending.Add("cururl", "http://" + setting.URLToHTTPPort + "web/record.html");
            sending.Add("-planrec_enable", checkBox1.Checked ? "1" : "0");
            sending.Add("-planrec_chn", comboBox1.SelectedIndex == 0 ? "11": "12");
            sending.Add("-planrec_time", numericUpDown1.Value.ToString());
            sending.Add("-planrec_type", "1");

            var resp = (Downloading.SendRequest(DownloadingPaths.ToPath(setting.IP) + DownloadingPaths.DeviceParam, setting.Name, setting.Password, sending));

            if (resp.Contains("[Succeed]set ok. system reboot."))
                MessageBox.Show("Настройки успешно изменены. Камера будет автоматически перезагружена");
            else
                MessageBox.Show("Настройки успешно изменены.");
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            TimeSpan tp = (TimeSpan)e.Argument;
            while (true)
            {
                tp = tp.Add(new TimeSpan(0,0,1));

                if(label6.InvokeRequired)
                    label6?.Invoke(new Action(() =>
                    {
                        label6.Text = "Камера работает: " + tp.ToString().Split('.')[0];
                    }));

                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
