using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Items.Interactors
{
    public class InteractorFreezeBlock : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (Session == null || Session.GetUser() == null || Item.InteractingUser > 0)
            {
                return;
            }

            string name = Session.GetUser().Username;
            RoomUser roomUserByUserId = Item.GetRoom().GetRoomUserManager().GetRoomUserByName(name);
            if (roomUserByUserId == null || roomUserByUserId.CountFreezeBall == 0 || roomUserByUserId.Freezed)
            {
                return;
            }

            Item.GetRoom().GetFreeze().throwBall(Item, roomUserByUserId);
        }

        public override void OnTick(Item item)
        {
        }
    }
}
