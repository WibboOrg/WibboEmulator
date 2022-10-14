namespace WibboEmulator.Games.Rooms.Events;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

public class ItemTriggeredEventArgs : EventArgs
{
    public RoomUser User { get; private set; }
    public Item Item { get; private set; }
    public string Value { get; private set; }

    public ItemTriggeredEventArgs(RoomUser user, Item item, string value = "")
    {
        this.User = user;
        this.Item = item;
        this.Value = value;
    }
}
