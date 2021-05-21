using System;

namespace SharedPacketLib
{
    public interface IDataParser : IDisposable, ICloneable
    {
        void HandlePacketData(byte[] packet, bool deciphered = false);
    }
}
