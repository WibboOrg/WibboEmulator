namespace Butterfly.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingUpdateComposer : ServerPacket
    {
        public TradingUpdateComposer()
            : base(ServerPacketHeader.TRADE_UPDATE)
        {

        }
    }
}
