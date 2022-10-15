namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class RockPaperScissors : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        if (userRoom.Team != TeamType.None || userRoom.InGame)
        {
            return;
        }

        if (session.User.SpectatorMode)
        {
            return;
        }

        var username = parameters[1];

        if (string.IsNullOrWhiteSpace(username))
        {
            return;
        }

        var roomUserTarget = room.RoomUserManager.GetRoomUserByName(username);
        if (roomUserTarget == null)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
            return;
        }

        if (roomUserTarget.UserId == userRoom.UserId)
        {
            return;
        }

        room.JankenManager.Start(userRoom, roomUserTarget);
    }
}
