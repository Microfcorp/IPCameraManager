using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPCamera.ONVIF
{
    /// <summary>
    /// Токены камеры
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Массив медиа токенов
        /// </summary>
        public string[] Media;
        /// <summary>
        /// Токен для PTZ
        /// </summary>
        public string PTZ;

        internal Token(string[] media, string pTZ)
        {
            Media = media;
            PTZ = pTZ;
        }
    }
    public static class TokensCollection
    {
        private static SortedList<string, Token> PTZs = new SortedList<string, Token>();

        public static void AddToken(this Structures set)
        {
            var pr = set.GetONVIFController().Profiles;
            PTZs.Add(set.IP, new Token(pr.Select(tmp => tmp.token).ToArray(), (pr.Length != 0 ? pr.FirstOrDefault().PTZConfiguration.token : "")), true);
        }

        public static bool ContainsToken(this Structures set)
        {
            //return PTZs.ContainsKey(set.IP);
            foreach (var item in PTZs.Keys)
            {
                if (item == set.IP) return true;
            }
            return false;
        }

        public static Token GetToken(this Structures set)
        {
            for (int i = 0; i < PTZs.Count; i++)
            {
                if (PTZs.Keys[i] == set.IP) return (PTZs.Values[i]);
            }
            return null;
        }
    }
}
