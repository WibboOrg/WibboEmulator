using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Triggers
{
    public class Timer : IWired, IWiredCycleable
    {
        private Item item;
        private WiredHandler handler;
        public int DelayCycle { get; set; }
        private readonly RoomEventDelegate delegateFunction;
        private bool disposed;

        public Timer(Item item, WiredHandler handler, int cycleCount, GameManager gameManager)
        {
            this.item = item;
            this.handler = handler;
            this.DelayCycle = cycleCount;
            this.delegateFunction = new RoomEventDelegate(this.ResetTimer);
            this.handler.TrgTimer += this.delegateFunction;
            this.disposed = false;
        }

        public void ResetTimer(object sender, EventArgs e)
        {
            this.handler.RequestCycle(new WiredCycle(this, null, null, this.DelayCycle));
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.handler.ExecutePile(this.item.Coordinate, null, null);
            return false;
        }

        public void Dispose()
        {
            this.disposed = true;
            this.item = null;
            this.handler.TrgTimer -= this.delegateFunction;
            this.handler = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.DelayCycle.ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int delay))
                this.DelayCycle = delay;
        }

        public void OnTrigger(Client Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_TRIGGER);
            Message.WriteBoolean(false);
            Message.WriteInteger(5);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.item.Id);
            Message.WriteString("");
            Message.WriteInteger(1);
            Message.WriteInteger(this.DelayCycle);
            Message.WriteInteger(1);
            Message.WriteInteger(3);
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
