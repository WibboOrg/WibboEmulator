namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
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

        var movement = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
        var rotation = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;

        var newPoint = MovementUtility.HandleMovement(item.Coordinate, (MovementState)movement);
        var newRot = MovementUtility.HandleRotation(item.Rotation, (RotationState)rotation);

        if (newPoint != item.Coordinate || newRot != item.Rotation)
        {
            var oldX = item.X;
            var oldY = item.Y;
            var oldZ = item.Z;
            if (this.RoomInstance.GetRoomItemHandler().SetFloorItem(null, item, newPoint.X, newPoint.Y, newRot, false, false, newRot != item.Rotation))
            {
                this.RoomInstance.SendPacket(new SlideObjectBundleComposer(oldX, oldY, oldZ, newPoint.X, newPoint.Y, item.Z, item.Id));
            }
        }

        return;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var movement = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
        var rotation = (this.IntParams.Count > 1) ? this.IntParams[1] : 0;

        var rotAndMove = rotation + ";" + movement;
        WiredUtillity.SaveTriggerItem(dbClient, this.Id, rotAndMove, string.Empty, false, this.Items, this.Delay);
    }

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        if (int.TryParse(row["delay"].ToString(), out var delay))
        {
            this.Delay = delay;
        }

        if (int.TryParse(row["trigger_data"].ToString(), out delay))
        {
            this.Delay = delay;
        }

        var triggerData2 = row["trigger_data_2"].ToString();
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
