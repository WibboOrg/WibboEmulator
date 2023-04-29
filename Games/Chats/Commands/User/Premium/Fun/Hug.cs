namespace WibboEmulator.Games.Chats.Commands.User.Premium;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class Hug : IChatCommand
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

        userRoom.OnChat($"*Serre {targetUser.GetUsername()} dans ses bras*", 16);
        targetUser.OnChat($"*Se fait câlinner chaleureusement par {userRoom.GetUsername()}*", 16);

        userRoom.ApplyEffect(9, true);
        userRoom.TimerResetEffect = 6;

        targetUser.ApplyEffect(9, true);
        targetUser.TimerResetEffect = 6;
    }
}