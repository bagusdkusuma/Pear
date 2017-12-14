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
            if(clientIP == null)
            {
                return "Unkown IP Address";
            }
            try
            {
                IPAddress myIP = IPAddress.Parse(clientIP);
                IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
                if (GetIPHost != null)
                {
                    List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
                    return compName.First();
                }
            }
            catch (Exception e)
            {
                if (e.Message.Length > 50) return "Unkown hostname";
                else return e.Message;
            }
            
            
            
            return "You Are Acessing from outside local network";
        }
    }
}
