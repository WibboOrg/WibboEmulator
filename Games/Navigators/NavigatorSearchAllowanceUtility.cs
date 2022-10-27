namespace WibboEmulator.Games.Navigators;

public static class NavigatorSearchAllowanceUtility
{
    public static NavigatorSearchAllowance GetSearchAllowanceByString(string categoryType) => categoryType.ToUpper() switch
    {
        "SHOW_MORE" => NavigatorSearchAllowance.ShowMore,
        "GO_BACK" => NavigatorSearchAllowance.GoBack,
        _ => NavigatorSearchAllowance.Nothing,
    };
}
