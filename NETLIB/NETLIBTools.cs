using System.Net;

namespace NETLIB
{
    public static class NETLIBTools
    {
        public static IPAddress LocalIPAddress()
        {
            IPHostEntry host;
            IPAddress localIP = null;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip;
                }
            }
            return localIP;
        }
    }
}
