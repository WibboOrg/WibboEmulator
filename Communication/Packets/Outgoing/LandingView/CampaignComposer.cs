namespace WibboEmulator.Communication.Packets.Outgoing.LandingView;

internal sealed class CampaignComposer : ServerPacket
{
    public CampaignComposer(string campaignString, string campaignName)
        : base(ServerPacketHeader.DESKTOP_CAMPAIGN)
    {
        this.WriteString(campaignString);
        this.WriteString(campaignName);
    }
}
