using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Triggers
{
    public class UserTriggerSelf : WiredTriggerBase, IWired
    {
        private readonly RoomEventDelegate delegateFunction;

        public UserTriggerSelf(Item item, Room room) : base(item, room, (int)WiredTriggerType.COLLISION)
        {
            this.delegateFunction = new RoomEventDelegate(this.roomUserManager_OnUserSays);
            room.OnTriggerSelf += this.delegateFunction;
        }

        private void roomUserManager_OnUserSays(object sender, EventArgs e)
        {
            RoomUser user = (RoomUser)sender;
            if (user == null || user.IsBot)
            {
                return;
            }

            this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, user, null);
        }

        public override void Dispose()
        {
            base.Dispose();

            this.RoomInstance.GetWiredHandler().GetRoom().OnTriggerSelf -= this.delegateFunction;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, null);
        }

        public void LoadFromDatabase(DataRow row)
        {

        }
    }
}
