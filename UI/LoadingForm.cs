using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera.UI
{
    public partial class LoadingForm : Form
    {
        private bool isprocents = true;
        private uint procents;
        private uint maxprocents;
        private ImageLoadType imageLoadType;

        /// <summary>
        /// Использовать ли проценты загрузки
        /// </summary>
        public bool IsProcents
        {
            get => isprocents;
            set
            {
                isprocents = value;
                if (value)
                {
                    progressBar1.Visible = true;
                }
                else
                {
                    progressBar1.Visible = false;
                }
                Procents = Procents;
            }
        }
        /// <summary>
        /// Возвращает или задает количество процентов загрузки
        /// </summary>
        public uint Procents
        {
            get => procents;
            set
            {
                if (value > MaxProcents)
                    value = MaxProcents;
                procents = value;
                progressBar1.Value = (int)value;

                if (isprocents)
                    Text = BaseText + " (" + value + "%)";
                else
                    Text = BaseText;
            }
        }

        /// <summary>
        /// Возвращает или задает максимальное количество процентов загрузки
        /// </summary>
        public uint MaxProcents
        {
            get => maxprocents;
            set
            {
                maxprocents = value;
                if (Procents < value)
                    Procents = value;

                progressBar1.Maximum = (int)value;
            }
        }

        /// <summary>
        /// Базовый текст для заголовка
        /// </summary>
        public string BaseText
        {
            get;
            set;
        }

        public ImageLoadType ImageLoadType
        {
            get => imageLoadType;
            set
            {
                imageLoadType = value;
                if (value == ImageLoadType.Wait) 
                    pictureBox1.Image = Properties.Resources._4dc11d17f5292fd463a60aa2bbb41f6a_w200;
                else
                    pictureBox1.Image = Properties.Resources.Камера_крутится;
            }
        }
        public LoadingForm(string basetext = "Загрузка...", ImageLoadType imageloadtype = ImageLoadType.Camera)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;
            BaseText = basetext;
            ImageLoadType = imageloadtype;
            IsProcents = false;
        }

        public LoadingForm(bool isProcents, string basetext = "Загрузка...", ImageLoadType imageloadtype = ImageLoadType.Camera, uint Maximum = 100) : this(basetext, imageloadtype)
        {
            IsProcents = isProcents;
            MaxProcents = Maximum;
            Procents = 0;
        }

        /// <summary>
        /// Высчитывает и устанавливает количество процентов по этапу и максимальному количеству этапов
        /// </summary>
        public void CalculateToProcents(uint Step, uint MaximumStep)
        {
            Procents = (uint)(((float)Step / (float)MaximumStep) * (float)MaxProcents);
            Update();
        }

        private void Loading_Load(object sender, EventArgs e)
        {
        }

        private void LoadingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
