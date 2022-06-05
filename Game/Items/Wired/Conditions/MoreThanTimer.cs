using Wibbo.Database.Interfaces;
using Wibbo.Game.Rooms;
using Wibbo.Game.Items.Wired.Interfaces;
using System.Data;

namespace Wibbo.Game.Items.Wired.Conditions
{
    public class MoreThanTimer : WiredConditionBase, IWiredCondition, IWired
    {
        public MoreThanTimer(Item item, Room room) : base(item, room, (int)WiredConditionType.TIME_ELAPSED_MORE)
        {
            this.IntParams.Add(0);
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
            this.IntParams.Clear();

            if (int.TryParse(row["trigger_data"].ToString(), out int timeout))
                this.IntParams.Add(timeout);
        }
    }
}
