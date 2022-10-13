namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class Pull : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.Team != TeamType.None || userRoom.InGame)
        {
            return;
        }

        if (!room.PushPullAllowed)
        {
            return;
        }

        if (parameters.Length != 2)
        {
            return;
        }

        var targetUser = room.RoomUserManager.GetRoomUserByName(Convert.ToString(parameters[1]));
        if (targetUser == null || targetUser.Client == null || targetUser.Client.GetUser() == null)
        {
            return;
        }

        if (targetUser.Client.GetUser().Id == session.GetUser().Id)
        {
            return;
        }

        if (targetUser.Client.GetUser().PremiumProtect && !session.GetUser().HasPermission("perm_mod"))
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", session.Langue));
            return;
        }

        if (userRoom.SetX - 1 == room.GameMap.Model.DoorX)
        {
            return;
        }

        if (Math.Abs(userRoom.X - targetUser.X) < 3 && Math.Abs(userRoom.Y - targetUser.Y) < 3)
        {
            userRoom.OnChat("*Tire " + parameters[1] + "*", 0, false);
            if (userRoom.RotBody % 2 != 0)
            {
                userRoom.RotBody--;
            }

            if (userRoom.RotBody == 0)
            {
                targetUser.MoveTo(userRoom.X, userRoom.Y - 1);
            }
            else if (userRoom.RotBody == 2)
            {
                targetUser.MoveTo(userRoom.X + 1, userRoom.Y);
            }
            else if (userRoom.RotBody == 4)
            {
                targetUser.MoveTo(userRoom.X, userRoom.Y + 1);
            }
            else if (userRoom.RotBody == 6)
            {
                targetUser.MoveTo(userRoom.X - 1, userRoom.Y);
            }
        }
        else
        {
            session.SendWhisper(parameters[1] + " est trop loin de vous.");
            return;
        }
    }
}
