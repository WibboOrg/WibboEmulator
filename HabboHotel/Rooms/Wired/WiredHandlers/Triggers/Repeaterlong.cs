using System.Data;
using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers
{
    public class Repeaterlong : IWired, IWiredCycleable
    {
        public int Delay { get; set; }
        private WiredHandler handler;
        private Item item;
        private bool disposed;

        public Repeaterlong(WiredHandler handler, Item item, int cyclesRequired)
        {
            this.handler = handler;
            this.Delay = cyclesRequired * 10;
            this.item = item;
            this.handler.RequestCycle(new WiredCycle(this, null, null, this.Delay));
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
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, (this.Delay / 10).ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if(int.TryParse(row["trigger_data"].ToString(), out int delay))
                this.Delay = delay * 10;
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_TRIGGER);
            Message.WriteBoolean(false);
            Message.WriteInteger(5);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.item.Id);
            Message.WriteString("");
            Message.WriteInteger(1);
            Message.WriteInteger(this.Delay / 10);
            Message.WriteInteger(0);
            Message.WriteInteger(12);
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
