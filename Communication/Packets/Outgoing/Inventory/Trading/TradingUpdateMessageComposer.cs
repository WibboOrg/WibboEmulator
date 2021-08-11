namespace Butterfly.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingUpdateMessageComposer : ServerPacket
    {
        public TradingUpdateMessageComposer()
            : base(ServerPacketHeader.TRADE_UPDATE)
        {

        }
    }
}
