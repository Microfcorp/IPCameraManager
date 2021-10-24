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
        public static VersionInfo CurrentVersions = new VersionInfo(27, new DateTime(2021, 10, 24), "Версия 2.7", "");
    }
}
