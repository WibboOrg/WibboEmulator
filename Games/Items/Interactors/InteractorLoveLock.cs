namespace WibboEmulator.Games.Items.Interactors;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni.Furni;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

public class InteractorLoveLock : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item)
    {
        item.InteractingUser = 0;
        item.InteractingUser2 = 0;
    }

    public override void OnRemove(GameClient session, Item item)
    {
        item.InteractingUser = 0;
        item.InteractingUser2 = 0;
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        RoomUser User = null;

        if (!userHasRights)
        {
            return;
        }

        if (session != null)
        {
            User = item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
        }

        if (User == null)
        {
            return;
        }

        if (Gamemap.TilesTouching(item.X, item.Y, User.X, User.Y))
        {
            if (item.ExtraData == null || item.ExtraData.Length <= 1 || !item.ExtraData.Contains(Convert.ToChar(5).ToString()))
            {
                Point pointOne;
                Point pointTwo;

                switch (item.Rotation)
                {
                    case 0:
                    case 2:
                        pointOne = new Point(item.X, item.Y + 1);
                        pointTwo = new Point(item.X, item.Y - 1);
                        break;

                    case 4:
                    case 6:
                        pointOne = new Point(item.X - 1, item.Y);
                        pointTwo = new Point(item.X + 1, item.Y);
                        break;

                    default:
                        return;
                }

                var UserOne = item.GetRoom().GetRoomUserManager().GetUserForSquare(pointOne.X, pointOne.Y);
                var UserTwo = item.GetRoom().GetRoomUserManager().GetUserForSquare(pointTwo.X, pointTwo.Y);

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

                item.InteractingUser = UserOne.GetClient().GetUser().Id;
                item.InteractingUser2 = UserTwo.GetClient().GetUser().Id;

                UserOne.GetClient().SendPacket(new LoveLockDialogueComposer(item.Id));
                UserTwo.GetClient().SendPacket(new LoveLockDialogueComposer(item.Id));
            }
        }
        else
        {
            User.MoveTo(item.SquareInFront);
        }
    }

    public override void OnTick(Item item)
    {
    }
}
