using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera
{
    public partial class TextInput : Form
    {
        public TextInput(string text, string InputValue = "")
        {
            InitializeComponent();
            label1.Text = text;
            richTextBox1.Text = InputValue;
        }

        private void TextInput_Load(object sender, EventArgs e)
        {

        }

        public string ReturnText
        {
            get => richTextBox1.Text;
        }
        public string ReturnTextNoLine
        {
            get => richTextBox1.Text.Replace("\n", "").Replace("\r", "");
        }

        private void button1_Click(object sender, EventArgs e)
        { 
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
