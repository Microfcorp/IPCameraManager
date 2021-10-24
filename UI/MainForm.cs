using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IPCamera.Network.AutoUpdate;
using IPCamera.Settings;

namespace IPCamera.UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var cams = Structures.Load();
            for (int i = 0; i < cams.Length; i++)
            {
                CameraMenuModule m = new CameraMenuModule(cams[i].IP);
                flowLayoutPanel1.Controls.Add(m);
            }
        }

        private void стараяПанельУправленияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var m = new Main(new ComandArgs.Parser(new string[0]));
            m.Show();
        }

        private void проверкаОбновленийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var vers = NetworkUpdate.GetVersionServer();
            if (vers > CurrentVersion.CurrentVersions)
                MessageBox.Show("Доспуна новая версия. Перезапустите приложение для обновления", "IPCamera Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("У вас уже установлена последняя версия");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            (new Viewers.GroupPlay()).OpenToWindowManager(this, "Групповой просмотр");
        }

        bool IsMonitors;

        private void button1_Click(object sender, EventArgs e)
        {
            if (!IsMonitors)
            {
                button1.BackColor = ProfessionalColors.CheckBackground;
                IsMonitors = true;
                flowLayoutPanel1.Controls.Clear();
                var cams = Monitors.MonitorsController.Load();
                for (int i = 0; i < cams.Length; i++)
                {
                    MonitorMenuModule m = new MonitorMenuModule((uint)i);
                    flowLayoutPanel1.Controls.Add(m);
                }
            }
            else
            {
                button1.BackColor = SystemColors.Control;
                IsMonitors = false;
                flowLayoutPanel1.Controls.Clear();
                OnLoad(null);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            (new ServiceForm()).OpenToWindowManager(this);
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
