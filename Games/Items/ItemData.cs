namespace WibboEmulator.Games.Items;

public class ItemData
{
    public int Id { get; set; }
    public int SpriteId { get; set; }
    public string ItemName { get; set; }
    public char Type { get; set; }
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
    public int RarityLevel { get; set; }
    public int Amount { get; set; }

    public ItemData(int id, int sprite, string name, string type, int width, int length, double height, bool stackable, bool walkable, bool isSeat,
        bool allowRecycle, bool allowTrade, bool allowGift, bool allowInventoryStack, InteractionType interactionType, int modes,
        string vendingIds, string adjustableHeights, int effectId, bool isRare, int rarityLevel, int amount)
    {
        this.Id = id;
        this.SpriteId = sprite;
        this.ItemName = name;
        this.Type = char.Parse(type);
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
                try
                {
                    this.VendingIds.Add(int.Parse(vendingId));
                }
                catch
                {
                    Console.WriteLine("Error with Item " + this.ItemName + " - Vending Ids");
                    continue;
                }
            }
        }
        else if (!string.IsNullOrEmpty(vendingIds) && int.Parse(vendingIds) > 0)
        {
            this.VendingIds.Add(int.Parse(vendingIds));
        }

        this.AdjustableHeights = new List<double>();

        try
        {
            if (adjustableHeights.Contains(','))
            {
                foreach (var H in adjustableHeights.Split(','))
                {
                    this.AdjustableHeights.Add(double.Parse(H));
                }
            }

            else if (!string.IsNullOrEmpty(adjustableHeights) && double.Parse(adjustableHeights) > 0)
            {
                this.AdjustableHeights.Add(double.Parse(adjustableHeights));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("ID ( " + this.Id + " ) : " + e);
        }

        this.EffectId = effectId;
        this.IsRare = isRare;
        this.RarityLevel = rarityLevel;
        this.Amount = amount;
    }
}
