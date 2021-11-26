using Butterfly.Game.Items;
using Butterfly.Game.Rooms;
using System;

namespace Butterfly.Utility.Events
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
