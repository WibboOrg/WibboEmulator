namespace WibboEmulator.Games.Catalogs;
using System.Data;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.Catalogs.Marketplace;
using WibboEmulator.Games.Catalogs.Pets;
using WibboEmulator.Games.Catalogs.Vouchers;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Users;
using WibboEmulator.Utilities;

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

    public void Initialize(IDbConnection dbClient, ItemDataManager itemDataManager)
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

        this._voucherManager.Initialize(dbClient);

        var catalogItemList = CatalogItemDao.GetAll(dbClient);

        if (catalogItemList.Count != 0)
        {
            foreach (var catalogItem in catalogItemList)
            {
                if (catalogItem.Amount <= 0)
                {
                    continue;
                }

                var itemId = catalogItem.Id;
                var pageId = catalogItem.PageId;
                var baseId = catalogItem.ItemId;

                if (!itemDataManager.GetItem(baseId, out var data))
                {
                    Console.WriteLine("Couldn't load Catalog Item " + itemId + ", no furniture record found.");
                    continue;
                }

                if (!this._badges.Contains(catalogItem.Badge))
                {
                    this._badges.Add(catalogItem.Badge);
                }

                if (!this._items.TryGetValue(pageId, out var items))
                {
                    this._items.Add(pageId, new Dictionary<int, CatalogItem>());
                }

                this._items[pageId].Add(catalogItem.Id, new CatalogItem(catalogItem.Id, catalogItem.ItemId,
                    data, catalogItem.CatalogName, catalogItem.PageId, catalogItem.CostCredits,
                    catalogItem.CostPixels, catalogItem.CostDiamonds, catalogItem.CostLimitCoins,
                    catalogItem.Amount, catalogItem.LimitedSells,
                    catalogItem.LimitedStack, catalogItem.OfferActive,
                    catalogItem.Badge));

                this._itemsPage.Add(catalogItem.Id, pageId);
            }

            var catalogPageList = CatalogPageDao.GetAll(dbClient);

            if (catalogPageList.Count != 0)
            {
                foreach (var catalogPage in catalogPageList)
                {
                    this._pages.Add(catalogPage.Id, new CatalogPage(catalogPage.Id, catalogPage.ParentId, catalogPage.Enabled, catalogPage.Caption,
                        catalogPage.PageLink, catalogPage.IconImage, catalogPage.RequiredRight, catalogPage.PageLayout,
                        catalogPage.PageStrings1, catalogPage.PageStrings2, catalogPage.CaptionEn ?? "",
                        catalogPage.CaptionBr ?? "", catalogPage.PageStrings2En ?? "", catalogPage.PageStrings2Br ?? "", catalogPage.IsPremium,
                        this._items.TryGetValue(catalogPage.Id, out var value) ? value : new Dictionary<int, CatalogItem>()));
                }
            }

            var botList = CatalogBotPresetDao.GetAll(dbClient);

            if (botList.Count != 0)
            {
                foreach (var bot in botList)
                {
                    this._botPresets.Add(bot.Id, new CatalogBot(bot.Id, bot.Name, bot.Figure, bot.Motto, bot.Gender, bot.AiType));
                }
            }

            var promotionList = CatalogPromotionDao.GetAll(dbClient);

            if (promotionList.Count != 0)
            {
                foreach (var promotion in promotionList)
                {
                    if (!this._promotions.ContainsKey(promotion.Id))
                    {
                        this._promotions.Add(promotion.Id, new CatalogPromotion(promotion.Id, promotion.Title, promotion.TitleEn, promotion.TitleBr, promotion.Image, promotion.Unknown, promotion.PageLink, promotion.ParentId));
                    }
                }
            }

            var petRaceList = CatalogPetRaceDao.GetAll(dbClient);

            if (petRaceList.Count != 0)
            {
                foreach (var petRace in petRaceList)
                {
                    var race = new PetRace(petRace.RaceId, petRace.Color1, petRace.Color2, petRace.Has1Color, petRace.Has2Color);
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

    public CatalogItem FindItem(int itemId, User user)
    {
        if (!this._itemsPage.TryGetValue(itemId, out var pageId))
        {
            return null;
        }

        if (!this._pages.TryGetValue(pageId, out var page))
        {
            return null;
        }

        if (page == null || !page.Enabled || !page.HavePermission(user))
        {
            return null;
        }

        if (page.Items.TryGetValue(itemId, out var item))
        {
            return item;
        }

        return null;
    }

    public List<int> GetAllItemsIdAllowed()
    {
        var furniIdAllow = new List<int>();

        foreach (var page in this._pages.Values)
        {
            if (page.RequiredRight == "")
            {
                foreach (var item in page.Items.Values)
                {
                    if (item.IsLimited || item.Amount > 1 ||
                        item.Data.InteractionType == InteractionType.EXCHANGE || item.Data.InteractionType == InteractionType.TROPHY ||
                        item.Data.InteractionType == InteractionType.BADGE || (item.Data.Type != ItemType.S && item.Data.Type != ItemType.I) ||
                        item.CostWibboPoints > 0 || item.CostLimitCoins > 0 ||
                        item.Data.IsRare || item.Data.RarityLevel > RaretyLevelType.None)
                    {
                        continue;
                    }

                    furniIdAllow.AddIfNotExists(item.ItemId);
                }
            }
        }

        return furniIdAllow;
    }

    public bool TryGetBot(int itemId, out CatalogBot bot) => this._botPresets.TryGetValue(itemId, out bot);

    public bool TryGetPage(int pageId, out CatalogPage page) => this._pages.TryGetValue(pageId, out page);

    public ICollection<CatalogPage> GetPages() => this._pages.Values;

    public ICollection<CatalogPromotion> GetPromotions() => this._promotions.Values;

    public MarketplaceManager GetMarketplace() => this._marketplace;

    public VoucherManager GetVoucherManager() => this._voucherManager;
}
