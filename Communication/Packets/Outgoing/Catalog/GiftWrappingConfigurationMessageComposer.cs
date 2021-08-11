namespace Butterfly.Communication.Packets.Outgoing.Catalog
{
    internal class GiftWrappingConfigurationMessageComposer : ServerPacket
    {
        public GiftWrappingConfigurationMessageComposer()
            : base(ServerPacketHeader.GIFT_CONFIG)
        {

        }
    }
}
