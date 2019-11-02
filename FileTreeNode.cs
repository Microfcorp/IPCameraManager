using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera
{
    public class FileTreeNode : TreeNode
    {
        public enum TypeNode
        {
            Directory,
            File,
        }
        public TypeNode typeNode
        {
            get;
            set;
        }
        public String URL
        {
            get;
            set;
        }

        public FileTreeNode() : base() { }
        public FileTreeNode(string text, FileTreeNode[] children, TypeNode tp, string url) : base(text, children) { this.typeNode = tp; this.URL = url; }
        public FileTreeNode(string text, TypeNode tp, string url) : base(text) { this.typeNode = tp; this.URL = url; }
    }
}
