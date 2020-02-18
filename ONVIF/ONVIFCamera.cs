using IPCamera.ServiceReference1;
using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace IPCamera.ONVIF
{
    /// <summary>
    /// ONVIF контроллер для камеры
    /// </summary>
    public class ONVIFCamera
    {
        private Structures set;

        public ONVIFCamera(Structures set)
        {
            this.set = set;
            IsSupported = set.IsActive;
        }
        /// <summary>
        /// Поддерживает ли камера ONVIF соеденение
        /// </summary>
        public bool IsSupported
        {
            get;
        }
        /// <summary>
        /// Получает контроллер PTZ для данной камеры
        /// </summary>
        public PTZ.PTZController PTZController
        {
            get => new PTZ.PTZController(set);
        }

        /// <summary>
        /// Ссылка на поток изображения
        /// </summary>
        public string ImageStreamURL
        {
            get => SendResponce.GetSnapsotURL(set, 0);
        }

        /// <summary>
        /// Массив всех доступных профилей
        /// </summary>
        public Profile[] Profiles
        {
            get => SendResponce.GetProfiles(set);
        }

        /// <summary>
        /// RTSP поток для заданного профиля
        /// </summary>
        /// <param name="profile">Номер профиля</param>
        /// <returns></returns>
        public string GetRTSPStream(int profile = 0)
        {
            return SendResponce.GetStreamURL(set, profile);
        }

        /// <summary>
        /// Разрешение изображения для заданного профиля
        /// </summary>
        /// <param name="profile">Номер профиля</param>
        /// <returns></returns>
        public Size GetVideoResolution(int profile = 0)
        {
            return SendResponce.GetVideoResolution(set, profile);
        }
    }
}
