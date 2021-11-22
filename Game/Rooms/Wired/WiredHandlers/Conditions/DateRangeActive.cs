using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Conditions
{
    public class DateRangeActive : IWiredCondition, IWired
    {
        private int StartDate;
        private int EndDate;
        private bool isDisposed;
        private readonly int ItemId;

        public DateRangeActive(int itemId, int startData, int endDate)
        {
            this.ItemId = itemId;

            this.StartDate = startData;
            this.EndDate = endDate;
            this.isDisposed = false;
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            int unixNow = ButterflyEnvironment.GetUnixTimestamp();

            if (this.StartDate > unixNow || this.EndDate < unixNow)
            {
                return false;
            }

            return true;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.ItemId, string.Empty, this.StartDate + ":" + this.EndDate, false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            string triggerData = row["trigger_data"].ToString();
            if (!triggerData.Contains(":"))
                return;

            if (int.TryParse(triggerData.Split(':')[0], out int startDate))
                this.StartDate = startDate;

            if (int.TryParse(triggerData.Split(':')[1], out int endDate))
                this.EndDate = endDate;
        }

        public void OnTrigger(Client Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_CONDITION);
            Message.WriteBoolean(false); //stuffTypeSelectionEnabled
            Message.WriteInteger(0); //furniLimit
            Message.WriteInteger(0); //count
            //Message.WriteInteger(0); //stuffIds
            Message.WriteInteger(SpriteId); //stuffTypeId
            Message.WriteInteger(this.ItemId); //id
            Message.WriteString(""); //stringParam
            Message.WriteInteger(2); //count
            Message.WriteInteger(this.StartDate); //intParams
            Message.WriteInteger(this.EndDate); //intParams
            Message.WriteInteger(0); //stuffTypeSelectionCode

            Message.WriteInteger(24); //type
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
