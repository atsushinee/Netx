using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Netx.Util
{
    public class CommonUtil
    {
        public async static Task<List<string>> GetIpv4AddrListAsync(bool showLocal = true)
        {
            var ipv4List = new List<string>();
            if (showLocal)
            {
                ipv4List.Add("0.0.0.0");
                ipv4List.Add("127.0.0.1");
            }

            var localHostEntry = await Dns.GetHostEntryAsync(Dns.GetHostName());
            var addrList = localHostEntry.AddressList;
            foreach (var addr in addrList)
            {
                if (!addr.IsIPv6LinkLocal)
                {
                    ipv4List.Add(addr.ToString());
                }
            }
            return ipv4List;
        }

        public static void Main(string[] args)
        {
            var a = GetIpv4AddrListAsync();
            Console.ReadLine();
        }

        const string FULL_DATE = "yyyy-MM-dd HH:mm:ss";
        public static string GetNowDateTime(string pattern = FULL_DATE)
        {
            return DateTime.Now.ToString(pattern);
        }
    }
}
