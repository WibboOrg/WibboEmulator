namespace Butterfly.Communication.Packets.Outgoing.Catalog
{
    internal class GiftWrappingConfigurationMessageComposer : ServerPacket
    {
        public GiftWrappingConfigurationMessageComposer()
            : base(ServerPacketHeader.GIFT_CONFIG)
        {
            this.WriteBoolean(true);
            this.WriteInteger(1);

            this.WriteInteger(9);
            for (int i = 3372; i < 3381; ++i)
            {
                this.WriteInteger(i);
            }

            this.WriteInteger(8);
            this.WriteInteger(0);
            this.WriteInteger(1);
            this.WriteInteger(2);
            this.WriteInteger(3);
            this.WriteInteger(4);
            this.WriteInteger(5);
            this.WriteInteger(6);
            this.WriteInteger(8);

            this.WriteInteger(11);
            this.WriteInteger(0);
            this.WriteInteger(1);
            this.WriteInteger(2);
            this.WriteInteger(3);
            this.WriteInteger(4);
            this.WriteInteger(5);
            this.WriteInteger(6);
            this.WriteInteger(7);
            this.WriteInteger(8);
            this.WriteInteger(9);
            this.WriteInteger(10);

            this.WriteInteger(7);
            for (int i = 187; i < 194; ++i)
            {
                this.WriteInteger(i);
            }
        }
    }
}
