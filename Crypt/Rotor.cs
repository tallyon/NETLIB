namespace Crypt
{
    public class Rotor
    {
        #region Variables

        int cifrasParaRotacionar;
        int passosMaximosPorRotacao;

        int encriptacoesFeitas;
        int passosDeEncryptFeitos;
        int passosDeEncryptPorRotacao;

        int decriptacoesFeitas;
        int passosDeDecryptFeitos;
        int passosDeDecryptPorRotacao;

        int[] encryptVet;
        int[] decryptVet;

        #endregion

        #region Constructor

        public Rotor(string key, int cifrasParaRotacionar, int passosMaximosPorRotacao)
        {

            this.cifrasParaRotacionar = cifrasParaRotacionar;
            this.passosMaximosPorRotacao = passosMaximosPorRotacao;

            this.encriptacoesFeitas = 0;
            this.decriptacoesFeitas = 0;

            this.passosDeEncryptFeitos = 0;
            this.passosDeDecryptFeitos = 0;

            this.passosDeEncryptPorRotacao = 1;
            this.passosDeDecryptPorRotacao = 1;

            encryptVet = new int[256];
            decryptVet = new int[256];

            RC4(key, encryptVet);
            InitializeDecryptVet(encryptVet, decryptVet);
        }

        #endregion

        #region Attributes

        public int CifrasParaRotacionar
        {
            get { return cifrasParaRotacionar; }
        }

        public int PassosMaximosPorRotacao
        {
            get { return passosMaximosPorRotacao; }
        }

        public int EncriptacoesFeitas
        {
            get { return encriptacoesFeitas; }
        }

        public int DecriptacoesFeitas
        {
            get { return decriptacoesFeitas; }
        }

        #endregion

        #region Methods

        private void EncryptRotation()
        {
            if (encriptacoesFeitas == cifrasParaRotacionar - 1)
            {
                encriptacoesFeitas = 0;

                passosDeEncryptFeitos += passosDeEncryptPorRotacao;
                passosDeEncryptFeitos %= 256;

                passosDeEncryptPorRotacao = (passosDeEncryptPorRotacao == passosMaximosPorRotacao) ? 1 : (passosDeEncryptPorRotacao + 1);
            }
            else
            {
                encriptacoesFeitas++;
            }
        }

        private void DecryptRotation()
        {
            if (decriptacoesFeitas == cifrasParaRotacionar - 1)
            {
                decriptacoesFeitas = 0;

                passosDeDecryptFeitos += passosDeDecryptPorRotacao;
                passosDeDecryptFeitos %= 256;

                passosDeDecryptPorRotacao = (passosDeDecryptPorRotacao == passosMaximosPorRotacao) ? 1 : (passosDeDecryptPorRotacao + 1);
            }
            else
            {
                decriptacoesFeitas++;
            }
        }

        public byte Encrypt(byte val)
        {
            int retorno = encryptVet[(val + passosDeEncryptFeitos) % 256];
            EncryptRotation();
            return (byte)retorno;
        }

        public byte Decrypt(byte val)
        {
            int retorno = (decryptVet[val] - passosDeDecryptFeitos + 256) % 256;
            DecryptRotation();
            return (byte)retorno;
        }

        public void Encrypt(ref byte val)
        {
            val = (byte)encryptVet[(val + passosDeEncryptFeitos) % 256];
            EncryptRotation();
        }

        public void Decrypt(ref byte val)
        {
            val = (byte)((decryptVet[val] - passosDeDecryptFeitos + 256) % 256);
            DecryptRotation();
        }

        private void RC4(string key, int[] vet)
        {
            int aux;
            int j = 0;

            for (int i = 0; i < 256; i++)
            {
                vet[i] = i;
            }

            for (int i = 0; i < 256; ++i)
            {
                j = (j + vet[i] + key[i % key.Length]) % 256;
                aux = vet[i];
                vet[i] = vet[j];
                vet[j] = aux;
            }
        }

        private void InitializeDecryptVet(int[] encryptVet, int[] decryptVet)
        {
            for (int i = 0; i < encryptVet.Length; i++)
            {
                decryptVet[encryptVet[i]] = i;
            }
        }

        #endregion
    }
}
