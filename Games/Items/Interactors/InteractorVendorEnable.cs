namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.PathFinding;

public class InteractorVendorEnable : FurniInteractor
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
        if (!(item.ExtraData != "1") || item.GetBaseItem().VendingIds.Count < 1 || item.InteractingUser != 0 || session == null)
        {
            return;
        }

        var roomUserByUserId = item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
        if (roomUserByUserId == null)
        {
            return;
        }

        if (!Gamemap.TilesTouching(roomUserByUserId.X, roomUserByUserId.Y, item.X, item.Y))
        {
            roomUserByUserId.MoveTo(item.SquareInFront);
        }
        else
        {
            item.InteractingUser = session.GetUser().Id;
            roomUserByUserId.SetRot(Rotation.Calculate(roomUserByUserId.X, roomUserByUserId.Y, item.X, item.Y), false);
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

        var roomUserByUserId = item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(item.InteractingUser);
        if (roomUserByUserId != null)
        {
            var vendingId = item.GetBaseItem().VendingIds[WibboEnvironment.GetRandomNumber(0, item.GetBaseItem().VendingIds.Count - 1)];
            roomUserByUserId.ApplyEffect(vendingId);
        }

        item.InteractingUser = 0;
        item.ExtraData = "0";
        item.UpdateState(false, true);
    }
}
