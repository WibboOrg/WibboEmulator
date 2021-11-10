using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.GameClients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Conditions
{
    public class RoomUserCount : IWiredCondition, IWired
    {
        private int RoomCountMin;
        private int RoomCountMax;
        private bool isDisposed;
        private readonly Item Item;

        public RoomUserCount(Item item, int CountMin, int CountMax)
        {
            if (CountMin < 1)
            {
                CountMin = 1;
            }

            if (CountMin > 50)
            {
                CountMin = 50;
            }

            if (CountMax > 50)
            {
                CountMax = 50;
            }

            if (CountMax < 1)
            {
                CountMax = 1;
            }

            this.Item = item;

            this.RoomCountMin = CountMin;
            this.RoomCountMax = CountMax;
            this.isDisposed = false;
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            if (this.Item.GetRoom().UserCount < this.RoomCountMin)
            {
                return false;
            }

            if (this.Item.GetRoom().UserCount > this.RoomCountMax)
            {
                return false;
            }

            return true;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            string SaveRoomCount = this.RoomCountMin + ":" + this.RoomCountMax;
            WiredUtillity.SaveTriggerItem(dbClient, this.Item.Id, string.Empty, SaveRoomCount, false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            string triggerData = row["trigger_data"].ToString();
            if (!triggerData.Contains(":"))
            {
                return;
            }

            string CountMin = triggerData.Split(':')[0];
            string CountMax = triggerData.Split(':')[1];

            if (int.TryParse(CountMax, out int numberMax))
                this.RoomCountMax = numberMax;

            if (int.TryParse(CountMin, out int numberMin))
                this.RoomCountMin = numberMin;
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_CONDITION);
            Message.WriteBoolean(false);
            Message.WriteInteger(5);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.Item.Id);
            Message.WriteString("");
            Message.WriteInteger(2);
            Message.WriteInteger(this.RoomCountMin);
            Message.WriteInteger(this.RoomCountMax);
            Message.WriteInteger(0);

            Message.WriteInteger(16);
            Session.SendPacket(Message);
        }

        public void Dispose()
        {
            this.isDisposed = true;
        }

        public bool Disposed()
        {
            return this.isDisposed;
        }
    }
}
