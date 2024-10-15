namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public class InteractorFreezeBlock : FurniInteractor
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

        var name = Session.User.Username;
        var roomUserByUserId = item.Room.RoomUserManager.GetRoomUserByName(name);
        if (roomUserByUserId == null || roomUserByUserId.CountFreezeBall == 0 || roomUserByUserId.Freezed)
        {
            return;
        }

        item.
        Room.Freeze.ThrowBall(item, roomUserByUserId);
    }

    public override void OnTick(Item item)
    {
    }
}
