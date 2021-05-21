namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class TradingStartMessageComposer : ServerPacket
    {
        public TradingStartMessageComposer()
            : base(ServerPacketHeader.TRADE)
        {

        }
    }
}
