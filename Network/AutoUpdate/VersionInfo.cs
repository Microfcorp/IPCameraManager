using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPCamera.Network.AutoUpdate
{
    /// <summary>
    /// Информация о версии
    /// </summary>
    public class VersionInfo
    {
        /// <summary>
        /// Внутренний номер релиза
        /// </summary>
        public byte Version;
        /// <summary>
        /// Дата релиза
        /// </summary>
        public DateTime Release;
        /// <summary>
        /// Описание релиза
        /// </summary>
        public string Note;
        /// <summary>
        /// Ссылка для загрузки
        /// </summary>
        public string URLDownload;

        public VersionInfo(byte version, DateTime release, string note, string uRLDownload)
        {
            Version = version;
            Release = release;
            Note = note;
            URLDownload = uRLDownload;
        }
        public VersionInfo(string text)
        {
            var t = text.Split(';');

            Version = byte.Parse(t[0]);
            Release = DateTime.Parse(t[1]);
            Note = t[2];
            URLDownload = t[3];
        }

        public static bool operator ==(VersionInfo left, VersionInfo right)
        {
            return (left.Version == right.Version);
        }

        public static bool operator !=(VersionInfo left, VersionInfo right)
        {
            return (left.Version != right.Version);
        }

        public static bool operator >(VersionInfo left, VersionInfo right)
        {
            return (left.Version > right.Version);
        }

        public static bool operator <(VersionInfo left, VersionInfo right)
        {
            return (left.Version < right.Version);
        }

        public static bool operator >=(VersionInfo left, VersionInfo right)
        {
            return (left.Version >= right.Version);
        }

        public static bool operator <=(VersionInfo left, VersionInfo right)
        {
            return (left.Version <= right.Version);
        }

        public override bool Equals(object obj)
        {
            return obj is VersionInfo info &&
                   Version == info.Version &&
                   Release == info.Release &&
                   URLDownload == info.URLDownload;
        }

        public override int GetHashCode()
        {
            var hashCode = -2028525349;
            hashCode = hashCode * -1521134295 + Version.GetHashCode();
            hashCode = hashCode * -1521134295 + Release.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(URLDownload);
            return hashCode;
        }

        public override string ToString()
        {
            var t = Version.ToString();
            var rt = "";
            foreach (var item in t)
                rt += item + ".";
            rt = rt.Substring(0, rt.Length - 1);
            return rt;
        }
    }
}
