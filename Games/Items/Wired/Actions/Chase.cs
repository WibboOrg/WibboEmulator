namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map.Movement;

public class Chase : WiredActionBase, IWiredEffect, IWired
{
    public Chase(Item item, Room room) : base(item, room, (int)WiredActionType.CHASE)
    {
    }

    public override bool OnCycle(RoomUser user, Item item)
    {
        var disableAnimation = this.RoomInstance.WiredHandler.DisableAnimate(this.ItemInstance.Coordinate);

        foreach (var roomItem in this.Items.ToList())
        {
            if (this.RoomInstance.RoomItemHandling.GetItem(roomItem.Id) == null)
            {
                continue;
            }

            var roomUser = this.RoomInstance.GameMap.SquareHasUserNear(roomItem.X, roomItem.Y);
            if (roomUser != null)
            {
                this.RoomInstance.WiredHandler.TriggerCollision(roomUser, roomItem);
                continue;
            }

            roomItem.Movement = this.RoomInstance.GameMap.GetChasingMovement(roomItem.X, roomItem.Y, roomItem.Movement);
            if (roomItem.Movement == MovementState.none)
            {
                continue;
            }

            var newPoint = MovementUtility.HandleMovement(roomItem.Coordinate, roomItem.Movement);

            if (newPoint != roomItem.Coordinate)
            {
                var oldX = disableAnimation ? newPoint.X : roomItem.X;
                var oldY = disableAnimation ? newPoint.Y : roomItem.Y;
                var oldZ = roomItem.Z;

                if (this.RoomInstance.RoomItemHandling.SetFloorItem(null, roomItem, newPoint.X, newPoint.Y, roomItem.Rotation, false, false, false))
                {
                    this.RoomInstance.SendPacket(new SlideObjectBundleComposer(oldX, oldY, disableAnimation ? roomItem.Z : oldZ, newPoint.X, newPoint.Y, roomItem.Z, roomItem.Id));
                }
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
