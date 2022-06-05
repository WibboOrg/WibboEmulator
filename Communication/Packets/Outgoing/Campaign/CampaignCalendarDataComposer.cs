namespace Wibbo.Communication.Packets.Outgoing.Campaign
{
    internal class CampaignCalendarDataComposer : ServerPacket
    {
        public CampaignCalendarDataComposer(string campaignName, string campaignImage, int currentDay, int campaignDays, List<int> openedDays, List<int> missedDays)
            : base(ServerPacketHeader.CAMPAIGN_CALENDAR_DATA)
        {
            this.WriteString(campaignName);
            this.WriteString(campaignImage);
            this.WriteInteger(currentDay);
            this.WriteInteger(campaignDays);

            this.WriteInteger(openedDays.Count);
            foreach(int openedDay in openedDays)
                this.WriteInteger(openedDay);

            this.WriteInteger(missedDays.Count);
            foreach(int missedDay in missedDays)
                this.WriteInteger(missedDay);
        }
    }
}