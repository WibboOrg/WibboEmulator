namespace WibboEmulator.Games.Navigators;
using System.Data;
using WibboEmulator.Core.Language;
using WibboEmulator.Database.Daos.Navigator;

public sealed class NavigatorManager
{
    private readonly Dictionary<int, FeaturedRoom> _featuredRooms;
    private readonly Dictionary<int, TopLevelItem> _topLevelItems;
    private readonly Dictionary<int, SearchResultList> _searchResultLists;

    public NavigatorManager()
    {
        this._topLevelItems = new Dictionary<int, TopLevelItem>();
        this._searchResultLists = new Dictionary<int, SearchResultList>();

        this._topLevelItems.Add(1, new TopLevelItem(1, "official_view", "", ""));
        this._topLevelItems.Add(2, new TopLevelItem(2, "hotel_view", "", ""));
        this._topLevelItems.Add(3, new TopLevelItem(3, "rooms_game", "", ""));
        this._topLevelItems.Add(4, new TopLevelItem(4, "myworld_view", "", ""));

        this._featuredRooms = new Dictionary<int, FeaturedRoom>();
    }

    public void Initialize(IDbConnection dbClient)
    {
        if (this._searchResultLists.Count > 0)
        {
            this._searchResultLists.Clear();
        }

        if (this._featuredRooms.Count > 0)
        {
            this._featuredRooms.Clear();
        }

        var categoryList = NavigatorCategoryDao.GetAll(dbClient);

        if (categoryList.Count != 0)
        {
            foreach (var category in categoryList)
            {
                if (!this._searchResultLists.ContainsKey(category.Id))
                {
                    this._searchResultLists.Add(category.Id, new SearchResultList(category.Id, category.Category, category.CategoryIdentifier, category.PublicName, true, -1, category.RequiredRank, category.Minimized, NavigatorViewModeUtility.GetViewModeByString(category.ViewMode), category.CategoryType, category.SearchAllowance, category.OrderId));
                }
            }
        }

        var navPublicList = NavigatorPublicDao.GetAll(dbClient);

        if (navPublicList.Count != 0)
        {
            foreach (var navPublic in navPublicList)
            {
                if (!this._featuredRooms.ContainsKey(navPublic.RoomId))
                {
                    this._featuredRooms.Add(navPublic.RoomId, new FeaturedRoom(navPublic.RoomId, navPublic.ImageUrl, LanguageManager.ParseLanguage(navPublic.Langue), navPublic.CategoryType));
                }
            }
        }
    }

    public List<SearchResultList> GetCategorysForSearch(string category)
    {
        var categorys =
            from Cat in this._searchResultLists
            where Cat.Value.Category == category
            orderby Cat.Value.OrderId ascending
            select Cat.Value;
        return categorys.ToList();
    }

    public ICollection<SearchResultList> GetResultByIdentifier(string category)
    {
        var categorys =
            from Cat in this._searchResultLists
            where Cat.Value.CategoryIdentifier == category
            orderby Cat.Value.OrderId ascending
            select Cat.Value;
        return categorys.ToList();
    }

    public ICollection<SearchResultList> GetFlatCategories()
    {
        var categorys =
            from Cat in this._searchResultLists
            where Cat.Value.CategoryType == NavigatorCategoryType.Category
            orderby Cat.Value.OrderId ascending
            select Cat.Value;
        return categorys.ToList();
    }

    public ICollection<SearchResultList> GetEventCategories()
    {
        var categorys =
            from Cat in this._searchResultLists
            where Cat.Value.CategoryType == NavigatorCategoryType.PromotionCategory
            orderby Cat.Value.OrderId ascending
            select Cat.Value;
        return categorys.ToList();
    }

    public ICollection<TopLevelItem> GetTopLevelItems() => this._topLevelItems.Values;

    public ICollection<SearchResultList> GetSearchResultLists() => this._searchResultLists.Values;

    public bool TryGetTopLevelItem(int id, out TopLevelItem topLevelItem) => this._topLevelItems.TryGetValue(id, out topLevelItem);

    public bool TryGetSearchResultList(int id, out SearchResultList searchResultList) => this._searchResultLists.TryGetValue(id, out searchResultList);

    public bool TryGetFeaturedRoom(int roomId, out FeaturedRoom publicRoom) => this._featuredRooms.TryGetValue(roomId, out publicRoom);

    public ICollection<FeaturedRoom> GetFeaturedRooms(Language langue) => this._featuredRooms.Values.Where(f => f.Langue == langue).ToList();
}
