namespace WibboEmulator.Games.Chats.Commands.User.Casino;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class Poke : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 4)
        {
            return;
        }

        if (userRoom.Team != TeamType.None || userRoom.InGame || room.IsGameMode)
        {
            return;
        }

        if (Session.User.IsSpectator)
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

        if (wpCount > Session.User.WibboPoints)
        {
            return;
        }

        var TargetUser = room.RoomUserManager.GetRoomUserByName(username);
        if (TargetUser == null || TargetUser.Client == null || TargetUser.Client.User == null)
        {
            return;
        }

        if (wpCount > TargetUser.Client.User.WibboPoints)
        {
            return;
        }


    }
}
