namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class HasUserInGroup : WiredConditionBase, IWiredCondition, IWired
{
    public HasUserInGroup(Item item, Room room) : base(item, room, (int)WiredConditionType.ACTOR_IS_GROUP_MEMBER)
    {
    }

    public bool AllowsExecution(RoomUser user, Item item)
    {
        if (user == null || user.IsBot || user.Client == null || user.Client.GetUser() == null)
        {
            return false;
        }

        if (this.RoomInstance.Data.Group == null)
        {
            return false;
        }

        if (!user.Client.GetUser().MyGroups.Contains(this.RoomInstance.Data.Group.Id))
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
