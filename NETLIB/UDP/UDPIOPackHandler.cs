namespace NETLIB.UDP
{
    public class UDPIOPackHandler : IOPackHandler<UDPPack>
    {
        #region Variables



        #endregion

        #region Constructor

        public UDPIOPackHandler(UDPPublisher publisher) : base(publisher) { }

        public UDPIOPackHandler(UDPPublisher publisher, ThrowPackHandler<UDPPack>[] eventDict) : base(publisher, eventDict) { }

        #endregion

        #region Attributes



        #endregion

        #region Methods

        public override UDPPack PackFactory(BasePack pack)
        {
            return new UDPPack(pack);
        }

        public override UDPPack PackFactory(byte[] pack)
        {
            return new UDPPack(pack);
        }

        public override UDPPack PackFactory()
        {
            return new UDPPack();
        }

        #endregion
    }
}
