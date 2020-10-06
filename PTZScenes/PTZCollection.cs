using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace IPCamera.PTZScenes
{
    abstract class PTZCollection
    {
        /// <summary>
        /// Загрузить настройки из фалйа
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public static PTZScene[] Load(string path)
        {
            BinaryFormatter bf = new BinaryFormatter();

            if (File.Exists(path))
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    var t2 = (PTZScene[])bf.Deserialize(fs);
                    return t2;
                }
            }
            return new PTZScene[] { PTZScene.Null };
        }
        /// <summary>
        /// Загрузить настройки из фалйа
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public static PTZScene[] Load()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\ptzscenes.dat";
            return Load(path);
        }
        /// <summary>
        /// Сохранить настройки в файл
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="str">Структура настроек</param>
        public static void Save(PTZScene[] str, string path)
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
        public static void Save(PTZScene[] str)
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\");
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\ptzscenes.dat";
            Save(str, path);
        }
        /// <summary>
        /// Удалить файл настроек
        /// </summary>
        public static void DeleteSetting()
        {
            DeleteSetting(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\ptzscenes.dat");
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
            ReplaceSetting(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\ptzscenes.dat", newpath);
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
            ReplaceSetting(newpath, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\ptzscenes.dat");
        }
    }
}
