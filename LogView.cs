using IPCamera.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera
{
    public partial class LogView : Form
    {
        Settings.Structures sett;

        int typelog = 1; //0 - All, 1 - IR. 2 - !IR

        public LogView(uint Selected)
        {
            InitializeComponent();
            sett = Settings.Structures.Load()[Selected];
        }

        private void LogRun()
        {
            var logs = Downloading.GetLogDevice(sett.URLToHTTPPort, sett.Name, sett.Password);

            richTextBox1.Text = "";

            if(typelog == 0)
            {
                foreach (var item in logs.Nodes)
                {
                    richTextBox1.Text += item.date.ToString() + "  " + item.Text + Environment.NewLine;
                }
            }
            if (typelog == 1)
            {
                foreach (IRCut item in logs.Nodes.Where(x => x is IRCut))
                {
                    var one = item.perechod == IRCut.Perehod.Day_Night ? "ИК подсветка включена (ночь)" : "ИК подстветка выключена (день)";
                    richTextBox1.Text += item.date.ToString() + "  " + one + Environment.NewLine;
                }
            }
            if (typelog == 2)
            {
                foreach (var item in logs.Nodes.Where(x => !(x is IRCut)))
                {
                    richTextBox1.Text += item.date.ToString() + "  " + item.Text + Environment.NewLine;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LogRun();
        }

        private void LogView_Load(object sender, EventArgs e)
        {
            LogRun();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog opg = new OpenFileDialog();
            opg.Filter = "Текстовый документ|*.txt";

            if(opg.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(opg.FileName, richTextBox1.Text);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            typelog = comboBox1.SelectedIndex;
            LogRun();
        }
    }
}
