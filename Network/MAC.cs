using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace IPCamera.Network
{
    public static class MAC
    {
        /// <summary>
        /// Ищет по заданному IP MAC адрес
        /// </summary>
        /// <param name="ip">IP фдрес</param>
        /// <returns></returns>
        public static string ConvertIpToMAC(IPAddress ip)
        {
            byte[] addr = new byte[6];
            int length = addr.Length;

            // TODO: Проверить, что результат - NO_ERROR
            SendARP(ip.GetHashCode(), 0, addr, ref length);

            return BitConverter.ToString(addr, 0, 6);
        }

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestinationIP, int SourceIP, [Out] byte[] pMacAddr, ref int PhyAddrLen);
    }
}
