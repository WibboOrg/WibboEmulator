namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class TradingClosedMessageComposer : ServerPacket
    {
        public TradingClosedMessageComposer()
            : base(ServerPacketHeader.TRADE_CLOSED)
        {

        }
    }
}
