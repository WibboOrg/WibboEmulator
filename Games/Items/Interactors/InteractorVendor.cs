namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.Map;
using WibboEmulator.Games.Rooms.PathFinding;
using WibboEmulator.Utilities;

public class InteractorVendor : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item)
    {
        item.ExtraData = "0";
        if (item.InteractingUser <= 0)
        {
            return;
        }

        item.InteractingUser = 0;
    }

    public override void OnRemove(GameClient session, Item item)
    {
        item.ExtraData = "0";
        if (item.InteractingUser <= 0)
        {
            return;
        }

        item.InteractingUser = 0;
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (!(item.ExtraData != "1") || item.ItemData.VendingIds.Count < 1 || item.InteractingUser != 0 || session == null || session.User == null)
        {
            return;
        }

        var roomUserTarget = item.Room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (roomUserTarget == null)
        {
            return;
        }

        if (!GameMap.TilesTouching(roomUserTarget.X, roomUserTarget.Y, item.X, item.Y))
        {
            roomUserTarget.MoveTo(item.SquareInFront);
        }
        else
        {
            item.InteractingUser = session.User.Id;
            roomUserTarget.SetRot(Rotation.Calculate(roomUserTarget.X, roomUserTarget.Y, item.X, item.Y), false);
            item.ReqUpdate(2);
            item.ExtraData = "1";
            item.UpdateState(false);
        }
    }

    public override void OnTick(Item item)
    {
        if (!(item.ExtraData == "1"))
        {
            return;
        }

        var roomUserTarget = item.Room.RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
        if (roomUserTarget != null)
        {
            var handitemId = item.ItemData.VendingIds.GetRandomElement();
            roomUserTarget.CarryItem(handitemId);
        }

        item.InteractingUser = 0;
        item.ExtraData = "0";
        item.UpdateState(false);
    }
}
