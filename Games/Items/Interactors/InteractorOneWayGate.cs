namespace WibboEmulator.Games.Items.Interactors;
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

        var roomUserByUserId = item.GetRoom().RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
        if (roomUserByUserId != null)
        {
            roomUserByUserId.UnlockWalking();
        }

        item.InteractingUser = 0;
    }

    public override void OnRemove(GameClient session, Item item)
    {
        item.ExtraData = "0";

        if (item.InteractingUser == 0)
        {
            return;
        }

        var roomUserByUserId = item.GetRoom().RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
        if (roomUserByUserId != null)
        {
            roomUserByUserId.UnlockWalking();
        }

        item.InteractingUser = 0;
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (session == null || session.GetUser() == null || item == null || item.GetRoom() == null)
        {
            return;
        }

        var roomUser = item.GetRoom().RoomUserManager.GetRoomUserByUserId(session.GetUser().Id);
        if (roomUser == null)
        {
            return;
        }

        if (roomUser.Coordinate != item.SquareInFront && roomUser.CanWalk)
        {
            roomUser.MoveTo(item.SquareInFront);
        }
        else
        {
            if (!roomUser.CanWalk)
            {
                return;
            }

            if (!item.GetRoom().GameMap.CanWalk(item.SquareBehind.X, item.SquareBehind.Y, roomUser.AllowOverride))
            {
                return;
            }

            var roomUserTarget = item.GetRoom().RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
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
            item.UpdateState(false, true);

            item.ReqUpdate(1);
        }
    }

    public override void OnTick(Item item)
    {
        RoomUser roomUserTarget = null;
        if (item.InteractingUser > 0)
        {
            roomUserTarget = item.GetRoom().RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
        }

        if (roomUserTarget == null)
        {
            item.InteractingUser = 0;
            return;
        }

        if (roomUserTarget.Coordinate == item.SquareBehind || !GameMap.TilesTouching(item.X, item.Y, roomUserTarget.X, roomUserTarget.Y))
        {
            roomUserTarget.UnlockWalking();
            item.ExtraData = "0";
            item.InteractingUser = 0;
            item.UpdateState(false, true);
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
