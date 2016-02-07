namespace NETLIB.UDP
{
    public class UDPPack : BasePack
    {
        #region Variáveis



        #endregion

        #region Constructor

        public UDPPack() : base() 
        {
            this.readPosition += sizeof(int) + 15;
        }

        public UDPPack(byte[] buffer) : base(buffer)
        {
            this.readPosition += sizeof(int) + 15;
        }

        public UDPPack(BasePack basePack) : base(basePack) 
        {
            this.readPosition += sizeof(int) + 15;
        }


        #endregion

        #region Atributos

        public string Address
        {
            get { return this.GetString(1); }
        }

        #endregion

        #region Métodos



        #endregion

    }
}
