namespace Butterfly.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingClosedMessageComposer : ServerPacket
    {
        public TradingClosedMessageComposer()
            : base(ServerPacketHeader.TRADE_CLOSED)
        {

        }
    }
}
