using IPCamera.ODEV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPCamera.ONVIF.StaticMembers
{
    public static class CapabilitiesStatic
    {
        private static SortedList<string, Capabilities> PTZs = new SortedList<string, Capabilities>();

        public static void AddCapabilities(this ONVIFCamera set, Capabilities cbp)
        {
            PTZs.Add(set.set.IP, cbp, true);
        }

        public static bool ContainsCapabilities(this ONVIFCamera set)
        {
            return PTZs.ContainsKey(set.set.IP);
        }

        public static Capabilities GetCapabilities(this ONVIFCamera set)
        {
            return PTZs[set.set.IP];
        }
    }
}
