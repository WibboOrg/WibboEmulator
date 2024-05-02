namespace WibboEmulator.Games.Items.Interactors;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;

public class InteractorPuzzleBox : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item)
    {
    }

    public override void OnRemove(GameClient session, Item item)
    {
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (session == null)
        {
            return;
        }

        var roomUserByUserId = item.Room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        var point1 = new Point(item.Coordinate.X + 1, item.Coordinate.Y);
        var point2 = new Point(item.Coordinate.X - 1, item.Coordinate.Y);
        var point3 = new Point(item.Coordinate.X, item.Coordinate.Y + 1);
        var point4 = new Point(item.Coordinate.X, item.Coordinate.Y - 1);

        if (roomUserByUserId == null)
        {
            return;
        }

        if (roomUserByUserId.Coordinate != point1 && roomUserByUserId.Coordinate != point2 && roomUserByUserId.Coordinate != point3 && roomUserByUserId.Coordinate != point4)
        {
            if (!roomUserByUserId.CanWalk)
            {
                return;
            }

            roomUserByUserId.MoveTo(item.SquareInFront);
        }
        else
        {
            var newX = item.Coordinate.X;
            var newY = item.Coordinate.Y;
            if (roomUserByUserId.Coordinate == point1)
            {
                newX = item.Coordinate.X - 1;
                newY = item.Coordinate.Y;
            }
            else if (roomUserByUserId.Coordinate == point2)
            {
                newX = item.Coordinate.X + 1;
                newY = item.Coordinate.Y;
            }
            else if (roomUserByUserId.Coordinate == point3)
            {
                newX = item.Coordinate.X;
                newY = item.Coordinate.Y - 1;
            }
            else if (roomUserByUserId.Coordinate == point4)
            {
                newX = item.Coordinate.X;
                newY = item.Coordinate.Y + 1;
            }

            if (!item.Room.GameMap.CanStackItem(newX, newY))
            {
                return;
            }

            var oldX = item.X;
            var oldY = item.Y;
            var oldZ = item.Z;
            var newZ = item.Room.GameMap.SqAbsoluteHeight(newX, newY);
            if (item.Room.RoomItemHandling.SetFloorItem(roomUserByUserId.Client, item, newX, newY, item.Rotation, false, false, false))
            {
                item.                Room.SendPacket(new SlideObjectBundleComposer(oldX, oldY, oldZ, newX, newY, newZ, item.Id));
            }
        }
    }

    public override void OnTick(Item item)
    {
    }
}
