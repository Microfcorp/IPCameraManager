using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;

namespace IPCamera.Network
{
    public class Downloading
    {
        public static Dictionary<string, string> GetDeviceParams(string ip, string login, string password)
        {
            var resp = GetHTML(DownloadingPaths.ToPath(ip) + DownloadingPaths.DeviceParam, login, password);
            var split = resp.Split('\n');
            var dict = new Dictionary<string, string>();

            foreach (var item in split.Where(x => x.Length > 3))
            {
                //var model="C9F0SgZ3N0P8L0";
                var items = item.Substring(4);
                var name = items.Split('=')[0];
                var value = items.Split('=')[1].Replace("\"", "").Replace(";", "");
                dict.Add(name, value);
            }
            return dict;
        }
        public static Dictionary<string, string> GetImageParams(string ip, string login, string password)
        {
            var resp = GetHTML(DownloadingPaths.ToPath(ip) + DownloadingPaths.ImageParam, login, password);
            var split = resp.Split('\n');
            var dict = new Dictionary<string, string>();

            foreach (var item in split.Where(x => x.Length > 3))
            {
                //var model="C9F0SgZ3N0P8L0";
                var items = item.Substring(4);
                var name = items.Split('=')[0];
                var value = items.Split('=')[1].Replace("\"", "").Replace(";", "").Trim(' ', '\r', '\n');
                dict.Add(name, value);
            }
            return dict;
        }

        public static Dictionary<string, string> GetTimeRecordParams(string ip, string login, string password)
        {
            var resp = GetHTML(DownloadingPaths.ToPath(ip) + DownloadingPaths.TimeRecord, login, password);
            var split = resp.Split('\n');
            var dict = new Dictionary<string, string>();

            foreach (var item in split.Where(x => x.Length > 3))
            {
                //var model="C9F0SgZ3N0P8L0";
                var items = item.Substring(4);
                var name = items.Split('=')[0];
                var value = items.Split('=')[1].Replace("\"", "").Replace(";", "");
                dict.Add(name, value);
            }
            return dict;
        }

        public static LogFile GetLogDevice(string ip, string login, string password)
        {
            var resp = GetHTML(DownloadingPaths.ToPath(ip) + DownloadingPaths.LogFile, login, password);

            return new LogFile(resp);
        }

        public static string GetHTML(string uri)
        {
            StringBuilder sb = new StringBuilder();
            byte[] buf = new byte[8192];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();
            int count = 0;
            do
            {
                count = resStream.Read(buf, 0, buf.Length);
                if (count != 0)
                {
                    sb.Append(Encoding.Default.GetString(buf, 0, count));
                }
            }
            while (count > 0);
            return sb.ToString();
        }
        public static string GetHTML(string uri, string login, string password)
        {
            StringBuilder sb = new StringBuilder();
            byte[] buf = new byte[8192];

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(login + ":" + password));
            request.Headers.Add("Authorization", "Basic " + encoded);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();
            int count = 0;
            do
            {
                count = resStream.Read(buf, 0, buf.Length);
                if (count != 0)
                {
                    sb.Append(Encoding.Default.GetString(buf, 0, count));
                }
            }
            while (count > 0);
            return sb.ToString();
        }

        public static string SendRequest(string uri, string login, string password, SortedList<string, string> data)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = "POST";
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(login + ":" + password));
            request.Headers.Add("Authorization", "Basic " + encoded);

            string command = "";
            foreach (var item in data)
            {
                command += item.Key + "=" + item.Value + "&";
            }

            byte[] bytes = Encoding.ASCII.GetBytes(command);
            request.ContentLength = bytes.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            using (var stream = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                return (stream.ReadToEnd());
            }
        }
    }

    public static class DownloadingPaths
    {
        public const string SD = "sd/";
        public const string Web = "web/admin.html";
        public const string DeviceParam = "web/cgi-bin/hi3510/param.cgi?cmd=getlanguage&cmd=getserverinfo&cmd=getnetattr&cmd=getstreamnum&cmd=getourddnsattr&cmd=get3thddnsattr&cmd=getaudioflag&cmd=getdevtype&cmd=getplanrecattr&cmd=getscheduleex&-ename=plan";
        public const string LogFile = "log/syslog.txt";
        public const string TimeRecord = "web/cgi-bin/hi3510/param.cgi?cmd=getlanguage&cmd=getplanrecattr&cmd=getscheduleex&-ename=plan";
        public const string SendCGI = "web/cgi-bin/hi3510/param.cgi";
        public const string ImageParam = "web/cgi-bin/hi3510/param.cgi?cmd=getlanguage&cmd=getimageattr&cmd=getsetupflag&cmd=getimagemaxsize&cmd=getaudioflag&cmd=getserverinfo&cmd=getvideoattr&cmd=getircutattr&cmd=getinfrared&cmd=getrtmpattr&cmd=getlampattrex";

        public static string ToPath(string IP)
        {
            return "http://" + IP + "/";
        }
    }
}
