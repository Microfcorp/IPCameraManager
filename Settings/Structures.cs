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
        /// Зона детектора движения
        /// </summary>
        public Rectangle ZoneDetect;

        /// <summary>
        /// Тип чипа камеры
        /// </summary>
        public Network.Network.TypeCamera TypeCamera;

        /// <summary>
        /// Структура записи
        /// </summary>
        public Record.Records Records;

        /// <summary>
        /// Нулевая структура
        /// </summary>
        public static Structures Null = new Structures("localhost","","",80,554,8080,0,1, 270000, new Rectangle(0,0,10,10), Network.Network.TypeCamera.HI3510, Record.Records.Default);

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
        public Structures(string IP, string Name, string Password, uint HTTPPort, uint RTSPPort, uint ONVIFPort, int SelectedFirstProfile, int SelectedSecondProfile, decimal vdm, Rectangle rt, Network.Network.TypeCamera typeCamera, Record.Records records)
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

            //this.MediaTokens = new string[] { "" };
            //this.PTZTokens = "";

            /*if (!IsActive)
            {
                this.MediaTokens = new string[] { "" };
                this.PTZTokens = "";
                return;
            }

            var ou = "http://" + IP + ":" + ONVIFPort;
            var pr = ONVIF.SendResponce.GetProfiles(ou, Name, Password);
            this.MediaTokens = pr.Select(tmp => tmp.token).ToArray();
            this.PTZTokens = pr.First().PTZConfiguration.token;*/
            //_Load();
        }

       /* private void _Load()
        {
            if (!IsActive)
            {
                this.MediaTokens = new string[] { "" };
                this.PTZTokens = "";
                return;
            }

            var pr = ONVIF.SendResponce.GetProfiles(GetONVIF, Name, Password);
            this.MediaTokens = pr.Select(tmp => tmp.token).ToArray();
            this.PTZTokens = pr.First().PTZConfiguration.token;
        }*/

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
                    /*for (int i = 0; i < t2.Length; i++)
                    {
                        t2[i] = new Structures(t2[i].IP, t2[i].Name, t2[i].Password, t2[i].HTTPPort, t2[i].RTSPPort, t2[i].ONVIFPort, t2[i].SelectFirstProfile, t2[i].SelectSecondProfile, t2[i].ValueMD, t2[i].ZoneDetect, t2[i].TypeCamera, t2[i].Records);
                    }*/
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
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofCorp\\IPCameraManager\\setting.dat";
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
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofCorp\\IPCameraManager\\");
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofCorp\\IPCameraManager\\setting.dat";
            Save(str, path);
        }
        /// <summary>
        /// Удалить файл настроек
        /// </summary>
        public static void DeleteSetting()
        {
            DeleteSetting(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofCorp\\IPCameraManager\\setting.dat");
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
        /// Логин камеры
        /// </summary>
        public string Login
        {
            get => Name;
        }

        /// <summary>
        /// Доступна ли камера
        /// </summary>
        public bool IsActive
        {
            get
            {
                return Network.Ping.IsOKServer(GetHTTP, 900);
            }
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
                return String.Format(GetPhoto, IP, HTTPPort, Name, Password);
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
        public ONVIF.PTZ.PTZController GetPTZController
        {
            get => new ONVIF.PTZ.PTZController(this);
        }

        /// <summary>
        /// Создает на основании данной камеры контроллер ONVIF
        /// </summary>
        public ONVIF.ONVIFCamera GetONVIFController
        {
            get => new ONVIF.ONVIFCamera(this);
        }
        public string[] MediaTokens
        {
            get
            {
                var pr = ONVIF.SendResponce.GetProfiles(GetONVIF, Name, Password);
                return pr.Select(tmp => tmp.token).ToArray();
            }
        }
        public string PTZTokens
        {
            get
            {
                var pr = ONVIF.SendResponce.GetProfiles(GetONVIF, Name, Password);
                return pr.First().PTZConfiguration.token; ;
            }            
        }
    }
}
