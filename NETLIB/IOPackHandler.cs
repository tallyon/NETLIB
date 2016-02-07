using System;
using System.IO;

namespace NETLIB
{
    public class ConnectionClosedException : IOException
    {
        #region Contructor

        public ConnectionClosedException() : base(){ }

        public ConnectionClosedException(string message)
            : base(message)
        { }

        #endregion
    }

    public class ConnectionRunnigException : IOException
    {
        #region Contructor

        public ConnectionRunnigException() : base() { }

        public ConnectionRunnigException(string message)
            : base(message)
        { }

        #endregion
    }

    public abstract class IOPackHandler<TPack> : Consumer<TPack> where TPack : BasePack
    {
        #region Variáveis

        Action<Consumer<TPack>, TPack>[] triggers;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the IOPackHandler
        /// </summary>
        /// <param name="publisher">Publisher to consume</param>
        /// <param name="eventDict">The dictionary reference of the pack's header trigger</param>
        public IOPackHandler(Publisher publisher, Action<Consumer<TPack>, TPack>[] eventDict)
            : base(publisher)
        {
            if (eventDict.Length <= sizeof(byte))
            {
                this.triggers = eventDict;   
            }
            else
            {
                throw new ArgumentException("Dicionário maior que o esperado!");
            }
        }

        /// <summary>
        /// Initialize the IOPackHandler
        /// </summary>
        /// <param name="publisher">Publisher to consume</param>
        public IOPackHandler(Publisher publisher)
            : base(publisher)
        {
            this.triggers = new Action<Consumer<TPack>, TPack>[256];
        }


        #endregion

        #region Atributos

        /// <summary>
        /// Dictionary byte events
        /// </summary>
        public Action<Consumer<TPack>, TPack>[] Triggers
        {
            get { return triggers; }
            set { triggers = value; }
        }

        /// <summary>
        ///     Gets the event regarding a key
        /// </summary>
        /// <param name="index">Index of the pack</param> 
        /// <returns>EventHandler event</returns>
        /// <exception cref="KeyNotFoundException">When the imput index do not exists</exception>
        /// <exception cref=""
        public Action<Consumer<TPack>, TPack> this[byte index]
        {
            get
            {
                return triggers[index];
            }

            set
            {
                triggers[index] = value;
            }
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Set new triggers to the PackHandler
        /// </summary>
        /// <param name="eventDict">New dictionary of triggers</param>
        public void SetTriggers(Action<Consumer<TPack>, TPack>[] eventDict)
        {
            if (eventDict.Length <= sizeof(byte))
            {
                this.triggers = eventDict;
            }
            else
            {
                throw new ArgumentException("Dicionário maior que o esperado!");
            }
        }

        /// <summary>
        /// Add a eathod that will be called when the pack's header is 'Key'
        /// </summary>
        /// <param name="key">Pack's header</param>
        /// <param name="value">Method</param>
        public void AddTrigger(byte key, Action<Consumer<TPack>, TPack> value)
        {
            triggers[key] += value;
        }

        /// <summary>
        /// Remove a pack's trigger that has been called
        /// </summary>
        /// <param name="key">Pack's header</param>
        public void RemoveTrigge(byte key)
        {
            triggers[key] = null;
        }

        /// <summary>
        /// Clear all the triggers
        /// </summary>
        public void ClearTriggers()
        {
            for (int i = 0; i < triggers.Length; i++)
            {
                triggers[i] = null;
            }
            base.ClearEvets();
        }

        /// <summary>
        ///  Function to handle with the pack
        /// </summary>
        /// <param name="buffer">Base of the pack</param>
        protected override void OnReceivedPackCall(TPack buffer)
        {
            if (triggers[buffer.ID] != null)
            {
                triggers[buffer.ID](this, buffer);
            }
            else
            {
                base.OnReceivedPackCall(buffer);
            }
        }

        #endregion
    }
}
