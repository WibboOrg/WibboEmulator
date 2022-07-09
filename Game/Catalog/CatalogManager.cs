using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Catalog.Marketplace;
using Wibbo.Game.Catalog.Pets;
using Wibbo.Game.Catalog.Vouchers;
using Wibbo.Game.Items;
using System.Data;

namespace Wibbo.Game.Catalog
{
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
        private readonly List<PetRace> _races = new List<PetRace>();

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
        }

        public void Init(IQueryAdapter dbClient, ItemDataManager ItemDataManager)
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

            DataTable CatalogueItems = CatalogItemDao.GetAll(dbClient);

            if (CatalogueItems != null)
            {
                foreach (DataRow Row in CatalogueItems.Rows)
                {
                    if (Convert.ToInt32(Row["amount"]) <= 0)
                    {
                        continue;
                    }

                    int ItemId = Convert.ToInt32(Row["id"]);
                    int PageId = Convert.ToInt32(Row["page_id"]);
                    int BaseId = Convert.ToInt32(Row["item_id"]);

                    if (!ItemDataManager.GetItem(BaseId, out ItemData Data))
                    {
                        Console.WriteLine("Couldn't load Catalog Item " + ItemId + ", no furniture record found.");
                        continue;
                    }

                    if (!this._badges.Contains((string)Row["badge"]))
                    {
                        this._badges.Add((string)Row["badge"]);
                    }

                    if (!this._items.ContainsKey(PageId))
                    {
                        this._items[PageId] = new Dictionary<int, CatalogItem>();
                    }

                    this._items[PageId].Add(Convert.ToInt32(Row["id"]), new CatalogItem(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["item_id"]),
                        Data, Convert.ToString(Row["catalog_name"]), Convert.ToInt32(Row["page_id"]), Convert.ToInt32(Row["cost_credits"]), Convert.ToInt32(Row["cost_pixels"]), Convert.ToInt32(Row["cost_diamonds"]), Convert.ToInt32(Row["cost_limitcoins"]),
                        Convert.ToInt32(Row["amount"]), Convert.ToInt32(Row["limited_sells"]), Convert.ToInt32(Row["limited_stack"]), WibboEnvironment.EnumToBool(Row["offer_active"].ToString()), Convert.ToString(Row["badge"])));

                    this._itemsPage.Add(Convert.ToInt32(Row["id"]), PageId);
                }

                DataTable CatalogPages = CatalogPageDao.GetAll(dbClient);

                if (CatalogPages != null)
                {
                    foreach (DataRow Row in CatalogPages.Rows)
                    {
                        this._pages.Add(Convert.ToInt32(Row["id"]), new CatalogPage(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["parent_id"]), Row["enabled"].ToString(), Convert.ToString(Row["caption"]),
                            Convert.ToString(Row["page_link"]), Convert.ToInt32(Row["icon_image"]), Convert.ToInt32(Row["min_rank"]), Convert.ToString(Row["page_layout"]),
                            Convert.ToString(Row["page_strings_1"]), Convert.ToString(Row["page_strings_2"]), Convert.ToString(Row["caption_en"]),
                            Convert.ToString(Row["caption_br"]), Convert.ToString(Row["page_strings_2_en"]), Convert.ToString(Row["page_strings_2_br"]),
                            this._items.ContainsKey(Convert.ToInt32(Row["id"])) ? this._items[Convert.ToInt32(Row["id"])] : new Dictionary<int, CatalogItem>()));
                    }
                }

                DataTable bots = CatalogBotPresetDao.GetAll(dbClient);

                if (bots != null)
                {
                    foreach (DataRow Row in bots.Rows)
                    {
                        this._botPresets.Add(Convert.ToInt32(Row[0]), new CatalogBot(Convert.ToInt32(Row[0]), Convert.ToString(Row[1]), Convert.ToString(Row[2]), Convert.ToString(Row[3]), Convert.ToString(Row[4]), Convert.ToString(Row[5])));
                    }
                }

                DataTable GetPromotions = CatalogPromotionDao.GetAll(dbClient);

                if (GetPromotions != null)
                {
                    foreach (DataRow Row in GetPromotions.Rows)
                    {
                        if (!this._promotions.ContainsKey(Convert.ToInt32(Row["id"])))
                        {
                            this._promotions.Add(Convert.ToInt32(Row["id"]), new CatalogPromotion(Convert.ToInt32(Row["id"]), Convert.ToString(Row["title"]), Convert.ToString(Row["title_en"]), Convert.ToString(Row["title_br"]), Convert.ToString(Row["image"]), Convert.ToInt32(Row["unknown"]), Convert.ToString(Row["page_link"]), Convert.ToInt32(Row["parent_id"])));
                        }
                    }
                }

                DataTable GetRaces = CatalogPetRaceDao.GetAll(dbClient);

                if (GetRaces != null)
                {
                    foreach (DataRow Row in GetRaces.Rows)
                    {
                        PetRace Race = new PetRace(Convert.ToInt32(Row["raceid"]), Convert.ToInt32(Row["color1"]), Convert.ToInt32(Row["color2"]), (Convert.ToString(Row["has1color"]) == "1"), (Convert.ToString(Row["has2color"]) == "1"));
                        if (!this._races.Contains(Race))
                        {
                            this._races.Add(Race);
                        }
                    }
                }
            }

            Console.WriteLine("Catalog Manager -> LOADED");
        }

        public List<PetRace> GetRacesForRaceId(int RaceId)
        {
            return this._races.Where(Race => Race.RaceId == RaceId).ToList();
        }

        public bool HasBadge(string Code)
        {
            return this._badges.Contains(Code);
        }

        public CatalogItem FindItem(int ItemId, int Rank)
        {
            if (!this._itemsPage.ContainsKey(ItemId))
            {
                return null;
            }

            int PageId = this._itemsPage[ItemId];
            if (!this._pages.ContainsKey(PageId))
            {
                return null;
            }

            CatalogPage page = this._pages[PageId];
            if (page == null || !page.Enabled || page.MinimumRank > Rank)
            {
                return null;
            }

            if (page.Items.ContainsKey(ItemId))
            {
                return page.Items[ItemId];
            }

            return null;
        }

        public bool TryGetBot(int ItemId, out CatalogBot Bot)
        {
            return this._botPresets.TryGetValue(ItemId, out Bot);
        }

        public bool TryGetPage(int pageId, out CatalogPage page)
        {
            return this._pages.TryGetValue(pageId, out page);
        }

        public ICollection<CatalogPage> GetPages()
        {
            return this._pages.Values;
        }

        public ICollection<CatalogPromotion> GetPromotions()
        {
            return this._promotions.Values;
        }

        public MarketplaceManager GetMarketplace()
        {
            return this._marketplace;
        }

        public VoucherManager GetVoucherManager()
        {
            return this._voucherManager;
        }
    }
}