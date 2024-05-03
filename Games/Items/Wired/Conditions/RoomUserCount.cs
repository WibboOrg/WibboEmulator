namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class RoomUserCount : WiredConditionBase, IWiredCondition, IWired
{
    public RoomUserCount(Item item, Room room) : base(item, room, (int)WiredConditionType.USER_COUNT_IN) => this.DefaultIntParams(0, 0);

    public bool AllowsExecution(RoomUser user, Item item)
    {
        var minUsers = this.GetIntParam(0);
        var maxUsers = this.GetIntParam(1);

        if (this.Room.UserCount < minUsers)
        {
            return false;
        }

        if (this.Room.UserCount > maxUsers)
        {
            return false;
        }

        return true;
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var minUsers = this.GetIntParam(0);
        var maxUsers = this.GetIntParam(1);

        WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, minUsers + ":" + maxUsers);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        if (!wiredTriggerData.Contains(':'))
        {
            return;
        }

        var countMin = wiredTriggerData.Split(':')[0];
        var countMax = wiredTriggerData.Split(':')[1];

        if (int.TryParse(countMin, out var minUsers))
        {
            this.SetIntParam(0, minUsers);
        }

        if (int.TryParse(countMax, out var maxUsers))
        {
            this.SetIntParam(1, maxUsers);
        }
    }
}
