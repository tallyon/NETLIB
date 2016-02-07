using System;

namespace NETLIB
{
    /// <summary>
    /// Class that override the buffer and manage the header
    /// </summary>
    public class BasePack
    {
        #region Variáveis

        public static int packSize = 1500;

        protected byte[] buffer;

        protected int readPosition = 1;
        protected int writePosition = 1;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialize the BasePack
        /// </summary>
        public BasePack()
        {
            this.buffer = new byte[packSize];
        }

        /// <summary>
        /// Initialize the BasePack
        /// </summary>
        /// <param name="basePack">BasePack that will be copy</param>
        protected BasePack(BasePack basePack)
        {
            this.buffer = basePack.buffer;
        }

        /// <summary>
        /// Initialize the BasePack
        /// </summary>
        /// <param name="buffer">Source buffer</param>
        protected BasePack(byte[] buffer)
        {
            if (buffer.Length == packSize)
            {
                this.buffer = buffer;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Entrada não entra nos parâmetros do pacote!");
            }
        }

        #endregion

        #region Atributos

        /// <summary>
        /// Pack's Header
        /// </summary>
        public virtual byte ID
        {
            get { return buffer[0]; }

            set { buffer[0] = value; }
        }

        /// <summary>
        /// Gets the buffer's byte
        /// </summary>
        /// <param name="index">Index of the byte</param>
        /// <returns></returns>
        public virtual byte this[int index]
        {
            get { return buffer[index]; }

            set { buffer[index] = value; }
        }

        /// <summary>
        /// Pack's Length
        /// </summary>
        public virtual int Length
        {
            get { return buffer.Length; }
        }

        /// <summary>
        /// The pack size related to BasePack.packSize
        /// </summary>
        public virtual int PackSize
        {
            get { return packSize; }
            set { packSize = value; }
        }

        /// <summary>
        ///     Returns the pack's buffer
        /// </summary>
        /// <exception cref = "ArgumentOutOfRangeException">
        ///     When the setter's buffer is larger than the maximum packet size
        /// </exception> 
        public virtual byte[] Buffer
        {
            get { return buffer; }
        }

        /// <summary>
        /// Return the read position
        /// </summary>
        public virtual int ReadPosition
        {
            get { return readPosition; }
            set { readPosition = value; }
        }

        /// <summary>
        /// return the write position
        /// </summary>
        public virtual int WritePosition
        {
            get { return writePosition; }
            set { writePosition = value; }
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Write a buffer in the pack
        /// </summary>
        /// <param name="buffer">Buffer to be writed</param>
        /// <param name="offset">Begin of sub buffer</param>
        /// <param name="count">Lenth of sub buffer</param>
        public virtual void Write(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.buffer[writePosition] = buffer[offset + i];
                writePosition++;
            }
        }

        /// <summary>
        /// Read a buffer from the pack
        /// </summary>
        /// <param name="buffer">Buffer to past the data</param>
        /// <param name="offset">Begin of writing data</param>
        /// <param name="count">Lenth of th data</param>
        public virtual void Read(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                buffer[offset + i] = this.buffer[readPosition];
                readPosition++;
            }
        }

        /// <summary>
        /// Get a string in the readPosition of the pack
        /// </summary>
        /// <returns>String from the pack</returns>
        public virtual string GetString()
        {
            string result = string.Empty;
            int size = BitConverter.ToInt32(buffer, readPosition);

            if (size > this.Length - sizeof(int))
            {
                throw new FormatException("Entrada não se encaixa como uma string!");
            }

            for (int i = readPosition + sizeof(int); i < size + sizeof(int) + readPosition; i++)
            {
                result += (char)this.buffer[i];
            }
            readPosition += size + sizeof(int);
            return result;
        }

