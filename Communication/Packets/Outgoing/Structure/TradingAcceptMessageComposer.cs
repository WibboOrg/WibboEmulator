namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class TradingAcceptMessageComposer : ServerPacket
    {
        public TradingAcceptMessageComposer()
            : base(ServerPacketHeader.TRADE_ACCEPTED)
        {

        }
    }
}
