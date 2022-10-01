using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Games.Calendars
{
    public class CalendarManager
    {
        private Dictionary<int, CalendarItem> _calendarItem;
        private int _currentDay;
        private int _campaignDays;

        public CalendarManager()
        {
        }

        public void Init(IQueryAdapter dbClient)
        {
            this._calendarItem = new Dictionary<int, CalendarItem>();
            this._currentDay = 0;
            this._campaignDays = 0;
        }

        public Dictionary<int, CalendarItem> CalendarItem()
        {
            return this._calendarItem;
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
