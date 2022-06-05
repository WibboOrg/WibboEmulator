using Wibbo.Game.Items;
using Wibbo.Game.Rooms;

namespace Wibbo.Utilities.Events
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
