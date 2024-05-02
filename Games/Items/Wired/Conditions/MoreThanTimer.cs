namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class MoreThanTimer : WiredConditionBase, IWiredCondition, IWired
{
    public MoreThanTimer(Item item, Room room) : base(item, room, (int)WiredConditionType.TIME_ELAPSED_MORE) => this.DefaultIntParams(new int[] { 0 });

    public bool AllowsExecution(RoomUser user, Item item)
    {
        var timeout = this.GetIntParam(0);

        _ = this.Room.LastTimerReset;
        return (DateTime.Now - this.Room.LastTimerReset).TotalSeconds > timeout / 2;
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var timeout = this.GetIntParam(0);

        WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, timeout.ToString());
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        if (int.TryParse(wiredTriggerData, out var timeout))
        {
            this.SetIntParam(0, timeout);
        }
    }
}
