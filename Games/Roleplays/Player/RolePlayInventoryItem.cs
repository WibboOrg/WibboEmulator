namespace WibboEmulator.Games.Roleplays.Player;

public class RolePlayInventoryItem(int id, int itemId, int count)
{
    public int Id { get; set; } = id;
    public int ItemId { get; set; } = itemId;
    public int Count { get; set; } = count;
}
