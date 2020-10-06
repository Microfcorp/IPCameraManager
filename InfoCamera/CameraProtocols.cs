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
    public partial class CameraProtocols : Form
    {
        Structures structures;
        public CameraProtocols(uint Selected)
        {
            InitializeComponent();
            structures = Structures.Load()[Selected];
        }

        private void CameraProtocols_Load(object sender, EventArgs e)
        {
            UpdateTree();
        }

        private void UpdateTree()
        {
            treeView1.Nodes.Clear();

            var protocolslist = structures.GetONVIFController().NetworkProtocols;
            TreeNode tn = new TreeNode(structures.IP);
            
            foreach (var item in protocolslist)
            {
                var text = "Сервис: " + item.Name.ToString() + " Порт: " + string.Join(" ", item.Port) + (item.Enabled ? " Включен" : "Выключен");
                tn.Nodes.Add(text);
            }

            treeView1.Nodes.Add(tn);
            treeView1.ExpandAll();
        }
    }
}
