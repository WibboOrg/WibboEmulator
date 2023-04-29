namespace WibboEmulator.Games.Chats.Commands.User.Premium;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class Slime : IChatCommand
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

        userRoom.OnChat($"*Jette du slime dans la direction de {targetUser.GetUsername()}*");
        targetUser.OnChat($"*Je suis couvert de slime à cause de {userRoom.GetUsername()}*");

        targetUser.ApplyEffect(169, true);
        targetUser.TimerResetEffect = 6;
    }
}