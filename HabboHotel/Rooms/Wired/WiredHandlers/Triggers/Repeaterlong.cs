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
            ServerPacket Messagelong = new ServerPacket(ServerPacketHeader.WIRED_TRIGGER);
            Messagelong.WriteBoolean(false);
            Messagelong.WriteInteger(5);
            Messagelong.WriteInteger(0);
            Messagelong.WriteInteger(SpriteId);
            Messagelong.WriteInteger(this.item.Id);
            Messagelong.WriteString("");
            Messagelong.WriteInteger(1);
            Messagelong.WriteInteger(this.Delay / 10);
            Messagelong.WriteInteger(0);
            Messagelong.WriteInteger(12);
            Messagelong.WriteInteger(0);
            Messagelong.WriteInteger(0);
            Session.SendPacket(Messagelong);
        }

        public bool Disposed()
        {
            return this.disposed;
        }

    }
}
