using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Items.Interactors
{
    public class InteractorBanzaiTele : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
        }

        public override void OnRemove(Client Session, Item Item)
        {
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights, bool Reverse)
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

            RoomUser roomUserByUserId = item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(item.InteractingUser);
            if (roomUserByUserId != null)
            {
                item.GetRoom().GetGameMap().TeleportToItem(roomUserByUserId, item);
                roomUserByUserId.SetRot(WibboEnvironment.GetRandomNumber(0, 7), false);
                roomUserByUserId.CanWalk = true;
            }

            item.InteractingUser = 0;
        }
    }
}
