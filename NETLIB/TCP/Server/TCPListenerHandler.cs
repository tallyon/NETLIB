using System;
using System.Net.Sockets;
using System.Threading;

namespace NETLIB.TCP.Server
{
    public class TCPListenerHandler
    {
        #region Variables

        public event Action<Publisher> ReceivedConnection;

        TcpListener listen;

        Thread listenThread;

        bool enabled;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize new TCPListenerHandler
        /// </summary>
        public TCPListenerHandler()
        {
            enabled = false;
            listenThread = new Thread(ReceiveConnection);
        }

        /// <summary>
        /// Close all connections
        /// </summary>
        ~TCPListenerHandler()
        {
            enabled = false;
            listenThread.Abort();
            if (listen != null)
            {
                listen.Stop();   
            }
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Gets enabled
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Call received connection
        /// </summary>
        /// <param name="conexao">New consumer</param>
        private void OnReceivedConnectionCall(Publisher conexao)
        {
            if (ReceivedConnection != null)
            {
                ReceivedConnection(conexao);
            }
        }

        /// <summary>
        /// Begin listen 
        /// </summary>
        /// <param name="port">Port to listen</param>
        public void BeginListen(int port)
        {
            if (!enabled)
            {
                listen = new TcpListener(port);

                listenThread.Start();
                enabled = true;
            }
            else
            {
                throw new ConnectionRunnigException("Escutas já iniciadas.");
            }
        }

        /// <summary>
        /// Stop listen
        /// </summary>
        public void StopListen()
        {
            if (enabled)
            {
                enabled = false;
                listen.Stop();
            }
        }

        /// <summary>
        /// Receive connections
        /// </summary>
        private void ReceiveConnection()
        {
            listen.Start();

            try
            {
                while (enabled)
                {
                    OnReceivedConnectionCall(new TCPPublisher(listen.AcceptTcpClient().GetStream()));
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message + " - Stream de entrada nula.");
            }
        }

        #endregion
    }
}
