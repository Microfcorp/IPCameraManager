using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace IPCamera.Monitors
{
    /// <summary>
    /// Монитор камеры
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Monitor
    {
        /// <summary>
        /// Камера
        /// </summary>
        public uint Camera;
        //public Structures Camera;
        /// <summary>
        /// Позиция камеры
        /// </summary>
        public uint Position;
        /// <summary>
        /// Включена ли
        /// </summary>
        public bool Enable;

        /// <summary>
        /// Нулевая конфигурация
        /// </summary>
        public static Monitor Null = new Monitor(0, 0, true);
        //public static Monitor Null = new Monitor(Structures.Null, 0, true);

        /*public Monitor(Structures camera, uint position, bool enable)
        {
            Camera = camera;
            Position = position;
            Enable = enable;
        }*/
        public Monitor(uint camera, uint position, bool enable)
        {
            Camera = camera;
            Position = position;
            Enable = enable;
        }
    }
}
