using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Items.Interactors
{
    public class InteractorFreezeTile : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
        }

        public override void OnRemove(Client Session, Item Item)
        {
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (Session == null || Session.GetUser() == null || Item.InteractingUser > 0)
            {
                return;
            }

            string pName = Session.GetUser().Username;
            RoomUser roomUserByUserId = Item.GetRoom().GetRoomUserManager().GetRoomUserByName(pName);
            if (roomUserByUserId == null || roomUserByUserId.CountFreezeBall == 0 || roomUserByUserId.Freezed)
            {
                return;
            }

            Item.GetRoom().GetFreeze().throwBall(Item, roomUserByUserId);
        }

        public override void OnTick(Item item)
        {
            if (item.InteractingUser <= 0)
            {
                return;
            }

            RoomUser roomUserTarget = item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(item.InteractingUser);
            if (roomUserTarget != null)
            {
                roomUserTarget.CountFreezeBall = 1;
            }

            item.ExtraData = "11000";
            item.UpdateState(false, true);
            item.GetRoom().GetFreeze().onFreezeTiles(item, item.FreezePowerUp, item.InteractingUser);
            item.InteractingUser = 0;
            item.InteractionCountHelper = 0;
        }
    }
}
