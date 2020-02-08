using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

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
        public static Structures Null = new Structures("localhost","","",80,554,270000, new Rectangle(0,0,10,10), Network.Network.TypeCamera.HI3510, Record.Records.Default);

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
        public Structures(string IP, string Name, string Password, uint HTTPPort, uint RTSPPort, decimal vdm, Rectangle rt, Network.Network.TypeCamera typeCamera, Record.Records records)
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
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofCorp\\IPCameraManager\\setting.dat";
            return Load(path);
        }
        /*/// <summary>
        /// Сохранить настройки в файл
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        public void Save(string path)
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofCorp\\IPCameraManager\\");
            BinaryFormatter bf = new BinaryFormatter();

            using (FileStream fs = File.Create(path))
                bf.Serialize(fs, this);
        }
        /// <summary>
        /// Сохранить настройки в файл
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        public void Save()
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofCorp\\IPCameraManager\\");
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofCorp\\IPCameraManager\\setting.dat";
            Save(path);
        }
        /// <summary>
        /// Сохранить настройки в файл
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="str">Структура настроек</param>*/
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
        /// Доступна ли камера
        /// </summary>
        public bool IsActive
        {
            get
            {
                return Network.Ping.IsOKServer(GetHTTP);
            }
        }
        /// <summary>
        /// Возвращает Http адрес
        /// </summary>
        public string GetHTTP
        {
            get
            {
                return "http://" + URLToHTTPPort + "/";
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
        /// Получить адрес фото
        /// </summary>
        public string GetPhoto
        {
            get
            {
                return Network.Network.GetPhoto(TypeCamera);
            }
        }
        /// <summary>
        /// Получить поток на фото
        /// </summary>
        public string GetPhotoStream
        {
            get
            {
                return String.Format(GetPhoto, IP, HTTPPort, Name, Password);
            }
        }
    }
}
