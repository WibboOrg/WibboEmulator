namespace WibboEmulator.Games.Navigator;

public static class NavigatorSearchAllowanceUtility
{
    public static NavigatorSearchAllowance GetSearchAllowanceByString(string categoryType) => categoryType.ToUpper() switch
    {
        "SHOW_MORE" => NavigatorSearchAllowance.SHOW_MORE,
        "GO_BACK" => NavigatorSearchAllowance.GO_BACK,
        _ => NavigatorSearchAllowance.NOTHING,
    };

    public static int GetIntegerValue(NavigatorSearchAllowance searchAllowance) => searchAllowance switch
    {
        NavigatorSearchAllowance.SHOW_MORE => 1,
        NavigatorSearchAllowance.GO_BACK => 2,
        _ => 0,
    };
}
