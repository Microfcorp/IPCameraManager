using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPCamera.Network.AutoUpdate
{
    public abstract class CurrentVersion
    {
        /// <summary>
        /// Возвращает текущую версию приложения
        /// </summary>
        public static VersionInfo CurrentVersions = new VersionInfo(30, new DateTime(2023, 02, 12), "Версия 3.0", "");
    }
}
