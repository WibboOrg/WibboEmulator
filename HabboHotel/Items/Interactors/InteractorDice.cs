using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.HabboHotel.Items.Interactors
{
    public class InteractorDice : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
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

            if (Gamemap.TilesTouching(Item.GetX, Item.GetY, roomUser.X, roomUser.Y))
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
    }
}
