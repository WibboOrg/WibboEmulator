using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Furni;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using System;
using System.Drawing;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorLoveLock : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
            Item.InteractingUser = 0;
            Item.InteractingUser2 = 0;
        }

        public override void OnRemove(Client Session, Item Item)
        {
            Item.InteractingUser = 0;
            Item.InteractingUser2 = 0;
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights)
        {
            RoomUser User = null;

            if (!UserHasRights)
            {
                return;
            }

            if (Session != null)
            {
                User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            }

            if (User == null)
            {
                return;
            }

            if (Gamemap.TilesTouching(Item.X, Item.Y, User.X, User.Y))
            {
                if (Item.ExtraData == null || Item.ExtraData.Length <= 1 || !Item.ExtraData.Contains(Convert.ToChar(5).ToString()))
                {
                    Point pointOne;
                    Point pointTwo;

                    switch (Item.Rotation)
                    {
                        case 0:
                        case 2:
                            pointOne = new Point(Item.X, Item.Y + 1);
                            pointTwo = new Point(Item.X, Item.Y - 1);
                            break;

                        case 4:
                        case 6:
                            pointOne = new Point(Item.X - 1, Item.Y);
                            pointTwo = new Point(Item.X + 1, Item.Y);
                            break;

                        default:
                            return;
                    }

                    RoomUser UserOne = Item.GetRoom().GetRoomUserManager().GetUserForSquare(pointOne.X, pointOne.Y);
                    RoomUser UserTwo = Item.GetRoom().GetRoomUserManager().GetUserForSquare(pointTwo.X, pointTwo.Y);

                    if (UserOne == null || UserTwo == null)
                    {
                        return;
                    }

                    if (UserOne.GetClient() == null || UserTwo.GetClient() == null)
                    {
                        return;
                    }

                    UserOne.CanWalk = false;
                    UserTwo.CanWalk = false;

                    Item.InteractingUser = UserOne.GetClient().GetHabbo().Id;
                    Item.InteractingUser2 = UserTwo.GetClient().GetHabbo().Id;

                    UserOne.GetClient().SendPacket(new LoveLockDialogueComposer(Item.Id));
                    UserTwo.GetClient().SendPacket(new LoveLockDialogueComposer(Item.Id));
                }
            }
            else
            {
                User.MoveTo(Item.SquareInFront);
            }
        }
    }
}
