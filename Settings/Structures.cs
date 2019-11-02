using System;
using System.Collections.Generic;
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
        /// Нулевая структура
        /// </summary>
        public static Structures Null = new Structures("http://localhost","","",80,554);

        /// <summary>
        /// Обыявление структуры настроек
        /// </summary>
        /// <param name="IP">IP адрес камеры</param>
        /// <param name="Name">Имя пользователя</param>
        /// <param name="Password">Пароль</param>
        /// <param name="HTTPPort">Порт HTTP</param>
        /// <param name="RTSPPort">Порт RTSP</param>
        public Structures(string IP, string Name, string Password, uint HTTPPort, uint RTSPPort)
        {
            this.IP = IP;
            //this.Uri = new Uri(IP);
            this.Name = Name;
            this.Password = Password;
            this.HTTPPort = HTTPPort;
            this.RTSPPort = RTSPPort;
        }

        /// <summary>
        /// Загрузить настройки из фалйа
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public static Structures Load(string path = "Settings.bin")
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
        /// Сохранить настройки в файл
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        public void Save(string path = "Settings.bin")
        {
            BinaryFormatter bf = new BinaryFormatter();

            using (FileStream fs = File.Create(path))
                bf.Serialize(fs, this);
        }
        /// <summary>
        /// Сохранить настройки в файл
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="str">Структура настроек</param>
        public static void Save(Structures str, string path = "Settings.bin")
        {
            BinaryFormatter bf = new BinaryFormatter();

            using (FileStream fs = File.Create(path))
                bf.Serialize(fs, str);
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
