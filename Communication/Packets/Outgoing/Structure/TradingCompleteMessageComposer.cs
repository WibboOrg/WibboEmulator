namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class TradingCompleteMessageComposer : ServerPacket
    {
        public TradingCompleteMessageComposer()
            : base(ServerPacketHeader.TRADE_CONFIRM)
        {

        }
    }
}
