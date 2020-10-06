using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPCamera.Network.AutoUpdate
{
    public class NetworkUpdate
    {
        public const string URLServer = "http://91.202.27.167/ipcamera/";
        public const string URLConfig = "config.php";
        public const string URLConfirm = "confirm.php";

        /// <summary>
        /// Получить версию с сервера
        /// </summary>
        /// <returns></returns>
        public static VersionInfo GetVersionServer()
        {
            var serverinf = Downloading.GetHTML(URLServer+URLConfig);
            if (serverinf == null) return CurrentVersion.CurrentVersions;
            return new VersionInfo(serverinf);
        }
    }
}
