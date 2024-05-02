namespace WibboEmulator.Games.Items.Interactors;

using System.Drawing;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map;

public class InteractorOneWayGate : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item)
    {
        item.ExtraData = "0";

        if (item.InteractingUser == 0)
        {
            return;
        }

        var roomUserByUserId = item.Room.RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
        roomUserByUserId?.UnlockWalking();

        item.InteractingUser = 0;
    }

    public override void OnRemove(GameClient session, Item item)
    {
        item.ExtraData = "0";

        if (item.InteractingUser == 0)
        {
            return;
        }

        var roomUserByUserId = item.Room.RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
        roomUserByUserId?.UnlockWalking();

        item.InteractingUser = 0;
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (session == null || session.User == null || item == null || item.Room == null)
        {
            return;
        }

        var roomUser = item.Room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (roomUser == null)
        {
            return;
        }

        var userCoords = new Point(roomUser.SetX, roomUser.SetY);

        if (userCoords != item.SquareInFront && roomUser.CanWalk)
        {
            roomUser.MoveTo(item.SquareInFront);
        }
        else
        {
            if (!roomUser.CanWalk)
            {
                return;
            }

            if (!item.Room.GameMap.CanWalk(item.SquareBehind.X, item.SquareBehind.Y, roomUser.AllowOverride))
            {
                return;
            }

            var roomUserTarget = item.Room.RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
            if (roomUserTarget != null)
            {
                return;
            }

            item.InteractingUser = 0;

            item.InteractingUser = roomUser.UserId;
            roomUser.CanWalk = false;

            roomUser.AllowOverride = true;
            roomUser.MoveTo(item.SquareBehind);

            item.ExtraData = "1";
            item.UpdateState(false);

            item.ReqUpdate(1);
        }
    }

    public override void OnTick(Item item)
    {
        RoomUser roomUserTarget = null;
        if (item.InteractingUser > 0)
        {
            roomUserTarget = item.Room.RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
        }

        if (roomUserTarget == null)
        {
            item.InteractingUser = 0;
            return;
        }

        var userCoords = new Point(roomUserTarget.SetX, roomUserTarget.SetY);

        if (userCoords == item.SquareBehind || roomUserTarget.Coordinate == item.SquareBehind || !GameMap.TilesTouching(item.X, item.Y, roomUserTarget.X, roomUserTarget.Y))
        {
            roomUserTarget.UnlockWalking();
            item.ExtraData = "0";
            item.InteractingUser = 0;
            item.UpdateState(false);
        }
        else
        {
            roomUserTarget.CanWalk = false;
            roomUserTarget.AllowOverride = true;
            roomUserTarget.MoveTo(item.SquareBehind);

            item.UpdateCounter = 1;
        }
    }
}
