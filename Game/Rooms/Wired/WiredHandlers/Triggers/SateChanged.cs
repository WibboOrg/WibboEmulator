using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.GameClients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers
{
    public class SateChanged : IWired, IWiredCycleable
    {
        private WiredHandler handler;
        private List<Item> items;
        private readonly Item item;
        private readonly OnItemTrigger delegateFunction;
        public int Delay { get; set; }
        private bool disposed;

        public SateChanged(WiredHandler handler, Item item, List<Item> items, int delay)
        {
            this.handler = handler;
            this.items = items;
            this.item = item;
            this.Delay = delay;
            this.delegateFunction = new OnItemTrigger(this.itemTriggered);

            foreach (Item roomItem in items)
            {
                roomItem.ItemTriggerEventHandler += this.delegateFunction;
            }

            this.disposed = false;
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            if (user != null)
            {
                this.onTrigger(user, item);
            }

            return false;
        }

        private void itemTriggered(object sender, ItemTriggeredArgs e)
        {
            if (this.Delay > 0)
            {
                this.handler.RequestCycle(new WiredCycle(this, e.TriggeringUser, e.item, this.Delay));
            }
            else
            {
                this.onTrigger(e.TriggeringUser, e.item);
            }
        }

        private void onTrigger(RoomUser user, Item item)
        {
            this.handler.ExecutePile(this.item.Coordinate, user, item);
        }

        public void Dispose()
        {
            this.disposed = true;
            this.handler = null;
            if (this.items != null)
            {
                foreach (Item roomItem in this.items)
                {
                    roomItem.ItemTriggerEventHandler -= this.delegateFunction;
                }

                this.items.Clear();
            }
            this.items = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.Delay.ToString(), false, this.items);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int delay))
                this.Delay = delay;

            string triggerItem = row["triggers_item"].ToString();

            if (triggerItem == "")
                return;

            foreach (string item in triggerItem.Split(';'))
            {
                Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(item));
                if (roomItem != null && !this.items.Contains(roomItem) && roomItem.Id != this.item.Id)
                {
                    roomItem.ItemTriggerEventHandler += this.delegateFunction;
                    this.items.Add(roomItem);
                }
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
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
            Message.WriteInteger(8);
            Message.WriteInteger(0);
            Message.WriteInteger(this.Delay);
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
