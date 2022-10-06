namespace WibboEmulator.Games.Chat.Commands.User.Casino;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class Poke : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 4)
        {
            return;
        }

        if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
        {
            return;
        }

        if (session.GetUser().SpectatorMode)
        {
            return;
        }

        var Username = parameters[1];
        _ = int.TryParse(parameters[2], out var DiceCount);
        _ = int.TryParse(parameters[3], out var WpCount);

        if (string.IsNullOrWhiteSpace(Username))
        {
            return;
        }

        if (DiceCount is > 5 or < 1)
        {
            return;
        }

        if (WpCount > session.GetUser().WibboPoints)
        {
            return;
        }

        var TargetUser = Room.GetRoomUserManager().GetRoomUserByName(Username);
        if (TargetUser == null || TargetUser.GetClient() == null || TargetUser.GetClient().GetUser() == null)
        {
            return;
        }

        if (WpCount > TargetUser.GetClient().GetUser().WibboPoints)
        {
            return;
        }


    }
}
