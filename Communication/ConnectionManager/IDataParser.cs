using System;

namespace Butterfly.Communication.ConnectionManager
{
    public interface IDataParser : IDisposable, ICloneable
    {
        void HandlePacketData(byte[] packet, bool isDecoded = false);
    }
}
