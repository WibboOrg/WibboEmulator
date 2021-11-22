using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Conditions
{
    public class DateRangeActive : WiredConditionBase, IWiredCondition, IWired
    {
        public DateRangeActive(Item item, List<int> intParams) : base()
        {
            this.Id = item.Id;
            this.Type = (int)WiredConditionType.DATE_RANGE_ACTIVE;
            this.IntParams = intParams;
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            int unixNow = ButterflyEnvironment.GetUnixTimestamp();

            int startDate = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
            int endDate = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;

            if (startDate > unixNow || endDate < unixNow)
            {
                return false;
            }

            return true;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            int startDate = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
            int endDate = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, startDate + ":" + endDate, false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            string triggerData = row["trigger_data"].ToString();
            if (!triggerData.Contains(":"))
                return;

            if (int.TryParse(triggerData.Split(':')[0], out int startDate))
                this.IntParams.Add(startDate);

            if (int.TryParse(triggerData.Split(':')[1], out int endDate))
                this.IntParams.Add(endDate);
        }

        public void OnTrigger(Client Session, int spriteId)
        {
            this.OnTrigger(Session);
        }
    }
}
