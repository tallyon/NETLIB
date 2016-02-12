using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace NETLIB
{
    /// <summary>
    /// Consume the pack published by a publisher
    /// </summary>
    /// <typeparam name="TPack"></typeparam>
    public abstract class Consumer<TPack> : IDisposable where TPack : BasePack
    {
        #region Variáveis

        public event Action<Consumer<TPack>, TPack> ReceivedPack;
        public event Action<Consumer<TPack>> ConnectionClosed;

        Publisher publisher;

        Queue<byte[]> packQueue;

        Thread consumerThread;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new Consumer
        /// </summary>
        /// <param name="publisher">Publisher to be consumed</param>
        public Consumer(Publisher publisher)
        {
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

        #region Atributos

        /// <summary>
        /// Gets the enable state of the imput/output
        /// </summary>
        public bool Enable
        {
            get { return publisher.Enable; }
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

        #region Métodos

        /// <summary>
        /// Call Publisher.SendPack
        /// </summary>
        /// <param name="packable">Pack</param>
        public virtual void SendPack(TPack pack, IPEndPoint IP = null)
        {
            publisher.SendPack(pack, IP);
        }

        /// <summary>
        /// Call Publisher.SendPack
        /// </summary>
        /// <param name="packable">Pack</param>
        public virtual void SendPack(byte[] pack, IPEndPoint IP = null)
        {
            publisher.SendPack(pack, IP);
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
        public void BeginConsume()
        {
            if (publisher.Enable)
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
                throw new ConnectionClosedException("Conexão já encerrada!");
            }
        }

        /// <summary>
        /// Begin the publish enad the consume
        /// </summary>
        public void BeginPublishConsume()
        {
            publisher.BeginPublish();
            BeginConsume();
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
                consumerThread.Interrupt();
                consumerThread.Abort();
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
                consumerThread.Abort();
            }
        }

        /// <summary>
        /// Consume the packs
        /// </summary>
        private void Consume()
        {
            while (publisher.ImputEnabled)
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
        protected void ClearEvets()
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
