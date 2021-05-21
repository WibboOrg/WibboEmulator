namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class GiftWrappingConfigurationMessageComposer : ServerPacket
    {
        public GiftWrappingConfigurationMessageComposer()
            : base(ServerPacketHeader.GIFT_CONFIG)
        {

        }
    }
}
