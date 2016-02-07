using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace NETLIB
{
    public abstract class Publisher
    {
        #region Variáveis

        public event Action ConnectionClosed;

        protected Queue<byte[]> pack_queue;
        protected ManualResetEvent manualEvent;

        protected bool enable;
        protected bool inputEnabled;

        protected Thread publisherThread;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new instance of Publisher
        /// </summary>
        /// <param name="manualEvent">Event to release the Consumer</param>
        /// <param name="pack_queue">Queue to publish packs</param>
        public Publisher()
        {
            this.manualEvent = new ManualResetEvent(false);
            this.pack_queue = new Queue<byte[]>();
            enable = true;
            inputEnabled = false;
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        ~Publisher()
        {
            if (enable != false)
            {
                CloseConnection();   
            }
        }

        #endregion

        #region Atributos

        /// <summary>
        /// Gets the imput state
        /// </summary>
        public bool ImputEnabled
        {
            get { return inputEnabled; }
        }

        /// <summary>
        /// Gets the enable state of the imput/output
        /// </summary>
        public bool Enable
        {
            get { return enable; }
        }

        /// <summary>
        /// Gets the pack queue
        /// </summary>
        public Queue<byte[]> PackQueue
        {
            get { return pack_queue; }
            set { pack_queue = value; }
        }

        /// <summary>
        /// Gets the manual event
        /// </summary>
        public ManualResetEvent ManualEvent
        {
            get { return manualEvent; }
            set { manualEvent = value; }
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Call CloseConnection event
        /// </summary>
        protected void OnConnectionClosedCall()
        {
            if (ConnectionClosed != null)
            {
                ConnectionClosed();
            }
        }

        /// <summary>
        /// Publish packs in pack_queue
        /// </summary>
        public abstract void Publish();

        /// <summary>
        /// Send a pack 
        /// </summary>
        /// <param name="pack">Pack to be sender</param>
        public abstract void SendPack(BasePack pack, IPEndPoint IP = null);

        /// <summary>
        /// Send a pack 
        /// </summary>
        /// <param name="pack">Pack to be sender</param>
        public abstract void SendPack(byte[] pack, IPEndPoint IP = null);

        /// <summary>
        /// Begin the imput 
        /// </summary>
        public void BeginPublish()
        {
            if (enable)
            {
                inputEnabled = true;
                publisherThread = new Thread(Publish);
                publisherThread.Start();
            }
            else
            {
                throw new ConnectionClosedException("Conexão já encerrada!");
            }
        }

        /// <summary>
        /// Stop the Imput and close the Stream
        /// </summary>
        public virtual void CloseConnection()
        {
            inputEnabled = false;
            enable = false;
            manualEvent.Set();
            publisherThread.Abort();
            OnConnectionClosedCall();
        }

        #endregion
    }
}
