using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers
{
    public class SateChanged : WiredTriggerBase, IWired, IWiredCycleable
    {
        private WiredHandler handler;
        private List<Item> items;
        private readonly Item item;
        private readonly OnItemTrigger delegateFunction;

        public int DelayCycle { get => this.Delay; set => this.Delay = value; }

        public SateChanged(WiredHandler handler, Item item, List<Item> items, List<int> stuffIds, int delay)
        {
            this.Id = item.Id;
            this.Type = (int)WiredTriggerType.TOGGLE_FURNI;
            this.StuffTypeSelectionEnabled = true;
            this.StuffTypeId = item.GetBaseItem().SpriteId;
            this.StuffIds = stuffIds;

            this.handler = handler;
            this.items = items;
            this.item = item;
            this.Delay = delay;
            this.delegateFunction = new OnItemTrigger(this.Triggered);

            foreach (Item roomItem in items)
            {
                roomItem.ItemTriggerEventHandler += this.delegateFunction;
            }
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            if (user != null)
            {
                this.OnTriggered(user, item);
            }

            return false;
        }

        private void Triggered(object sender, ItemTriggeredArgs e)
        {
            if (this.Delay > 0)
            {
                this.handler.RequestCycle(new WiredCycle(this, e.TriggeringUser, e.item, this.Delay));
            }
            else
            {
                this.OnTriggered(e.TriggeringUser, e.item);
            }
        }

        private void OnTriggered(RoomUser user, Item item)
        {
            this.handler.ExecutePile(this.item.Coordinate, user, item);
        }

        public override void Dispose()
        {
            this.isDisposed = true;
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

        public void OnTrigger(Client Session, int SpriteId)
        {
            this.SendWiredPacket(Session);
            return;
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_TRIGGER);
            Message.WriteBoolean(true);
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
            Message.WriteInteger((int)WiredTriggerType.TOGGLE_FURNI);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }
    }
}
