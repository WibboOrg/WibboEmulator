namespace WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map.Movement;

public class Escape(Item item, Room room) : WiredActionBase(item, room, (int)WiredActionType.FLEE), IWiredEffect, IWired
{
    public override bool OnCycle(RoomUser user, Item item)
    {
        var disableAnimation = this.Room.WiredHandler.DisableAnimate(this.Item.Coordinate);

        foreach (var roomItem in this.Items.ToList())
        {
            if (this.Room.RoomItemHandling.GetItem(roomItem.Id) == null)
            {
                continue;
            }

            var roomUser = this.Room.GameMap.SquareHasUserNear(roomItem.X, roomItem.Y);
            if (roomUser != null)
            {
                this.Room.WiredHandler.TriggerCollision(roomUser, roomItem);
                continue;
            }

            roomItem.Movement = this.Room.GameMap.GetEscapeMovement(roomItem.X, roomItem.Y, roomItem.Movement);
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

                if (this.Room.RoomItemHandling.SetFloorItem(null, roomItem, newPoint.X, newPoint.Y, roomItem.Rotation, false, false, false))
                {
                    this.Room.SendPacket(new SlideObjectBundleComposer(oldX, oldY, disableAnimation ? roomItem.Z : oldZ, newPoint.X, newPoint.Y, roomItem.Z, roomItem.Id));
                }
            }
        }

        return false;
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, string.Empty, false, this.Items, this.Delay);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        this.LoadStuffIds(wiredTriggersItem);
    }
}
