using System;
using System.Net;
using System.Net.Sockets;

namespace NETLIB.UDP
{
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
        public override void SendPack(BasePack pack, IPEndPoint IP = null)
        {
            if (enable)
            {
                if (IP == null)
                {
                    throw new ArgumentNullException("IP não pode ser nulo!");
                }

                sender.Send(pack.Buffer, pack.Buffer.Length, IP);
            }
            else
            {
                throw new ConnectionClosedException("Conexão já encerrada!");
            }
        }

        /// <summary>
        /// Send a pack 
        /// </summary>
        /// <param name="pack">Pack to be sender</param>
        public override void SendPack(byte[] pack, IPEndPoint IP = null)
        {
            if (enable)
            {
                if (IP == null)
                {
                    throw new ArgumentNullException("IP não pode ser nulo!");
                }

                sender.Send(pack, pack.Length, IP);
            }
            else
            {
                throw new ConnectionClosedException("Conexão já encerrada!");
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
            if (enable)
            {
                sender.Send(pack.Buffer, pack.Buffer.Length);
            }
            else
            {
                throw new ConnectionClosedException("Conexão já encerrada!");
            }
        }

        /// <summary>
        /// Publish packs in pack_queue
        /// </summary>
        /// <exception cref="Exception"></exception>
        public override void Publish()
        {
            byte[] buffer;

            while (inputEnabled)
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
                    Console.WriteLine("Conexão encerrada no recebimento. Host destino não responde.");
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
