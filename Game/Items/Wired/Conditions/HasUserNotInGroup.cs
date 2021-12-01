using Butterfly.Database.Interfaces;
using Butterfly.Game.Rooms;
using Butterfly.Game.Items.Wired.Interfaces;
using System.Data;

namespace Butterfly.Game.Items.Wired.Conditions
{
    public class HasUserNotInGroup : WiredConditionBase, IWiredCondition, IWired
    {
        public HasUserNotInGroup(Item item, Room room) : base(item, room, (int)WiredConditionType.NOT_ACTOR_IN_GROUP)
        {
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            if (user == null || user.IsBot || user.GetClient() == null || user.GetClient().GetHabbo() == null)
            {
                return false;
            }

            if (this.RoomInstance.RoomData.Group == null)
            {
                return false;
            }

            if (user.GetClient().GetHabbo().MyGroups.Contains(this.RoomInstance.RoomData.Group.Id))
            {
                return false;
            }

            return true;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
        }

        public void LoadFromDatabase(DataRow row)
        {
        }
    }
}
