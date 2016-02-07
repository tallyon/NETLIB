using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace NETLIB.TCP
{
    public class TCPPublisher : Publisher
    {
        #region Variáveis

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

        #region Atributos

        /// <summary>
        /// Gets the imput output stream
        /// </summary>
        public Stream InputOutput
        {
            get { return input; }
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Send a pack 
        /// </summary>
        /// <param name="pack">Pack to be sender</param>
        /// <param name="IP">Ip to send</param>
        public override void SendPack(BasePack pack, IPEndPoint IP = null)
        {
            if (enable)
            {
                try
                {
                    input.Write(pack.Buffer, 0, pack.Buffer.Length);
                }
                catch (IOException e)
                {
                    OnConnectionClosedCall();
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Conexão encerrada no envio. Host destino não responde.");
                }
            }
        }

        /// <summary>
        /// Send a pack 
        /// </summary>
        /// <param name="pack">Pack to be sender</param>
        /// <param name="IP">Ip to send</param>
        public override void SendPack(byte[] pack, IPEndPoint IP = null)
        {
            if (enable)
            {
                try
                {
                    input.Write(pack, 0, pack.Length);
                }
                catch (IOException e)
                {
                    OnConnectionClosedCall();
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Conexão encerrada no envio. Host destino não responde.");
                }
            }
        }

        /// <summary>
        /// Publish packs in pack_queue
        /// </summary>
        public override void Publish()
        {
            byte[] buffer;

            try
            {
                while (imputEnabled)
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
                Console.WriteLine("Conexão encerrada no recebimento. Host destino não responde.");
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

        public static TCPPublisher GetPublisher(string IP, int port)
        {
            TcpClient client = new TcpClient(IP, port);
            return new TCPPublisher(client.GetStream());
        }

        #endregion
    }
}
