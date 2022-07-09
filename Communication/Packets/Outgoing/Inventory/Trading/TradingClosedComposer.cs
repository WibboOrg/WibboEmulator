namespace Wibbo.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingClosedComposer : ServerPacket
    {
        public TradingClosedComposer(int userId)
            : base(ServerPacketHeader.TRADE_CLOSED)
        {
            this.WriteInteger(userId);
            this.WriteInteger(2);
        }
    }
}
