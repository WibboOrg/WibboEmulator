namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Trading;

internal sealed class TradingAcceptComposer : ServerPacket
{
    public TradingAcceptComposer(int userId, int statut)
        : base(ServerPacketHeader.TRADE_ACCEPTED)
    {
        this.WriteInteger(userId);
        this.WriteInteger(statut);
    }
}
