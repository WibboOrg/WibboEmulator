namespace WibboEmulator.Utilities.Events;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

public class ItemTriggeredArgs : EventArgs
{
    public RoomUser User { get; private set; }
    public Item Item { get; private set; }

    public ItemTriggeredArgs(RoomUser user, Item item)
    {
        this.User = user;
        this.Item = item;
    }
}
