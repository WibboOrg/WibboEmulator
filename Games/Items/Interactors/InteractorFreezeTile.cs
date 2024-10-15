namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public class InteractorFreezeTile : FurniInteractor
{
    public override void OnPlace(GameClient Session, Item item)
    {
    }

    public override void OnRemove(GameClient Session, Item item)
    {
    }

    public override void OnTrigger(GameClient Session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (Session == null || Session.User == null || item.InteractingUser > 0)
        {
            return;
        }

        var pName = Session.User.Username;
        var roomUserByUserId = item.Room.RoomUserManager.GetRoomUserByName(pName);
        if (roomUserByUserId == null || roomUserByUserId.CountFreezeBall == 0 || roomUserByUserId.Freezed)
        {
            return;
        }

        item.
        Room.Freeze.ThrowBall(item, roomUserByUserId);
    }

    public override void OnTick(Item item)
    {
        if (item.InteractingUser <= 0)
        {
            return;
        }

        var roomUserTarget = item.Room.RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
        if (roomUserTarget != null)
        {
            roomUserTarget.CountFreezeBall = 1;
        }

        item.ExtraData = "11000";
        item.UpdateState(false);
        item.Room.Freeze.OnFreezeTiles(item, item.FreezePowerUp, item.InteractingUser);
        item.InteractingUser = 0;
        item.InteractionCountHelper = 0;
    }
}
