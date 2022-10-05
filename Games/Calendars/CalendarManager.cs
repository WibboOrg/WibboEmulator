namespace WibboEmulator.Games.Calendars;
using WibboEmulator.Database.Interfaces;

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

    public Dictionary<int, CalendarItem> CalendarItem() => this._calendarItem;

    public int CurrentDay() => this._currentDay;

    public int CampaignDays() => this._campaignDays;
}
