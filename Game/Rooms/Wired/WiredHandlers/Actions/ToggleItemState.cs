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
    public class ToggleItemState : IWired, IWiredCycleable, IWiredEffect
    {
        private readonly Item item;
        private Gamemap gamemap;
        private WiredHandler handler;
        private readonly List<Item> items;
        public int DelayCycle { get; set; }
        private bool disposed;

        public ToggleItemState(Gamemap gamemap, WiredHandler handler, List<Item> items, int delay, Item Item)
        {
            this.item = Item;
            this.gamemap = gamemap;
            this.handler = handler;
            this.items = items;
            this.DelayCycle = delay;
            this.disposed = false;
        }

        public bool OnCycle(RoomUser user, Item item)
        {

            this.ToggleItems(user);
            return false;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.DelayCycle > 0)
            {
                this.handler.RequestCycle(new WiredCycle(this, user, TriggerItem, this.DelayCycle));
            }
            else
            {
                this.ToggleItems(user);
            }
        }

        private bool ToggleItems(RoomUser user)
        {
            bool flag = false;
            foreach (Item roomItem in this.items)
            {
                if (roomItem != null)
                {
                    if (user != null && user.GetClient() != null)
                    {
                        roomItem.Interactor.OnTrigger(user.GetClient(), roomItem, 0, true);
                    }
                    else
                    {
                        roomItem.Interactor.OnTrigger(null, roomItem, 0, true);
                    }

                    flag = true;
                }
            }
            return flag;
        }

        public void Dispose()
        {
            this.disposed = true;
            this.gamemap = null;
            this.handler = null;
            if (this.items != null)
            {
                this.items.Clear();
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.DelayCycle.ToString(), false, this.items);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int delay))
                this.DelayCycle = delay;

            string triggerItem = row["triggers_item"].ToString();

            if (triggerItem == "")
                return;

            foreach (string item in triggerItem.Split(';'))
            {
                Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(item));
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
            Message.WriteString(this.DelayCycle.ToString());
            Message.WriteInteger(0);
            Message.WriteInteger(8);
            Message.WriteInteger(0);
            Message.WriteInteger(this.DelayCycle); //Seconde
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
