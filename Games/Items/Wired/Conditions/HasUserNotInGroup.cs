namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
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
        if (user == null || user.IsBot || user.Client == null || user.Client.User == null)
        {
            return false;
        }

        if (this.RoomInstance.RoomData.Group == null)
        {
            return false;
        }

        if (user.Client.User.MyGroups.Contains(this.RoomInstance.RoomData.Group.Id))
        {
            return false;
        }

        return true;
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
    }
}
