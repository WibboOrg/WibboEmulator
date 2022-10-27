namespace WibboEmulator.Games.Navigators;

public static class NavigatorViewModeUtility
{
    public static NavigatorViewMode GetViewModeByString(string viewMode) => viewMode.ToUpper() switch
    {
        "THUMBNAIL" => NavigatorViewMode.Thumbnail,
        _ => NavigatorViewMode.Regular,
    };
}
