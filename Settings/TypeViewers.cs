using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCamera.Settings
{
    /// <summary>
    /// Типы воспроизведения видео
    /// </summary>
    public enum TypeViewers : byte
    {
        /// <summary>
        /// Используя ffplay
        /// </summary>
        FFPLAY,
        /// <summary>
        /// Используя mplayer
        /// </summary>
        MPlayer,
        /// <summary>
        /// Используя фото просмотр
        /// </summary>
        ImageV,
        /// <summary>
        /// Используя OpenCV
        /// </summary>
        OpenCV,
    }
}
