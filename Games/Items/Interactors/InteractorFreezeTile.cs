namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public class InteractorFreezeTile : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item)
    {
    }

    public override void OnRemove(GameClient session, Item item)
    {
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (session == null || session.GetUser() == null || item.InteractingUser > 0)
        {
            return;
        }

        var pName = session.GetUser().Username;
        var roomUserByUserId = item.GetRoom().RoomUserManager.GetRoomUserByName(pName);
        if (roomUserByUserId == null || roomUserByUserId.CountFreezeBall == 0 || roomUserByUserId.Freezed)
        {
            return;
        }

        item.GetRoom().Freeze.ThrowBall(item, roomUserByUserId);
    }

    public override void OnTick(Item item)
    {
        if (item.InteractingUser <= 0)
        {
            return;
        }

        var roomUserTarget = item.GetRoom().RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
        if (roomUserTarget != null)
        {
            roomUserTarget.CountFreezeBall = 1;
        }

        item.ExtraData = "11000";
        item.UpdateState(false, true);
        item.GetRoom().Freeze.OnFreezeTiles(item, item.FreezePowerUp, item.InteractingUser);
        item.InteractingUser = 0;
        item.InteractionCountHelper = 0;
    }
}
