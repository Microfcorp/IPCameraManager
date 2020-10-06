using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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
        public static Boolean IsOKServer(string Server, int timeout = 800)
        {
            /*WebRequest request = WebRequest.Create(Server);
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(login + ":" + password));
            request.Headers.Add("Authorization", "Basic " + encoded);
            request.Timeout = timeout;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response == null || (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Unauthorized && response.StatusCode != HttpStatusCode.BadRequest))
                {
                    return false;
                }
                response.Close();
            }
            catch
            {
                return false;
            }            
            return true;*/
            try
            {
                Server = Server.Replace("http://", "").Replace("/", "").Split(':').First();
                var ip = IPAddress.Parse(Server);
                var p = new System.Net.NetworkInformation.Ping();
                var rq = p.Send(ip, timeout, new byte[] { 100, 200 });
                return rq.Status == System.Net.NetworkInformation.IPStatus.Success;
            }
            catch
            {
                //Console.WriteLine("PING Error");
                return false;
            }
        }

        public static Boolean IsOKResource(string Server, string login = "", string password = "", int timeout = 800)
        {
            WebRequest request = WebRequest.Create(Server);
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(login + ":" + password));
            request.Headers.Add("Authorization", "Basic " + encoded);
            request.Timeout = timeout;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response == null || (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Unauthorized && response.StatusCode != HttpStatusCode.BadRequest))
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

        public static Boolean IsPort()
        {
            int port = 456; //<--- This is your value
            bool isAvailable = true;

            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    isAvailable = false;
                    break;
                }
            }

            return isAvailable;
        }

        public static bool IsOnline(string Server, int port)
        {
            if (Server != null)
            {
                Server = Server.Replace("http://", "").Replace("/", "").Split(':').First();
                var ip = IPAddress.Parse(Server);

                IPEndPoint endPoint = new IPEndPoint(ip, port);
                Socket checkerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IAsyncResult result = checkerSocket.BeginConnect(endPoint, (ConnectCallback), checkerSocket);
                if (!result.AsyncWaitHandle.WaitOne(100, false)) //пусть подождет ответа в течение 100 милисекунд
                {
                    checkerSocket.Close();
                    return false;
                }
                else
                {
                    checkerSocket.Close();
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        //см. isOnline
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
            }
            catch (Exception error)
            {
            }
        }
    }
}
