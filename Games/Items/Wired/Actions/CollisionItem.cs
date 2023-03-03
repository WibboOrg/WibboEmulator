namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class CollisionItem : WiredActionBase, IWiredEffect, IWired
{
    public CollisionItem(Item item, Room room) : base(item, room, (int)WiredActionType.CHASE)
    {
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        foreach (var roomItem in this.Items.ToList())
        {
            if (this.RoomInstance.RoomItemHandling.GetItem(roomItem.Id) == null)
            {
                continue;
            }

            this.RoomInstance.WiredHandler.TriggerCollision(null, roomItem);
        }

        return false;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var isAllUser = this.IntParams.Count > 0 ? this.IntParams[0] : 0;

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, this.Items, this.Delay);
    }

    public void LoadFromDatabase(DataRow row)
    {
        if (int.TryParse(row["delay"].ToString(), out var delay))
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
