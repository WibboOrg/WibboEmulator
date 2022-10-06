namespace WibboEmulator.Games.Navigator;

public static class NavigatorViewModeUtility
{
    public static NavigatorViewMode GetViewModeByString(string viewMode) => viewMode.ToUpper() switch
    {
        "THUMBNAIL" => NavigatorViewMode.THUMBNAIL,
        _ => NavigatorViewMode.REGULAR,
    };
}
