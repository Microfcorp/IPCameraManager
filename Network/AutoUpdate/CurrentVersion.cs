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
        public static VersionInfo CurrentVersions = new VersionInfo(24, new DateTime(2020, 10, 06), "Версия 2.4", "");
    }
}
