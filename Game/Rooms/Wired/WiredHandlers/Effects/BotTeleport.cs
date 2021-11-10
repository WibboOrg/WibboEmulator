using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.GameClients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Effects
{
    public class BotTeleport : IWired, IWiredEffect
    {
        private WiredHandler handler;
        private readonly int itemID;
        private string NameBot;
        private List<Item> items;
        private Gamemap gamemap;

        public BotTeleport(string namebot, List<Item> items, Gamemap gamemap, WiredHandler handler, int itemID)
        {
            this.itemID = itemID;
            this.handler = handler;
            this.NameBot = namebot;
            this.items = items;
            this.gamemap = gamemap;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.NameBot == "" || this.items.Count == 0)
            {
                return;
            }

            Room room = this.handler.GetRoom();
            RoomUser Bot = room.GetRoomUserManager().GetBotOrPetByName(this.NameBot);
            if (Bot == null)
            {
                return;
            }

            Item roomItem = this.items[ButterflyEnvironment.GetRandomNumber(0, this.items.Count - 1)];
            if (roomItem == null)
            {
                return;
            }

            if (roomItem.Coordinate != Bot.Coordinate)
            {
                this.gamemap.TeleportToItem(Bot, roomItem);
            }
        }

        public void Dispose()
        {
            this.handler = null;
            this.gamemap = null;
            if (this.items != null)
            {
                this.items.Clear();
            }

            this.items = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, this.NameBot, false, this.items);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            this.NameBot = row["trigger_data"].ToString();

            string triggerItem = row["triggers_item"].ToString();

            if (triggerItem == "")
                return;

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
            Message.WriteString(this.NameBot);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(21); //7
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }
    }
}
