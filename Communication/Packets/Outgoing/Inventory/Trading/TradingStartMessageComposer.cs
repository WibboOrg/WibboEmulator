namespace Butterfly.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingStartMessageComposer : ServerPacket
    {
        public TradingStartMessageComposer()
            : base(ServerPacketHeader.TRADE)
        {

        }
    }
}
