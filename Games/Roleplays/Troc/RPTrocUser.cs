namespace WibboEmulator.Games.Roleplays.Troc;

public class RPTrocUser(int userId)
{
    public int UserId { get; set; } = userId;
    public Dictionary<int, int> ItemIds { get; set; } = [];
    public bool Accepted { get; set; } = false;
    public bool Confirmed { get; set; } = false;

    public int GetCountItem(int itemId)
    {
        if (this.ItemIds.TryGetValue(itemId, out var value))
        {
            return value;
        }
        else
        {
            return 0;
        }
    }

    public void AddItemId(int itemId)
    {
        if (!this.ItemIds.ContainsKey(itemId))
        {
            this.ItemIds.Add(itemId, 1);
        }
        else
        {
            this.ItemIds[itemId]++;
        }
    }

    public void RemoveItemId(int itemId)
    {
        if (!this.ItemIds.TryGetValue(itemId, out var itemCount))
        {
            return;
        }

        if (itemCount > 1)
        {
            this.ItemIds[itemId]--;
        }
        else
        {
            _ = this.ItemIds.Remove(itemId);
        }
    }
}
