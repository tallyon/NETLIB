using System.Net;
using Crypt;

namespace NETLIB.Security
{
    public class CryptIOPackHandler : IOPackHandler<CryptPack>
    {
        #region Variáveis

        RotorServiceProvider cryptProvider;

        bool encrypted = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new CryptIOPackHandler
        /// </summary>
        /// <param name="publisher"> Publisher to be cosumed</param>
        /// <param name="num_rotores">Number of rotors</param>
        /// <param name="keys">Keys of the rotors</param>
        /// <param name="num_encriptacoes">Number of encrypt to rotate</param>
        /// <param name="num_rotacoes">Number of revolutions</param>
        public CryptIOPackHandler(Publisher publisher, int num_rotores, string[] keys, int[] num_encriptacoes, int[] num_rotacoes)
            : base(publisher)
        {
            cryptProvider = new RotorServiceProvider(num_rotores, keys, num_encriptacoes, num_rotacoes);
            encrypted = true;
        }

        /// <summary>
        /// Create a new CryptIOPackHandler
        /// </summary>
        /// <param name="publisher"> Publisher to be cosumed</param>
        /// <param name="num_rotores">Number of rotors</param>
        /// <param name="keys">Keys of the rotors</param>
        public CryptIOPackHandler(Publisher publisher, int num_rotores, string[] keys)
            : base(publisher)
        {
            cryptProvider = new RotorServiceProvider(num_rotores, keys);
            encrypted = true;
        }


        /// <summary>
        /// Create a new CryptIOPackHandler
        /// </summary>
        /// <param name="publisher"> Publisher to be cosumed</param>
        /// <param name="num_rotores">Number of rotors</param>
        /// <param name="key">Key of the keys of the rotors</param>
        public CryptIOPackHandler(Publisher publisher, int num_rotores, string key)
            : base(publisher)
        {
            cryptProvider = new RotorServiceProvider(num_rotores, key);
            encrypted = true;
        }

        /// <summary>
        /// Create a new CryptIOPackHandler
        /// </summary>
        /// <param name="publisher">Publisher to be cosumed</param>
        public CryptIOPackHandler(Publisher publisher) : base(publisher) { encrypted = false; }


        #endregion

        #region Atributos

        public bool Encryptated
        {
            get { return encrypted; }
        }

        #endregion

        #region Métodos

        public void BeginEncrypt(int num_rotores, string[] keys, int[] num_encriptacoes, int[] num_rotacoes)
        {
            cryptProvider = new RotorServiceProvider(num_rotores, keys, num_encriptacoes, num_rotacoes);
            encrypted = true;
        }

        public void BeginEncrypt(int num_rotores, string[] keys)
        {
            cryptProvider = new RotorServiceProvider(num_rotores, keys);
            encrypted = true;
        }

        public void BeginEncrypt(int num_rotores, string key)
        {
            cryptProvider = new RotorServiceProvider(num_rotores, key);
            encrypted = true;
        }

        public void EndEncrypt()
        {
            this.encrypted = false;
        }

        public override void SendPack(CryptPack pack, IPEndPoint IP = null)
        {
            if (encrypted && pack.IsEncrypted)
            {
                byte[] buffer = cryptProvider.GetEncrypt(pack.Buffer, pack.BeginOfCryptography, pack.Length - pack.BeginOfCryptography);
                base.SendPack(buffer, IP);
            }
            else
            {
                base.SendPack(pack, IP);
            }
        }

        protected override void OnReceivedPackCall(CryptPack pack)
        {
            if (encrypted && pack.IsEncrypted)
            {
                cryptProvider.Decrypt(pack.Buffer, pack.BeginOfCryptography, pack.Length - pack.BeginOfCryptography);
            }

            base.OnReceivedPackCall(pack);         
        }

        public override CryptPack PackFactory(BasePack pack)
        {
            return new CryptPack(pack);
        }

        public override CryptPack PackFactory(byte[] pack)
        {
            return new CryptPack(pack);
        }

        public override CryptPack PackFactory()
        {
            return new CryptPack();
        }

        #endregion
    }
}