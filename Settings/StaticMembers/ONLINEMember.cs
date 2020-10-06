using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPCamera.Settings.StaticMembers
{
    public static class ONLINEMembers
    {
        private static SortedList<string, bool> onlines = new SortedList<string, bool>();

        public static void AddONLINE(this Structures set, bool cbp)
        {
            onlines.Add(set.IP, cbp, true);
        }

        public static bool ContainsONLINE(this Structures set)
        {
            foreach (var item in onlines.Keys)
            {
                if (item == set.IP) return true;
            }
            return false;
            /*var t = onlines.Keys.Contains(set.IP);
            System.Threading.Thread.Sleep(10);
            return t;*/
        }

        public static bool GetONLINE(this Structures set)
        {
            for (int i = 0; i < onlines.Count; i++)
            {
                if (onlines.Keys[i] == set.IP) return (onlines.Values[i]);
            }
            return false;
        }
    }
}
