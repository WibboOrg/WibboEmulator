namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.Map;

public class InteractorBanzaiTele : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item)
    {
    }

    public override void OnRemove(GameClient session, Item item)
    {
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
    }

    public override void OnTick(Item item)
    {
        if (item.InteractingUser == 0)
        {
            item.ExtraData = string.Empty;
            item.UpdateState();
            return;
        }

        item.ExtraData = "1";
        item.UpdateState();

        item.UpdateCounter = 1;

        var roomUserByUserId = item.Room.RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
        if (roomUserByUserId != null)
        {
            GameMap.TeleportToItem(roomUserByUserId, item);
            roomUserByUserId.SetRot(WibboEnvironment.GetRandomNumber(0, 7), false);
            roomUserByUserId.CanWalk = true;
        }

        item.InteractingUser = 0;
    }
}
