using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPCamera.ONVIF
{
    public static class ONVIFStatic
    {
        private static SortedList<string, ONVIFCamera> PTZs = new SortedList<string, ONVIFCamera>();

        public static void AddONVIF(this Structures set)
        {
            PTZs.Add(set.IP, new ONVIFCamera(set), true);
        }

        public static bool ContainsONVIF(this Structures set)
        {
            return PTZs.ContainsKey(set.IP);
        }

        public static ONVIFCamera GetONVIF(this Structures set)
        {
            return PTZs[set.IP];
        }
    }
}
