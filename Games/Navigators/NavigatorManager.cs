namespace WibboEmulator.Games.Navigators;
using System.Data;
using WibboEmulator.Core.Language;
using WibboEmulator.Database.Daos.Moderation;
using WibboEmulator.Database.Interfaces;

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

    public void Init(IQueryAdapter dbClient)
    {
        if (this._searchResultLists.Count > 0)
        {
            this._searchResultLists.Clear();
        }

        if (this._featuredRooms.Count > 0)
        {
            this._featuredRooms.Clear();
        }

        var table = NavigatorCategoryDao.GetAll(dbClient);

        if (table != null)
        {
            foreach (DataRow row in table.Rows)
            {
                if (!this._searchResultLists.ContainsKey(Convert.ToInt32(row["id"])))
                {
                    this._searchResultLists.Add(Convert.ToInt32(row["id"]), new SearchResultList(Convert.ToInt32(row["id"]), Convert.ToString(row["category"]), Convert.ToString(row["category_identifier"]), Convert.ToString(row["public_name"]), true, -1, Convert.ToInt32(row["required_rank"]), Convert.ToBoolean(row["minimized"]), NavigatorViewModeUtility.GetViewModeByString(Convert.ToString(row["view_mode"])), Convert.ToString(row["category_type"]), Convert.ToString(row["search_allowance"]), Convert.ToInt32(row["order_id"])));
                }
            }
        }

        var getPublics = NavigatorPublicDao.GetAll(dbClient);

        if (getPublics != null)
        {
            foreach (DataRow row in getPublics.Rows)
            {
                if (!this._featuredRooms.ContainsKey(Convert.ToInt32(row["room_id"])))
                {
                    this._featuredRooms.Add(Convert.ToInt32(row["room_id"]), new FeaturedRoom(Convert.ToInt32(row["room_id"]), Convert.ToString(row["image_url"]), LanguageManager.ParseLanguage(Convert.ToString(row["langue"])), (string)row["category_type"]));
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
