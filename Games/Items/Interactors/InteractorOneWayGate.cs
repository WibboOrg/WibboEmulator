using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Items.Interactors
{
    public class InteractorOneWayGate : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser == 0)
            {
                return;
            }

            RoomUser roomUserByUserId = Item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Item.InteractingUser);
            if (roomUserByUserId != null)
            {
                roomUserByUserId.UnlockWalking();
            }

            Item.InteractingUser = 0;
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser == 0)
            {
                return;
            }

            RoomUser roomUserByUserId = Item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Item.InteractingUser);
            if (roomUserByUserId != null)
            {
                roomUserByUserId.UnlockWalking();
            }

            Item.InteractingUser = 0;
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (Session == null || Session.GetUser() == null || Item == null || Item.GetRoom() == null)
            {
                return;
            }

            RoomUser roomUser = Item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (roomUser == null)
            {
                return;
            }

            if (roomUser.Coordinate != Item.SquareInFront && roomUser.CanWalk)
            {
                roomUser.MoveTo(Item.SquareInFront);
            }
            else
            {
                if (!roomUser.CanWalk)
                {
                    return;
                }

                if (!Item.GetRoom().GetGameMap().CanWalk(Item.SquareBehind.X, Item.SquareBehind.Y, roomUser.AllowOverride))
                {
                    return;
                }

                RoomUser roomUserTarget = Item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Item.InteractingUser);
                if (roomUserTarget != null)
                {
                    return;
                }

                Item.InteractingUser = 0;

                Item.InteractingUser = roomUser.UserId;
                roomUser.CanWalk = false;

                roomUser.AllowOverride = true;
                roomUser.MoveTo(Item.SquareBehind);

                Item.ExtraData = "1";
                Item.UpdateState(false, true);

                Item.ReqUpdate(1);
            }
        }

        public override void OnTick(Item item)
        {
            RoomUser roomUserTarget = null;
            if (item.InteractingUser > 0)
            {
                roomUserTarget = item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(item.InteractingUser);
            }

            if (roomUserTarget == null)
            {
                item.InteractingUser = 0;
                return;
            }

            if (roomUserTarget.Coordinate == item.SquareBehind || !Gamemap.TilesTouching(item.X, item.Y, roomUserTarget.X, roomUserTarget.Y))
            {
                roomUserTarget.UnlockWalking();
                item.ExtraData = "0";
                item.InteractingUser = 0;
                item.UpdateState(false, true);
            }
            else
            {
                roomUserTarget.CanWalk = false;
                roomUserTarget.AllowOverride = true;
                roomUserTarget.MoveTo(item.SquareBehind);

                item.UpdateCounter = 1;
            }
        }
    }
}
