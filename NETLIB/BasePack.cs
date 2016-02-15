using System;

namespace NETLIB
{
    /// <summary>
    /// Is the basic unit of the network communication, in other words all the information that travels over 
    /// the network is converted in BasePack before transmission and is subsequently
    /// reassembled by the receiver. It simplifies operations with the network buffer and handle reading and writing.
    /// </summary>
    /// <example>
    /// This example shows how to create a new pack, put and get some fields from there. 
    /// <code>
    /// void Example()
    /// {
    ///     int d1 = 5;
    ///     int d2 = 6;
    ///     
    ///     int c1;
    ///     int c2;
    ///     
    ///     BasePack newPack = new BasePack();
    ///     newPack.ID = 10;
    ///     newPack.PutInt(d1);
    ///     newPack.PutInt(d2);
    ///     
    ///     c1 = newPack.GetInt();
    ///     c2 = newPack.GetInt();
    /// }
    /// </code>
    /// </example>
    public class BasePack
    {
        #region Variables

        /// <summary>
        /// Represent the maximum size of the network buffer. The pack buffer will never be bigger than this.
        /// <para>Used by <see cref="Publisher"/> to receive the network buffer</para>
        /// </summary>
        /// <seealso cref="Publisher"/>
        /// <seealso cref="TCP.TCPPublisher"/>
        public static int packSize = 1500;

        /// <summary>
        /// Hold the pack information
        /// <para>Used by <see cref="Publisher"/> to send the information</para>
        /// </summary>
        protected byte[] buffer;

        /// <summary>
        /// Stores the index to be used in the buffer for the next read
        /// </summary>
        /// The read position is initialized to 1 because the first byte is used as a packet <see cref="ID"/>
        /// <seealso cref="ID"/>
        /// <seealso cref="IOPackHandler{TPack}.OnReceivedPackCall(TPack)"/>
        /// <seealso cref="IOPackHandler{TPack}.triggers"/>
        protected int readPosition = 1;

        /// <summary>
        /// Stores the index to be used in the buffer for the next write
        /// </summary>
        /// The write position is initialized to 1 because the first byte is used as a packet <see cref="ID"/>
        /// <seealso cref="IOPackHandler{TPack}.OnReceivedPackCall(TPack)"/>
        /// <seealso cref="IOPackHandler{TPack}.triggers"/>
        protected int writePosition = 1;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the inner buffer with packSize
        /// </summary>
        /// <seealso cref="packSize"/>
        public BasePack()
        {
            this.buffer = new byte[packSize];
        }

        /// <summary>
        /// Takes the <paramref name="basePack"/> inner buffer as its own inner beffer.
        /// The <see cref="readPosition"/> and the <see cref="writePosition"/> are not copied
        /// </summary>
        /// <param name="basePack">BasePack that will be copied</param>
        protected BasePack(BasePack basePack)
        {
            this.buffer = basePack.buffer;
        }

