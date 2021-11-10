using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.GameClients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Conditions
{
    public class FurniNotStuffIs : IWiredCondition, IWired
    {
        private readonly int itemID;
        private List<Item> items;
        private bool isDisposed;

        public FurniNotStuffIs(Item item, List<Item> items)
        {
            this.itemID = item.Id;
            this.isDisposed = false;

            this.items = items;
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            if (TriggerItem == null)
            {
                return false;
            }

            foreach (Item roomItem in this.items)
            {
                if (roomItem.BaseItem == TriggerItem.BaseItem && roomItem.ExtraData == TriggerItem.ExtraData)
                {
                    return false;
                }
            }
            return true;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, string.Empty, false, this.items);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            string triggerItem = row["triggers_item"].ToString();

            if (triggerItem == "")
            {
                return;
            }

            foreach (string ItemId in triggerItem.Split(';'))
            {
                Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(ItemId));
                if (roomItem != null && !this.items.Contains(roomItem) && roomItem.Id != this.itemID)
                {
                    this.items.Add(roomItem);
                }
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_CONDITION);
            Message.WriteBoolean(false);
            Message.WriteInteger(10);
            Message.WriteInteger(this.items.Count);
            foreach (Item roomItem in this.items)
            {
                Message.WriteInteger(roomItem.Id);
            }

            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.itemID);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteBoolean(false);
            Message.WriteBoolean(true);
            Session.SendPacket(Message);
        }

        public void Dispose()
        {
            this.isDisposed = true;
            if (this.items != null)
            {
                this.items.Clear();
            }

            this.items = null;
        }

        public bool Disposed()
        {
            return this.isDisposed;
        }
    }
}
