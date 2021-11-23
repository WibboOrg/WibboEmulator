using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Triggers
{
    public class UserCollision : WiredTriggerBase, IWired
    {
        private Item item;
        private WiredHandler handler;
        private readonly RoomEventDelegate delegateFunction;

        public UserCollision(Item item, Room room) : base(item, room, (int)WiredTriggerType.COLLISION)
        {
            this.delegateFunction = new RoomEventDelegate(this.userCollisionDelegate);
            this.RoomInstance.GetWiredHandler().GetRoom().OnUserCls += this.delegateFunction;
        }

        private void userCollisionDelegate(object sender, EventArgs e)
        {
            RoomUser user = (RoomUser)sender;
            if (user == null)
            {
                return;
            }

            this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, user, null);
        }
        public override void Dispose()
        {
            this.RoomInstance.GetWiredHandler().GetRoom().OnUserCls -= this.delegateFunction;

            base.Dispose();
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
