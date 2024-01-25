namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class LessThanTimer : WiredConditionBase, IWiredCondition, IWired
{
    public LessThanTimer(Item item, Room room) : base(item, room, (int)WiredConditionType.TIME_ELAPSED_LESS) => this.DefaultIntParams(new int[] { 0 });

    public bool AllowsExecution(RoomUser user, Item item)
    {
        var timeout = this.GetIntParam(0);

        var dateTime = this.RoomInstance.LastTimerReset;
        return (DateTime.Now - dateTime).TotalSeconds < timeout / 2;
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
