using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IPCamera.Settings;

namespace IPCamera.UI.Viewers
{
    /// <summary>
    /// Тип плеера
    /// </summary>
    public enum TypePlayer : byte
    {
        /// <summary>
        /// Высокого качества
        /// </summary>
        HD,
        /// <summary>
        /// Низкого качества
        /// </summary>
        SD,
    }
    public partial class InjectedAutoPlayer : UserControl
    {
        /// <summary>
        /// Возвращет или задает тип данного плеера
        /// </summary>
        public TypeViewers Viever
        {
            get;
            set;
        }
        /// <summary>
        /// Тип плеера воспроизведения
        /// </summary>
        public TypePlayer Player
        {
            get;
            set;
        }
        /// <summary>
        /// Камера для воспроизведения
        /// </summary>
        public Structures Camera
        {
            get;
            set;
        }
        public InjectedAutoPlayer()
        {
            InitializeComponent();
        }
        public InjectedAutoPlayer(TypeViewers typeViewers, Structures Camera, TypePlayer tp = TypePlayer.SD) : this()
        {
            this.Camera = Camera;
            Viever = typeViewers;
            Player = tp;

            if (typeViewers == TypeViewers.ImageV)
            {
                ImageVPlayer play = new ImageVPlayer
                {
                    ImageURL = Camera.GetPhotoStream,
                    FPS = (byte)Settings.StaticMembers.ImageSettings.FPS,
                    Dock = DockStyle.Fill,
                };
                Controls.Add(play);
            }
            else if (typeViewers == TypeViewers.FFPLAY)
            {
                ffplayer ffplayer1 = new ffplayer();
                ffplayer1.Dock = DockStyle.Fill;
                ffplayer1.Disposed += (o, q) => { Dispose(); };
                ffplayer1.FilePath = Player == TypePlayer.SD ? Camera.GetRTSPSecondONVIF : Camera.GetRTSPFirstONVIF;
                ffplayer1.StartFFPLAY();
                Controls.Add(ffplayer1);
            }
        }

        private void InjectedAutoPlayer_Load(object sender, EventArgs e)
        {

        }
    }
}
