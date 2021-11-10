using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.GameClients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Conditions
{
    public class LessThanTimer : IWiredCondition, IWired
    {
        private int timeout;
        private Room room;
        private Item item;
        private bool isDisposed;

        public LessThanTimer(int timeout, Room room, Item item)
        {
            this.timeout = timeout;
            this.room = room;
            this.isDisposed = false;
            this.item = item;
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            DateTime dateTime = this.room.lastTimerReset;
            return (DateTime.Now - dateTime).TotalSeconds < this.timeout / 2;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.timeout.ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int timeout))
                this.timeout = timeout;
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_CONDITION);
            Message.WriteBoolean(false);
            Message.WriteInteger(5);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.item.Id);
            Message.WriteString("");
            Message.WriteInteger(1);
            Message.WriteInteger(this.timeout);
            Message.WriteInteger(1);
            Message.WriteInteger(3);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }

        public void Dispose()
        {
            this.isDisposed = true;
            this.room = null;
            this.item = null;
        }

        public bool Disposed()
        {
            return this.isDisposed;
        }
    }
}
