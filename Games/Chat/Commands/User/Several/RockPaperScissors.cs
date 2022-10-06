namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class RockPaperScissors : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 2)
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
