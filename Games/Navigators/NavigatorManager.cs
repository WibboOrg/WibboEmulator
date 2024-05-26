namespace WibboEmulator.Games.Navigators;
using System.Data;
using WibboEmulator.Core.Language;
using WibboEmulator.Database.Daos.Navigator;

public static class NavigatorManager
{
    private static readonly Dictionary<int, FeaturedRoom> FeaturedRooms = [];
    private static readonly Dictionary<int, TopLevelItem> TopLevelItemsDefault = [];
    private static readonly Dictionary<int, SearchResultList> SearchResultListsDefault = [];

    public static void Initialize(IDbConnection dbClient)
    {
        FeaturedRooms.Clear();
        TopLevelItemsDefault.Clear();
        SearchResultListsDefault.Clear();

        TopLevelItemsDefault.Add(1, new(1, "official_view", "", ""));
        TopLevelItemsDefault.Add(2, new(2, "hotel_view", "", ""));
        TopLevelItemsDefault.Add(3, new(3, "rooms_game", "", ""));
        TopLevelItemsDefault.Add(4, new(4, "myworld_view", "", ""));

        var categoryList = NavigatorCategoryDao.GetAll(dbClient);

        if (categoryList.Count != 0)
        {
            foreach (var category in categoryList)
            {
                if (!SearchResultListsDefault.ContainsKey(category.Id))
                {
                    SearchResultListsDefault.Add(category.Id, new SearchResultList(category.Id, category.Category, category.CategoryIdentifier, category.PublicName, true, -1, category.RequiredRank, category.Minimized, NavigatorViewModeUtility.GetViewModeByString(category.ViewMode), category.CategoryType, category.SearchAllowance, category.OrderId));
                }
            }
        }

        var navPublicList = NavigatorPublicDao.GetAll(dbClient);

        if (navPublicList.Count != 0)
        {
            foreach (var navPublic in navPublicList)
            {
                if (!FeaturedRooms.ContainsKey(navPublic.RoomId))
                {
                    FeaturedRooms.Add(navPublic.RoomId, new FeaturedRoom(navPublic.RoomId, navPublic.ImageUrl, LanguageManager.ParseLanguage(navPublic.Langue), navPublic.CategoryType));
                }
            }
        }
    }

    public static List<SearchResultList> GetCategorysForSearch(string category)
    {
        var categorys =
            from Cat in SearchResultListsDefault
            where Cat.Value.Category == category
            orderby Cat.Value.OrderId ascending
            select Cat.Value;
        return categorys.ToList();
    }

    public static ICollection<SearchResultList> GetResultByIdentifier(string category)
    {
        var categorys =
            from Cat in SearchResultListsDefault
            where Cat.Value.CategoryIdentifier == category
            orderby Cat.Value.OrderId ascending
            select Cat.Value;
        return categorys.ToList();
    }

    public static ICollection<SearchResultList> FlatCategories
    {
        get
        {
            var categorys =
                from Cat in SearchResultListsDefault
                where Cat.Value.CategoryType == NavigatorCategoryType.Category
                orderby Cat.Value.OrderId ascending
                select Cat.Value;
            return categorys.ToList();
        }
    }

    public static ICollection<SearchResultList> EventCategories
    {
        get
        {
            var categorys =
                from Cat in SearchResultListsDefault
                where Cat.Value.CategoryType == NavigatorCategoryType.PromotionCategory
                orderby Cat.Value.OrderId ascending
                select Cat.Value;
            return categorys.ToList();
        }
    }

    public static ICollection<TopLevelItem> TopLevelItems => TopLevelItemsDefault.Values;

    public static ICollection<SearchResultList> SearchResultLists => SearchResultListsDefault.Values;

    public static bool TryGetTopLevelItem(int id, out TopLevelItem topLevelItem) => TopLevelItemsDefault.TryGetValue(id, out topLevelItem);

    public static bool TryGetSearchResultList(int id, out SearchResultList searchResultList) => SearchResultListsDefault.TryGetValue(id, out searchResultList);

    public static bool TryGetFeaturedRoom(int roomId, out FeaturedRoom publicRoom) => FeaturedRooms.TryGetValue(roomId, out publicRoom);

    public static ICollection<FeaturedRoom> GetFeaturedRooms(Language langue) => FeaturedRooms.Values.Where(f => f.Langue == langue).ToList();
}
