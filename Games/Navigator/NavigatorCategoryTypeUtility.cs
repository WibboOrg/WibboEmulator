namespace WibboEmulator.Games.Navigator;

public static class NavigatorCategoryTypeUtility
{
    public static NavigatorCategoryType GetCategoryTypeByString(string categoryType) => categoryType.ToLower() switch
    {
        "featured" => NavigatorCategoryType.FEATURED,
        "featured_new" => NavigatorCategoryType.FEATURED_NEW,
        "featured_help_security" => NavigatorCategoryType.FEATURED_HELP_SECURITY,
        "featured_run" => NavigatorCategoryType.FEATURED_RUN,
        "featured_game" => NavigatorCategoryType.FEATURED_GAME,
        "featured_casino" => NavigatorCategoryType.FEATURED_CASINO,
        "popular" => NavigatorCategoryType.POPULAR,
        "recommended" => NavigatorCategoryType.RECOMMENDED,
        "query" => NavigatorCategoryType.QUERY,
        "my_rooms" => NavigatorCategoryType.MY_ROOMS,
        "my_favorites" => NavigatorCategoryType.MY_FAVORITES,
        "my_groups" => NavigatorCategoryType.MY_GROUPS,
        "my_history" => NavigatorCategoryType.MY_HISTORY,
        "my_friends_room" => NavigatorCategoryType.MY_FRIENDS_ROOMS,
        "my_history_freq" => NavigatorCategoryType.MY_HISTORY_FREQ,
        "top_promotions" => NavigatorCategoryType.TOP_PROMOTIONS,
        "promotion_category" => NavigatorCategoryType.PROMOTION_CATEGORY,
        "my_rights" => NavigatorCategoryType.MY_RIGHTS,
        _ => NavigatorCategoryType.CATEGORY,
    };
}
