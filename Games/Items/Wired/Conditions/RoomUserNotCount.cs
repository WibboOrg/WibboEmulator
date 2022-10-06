namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class RoomUserNotCount : WiredConditionBase, IWiredCondition, IWired
{
    public RoomUserNotCount(Item item, Room room) : base(item, room, (int)WiredConditionType.NOT_USER_COUNT_IN)
    {
        this.IntParams.Add(0);
        this.IntParams.Add(0);
    }

    public bool AllowsExecution(RoomUser user, Item item)
    {
        var minUsers = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
        var maxUsers = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;

        if (this.RoomInstance.UserCount > minUsers)
        {
            return false;
        }

        if (this.RoomInstance.UserCount < maxUsers)
        {
            return false;
        }

        return true;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var minUsers = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
        var maxUsers = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, minUsers + ":" + maxUsers, false, null);
    }

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        var triggerData = row["trigger_data"].ToString();

        if (triggerData == null || !triggerData.Contains(':'))
        {
            return;
        }

        var countMin = triggerData.Split(':')[0];
        var countMax = triggerData.Split(':')[1];

        if (int.TryParse(countMin, out var minUsers))
        {
            this.IntParams.Add(minUsers);
        }

        if (int.TryParse(countMax, out var maxUsers))
        {
            this.IntParams.Add(maxUsers);
        }
    }
}
