using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IPCamera.Settings;
using IPCamera.Settings.Record;
using IPCamera.Network;
using System.IO;

namespace IPCamera.Record
{
    public partial class ManagerRecords : Form
    {
        uint Selected = 0;

        public Structures set;
        public Records rec;

        public RecEnamble RecEnamble
        {
            get
            {
                if (radioButton1.Checked) return RecEnamble.ON;
                else if (radioButton2.Checked) return RecEnamble.OFF;
                else return RecEnamble.Auto;
            }
            set
            {
                if (value == RecEnamble.ON) radioButton1.Checked = true;
                else if (value == RecEnamble.OFF) radioButton2.Checked = true;
                else radioButton3.Checked = true;
            }
        }
        public AutoEnabmle AutoloadRec
        {
            get
            {
                if (radioButton6.Checked) return AutoEnabmle.ON;
                else return AutoEnabmle.OFF;
            }
            set
            {
                if (value == AutoEnabmle.ON) radioButton6.Checked = true;
                else radioButton4.Checked = true;
            }
        }

        public DateTime StartRecord
        {
            get
            {
                return dateTimePicker1.Value;
            }
            set
            {
                try
                {
                    dateTimePicker1.Value = value;
                }
                catch { }
            }
        }
        public DateTime StopRecord
        {
            get
            {
                return dateTimePicker2.Value;
            }
            set
            {
                try
                {
                    dateTimePicker2.Value = value;
                }
                catch { }
            }
        }
        public StreamType StreamRec
        {
            get
            {
                if (radioButton5.Checked) return StreamType.Primary;
                else return StreamType.Secand;
            }
            set
            {
                if (value == StreamType.Primary) radioButton5.Checked = true;
                else radioButton7.Checked = true;
            }
        }

        public string PathRecord
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
                label5.Text = "Свободно " + formatFileSize((new DriveInfo(value.Substring(0,1))).AvailableFreeSpace/1024).ToString();
            }
        }
        public int RecordDuration
        {
            get
            {
                return int.Parse(maskedTextBox1.Text);
            }
            set
            {
                maskedTextBox1.Text = value.ToString();
            }
        }
        public int RecordCount
        {
            get
            {
                return int.Parse(maskedTextBox2.Text);
            }
            set
            {
                maskedTextBox2.Text = value.ToString();
            }
        }

        public ManagerRecords(uint Selected)
        {
            InitializeComponent();
            this.Selected = Selected;
            set = Structures.Load()[Selected];
            rec = set.Records;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            rec.Enamble = RecEnamble;
            rec.AutoLoad = AutoloadRec;
            rec.StartRecord = StartRecord;
            rec.StopRecord = StopRecord;
            rec.StreamType = StreamRec;
            rec.RecordFolder = PathRecord;
            rec.RecoedDuration = RecordDuration;
            rec.RecordsCount = RecordCount;
            set.Records = rec;
            var st = Structures.Load();
            st[Selected] = set;
            Structures.Save(st);
            MessageBox.Show("Успешно сохранено");
        }

        private string formatFileSize(long size)
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

        private void ManagerRecords_Load(object sender, EventArgs e)
        {
            RecEnamble = rec.Enamble;
            AutoloadRec = rec.AutoLoad;
            StartRecord = rec.StartRecord;
            StopRecord = rec.StopRecord;
            StreamRec = rec.StreamType;
            PathRecord = rec.RecordFolder;
            RecordDuration = rec.RecoedDuration;
            RecordCount = rec.RecordsCount;
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            FolderBrowserDialog opg = new FolderBrowserDialog();
            opg.Description = "Папка для записи видео с камеры " + set.IP;
            if(opg.ShowDialog() == DialogResult.OK)
            {
                PathRecord = opg.SelectedPath;               
            }
        }
    }
}
