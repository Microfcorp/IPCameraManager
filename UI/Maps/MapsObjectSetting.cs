using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPCamera.UI.Maps
{
    public partial class MapsObjectSetting : Form
    {
        public MapsObjectSetting()
        {
            InitializeComponent();
            comboBox2.Items.Clear();
            var load = Settings.Structures.Load();
            for (int i = 0; i < load.Length; i++)
            {
                comboBox2.Items.Add(i + " - " + load[i].NameCamera + " (" + load[i].IP + ")");
            }
        }

        public int TypeObject
        {
            get => comboBox1.SelectedIndex;
            set => comboBox1.SelectedIndex = value;
        }

        public string NameObject
        {
            get => textBox1.Text;
            set => textBox1.Text = value;
        }

        public int Camera
        {
            get => comboBox2.SelectedIndex;
            set => comboBox2.SelectedIndex = value;
        }
        

        public string SelectFile
        {
            get => comboBox3.Text;
            set => comboBox3.Text = value;
        }

        public string[] ToLoadFile
        {
            get => toload.ToArray();
        }

        List<string> toload = new List<string>();

        public string[] Files
        {
            get
            {
                List<string> str = new List<string>();
                foreach (var item in comboBox3.Items)
                    str.Add(item.ToString());
                return str.ToArray();
            }
            set { comboBox3.Items.Clear(); comboBox3.Items.AddRange(value); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void MapsObjectSetting_Load(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog opg = new OpenFileDialog()
            {
                Title = "Открыть фота",
                Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG"
            };
            if(opg.ShowDialog() == DialogResult.OK)
            {
                comboBox3.Items.Add(Path.GetFileName(opg.FileName));
                toload.Add(opg.FileName);
            }
        }
    }
}
