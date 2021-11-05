using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers
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
            if(int.TryParse(row["trigger_data"].ToString(), out int delay))
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
            ServerPacket Message8 = new ServerPacket(ServerPacketHeader.WIRED_ACTION);
            Message8.WriteBoolean(false);
            Message8.WriteInteger(10);
            Message8.WriteInteger(this.items.Count);
            foreach (Item roomItem in this.items)
            {
                Message8.WriteInteger(roomItem.Id);
            }

            Message8.WriteInteger(SpriteId);
            Message8.WriteInteger(this.item.Id);
            Message8.WriteString("");
            Message8.WriteInteger(0);
            Message8.WriteInteger(8);
            Message8.WriteInteger(0);
            Message8.WriteInteger(this.Delay);
            Message8.WriteInteger(0);
            Message8.WriteInteger(0);
            Session.SendPacket(Message8);
        }

        public bool Disposed()
        {
            return this.disposed;
        }
    }
}
