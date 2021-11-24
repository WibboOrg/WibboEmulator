namespace Butterfly.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingStartComposer : ServerPacket
    {
        public TradingStartComposer()
            : base(ServerPacketHeader.TRADE)
        {

        }
    }
}
