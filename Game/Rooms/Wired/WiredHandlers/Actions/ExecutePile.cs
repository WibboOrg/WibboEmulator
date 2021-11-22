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
    public class ExecutePile : IWired, IWiredEffect, IWiredCycleable
    {
        private WiredHandler handler;
        private readonly Item item;
        private List<Item> items;
        public int DelayCycle { get; set; }
        private bool disposed;

        public ExecutePile(List<Item> items, int mDeley, WiredHandler handler, Item item)
        {
            this.DelayCycle = mDeley;
            this.disposed = false;

            this.items = items;
            this.item = item;
            this.handler = handler;
        }

        public bool Disposed()
        {
            return this.disposed;
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.HandleEffect(user, item);
            return false;
        }

        private void HandleEffect(RoomUser user, Item TriggerItem)
        {
            foreach (Item roomItem in this.items)
            {
                if (roomItem.Coordinate != this.item.Coordinate)
                {
                    this.handler.ExecutePile(roomItem.Coordinate, user, TriggerItem);
                }
            }
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.DelayCycle > 0)
            {
                this.handler.RequestCycle(new WiredCycle(this, user, TriggerItem, this.DelayCycle));
            }
            else
            {
                this.HandleEffect(user, TriggerItem);
            }
        }

        public void Dispose()
        {
            this.disposed = true;
            this.handler = null;
            if (this.items != null)
            {
                this.items.Clear();
            }

            this.items = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, this.DelayCycle.ToString(), string.Empty, false, this.items);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if (int.TryParse(row["trigger_data_2"].ToString(), out int delay))
                this.DelayCycle = delay;

            string triggerItem = row["triggers_item"].ToString();

            if (triggerItem == "")
                return;

            foreach (string itemid in triggerItem.Split(';'))
            {
                Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(itemid));
                if (roomItem != null && !this.items.Contains(roomItem) && roomItem.Id != this.item.Id)
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
            Message.WriteInteger(this.item.Id);
            Message.WriteString("");
            Message.WriteInteger(0);

            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(this.DelayCycle);

            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }
    }
}
