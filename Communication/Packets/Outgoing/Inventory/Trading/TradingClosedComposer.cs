namespace Butterfly.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingClosedComposer : ServerPacket
    {
        public TradingClosedComposer(int UserId)
            : base(ServerPacketHeader.TRADE_CLOSED)
        {
            this.WriteInteger(UserId);
            this.WriteInteger(0);
        }
    }
}
