using System;
using System.IO;

namespace NETLIB
{
    #region teste



























    #endregion



    /// <summary>
    /// Handle the input and output packs using events
    /// </summary>
    /// <typeparam name="TPack"></typeparam>
    public abstract class IOPackHandler<TPack> : Consumer<TPack> where TPack : BasePack
    {
        #region Variables

        private ThrowPackHandler<TPack>[] triggers;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the IOPackHandler
        /// </summary>
        /// <param name="publisher">Publisher to consume</param>
        /// <param name="eventDict">The dictionary reference of the pack's header trigger</param>
        public IOPackHandler(Publisher publisher, ThrowPackHandler<TPack>[] eventDict)
            : base(publisher)
        {
            if (eventDict != null)
            {
                if (eventDict.Length <= byte.MaxValue)
                {
                    this.triggers = eventDict;
                }
                else
                {
                    throw new ArgumentException("Out of range", "eventDict");
                }
            }
            else
            {
                throw new ArgumentNullException("Null", "eventDict");
            }
        }

        /// <summary>
        /// Initialize the IOPackHandler
        /// </summary>
        /// <param name="publisher">Publisher to consume</param>
        public IOPackHandler(Publisher publisher)
            : base(publisher)
        {
            this.triggers = new ThrowPackHandler<TPack>[256];
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Dictionary byte events
        /// </summary>
        public ThrowPackHandler<TPack>[] Triggers
        {
            get { return triggers; }
            set { triggers = value; }
        }

        /// <summary>
        /// Gets the method reletad to a specific id
        /// </summary>
        /// <param name="index">Index of the pack</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException">When the imput index do not exists</exception>
        public ThrowPackHandler<TPack> this[byte index]
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

        #region Methods

        /// <summary>
        /// Set new triggers to the PackHandler
        /// </summary>
        /// <param name="eventDict">New dictionary of triggers</param>
        public void SetTriggers(ThrowPackHandler<TPack>[] eventDict)
        {
            if (eventDict.Length <= byte.MaxValue)
            {
                this.triggers = eventDict;
            }
            else
            {
                throw new ArgumentException("Out of range", "eventDict");
            }
        }

        /// <summary>
        /// Add a method that will be called when the pack's header is equal 'Key'
        /// </summary>
        /// <param name="key">Pack's header</param>
        /// <param name="value">Method</param>
        public void AddTrigger(byte key, ThrowPackHandler<TPack> value)
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
            base.ClearEvents();
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
