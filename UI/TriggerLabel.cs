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
    /// <summary>
    /// Закладка с триггером
    /// </summary>
    public partial class TriggerLabel : UserControl
    {
        /// <summary>
        /// Клик по закладочке
        /// </summary>
        public event EventHandler ClickBool;
        public TriggerLabel()
        {
            InitializeComponent();
            base.Click += (o,e) => ClickBool?.Invoke(this, new EventArgs());
        }
        public TriggerLabel(string Text) : this()
        {
            this.TextBook = Text;
        }

        /// <summary>
        /// Возвращает или задает текст закладки
        /// </summary>
        public string TextBook
        {
            get
            {
                return label1.Text;
            }
            set
            {
                label1.Text = value;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            ClickBool?.Invoke(this, new EventArgs());
        }

        private void TriggerLabel_Load(object sender, EventArgs e)
        {

        }
    }
}
