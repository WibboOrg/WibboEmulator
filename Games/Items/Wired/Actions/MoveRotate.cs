namespace WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map.Movement;

public class MoveRotate : WiredActionBase, IWiredEffect, IWired
{
    public MoveRotate(Item item, Room room) : base(item, room, (int)WiredActionType.MOVE_FURNI) => this.DefaultIntParams(new int[] { 0, 0 });

    public override bool OnCycle(RoomUser user, Item item)
    {
        var disableAnimation = this.RoomInstance.WiredHandler.DisableAnimate(this.ItemInstance.Coordinate);

        foreach (var roomItem in this.Items.ToList())
        {
            if (this.RoomInstance.RoomItemHandling.GetItem(roomItem.Id) == null)
            {
                continue;
            }

            var movement = this.GetIntParam(0);
            var rotation = this.GetIntParam(1);

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

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var movement = this.GetIntParam(0);
        var rotation = this.GetIntParam(1);

        var rotAndMove = rotation + ";" + movement;
        WiredUtillity.SaveInDatabase(dbClient, this.Id, rotAndMove, string.Empty, false, this.Items, this.Delay);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        if (int.TryParse(wiredTriggerData, out var delay))
        {
            this.Delay = delay;
        }

        if (wiredTriggerData2.Contains(';'))
        {
            if (int.TryParse(wiredTriggerData2.Split(';')[1], out var movement))
            {
                this.SetIntParam(0, movement);
            }

            if (int.TryParse(wiredTriggerData2.Split(';')[0], out var rotationint))
            {
                this.SetIntParam(1, rotationint);
            }
        }

        this.LoadStuffIds(wiredTriggersItem);
    }
}
