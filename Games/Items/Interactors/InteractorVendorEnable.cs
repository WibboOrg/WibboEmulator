namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.Map;
using WibboEmulator.Games.Rooms.PathFinding;
using WibboEmulator.Utilities;

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
        if (!(item.ExtraData != "1") || item.ItemData.VendingIds.Count < 1 || item.InteractingUser != 0 || session == null)
        {
            return;
        }

        var roomUserByUserId = item.Room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (roomUserByUserId == null)
        {
            return;
        }

        if (!GameMap.TilesTouching(roomUserByUserId.X, roomUserByUserId.Y, item.X, item.Y))
        {
            roomUserByUserId.MoveTo(item.SquareInFront);
        }
        else
        {
            item.InteractingUser = session.User.Id;
            roomUserByUserId.SetRot(Rotation.Calculate(roomUserByUserId.X, roomUserByUserId.Y, item.X, item.Y), false);
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

        var roomUserByUserId = item.Room.RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
        if (roomUserByUserId != null)
        {
            var vendingId = item.ItemData.VendingIds.GetRandomElement();
            roomUserByUserId.ApplyEffect(vendingId);
        }

        item.InteractingUser = 0;
        item.ExtraData = "0";
        item.UpdateState(false);
    }
}
