using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class ItemUserCollision : WiredActionBase, IWiredEffect, IWired
    {
        public ItemUserCollision(Item item, Room room) : base(item, room, (int)WiredActionType.FLEE)
        {
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            this.HandleItems();
        }

        private void HandleItems()
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

            foreach (Point Coord in item.GetCoords)
            {
                RoomUser roomUser = this.RoomInstance.GetRoomUserManager().GetUserForSquare(Coord.X, Coord.Y);
                if (roomUser != null)
                {
                    this.RoomInstance.GetWiredHandler().TriggerCollision(roomUser, item);
                    return;
                }
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, this.Items);
        }

        public void LoadFromDatabase(DataRow row)
        {
            string triggerItems = row["triggers_item"].ToString();

            if (triggerItems == "")
            {
                return;
            }

            foreach (string itemId in triggerItems.Split(';'))
            {
                if (!int.TryParse(itemId, out int id))
                    continue;

                if(!this.StuffIds.Contains(id))
                    this.StuffIds.Add(id);
            }
        }
    }
}
