using IPCamera.Settings;
using IPCamera.Monitors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPCamera.UI
{
    public partial class MonitorSettings : Form
    {
        uint mid;
        Monitors.MonitorSettings ms;

        /// <summary>
        /// Возникает при сохранении настроек
        /// </summary>
        public event EventHandler OnSave;

        public MonitorSettings()
        {
            InitializeComponent();
            numericUpDown1.Maximum = uint.MaxValue;
        }

        public MonitorSettings(uint id) : this()
        {
            mid = id;
            ms = MonitorsController.Load()[id];
            
        }

        void Start()
        {
            textBox1.Text = ms.Name;
            comboBox1.Items.Clear();
            for (int i = 0; i < ms.Monitors.Length; i++)
                comboBox1.Items.Add(i + " - " + (ms.Monitors[i].Enable ? "Активировано" : "Не активировано"));
            comboBox1.SelectedIndex = 0;
        }

        void SaveSettings()
        {
            ms.Name = textBox1.Text;
            var md = MonitorsController.Load();
            md[mid] = ms;
            MonitorsController.Save(md);
            OnSave?.Invoke(this, new EventArgs());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            groupBox1.Text = "Настройка элемента монитора - " + comboBox1.SelectedIndex;
            if (ms.Monitors[comboBox1.SelectedIndex].IP != null)
                comboBox2.SelectedIndex = Structures.Load().Select(tmp => tmp.IP).ToList().IndexOf(ms.Monitors[comboBox1.SelectedIndex].IP);
            else comboBox2.Text = "";
            numericUpDown1.Value = ms.Monitors[comboBox1.SelectedIndex].Position;
            checkBox1.Checked = ms.Monitors[comboBox1.SelectedIndex].Enable;
        }

        private void MonitorSettings_Load(object sender, EventArgs e)
        {
            comboBox2.Items.AddRange(Structures.Load().Select(tmp => tmp.NameCamera + " - " + tmp.IP).ToArray());
            Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(ms.Monitors.Length <= 1)
            {
                MessageBox.Show("Нельзя удалить последний элемент");
                return;
            }
            var l = ms.Monitors.ToList();
            l.RemoveAt(comboBox1.SelectedIndex);
            l.Skip(comboBox1.SelectedIndex).ToList().ForEach(tmp => tmp.Position -= 1);
            ms.Monitors = l.ToArray();
            Start();
        }

        private void MonitorSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            var d = MessageBox.Show("Сохранить настройки?", "Microf IP Camera Manager", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (d == DialogResult.Yes)
            {
                SaveSettings();
            }
            else if (d == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ms.Monitors[comboBox1.SelectedIndex].Enable = checkBox1.Checked;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ms.Monitors[comboBox1.SelectedIndex].IP = Structures.Load()[comboBox2.SelectedIndex].IP;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            ms.Monitors[comboBox1.SelectedIndex].Position = (uint)numericUpDown1.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var l = ms.Monitors.ToList();
            l.Add(Monitor.Null);
            ms.Monitors = l.ToArray();
        }
    }
}
