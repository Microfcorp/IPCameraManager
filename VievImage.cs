using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IPCamera.Settings;

namespace IPCamera
{
    public partial class VievImage : PictureBox
    {
        private int fPS;
        private float _fPS;
        private Structures set;
        //private bool usingPTZ;

        /// <summary>
        /// Сслылка на поток
        /// </summary>
        public string URL
        {
            get;
            set;
        }
        /// <summary>
        /// Количество кадров в секунду
        /// </summary>
        public float FPS
        {
            get => _fPS;
            set { _fPS = value; fPS = (int)((float)(1 / value) * 1000); timer1.Interval = fPS; }
        }
        /// <summary>
        /// Необходима ли авторизация
        /// </summary>
        public bool IsAuthorized
        {
            get;
            set;
        }
        /// <summary>
        /// Работает ли
        /// </summary>
        public bool Running
        {
            get;
            set;
        }
        /// <summary>
        /// Было ли загружено изображение
        /// </summary>
        public bool IsLoad
        {
            get;
            private set;
        }
        /*
        /// <summary>
        /// Использовать PTZ
        /// </summary>
        public bool UsingPTZ
        {
            get => usingPTZ;
            set 
            { 
                usingPTZ = value; 
                (contextMenuStrip1.Items[3] as ToolStripMenuItem).Checked = UsingPTZ;
            }
        }*/

        /*public event MouseEventHandler PictureMouseMove;
        public event MouseEventHandler PictureMouseClick;
        public event MouseEventHandler PictureMouseWheel;
        public event EventHandler PictureDoubleClick;
        public event EventHandler PictureClick;
        public event KeyEventHandler PictureKeyDown;
        */
        public VievImage()
        {
            InitializeComponent();
            FPS = 15;
            Running = false;

            contextMenuStrip1.Items[0].Click += (o, e) =>
            {
                Clipboard.SetText(set.GetPhotoStreamFirstONVIF);
            };
            contextMenuStrip1.Items[1].Click += (o, e) =>
            {
                Clipboard.SetImage(Image);
            };
            contextMenuStrip1.Items[2].Click += (o, e) =>
            {
                MessageBox.Show(set.IP);
            };
        }
        /*private void Podpis(Control ctl)
        {
            ctl.MouseMove += PictureMouseMove;
            ctl.MouseClick += PictureMouseClick;
            ctl.MouseWheel += PictureMouseWheel;
            ctl.DoubleClick += PictureDoubleClick;
            ctl.Click += PictureClick;
            ctl.KeyDown += PictureKeyDown;
        }*/
        public VievImage(Structures set, float FPS = 25f) : this()
        {
            Start(set, FPS);
        }

        public void AddContentMenu(ToolStripMenuItem[] tsi)
        {
            contextMenuStrip1.Items.Add(new ToolStripSeparator());
            foreach (var item in tsi)
            {
                contextMenuStrip1.Items.Add(item);
            }
        }
        public void AddContentMenu(ToolStripMenuItem tsi)
        {
            //contextMenuStrip1.Items.Add(new ToolStripSeparator());
            contextMenuStrip1.Items.Add(tsi);
        }

        public void Start(Structures set, float FPS = 25f)
        {
            this.set = set;
            this.FPS = FPS;

            timer1.Start();
            URL = set.TypeCamera == Network.Network.TypeCamera.HI3510 ? set.GetPhotoStream : set.GetPhotoStreamSecondONVIF;
            Running = true;

            WaitOnLoad = false;
            LoadAsync(URL);
            LoadCompleted += (o, e) => { if (e.Error != null) IsAuthorized = true; IsLoad = true; };
        }

        /// <summary>
        /// Блокирует поток до окончания ассинхронной загрузки изображения и возвращает изображение
        /// </summary>
        public Image AsyncImage
        {
            get
            {
                while (!IsLoad) ;
                return Image;
            }
        }

        protected void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Running)
                {
                    if (IsAuthorized) Image = Network.Downloading.GetImageWitchAutorized(URL, set.Login, set.Password);
                    else LoadAsync(URL);

                    Update();
                }
            }
            catch
            {

            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }
    }
}
