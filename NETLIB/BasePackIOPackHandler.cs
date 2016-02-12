using System;

namespace NETLIB
{
    public class BasePackIOPackHandler : IOPackHandler<BasePack>
    {
        #region Variables



        #endregion

        #region Constructor

        public BasePackIOPackHandler(Publisher publisher) : base(publisher) { }

        public BasePackIOPackHandler(Publisher publisher, Action<Consumer<BasePack>, BasePack>[] eventDict) : base(publisher, eventDict) { }

        #endregion

        #region Attributes



        #endregion

        #region Methods

        public override BasePack PackFactory(BasePack pack)
        {
            return pack;
        }

        public override BasePack PackFactory(byte[] pack)
        {
            return pack;
        }

        public override BasePack PackFactory()
        {
            return new BasePack();
        }

        #endregion
    }
}
