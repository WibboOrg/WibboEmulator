using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Effects
{
    public class UserMove : IWired, IWiredEffect, IWiredCycleable
    {
        private readonly WiredHandler handler;
        private readonly int itemID;
        private List<Item> items;
        public int DelayCycle { get; set; }
        private bool disposed;

        public UserMove(List<Item> items, int pDelay, WiredHandler handler, int itemID)
        {
            this.itemID = itemID;
            this.handler = handler;
            this.items = items;
            this.DelayCycle = pDelay;
            this.disposed = false;
        }

        private void Execute(RoomUser User)
        {
            if (this.items.Count == 0)
            {
                return;
            }

            Item roomItem = this.items[0];
            if (roomItem == null)
            {
                return;
            }

            if (roomItem.Coordinate != User.Coordinate)
            {
                User.IsWalking = true;
                User.GoalX = roomItem.GetX;
                User.GoalY = roomItem.GetY;
            }
        }

        public bool OnCycle(RoomUser User, Item Item)
        {
            this.Execute(User);
            return false;
        }

        public void Handle(RoomUser User, Item TriggerItem)
        {
            if (this.DelayCycle > 0)
            {
                this.handler.RequestCycle(new WiredCycle(this, User, TriggerItem, this.DelayCycle));
            }
            else
            {
                this.Execute(User);
            }
        }

        public void Dispose()
        {
            this.disposed = true;
            if (this.items != null)
            {
                this.items.Clear();
            }

            this.items = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, this.DelayCycle.ToString(), false, this.items);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int delay))
                this.DelayCycle = delay;

            string triggerItem = row["triggers_item"].ToString();

            if (triggerItem == "")
            {
                return;
            }

            foreach (string item in triggerItem.Split(';'))
            {
                Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(item));
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
            Message.WriteInteger(1);
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
            Message.WriteInteger(this.DelayCycle);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }

        public bool Disposed()
        {
            return this.disposed;
        }
    }
}
