using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IPCamera.Settings.StaticMembers
{
    /// <summary>
    /// Настройки фотопроигрывателя
    /// </summary>
    public abstract class ImageSettings
    {
        /// <summary>
        /// Количество кадров в секунду
        /// </summary>       
        public static uint FPS
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\imagev\\fps.txt";
                if (File.Exists(path))
                {
                    return uint.Parse(File.ReadAllText(path));
                }
                return 15;
            }
            set
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\imagev");
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\imagev\\fps.txt";
                File.WriteAllText(path, value.ToString());
            }
        }

        /// <summary>
        /// Отступы в режиме мультипросмотра
        /// </summary>       
        public static int Padding
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\imagev\\padding.txt";
                if (File.Exists(path))
                {
                    return int.Parse(File.ReadAllText(path));
                }
                return 10;
            }
            set
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\imagev");
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\imagev\\padding.txt";
                File.WriteAllText(path, value.ToString());
            }
        }
    }
}
