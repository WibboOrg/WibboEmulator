using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.PathFinding;

namespace WibboEmulator.Games.Items.Interactors
{
    public class InteractorVendor : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
            Item.ExtraData = "0";
            if (Item.InteractingUser <= 0)
            {
                return;
            }

            Item.InteractingUser = 0;
        }

        public override void OnRemove(Client Session, Item Item)
        {
            Item.ExtraData = "0";
            if (Item.InteractingUser <= 0)
            {
                return;
            }

            Item.InteractingUser = 0;
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (!(Item.ExtraData != "1") || Item.GetBaseItem().VendingIds.Count < 1 || (Item.InteractingUser != 0 || Session == null || Session.GetUser() == null))
            {
                return;
            }

            RoomUser roomUserTarget = Item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (roomUserTarget == null)
            {
                return;
            }

            if (!Gamemap.TilesTouching(roomUserTarget.X, roomUserTarget.Y, Item.X, Item.Y))
            {
                roomUserTarget.MoveTo(Item.SquareInFront);
            }
            else
            {
                Item.InteractingUser = Session.GetUser().Id;
                roomUserTarget.SetRot(Rotation.Calculate(roomUserTarget.X, roomUserTarget.Y, Item.X, Item.Y), false);
                Item.ReqUpdate(2);
                Item.ExtraData = "1";
                Item.UpdateState(false, true);
            }
        }

        public override void OnTick(Item item)
        {
            if (!(item.ExtraData == "1"))
            {
                return;
            }

            RoomUser roomUserTarget = item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(item.InteractingUser);
            if (roomUserTarget != null)
            {
                int handitemId = item.GetBaseItem().VendingIds[WibboEnvironment.GetRandomNumber(0, item.GetBaseItem().VendingIds.Count - 1)];
                roomUserTarget.CarryItem(handitemId);
            }

            item.InteractingUser = 0;
            item.ExtraData = "0";
            item.UpdateState(false, true);
        }
    }
}
