using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace NETLIB
{
    /// <summary>
    /// Publish a pack received
    /// </summary>
    public abstract class Publisher : IDisposable
    {
        #region Variables

        public event Action ConnectionClosed;

        protected Queue<byte[]> pack_queue;
        protected ManualResetEvent manualEvent;

        protected bool isEnable;
        protected bool isInputEnabled;

        protected Thread publisherThread;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new instance of Publisher
        /// </summary>
        public Publisher()
        {
            manualEvent = new ManualResetEvent(false);
            pack_queue = new Queue<byte[]>();
            isEnable = true;
            isInputEnabled = false;
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        ~Publisher()
        {
            if (isEnable != false)
            {
                CloseConnection();   
            }
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        public void Dispose()
        {
            if (isEnable != false)
            {
                CloseConnection();
            }
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Gets if the input is enabled
        /// </summary>
        public bool IsInputEnabled
        {
            get { return isInputEnabled; }
        }

        /// <summary>
        /// Gets the enable state of the input/output
        /// </summary>
        public bool IsEnable
        {
            get { return isEnable; }
        }

        /// <summary>
        /// Gets the pack queue
        /// </summary>
        public Queue<byte[]> PackQueue
        {
            get { return pack_queue; }
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

        #region Methods

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
        protected abstract void Publish();

        /// <summary>
        /// Send a pack 
        /// </summary>
        /// <param name="pack">Pack to be sender</param>
        ///  <param name="ip">Optional parameter used only by UDP protocol</param>
        public abstract void SendPack(BasePack pack, IPEndPoint ip = null);

        /// <summary>
        /// Send a pack 
        /// </summary>
        /// <param name="pack">Pack to be sender</param>
        /// <param name="ip">Optional parameter used only by UDP protocol</param>
        public abstract void SendPack(byte[] pack, IPEndPoint ip = null);

        /// <summary>
        /// Begin the imput 
        /// </summary>
        public void Start()
        {
            if (isEnable)
            {
                isInputEnabled = true;
                publisherThread = new Thread(Publish);
                publisherThread.Start();
            }
            else
            {
                throw new ConnectionClosedException("Connection closed!");
            }
        }

        /// <summary>
        /// Stop the Input and close the Stream
        /// </summary>
        public virtual void CloseConnection()
        {
            isInputEnabled = false;
            isEnable = false;
            manualEvent.Set();
            OnConnectionClosedCall();
        }

        #endregion
    }
}
