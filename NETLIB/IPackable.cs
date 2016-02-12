namespace NETLIB
{
    /// <summary>
    /// Use when you want to send/receive the class on the network.
    /// This interface describe the necessary methods that allow the class insert itself in a BasePack.
    /// Used by BasePack to insert a custom class in the buffer
    /// </summary>
    public interface IPackable
    {
        /// <summary>
        /// Insert the desired fields in the BasePack
        /// </summary>
        /// <param name="pack">Pack to put the fields</param>
        void Pack(BasePack pack);

        /// <summary>
        /// Get the disired fields from the BasePack
        /// </summary>
        /// <param name="pack">Pack that contains your fields</param>
        void Unpack(BasePack pack);
    }
}