using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorDice : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
        }

        public override void OnRemove(Client Session, Item Item)
        {
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (Session == null)
            {
                return;
            }

            RoomUser roomUser = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);

            if (roomUser == null)
            {
                return;
            }

            if (Gamemap.TilesTouching(Item.X, Item.Y, roomUser.X, roomUser.Y))
            {
                if (!(Item.ExtraData != "-1"))
                {
                    return;
                }

                if (Request == -1)
                {
                    Item.ExtraData = "0";
                    Item.UpdateState();
                }
                else
                {
                    Item.ExtraData = "-1";
                    Item.UpdateState(false, true);
                    Item.ReqUpdate(4);
                }
            }
            else
            {
                roomUser.MoveTo(Item.SquareInFront);
            }
        }

        public override void OnTick(Item item)
        {
            item.ExtraData = ButterflyEnvironment.GetRandomNumber(1, 6).ToString();
            item.UpdateState();
        }
    }
}
