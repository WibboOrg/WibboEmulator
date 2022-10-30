namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class Push : IChatCommand
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

        var targetRoomUser = room.RoomUserManager.GetRoomUserByName(Convert.ToString(parameters[1]));

        if (targetRoomUser == null)
        {
            userRoom.SendWhisperChat(Convert.ToString(parameters[1]) + " n'est plus ici.");
            return;
        }

        if (targetRoomUser.Client.User.Id == session.User.Id)
        {
            return;
        }

        if (targetRoomUser.Client.User.PremiumProtect && !session.User.HasPermission("mod"))
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", session.Langue));
            return;
        }

        if (!(Math.Abs(targetRoomUser.X - userRoom.X) >= 2) || Math.Abs(targetRoomUser.Y - userRoom.Y) >= 2)
        {
            if (targetRoomUser.SetX - 1 == room.GameMap.Model.DoorX)
            {
                return;
            }
        }

        if (!(Math.Abs(targetRoomUser.X - userRoom.X) >= 2 || Math.Abs(targetRoomUser.Y - userRoom.Y) >= 2))
        {
            if (userRoom.RotBody == 4)
            { targetRoomUser.MoveTo(targetRoomUser.X, targetRoomUser.Y + 1); }

            if (userRoom.RotBody == 0)
            { targetRoomUser.MoveTo(targetRoomUser.X, targetRoomUser.Y - 1); }

            if (userRoom.RotBody == 6)
            { targetRoomUser.MoveTo(targetRoomUser.X - 1, targetRoomUser.Y); }

            if (userRoom.RotBody == 2)
            { targetRoomUser.MoveTo(targetRoomUser.X + 1, targetRoomUser.Y); }

            if (userRoom.RotBody == 3)
            {
                targetRoomUser.MoveTo(targetRoomUser.X + 1, targetRoomUser.Y);
                targetRoomUser.MoveTo(targetRoomUser.X, targetRoomUser.Y + 1);
            }

            if (userRoom.RotBody == 1)
            {
                targetRoomUser.MoveTo(targetRoomUser.X + 1, targetRoomUser.Y);
                targetRoomUser.MoveTo(targetRoomUser.X, targetRoomUser.Y - 1);
            }

            if (userRoom.RotBody == 7)
            {
                targetRoomUser.MoveTo(targetRoomUser.X - 1, targetRoomUser.Y);
                targetRoomUser.MoveTo(targetRoomUser.X, targetRoomUser.Y - 1);
            }

            if (userRoom.RotBody == 5)
            {
                targetRoomUser.MoveTo(targetRoomUser.X - 1, targetRoomUser.Y);
                targetRoomUser.MoveTo(targetRoomUser.X, targetRoomUser.Y + 1);
            }

            userRoom.OnChat("*pousse " + parameters[1] + "*", 0, false);
        }
        else
        {
            userRoom.SendWhisperChat(parameters[1] + " est trop loin de vous.");
        }
    }
}
