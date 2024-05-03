namespace WibboEmulator.Games.Loots;

public class Loot(int probability, int pageId, int itemId, string category, int amount)
{
    public int Probability { get; private set; } = probability;
    public int PageId { get; private set; } = pageId;
    public int ItemId { get; private set; } = itemId;
    public string Category { get; private set; } = category;
    public int Amount { get; private set; } = amount;
}
