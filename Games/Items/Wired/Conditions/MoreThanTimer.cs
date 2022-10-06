namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class MoreThanTimer : WiredConditionBase, IWiredCondition, IWired
{
    public MoreThanTimer(Item item, Room room) : base(item, room, (int)WiredConditionType.TIME_ELAPSED_MORE) => this.IntParams.Add(0);

    public bool AllowsExecution(RoomUser user, Item item)
    {
        var timeout = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;

        var dateTime = this.RoomInstance.lastTimerReset;
        return (DateTime.Now - this.RoomInstance.lastTimerReset).TotalSeconds > timeout / 2;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var timeout = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, timeout.ToString(), false, null);
    }

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        if (int.TryParse(row["trigger_data"].ToString(), out var timeout))
        {
            this.IntParams.Add(timeout);
        }
    }
}
