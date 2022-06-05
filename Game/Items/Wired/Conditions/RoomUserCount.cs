using Wibbo.Database.Interfaces;
using Wibbo.Game.Rooms;
using Wibbo.Game.Items.Wired.Interfaces;
using System.Data;

namespace Wibbo.Game.Items.Wired.Conditions
{
    public class RoomUserCount : WiredConditionBase, IWiredCondition, IWired
    {
        public RoomUserCount(Item item, Room room) : base(item, room, (int)WiredConditionType.USER_COUNT_IN)
        {
            this.IntParams.Add(0);
            this.IntParams.Add(0);
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            int minUsers = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);
            int maxUsers = ((this.IntParams.Count > 1) ? this.IntParams[1] : 0);

            if (this.RoomInstance.UserCount < minUsers)
            {
                return false;
            }

            if (this.RoomInstance.UserCount > maxUsers)
            {
                return false;
            }

            return true;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            int minUsers = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0);
            int maxUsers = ((this.IntParams.Count > 1) ? this.IntParams[1] : 0);

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, minUsers + ":" + maxUsers, false, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            this.IntParams.Clear();

            string triggerData = row["trigger_data"].ToString();
            if (!triggerData.Contains(":"))
            {
                return;
            }

            string countMin = triggerData.Split(':')[0];
            string countMax = triggerData.Split(':')[1];

            if (int.TryParse(countMin, out int minUsers))
                this.IntParams.Add(minUsers);

            if (int.TryParse(countMax, out int maxUsers))
                this.IntParams.Add(maxUsers);
        }
    }
}
