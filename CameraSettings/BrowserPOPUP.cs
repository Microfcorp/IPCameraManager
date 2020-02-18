using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera.CameraSettings
{
    public partial class BrowserPOPUP : Form
    {
        public BrowserPOPUP(string html)
        {
            InitializeComponent();

            webBrowser1.DocumentText = (html);
        }

        private void BrowserPOPUP_Load(object sender, EventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }
}
