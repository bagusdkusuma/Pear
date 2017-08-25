using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Common.Helpers
{
    public class DomainHelper
    {
        public static string GetComputerName(string clientIP)
        {
            IPAddress myIP = IPAddress.Parse(clientIP);
            IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
            List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
            return compName.First();
        }
    }
}
