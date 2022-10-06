namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class HasUserNotInGroup : WiredConditionBase, IWiredCondition, IWired
{
    public HasUserNotInGroup(Item item, Room room) : base(item, room, (int)WiredConditionType.NOT_ACTOR_IN_GROUP)
    {
    }

    public bool AllowsExecution(RoomUser user, Item item)
    {
        if (user == null || user.IsBot || user.GetClient() == null || user.GetClient().GetUser() == null)
        {
            return false;
        }

        if (this.RoomInstance.RoomData.Group == null)
        {
            return false;
        }

        if (user.GetClient().GetUser().MyGroups.Contains(this.RoomInstance.RoomData.Group.Id))
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
