using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel.Discovery;
using System.Text;
using System.Xml;
using IPCamera.ONVIF.PTZ;
using IPCamera.ONVIF;
using IPCamera.Settings.StaticMembers;

namespace IPCamera.Settings
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Structures
    {
        /// <summary>
        /// Адрес камеры
        /// </summary>
        public string IP;
        /// <summary>
        /// Адрес камеры
        /// </summary>
        //public Uri Uri;
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string MAC;

        /// <summary>
        /// Имя камеры
        /// </summary>
        public string NameCamera;
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name;
        /// <summary>
        /// Пароль
        /// </summary>
        public string Password;

        /// <summary>
        /// Порт HTTP
        /// </summary>
        public uint HTTPPort;
        /// <summary>
        /// Порт RTSP
        /// </summary>
        public uint RTSPPort;
        /// <summary>
        /// Порт ONVIF
        /// </summary>
        public uint ONVIFPort;

        /// <summary>
        /// Выбранный первичный профиль
        /// </summary>
        public int SelectFirstProfile;

        /// <summary>
        /// Выбранный вторичный профиль
        /// </summary>
        public int SelectSecondProfile;

        /// <summary>
        /// Порог детектора движения
        /// </summary>
        public decimal ValueMD;

        /// <summary>
        /// Имеется ли в камере PTZ
        /// </summary>
        public bool PTZ;

        /// <summary>
        /// Зона детектора движения
        /// </summary>
        public Rectangle ZoneDetect;

        /// <summary>
        /// Тип чипа камеры
        /// </summary>
        public Network.Network.TypeCamera TypeCamera;

        /// <summary>
        /// Проигрыватель для одиночного просмотра
        /// </summary>
        public TypeViewers SinglePlay;
        /// <summary>
        /// Проигрыватель для группового просмотра
        /// </summary>
        public TypeViewers GroupPlay;
        /// <summary>
        /// Проигрыватель для мониторов
        /// </summary>
        public TypeViewers MonitorPlay;

        /// <summary>
        /// Структура записи
        /// </summary>
        public Record.Records Records;

        /// <summary>
        /// Нулевая структура
        /// </summary>
        public static Structures Null = new Structures("localhost","","",80,554,8080,0,1,false, 270000, new Rectangle(0,0,10,10), Network.Network.TypeCamera.Other, Record.Records.Default, "Defalt", "", TypeViewers.FFPLAY, TypeViewers.ImageV, TypeViewers.ImageV);

        /// <summary>
        /// Обыявление структуры настроек
        /// </summary>
        /// <param name="IP">IP адрес камеры</param>
        /// <param name="Name">Имя пользователя</param>
        /// <param name="Password">Пароль</param>
        /// <param name="HTTPPort">Порт HTTP</param>
        /// <param name="RTSPPort">Порт RTSP</param>
        /// <param name="vdm">Порог детектора движения</param>
        /// <param name="rt">Зона детектора движения</param>
        /// <param name="typeCamera">Тип чипа камеры</param>
        /// <param name="records">Структура для записи</param>
        public Structures(string IP, string Name, string Password, uint HTTPPort, uint RTSPPort, uint ONVIFPort, int SelectedFirstProfile, int SelectedSecondProfile, bool ptz, decimal vdm, Rectangle rt, Network.Network.TypeCamera typeCamera, Record.Records records, string NameCamera, string MAC, TypeViewers v1, TypeViewers v2, TypeViewers v3)
        {
            this.IP = IP;
            //this.Uri = new Uri(IP);
            this.Name = Name;
            this.Password = Password;
            this.HTTPPort = HTTPPort;
            this.RTSPPort = RTSPPort;
            this.ValueMD = vdm;
            this.ZoneDetect = rt;
            this.TypeCamera = typeCamera;
            this.Records = records;
            this.ONVIFPort = ONVIFPort;
            this.SelectFirstProfile = SelectedFirstProfile;
            this.SelectSecondProfile = SelectedSecondProfile;
            this.PTZ = ptz;
            this.NameCamera = NameCamera;
            this.MAC = MAC;
            this.SinglePlay = v1;
            this.GroupPlay = v2;
            this.MonitorPlay = v3;
        }

        /// <summary>
        /// Загрузить настройки из фалйа
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public static Structures[] Load(string path)
        {
            BinaryFormatter bf = new BinaryFormatter();

            if (File.Exists(path))
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    var t2 = (Structures[])bf.Deserialize(fs);
                    return t2;
                }
            }
            return new Structures[] { Structures.Null };
        }
        /// <summary>
        /// Загрузить настройки из фалйа
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public static Structures[] Load()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\setting.dat";
            return Load(path);
        }
        /// <summary>
        /// Сохранить настройки в файл
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="str">Структура настроек</param>
        public static void Save(Structures[] str, string path)
        {
            BinaryFormatter bf = new BinaryFormatter();
            
            using (FileStream fs = File.Create(path))
                bf.Serialize(fs, str);
        }
        /// <summary>
        /// Сохранить настройки в файл
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="str">Структура настроек</param>
        public static void Save(Structures[] str)
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\");
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\setting.dat";
            Save(str, path);
        }
        /// <summary>
        /// Удалить файл настроек
        /// </summary>
        public static void DeleteSetting()
        {
            DeleteSetting(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\setting.dat");
        }
        /// <summary>
        /// Удалить файл настроек
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        public static void DeleteSetting(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        /// <summary>
        /// Импортирует файл настроек
        /// </summary>
        public static void ReplaceSetting(string newpath)
        {
            ReplaceSetting(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\setting.dat", newpath);
        }
        /// <summary>
        /// Импортирует файл настроек
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        public static void ReplaceSetting(string path, string newpath)
        {
            if (File.Exists(newpath))
                File.Copy(newpath, path, true);
        }
        /// <summary>
        /// Копирует файл настроек
        /// </summary>
        public static void CopySetting(string newpath)
        {
            ReplaceSetting(newpath, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\setting.dat");
        }

        /// <summary>
        /// Логин камеры
        /// </summary>
        public string Login
        {
            get => Name;
        }

        /// <summary>
        /// Получает из сети MAC адрес
        /// </summary>
        public string GetMAC
        {
            get
            {
                return Network.MAC.ConvertIpToMAC(System.Net.IPAddress.Parse(IP));
            }
        }

        /// <summary>
        /// Доступна ли камера
        /// </summary>
        public bool IsActive
        {
            get
            {
                return Network.Ping.IsOKServer(GetHTTP, 500);
                //return Network.Ping.IsOnline(IP, (int)HTTPPort);
                /*if (this.ContainsONLINE())
                    return this.GetONLINE();
                else this.AddONLINE(Network.Ping.IsOKServer(GetHTTP, 500));
                return this.GetONLINE();*/
            }
        }

        /// <summary>
        /// Доступна ли камера
        /// </summary>
        public bool IsActivePro(int timeout = 500)
        {
            return Network.Ping.IsOKServer(GetHTTP, timeout);
        }
        /// <summary>
        /// Возвращает базу для HTTP запроса
        /// </summary>
        public string GetHTTP
        {
            get
            {
                return "http://" + URLToHTTPPort + "/";
            }
        }
        /// <summary>
        /// Возвращает базу для ONVIF запросов
        /// </summary>
        public string GetONVIF
        {
            get
            {
                return "http://" + URLToONVIFPort + "/";
            }
        }
        /// <summary>
        /// Адрес и RTSP порт
        /// </summary>
        public string URLToRTSPPort
        {
            get
            {
                return IP + ":" + RTSPPort;
            }
        }
        /// <summary>
        /// Адрес и HTTP порт
        /// </summary>
        public string URLToHTTPPort
        {
            get
            {
                return IP + ":" + HTTPPort;
            }
        }
        /// <summary>
        /// Адрес и ONVIF порт
        /// </summary>
        public string URLToONVIFPort
        {
            get
            {
                return IP + ":" + ONVIFPort;
            }
        }
        /// <summary>
        /// Получить RTSP масску для камеры
        /// </summary>
        public string GetRTSP
        {
            get
            {
                return Network.Network.GetRTSP(TypeCamera);
            }
        }
        /// <summary>
        /// Получить RTSP адрес первичного потока для камеры
        /// </summary>
        public string GetRTSPFirst
        {
            get
            {
                string stream = TypeCamera == Network.Network.TypeCamera.HI3510 ? ((int)Network.RTSPStream.First_Stream).ToString() : ((int)Network.RTSPStream.First_SDP).ToString(); //11 and 12
                return String.Format(GetRTSP, Name, Password, IP, RTSPPort, stream);
            }
        }
        /// <summary>
        /// Получить RTSP адрес вторичного потока для камеры
        /// </summary>
        public string GetRTSPSecond
        {
            get
            {
                string stream = TypeCamera == Network.Network.TypeCamera.HI3510 ? ((int)Network.RTSPStream.Second_Stream).ToString() : ((int)Network.RTSPStream.Second_SDP).ToString(); //11 and 12
                return String.Format(GetRTSP, Name, Password, IP, RTSPPort, stream);
            }
        }
        /// <summary>
        /// Получить RTSP адрес первичного потока для камеры через ONVIF
        /// </summary>
        public string GetRTSPFirstONVIF
        {
            get
            {
                return ONVIF.SendResponce.GetStreamURL(this, SelectFirstProfile);
            }
        }
        /// <summary>
        /// Получить RTSP адрес вторичного потока для камеры через ONVIF
        /// </summary>
        public string GetRTSPSecondONVIF
        {
            get
            {
                return ONVIF.SendResponce.GetStreamURL(this, SelectSecondProfile);
            }
        }
        /// <summary>
        /// Получить адрес фото (часть пути без сервера)
        /// </summary>
        public string GetPhoto
        {
            get
            {
                return Network.Network.GetPhoto(TypeCamera);
            }
        }
        /// <summary>
        /// Получить поток на фото (полный http адрес)
        /// </summary>
        public string GetPhotoStream
        {
            get
            {
                if (TypeCamera == Network.Network.TypeCamera.HI3510)
                    return String.Format(GetPhoto, IP, HTTPPort, Name, Password);
                else
                    return GetPhotoStreamSecondONVIF;
            }
        }
        /// <summary>
        /// Получить поток на фото (полный http адрес) через ONVIF
        /// </summary>
        public string GetPhotoStreamFirstONVIF
        {
            get
            {
                return ONVIF.SendResponce.GetSnapsotURL(this, SelectFirstProfile);
            }
        }
        /// <summary>
        /// Получить поток на фото (полный http адрес) через ONVIF
        /// </summary>
        public string GetPhotoStreamSecondONVIF
        {
            get
            {
                return ONVIF.SendResponce.GetSnapsotURL(this, SelectSecondProfile);
            }
        }

        /// <summary>
        /// Создает на основании данной камеры контроллер PTZ
        /// </summary>
        public PTZController GetPTZController()
        {
            if (this.ContainsPTZ())
                return this.GetPTZ();
            else this.AddPTZ();
            return this.GetPTZ();
        }

        /// <summary>
        /// Создает на основании данной камеры контроллер ONVIF
        /// </summary>
        public ONVIFCamera GetONVIFController()
        {
            if (this.ContainsONVIF())
                return ONVIFStatic.GetONVIF(this);
            else this.AddONVIF();
            return ONVIFStatic.GetONVIF(this);
        }

        /// <summary>
        /// Массив доступных токенов для медии
        /// </summary>
        public string[] GetMediaTokens()
        {
            if (this.ContainsToken())
                return this.GetToken().Media;
            else this.AddToken();
            return this.GetToken().Media;
        }

        /// <summary>
        /// Токен для PTZ управления
        /// </summary>
        public string GetPTZTokens()
        {
            /*if (this.ContainsToken())
                return this.GetToken().PTZ;
            else this.AddToken();
            return this.GetToken().PTZ;*/
            if (this.ContainsToken())
                return this.GetToken().Media[0];
            else this.AddToken();
            return this.GetToken().Media[0];
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}
