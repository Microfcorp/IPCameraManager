using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using IPCamera.Settings;
using IPCamera.Settings.Record;
using IPCamera.DLL;

namespace IPCamera.Record
{
    public class RecordStarting
    {
        public const string ffmpegParams = "-i {0} -vcodec copy -acodec copy -map 0 -f segment -segment_time {1} -segment_format mkv \"{2}\"";

        public static IntPtr MainForm;

        public static RecordParameters StartRecord(uint Selected, int i)
        {            
            var ret = new RecordParameters();
            ret.IsRunning = true;
            var set = Structures.Load()[Selected];
            var rec = set.Records;
            var uri = rec.StreamType == StreamType.Primary ? set.GetRTSPFirst : set.GetRTSPSecond;
            if (rec.RecordFolder == null) { ret.IsRunning = false; return ret; }
            Directory.CreateDirectory(rec.RecordFolder);
            var path = rec.RecordFolder + "\\"+ DateTime.Now.ToString().Replace(":", "-") + " - REC-%d" + ".mkv";

            Process st = new Process();
            st.StartInfo.FileName = Convert.ffmpeg;
            st.StartInfo.Arguments = string.Format(ffmpegParams, uri, rec.RecoedDuration*60, path);
            st.StartInfo.UseShellExecute = false;
            st.StartInfo.CreateNoWindow = true;
            st.EnableRaisingEvents = true;
            st.Exited += (o, e) => { ret.IsRunning = false; ret.Exit(o,e); };
            //st.StartInfo.RedirectStandardError = true;
            //st.StartInfo.RedirectStandardInput = true;
            //st.StartInfo.RedirectStandardOutput = true;            
            st.Start();
            ret.Process = st;   
            ret.ID = i;
            Thread.Sleep(200);
            ret.Handler = st.MainWindowHandle;
            ret.Handler.SetParent(MainForm);
            ret.Handler.ShowWindow((int)CommonFunctions.nCmdShow.SW_HIDE);
            return ret;
        }
    }
}
