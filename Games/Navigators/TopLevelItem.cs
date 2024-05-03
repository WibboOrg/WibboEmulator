namespace WibboEmulator.Games.Navigators;

public class TopLevelItem(int id, string searchCode, string filter, string localization)
{
    public int Id { get; private set; } = id;
    public string SearchCode { get; private set; } = searchCode;
    public string Filter { get; private set; } = filter;
    public string Localization { get; private set; } = localization;
}
