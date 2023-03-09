namespace WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class ExecutePile : WiredActionBase, IWired, IWiredEffect, IWiredCycleable
{
    public ExecutePile(Item item, Room room) : base(item, room, (int)WiredActionType.CALL_ANOTHER_STACK) => this.DefaultIntParams(new int[] { 0 });

    public override bool OnCycle(RoomUser user, Item item)
    {
        var ignoreCondition = this.GetIntParam(0) == 1;

        foreach (var roomItem in this.Items.ToList())
        {
            foreach (var coord in roomItem.GetAffectedTiles)
            {
                if (coord == this.ItemInstance.Coordinate && this.RoomInstance.WiredHandler.SecurityEnabled)
                {
                    continue;
                }

                this.RoomInstance.WiredHandler.ExecutePile(coord, user, item, ignoreCondition);
            }
        }

        return false;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var ignoreCondition = this.GetIntParam(0);

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, ignoreCondition.ToString(), false, this.Items, this.Delay);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        if (int.TryParse(wiredTriggerData, out var ignoreCondition))
        {
            this.SetIntParam(0, ignoreCondition);
        }

        if (int.TryParse(wiredTriggerData2, out var delay))
        {
            this.Delay = delay;
        }

        this.LoadStuffIds(wiredTriggersItem);
    }
}
