namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Trading;

internal class TradingStartComposer : ServerPacket
{
    public TradingStartComposer(int userOneId, int userTwoId)
        : base(ServerPacketHeader.TRADE_OPEN)
    {
        this.WriteInteger(userOneId);
        this.WriteInteger(1);
        this.WriteInteger(userTwoId);
        this.WriteInteger(1);
    }
}
