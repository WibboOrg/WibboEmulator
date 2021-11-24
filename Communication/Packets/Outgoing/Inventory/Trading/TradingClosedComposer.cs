namespace Butterfly.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingClosedComposer : ServerPacket
    {
        public TradingClosedComposer()
            : base(ServerPacketHeader.TRADE_CLOSED)
        {

        }
    }
}
