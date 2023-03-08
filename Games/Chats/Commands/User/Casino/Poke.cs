namespace WibboEmulator.Games.Chats.Commands.User.Casino;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class Poke : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 4)
        {
            return;
        }

        if (userRoom.Team != TeamType.None || userRoom.InGame || room.IsGameMode)
        {
            return;
        }

        if (session.User.SpectatorMode)
        {
            return;
        }

        var username = parameters[1];
        _ = int.TryParse(parameters[2], out var diceCount);
        _ = int.TryParse(parameters[3], out var wpCount);

        if (string.IsNullOrWhiteSpace(username))
        {
            return;
        }

        if (diceCount is > 5 or < 1)
        {
            return;
        }

        if (wpCount > session.User.WibboPoints)
        {
            return;
        }

        var targetUser = room.RoomUserManager.GetRoomUserByName(username);
        if (targetUser == null || targetUser.Client == null || targetUser.Client.User == null)
        {
            return;
        }

        if (wpCount > targetUser.Client.User.WibboPoints)
        {
            return;
        }


    }
}
