using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPCamera.Network
{
    /// <summary>
    /// Адреса потоков
    /// </summary>
    public enum RTSPStream
    {
        /// <summary>
        /// Первичный поток 1920х1080
        /// </summary>
        First_Stream = 11,
        /// <summary>
        /// Вторичный поток (маленький)
        /// </summary>
        Second_Stream = 12,
    }
}