        /// <summary>
        /// Get a string in the offset of the pack
        /// </summary>
        /// <param name="offset">Index to get the string</param>
        /// <returns>String from the pack</returns>
        protected virtual string GetString(int offset)
        {
            string result = string.Empty;
            int size = BitConverter.ToInt32(buffer, offset);

            if (size > this.Length - sizeof(int))
            {
                throw new FormatException("Entrada não se encaixa como uma string!");
            }

            for (int i = offset + sizeof(int); i < size + sizeof(int) + offset; i++)
            {
                result += (char)this.buffer[i];
            }
            return result;
        }

        /// <summary>
        /// Get a int readPosition From the pack
        /// </summary>
        /// <returns>A int from the pack</returns>
        public virtual int GetInt()
        {
            int ret = BitConverter.ToInt32(buffer, readPosition);
            readPosition += sizeof(int);
            return ret;
        }

        /// <summary>
        /// Get a int offset From the pack
        /// </summary>
        /// <param name="offset">Index to get the int</param>
        /// <returns>A int from the pack</returns>
        protected virtual int GetInt(int offset)
        {
            return BitConverter.ToInt32(buffer, offset);
        }

        /// <summary>
        /// Get a double readPosition From the pack
        /// </summary>
        /// <returns>A double from the pack</returns>
        public virtual double GetDouble()
        {
            double ret = BitConverter.ToDouble(buffer, readPosition);
            readPosition += sizeof(double);
            return ret;
        }

        /// <summary>
        /// Get a double offset From the pack
        /// </summary>
        /// <param name="offset">Index to get the double</param>
        /// <returns>A double from the pack</returns>
        protected virtual double GetDouble(int offset)
        {
            return BitConverter.ToDouble(buffer, offset);
        }

        /// <summary>
        /// Get a float readPosition From the pack
        /// </summary>
        /// <returns>A float from the pack</returns>
        public virtual float GetFloat()
        {
            float ret = BitConverter.ToSingle(buffer, readPosition);
            readPosition += sizeof(float);
            return ret;
        }

        /// <summary>
        /// Get a float offset From the pack
        /// </summary>
        /// <param name="offset">Index to get the float</param>
        /// <returns>A float from the pack</returns>
        protected virtual float GetFloat(int offset)
        {
            return BitConverter.ToSingle(buffer, offset);
        }

        /// <summary>
        /// Get a char readPosition From the pack
        /// </summary>
        /// <returns>A char from the pack</returns>
        public virtual char GetChar()
        {
            char ret = (char)buffer[readPosition];
            readPosition++;
            return ret;
        }

        /// <summary>
        /// Get a char offset From the pack
        /// </summary>
        /// <param name="offset">Index to get the char</param>
        /// <returns>A char from the pack</returns>
        protected virtual char GetChar(int offset)
        {
            return (char)buffer[offset];
        }

        /// <summary>
        /// Get a byte readPosition From the pack
        /// </summary>
        /// <returns>A byte from the pack</returns>
        public virtual byte GetByte()
        {
            byte retorno = buffer[readPosition];
            readPosition++;
            return retorno;
        }

        /// <summary>
        /// Get a byte offset From the pack
        /// </summary>
        /// <param name="offset">Index to get the byte</param>
        /// <returns>A byte from the pack</returns>
        protected virtual byte GetByte(int offset)
        {
            return buffer[offset];
        }

        /// <summary>
        /// Get a bool readPosition From the pack
        /// </summary>
        /// <returns>A bool from the pack</returns>
        public virtual bool GetBool()
        {
            bool ret = buffer[readPosition] == 1;
            readPosition++;
            return ret;
        }

        /// <summary>
        /// Get a bool offset From the pack
        /// </summary>
        /// <param name="offset">Index to get the bool</param>
        /// <returns>A bool from the pack</returns>
        protected virtual bool GetBool(int offset)
        {
            return buffer[offset] == 1;
        }

        /// <summary>
        /// Get a custom Type packable from the pack
        /// </summary>
        /// <typeparam name="CustomType">IPackable Custom Type</typeparam>
        /// <returns>A custom type</returns>
        public virtual CustomType GetPackable<CustomType>() where CustomType : IPackable, new()
        {
            CustomType retorno = new CustomType();
            retorno.Unpack(this);
            return retorno;
        }

