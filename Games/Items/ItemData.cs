namespace WibboEmulator.Games.Items;

using WibboEmulator.Database.Daos.Item;

public class ItemData
{
    public int Id { get; set; }
    public int SpriteId { get; set; }
    public string ItemName { get; set; }
    public ItemType Type { get; set; }
    public int Width { get; set; }
    public int Length { get; set; }
    public double Height { get; set; }
    public bool Stackable { get; set; }
    public bool Walkable { get; set; }
    public bool IsSeat { get; set; }
    public bool AllowEcotronRecycle { get; set; }
    public bool AllowTrade { get; set; }
    public bool AllowMarketplaceSell { get; set; }
    public bool AllowGift { get; set; }
    public bool AllowInventoryStack { get; set; }
    public InteractionType InteractionType { get; set; }
    public int Modes { get; set; }
    public List<int> VendingIds { get; set; }
    public List<double> AdjustableHeights { get; set; }
    public int EffectId { get; set; }
    public bool IsRare { get; set; }
    public RaretyLevelType RarityLevel { get; set; }
    public int Amount { get; set; }

    public ItemData(int id, int sprite, string name, ItemType type, int width, int length, double height, bool stackable, bool walkable, bool isSeat,
        bool allowRecycle, bool allowTrade, bool allowGift, bool allowInventoryStack, InteractionType interactionType, int modes,
        string vendingIds, string adjustableHeights, int effectId, bool isRare, int rarityLevel, int amount)
    {
        this.Id = id;
        this.SpriteId = sprite;
        this.ItemName = name;
        this.Type = type;
        this.Width = width;
        this.Length = length;
        this.Height = height;
        this.Stackable = stackable;
        this.Walkable = walkable;
        this.IsSeat = isSeat;
        this.AllowEcotronRecycle = allowRecycle;
        this.AllowTrade = allowTrade;
        this.AllowGift = allowGift;
        this.AllowInventoryStack = allowInventoryStack;
        this.InteractionType = interactionType;
        this.Modes = modes;
        this.VendingIds = new List<int>();
        if (vendingIds.Contains(','))
        {
            foreach (var vendingId in vendingIds.Split(','))
            {
                if (int.TryParse(vendingId, out var result))
                {
                    this.VendingIds.Add(result);
                }
            }
        }
        else if (!string.IsNullOrEmpty(vendingIds) && int.Parse(vendingIds) > 0)
        {
            this.VendingIds.Add(int.Parse(vendingIds));
        }

        this.AdjustableHeights = new List<double>();

        if (adjustableHeights.Contains(','))
        {
            foreach (var h in adjustableHeights.Split(','))
            {
                if (double.TryParse(h, out var result))
                {
                    this.AdjustableHeights.Add(result);
                }
            }
        }

        else if (!string.IsNullOrEmpty(adjustableHeights) && double.Parse(adjustableHeights) > 0)
        {
            if (double.TryParse(adjustableHeights, out var result))
            {
                this.AdjustableHeights.Add(result);
            }
        }

        this.EffectId = effectId;
        this.IsRare = isRare;
        this.RarityLevel = (RaretyLevelType)rarityLevel;
        this.Amount = amount;
    }
}

public enum RaretyLevelType
{
    None = 0,
    Basic = 1,
    Commun = 2,
    Epic = 3,
    Legendary = 4
}