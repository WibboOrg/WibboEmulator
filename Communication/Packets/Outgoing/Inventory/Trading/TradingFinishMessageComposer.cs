namespace Butterfly.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingFinishMessageComposer : ServerPacket
    {
        public TradingFinishMessageComposer()
            : base(ServerPacketHeader.TRADE_CLOSE)
        {

        }
    }
}
