using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace IPCamera.FFprobe
{
    class FFprobe
    {
        public const string ffprobe = "ffprobe.exe";
        public static string GetDuration(string file)
        {
            var p2 = new Process(); //запуск конвертера
            p2.StartInfo.FileName = ffprobe;
            p2.StartInfo.Arguments = String.Format("-v error -show_entries stream=duration -select_streams v:0 -of default=noprint_wrappers=1:nokey=1 -print_format csv \"{0}\"", file);
            p2.StartInfo.UseShellExecute = false;
            p2.StartInfo.CreateNoWindow = true;
            p2.StartInfo.RedirectStandardOutput = true;
            p2.EnableRaisingEvents = true;
            string str = "";
            p2.OutputDataReceived += (o, e) =>
            {
                if(e.Data != null) str = e.Data.Split(',').LastOrDefault().Split('.').FirstOrDefault();
            };
            p2.Start();
            p2.BeginOutputReadLine();
            while (str == "") ;
            return str;
        }
    }
}
