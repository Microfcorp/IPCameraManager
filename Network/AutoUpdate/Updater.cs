using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using IPCamera.DLL;

namespace IPCamera.Network.AutoUpdate
{
    public partial class Updater : Form
    {
        VersionInfo ver;
        public Updater(VersionInfo ver)
        {
            InitializeComponent();
            this.ver = ver;
            this.Flash(5);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(ver.Note);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
            using (WebDownload wc = new WebDownload())
            {
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += (object senders, AsyncCompletedEventArgs es) => 
                {
                    MessageBox.Show("Успешно загружено. Сейчас начнется установка");
                    Process.Start("UpdateDownloading.exe");
                    Application.Exit();
                };
                wc.DownloadFileAsync(
                    // Param1 = Link of file
                    new System.Uri(NetworkUpdate.URLServer + ver.URLDownload),
                    // Param2 = Path to save
                    "UpdateDownloading.exe"
                );
                Console.WriteLine(Downloading.GetHTML(NetworkUpdate.URLServer + NetworkUpdate.URLConfirm));
            }
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var WC = sender as WebDownload;
            progressBar1.Value = e.ProgressPercentage;
            label3.Text = e.ProgressPercentage + "%";
        }

        private void Updater_Move(object sender, EventArgs e)
        {
            this.StopFlashing();
        }
    }
}
