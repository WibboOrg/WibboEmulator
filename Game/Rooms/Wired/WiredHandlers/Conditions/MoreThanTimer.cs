using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Conditions
{
    public class MoreThanTimer : WiredConditionBase, IWiredCondition, IWired
    {
        public MoreThanTimer(Item item, Room room) : base(item, room, (int)WiredConditionType.TIME_ELAPSED_MORE)
        {
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
             int timeout = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);

            DateTime dateTime = this.RoomInstance.lastTimerReset;
            return (DateTime.Now - this.RoomInstance.lastTimerReset).TotalSeconds > timeout / 2;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
             int timeout = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, timeout.ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int timeout))
                this.IntParams.Add(timeout);
        }
    }
}
