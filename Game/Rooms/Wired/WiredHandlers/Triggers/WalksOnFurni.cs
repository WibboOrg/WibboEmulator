using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Triggers
{
    public class WalksOnFurni : WiredTriggerBase, IWired, IWiredCycleable
    {
        private Item item;
        private WiredHandler handler;
        private List<Item> items;
        private readonly UserAndItemDelegate delegateFunction;
        public int DelayCycle { get => this.Delay; set => this.Delay = value; }

        public WalksOnFurni(Item item, WiredHandler handler, List<Item> targetItems, List<int> stuffIds, int requiredCycles)
        {
            this.Id = item.Id;
            this.Type = (int)WiredTriggerType.AVATAR_WALKS_ON_FURNI;
            this.StuffTypeSelectionEnabled = true;
            this.StuffTypeId = item.GetBaseItem().SpriteId;
            this.StuffIds = stuffIds;

            this.item = item;
            this.handler = handler;
            this.items = targetItems;
            this.delegateFunction = new UserAndItemDelegate(this.OnUserWalksOnFurni);
            this.DelayCycle = requiredCycles;
            foreach (Item roomItem in targetItems)
            {
                roomItem.OnUserWalksOnFurni += this.delegateFunction;
            }
        }

        public bool OnCycle(RoomUser User, Item Item)
        {
            if (User != null)
            {
                this.handler.ExecutePile(this.item.Coordinate, User, Item);
            }

            return false;
        }

        private void OnUserWalksOnFurni(RoomUser user, Item item)
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
            this.isDisposed = true;
            if (this.items != null)
            {
                foreach (Item roomItem in this.items)
                {
                    roomItem.OnUserWalksOnFurni -= this.delegateFunction;
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

            string triggerItem = row["triggers_item"].ToString();

            if (triggerItem == "")
                return;

            foreach (string id in triggerItem.Split(';'))
            {
                int.TryParse(id, out int itemId);

                if (itemId == 0)
                {
                    continue;
                }

                Item roomItem = insideRoom.GetRoomItemHandler().GetItem(itemId);
                if (roomItem != null && !this.items.Contains(roomItem) && roomItem.Id != this.item.Id)
                {
                    roomItem.OnUserWalksOnFurni += new UserAndItemDelegate(this.OnUserWalksOnFurni);
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
