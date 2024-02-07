namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;

internal sealed class GiftWrappingConfigurationComposer : ServerPacket
{
    public GiftWrappingConfigurationComposer()
        : base(ServerPacketHeader.GIFT_WRAPPER_CONFIG)
    {
        this.WriteBoolean(true); // isEnabled
        this.WriteInteger(1); // price

        this.WriteInteger(9); // total giftWrappers
        for (var i = 3372; i < 3381; i++)
        {
            this.WriteInteger(i);
        }

        this.WriteInteger(7); // boxTypes total
        for (var i = 0; i < 9; i++)
        {
            if (i is not 7 or 8)
            {
                this.WriteInteger(i);
            }
        }

        this.WriteInteger(11); // ribbonTypes total
        for (var i = 0; i < 11; i++)
        {
            this.WriteInteger(i);
        }

        this.WriteInteger(7); // giftFurnis total
        for (var i = 187; i < 194; i++)
        {
            this.WriteInteger(i);
        }
    }
}
