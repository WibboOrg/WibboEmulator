namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Trading;

internal sealed class TradingCompleteComposer : ServerPacket
{
    public TradingCompleteComposer()
        : base(ServerPacketHeader.TRADE_CONFIRMATION)
    {

    }
}
