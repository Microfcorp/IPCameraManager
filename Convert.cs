using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace IPCamera
{
    public partial class Convert : Form
    {
        public const string PFfmpeg = "-r {0} -i {1} -i {2} -acodec copy -vcodec copy {3}"; //-vcodec copy

        public const string ffmpeg = "ffmpeg.exe";
        public const string converter = "convert.exe";
        public const string dbreader = "IPCameraDBParser.exe";

        string Final = null;
        decimal FPS;

        List<string> Files = new List<string>();
        public Convert()
        {
            InitializeComponent();
            FPS = numericUpDown1.Value;
        }

        private void ConvertStart(string path)
        {
            Thread th = new Thread(new ParameterizedThreadStart(ConvertProg));
            th.Start(path);
        }

        private void PlusPB()
        {
            progressBar1?.Invoke(new Action(() => {

                progressBar1.Value++;
                if (progressBar1.Value == progressBar1.Maximum)
                    toolStripStatusLabel1.Text = "Конвертирование завершено";
            }));
        }

        private void ConvertProg(object param)
        {
            var path = "\"" + param.ToString() + "\"";

            var pathname = Path.GetFileName(param.ToString());

            string path264 = (param.ToString().Contains(".264.h264")) ? param.ToString().Replace(".264.h264", "") : param.ToString();

            if (Final == null)
                Final = Path.GetDirectoryName(path264);

            /*Console.WriteLine(PFfmpeg, FPS.ToString().Replace(',','.'),
                "\"" + path264 + ".mp4" + "\"",
                "\"" + path264 + ".wav" + "\"",
                "\"" + Final + "\\" + pathname + "_ff.mkv" + "\"");*/

            var p1 = new Process(); //запуск конвертера
            p1.StartInfo = new ProcessStartInfo(converter, path);           
            p1.StartInfo.UseShellExecute = false;
            p1.StartInfo.CreateNoWindow = true;
            p1.Start();

            PlusPB();

            while (!p1.HasExited) ;
            PlusPB();

            var p2 = new Process(); //запуск конвертера
            p2.StartInfo = new ProcessStartInfo(ffmpeg, String.Format(PFfmpeg, //Запуск фмпега 
                FPS.ToString().Replace(',', '.'),
                "\"" + path264 + ".mp4" + "\"",
                "\"" + path264 + ".wav" + "\"", 
                "\"" + Final + "\\" + pathname + "_ff.mkv" + "\""
                ));
            p2.StartInfo.UseShellExecute = false;
            p2.StartInfo.CreateNoWindow = true;
            p2.Start();
            PlusPB();

            while (!p2.HasExited) ;
            PlusPB();
            try
            {
                p2.Kill();
                File.Delete(path264 + ".mp4");
                File.Delete(path264 + ".wav");
            }
            catch { }
        }

        private void GetFiles(string directory)
        {
            var dir = Directory.GetFiles(directory, "*.h264");            
            
            foreach (var item in dir)
            {
                if (!Files.Contains(item))
                {
                    Files.Add(item);
                    checkedListBox1.Items.Add(item);
                }
           }

            var dirs = Directory.GetDirectories(directory);
            foreach (var item in dirs)
            {
                GetFiles(item);
            }
        }
        private void GetFiles(string[] file)
        {          
            foreach (var item in file)
            {
                if (!Files.Contains(item))
                {                   
                    checkedListBox1.Items.Add(item);
                    Files.Add(item);
                }
            }
        }
        private void GetFile(string file)
        {
            if (!Files.Contains(file))
            {
                Files.Add(file);
                checkedListBox1.Items.Add(file);
            }
        }

        private void Convert_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Открыть папку с .h264 видео";

            if (fbd.ShowDialog() == DialogResult.OK)
                GetFiles(fbd.SelectedPath);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog opg = new OpenFileDialog();
            opg.Filter = "Файлы .h264|*.h264";
            opg.Multiselect = true;

            if (opg.ShowDialog() == DialogResult.OK)
                GetFiles(opg.FileNames);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<string> SelctedFile = new List<string>();
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if(checkedListBox1.GetItemChecked(i))
                    SelctedFile.Add(checkedListBox1.Items[i].ToString());
            }

            progressBar1.Value = 0;
            progressBar1.Maximum = SelctedFile.Count * 4;

            foreach (var item in SelctedFile.ToArray())
            {
                ConvertStart(item);
                button3.Enabled = false;                
            }
            button3.Enabled = true;
            toolStripStatusLabel1.Text = "Конвертирование запущенно";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Files.Clear();
            checkedListBox1.Items.Clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Открыть папку для конечных видео";
            fbd.SelectedPath = Final;

            if (fbd.ShowDialog() == DialogResult.OK)
                Final = fbd.SelectedPath;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            FPS = numericUpDown1.Value;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog opg = new OpenFileDialog();
            opg.Filter = "Файлы .db|*.db";
            opg.Multiselect = true;

            if (opg.ShowDialog() == DialogResult.OK)
                Process.Start(dbreader, opg.FileName);
        }
    }
}
