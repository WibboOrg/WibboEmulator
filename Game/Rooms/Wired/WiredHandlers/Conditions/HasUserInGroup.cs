using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Conditions
{
    public class HasUserInGroup : WiredConditionBase, IWiredCondition, IWired
    {
        public HasUserInGroup(Item item, Room room) : base(item, room, (int)WiredConditionType.ACTOR_IS_GROUP_MEMBER)
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

            if (!user.GetClient().GetHabbo().MyGroups.Contains(this.RoomInstance.RoomData.Group.Id))
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
