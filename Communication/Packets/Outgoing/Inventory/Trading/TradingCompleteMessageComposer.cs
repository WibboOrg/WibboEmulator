namespace Butterfly.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingCompleteMessageComposer : ServerPacket
    {
        public TradingCompleteMessageComposer()
            : base(ServerPacketHeader.TRADE_CONFIRM)
        {

        }
    }
}
