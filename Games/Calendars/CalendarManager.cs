namespace WibboEmulator.Games.Calendars;

public static class CalendarManager
{
    private static Dictionary<int, CalendarItem> _calendarItem;
    private static int _currentDay;
    private static int _campaignDays;

    public static void Initialize()
    {
        _calendarItem = [];
        _currentDay = 0;
        _campaignDays = 0;
    }

    public static Dictionary<int, CalendarItem> CalendarItem() => _calendarItem;

    public static int CurrentDay() => _currentDay;

    public static int CampaignDays() => _campaignDays;
}
