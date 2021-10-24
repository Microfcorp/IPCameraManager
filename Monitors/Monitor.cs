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
        uint _Camera;
        public uint Camera
        {
            get
            {
                return Structures.Load().Length < _Camera ? 0 : _Camera;
            }
            set
            {
                _Camera = value; 
            }
        }
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
        /// IP адрес камеры для идентификации
        /// </summary>
        public string IP;
        /// <summary>
        /// Загружает данную камеру из настроек
        /// </summary>
        public Structures GetCamera
        {
            get
            {
                var t = this;
                return Structures.Load().Where(tmp => tmp.IP == t.IP).FirstOrDefault();
            }
        }
        /// <summary>
        /// Нулевая конфигурация
        /// </summary>
        public static Monitor Null = new Monitor(0, 0, true, "");
        //public static Monitor Null = new Monitor(Structures.Null, 0, true);

        /*public Monitor(Structures camera, uint position, bool enable)
        {
            Camera = camera;
            Position = position;
            Enable = enable;
        }*/
        public Monitor(uint camera, uint position, bool enable, string MAC)
        {
            _Camera = camera;
            Position = position;
            Enable = enable;
            this.IP = MAC;
        }
    }
}
