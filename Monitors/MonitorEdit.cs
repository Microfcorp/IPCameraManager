using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera.Monitors
{
    public partial class MonitorEdit : Form
    {
        MonitorSettings monitor;
        Structures[] settings;
        int ids;
        public MonitorEdit(MonitorSettings ms, int id)
        {
            InitializeComponent();
            settings = Structures.Load();
            monitor = ms;
            ids = id;
            textBox1.Text = ms.Name;
            LoadCameras();

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private void LoadCameras()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();

            foreach (var item in monitor.Monitors)
            {
                if(settings[item.Camera].IP != null)
                {
                    comboBox1.Items.Add(item.Position + " - " + settings[item.Camera].IP);
                }
            }
            comboBox2.Items.AddRange(settings.Select(tmp => tmp.IP).ToArray());
        }

        private void MonitorEdit_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ind = comboBox1.SelectedIndex;
            comboBox2.Text = settings[monitor.Monitors[ind].Camera].IP;
            checkBox1.Checked = monitor.Monitors[ind].Enable;
            groupBox1.Text = settings[monitor.Monitors[ind].Camera].IP;
            numericUpDown1.Value = (decimal)monitor.Monitors[ind].Position;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Add("localhost");
            var l = monitor.Monitors.ToList();
            l.Add(new Monitor(0, (uint)comboBox1.Items.Count-1, true));
            monitor.Monitors = l.ToArray();
            comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var ind = comboBox1.SelectedIndex;
            monitor.Monitors[ind].Position = (uint)numericUpDown1.Value;
            monitor.Monitors[ind].Enable = checkBox1.Checked;
            monitor.Monitors[ind].Camera = (uint)comboBox2.SelectedIndex;
            groupBox1.Text = settings[monitor.Monitors[ind].Camera].IP;
            LoadCameras();
            comboBox1.SelectedIndex = ind;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            monitor.Name = textBox1.Text;

            var mons = MonitorsController.Load();
            mons[ids] = monitor;
            MonitorsController.Save(mons);
            MessageBox.Show("Успешно сохранено");

            LoadCameras();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                var ind = (uint)numericUpDown1.Value;

                if (monitor.Monitors.Where(tmp => tmp.Position == ind && settings[tmp.Camera].IP != settings[comboBox2.SelectedIndex].IP).Count() > 0)
                {
                    MessageBox.Show("Данная позиция уже занята. Пожалуйста, используйте другую");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var ind = comboBox1.SelectedIndex;
            var l = monitor.Monitors.ToList();
            l.RemoveAt(ind);
            monitor.Monitors = l.ToArray();
            comboBox1.Items.RemoveAt(ind);
            comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
        }
    }
}
