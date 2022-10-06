namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class DateRangeActive : WiredConditionBase, IWiredCondition, IWired
{
    public DateRangeActive(Item item, Room room) : base(item, room, (int)WiredConditionType.DATE_RANGE_ACTIVE)
    {
        this.IntParams.Add(WibboEnvironment.GetUnixTimestamp());
        this.IntParams.Add(WibboEnvironment.GetUnixTimestamp());
    }

    public bool AllowsExecution(RoomUser user, Item item)
    {
        var unixNow = WibboEnvironment.GetUnixTimestamp();

        var startDate = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
        var endDate = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;

        if (startDate > unixNow || endDate < unixNow)
        {
            return false;
        }

        return true;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var startDate = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
        var endDate = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, startDate + ":" + endDate, false, null);
    }

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        var triggerData = row["trigger_data"].ToString();
        if (triggerData == null || !triggerData.Contains(':'))
        {
            return;
        }

        if (int.TryParse(triggerData.Split(':')[0], out var startDate))
        {
            this.IntParams.Add(startDate);
        }

        if (int.TryParse(triggerData.Split(':')[1], out var endDate))
        {
            this.IntParams.Add(endDate);
        }
    }
}
