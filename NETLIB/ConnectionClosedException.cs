using System.IO;

namespace NETLIB
{
    /// <summary>
    /// Connection has been closed
    /// </summary>
    public class ConnectionClosedException : IOException
    {
        #region Contructor

        public ConnectionClosedException() : base() { }

        public ConnectionClosedException(string message)
            : base(message)
        { }

        #endregion
    }
}