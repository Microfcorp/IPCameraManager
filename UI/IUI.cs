using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPCamera.UI
{
    public interface IUI
    {
        void UpdateForm();
    }

    /// <summary>
    /// Стиль изображения загрузки
    /// </summary>
    public enum ImageLoadType : byte
    {
        /// <summary>
        /// Колесо
        /// </summary>
        Wait,
        /// <summary>
        /// Камера
        /// </summary>
        Camera,
    }
}
