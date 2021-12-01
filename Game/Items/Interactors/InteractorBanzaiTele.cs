using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Items.Interactors
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

            RoomUser roomUserByHabbo = item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(item.InteractingUser);
            if (roomUserByHabbo != null)
            {
                item.GetRoom().GetGameMap().TeleportToItem(roomUserByHabbo, item);
                roomUserByHabbo.SetRot(ButterflyEnvironment.GetRandomNumber(0, 7), false);
                roomUserByHabbo.CanWalk = true;
            }

            item.InteractingUser = 0;
        }
    }
}
