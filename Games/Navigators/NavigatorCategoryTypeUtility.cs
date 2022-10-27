namespace WibboEmulator.Games.Navigators;

public static class NavigatorCategoryTypeUtility
{
    public static NavigatorCategoryType GetCategoryTypeByString(string categoryType) => categoryType.ToLower() switch
    {
        "featured" => NavigatorCategoryType.Featured,
        "featured_new" => NavigatorCategoryType.FeaturedNovelty,
        "featured_help_security" => NavigatorCategoryType.FeaturedHelpSecurity,
        "featured_run" => NavigatorCategoryType.FeaturedRun,
        "featured_game" => NavigatorCategoryType.FeaturedGame,
        "featured_casino" => NavigatorCategoryType.FeaturedCasino,
        "popular" => NavigatorCategoryType.Popular,
        "recommended" => NavigatorCategoryType.Recommended,
        "query" => NavigatorCategoryType.Query,
        "my_rooms" => NavigatorCategoryType.MyRooms,
        "my_favorites" => NavigatorCategoryType.MyFavorites,
        "my_groups" => NavigatorCategoryType.MyGroups,
        "my_history" => NavigatorCategoryType.MyHistory,
        "my_friends_room" => NavigatorCategoryType.MyFriendsRooms,
        "my_history_freq" => NavigatorCategoryType.MyHistoryFreq,
        "top_promotions" => NavigatorCategoryType.TopPromotions,
        "promotion_category" => NavigatorCategoryType.PromotionCategory,
        "my_rights" => NavigatorCategoryType.MyRights,
        _ => NavigatorCategoryType.Category,
    };
}
