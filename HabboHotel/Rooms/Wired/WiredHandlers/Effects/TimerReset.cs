using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class TimerReset : IWiredEffect, IWired, IWiredCycleable
    {
        private Room room;
        private readonly int itemID;
        private WiredHandler handler;
        public int Delay { get; set; }
        private bool disposed;

        public TimerReset(Room room, WiredHandler handler, int delay, int itemID)
        {
            this.room = room;
            this.handler = handler;
            this.Delay = delay;
            this.disposed = false;
            this.itemID = itemID;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.Delay > 0)
            {
                this.handler.RequestCycle(new WiredCycle(this, user, TriggerItem, this.Delay));
            }
            else
            {
                this.ResetTimers();
            }
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.ResetTimers();
            return false;
        }

        public void Dispose()
        {
            this.disposed = true;
            this.room = null;
            this.handler = null;
        }

        private void ResetTimers()
        {
            this.handler.TriggerTimer();
            this.room.lastTimerReset = DateTime.Now;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, this.Delay.ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int delay))
                this.Delay = delay;
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_ACTION);
            Message.WriteBoolean(false);
            Message.WriteInteger(5);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.itemID);
            Message.WriteString(this.Delay.ToString());
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(1);
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
