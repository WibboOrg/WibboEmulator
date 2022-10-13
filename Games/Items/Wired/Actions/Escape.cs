namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map.Movement;

public class Escape : WiredActionBase, IWiredEffect, IWired
{
    public Escape(Item item, Room room) : base(item, room, (int)WiredActionType.FLEE)
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
        if (this.RoomInstance.RoomItemHandling.GetItem(item.Id) == null)
        {
            return;
        }

        var roomUser = this.RoomInstance.GameMap.SquareHasUserNear(item.X, item.Y);
        if (roomUser != null)
        {
            this.RoomInstance.WiredHandler.TriggerCollision(roomUser, item);
            return;
        }

        item.Movement = this.RoomInstance.GameMap.GetEscapeMovement(item.X, item.Y, item.Movement);
        if (item.Movement == MovementState.none)
        {
            return;
        }

        var newPoint = MovementUtility.HandleMovement(item.Coordinate, item.Movement);

        if (newPoint != item.Coordinate)
        {
            var oldX = item.X;
            var oldY = item.Y;
            var oldZ = item.Z;
            if (this.RoomInstance.RoomItemHandling.SetFloorItem(null, item, newPoint.X, newPoint.Y, item.Rotation, false, false, false))
            {
                this.RoomInstance.SendPacket(new SlideObjectBundleComposer(oldX, oldY, oldZ, newPoint.X, newPoint.Y, item.Z, item.Id));
            }
        }
        return;
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
