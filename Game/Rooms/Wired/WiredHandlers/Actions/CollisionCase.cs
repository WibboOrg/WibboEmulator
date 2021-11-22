using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class CollisionCase : IWiredEffect, IWired
    {
        private Room room;
        private WiredHandler handler;
        private readonly int itemID;
        private List<Item> items;
        private bool isDisposed;

        public CollisionCase(List<Item> items, Room room, WiredHandler handler, int itemID)
        {
            this.items = items;
            this.room = room;
            this.handler = handler;
            this.itemID = itemID;
            this.isDisposed = false;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            this.HandleItems();
        }

        public void Dispose()
        {
            this.isDisposed = true;
            this.room = null;
            this.handler = null;
            if (this.items != null)
            {
                this.items.Clear();
            }

            this.items = null;
        }

        private void HandleItems()
        {
            foreach (Item roomItem in this.items.ToArray())
            {
                this.HandleMovement(roomItem);
            }
        }

        private void HandleMovement(Item item)
        {
            if (this.room.GetRoomItemHandler().GetItem(item.Id) == null)
            {
                return;
            }

            RoomUser roomUser = this.room.GetRoomUserManager().GetUserForSquare(item.GetX, item.GetY);
            if (roomUser != null)
            {
                this.handler.TriggerCollision(roomUser, item);
                return;
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, string.Empty, false, this.items);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            string triggerItem = row["triggers_item"].ToString();

            if (triggerItem == "")
                return;

            foreach (string itemid in triggerItem.Split(';'))
            {
                Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(itemid));
                if (roomItem != null && !this.items.Contains(roomItem) && roomItem.Id != this.itemID)
                {
                    this.items.Add(roomItem);
                }
            }
        }

        public void OnTrigger(Client Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_ACTION);
            Message.WriteBoolean(false);
            Message.WriteInteger(10);
            Message.WriteInteger(this.items.Count);
            foreach (Item roomItem in this.items)
            {
                Message.WriteInteger(roomItem.Id);
            }

            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.itemID);
            Message.WriteString("");
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(12);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }

        public bool Disposed()
        {
            return this.isDisposed;
        }
    }
}
