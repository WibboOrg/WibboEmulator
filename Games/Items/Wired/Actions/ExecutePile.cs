namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class ExecutePile : WiredActionBase, IWired, IWiredEffect, IWiredCycleable
{
    public ExecutePile(Item item, Room room) : base(item, room, (int)WiredActionType.CALL_ANOTHER_STACK)
    {

    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        var ignoreCondition = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;

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

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, this.Items, this.Delay);

    public void LoadFromDatabase(DataRow row)
    {
        if (int.TryParse(row["delay"].ToString(), out var delay))
        {
            this.Delay = delay;
        }

        if (int.TryParse(row["trigger_data_2"].ToString(), out delay))
        {
            this.Delay = delay;
        }

        var triggerItems = row["triggers_item"].ToString();

        if (triggerItems is null or "")
        {
            return;
        }

        foreach (var itemId in triggerItems.Split(';'))
        {
            if (!int.TryParse(itemId, out var id))
            {
                continue;
            }

            if (!this.StuffIds.Contains(id))
            {
                this.StuffIds.Add(id);
            }
        }
    }
}
