namespace Wibbo.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingCompleteComposer : ServerPacket
    {
        public TradingCompleteComposer()
            : base(ServerPacketHeader.TRADE_CONFIRMATION)
        {

        }
    }
}
