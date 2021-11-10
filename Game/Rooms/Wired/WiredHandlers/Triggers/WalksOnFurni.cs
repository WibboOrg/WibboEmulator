using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.GameClients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Triggers
{
    public class WalksOnFurni : IWired, IWiredCycleable
    {
        private Item item;
        private WiredHandler handler;
        private List<Item> items;
        private readonly UserAndItemDelegate delegateFunction;
        public int Delay { get; set; }
        private bool disposed;

        public WalksOnFurni(Item item, WiredHandler handler, List<Item> targetItems, int requiredCycles)
        {
            this.item = item;
            this.handler = handler;
            this.items = targetItems;
            this.delegateFunction = new UserAndItemDelegate(this.targetItem_OnUserWalksOnFurni);
            this.Delay = requiredCycles;
            foreach (Item roomItem in targetItems)
            {
                roomItem.OnUserWalksOnFurni += this.delegateFunction;
            }

            this.disposed = false;
        }

        public bool OnCycle(RoomUser User, Item Item)
        {
            if (User != null)
            {
                this.handler.ExecutePile(this.item.Coordinate, User, Item);
            }

            return false;
        }

        private void targetItem_OnUserWalksOnFurni(RoomUser user, Item item)
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
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.Delay.ToString(), false, this.items);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int delay))
                this.Delay = delay;

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
                    roomItem.OnUserWalksOnFurni += new UserAndItemDelegate(this.targetItem_OnUserWalksOnFurni);
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
