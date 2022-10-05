namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Trading;

internal class TradingStartComposer : ServerPacket
{
    public TradingStartComposer(int UserOneId, int UserTwoId)
        : base(ServerPacketHeader.TRADE_OPEN)
    {
        this.WriteInteger(UserOneId);
        this.WriteInteger(1);
        this.WriteInteger(UserTwoId);
        this.WriteInteger(1);
    }
}
