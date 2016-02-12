using System.IO;

namespace NETLIB
{
    /// <summary>
    /// Connection still runnig
    /// </summary>
    public class ConnectionRunnigException : IOException
    {
        #region Contructor

        public ConnectionRunnigException() : base() { }

        public ConnectionRunnigException(string message)
            : base(message)
        { }

        #endregion
    }
}