using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Pathfinding;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorVendorEnable : FurniInteractor
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
            if (!(Item.ExtraData != "1") || Item.GetBaseItem().VendingIds.Count < 1 || (Item.InteractingUser != 0 || Session == null))
            {
                return;
            }

            RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
            {
                return;
            }

            if (!Gamemap.TilesTouching(roomUserByHabbo.X, roomUserByHabbo.Y, Item.X, Item.Y))
            {
                roomUserByHabbo.MoveTo(Item.SquareInFront);
            }
            else
            {
                Item.InteractingUser = Session.GetHabbo().Id;
                roomUserByHabbo.SetRot(Rotation.Calculate(roomUserByHabbo.X, roomUserByHabbo.Y, Item.X, Item.Y), false);
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

            RoomUser roomUserByHabboEnable = item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(item.InteractingUser);
            if (roomUserByHabboEnable != null)
            {
                int vendingId = item.GetBaseItem().VendingIds[ButterflyEnvironment.GetRandomNumber(0, item.GetBaseItem().VendingIds.Count - 1)];
                roomUserByHabboEnable.ApplyEffect(vendingId);
            }

            item.InteractingUser = 0;
            item.ExtraData = "0";
            item.UpdateState(false, true);
        }
    }
}
