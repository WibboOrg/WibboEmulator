namespace WibboEmulator.Games.Catalogs;
using System.Data;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Catalogs.Marketplace;
using WibboEmulator.Games.Catalogs.Pets;
using WibboEmulator.Games.Catalogs.Vouchers;
using WibboEmulator.Games.Items;

public class CatalogManager
{
    private readonly MarketplaceManager _marketplace;
    private readonly VoucherManager _voucherManager;

    private readonly Dictionary<int, CatalogPage> _pages;
    private readonly Dictionary<int, CatalogBot> _botPresets;
    private readonly Dictionary<int, Dictionary<int, CatalogItem>> _items;
    private readonly Dictionary<int, CatalogPromotion> _promotions;
    private readonly Dictionary<int, int> _itemsPage;
    private readonly List<string> _badges;
    private readonly List<PetRace> _races;

    public CatalogManager()
    {
        this._marketplace = new MarketplaceManager();
        this._voucherManager = new VoucherManager();
        this._pages = new Dictionary<int, CatalogPage>();
        this._botPresets = new Dictionary<int, CatalogBot>();
        this._items = new Dictionary<int, Dictionary<int, CatalogItem>>();
        this._promotions = new Dictionary<int, CatalogPromotion>();
        this._itemsPage = new Dictionary<int, int>();
        this._badges = new List<string>();
        this._races = new List<PetRace>();
    }

