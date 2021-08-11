namespace Butterfly.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingAcceptMessageComposer : ServerPacket
    {
        public TradingAcceptMessageComposer()
            : base(ServerPacketHeader.TRADE_ACCEPTED)
        {

        }
    }
}
