using IPCamera.Network;
using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera.CameraSettings
{
    public partial class Email : Form
    {
        Structures structures;
        public string SaveLink
        {
            get
            {
                return comboBox2.SelectedIndex.ToString();
            }
            set
            {
                comboBox2.SelectedIndex = int.Parse(value);
            }
        }
        public string Authenication
        {
            get
            {
                return checkBox1.Checked ? "1" : "0";
            }
            set
            {
                checkBox1.Checked = value == "1" ? true : false;
            }
        }
        public string Server
        {
            get
            {
                return comboBox1.Text;
            }
            set
            {
                comboBox1.Text = value;
            }
        }
        public string Port
        {
            get
            {
                return numericUpDown1.Value.ToString();
            }
            set
            {
                numericUpDown1.Value = int.Parse(value);
            }
        }
        public string Login
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }
        public string Password
        {
            get
            {
                return textBox2.Text;
            }
            set
            {
                textBox2.Text = value;
            }
        }
        public string To
        {
            get
            {
                return textBox3.Text;
            }
            set
            {
                textBox3.Text = value;
            }
        }
        public string From
        {
            get
            {
                return textBox4.Text;
            }
            set
            {
                textBox4.Text = value;
            }
        }
        public string Theme
        {
            get
            {
                return textBox5.Text;
            }
            set
            {
                textBox5.Text = value;
            }
        }
        public string Message
        {
            get
            {
                return textBox6.Text;
            }
            set
            {
                textBox6.Text = value;
            }
        }
        public Email(uint Selected)
        {
            InitializeComponent();
            structures = Structures.Load()[Selected];
        }

        private void Email_Load(object sender, EventArgs e)
        {
            UpdateSettings();
        }

        private void UpdateSettings()
        {
            var Params = Downloading.GetEmail(structures.IP, structures.Name, structures.Password);
            Server = Params["ma_server"];
            SaveLink = Params["ma_ssl"];
            Port = Params["ma_port"];
            Authenication = Params["ma_logintype"];
            Login = Params["ma_username"];
            Password = Params["ma_password"];
            To = Params["ma_to"];
            From = Params["ma_from"];
            Theme = Params["ma_subject"];
            Message = Params["ma_text"];
        }

        private void Save()
        {
            SortedList<string, string> sending = new SortedList<string, string>();
            sending.Add("cmd", "setsmtpattr");
            sending.Add("cururl", "http://" + structures.URLToHTTPPort + "web/email.html");
            sending.Add("-ma_server", Server);
            sending.Add("-ma_from", From);
            sending.Add("-ma_to", To);
            sending.Add("-ma_subject", Theme);
            sending.Add("-ma_text", Message);
            sending.Add("-ma_logintype", Authenication);
            sending.Add("-ma_username", Login);
            sending.Add("-ma_password", Password);
            sending.Add("-ma_port", Port);
            sending.Add("-ma_ssl", SaveLink);
            var resp = (Downloading.SendRequest(DownloadingPaths.ToPath(structures.IP) + DownloadingPaths.DeviceParam, structures.Name, structures.Password, sending));
            UpdateSettings();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = checkBox1.Checked;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 2) numericUpDown1.Value = 465;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0) comboBox2.SelectedIndex = 2;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0) textBox4.Text = textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var html = Downloading.GetTestSMTP(structures.IP, structures.Name, structures.Password);
            var doc = "";

            if (html["result"] == "-4") doc = "Неверный логин или пароль";
            if (html["result"] == "0") doc = "Успешно. В течении нескольких минут вам придет образцовое письмо";
            if (html["result"] == "-1") doc = "Неизветсная ошибка";
            if (html["result"] == "-2") doc = "Ошибка шифрования. Убедитесь, что выбран правильный метод шифрования";
            if (html["result"] == "-3") doc = "Ошибка подключения к серверу. Проверьте адрес сервера и порт";

            MessageBox.Show(doc);
        }
    }
}
