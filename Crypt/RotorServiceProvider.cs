using System;

namespace Crypt
{
    public class RotorServiceProvider
    {
        #region Variáveis

        const int DEFAULT_CIFRAS_PARA_ROTACIONAR = 1;
        const int DEFAULT_PASSOS_MAXIMOS_POR_ROTACAO = 3;

        Rotor[] rotores;

        #endregion

        #region Constructor

        public RotorServiceProvider(int numRotores, string[] keys, int[] cifrasParaRotacionar, int[] passosMaximosPorRotacao)
        {
            if (keys.Length < numRotores | cifrasParaRotacionar.Length < numRotores | passosMaximosPorRotacao.Length < numRotores)
            {
                throw new ArgumentOutOfRangeException("Número de parâmetros não condiz com o número de rotores");
            }

            this.rotores = new Rotor[numRotores];

            for (int i = 0; i < rotores.Length; i++)
            {
                this.rotores[i] = new Rotor(keys[i], cifrasParaRotacionar[i], passosMaximosPorRotacao[i]);
            }
        }

        public RotorServiceProvider(int numRotores, string[] keys, int cifrasParaRotacionar, int passosMaximosPorRotacao)
        {
            if (keys.Length < numRotores)
            {
                throw new ArgumentOutOfRangeException("Número de keys não condiz com o número de rotores");
            }

            this.rotores = new Rotor[numRotores];

            for (int i = 0; i < rotores.Length; i++)
            {
                this.rotores[i] = new Rotor(keys[i], cifrasParaRotacionar, passosMaximosPorRotacao);
            }
        }

        public RotorServiceProvider(int numRotores, string key, int cifrasParaRotacionar, int passosMaximosPorRotacao)
        {
            this.rotores = new Rotor[numRotores];
            string[] keys = KeyServiceProvider.GerarKeys(key, numRotores);

            for (int i = 0; i < rotores.Length; i++)
            {
                this.rotores[i] = new Rotor(keys[i], cifrasParaRotacionar, passosMaximosPorRotacao);
            }
        }

        public RotorServiceProvider(int numRotores, string key, int passosMaximosPorRotacao)
        {
            this.rotores = new Rotor[numRotores];
            string[] keys = KeyServiceProvider.GerarKeys(key, numRotores);

            for (int i = 0; i < rotores.Length; i++)
            {
                this.rotores[i] = new Rotor(keys[i], DEFAULT_CIFRAS_PARA_ROTACIONAR, passosMaximosPorRotacao);
            }
        }

        public RotorServiceProvider(int numRotores, string[] keys)
        {
            if (keys.Length < numRotores)
            {
                throw new ArgumentOutOfRangeException("Número de parâmetros não condiz com o número de rotores");
            }

            this.rotores = new Rotor[numRotores];

            for (int i = 0; i < rotores.Length; i++)
            {
                this.rotores[i] = new Rotor(keys[i], DEFAULT_CIFRAS_PARA_ROTACIONAR, DEFAULT_PASSOS_MAXIMOS_POR_ROTACAO);
            }
        }

        public RotorServiceProvider(int numRotores, string key)
        {
            this.rotores = new Rotor[numRotores];
            string[] keys = KeyServiceProvider.GerarKeys(key, numRotores);

            for (int i = 0; i < rotores.Length; i++)
            {
                this.rotores[i] = new Rotor(keys[i], DEFAULT_CIFRAS_PARA_ROTACIONAR, DEFAULT_PASSOS_MAXIMOS_POR_ROTACAO);
            }
        }

        public RotorServiceProvider(string key)
        {
            this.rotores = new Rotor[1];

            rotores[0] = new Rotor(key, DEFAULT_CIFRAS_PARA_ROTACIONAR, DEFAULT_PASSOS_MAXIMOS_POR_ROTACAO);
        }

        #endregion

        #region Atributos

        public Rotor[] Rotores
        {
            get { return rotores; }
        }

        #endregion

        #region Métodos

        public void Encrypt(byte[] buffer, int offset, int count)
        {
            int fim = offset + count;
            for (int i = 0; i < rotores.Length; i++)
            {
                for (int j = offset; j < fim; j++)
                {
                    rotores[i].Encrypt(ref buffer[j]);
                }
            }
        }

        public void Decrypt(byte[] buffer, int offset, int count)
        {
            int fim = offset + count;
            for (int i = rotores.Length - 1; i >= 0; i--)
            {
                for (int j = offset; j < fim; j++)
                {
                    rotores[i].Decrypt(ref buffer[j]);
                }
            }
        }

        public byte[] GetEncrypt(byte[] buffer, int offset, int count)
        {
            byte[] outBuffer = new byte[buffer.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                outBuffer[i] = buffer[i];
            }

            int fim = offset + count;
            for (int i = 0; i < rotores.Length; i++)
            {
                for (int j = offset; j < fim; j++)
                {
                    rotores[i].Encrypt(ref outBuffer[j]);
                }
            }
            return outBuffer;
        }

        public byte[] GetDecrypt(byte[] buffer, int offset, int count)
        {
            byte[] outBuffer = new byte[buffer.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                outBuffer[i] = buffer[i];
            }
            int fim = offset + count;
            for (int i = rotores.Length - 1; i >= 0; i--)
            {
                for (int j = offset; j < fim; j++)
                {
                    rotores[i].Decrypt(ref outBuffer[j]);
                }
            }
            return outBuffer;
        }

        #endregion
    }

    public static class KeyServiceProvider
    {
        #region Variáveis



        #endregion

        #region Constructor



        #endregion

        #region Atributos



        #endregion

        #region Métodos

        public static string[] GerarKeys(string key, int num_keys)
        {
            string[] ret = new string[num_keys];
            ret[0] = key;
            for (int i = 1; i < num_keys; i++)
            {
                ret[i] = RC4(ret[i - 1]);
            }
            return ret;
        }

        private static string RC4(string key)
        {
            int aux;
            int j = 0;
            int[] vet = new int[256];

            for (int i = 0; i < 256; i++)
            {
                vet[i] = (char)i;
            }

            for (int i = 0; i < 256; ++i)
            {
                j = (j + vet[i] + key[i % key.Length]) % 256;
                aux = vet[i];
                vet[i] = vet[j];
                vet[j] = (char)aux;
            }

            string s = string.Empty;
            for (int i = 0; i < 256; i++)
            {
                s += (char)vet[i];
            }

            return s;
        }

        #endregion
    }
}
