using IPCamera.ServiceReference1;
using IPCamera.Settings;
using IPCamera.ONVIF.StaticMembers;
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
        internal Structures set;

        public ONVIFCamera(Structures set)
        {
            this.set = set;
            isSupported = set.IsActive && set.ONVIFPort != 0;
        }

        private readonly bool isSupported;

        /// <summary>
        /// Поддерживает ли камера ONVIF соеденение
        /// </summary>
        public bool IsSupported => isSupported;

        /// <summary>
        /// Получает контроллер PTZ для данной камеры
        /// </summary>
        public PTZ.PTZController PTZController
        {
            get => set.GetPTZController();
        }

        /// <summary>
        /// Ссылка на поток изображения
        /// </summary>
        public string ImageStreamURL
        {
            get => SendResponce.GetSnapsotURL(set, 0);
        }

        /// <summary>
        /// Информация об устройстве
        /// </summary>
        public DeviceONVIF.DeviceInformation DeviceInformation
        {
            get => DeviceONVIF.GetDeviceInformation(set);
        }

        /// <summary>
        /// Дата и время на камере
        /// </summary>
        public CameraDateTime CameraDateTime
        {
            get { var e = DeviceONVIF.GetCameraDate(set); return new CameraDateTime(e.UTCDateTime, e.TimeZone.TZ); }
        }

        /// <summary>
        /// Устанавливает время и дату на камере
        /// </summary>
        /// <param name="TT">NTP или вручную</param>
        /// <param name="TZ">Часовой пояс/param>
        /// <param name="DT">Дата и время</param>

        public void SetCameraTime(ODEV.SetDateTimeType TT, string TZ, DateTime DT)
        {
            DeviceONVIF.SetCameraDate(set, TT, TZ, DT);
        }

        /// <summary>
        /// URL для реплеев видео
        /// </summary>
        public string ReplayURL
        {
            get => SendResponce.GetReplayUri(set);
        }

        /// <summary>
        /// Обнаружено ли движение (Странно работает)
        /// </summary>
        public bool IsMotionDetect
        {
            get => MotionDetect.IsDetect(SendResponce.GetEvents(set));
        }

        /// <summary>
        /// Список всех протоколов, поддерживаемых камерой
        /// </summary>
        public ODEV.NetworkProtocol[] NetworkProtocols
        {
            get => DeviceONVIF.GetNetworkProtocols(set);
        }

        /// <summary>
        /// Имя хоста камеры
        /// </summary>
        public string Hostname
        {
            get => DeviceONVIF.GetHostname(set);
            set => DeviceONVIF.SetHostname(set, value);
        }

        /// <summary>
        /// Все возможности камеры
        /// </summary>
        public ODEV.Capabilities Capabilities
        {
            get
            {
                if (this.ContainsCapabilities())
                    return this.GetCapabilities();
                else this.AddCapabilities(DeviceONVIF.GetCapabilities(set));
                return this.GetCapabilities();
            }
            //get => DeviceONVIF.GetCapabilities(set);
        }

        /// <summary>
        /// Массив всех пользователей камеры
        /// </summary>
        public ODEV.User[] Users
        {
            get => DeviceONVIF.GetUsers(set);
        }

        /// <summary>
        /// Версия ONVIF протокола
        /// </summary>
        public ONVIFVersion Version
        {
            get => new ONVIFVersion(DeviceONVIF.GetOnvifVersion(set));
        }

        /// <summary>
        /// Получить настройки изображения
        /// </summary>
        public OIMG.ImagingSettings20 GetImagingSettings(uint tokenid)
        {
            return ImagingResponce.GetImageSetting(set, tokenid) ?? null;
        }

        /// <summary>
        /// Перезагрузает камеру
        /// </summary>
        /// <param name="ok">Подтверждение операции</param>
        /// <returns></returns>
        public string Reboot(bool ok)
        {
           if(ok) return DeviceONVIF.Reboot(set);
            return "Not Valid";
        }


        /// <summary>
        /// Тест
        /// </summary>
        public string Test
        {
            get => DeviceONVIF.Test(set);
        }

        /// <summary>
        /// Массив всех доступных профилей
        /// </summary>
        public Profile[] Profiles => SendResponce.GetProfiles(set);

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
