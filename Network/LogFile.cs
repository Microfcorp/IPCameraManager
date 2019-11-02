using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPCamera.Network
{
    public class LogFile
    {
        private string str = "";
        public List<LogNode> Nodes = new List<LogNode>();

        public LogFile(string text)
        {
            str = text.Replace("\r", "");

            foreach (var item in str.Split('\n'))
            {
                if(item != "")
                    Nodes.Add(LogNode.AutoDetect(item));
            }

        }
    }

    public class LogNode
    {
        public DateTime date;
        public string Text;

        public LogNode(string txt)
        {
            if (txt.Length > 10)
            {
                var t1 = StringMaster.NotIntArray(txt.Substring(1).Split(']'), 0);
                date = DateTime.Parse(txt.Substring(1).Split(']')[0].Replace("_", "-"));
                Text = StringMaster.ArrayToString(t1, "]").Substring(1);
            }
        }

        public static LogNode AutoDetect(string txt)
        {
            if (txt.Contains("ircut"))
                return new IRCut(txt);
            //else if (txt.Contains("update time[ntp]"))
            //    return new UpdateTime(txt);
            else
                return new LogNode(txt);
        }
    }
    public class UpdateTime : LogNode
    {
        public UpdateTime(string txt) : base(txt)
        {
            
        }

        public enum TypeUD
        {
            NTP,
            CGI,
        }
        public DateTime newDT;
    }
    public class IRCut : LogNode
    {
        //ircut: display switch(blackwhite -> color).

        public IRCut(string txt) : base(txt)
        {
            var t = Text.Substring(22, 19); //blackwhite -> color
            var firstcolor = t.Split('-')[0];

            if (firstcolor.Contains("color"))
                perechod = Perehod.Day_Night;
            else
                perechod = Perehod.Night_Day;
        }

        public enum Perehod
        {
            Day_Night,
            Night_Day,
            Null,
        }
        public Perehod perechod = Perehod.Null;
    }
}
