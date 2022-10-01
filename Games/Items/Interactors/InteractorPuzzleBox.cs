using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using System.Drawing;

namespace WibboEmulator.Games.Items.Interactors
{
    public class InteractorPuzzleBox : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (Session == null)
            {
                return;
            }

            RoomUser roomUserByUserId = Item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            Point point1 = new Point(Item.Coordinate.X + 1, Item.Coordinate.Y);
            Point point2 = new Point(Item.Coordinate.X - 1, Item.Coordinate.Y);
            Point point3 = new Point(Item.Coordinate.X, Item.Coordinate.Y + 1);
            Point point4 = new Point(Item.Coordinate.X, Item.Coordinate.Y - 1);

            if (roomUserByUserId == null)
            {
                return;
            }

            if (roomUserByUserId.Coordinate != point1 && roomUserByUserId.Coordinate != point2 && (roomUserByUserId.Coordinate != point3 && roomUserByUserId.Coordinate != point4))
            {
                if (!roomUserByUserId.CanWalk)
                {
                    return;
                }

                roomUserByUserId.MoveTo(Item.SquareInFront);
            }
            else
            {
                int newX = Item.Coordinate.X;
                int newY = Item.Coordinate.Y;
                if (roomUserByUserId.Coordinate == point1)
                {
                    newX = Item.Coordinate.X - 1;
                    newY = Item.Coordinate.Y;
                }
                else if (roomUserByUserId.Coordinate == point2)
                {
                    newX = Item.Coordinate.X + 1;
                    newY = Item.Coordinate.Y;
                }
                else if (roomUserByUserId.Coordinate == point3)
                {
                    newX = Item.Coordinate.X;
                    newY = Item.Coordinate.Y - 1;
                }
                else if (roomUserByUserId.Coordinate == point4)
                {
                    newX = Item.Coordinate.X;
                    newY = Item.Coordinate.Y + 1;
                }

                if (!Item.GetRoom().GetGameMap().CanStackItem(newX, newY))
                {
                    return;
                }

                int oldX = Item.X;
                int oldY = Item.Y;
                double oldZ = Item.Z;
                double newZ = Item.GetRoom().GetGameMap().SqAbsoluteHeight(newX, newY);
                if (Item.GetRoom().GetRoomItemHandler().SetFloorItem(roomUserByUserId.GetClient(), Item, newX, newY, Item.Rotation, false, false, false))
                {
                    Item.GetRoom().SendPacket(new SlideObjectBundleComposer(oldX, oldY, oldZ, newX, newY, newZ, Item.Id));
                }
            }
        }

        public override void OnTick(Item item)
        {
        }
    }
}