        /// <summary>
        /// Initialize the BasePack taking <paramref name="buffer"/> as your own inner buffer
        /// </summary>
        /// <param name="buffer">Source buffer</param>
        protected BasePack(byte[] buffer)
        {
            if (buffer.Length <= packSize)
            {
                this.buffer = buffer;
            }
            else
            {
                throw new ArgumentOutOfRangeException("buffer");
            }
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Used by <see cref="IOPackHandler{TPack}"/> to classify and redirect incoming packs to the proper handle function.
        /// <para>Refers to the first byte of the buffer</para>
        /// </summary>
        /// <remarks>As with any application protocol it is required something to identify the kind of the package, thus,
        /// I set that the first byte in the buffer will be used for that purpose</remarks>
        /// <seealso cref="IOPackHandler{TPack}.OnReceivedPackCall(TPack)"/>
        /// <seealso cref="IOPackHandler{TPack}.triggers"/>
        public virtual byte ID
        {
            get { return buffer[0]; }

            set { buffer[0] = value; }
        }

        /// <summary>
        /// Make the buffer's data public but deny the exchange of buffer reference.
        /// </summary>
        /// <param name="index">Index of the byte to be read</param>
        /// <returns>A byte of the byffer indexed by index</returns>
        /// <exception cref = "ArgumentOutOfRangeException">
        ///     When the index is larger than <see cref="buffer"/>
        /// </exception> 
        public virtual byte this[int index]
        {
            get { return buffer[index]; }

            set { buffer[index] = value; }
        }

        /// <summary>
        /// Length of the inner buffer
        /// </summary>
        public virtual int Length
        {
            get { return buffer.Length; }
        }

        /// <summary>
        ///     Returns the inner buffer but deny the exchange of buffer reference
        /// </summary>
        public virtual byte[] Buffer
        {
            get { return buffer; }
        }

        /// <summary>
        /// Gets and sets the readPosition
        /// </summary>
        public virtual int ReadPosition
        {
            get { return readPosition; }
            set { readPosition = value; }
        }

        /// <summary>
        /// Gets and sets the writePosition
        /// </summary>
        public virtual int WritePosition
        {
            get { return writePosition; }
            set { writePosition = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Copies a sequence of bytes to the pack and advances the current <see cref="writePosition"/> by the number of bytes copied.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the pack.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the pack.</param>
        /// <param name="count">The number of bytes to be copied.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="writePosition"/> plus <paramref name="count"/> is larger or equal than the inner buffer length.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when the <paramref name="offset"/> is larger or equal than the buffer length 
        /// and when <paramref name="offset"/> plus <paramref name="count"/> is larger than the buffer size.
        /// </exception>
        public virtual void Write(byte[] buffer, int offset, int count)
        {
            if (writePosition + count >= this.buffer.Length)
            {
                throw new IndexOutOfRangeException("Write position larger than buffer length.");
            }

            if (offset >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (offset + count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            for (int i = 0; i < count; i++)
            {
                this.buffer[writePosition] = buffer[offset + i];
                writePosition++;
            }
        }

        /// <summary>
        /// Copies a sequence of bytes from the pack and advances the current <see cref="readPosition"/> by the number of bytes copied.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from pack to the buffer.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the pack.</param>
        /// <param name="count">The number of bytes to be copied.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="readPosition"/> plus <paramref name="count"/> is larger or equal than the inner buffer length.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when the <paramref name="offset"/> is larger or equal than the buffer length
        /// and when <paramref name="offset"/> plus <paramref name="count"/> is larger than the buffer size.
        /// </exception>
        public virtual void Read(byte[] buffer, int offset, int count)
        {
            if (readPosition + count >= this.buffer.Length)
            {
                throw new IndexOutOfRangeException("Read position larger than buffer length.");
            }

            if (offset >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (offset + count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            for (int i = 0; i < count; i++)
            {
                buffer[offset + i] = this.buffer[readPosition];
                readPosition++;
            }
        }

        /// <summary>
        /// Converts a part of the inner buffer(started in <see cref="readPosition"/>) in a string.
        /// <para>
        /// First it reads an integer from the buffer, that refers to the lenght of the string.
        /// Then reads the chars one by one from the buffer and put them in a string.
        /// </para>
        /// </summary>
        /// <returns>String converted from the inner buffer started in <see cref="readPosition"/>.</returns>
        /// <exception cref="FormatException">
        /// Throws when the length of the string read from the inner buffer is 
        /// bigger than the remaining bytes in the inner buffer
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="readPosition"/> is larger or equal than the inner buffer length minus sizeof(int).
        /// </exception>
        public virtual string GetString()
        {
            if (readPosition >= this.buffer.Length - sizeof(int))
            {
                throw new IndexOutOfRangeException("Read position larger than buffer length.");
            }

            string result = string.Empty;
            int size = BitConverter.ToInt32(buffer, readPosition);
            readPosition += sizeof(int);

            if (size + readPosition >= this.buffer.Length)
            {
                throw new FormatException("The read length is larger than the remaining bytes!");
            }

            for (int i = readPosition; i < size + readPosition; i++)
            {
                result += (char)this.buffer[i];
            }
            readPosition += size + sizeof(int);
            return result;
        }

        /// <summary>
        /// Converts a part of the inner buffer(started in <paramref name="offset"/>) in a string.
        /// <para>
        /// First it reads an integer from the buffer, that refers to the lenght of the string.
        /// Then reads the chars one by one from the buffer and put them in a string.
        /// </para>
        /// </summary>
        /// <param name="offset">Starting position for the string conversion.</param>
        /// <returns>String converted from the inner buffer started in <paramref name="offset"/>.</returns>
        /// <exception cref="FormatException">
        /// Throws when the length of the string read from the inner buffer is 
        /// less than the remaining bytes in the inner buffer.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when <paramref name="offset"/> is larger or equal than the inner buffer length minus sizeof(int).
        /// </exception>
        protected virtual string GetString(int offset)
        {
            if (offset >= this.buffer.Length)
            {
                throw new IndexOutOfRangeException("Offset larger than buffer length.");
            }

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
        /// Converts a part of the inner buffer(started in <see cref="readPosition"/>) in a int.
        /// </summary>
        /// <returns>Int converted from the inner buffer started in <see cref="readPosition"/>.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="readPosition"/> is larger or equal than the inner buffer length minus sizeof(int).
        /// </exception>
        public virtual int GetInt()
        {
            if (readPosition >= this.buffer.Length - sizeof(int))
            {
                throw new IndexOutOfRangeException("Read position larger than buffer length.");
            }

            int ret = BitConverter.ToInt32(buffer, readPosition);
            readPosition += sizeof(int);
            return ret;
        }

        /// <summary>
        /// Converts a part of the inner buffer(started in <paramref name="offset"/>) in a int.
        /// </summary>
        /// <returns>Int converted from the inner buffer started in <paramref name="offset"/>.</returns>
        /// <param name="offset">Starting position for the int conversion.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="readPosition"/> is larger or equal than the inner buffer length minus sizeof(int).
        /// </exception>
        protected virtual int GetInt(int offset)
        {
            if (offset >= this.buffer.Length - sizeof(int))
            {
                throw new IndexOutOfRangeException("Offset larger than buffer length.");
            }

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
            byte returnedByte = buffer[readPosition];
            readPosition++;
            return returnedByte;
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
            CustomType returnedCustomType = new CustomType();
            returnedCustomType.Unpack(this);
            return returnedCustomType;
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
            BasePack returnedBasePack = new BasePack();

            for (int i = 0; i < packSize; i++)
            {
                returnedBasePack[i] = this[i];
            }
            return returnedBasePack;
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

        #region Methods estáticos

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