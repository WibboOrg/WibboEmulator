using Wibbo.Database.Interfaces;

namespace Wibbo.Game.Badges
{
    public class CalendarManager
    {
        private string _campaignName;
        private string _campaignImage;
        private int _currentDay;
        private int _campaignDays;

        public CalendarManager()
        {
        }

        public void Init(IQueryAdapter dbClient)
        {
            this._campaignName = "";
            this._campaignImage = "";
            this._currentDay = 0;
            this._campaignDays = 0;
        }

        public string CampaignName()
        {
            return this._campaignName;
        }

        public string CampaignImage()
        {
            return this._campaignImage;
        }

        public int CurrentDay()
        {
            return this._currentDay;
        }

        public int CampaignDays()
        {
            return this._campaignDays;
        }
    }
}
