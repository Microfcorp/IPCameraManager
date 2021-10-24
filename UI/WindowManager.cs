using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera.UI
{
    public static class WindowManager
    {
        /// <summary>
        /// Возвращает или задает главную форму
        /// </summary>
        public static MainForm MainForms
        {
            get;
            set;
        }
        /// <summary>
        /// Откравает данную форму в оконном менэджере
        /// </summary>
        /// <param name="mainForm">Основная форма с менеджером вкладок</param>
        /// <param name="frm">Форма</param>
        public static void OpenToWindowManager(this Form frm, MainForm mainForm)
        {
            frm.FormClosing += (o, e) => { frm = null; };
            frm.Show();
            var Tabs = TabBox.CreateFromForm(frm);
            mainForm.flowLayoutPanel2.Controls.Add(Tabs);
        }
        /// <summary>
        /// Откравает данную форму в оконном менэджере с указанием имени вкладки
        /// </summary>
        /// <param name="frm">Форма</param>
        /// <param name="mainForm">Основная форма с менеджером вкладок</param>
        /// <param name="Text">Имя вкладки</param>
        public static void OpenToWindowManager(this Form frm, MainForm mainForm, string Text)
        {
            frm.FormClosing += (o, e) => { frm = null; };
            frm.Show();
            var Tabs = TabBox.CreateFromForm(frm, Text);
            mainForm.flowLayoutPanel2.Controls.Add(Tabs);
        }

        /// <summary>
        /// Маштабирует пропорционально элемент управления в родительском элементе управления
        /// </summary>
        /// <param name="panel">Элемент маштабирования</param>
        /// <param name="Parent">Родительский элемент</param>
        /// <param name="SizeSET">Число всех элементов</param>
        public static void SizeOrient(this Control panel, Control Parent, int SizeSET)
        {
            if (true)
            {
                int Padding = Settings.StaticMembers.ImageSettings.Padding;
                if (GroupV.Orientation.OrientationSensor.GetOrientationType(Parent.Size) == GroupV.Orientation.OrientationType.Vertical) panel.Size = new Size((Parent.ClientSize.Width) - Padding, (Parent.ClientSize.Height / SizeSET) - Padding);
                else if (GroupV.Orientation.OrientationSensor.GetOrientationType(Parent.Size) == GroupV.Orientation.OrientationType.Horizontal) panel.Size = new Size((Parent.ClientSize.Width / SizeSET) - Padding, (Parent.ClientSize.Height) - Padding);
            }
        }

        /// <summary>
        /// Маштабирует пропорционально все дочерние элементы управления
        /// </summary>
        /// <param name="Elements">Основная форма</param>
        public static void ResizeF(this Control Elements)
        {
            Elements.SuspendLayout();
            for (int i = 0; i < Elements.Controls.Count; i++)
            {
                Elements.Controls[i].SizeOrient(Elements, Elements.Controls.Count);
            }
            Elements.ResumeLayout();
        }

        /// <summary>
        /// Устанавливает прозрачный фон элементу
        /// </summary>
        /// <param name="Element">Элемент</param>
        public static void SetBackgroundTransparent(this Control Element)
        {
            GraphicsPath gr = new GraphicsPath();
            foreach (Control c in Element.Controls)
            {
                gr.AddRectangle(new Rectangle(c.Location, c.Size));
            }
            Element.Region = new Region(gr);
        }
    }
}
