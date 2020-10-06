using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IPCamera.Settings.StaticMembers
{
    /// <summary>
    /// Настройки PTZ
    /// </summary>
    public abstract class PTZSettings
    {
        /// <summary>
        /// Тайм-аут поворота
        /// </summary>       
        public static uint Timeout
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\ptz\\timeout.txt";
                if (File.Exists(path))
                {
                    return uint.Parse(File.ReadAllText(path));
                }
                return 800;
            }
            set
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\ptz");
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\ptz\\timeout.txt";
                File.WriteAllText(path, value.ToString());
            }
        }
        /// <summary>
        /// Время одного шага
        /// </summary>       
        public static uint StepTimeout
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\ptz\\steeptimeout.txt";
                if (File.Exists(path))
                {
                    return uint.Parse(File.ReadAllText(path));
                }
                return 200;
            }
            set
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\ptz");
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrofDev\\IPCameraManager\\ptz\\steeptimeout.txt";
                File.WriteAllText(path, value.ToString());
            }
        }
    }
}
