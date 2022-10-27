namespace WibboEmulator.Games.Roleplays.Player;

public class RolePlayInventoryItem
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int Count { get; set; }

    public RolePlayInventoryItem(int id, int itemId, int count)
    {
        this.Id = id;
        this.ItemId = itemId;
        this.Count = count;
    }
}
