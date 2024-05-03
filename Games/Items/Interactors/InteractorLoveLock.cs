namespace WibboEmulator.Games.Items.Interactors;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni.Furni;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map;

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
        RoomUser user = null;

        if (!userHasRights)
        {
            return;
        }

        if (session != null)
        {
            user = item.Room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        }

        if (user == null)
        {
            return;
        }

        if (GameMap.TilesTouching(item.X, item.Y, user.X, user.Y))
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

                var userOne = item.Room.RoomUserManager.GetUserForSquare(pointOne.X, pointOne.Y);
                var userTwo = item.Room.RoomUserManager.GetUserForSquare(pointTwo.X, pointTwo.Y);

                if (userOne == null || userTwo == null)
                {
                    return;
                }

                if (userOne.Client == null || userTwo.Client == null)
                {
                    return;
                }

                userOne.CanWalk = false;
                userTwo.CanWalk = false;

                item.InteractingUser = userOne.Client.User.Id;
                item.InteractingUser2 = userTwo.Client.User.Id;

                userOne.Client.SendPacket(new LoveLockDialogueComposer(item.Id));
                userTwo.Client.SendPacket(new LoveLockDialogueComposer(item.Id));
            }
        }
        else
        {
            user.MoveTo(item.SquareInFront);
        }
    }

    public override void OnTick(Item item)
    {
    }
}