    public void Init(IQueryAdapter dbClient, ItemDataManager itemDataManager)
    {
        if (this._pages.Count > 0)
        {
            this._pages.Clear();
        }

        if (this._botPresets.Count > 0)
        {
            this._botPresets.Clear();
        }

        if (this._items.Count > 0)
        {
            this._items.Clear();
        }

        if (this._promotions.Count > 0)
        {
            this._promotions.Clear();
        }

        if (this._itemsPage.Count > 0)
        {
            this._itemsPage.Clear();
        }

        if (this._badges.Count > 0)
        {
            this._badges.Clear();
        }

        if (this._races.Count > 0)
        {
            this._races.Clear();
        }

        this._voucherManager.Init(dbClient);

        var catalogueItems = CatalogItemDao.GetAll(dbClient);

        if (catalogueItems != null)
        {
            foreach (DataRow row in catalogueItems.Rows)
            {
                if (Convert.ToInt32(row["amount"]) <= 0)
                {
                    continue;
                }

                var itemId = Convert.ToInt32(row["id"]);
                var pageId = Convert.ToInt32(row["page_id"]);
                var baseId = Convert.ToInt32(row["item_id"]);

                if (!itemDataManager.GetItem(baseId, out var data))
                {
                    Console.WriteLine("Couldn't load Catalog Item " + itemId + ", no furniture record found.");
                    continue;
                }

                if (!this._badges.Contains((string)row["badge"]))
                {
                    this._badges.Add((string)row["badge"]);
                }

                if (!this._items.ContainsKey(pageId))
                {
                    this._items[pageId] = new Dictionary<int, CatalogItem>();
                }

                this._items[pageId].Add(Convert.ToInt32(row["id"]), new CatalogItem(Convert.ToInt32(row["id"]), Convert.ToInt32(row["item_id"]),
                    data, Convert.ToString(row["catalog_name"]), Convert.ToInt32(row["page_id"]), Convert.ToInt32(row["cost_credits"]), Convert.ToInt32(row["cost_pixels"]), Convert.ToInt32(row["cost_diamonds"]), Convert.ToInt32(row["cost_limitcoins"]),
                    Convert.ToInt32(row["amount"]), DBNull.Value.Equals(row["limited_sells"]) ? 0 : Convert.ToInt32(row["limited_sells"]), DBNull.Value.Equals(row["limited_stack"]) ? 0 : Convert.ToInt32(row["limited_stack"]), WibboEnvironment.EnumToBool(row["offer_active"].ToString()), Convert.ToString(row["badge"])));

                this._itemsPage.Add(Convert.ToInt32(row["id"]), pageId);
            }

            var catalogPages = CatalogPageDao.GetAll(dbClient);

            if (catalogPages != null)
            {
                foreach (DataRow row in catalogPages.Rows)
                {
                    this._pages.Add(Convert.ToInt32(row["id"]), new CatalogPage(Convert.ToInt32(row["id"]), Convert.ToInt32(row["parent_id"]), row["enabled"].ToString(), Convert.ToString(row["caption"]),
                        Convert.ToString(row["page_link"]), Convert.ToInt32(row["icon_image"]), Convert.ToInt32(row["min_rank"]), Convert.ToString(row["page_layout"]),
                        Convert.ToString(row["page_strings_1"]), Convert.ToString(row["page_strings_2"]), Convert.ToString(row["caption_en"]),
                        Convert.ToString(row["caption_br"]), Convert.ToString(row["page_strings_2_en"]), Convert.ToString(row["page_strings_2_br"]), row["is_premium"].ToString(),
                        this._items.TryGetValue(Convert.ToInt32(row["id"]), out var value) ? value : new Dictionary<int, CatalogItem>()));
                }
            }

            var bots = CatalogBotPresetDao.GetAll(dbClient);

            if (bots != null)
            {
                foreach (DataRow row in bots.Rows)
                {
                    this._botPresets.Add(Convert.ToInt32(row["id"]), new CatalogBot(Convert.ToInt32(row["id"]), Convert.ToString(row["name"]), Convert.ToString(row["figure"]), Convert.ToString(row["motto"]), Convert.ToString(row["gender"]), Convert.ToString(row["ai_type"])));
                }
            }

            var getPromotions = CatalogPromotionDao.GetAll(dbClient);

            if (getPromotions != null)
            {
                foreach (DataRow row in getPromotions.Rows)
                {
                    if (!this._promotions.ContainsKey(Convert.ToInt32(row["id"])))
                    {
                        this._promotions.Add(Convert.ToInt32(row["id"]), new CatalogPromotion(Convert.ToInt32(row["id"]), Convert.ToString(row["title"]), Convert.ToString(row["title_en"]), Convert.ToString(row["title_br"]), Convert.ToString(row["image"]), Convert.ToInt32(row["unknown"]), Convert.ToString(row["page_link"]), Convert.ToInt32(row["parent_id"])));
                    }
                }
            }

            var getRaces = CatalogPetRaceDao.GetAll(dbClient);

            if (getRaces != null)
            {
                foreach (DataRow row in getRaces.Rows)
                {
                    var race = new PetRace(Convert.ToInt32(row["raceid"]), Convert.ToInt32(row["color1"]), Convert.ToInt32(row["color2"]), Convert.ToString(row["has1color"]) == "1", Convert.ToString(row["has2color"]) == "1");
                    if (!this._races.Contains(race))
                    {
                        this._races.Add(race);
                    }
                }
            }
        }

        Console.WriteLine("Catalog Manager -> LOADED");
    }

    public List<PetRace> GetRacesForRaceId(int raceId) => this._races.Where(race => race.RaceId == raceId).ToList();

    public bool HasBadge(string code) => this._badges.Contains(code);

    public CatalogItem FindItem(int itemId, int rank)
    {
        if (!this._itemsPage.ContainsKey(itemId))
        {
            return null;
        }

        var pageId = this._itemsPage[itemId];
        if (!this._pages.ContainsKey(pageId))
        {
            return null;
        }

        var page = this._pages[pageId];
        if (page == null || !page.Enabled || page.MinimumRank > rank)
        {
            return null;
        }

        if (page.Items.TryGetValue(itemId, out var value))
        {
            return value;
        }

        return null;
    }

    public bool TryGetBot(int itemId, out CatalogBot bot) => this._botPresets.TryGetValue(itemId, out bot);

    public bool TryGetPage(int pageId, out CatalogPage page) => this._pages.TryGetValue(pageId, out page);

    public ICollection<CatalogPage> GetPages() => this._pages.Values;

    public ICollection<CatalogPromotion> GetPromotions() => this._promotions.Values;

    public MarketplaceManager GetMarketplace() => this._marketplace;

    public VoucherManager GetVoucherManager() => this._voucherManager;
}
