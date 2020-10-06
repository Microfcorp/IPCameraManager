using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera.InfoCamera
{
    public partial class CameraUsers : Form
    {
        Structures structures;
        public CameraUsers(uint Selected)
        {
            InitializeComponent();
            structures = Structures.Load()[Selected];
        }

        private void CameraUsers_Load(object sender, EventArgs e)
        {
            UpdateTree();
        }
        private void UpdateTree()
        {
            treeView1.Nodes.Clear();

            var userslist = structures.GetONVIFController().Users;
            TreeNode tn = new TreeNode(structures.IP);

            foreach (var item in userslist.OrderBy(tmp => tmp.UserLevel != ODEV.UserLevel.Administrator))
            {
                var text = "Права доступа: " + item.UserLevel.ToString() + " Логин: " + item.Username + " Пароль: " + item.Password;
                tn.Nodes.Add(text);
            }

            treeView1.Nodes.Add(tn);
            treeView1.ExpandAll();
        }
    }
}
