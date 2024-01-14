namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Wired;

public class TimerTrigger : WiredTriggerBase, IWired, IWiredCycleable
{
    public int DelayCycle => this.GetIntParam(0);
    public bool IsTeleport => false;

    private int _skipCycleCount;

    public TimerTrigger(Item item, Room room) : base(item, room, (int)WiredTriggerType.TRIGGER_ONCE)
    {
        this.RoomInstance.WiredHandler.TrgTimer += this.OnResetTimer;

        this.DefaultIntParams(new int[] { 0 });
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

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, null, this.DelayCycle);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.SetIntParam(0, wiredDelay);

        if (int.TryParse(wiredTriggerData, out var delay))
        {
            this.SetIntParam(0, delay);
        }
    }
}
