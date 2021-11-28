using Butterfly.Database.Interfaces;
using Butterfly.Game.Rooms;
using Butterfly.Game.Items.Wired.Interfaces;
using System;
using System.Data;

namespace Butterfly.Game.Items.Wired.Conditions
{
    public class LessThanTimer : WiredConditionBase, IWiredCondition, IWired
    {
        public LessThanTimer(Item item, Room room) : base(item, room, (int)WiredConditionType.TIME_ELAPSED_LESS)
        {
        }

        public override void LoadItems(bool inDatabase = false)
        {
            base.LoadItems(inDatabase);

            if (inDatabase)
                return;

            this.IntParams.Add(0);
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            int timeout = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);

            DateTime dateTime = this.RoomInstance.lastTimerReset;
            return (DateTime.Now - dateTime).TotalSeconds < timeout / 2;
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
