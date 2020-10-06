using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPCamera.ONVIF.PTZ
{
    public static class PTZStatic
    {
        private static SortedList<string, PTZController> PTZs = new SortedList<string, PTZController>();

        public static void AddPTZ(this Structures set)
        {
            PTZs.Add(set.IP, new PTZController(set), true);
        }

        public static bool ContainsPTZ(this Structures set)
        {
            return PTZs.ContainsKey(set.IP);
            /*foreach (var item in PTZs.Keys)
            {
                if (item == set.IP) return true;
            }
            return false;*/
        }

        public static PTZController GetPTZ(this Structures set)
        {
            return PTZs[set.IP];
            /*for (int i = 0; i < PTZs.Count; i++)
            {
                if (PTZs.Keys[i] == set.IP) return (PTZs.Values[i]);
            }
            return null;*/
        }
    }
}
