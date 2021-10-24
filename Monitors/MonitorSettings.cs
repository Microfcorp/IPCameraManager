using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace IPCamera.Monitors
{
    /// <summary>
    /// Настройки мониторов
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MonitorSettings
    {
        /// <summary>
        /// Список мониторов
        /// </summary>
        public Monitor[] Monitors;

        /// <summary>
        /// Список монитор, упорядоченные по их позиции
        /// </summary>
        public Monitor[] GetMonitorsSortPosition
        {
            get
            {
                return Monitors.Where(tmp => tmp.Enable).OrderBy(tmp => tmp.Position).ToArray();
            }
        }
        /// <summary>
        /// Название коллекции мониторов
        /// </summary>
        public string Name;

        /// <summary>
        /// Нулевая конфигурация
        /// </summary>
        public static MonitorSettings Null = new MonitorSettings(new Monitor[] { Monitor.Null }, "Default");

        public MonitorSettings(Monitor[] monitors, string name)
        {
            Monitors = monitors;
            Name = name;
        }
    }
    
    public class MonitorsController
    {
        /// <summary>
        /// Загрузить настройки из фалйа
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public static MonitorSettings[] Load(string path)
        {
            BinaryFormatter bf = new BinaryFormatter();

            if (File.Exists(path))
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    var t2 = (MonitorSettings[])bf.Deserialize(fs);
                    return t2;
                }
            }
            return new MonitorSettings[] { MonitorSettings.Null };
        }
        /// <summary>
        /// Загрузить настройки из фалйа
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public static MonitorSettings[] Load()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\monitors.dat";
            return Load(path);
        }
        /// <summary>
        /// Сохранить настройки в файл
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="str">Структура настроек</param>
        public static void Save(MonitorSettings[] str, string path)
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
        public static void Save(MonitorSettings[] str)
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\");
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\monitors.dat";
            Save(str, path);
        }
        /// <summary>
        /// Удалить файл настроек
        /// </summary>
        public static void DeleteSetting()
        {
            DeleteSetting(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\monitors.dat");
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
            ReplaceSetting(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\monitors.dat", newpath);
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
            ReplaceSetting(newpath, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\monitors.dat");
        }
    }
}
