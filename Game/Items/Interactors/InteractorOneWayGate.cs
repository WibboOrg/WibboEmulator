using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorOneWayGate : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser == 0)
            {
                return;
            }

            RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser);
            if (roomUserByHabbo != null)
            {
                roomUserByHabbo.UnlockWalking();
            }

            Item.InteractingUser = 0;
        }

        public override void OnRemove(Client Session, Item Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser == 0)
            {
                return;
            }

            RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser);
            if (roomUserByHabbo != null)
            {
                roomUserByHabbo.UnlockWalking();
            }

            Item.InteractingUser = 0;
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights)
        {
            if (Session == null || Session.GetHabbo() == null || Item == null || Item.GetRoom() == null)
            {
                return;
            }

            RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
            {
                return;
            }

            if (roomUserByHabbo.Coordinate != Item.SquareInFront && roomUserByHabbo.CanWalk)
            {
                roomUserByHabbo.MoveTo(Item.SquareInFront);
            }
            else
            {
                if (!roomUserByHabbo.CanWalk)
                {
                    return;
                }

                if (!Item.GetRoom().GetGameMap().CanWalk(Item.SquareBehind.X, Item.SquareBehind.Y, roomUserByHabbo.AllowOverride))
                {
                    return;
                }

                RoomUser roomUserByHabboItem = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser);
                if (roomUserByHabboItem != null)
                {
                    return;
                }

                Item.InteractingUser = 0;

                Item.InteractingUser = roomUserByHabbo.HabboId;
                roomUserByHabbo.CanWalk = false;

                roomUserByHabbo.AllowOverride = true;
                roomUserByHabbo.MoveTo(Item.SquareBehind);

                Item.ExtraData = "1";
                Item.UpdateState(false, true);

                Item.ReqUpdate(1);
            }
        }

        public override void OnTick(Item item)
        {
            RoomUser roomUser3 = null;
            if (item.InteractingUser > 0)
            {
                roomUser3 = item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(item.InteractingUser);
            }

            if (roomUser3 == null)
            {
                item.InteractingUser = 0;
                return;
            }

            if (roomUser3.Coordinate == item.SquareBehind || !Gamemap.TilesTouching(item.X, item.Y, roomUser3.X, roomUser3.Y))
            {
                roomUser3.UnlockWalking();
                item.ExtraData = "0";
                item.InteractingUser = 0;
                item.UpdateState(false, true);
            }
            else
            {
                roomUser3.CanWalk = false;
                roomUser3.AllowOverride = true;
                roomUser3.MoveTo(item.SquareBehind);

                item.UpdateCounter = 1;
            }
        }
    }
}
