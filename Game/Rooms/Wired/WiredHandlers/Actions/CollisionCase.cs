using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class CollisionCase : WiredActionBase, IWiredEffect, IWired
    {
        public CollisionCase(Item item, Room room) : base(item, room, (int)WiredActionType.CHASE)
        {
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            foreach (Item roomItem in this.Items.ToArray())
            {
                this.HandleMovement(roomItem);
            }
        }

        private void HandleMovement(Item item)
        {
            if (this.RoomInstance.GetRoomItemHandler().GetItem(item.Id) == null)
            {
                return;
            }

            RoomUser roomUser = this.RoomInstance.GetRoomUserManager().GetUserForSquare(item.GetX, item.GetY);
            if (roomUser != null)
            {
                this.RoomInstance.GetWiredHandler().TriggerCollision(roomUser, item);
                return;
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, this.Items);
        }

        public void LoadFromDatabase(DataRow row)
        {
            string triggerItem = row["triggers_item"].ToString();

            if (triggerItem == "")
                return;

            foreach (string itemid in triggerItem.Split(';'))
            {
                Item roomItem = this.RoomInstance.GetRoomItemHandler().GetItem(Convert.ToInt32(itemid));
                if (roomItem != null && !this.Items.Contains(roomItem) && roomItem.Id != this.Id)
                {
                    this.Items.Add(roomItem);
                }
            }
        }
    }
}
