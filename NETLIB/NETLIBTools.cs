using System.Net;

namespace NETLIB
{
    /// <summary>
    /// General methods and tools
    /// </summary>
    public static class NETLIBTools
    {
        /// <summary>
        /// Returns the the local IP Address
        /// </summary>
        /// <returns>IPAddress related to the local IP</returns>
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
