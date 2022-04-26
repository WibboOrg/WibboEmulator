using Butterfly.Game.Items;

namespace Butterfly.Game.Catalog
{
    public class CatalogItem
    {
        public int Id;
        public int ItemId;
        public ItemData Data;
        public int Amount;
        public int CostCredits;
        public bool HaveOffer;
        public bool IsLimited;
        public string Name;
        public int PageID;
        public int CostDuckets;
        public int LimitedEditionStack;
        public int LimitedEditionSells;
        public int CostWibboPoints;
        public int CostLimitCoins;
        public string Badge;

        public CatalogItem(int Id, int ItemId, ItemData Data, string CatalogName, int PageId, int CostCredits, int CostPixels,
            int CostWibboPoints, int CostLimitCoins, int Amount, int LimitedEditionSells, int LimitedEditionStack, bool HaveOffer, string Badge)
        {
            this.Id = Id;
            this.Name = CatalogName;
            this.ItemId = ItemId;
            this.Data = Data;
            this.PageID = PageId;
            this.CostCredits = CostCredits;
            this.CostDuckets = CostPixels;
            this.CostWibboPoints = CostWibboPoints;
            this.CostLimitCoins = CostLimitCoins;
            this.Amount = Amount;
            this.LimitedEditionSells = LimitedEditionSells;
            this.LimitedEditionStack = LimitedEditionStack;
            this.IsLimited = (LimitedEditionStack > 0);
            this.HaveOffer = HaveOffer;
            this.Badge = Badge;
        }
    }
}