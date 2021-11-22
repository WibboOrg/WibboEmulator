using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers
{
    public class Repeater : IWired, IWiredCycleable
    {
        public int DelayCycle { get; set; }
        private WiredHandler handler;
        private Item item;
        private bool disposed;

        public Repeater(WiredHandler handler, Item item, int cyclesRequired)
        {
            this.handler = handler;
            this.DelayCycle = cyclesRequired;
            this.item = item;
            this.handler.RequestCycle(new WiredCycle(this, null, null, this.DelayCycle));
            this.disposed = false;
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.handler.ExecutePile(this.item.Coordinate, null, null);
            return true;
        }

        public void Dispose()
        {
            this.disposed = true;
            this.handler = null;
            this.item = null;
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

        public bool Disposed()
        {
            return this.disposed;
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
            Message.WriteInteger(0);
            Message.WriteInteger(6); //6
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }
    }
}
