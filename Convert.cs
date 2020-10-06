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
        public const string PFfmpeg = "-r {0} -i {1} -i {2} -acodec copy -vcodec copy {3}"; //-vcodec copy CBR
        public const string PFfmpegVBR = " -i {1} -i {2} -acodec copy -vcodec copy {3}"; //-vcodec copy VBR

        public const string PFfmpegR = "-i {0} -i {1} -c:v libx264 -minrate 2M -maxrate 2M -b:v 2M -pix_fmt yuv420p -r 16 {2}"; //Раcширенное CBR
        public const string PFfmpegRVBR = "-i {0} -i {1} -c:v libx264 -minrate 2M -maxrate 2M -b:v 2M -pix_fmt yuv420p {2}"; //Раcширенное VBR


        public const string ffmpeg = "ffmpeg.exe";
        public const string converter = "convert.exe";
        public const string dbreader = "IPCameraDBParser.exe";

        string Final = null;
        decimal FPS;
        readonly List<string> Files = new List<string>();
        bool AutoConv;
        public Convert()
        {
            InitializeComponent();
            FPS = numericUpDown1.Value;
        }
        public Convert(string[] files, bool autostart = false) : this()
        {
            GetFiles(files);
            AutoConv = autostart;
            if (autostart)
            {
                RazhConv = false;
                StartConvert();
            }
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
                {
                    toolStripStatusLabel1.Text = "Конвертирование завершено";
                    if (AutoConv) Close();
                }
            }));
        }

        private void ConvertProg(object param)
        {
            var path = "\"" + param.ToString() + "\"";

            var pathname = Path.GetFileName(param.ToString());

            string path264 = (param.ToString().Contains(".264.h264")) ? param.ToString().Replace(".264.h264", "") : param.ToString();

            if (Final == null)
                Final = Path.GetDirectoryName(path264);

            if (File.Exists(Final + "\\" + pathname + "_ff.mkv"))
            {
                File.Delete(Final + "\\" + pathname + "_ff.mkv");
                if (File.Exists(Final + "\\" + pathname + "_ff.mkv"))
                {
                    MessageBox.Show("Файл уже существует. Ошибка автоматического удаления. Удалите файл и повторите попытку позднее");
                    toolStripStatusLabel1.Text = "Ошибка конвертирования";
                    return;
                }
            }

            /*Console.WriteLine(PFfmpeg, FPS.ToString().Replace(',','.'),
                "\"" + path264 + ".mp4" + "\"",
                "\"" + path264 + ".wav" + "\"",
                "\"" + Final + "\\" + pathname + "_ff.mkv" + "\"");*/

            var p1 = new Process
            {
                StartInfo = new ProcessStartInfo(converter, path)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            }; //запуск конвертера
            p1.Start();

            PlusPB();

            while (!p1.HasExited) ;
            PlusPB();                    

            var p2 = new Process(); //запуск конвертера
            p2.StartInfo.FileName = ffmpeg;
            if (!RazhConv)
            {
                p2.StartInfo.Arguments = String.Format(IsVBR1 ? PFfmpegVBR : PFfmpeg, //Запуск фмпега 
                    FPS.ToString().Replace(',', '.'),
                    "\"" + path264 + ".mp4" + "\"",
                    "\"" + path264 + ".wav" + "\"",
                    "\"" + Final + "\\" + pathname + "_ff.mkv" + "\""
                    );
                p2.StartInfo.UseShellExecute = false;
                p2.StartInfo.CreateNoWindow = true;
            }
            else
            {
                p2.StartInfo.Arguments = String.Format(IsVBR1 ? PFfmpegRVBR : PFfmpegR, //Запуск фмпега 
                    "\"" + path264 + ".mp4" + "\"",
                    "\"" + path264 + ".wav" + "\"",
                    "\"" + Final + "\\" + pathname + "_ff.mkv" + "\""
                    );
                //p2.StartInfo.UseShellExecute = false;
                //p2.StartInfo.CreateNoWindow = true;
            }
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

            if(IsPlay) Process.Start("mplayer.exe", checkedListBox1.Items[Selected] + "_ff.mkv -x 640 -y 360");
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
        /*
        private void GetFile(string file)
        {
            if (!Files.Contains(file))
            {
                Files.Add(file);
                checkedListBox1.Items.Add(file);
            }
        }
        */
        private void Convert_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                Description = "Открыть папку с .h264 видео"
            };

            if (fbd.ShowDialog() == DialogResult.OK)
                GetFiles(fbd.SelectedPath);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog opg = new OpenFileDialog
            {
                Filter = "Файлы .h264|*.h264",
                Multiselect = true
            };

            if (opg.ShowDialog() == DialogResult.OK)
                GetFiles(opg.FileNames);
        }

        private void StartConvert()
        {
            List<string> SelctedFile = new List<string>();
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i) || AutoConv)
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

        private void button3_Click(object sender, EventArgs e)
        {
            RazhConv = false;
            StartConvert();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Files.Clear();
            checkedListBox1.Items.Clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                Description = "Открыть папку для конечных видео",
                SelectedPath = Final
            };

            if (fbd.ShowDialog() == DialogResult.OK)
                Final = fbd.SelectedPath;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            FPS = numericUpDown1.Value;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog opg = new OpenFileDialog
            {
                Filter = "Файлы .db|*.db",
                Multiselect = true
            };

            if (opg.ShowDialog() == DialogResult.OK)
                Process.Start(dbreader, opg.FileName);
        }

        bool RazhConv = false;

        private void button7_Click(object sender, EventArgs e)
        {
            RazhConv = true;
            StartConvert();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, true);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, false);
        }

        int Selected = 0;

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Selected = checkedListBox1.SelectedIndex;
        }
        public bool IsPlay = false;

        public bool IsVBR1 { get; set; } = true;

        private void конвертироватьИВоспроизвестиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> SelctedFile = new List<string>
            {
                checkedListBox1.Items[Selected].ToString()
            };

            progressBar1.Value = 0;
            progressBar1.Maximum = SelctedFile.Count * 4;

            foreach (var item in SelctedFile.ToArray())
            {
                ConvertStart(item);
                button3.Enabled = false;
            }
            button3.Enabled = true;
            toolStripStatusLabel1.Text = "Конвертирование запущенно";

            IsPlay = true;           
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            IsVBR1 = true;
            label1.Visible = false;
            numericUpDown1.Visible = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            IsVBR1 = false;
            label1.Visible = true;
            numericUpDown1.Visible = true;
        }
    }
}
