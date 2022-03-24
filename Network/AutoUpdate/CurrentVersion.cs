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
        public static VersionInfo CurrentVersions = new VersionInfo(29, new DateTime(2022, 03, 24), "Версия 2.9", "Добавлена карта");
    }
}
