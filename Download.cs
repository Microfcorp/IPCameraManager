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
using DBFiles;
using System.Diagnostics;
using System.Threading;

namespace IPCamera
{
    public partial class Download : Form
    {
        Structures setting;
        Loading ld = new Loading();
        public Download(uint Selected)
        {
            InitializeComponent();
            setting = Structures.Load()[Selected];
            //ld.FormClosed += (a, b) => { if (b.CloseReason == CloseReason.UserClosing) Invoke(new Action(() => Close())); };
        }

        FileTreeNode[] GetServer(string startpath = "", bool showdownload = false)
        {
            Log = "Идет загрузка...";
            if(showdownload) new Thread(() => ld.ShowDialog()).Start();

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
                        tr = new FileTreeNode(path, /*GetServer(startpath + "" + path),*/ FileTreeNode.TypeNode.Directory, startpath + "" + path);
                        tr.SelectedImageKey = "f";
                        tr.ImageKey = "f";
                        tr.Nodes.Add("");
                    }
                    else
                    {
                        tr = new FileTreeNode(path, FileTreeNode.TypeNode.File, startpath + "" + path);
                        tr.SelectedImageKey = "v";
                        tr.ImageKey = "v";
                    }
                    tr.ContextMenuStrip = contextMenuStrip1;
                    tr.ToolTipText = date + " (" + (size) + ")";
                    list.Add(tr);
                }
            }
            if (showdownload) { ld.Invoke(new Action(() => ld.Close())); Log = "Загрузка завершена"; }
            return list.ToArray();
        }

        private string formatFileSize(int size)
        {
            var a = new string[] { "KB", "MB", "GB", "TB", "PB" };
            int pos = 0;
            while (size >= 1024)
            {
                size /= 1024;
                pos++;
            }
            return Math.Round((double)size, 2) + " " + a[pos];
        }

        void UpdateServer(string startpath = "")
        {
            Log = "Идет загрузка...";
            treeView1.Nodes.Clear();
            new Thread(() => ld.ShowDialog()).Start();

            var DeviceInfo = Downloading.GetDeviceParams(setting.URLToHTTPPort, setting.Name, setting.Password);
            label1.Text = "Свободно места на SD (" + formatFileSize(int.Parse(DeviceInfo["sdfreespace"])) + ") из " + formatFileSize(int.Parse(DeviceInfo["sdtotalspace"]));
            progressBar2.Maximum = int.Parse(DeviceInfo["sdtotalspace"]);
            progressBar2.Value = int.Parse(DeviceInfo["sdtotalspace"]) - int.Parse(DeviceInfo["sdfreespace"]);
            label2.Text = (DeviceInfo["sdstatus"].Contains("Ready")) ? "SD карта работает исправно" : "Ошибка SD карты";
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
                    tr.ToolTipText = date + " (" + size + ")";
                    tr.ContextMenuStrip = contextMenuStrip1;
                    treeView1.Nodes.Add(tr);
                }
            }
            if (backgroundWorker1.IsBusy) backgroundWorker1.CancelAsync();
            if (!backgroundWorker1.IsBusy) backgroundWorker1.RunWorkerAsync(DateTime.Now - DateTime.Parse(DeviceInfo["startdate"]));
            ld.Invoke(new Action(() => ld.Close()));
            Log = "Загрузка завершена";
        }

        FileTreeNode selected;

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
                progressBar1.Maximum += 100;
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
            progressBar1.Maximum = 0;
            progressBar1.Value = 0;
            progressBar1.Style = ProgressBarStyle.Blocks;
            Sizefiles.Clear();

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
            using (WebDownload wc = new WebDownload())
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

        SortedList<string, byte> Sizefiles = new SortedList<string, byte>();

        // Event to track the progress
        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var WC = sender as WebDownload;

            if (!Sizefiles.ContainsKey(WC.UriDownload))
            {
                Sizefiles.Add(WC.UriDownload, (byte)e.ProgressPercentage);
                //Console.WriteLine(WC.UriDownload);
            }

            Sizefiles[WC.UriDownload] = (byte)e.ProgressPercentage;

            var percents = 0x00;
            foreach (var item in Sizefiles)
                percents += (int)item.Value;

            progressBar1.Value = percents;

            //progressBar1.Style = ProgressBarStyle.Marquee;
            //progressBar1.Value = progressBar1.Maximum;

            Log = "Идет скачивание файлов... " + (int)((decimal)percents).Map(0, progressBar1.Maximum, 0, 100) + "%";

            if (Sizefiles.Values.Sum(x => (int)x) == progressBar1.Maximum)
            {
                Log = "Скачивание файлов завершено";
                //progressBar1.Style = ProgressBarStyle.Blocks;
                progressBar1.Value = progressBar1.Minimum;
                //progressBar1.Style = ProgressBarStyle.Marquee;
                //MessageBox.Show("Скачивание завершено");
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
            sending.Add("-planrec_chn", comboBox1.SelectedIndex == 0 ? "11" : "12");
            sending.Add("-planrec_time", numericUpDown1.Value.ToString());
            sending.Add("-planrec_type", "1");

            var resp = (Downloading.SendRequest(DownloadingPaths.ToPath(setting.IP) + DownloadingPaths.DeviceParam, setting.Name, setting.Password, sending));

            if (resp.Contains("[Succeed]set ok. system reboot."))
                MessageBox.Show("Настройки успешно изменены. Камера будет автоматически перезагружена");
            else
                MessageBox.Show("Настройки успешно изменены.");
        }

        private string TimeSpanToString(TimeSpan ts)
        {
            if (ts.Days == 0)
                return String.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
            else
                return String.Format("{0} days {1}:{2}:{3}", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            TimeSpan tp = (TimeSpan)e.Argument;
            while (true)
            {
                tp = tp.Add(new TimeSpan(0, 0, 1));

                if (label6.InvokeRequired)
                    label6?.Invoke(new Action(() =>
                    {
                        label6.Text = "Камера работает: " + TimeSpanToString(tp);
                    }));

                System.Threading.Thread.Sleep(1000);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.Description = "Папка для загрузки файлов";

            if (fd.ShowDialog() == DialogResult.OK)
            {
                OpenFileDialog opg = new OpenFileDialog();
                opg.Filter = "DB File|*.db";

                if (opg.ShowDialog() == DialogResult.OK)
                {
                    var db = DBFile.Read(opg.FileName);
                    progressBar1.Maximum = 0;
                    progressBar1.Value = 0;
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    Sizefiles.Clear();

                    foreach (var item in db)
                    {
                        //MessageBox.Show(DownloadingPaths.ToPath(setting.URLToHTTPPort) + DownloadingPaths.SD + item.HTTPPath);
                        progressBar1.Maximum += 100;
                        BtnDownload(DownloadingPaths.ToPath(setting.URLToHTTPPort) + DownloadingPaths.SD + item.HTTPPath, fd.SelectedPath + "\\" + item.HTTPPath.Split('/').Where(x => x.Contains(".")).ToArray()[0]);
                    }
                }
            }
        }

        private void скачатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selected == null) return;
            progressBar1.Maximum = 0;
            progressBar1.Value = 0;
            progressBar1.Style = ProgressBarStyle.Blocks;
            Sizefiles.Clear();

            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.Description = "Папка для загрузки файлов";

            if (fd.ShowDialog() == DialogResult.OK)
            {
                progressBar1.Maximum += 100;
                BtnDownload(DownloadingPaths.ToPath(setting.URLToHTTPPort) + DownloadingPaths.SD + selected.URL, fd.SelectedPath + "\\" + selected.URL.Split('/').Where(x => x.Contains(".")).ToArray()[0]);
                //MessageBox.Show(DownloadingPaths.ToPath(setting.URLToHTTPPort) + DownloadingPaths.SD + selected.URL);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            selected = (FileTreeNode)e.Node;
            просмотрToolStripMenuItem.Enabled = selected.typeNode == FileTreeNode.TypeNode.File /*&& selected.URL.EndsWith(".jpg")*/;
        }
        //bool IsPlay = false;
        private void смотретьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache) + "\\" + "VideoFile.264";
            if (selected == null) return;
            progressBar1.Maximum = 0;
            progressBar1.Value = 0;
            progressBar1.Style = ProgressBarStyle.Blocks;
            Sizefiles.Clear();
            
            progressBar1.Maximum += 100;
            BtnDownload(DownloadingPaths.ToPath(setting.URLToHTTPPort) + DownloadingPaths.SD + selected.URL, path);
            
        }

        private void просмотрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selected.typeNode == FileTreeNode.TypeNode.File && selected.URL.EndsWith(".jpg"))
            {
                var url = DownloadingPaths.ToPath(setting.URLToHTTPPort) + DownloadingPaths.SD + selected.URL;
                var img = Downloading.GetImageWitchAutorized(url, setting.Login, setting.Password);
                ImagePlayer ip = new ImagePlayer(img, url);
                ip.Show();
            }
            else if(selected.typeNode == FileTreeNode.TypeNode.File && !selected.URL.EndsWith(".jpg"))
            {
                new Thread(() => ld.ShowDialog()).Start();
                var url = DownloadingPaths.ToPath(setting.URLToHTTPPort) + DownloadingPaths.SD + selected.URL;
                var path = Downloading.DownloadFile(url, setting.Name, setting.Password, Environment.GetFolderPath(Environment.SpecialFolder.MyVideos, Environment.SpecialFolderOption.Create) + "\\IPCamera\\");
                if (File.Exists(path + ".h264"))
                    File.Delete(path + ".h264");
                File.Move(path, path + ".h264");
                Convert cnv = new Convert(new string[] { path + ".h264" }, true);
                cnv.Show();
                cnv.IsPlay = true;
                ld.Invoke(new Action(() => ld.Close()));
            }
        }

        private void скопироватьСсылкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(DownloadingPaths.ToPath(setting.URLToHTTPPort) + DownloadingPaths.SD + selected.URL);
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            var nod = e.Node as FileTreeNode;
            if (nod.Nodes[0].Text == "")
            {
                nod.Nodes.Clear();
                nod.Nodes.AddRange(GetServer(nod.URL, true));
            }
        }
    }
}

                    /*if (path.EndsWith(".db"))
                    {
                        FileTreeNode tr;
                        tr = new FileTreeNode(path, FileTreeNode.TypeNode.File, startpath + "" + path);
                        tr.SelectedImageKey = "v";
                        tr.ImageKey = "v";
                        tr.ContextMenuStrip = contextMenuStrip1;
                        tr.ToolTipText = date + " (" + (size) + ")";
                        list.Add(tr);
                        Downloading.DownloadFile(DownloadingPaths.ToPath(setting.URLToHTTPPort) + DownloadingPaths.SD + startpath + "/" + path, setting.Name, setting.Password, Environment.CurrentDirectory + "\\" + size);
                        var fil = DBFile.Read(Environment.CurrentDirectory + "\\" + size + "\\" + path);
                        foreach (var abc in fil)
                        {
                            FileTreeNode rt;
                            rt = new FileTreeNode(abc.FileName, FileTreeNode.TypeNode.File, DownloadingPaths.ToPath(setting.URLToHTTPPort) + DownloadingPaths.SD + "/" + abc.HTTPPath);
                            rt.SelectedImageKey = "v";
                            rt.ImageKey = "v";
                            rt.ContextMenuStrip = contextMenuStrip1;
                            rt.ToolTipText = abc.FileNameNoneExtension + " (" + (abc.Type) + ")";
                            list.Add(rt);
                        }

                        File.Delete(Environment.CurrentDirectory + "\\" + size + "\\" + path);
                        Directory.Delete(Environment.CurrentDirectory + "\\" + size);
                    }*/
