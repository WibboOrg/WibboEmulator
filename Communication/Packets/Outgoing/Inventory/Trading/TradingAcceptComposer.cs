namespace Butterfly.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingAcceptComposer : ServerPacket
    {
        public TradingAcceptComposer()
            : base(ServerPacketHeader.TRADE_ACCEPTED)
        {

        }
    }
}
