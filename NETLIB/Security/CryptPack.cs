namespace NETLIB.Security
{
    public class CryptPack : BasePack
    {
        #region Variables

        const int CID_POSITION = 2;

        protected int beginOfCryptography = 2;
        protected bool encrypted  = true;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialize the CryptoPack
        /// </summary>
        public CryptPack() 
            : base() 
        {
            this.PutBool(true, 1);
            this.readPosition = CID_POSITION + sizeof(int);
            this.writePosition = CID_POSITION + sizeof(int);
        }

        /// <summary>
        /// Initialize the Crypto pack
        /// </summary>
        /// <param name="encrypted">Define if the CryptoPack is encrypted</param>
        public CryptPack(bool encrypted)
            : base()
        {
            this.encrypted = encrypted;
            this.PutBool(encrypted, 1);
            this.readPosition = CID_POSITION + sizeof(int);
            this.writePosition = CID_POSITION + sizeof(int);
        }

        /// <summary>
        ///     Initialize the CryptoPack
        /// </summary>
        /// <param name="basePack">BasePack that will be copy</param>
        public CryptPack(BasePack basePack) 
            : base(basePack) 
        {
            this.encrypted = GetBool(1);
            this.readPosition = CID_POSITION + sizeof(int);
            this.writePosition = CID_POSITION + sizeof(int);
        }

        /// <summary>
        ///     Initialize the CryptoPack
        /// </summary>
        /// <param name="basePack">Buffer that will be copy</param>
        public CryptPack(byte[] buffer) 
            : base(buffer) 
        {
            this.encrypted = GetBool(1);
            this.readPosition = this.beginOfCryptography + sizeof(int);
            this.writePosition = this.beginOfCryptography + sizeof(int);
        }

        #endregion

        #region Attributes

        public override byte ID
        {
            get { return base.ID; }
            set
            {
                base.ID = value;
                if (encrypted)
                {
                    this.PutInt((int)this.ID, CID_POSITION);
                }
            }
        }

        /// <summary>
        /// Return the begin of the cryptography
        /// </summary>
        public virtual int BeginOfCryptography
        {
            get { return beginOfCryptography; }
            set { beginOfCryptography = value; }
        }

        /// <summary>
        /// Returns if the pack is corrupted
        /// </summary>
        public virtual bool IsCorrupted
        {
            get 
            {
                return  encrypted && (ID != this.GetInt(CID_POSITION));
            }
        }

        public virtual bool IsEncrypted
        {
            get { return encrypted; }
        }

        #endregion

        #region Methods



        #endregion
    }
}
