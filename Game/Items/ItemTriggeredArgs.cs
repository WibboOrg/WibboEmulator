using Butterfly.Game.Rooms;
using System;

namespace Butterfly.Game.Items
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
