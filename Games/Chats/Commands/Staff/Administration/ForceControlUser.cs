namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ForceControlUser : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var username = parameters[1];

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByName(username);
        if (roomUserByUserId == null || roomUserByUserId.Client == null)
        {
            return;
        }

        if (session.Language != roomUserByUserId.Client.Language)
        {
            session.SendWhisper(string.Format(LanguageManager.TryGetValue("cmd.authorized.langue.user", roomUserByUserId.Client.Language), session.Language));
            return;
        }

        session.User.ControlUserId = roomUserByUserId.Client.User.Id;

    }
}
