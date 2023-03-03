namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Wired;

public class TimerTrigger : WiredTriggerBase, IWired, IWiredCycleable
{
    public int DelayCycle => (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
    private int _skipCycleCount;

    public TimerTrigger(Item item, Room room) : base(item, room, (int)WiredTriggerType.TRIGGER_ONCE)
    {
        this.RoomInstance.WiredHandler.TrgTimer += this.OnResetTimer;

        this.IntParams.Add(0);
    }

    public void OnResetTimer(object sender, EventArgs e)
    {
        this._skipCycleCount++;
        this.RoomInstance.WiredHandler.RequestCycle(new WiredCycle(this, null, null));
    }

    public bool OnCycle(RoomUser user, Item item)
    {
        this._skipCycleCount--;

        if (this._skipCycleCount > 0)
        {
            return false;
        }

        this.RoomInstance.WiredHandler.ExecutePile(this.ItemInstance.Coordinate, null, null);
        return false;
    }

    public override void Dispose()
    {
        this.RoomInstance.WiredHandler.TrgTimer -= this.OnResetTimer;

        base.Dispose();
    }

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.DelayCycle.ToString(), false, null);

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        if (int.TryParse(row["trigger_data"].ToString(), out var delay))
        {
            this.IntParams.Add(delay);
        }
    }
}
