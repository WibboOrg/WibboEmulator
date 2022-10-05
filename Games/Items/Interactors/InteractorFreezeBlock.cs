namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public class InteractorFreezeBlock : FurniInteractor
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

        var name = session.GetUser().Username;
        var roomUserByUserId = item.GetRoom().GetRoomUserManager().GetRoomUserByName(name);
        if (roomUserByUserId == null || roomUserByUserId.CountFreezeBall == 0 || roomUserByUserId.Freezed)
        {
            return;
        }

        item.GetRoom().GetFreeze().ThrowBall(item, roomUserByUserId);
    }

    public override void OnTick(Item item)
    {
    }
}