        /// <summary>
        /// Writes a string in the writePosition of the pack
        /// </summary>
        /// <param name="value"> String to be writed</param>
        public virtual void PutString(string value)
        {
            int i;
            byte[] aux = BitConverter.GetBytes(value.Length);

            for (i = 0; i < sizeof(int); i++)
            {
                buffer[i + writePosition] = aux[i];
            }

            for (; i < value.Length + sizeof(int); i++)
            {
                buffer[i + writePosition] = (byte)value[i - sizeof(int)];
            }

            writePosition += value.Length + sizeof(int);
        }

        /// <summary>
        /// Writes a string in the writePosition of the pack
        /// </summary>
        /// <param name="value">String to be writed</param>
        /// <param name="offset">Index to put the string</param>
        protected virtual void PutString(string value, int offset)
        {
            int i;
            byte[] aux = BitConverter.GetBytes(value.Length);

            for (i = 0; i < sizeof(int); i++)
            {
                buffer[i + offset] = aux[i];
            }

            for (; i < value.Length + sizeof(int); i++)
            {
                buffer[i + offset] = (byte)value[i - sizeof(int)];
            }
        }

        /// <summary>
        /// Writes a int in the writePosition of the pack
        /// </summary>
        /// <param name="value"> Int to be writed</param>
        public virtual void PutInt(int value)
        {
            int i;
            byte[] aux = BitConverter.GetBytes(value);

            for (i = 0; i < aux.Length; i++)
            {
                buffer[i + writePosition] = aux[i];
            }

            writePosition += aux.Length;
        }

        /// <summary>
        /// Writes a int in the writePosition of the pack
        /// </summary>
        /// <param name="value">Int to be writed</param>
        /// <param name="offset">Index to put the int</param>
        protected virtual void PutInt(int value, int offset)
        {
            int i;
            byte[] aux = BitConverter.GetBytes(value);

            for (i = 0; i < aux.Length; i++)
            {
                buffer[i + offset] = aux[i];
            }
        }

        /// <summary>
        /// Writes a double in the writePosition of the pack
        /// </summary>
        /// <param name="value"> Double to be writed</param>
        public virtual void PutDouble(double value)
        {
            int i;
            byte[] aux = BitConverter.GetBytes(value);

            for (i = 0; i < aux.Length; i++)
            {
                buffer[i + writePosition] = aux[i];
            }

            writePosition += aux.Length;
        }

        /// <summary>
        /// Writes a double in the writePosition of the pack
        /// </summary>
        /// <param name="value">Double to be writed</param>
        /// <param name="offset">Index to put the double</param>
        protected virtual void PutDouble(double value, int offset)
        {
            int i;
            byte[] aux = BitConverter.GetBytes(value);

            for (i = 0; i < aux.Length; i++)
            {
                buffer[i + offset] = aux[i];
            }
        }

        /// <summary>
        /// Writes a float in the writePosition of the pack
        /// </summary>
        /// <param name="value"> Float to be writed</param>
        public virtual void PutFloat(float value)
        {
            int i;
            byte[] aux = BitConverter.GetBytes(value);

            for (i = 0; i < aux.Length; i++)
            {
                buffer[i + writePosition] = aux[i];
            }

            writePosition += aux.Length;
        }

        /// <summary>
        /// Writes a float in the writePosition of the pack
        /// </summary>
        /// <param name="value">Float to be writed</param>
        /// <param name="offset">Index to put the float</param>
        protected virtual void PutFloat(float value, int offset)
        {
            int i;
            byte[] aux = BitConverter.GetBytes(value);

            for (i = 0; i < aux.Length; i++)
            {
                buffer[i + offset] = aux[i];
            }
        }

        /// <summary>
        /// Writes a char in the writePosition of the pack
        /// </summary>
        /// <param name="value"> Char to be writed</param>
        public virtual void PutChar(char value)
        {
            buffer[writePosition] = (byte)value;
            writePosition++;
        }

