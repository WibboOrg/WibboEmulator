namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.Map;
using WibboEmulator.Games.Rooms.PathFinding;

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
        if (!(item.ExtraData != "1") || item.GetBaseItem().VendingIds.Count < 1 || item.InteractingUser != 0 || session == null || session.GetUser() == null)
        {
            return;
        }

        var roomUserTarget = item.GetRoom().RoomUserManager.GetRoomUserByUserId(session.GetUser().Id);
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
            item.InteractingUser = session.GetUser().Id;
            roomUserTarget.SetRot(Rotation.Calculate(roomUserTarget.X, roomUserTarget.Y, item.X, item.Y), false);
            item.ReqUpdate(2);
            item.ExtraData = "1";
            item.UpdateState(false, true);
        }
    }

    public override void OnTick(Item item)
    {
        if (!(item.ExtraData == "1"))
        {
            return;
        }

        var roomUserTarget = item.GetRoom().RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
        if (roomUserTarget != null)
        {
            var handitemId = item.GetBaseItem().VendingIds[WibboEnvironment.GetRandomNumber(0, item.GetBaseItem().VendingIds.Count - 1)];
            roomUserTarget.CarryItem(handitemId);
        }

        item.InteractingUser = 0;
        item.ExtraData = "0";
        item.UpdateState(false, true);
    }
}
