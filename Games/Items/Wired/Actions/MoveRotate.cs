namespace WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map.Movement;

public class MoveRotate : WiredActionBase, IWiredEffect, IWired
{
    public MoveRotate(Item item, Room room) : base(item, room, (int)WiredActionType.MOVE_FURNI)
    {
        this.IntParams.Add(0);
        this.IntParams.Add(0);
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

            var movement = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
            var rotation = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;

            var newPoint = MovementUtility.HandleMovement(roomItem.Coordinate, (MovementState)movement);
            var newRot = MovementUtility.HandleRotation(roomItem.Rotation, (RotationState)rotation);

            if (newPoint != roomItem.Coordinate || newRot != roomItem.Rotation)
            {
                var oldX = disableAnimation ? newPoint.X : roomItem.X;
                var oldY = disableAnimation ? newPoint.Y : roomItem.Y;
                var oldZ = roomItem.Z;
                if (this.RoomInstance.RoomItemHandling.SetFloorItem(null, roomItem, newPoint.X, newPoint.Y, newRot, false, false, newRot != roomItem.Rotation))
                {
                    this.RoomInstance.SendPacket(new SlideObjectBundleComposer(oldX, oldY, disableAnimation ? roomItem.Z : oldZ, newPoint.X, newPoint.Y, roomItem.Z, roomItem.Id));
                }
            }
        }

        return false;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var movement = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
        var rotation = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;

        var rotAndMove = rotation + ";" + movement;
        WiredUtillity.SaveTriggerItem(dbClient, this.Id, rotAndMove, string.Empty, false, this.Items, this.Delay);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.IntParams.Clear();

        this.Delay = wiredDelay;

        if (int.TryParse(wiredTriggerData, out var delay))
        {
            this.Delay = delay;
        }

        var triggerData2 = wiredTriggerData2;
        if (triggerData2 != null && triggerData2.Contains(';'))
        {
            if (int.TryParse(triggerData2.Split(';')[1], out var movement))
            {
                this.IntParams.Add(movement);
            }

            if (int.TryParse(triggerData2.Split(';')[0], out var rotationint))
            {
                this.IntParams.Add(rotationint);
            }
        }

        var triggerItems = wiredTriggersItem;
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