        /// <summary>
        /// Writes a char in the writePosition of the pack
        /// </summary>
        /// <param name="value">Char to be writed</param>
        /// <param name="offset">Index to put the char</param>
        protected virtual void PutChar(char value, int offset)
        {
            buffer[offset] = (byte)value;
        }

        /// <summary>
        /// Writes a byte in the writePosition of the pack
        /// </summary>
        /// <param name="value">Byte to be writed</param>
        public virtual void PutByte(byte value)
        {
            buffer[writePosition] = value;
            writePosition++;
        }

        /// <summary>
        /// Writes a byte in the writePosition of the pack
        /// </summary>
        /// <param name="value">Byte to be writed</param>
        /// <param name="offset">Index to put the byte</param>
        protected virtual void PutByte(byte value, int offset)
        {
            buffer[offset] = value;
        }

        /// <summary>
        /// Writes a bool in the writePosition of the pack
        /// </summary>
        /// <param name="value">Bool to be writed</param>
        public virtual void PutBool(bool value)
        {
            buffer[writePosition] = (value) ? (byte)1 : (byte)0;
            writePosition++;
        }

        /// <summary>
        /// Writes a bool in the writePosition of the pack
        /// </summary>
        /// <param name="value">Bool to be writed</param>
        /// <param name="offset">Index to put the bool</param>
        protected virtual void PutBool(bool value, int offset)
        {
            buffer[offset] = (value) ? (byte)1 : (byte)0;
        }

        /// <summary>
        /// Put a custom Type packable in the pack
        /// </summary>
        /// <param name="packable">Custom Type to be pack</param>
        public virtual void PutPackable<CustomType>(CustomType packable) where CustomType : IPackable
        {
            packable.Pack(this);
        }

        /// <summary>
        /// Retunrs a deep copy from this
        /// </summary>
        /// <returns>Deep copy BasePack</returns>
        public virtual BasePack DeepCopy()
        {
            BasePack retorno = new BasePack();

            for (int i = 0; i < packSize; i++)
            {
                retorno[i] = this[i];
            }
            return retorno;
        }

        /// <summary>
        ///      Initialize and returns the BazePack
        ///</summary>
        ///<param name="buffer">
        ///     The buffer that is the base of the pack
        ///</param>
        ///<exception cref = "ArgumentOutOfRangeException">
        ///     When the base_pack buffer is larger than the maximum packet size
        ///</exception>
        public static implicit operator BasePack(byte[] buffer)
        {
            if (buffer.Length == packSize)
            {
                return new BasePack(buffer);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Entrada não entra nos parâmetros do pacote!");
            }
        }

        #endregion

        #region Métodos estáticos

        /// <summary>
        /// Get a string in the offset of the pack
        /// </summary>
        /// <param name="offset">Index to get the string</param>
        /// <returns>String from the pack</returns>
        public static string GetString(byte[] buffer, int offset)
        {
            string result = string.Empty;
            int size = BitConverter.ToInt32(buffer, offset);

            if (size > buffer.Length - sizeof(int))
            {
                throw new FormatException("Entrada maior que o buffer destino!");
            }

            for (int i = offset + sizeof(int); i < size + sizeof(int) + offset; i++)
            {
                result += (char)buffer[i];
            }
            return result;
        }

        /// <summary>
        /// Get a int offset From the pack
        /// </summary>
        /// <param name="offset">Index to get the int</param>
        /// <returns>A int from the pack</returns>
        public static int GetInt(byte[] buffer, int offset)
        {
            return BitConverter.ToInt32(buffer, offset);
        }

        /// <summary>
        /// Get a double offset From the pack
        /// </summary>
        /// <param name="offset">Index to get the double</param>
        /// <returns>A double from the pack</returns>
        public static double GetDouble(byte[] buffer, int offset)
        {
            return BitConverter.ToDouble(buffer, offset);
        }

