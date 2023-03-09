namespace WibboEmulator.Games.Items.Wired.Conditions;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class DateRangeActive : WiredConditionBase, IWiredCondition, IWired
{
    public DateRangeActive(Item item, Room room) : base(item, room, (int)WiredConditionType.DATE_RANGE_ACTIVE) => this.DefaultIntParams(new int[] { WibboEnvironment.GetUnixTimestamp(), WibboEnvironment.GetUnixTimestamp() });

    public bool AllowsExecution(RoomUser user, Item item)
    {
        var unixNow = WibboEnvironment.GetUnixTimestamp();

        var startDate = this.GetIntParam(0);
        var endDate = this.GetIntParam(1);

        if (startDate > unixNow || endDate < unixNow)
        {
            return false;
        }

        return true;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var startDate = this.GetIntParam(0);
        var endDate = this.GetIntParam(1);

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, startDate + ":" + endDate, false, null);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        if (!wiredTriggerData.Contains(':'))
        {
            return;
        }

        if (int.TryParse(wiredTriggerData.Split(':')[0], out var startDate))
        {
            this.SetIntParam(0, startDate);
        }

        if (int.TryParse(wiredTriggerData.Split(':')[1], out var endDate))
        {
            this.SetIntParam(1, endDate);
        }
    }
}
