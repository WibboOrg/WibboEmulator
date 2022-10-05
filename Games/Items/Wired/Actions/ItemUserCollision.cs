namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class ItemUserCollision : WiredActionBase, IWiredEffect, IWired
{
    public ItemUserCollision(Item item, Room room) : base(item, room, (int)WiredActionType.FLEE)
    {
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        foreach (var roomItem in this.Items.ToList())
        {
            this.HandleMovement(roomItem);
        }

        return false;
    }

    private void HandleMovement(Item item)
    {
        if (this.RoomInstance.GetRoomItemHandler().GetItem(item.Id) == null)
        {
            return;
        }

        foreach (var Coord in item.GetCoords)
        {
            var roomUser = this.RoomInstance.GetRoomUserManager().GetUserForSquare(Coord.X, Coord.Y);
            if (roomUser != null)
            {
                this.RoomInstance.GetWiredHandler().TriggerCollision(roomUser, item);
                return;
            }
        }
    }

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, this.Items, this.Delay);

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
