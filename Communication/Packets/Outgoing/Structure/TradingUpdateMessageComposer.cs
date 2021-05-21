namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class TradingUpdateMessageComposer : ServerPacket
    {
        public TradingUpdateMessageComposer()
            : base(ServerPacketHeader.TRADE_UPDATE)
        {

        }
    }
}