        /// <summary>
        /// Get a float offset From the pack
        /// </summary>
        /// <param name="offset">Index to get the float</param>
        /// <returns>A float from the pack</returns>
        public static float GetFloat(byte[] buffer, int offset)
        {
            return BitConverter.ToSingle(buffer, offset);
        }

        /// <summary>
        /// Get a char offset From the pack
        /// </summary>
        /// <param name="offset">Index to get the char</param>
        /// <returns>A char from the pack</returns>
        public static char GetChar(byte[] buffer, int offset)
        {
            return (char)buffer[offset];
        }

        /// <summary>
        /// Get a byte offset From the pack
        /// </summary>
        /// <param name="offset">Index to get the byte</param>
        /// <returns>A byte from the pack</returns>
        public static byte GetByte(byte[] buffer, int offset)
        {
            return buffer[offset];
        }

        /// <summary>
        /// Get a bool offset From the pack
        /// </summary>
        /// <param name="offset">Index to get the bool</param>
        /// <returns>A bool from the pack</returns>
        public static bool GetBool(byte[] buffer, int offset)
        {
            return buffer[offset] == 1;
        }

        /// <summary>
        /// Writes a string in the writePosition of the pack
        /// </summary>
        /// <param name="value">String to be writed</param>
        /// <param name="offset">Index to put the string</param>
        public static void PutString(byte[] buffer, string value, int offset)
        {
            int i;
            byte[] aux = BitConverter.GetBytes(value.Length);

            for (i = 0; i < sizeof(int); i++)
            {
                buffer[i + offset] = aux[i];
            }

            for (; i < value.Length + sizeof(int); i++)
            {
                buffer[i + offset] = (byte)value[i - sizeof(int)];
            }
        }

        /// <summary>
        /// Writes a int in the writePosition of the pack
        /// </summary>
        /// <param name="value">Int to be writed</param>
        /// <param name="offset">Index to put the int</param>
        public static void PutInt(byte[] buffer, int value, int offset)
        {
            int i;
            byte[] aux = BitConverter.GetBytes(value);

            for (i = 0; i < aux.Length; i++)
            {
                buffer[i + offset] = aux[i];
            }
        }

        /// <summary>
        /// Writes a double in the writePosition of the pack
        /// </summary>
        /// <param name="value">Double to be writed</param>
        /// <param name="offset">Index to put the double</param>
        public static void PutDouble(byte[] buffer, double value, int offset)
        {
            int i;
            byte[] aux = BitConverter.GetBytes(value);

            for (i = 0; i < aux.Length; i++)
            {
                buffer[i + offset] = aux[i];
            }
        }

        /// <summary>
        /// Writes a float in the writePosition of the pack
        /// </summary>
        /// <param name="value">Float to be writed</param>
        /// <param name="offset">Index to put the float</param>
        public static void PutFloat(byte[] buffer, float value, int offset)
        {
            int i;
            byte[] aux = BitConverter.GetBytes(value);

            for (i = 0; i < aux.Length; i++)
            {
                buffer[i + offset] = aux[i];
            }
        }

        /// <summary>
        /// Writes a char in the writePosition of the pack
        /// </summary>
        /// <param name="value">Char to be writed</param>
        /// <param name="offset">Index to put the char</param>
        public static void PutChar(byte[] buffer, char value, int offset)
        {
            buffer[offset] = (byte)value;
        }

        /// <summary>
        /// Writes a byte in the writePosition of the pack
        /// </summary>
        /// <param name="value">Byte to be writed</param>
        /// <param name="offset">Index to put the byte</param>
        public static void PutByte(byte[] buffer, byte value, int offset)
        {
            buffer[offset] = value;
        }

        /// <summary>
        /// Writes a bool in the writePosition of the pack
        /// </summary>
        /// <param name="value">Bool to be writed</param>
        /// <param name="offset">Index to put the bool</param>
        public static void PutBool(byte[] buffer, bool value, int offset)
        {
            buffer[offset] = (value) ? (byte)1 : (byte)0;
        }

        #endregion
    }
}