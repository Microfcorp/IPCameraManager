using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPCamera.UI
{
    public partial class TabBox : UserControl
    {
        /// <summary>
        /// Возвращет иконку вкладки
        /// </summary>
        public Image Image
        {
            get
            {
                return pictureBox1.Image;
            }
            private set
            {
                pictureBox1.Image = value;
            }
        }
        /// <summary>
        /// Возвращет текст вкладки
        /// </summary>
        public new string Text
        {
            get
            {
                return button1.Text;
            }
            private set
            {
                button1.Text = value;              
            }
        }

        /// <summary>
        /// Происходит при выборе вкладки
        /// </summary>
        public event EventHandler TabSelect;

        public TabBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Создает вкладку для указанной формы
        /// </summary>
        /// <param name="form">Указанная форма</param>
        /// <returns></returns>
        public static TabBox CreateFromForm(Form form)
        {
            TabBox tb = new TabBox();
            tb.Image = form.Icon.ToBitmap();
            tb.Text = form.Text;
            tb.TabSelect += (o, e) => { form.WindowState = FormWindowState.Normal; form.Activate(); };
            form.FormClosed += (o, e) => { tb.Dispose(); };
            return tb;
        }
        /// <summary>
        /// Создает вкладку для указанной формы с указанием имени вкладки
        /// </summary>
        /// <param name="form">Указанная форма</param>
        /// <param name="Text">Имя вкладки</param>
        /// <returns></returns>
        public static TabBox CreateFromForm(Form form, string Text)
        {
            TabBox tb = CreateFromForm(form);
            tb.Text = Text;
            return tb;
        }

        /// <summary>
        /// Выбрать данную вкладку
        /// </summary>

        public void SelectTab()
        {
            for (int i = 0; i < Parent.Controls.Count; i++)
            {
                if (Parent.Controls[i] is TabBox TB) {
                    TB.UnSelect();
                }
            }
            
            TabSelect?.Invoke(this, new EventArgs());
        }

        public void UnSelect()
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectTab();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            SelectTab();
        }
    }
}
