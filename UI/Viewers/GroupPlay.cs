using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IPCamera.GroupV;

namespace IPCamera.UI.Viewers
{
    public partial class GroupPlay : Form
    {
        public GroupPlay()
        {
            InitializeComponent();
        }

        public new int Padding = Settings.StaticMembers.ImageSettings.Padding;
        int SizeSET = 1;

        private void GroupPlay_Load(object sender, EventArgs e)
        {
            var cams = Structures.Load();
            SizeSET = cams.Length;
            for (int i = 0; i < cams.Length; i++)
            {
                if (cams[i].IsActive)
                {
                    var Player = new InjectedAutoPlayer(cams[i].GroupPlay, cams[i]);
                    flowLayoutPanel1.Controls.Add(Player);
                }
            }
            flowLayoutPanel1.ResizeF();
        }   

        private void GroupPlay_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void GroupPlay_Resize(object sender, EventArgs e)
        {
            flowLayoutPanel1.ResizeF();
        }
    }
}
