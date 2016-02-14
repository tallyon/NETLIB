namespace NETLIB
{
    /// <summary>
    /// Interface which describes the methods for packing and unpacking the calling Class in a package
    /// called BasePack. 
    /// Use when you want to send/receive the class on the network.
    /// This interface describe the necessary methods that allow the class insert itself in a BasePack.
    /// Used by BasePack to insert a custom class in the buffer
    /// </summary>
    public interface IPackable
    {
        /// <summary>
        /// The class must implement this method in order to fill a BasePack, inserting its fields
        /// and other desired information on the package. The fiels and information MUST BE in the same orther 
        /// than they will be unpacked.
        /// The BasePack will be sent trhough the network.
        /// 
        /// Insert the desired fields in the BasePack
        /// </summary>
        /// <param name="pack">Pack to put the fields</param>
        /// <example>
        /// public class Test : IPackable
        /// {
        ///     private int i;
        ///     private float j;
        ///     private double k;
        ///     private string str;    
        /// 
        ///     public Pack(BasePack pack)
        ///     {
        ///         pack.PutInt(i);
        ///         pack.PutFloat(j);
        ///         pack.PutDouble(k);
        ///         pack.PutString(str);
        ///     }
        /// 
        ///     public Unpack(BasePack pack)
        ///     {
        ///         this.i = pack.GetInt();
        ///         this.j = pack.GetFloat();
        ///         this.k = pack.GetDouble();
        ///         this.str = pack.GetString();
        ///     }
        /// }
        /// </example>
        void Pack(BasePack pack);

        /// <summary>
        /// The class must implement this method in order to extract fields
        /// and other desired information from a BasePack. The fiels and information MUST BE in the same orther 
        /// than they were packed.
        /// </summary>
        /// <param name="pack">Pack that contains your fields</param>
        /// <example>
        /// public class Test : IPackable
        /// {
        ///     private int i;
        ///     private float j;
        ///     private double k;
        ///     private string str;    
        /// 
        ///     public Pack(BasePack pack)
        ///     {
        ///         pack.PutInt(i);
        ///         pack.PutFloat(j);
        ///         pack.PutDouble(k);
        ///         pack.PutString(str);
        ///     }
        /// 
        ///     public Unpack(BasePack pack)
        ///     {
        ///         this.i = pack.GetInt();
        ///         this.j = pack.GetFloat();
        ///         this.k = pack.GetDouble();
        ///         this.str = pack.GetString();
        ///     }
        /// }
        /// </example>
        void Unpack(BasePack pack);
    }
}