using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera.PTZScenes
{
    public partial class Editor : Form
    {
        PTZScene monitor;
        Structures[] settings;
        int ids;
        public Editor(PTZScene ms, int id)
        {
            InitializeComponent();
            settings = Structures.Load();
            monitor = ms;
            ids = id;
            textBox1.Text = ms.Name;
            numericUpDown1.Value = (decimal)ms.Timeout;
            LoadTiles();
            comboBox1.SelectedIndex = 0;
        }

        private void LoadTiles()
        {
            comboBox1.Items.Clear();

            for (int i = 0; i < monitor.Tiles.Length; i++)
            {
                comboBox1.Items.Add(i);
            }
        }

        private void Editor_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ind = comboBox1.SelectedIndex;
            comboBox2.SelectedIndex = (int)monitor.Tiles[ind].Vector;
            numericUpDown1.Value = (decimal)monitor.Tiles[ind].Steep;
            numericUpDown2.Value = (decimal)monitor.Tiles[ind].SleepTime;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Add(comboBox1.Items.Count);
            var l = monitor.Tiles.ToList();
            l.Add(PTZTile.Null);
            monitor.Tiles = l.ToArray();
            comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var ind = comboBox1.SelectedIndex;
            var l = monitor.Tiles.ToList();
            l.RemoveAt(ind);
            monitor.Tiles = l.ToArray();
            comboBox1.Items.RemoveAt(ind);
            comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var ind = comboBox1.SelectedIndex;
            monitor.Tiles[ind].Vector = (ONVIF.PTZ.PTZParameters.Vector)comboBox2.SelectedIndex;
            monitor.Tiles[ind].Steep = (int)numericUpDown1.Value;
            monitor.Tiles[ind].SleepTime = (int)numericUpDown2.Value;
            LoadTiles();
            comboBox1.SelectedIndex = ind;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            monitor.Name = textBox1.Text;
            monitor.Timeout = (int)numericUpDown3.Value;

            var mons = PTZCollection.Load();
            mons[ids] = monitor;
            PTZCollection.Save(mons);
            MessageBox.Show("Успешно сохранено");

            LoadTiles();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            EditTrigger et = new EditTrigger(monitor);
            if(et.ShowDialog() == DialogResult.OK)
            {
                monitor.DefaultTriggerData = et.ReturnData;
                monitor.Trigger = et.ReturnTrigger;
            }
        }
    }
}
