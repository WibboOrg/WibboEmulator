using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Triggers
{
    public class WalksOffFurni : IWired, IWiredCycleable
    {
        private Item item;
        private WiredHandler handler;
        private List<Item> items;
        private readonly UserAndItemDelegate delegateFunction;
        public int Delay { get; set; }
        private bool disposed;

        public WalksOffFurni(Item item, WiredHandler handler, List<Item> targetItems, int requiredCycles)
        {
            this.item = item;
            this.handler = handler;
            this.items = targetItems;
            this.delegateFunction = new UserAndItemDelegate(this.targetItem_OnUserWalksOffFurni);
            this.Delay = requiredCycles;
            foreach (Item roomItem in targetItems)
            {
                roomItem.OnUserWalksOffFurni += this.delegateFunction;
            }

            this.disposed = false;
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            if (user != null)
            {
                this.handler.ExecutePile(this.item.Coordinate, user, item);
            }

            return false;
        }

        private void targetItem_OnUserWalksOffFurni(RoomUser user, Item item)
        {
            if (this.Delay > 0)
            {
                this.handler.RequestCycle(new WiredCycle(this, user, item, this.Delay));
            }
            else
            {
                this.handler.ExecutePile(this.item.Coordinate, user, item);
            }
        }

        public void Dispose()
        {
            this.disposed = true;
            if (this.items != null)
            {
                foreach (Item roomItem in this.items)
                {
                    roomItem.OnUserWalksOffFurni -= this.delegateFunction;
                }

                this.items.Clear();
            }
            this.items = null;
            this.item = null;
            this.handler = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.Delay.ToString(), false, this.items);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if(int.TryParse(row["trigger_data"].ToString(), out int delay))
                this.Delay = delay;

            string itemslist = row["triggers_item"].ToString();

            if (itemslist == "")
                return;

            foreach (string item in itemslist.Split(';'))
            {
                Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(item));
                if (roomItem != null && !this.items.Contains(roomItem) && roomItem.Id != this.item.Id)
                {
                    roomItem.OnUserWalksOffFurni += this.delegateFunction;
                    this.items.Add(roomItem);
                }
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message10 = new ServerPacket(ServerPacketHeader.WIRED_ACTION);
            Message10.WriteBoolean(false);
            Message10.WriteInteger(10);
            Message10.WriteInteger(this.items.Count);
            foreach (Item roomItem in this.items)
            {
                Message10.WriteInteger(roomItem.Id);
            }

            Message10.WriteInteger(SpriteId);
            Message10.WriteInteger(this.item.Id);
            Message10.WriteString("");
            Message10.WriteInteger(0);
            Message10.WriteInteger(8);
            Message10.WriteInteger(0);
            Message10.WriteInteger(this.Delay);
            Message10.WriteInteger(0);
            Message10.WriteInteger(0);
            Session.SendPacket(Message10);
        }

        public bool Disposed()
        {
            return this.disposed;
        }
    }
}
