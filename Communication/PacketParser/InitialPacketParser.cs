using Butterfly.Communication.ConnectionManager;
using System;

namespace Butterfly.Net
{
    public class InitialPacketParser : IDataParser, IDisposable, ICloneable
    {
        public byte[] CurrentData;

        public event InitialPacketParser.NoParamDelegate SwitchParserRequest;

        public void HandlePacketData(byte[] packet, bool isDecoded = false)
        {
            if (this.SwitchParserRequest == null)
            {
                return;
            }

            this.CurrentData = packet;
            this.SwitchParserRequest();
        }

        public void Dispose()
        {
            this.SwitchParserRequest = null;
        }

        public object Clone()
        {
            return new InitialPacketParser();
        }

        public delegate void NoParamDelegate();
    }
}
