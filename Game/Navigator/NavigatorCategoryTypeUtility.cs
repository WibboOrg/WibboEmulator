namespace Butterfly.Game.Navigator
{
    public static class NavigatorCategoryTypeUtility
    {
        public static NavigatorCategoryType GetCategoryTypeByString(string CategoryType)
        {
            switch (CategoryType.ToLower())
            {
                default:
                case "category":
                    return NavigatorCategoryType.CATEGORY;
                case "featured":
                    return NavigatorCategoryType.FEATURED;
                case "featured_new":
                    return NavigatorCategoryType.FEATURED_NEW;
                case "featured_help_security":
                    return NavigatorCategoryType.FEATURED_HELP_SECURITY;
                case "featured_RUN":
                    return NavigatorCategoryType.FEATURED_RUN;
                case "featured_game":
                    return NavigatorCategoryType.FEATURED_GAME;
                case "popular":
                    return NavigatorCategoryType.POPULAR;
                case "recommended":
                    return NavigatorCategoryType.RECOMMENDED;
                case "query":
                    return NavigatorCategoryType.QUERY;
                case "my_rooms":
                    return NavigatorCategoryType.MY_ROOMS;
                case "my_favorites":
                    return NavigatorCategoryType.MY_FAVORITES;
                case "my_groups":
                    return NavigatorCategoryType.MY_GROUPS;
                case "my_history":
                    return NavigatorCategoryType.MY_HISTORY;
                case "my_friends_room":
                    return NavigatorCategoryType.MY_FRIENDS_ROOMS;
                case "my_history_freq":
                    return NavigatorCategoryType.MY_HISTORY_FREQ;
                case "top_promotions":
                    return NavigatorCategoryType.TOP_PROMOTIONS;
                case "promotion_category":
                    return NavigatorCategoryType.PROMOTION_CATEGORY;
                case "my_rights":
                    return NavigatorCategoryType.MY_RIGHTS;
            }
        }
    }
}
