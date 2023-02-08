namespace WibboEmulator.Games.Roleplays.Troc;

public class RPTrocUser
{
    public int UserId { get; set; }
    public Dictionary<int, int> ItemIds { get; set; }
    public bool Accepted { get; set; }
    public bool Confirmed { get; set; }

    public RPTrocUser(int userId)
    {
        this.UserId = userId;
        this.ItemIds = new Dictionary<int, int>();
        this.Accepted = false;
        this.Confirmed = false;
    }

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
        if (!this.ItemIds.ContainsKey(itemId))
        {
            return;
        }

        if (this.ItemIds[itemId] > 1)
        {
            this.ItemIds[itemId]--;
        }
        else
        {
            _ = this.ItemIds.Remove(itemId);
        }
    }
}
