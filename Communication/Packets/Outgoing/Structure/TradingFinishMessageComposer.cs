namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class TradingFinishMessageComposer : ServerPacket
    {
        public TradingFinishMessageComposer()
            : base(ServerPacketHeader.TRADE_CLOSE)
        {

        }
    }
}
