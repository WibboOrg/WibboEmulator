namespace WibboEmulator.Communication.Packets.Outgoing.Campaign;

internal sealed class CampaignCalendarDoorOpenedComposer : ServerPacket
{
    public CampaignCalendarDoorOpenedComposer(bool doorOpened, string productName, string customImage, string furnitureClassName)
        : base(ServerPacketHeader.CAMPAIGN_CALENDAR_DOOR_OPENED)
    {
        this.WriteBoolean(doorOpened);
        this.WriteString(productName);
        this.WriteString(customImage);
        this.WriteString(furnitureClassName);
    }
}