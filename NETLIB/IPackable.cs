namespace NETLIB
{
    /// <summary>
    /// Makes a class packable and unpackable
    /// </summary>
    public interface IPackable
    {
        void Pack(BasePack pack);
        void Unpack(BasePack pack);
    }
}