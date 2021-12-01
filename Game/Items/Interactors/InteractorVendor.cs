using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Pathfinding;

namespace Butterfly.Game.Items.Interactors
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

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights)
        {
            if (!(Item.ExtraData != "1") || Item.GetBaseItem().VendingIds.Count < 1 || (Item.InteractingUser != 0 || Session == null || Session.GetHabbo() == null))
            {
                return;
            }

            RoomUser roomUserTarget = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
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
                Item.InteractingUser = Session.GetHabbo().Id;
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

            RoomUser roomUserTarget = item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(item.InteractingUser);
            if (roomUserTarget != null)
            {
                int handitemId = item.GetBaseItem().VendingIds[ButterflyEnvironment.GetRandomNumber(0, item.GetBaseItem().VendingIds.Count - 1)];
                roomUserTarget.CarryItem(handitemId);
            }

            item.InteractingUser = 0;
            item.ExtraData = "0";
            item.UpdateState(false, true);
        }
    }
}
