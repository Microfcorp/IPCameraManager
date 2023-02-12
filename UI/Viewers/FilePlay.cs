using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IPCamera.Settings;

namespace IPCamera.UI.Viewers
{
    public partial class FilePlay : Form
    {
        private string filepath;       

        public FilePlay()
        {
            InitializeComponent();
        }

        public FilePlay(string filepath, TypeViewers typeViewers = TypeViewers.FFPLAY) : this()
        {
            this.filepath = filepath;

            if (typeViewers == TypeViewers.FFPLAY)
            {
                ffplayer ffplayer1 = new ffplayer();
                ffplayer1.Dock = DockStyle.Fill;
                ffplayer1.Disposed += (o, q) => { Dispose(); };
                ffplayer1.FilePath = filepath;
                ffplayer1.StartFFPLAY();
                panel1.Controls.Add(ffplayer1);
            }
            else if (typeViewers == TypeViewers.MPlayer)
            {
                mplayer mplayer1 = new mplayer();
                mplayer1.Dock = DockStyle.Fill;
                mplayer1.Disposed += (o, q) => { Dispose(); };
                mplayer1.FilePath = filepath;
                mplayer1.Startmplayer();
                panel1.Controls.Add(mplayer1);
            }
            else
            {
                MessageBox.Show("Данный файл не возможно воспроизвести данным проишгрывателем");
                Close();
            }
        }
    }
}
