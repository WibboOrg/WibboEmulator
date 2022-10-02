namespace WibboEmulator.Games.Calendars
{
    public class CalendarItem
    {
        private readonly string _campaignName;
        private readonly string _campaignImage;

        public CalendarItem(string campaignName, string campaignImage)
        {
            this._campaignName = campaignName;
            this._campaignImage = campaignImage;
        }

        public string CampaignName()
        {
            return this._campaignName;
        }

        public string CampaignImage()
        {
            return this._campaignImage;
        }
    }
}
