using System;
using System.Net;
using System.Net.Sockets;

namespace NETLIB.UDP
{
    /// <summary>
    /// Publisher using UDP
    /// </summary>
    public class UDPPublisher : Publisher
    {
        #region Variables

        UdpClient listener;
        UdpClient sender;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new instance of TCPPublisher
        /// </summary>
        /// <param name="input">TCPClient's imput stream</param>
        /// <param name="manualEvent">Event to release the Consumer</param>
        /// <param name="pack_queue">Queue to publish packs</param>
        public UDPPublisher(int port)
        {
            listener = new UdpClient();
            listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listener.Client.Bind(new IPEndPoint(IPAddress.Any, port));

            sender = new UdpClient();
            sender.EnableBroadcast = true;
            sender.ExclusiveAddressUse = false;
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Get the listener
        /// </summary>
        public UdpClient Listener
        {
            get { return listener; }
            set { listener = value; }
        }

        /// <summary>
        /// Gets the sender
        /// </summary>
        public UdpClient Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send a pack 
        /// </summary>
        /// <param name="pack">Pack to be sender</param>
        /// <param name="ip">Address to send</param>
        public override void SendPack(BasePack pack, IPEndPoint ip = null)
        {
            if (isEnable)
            {
                if (ip == null)
                {
                    throw new ArgumentNullException("IP cant be null!");
                }

                sender.Send(pack.Buffer, pack.Buffer.Length, ip);
            }
            else
            {
                throw new ConnectionClosedException("Connection closed!");
            }
        }

        /// <summary>
        /// Send a pack 
        /// </summary>
        /// <param name="pack">Pack to be sender</param>
        /// <param name="ip">Address to send</param>
        public override void SendPack(byte[] pack, IPEndPoint ip = null)
        {
            if (isEnable)
            {
                if (ip == null)
                {
                    throw new ArgumentNullException("IP can't be null");
                }

                sender.Send(pack, pack.Length, ip);
            }
            else
            {
                throw new ConnectionClosedException("Connection closed!");
            }
        }

        /// <summary>
        /// Send a pack 
        /// </summary>
        /// <param name="pack">Pack to be sender</param>
        /// <param name="host_name">Host's name</param>
        /// <param name="port">Host's port</param>
        public void SendPack(BasePack pack, string host_name, int port)
        {
            if (isEnable)
            {
                sender.Send(pack.Buffer, pack.Buffer.Length);
            }
            else
            {
                throw new ConnectionClosedException("Connection closed!");
            }
        }

        /// <summary>
        /// Publish packs in pack_queue
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected override void Publish()
        {
            byte[] buffer;

            while (isInputEnabled)
            {
                IPEndPoint info = null;
                try
                {
                    buffer = listener.Receive(ref info);
                    BasePack.PutString(buffer, info.Address.ToString(), 1);
                    pack_queue.Enqueue(buffer);
                    manualEvent.Set();
                }
                catch (Exception e)
                {
                    OnConnectionClosedCall();
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Connection closed! Host do not answer.");
                }
            }
        }

        /// <summary>
        /// Stop the Input and clode the Stream
        /// </summary>
        public override void CloseConnection()
        {
            base.CloseConnection();
            listener.Close();
            sender.Close();
        }

        #endregion
    }
}
