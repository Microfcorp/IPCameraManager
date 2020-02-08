using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace IPCamera.Network
{
    public class Ping
    {
        /// <summary>
        /// Проверка доступности сервера
        /// </summary>
        /// <param name="Server">Адрес сервера</param>
        /// <returns></returns>
        public static Boolean IsOKServer(string Server, string login = "", string password = "")
        {
            WebRequest request = WebRequest.Create(Server);
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(login + ":" + password));
            request.Headers.Add("Authorization", "Basic " + encoded);
            request.Timeout = 500;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response == null || (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Unauthorized))
                {
                    return false;
                }
                response.Close();
            }
            catch
            {
                return false;
            }            
            return true;
        }
    }
}
