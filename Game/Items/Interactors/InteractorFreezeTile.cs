using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorFreezeTile : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
        }

        public override void OnRemove(Client Session, Item Item)
        {
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights)
        {
            if (Session == null || Session.GetHabbo() == null || Item.InteractingUser > 0)
            {
                return;
            }

            string pName = Session.GetHabbo().Username;
            RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByName(pName);
            if (roomUserByHabbo == null || roomUserByHabbo.CountFreezeBall == 0 || roomUserByHabbo.Freezed)
            {
                return;
            }

            Item.GetRoom().GetFreeze().throwBall(Item, roomUserByHabbo);
        }

        public override void OnTick(Item item)
        {
            if (item.InteractingUser <= 0)
            {
                return;
            }

            RoomUser roomUserByHabbo3 = item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(item.InteractingUser);
            if (roomUserByHabbo3 != null)
            {
                roomUserByHabbo3.CountFreezeBall = 1;
            }

            item.ExtraData = "11000";
            item.UpdateState(false, true);
            item.GetRoom().GetFreeze().onFreezeTiles(item, item.FreezePowerUp, item.InteractingUser);
            item.InteractingUser = 0;
            item.InteractionCountHelper = 0;
        }
    }
}
