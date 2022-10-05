namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games;

internal class RockPaperScissors : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 2)
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

        if (string.IsNullOrWhiteSpace(Username))
        {
            return;
        }

        var roomUserTarget = Room.GetRoomUserManager().GetRoomUserByName(Username);
        if (roomUserTarget == null)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
            return;
        }

        if (roomUserTarget.UserId == UserRoom.UserId)
        {
            return;
        }

        var Jankan = Room.GetJanken();
        Jankan.Start(UserRoom, roomUserTarget);
    }
}
