using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace NETLIB
{
    /// <summary>
    /// Delegate used to encapsulate a method tha will be called when a pack arrives
    /// </summary>
    /// <typeparam name="TPack">Type derived from BasePack that specify the pack to be used</typeparam>
    /// <param name="consumer">Consumer that called that method</param>
    /// <param name="receivedPack">Pack that arrived</param>
    public delegate void ThrowPackHandler<TPack>(Consumer<TPack> consumer, TPack receivedPack) where TPack : BasePack;

    /// <summary>
    /// Consume the pack published by a publisher
    /// </summary>
    /// <typeparam name="TPack"></typeparam>
    public abstract class Consumer<TPack> : IDisposable where TPack : BasePack
    {
        #region Variables

        public event ThrowPackHandler<TPack> ReceivedPack;
        public event Action<Consumer<TPack>> ConnectionClosed;

        Publisher publisher;

        Queue<byte[]> packQueue;

        Thread consumerThread;

        bool isEnabled;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new Consumer
        /// </summary>
        /// <param name="publisher">Publisher to be consumed</param>
        public Consumer(Publisher publisher)
        {
            this.isEnabled = false;
            this.publisher = publisher;
            this.packQueue = publisher.PackQueue;
        }

        /// <summary>
        /// End the consume
        /// </summary>
        ~Consumer()
        {
            EndConsume();
        }

        /// <summary>
        /// End the consume
        /// </summary>
        public void Dispose()
        {
            EndConsume();
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Gets if the consume is enabled
        /// </summary>
        public bool IsEnabled
        {
            get { return isEnabled; }
        }

        /// <summary>
        /// Gets is the input is enabled
        /// </summary>
        public bool IsPublishEnabled
        {
            get { return publisher.IsEnable; }
        }

        /// <summary>
        /// Gets the publisher
        /// </summary>
        public Publisher Publisher
        {
            get { return publisher; }
            set { publisher = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Call Publisher.SendPack
        /// </summary>
        /// <param name="packable">Pack</param>
        /// <param name="ip">Optional parameter used only by UDP protocol</param>
        public virtual void SendPack(TPack pack, IPEndPoint ip = null)
        {
            publisher.SendPack(pack, ip);
        }

        /// <summary>
        /// Call Publisher.SendPack
        /// </summary>
        /// <param name="pack">Pack to be sender</param>
        /// <param name="ip">Optional parameter used only by UDP protocol</param>
        public virtual void SendPack(byte[] pack, IPEndPoint ip = null)
        {
            publisher.SendPack(pack, ip);
        }

        /// <summary>
        ///  Function to handle with the pack
        /// </summary>
        /// <param name="buffer">Base of the pack</param>
        protected virtual void OnReceivedPackCall(TPack buffer)
        {
            if (ReceivedPack != null)
            {
                ReceivedPack(this, buffer);
            }
        }

        /// <summary>
        /// Begin the imput 
        /// </summary>
        public void StartConsume()
        {
            if (publisher.IsEnable)
            {
                if (consumerThread != null && consumerThread.IsAlive)
                {
                    throw new System.Exception();
                }

                consumerThread = new Thread(Consume);
                consumerThread.Start();
            }
            else
            {
                throw new ConnectionClosedException("Connection closed!");
            }
        }

        /// <summary>
        /// Begin the publish enad the consume
        /// </summary>
        public void Start()
        {
            publisher.Start();
            StartConsume();
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        public void CloseConnection()
        {
            publisher.CloseConnection();
        }

        /// <summary>
        /// Finhish the consume
        /// </summary>
        public void EndConsume()
        {
            if (consumerThread != null)
            {
                isEnabled = false;
            }
        }

        /// <summary>
        /// Finhish the publish and the consume
        /// </summary>
        public void EndPublishConsume()
        {
            if (consumerThread != null)
            {
                publisher.CloseConnection();
                isEnabled = false;
            }
        }

        /// <summary>
        /// Consume the packs
        /// </summary>
        private void Consume()
        {
            while (publisher.IsImputEnabled && isEnabled)
            {
                while (packQueue.Count > 0)
                {
                    OnReceivedPackCall(PackFactory(packQueue.Dequeue()));
                }

                publisher.ManualEvent.WaitOne();
                publisher.ManualEvent.Reset();
            }

            while (publisher.PackQueue.Count > 0)
            {
                OnReceivedPackCall(PackFactory(packQueue.Dequeue()));
            }

            OnConnectionClosedCall();
        }

        /// <summary>
        /// Set the class events to NULL
        /// </summary>
        protected void ClearEvents()
        {
            this.ReceivedPack = null;
            this.ConnectionClosed = null;
        }

        private void OnConnectionClosedCall()
        {
            if (ConnectionClosed != null)
            {
                ConnectionClosed(this);
            }
        }

        /// <summary>
        /// Buld a TPack Pack
        /// </summary>
        /// <param name="pack">BasePack to be based</param>
        /// <returns>New TPack pack</returns>
        public abstract TPack PackFactory(BasePack pack);

        /// <summary>
        /// Buld a TPack Pack
        /// </summary>
        /// <param name="pack">Buffer to be based</param>
        /// <returns>New TPack pack</returns>
        public abstract TPack PackFactory(byte[] pack);

        /// <summary>
        /// Buld a TPack Pack
        /// </summary>
        /// <returns>New TPack pack</returns>
        public abstract TPack PackFactory();

        #endregion
    }
}
