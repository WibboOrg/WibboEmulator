namespace WibboEmulator.Games.Chats.Commands.User.Several;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class RockPaperScissors : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
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

        if (string.IsNullOrWhiteSpace(username))
        {
            return;
        }

        var roomUserTarget = room.RoomUserManager.GetRoomUserByName(username);
        if (roomUserTarget == null)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("input.usernotfound", Session.Language));
            return;
        }

        if (roomUserTarget.UserId == userRoom.UserId)
        {
            return;
        }

        room.JankenManager.Start(userRoom, roomUserTarget);
    }
}
