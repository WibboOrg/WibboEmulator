namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Trading;

internal sealed class TradingFinishComposer : ServerPacket
{
    public TradingFinishComposer()
        : base(ServerPacketHeader.TRADE_COMPLETED)
    {

    }
}
