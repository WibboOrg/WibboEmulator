using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class TeleportToItem : IWired, IWiredCycleable, IWiredEffect
    {
        private Gamemap gamemap;
        private WiredHandler handler;
        private List<Item> items;
        public int Delay { get; set; }

        private readonly int itemID;
        private bool disposed;

        public TeleportToItem(Gamemap gamemap, WiredHandler handler, List<Item> items, int delay, int itemID)
        {
            this.gamemap = gamemap;
            this.handler = handler;
            this.items = items;
            this.Delay = delay;
            this.itemID = itemID;
            this.disposed = false;
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            if (user != null)
            {
                this.TeleportUser(user);
            }

            return false;
        }

        private void DoAnimation(RoomUser user)
        {
            user.ApplyEffect(4, true);
            user.Freeze = true;
        }
        private void ResetAnimation(RoomUser user)
        {
            user.ApplyEffect(user.CurrentEffect, true);
            if (user.FreezeEndCounter <= 0)
            {
                user.Freeze = false;
            }
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.items.Count == 0)
            {
                return;
            }

            if (user == null)
            {
                return;
            }

            this.DoAnimation(user);
            this.handler.RequestCycle(new WiredCycle(this, user, null, this.Delay));
        }

        private void TeleportUser(RoomUser user)
        {
            if (user == null)
            {
                return;
            }

            if (this.items.Count > 1)
            {
                Item roomItem = this.items[ButterflyEnvironment.GetRandomNumber(0, this.items.Count - 1)];
                if (roomItem == null)
                {
                    return;
                }

                if (roomItem.Coordinate != user.Coordinate)
                {
                    this.gamemap.TeleportToItem(user, roomItem);
                }
            }
            else if (this.items.Count == 1)
            {
                this.gamemap.TeleportToItem(user, Enumerable.First<Item>(this.items));
            }
            this.ResetAnimation(user);
            return;
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

            this.items = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, this.Delay.ToString(), false, this.items);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int delay))
                this.Delay = delay;

            string triggerItem = row["triggers_item"].ToString();

            if (triggerItem == "")
            {
                return;
            }

            foreach (string item in triggerItem.Split(';'))
            {
                Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(item));
                if (roomItem != null && !this.items.Contains(roomItem) && roomItem.Id != this.itemID)
                {
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
            Message.WriteInteger(this.itemID);
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
