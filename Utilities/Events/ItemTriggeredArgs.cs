using WibboEmulator.Game.Items;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Utilities.Events
{
    public class ItemTriggeredArgs : EventArgs
    {
        public readonly RoomUser User;
        public readonly Item Item;

        public ItemTriggeredArgs(RoomUser user, Item item)
        {
            this.User = user;
            this.Item = item;
        }
    }
}
