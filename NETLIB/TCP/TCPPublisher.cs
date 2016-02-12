using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace NETLIB.TCP
{
    /// <summary>
    /// Publisher using TCP
    /// </summary>
    public class TCPPublisher : Publisher
    {
        #region Variables

        Stream input;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new instance of TCPPublisher
        /// </summary>
        /// <param name="imput">TCPClient's imput stream</param>
        public TCPPublisher(Stream imput)
        {
            this.input = imput;
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Gets the imput output stream
        /// </summary>
        public Stream InputOutput
        {
            get { return input; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send a pack 
        /// </summary>
        /// <param name="pack">Pack to be sender</param>
        ///  <param name="ip">Optional parameter used only by UDP protocol</param>
        public override void SendPack(BasePack pack, IPEndPoint ip = null)
        {
            if (isEnable)
            {
                try
                {
                    input.Write(pack.Buffer, 0, pack.Buffer.Length);
                }
                catch (IOException e)
                {
                    OnConnectionClosedCall();
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Connection closed! Host do not answer.");
                }
            }
        }

        /// <summary>
        /// Send a pack 
        /// </summary>
        /// <param name="pack">Pack to be sender</param>
        ///  <param name="ip">Optional parameter used only by UDP protocol</param>
        public override void SendPack(byte[] pack, IPEndPoint ip = null)
        {
            if (isEnable)
            {
                try
                {
                    input.Write(pack, 0, pack.Length);
                }
                catch (IOException e)
                {
                    OnConnectionClosedCall();
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Connection closed! Host do not answer.");
                }
            }
        }

        /// <summary>
        /// Publish packs in pack_queue
        /// </summary>
        protected override void Publish()
        {
            byte[] buffer;

            try
            {
                while (isInputEnabled)
                {
                    buffer = new byte[BasePack.packSize];
                    if (input.Read(buffer, 0, buffer.Length) == 0)
                        break;
                    pack_queue.Enqueue(buffer);
                    manualEvent.Set();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("Connection closed! Host do not answer.");
                CloseConnection();
            }

        }

        /// <summary>
        /// Stop the Imput and clode the Stream
        /// </summary>
        public override void CloseConnection()
        {
            input.Close();
            base.CloseConnection();
        }

        public static TCPPublisher GetPublisher(string ip, int port)
        {
            TcpClient client = new TcpClient(ip, port);
            return new TCPPublisher(client.GetStream());
        }

        #endregion
    }
}
