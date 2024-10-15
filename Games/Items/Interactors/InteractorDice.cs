namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.Map;

public class InteractorDice : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item)
    {
        item.ExtraData = "0";
        item.UpdateState();
    }

    public override void OnRemove(GameClient session, Item item)
    {
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (session == null)
        {
            return;
        }

        var roomUser = item.Room.RoomUserManager.GetRoomUserByUserId(session.User.Id);

        if (roomUser == null)
        {
            return;
        }

        if (GameMap.TilesTouching(item.X, item.Y, roomUser.X, roomUser.Y))
        {
            if (item.ExtraData == "-1")
            {
                return;
            }

            if (request == -1)
            {
                item.ExtraData = "0";
                item.UpdateState();

                if (roomUser.DiceCounterAmount > 0 && !roomUser.InGame)
                {
                    roomUser.DiceCounterAmount = 0;
                    roomUser.DiceCounter = 0;
                    roomUser.OnChat($"Dée: remise à 0 ({roomUser.Username})", 33);
                }
            }
            else
            {
                item.ExtraData = "-1";
                item.UpdateState(false);
                item.ReqUpdate(4);

                item.InteractingUser = roomUser.UserId;
            }
        }
        else
        {
            roomUser.MoveTo(item.SquareInFront);
        }
    }

    public override void OnTick(Item item)
    {
        var numberDice = WibboEnvironment.GetRandomNumber(1, 6);

        item.ExtraData = numberDice.ToString();
        item.UpdateState();

        var user = item.Room.RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
        if (user != null)
        {
            if (!user.InGame)
            {
                user.DiceCounterAmount += numberDice;
                user.DiceCounter++;
                user.OnChat($"Dée {user.DiceCounter}: +{numberDice} = {user.DiceCounterAmount} ({user.Username})", 33);
            }
        }
    }
}
