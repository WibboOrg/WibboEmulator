namespace WibboEmulator.Games.Chats.Commands.User.Premium;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class Tied : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.Team != TeamType.None || userRoom.InGame || room.IsGameMode)
        {
            return;
        }

        if (parameters.Length != 2)
        {
            return;
        }

        var targetUser = room.RoomUserManager.GetRoomUserByName(parameters[1]);
        if (targetUser == null || targetUser.Client == null || targetUser.Client.User == null)
        {
            return;
        }

        if (targetUser.Client.User.Id == session.User.Id)
        {
            return;
        }

        if (targetUser.Client.User.PremiumProtect && !session.User.HasPermission("mod"))
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", session.Langue));
            return;
        }

        if (Math.Abs(targetUser.X - userRoom.X) >= 2 || Math.Abs(targetUser.Y - userRoom.Y) >= 2)
        {
            return;
        }

        var timeSpan = DateTime.Now - session.User.CommandFunTimer;
        if (timeSpan.TotalSeconds < 10)
        {
            userRoom.SendWhisperChat($"Veuillez patienter pendant {timeSpan.TotalSeconds} secondes avant de pouvoir réutiliser la commande fun.");
            return;
        }

        session.User.CommandFunTimer = DateTime.Now;

        userRoom.OnChat($"*Ligote {targetUser.GetUsername()} avec une corde*", 32);
        targetUser.OnChat($"*S'est ligoter par {userRoom.GetUsername()}*", 18);

        targetUser.ApplyEffect(729, true);
        targetUser.TimerResetEffect = 6;
    }
}
