using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Triggers
{
    public class WalksOffFurni : WiredTriggerBase, IWired, IWiredCycleable
    {
        private Item item;
        private WiredHandler handler;
        private List<Item> items;
        private readonly UserAndItemDelegate delegateFunction;
        public int DelayCycle { get => this.Delay; set => this.Delay = value; }
        private bool disposed;

        public WalksOffFurni(Item item, WiredHandler handler, List<Item> targetItems, List<int> stuffIds, int requiredCycles)
        {
            this.Id = item.Id;
            this.Type = (int)WiredTriggerType.AVATAR_WALKS_OFF_FURNI;
            this.StuffTypeSelectionEnabled = true;
            this.StuffTypeId = item.GetBaseItem().SpriteId;
            this.StuffIds = stuffIds;

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
            if (this.DelayCycle > 0)
            {
                this.handler.RequestCycle(new WiredCycle(this, user, item, this.DelayCycle));
            }
            else
            {
                this.handler.ExecutePile(this.item.Coordinate, user, item);
            }
        }

        public override void Dispose()
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
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.DelayCycle.ToString(), false, this.items);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int delay))
                this.DelayCycle = delay;

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

        public void OnTrigger(Client Session, int SpriteId)
        {
            this.SendWiredPacket(Session);
        }
    }
}
