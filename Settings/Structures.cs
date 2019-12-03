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
    struct Structures
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
        /// Нулевая структура
        /// </summary>
        public static Structures Null = new Structures("localhost","","",80,554,270000, new Rectangle(0,0,10,10));

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
        public Structures(string IP, string Name, string Password, uint HTTPPort, uint RTSPPort, decimal vdm, Rectangle rt)
        {
            this.IP = IP;
            //this.Uri = new Uri(IP);
            this.Name = Name;
            this.Password = Password;
            this.HTTPPort = HTTPPort;
            this.RTSPPort = RTSPPort;
            this.ValueMD = vdm;
            this.ZoneDetect = rt;
        }

        /// <summary>
        /// Загрузить настройки из фалйа
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public static Structures Load(string path)
        {
            BinaryFormatter bf = new BinaryFormatter();

            if (File.Exists(path))
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    var t2 = (Structures)bf.Deserialize(fs);

                    return t2;
                }
            }
            return Structures.Null;
        }
        /// <summary>
        /// Загрузить настройки из фалйа
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public static Structures Load()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofCorp\\IPCameraManager\\setting.dat";
            return Load(path);
        }
        /// <summary>
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
        /// <param name="str">Структура настроек</param>
        public static void Save(Structures str, string path)
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
        public static void Save(Structures str)
        {
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

        public string URLToHTTPPort
        {
            get
            {
                return IP + ":" + HTTPPort;
            }
        }
        public string URLToRTSPPort
        {
            get
            {
                return IP + ":" + RTSPPort;
            }
        }
    }
}
