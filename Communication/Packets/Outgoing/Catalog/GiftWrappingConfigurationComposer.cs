namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;

internal sealed class GiftWrappingConfigurationComposer : ServerPacket
{
    public GiftWrappingConfigurationComposer()
        : base(ServerPacketHeader.GIFT_WRAPPER_CONFIG)
    {
        this.WriteBoolean(true);
        this.WriteInteger(1);

        this.WriteInteger(9);
        for (var i = 3372; i < 3381; ++i)
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
        for (var i = 187; i < 194; ++i)
        {
            this.WriteInteger(i);
        }
    }
}
