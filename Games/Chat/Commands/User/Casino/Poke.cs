namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games;

internal class Poke : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 4)
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

        var Username = Params[1];
        int.TryParse(Params[2], out var DiceCount);
        int.TryParse(Params[3], out var WpCount);

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
