using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace IPCamera.Network
{
    class WebDownload : WebClient
    {
        public string Name
        {
            get;
            set;
        }

        public string UriDownload
        {
            get;
            set;
        }

        public new void DownloadFileAsync(Uri uri, string ToPath)
        {
            base.DownloadFileAsync(uri, ToPath);
            UriDownload = uri.ToString();
        }
        public void DownloadFileAsync(string uri, string ToPath)
        {
            base.DownloadFileAsync(new Uri(uri), ToPath);
            UriDownload = uri.ToString();
        }
    }
}
