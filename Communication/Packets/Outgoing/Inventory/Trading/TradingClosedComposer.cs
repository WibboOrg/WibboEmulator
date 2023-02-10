namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Trading;

internal sealed class TradingClosedComposer : ServerPacket
{
    public TradingClosedComposer(int userId)
        : base(ServerPacketHeader.TRADE_CLOSED)
    {
        this.WriteInteger(userId);
        this.WriteInteger(2);
    }
}
