namespace WibboEmulator.Games.Catalogs;
using System.Data;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.Catalogs.Pets;
using WibboEmulator.Games.Catalogs.Vouchers;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Users;
using WibboEmulator.Utilities;

public static class CatalogManager
{
    private static readonly Dictionary<int, CatalogPage> CatalogPages = new();
    private static readonly Dictionary<int, CatalogBot> BotPresets = new();
    private static readonly Dictionary<int, Dictionary<int, CatalogItem>> Items = new();
    private static readonly Dictionary<int, CatalogPromotion> CatalogPromotions = new();
    private static readonly Dictionary<int, int> ItemsPage = new();
    private static readonly List<string> Badges = new();
    private static readonly List<PetRace> Races = new();

    public static void Initialize(IDbConnection dbClient)
    {
        if (CatalogPages.Count > 0)
        {
            CatalogPages.Clear();
        }

        if (BotPresets.Count > 0)
        {
            BotPresets.Clear();
        }

        if (Items.Count > 0)
        {
            Items.Clear();
        }

        if (CatalogPromotions.Count > 0)
        {
            CatalogPromotions.Clear();
        }

        if (ItemsPage.Count > 0)
        {
            ItemsPage.Clear();
        }

        if (Badges.Count > 0)
        {
            Badges.Clear();
        }

        if (Races.Count > 0)
        {
            Races.Clear();
        }

        VoucherManager.Initialize(dbClient);

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

                if (!ItemManager.GetItem(baseId, out var data))
                {
                    Console.WriteLine("Couldn't load Catalog Item " + itemId + ", no furniture record found.");
                    continue;
                }

                if (!Badges.Contains(catalogItem.Badge))
                {
                    Badges.Add(catalogItem.Badge);
                }

                if (!Items.TryGetValue(pageId, out var items))
                {
                    Items.Add(pageId, new Dictionary<int, CatalogItem>());
                }

                Items[pageId].Add(catalogItem.Id, new CatalogItem(catalogItem.Id, catalogItem.ItemId,
                   data, catalogItem.CatalogName, catalogItem.PageId, catalogItem.CostCredits,
                   catalogItem.CostPixels, catalogItem.CostDiamonds, catalogItem.CostLimitCoins,
                   catalogItem.Amount, catalogItem.LimitedSells,
                   catalogItem.LimitedStack, catalogItem.OfferActive,
                   catalogItem.Badge));

                ItemsPage.Add(catalogItem.Id, pageId);
            }

            var catalogPageList = CatalogPageDao.GetAll(dbClient);

            if (catalogPageList.Count != 0)
            {
                foreach (var catalogPage in catalogPageList)
                {
                    CatalogPages.Add(catalogPage.Id, new CatalogPage(catalogPage.Id, catalogPage.ParentId, catalogPage.Enabled, catalogPage.Caption,
                       catalogPage.PageLink, catalogPage.IconImage, catalogPage.RequiredRight, catalogPage.PageLayout,
                       catalogPage.PageStrings1, catalogPage.PageStrings2, catalogPage.CaptionEn ?? "",
                       catalogPage.CaptionBr ?? "", catalogPage.PageStrings2En ?? "", catalogPage.PageStrings2Br ?? "", catalogPage.IsPremium,
                        Items.TryGetValue(catalogPage.Id, out var value) ? value : new Dictionary<int, CatalogItem>()));
                }
            }

            var botList = CatalogBotPresetDao.GetAll(dbClient);

            if (botList.Count != 0)
            {
                foreach (var bot in botList)
                {
                    BotPresets.Add(bot.Id, new CatalogBot(bot.Id, bot.Name, bot.Figure, bot.Motto, bot.Gender, bot.AiType));
                }
            }

            var promotionList = CatalogPromotionDao.GetAll(dbClient);

            if (promotionList.Count != 0)
            {
                foreach (var promotion in promotionList)
                {
                    if (!CatalogPromotions.ContainsKey(promotion.Id))
                    {
                        CatalogPromotions.Add(promotion.Id, new CatalogPromotion(promotion.Id, promotion.Title, promotion.TitleEn, promotion.TitleBr, promotion.Image, promotion.Unknown, promotion.PageLink, promotion.ParentId));
                    }
                }
            }

            var petRaceList = CatalogPetRaceDao.GetAll(dbClient);

            if (petRaceList.Count != 0)
            {
                foreach (var petRace in petRaceList)
                {
                    var race = new PetRace(petRace.RaceId, petRace.Color1, petRace.Color2, petRace.Has1Color, petRace.Has2Color);
                    if (!Races.Contains(race))
                    {
                        Races.Add(race);
                    }
                }
            }
        }

        Console.WriteLine("Catalog Manager -> LOADED");
    }

    public static List<PetRace> GetRacesForRaceId(int raceId) => Races.Where(race => race.RaceId == raceId).ToList();

    public static bool HasBadge(string code) => Badges.Contains(code);

    public static CatalogItem FindItem(int itemId, User user)
    {
        if (!ItemsPage.TryGetValue(itemId, out var pageId))
        {
            return null;
        }

        if (!CatalogPages.TryGetValue(pageId, out var page))
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

    public static List<int> AllItemsIdAllowed
    {
        get
        {
            var furniIdAllow = new List<int>();

            foreach (var page in CatalogPages.Values)
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

                        furniIdAllow.TryAdd(item.ItemId);
                    }
                }
            }

            return furniIdAllow;
        }
    }

    public static bool TryGetBot(int itemId, out CatalogBot bot) => BotPresets.TryGetValue(itemId, out bot);

    public static bool TryGetPage(int pageId, out CatalogPage page) => CatalogPages.TryGetValue(pageId, out page);

    public static ICollection<CatalogPage> Pages => CatalogPages.Values;

    public static ICollection<CatalogPromotion> Promotions => CatalogPromotions.Values;
}
