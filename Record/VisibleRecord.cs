using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IPCamera.DLL;

namespace IPCamera.Record
{
    public partial class VisibleRecord : Form
    {
        uint Selected;
        public VisibleRecord(uint Selected)
        {
            InitializeComponent();
            this.Selected = Selected;
            Text = "Архив записей - " + Settings.Structures.Load()[Selected].IP;
        }

        private void UpdateTree()
        {
            treeView1.Nodes.Clear();
            var Path = Settings.Structures.Load()[Selected].Records.RecordFolder;
            var _dir = Directory.GetFiles(Path, "*.mkv");
            foreach (var item in _dir)
            {
                TreeNode tr = new TreeNode(new FileInfo(item).Name);
                tr.Name = item;
                treeView1.Nodes.Add(tr);
            }
        }

        private void VisibleRecord_Load(object sender, EventArgs e)
        {
            UpdateTree();
        }

        private string formatFileSize(long size)
        {
            size /= 1024;
            var a = new string[] { "KB", "MB", "GB", "TB", "PB" };
            int pos = 0;
            while (size >= 1024)
            {
                size /= 1024;
                pos++;
            }
            return Math.Round((double)size, 2) + " " + a[pos];
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            /*label2.Text = e.Node.Text;
            label3.Text = formatFileSize(new FileInfo(e.Node.Name).Length);
            label5.Text = new FileInfo(e.Node.Name).LastWriteTime.ToString();
            label7.Text = e.Node.Text.Split('-').Last().Split('.')[0];
            t = e.Node;*/
        }

        TreeNode t;

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            axWindowsMediaPlayer1.URL = treeView1.SelectedNode.Name;
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.URL = treeView1.SelectedNode.Name;
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить этот файл?", Text, MessageBoxButtons.YesNo) == DialogResult.Yes && t != null)
                t.Name.DeleteFile();
            UpdateTree();
        }

        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            label2.Text = e.Node.Text;
            label3.Text = formatFileSize(new FileInfo(e.Node.Name).Length);
            label5.Text = new FileInfo(e.Node.Name).LastWriteTime.ToString();
            label7.Text = e.Node.Text.Split('-').Last().Split('.').First();
            label10.Text = e.Node.Text.Split(' ').First();
            t = e.Node;
        }
    }
}
