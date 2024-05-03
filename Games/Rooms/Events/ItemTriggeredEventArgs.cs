namespace WibboEmulator.Games.Rooms.Events;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

public class ItemTriggeredEventArgs(RoomUser user, Item item, string value = "") : EventArgs
{
    public RoomUser User { get; private set; } = user;
    public Item Item { get; private set; } = item;
    public string Value { get; private set; } = value;
}
