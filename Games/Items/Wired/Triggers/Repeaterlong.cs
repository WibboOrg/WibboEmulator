namespace WibboEmulator.Games.Items.Wired.Triggers;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Wired;

public class Repeaterlong : WiredTriggerBase, IWired, IWiredCycleable
{
    public int DelayCycle => this.GetIntParam(0) * 10;
    public bool IsTeleport => false;

    public Repeaterlong(Item item, Room room) : base(item, room, (int)WiredTriggerType.TRIGGER_PERIODICALLY_LONG)
    {
        this.DefaultIntParams(new int[] { 0 });

        this.RoomInstance.WiredHandler.RequestCycle(new WiredCycle(this, null, null));
    }

    public bool OnCycle(RoomUser user, Item item)
    {
        this.RoomInstance.WiredHandler.ExecutePile(this.ItemInstance.Coordinate, null, null);
        return true;
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, string.Empty, false, null, this.DelayCycle / 10);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.SetIntParam(0, wiredDelay);

        if (int.TryParse(wiredTriggerData, out var delay))
        {
            this.SetIntParam(0, delay);
        }
    }
}
