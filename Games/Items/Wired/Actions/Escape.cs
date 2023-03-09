namespace WibboEmulator.Games.Items.Wired.Actions;
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

            roomItem.Movement = this.RoomInstance.GameMap.GetEscapeMovement(roomItem.X, roomItem.Y, roomItem.Movement);
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

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        this.LoadStuffIds(wiredTriggersItem);
    }
}
