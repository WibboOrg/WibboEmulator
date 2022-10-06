namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ForceControlUser : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var username = parameters[1];

        var roomUserByUserId = session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByName(username);
        if (roomUserByUserId == null || roomUserByUserId.GetClient() == null)
        {
            return;
        }

        if (session.Langue != roomUserByUserId.GetClient().Langue)
        {
            session.SendWhisper(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.langue.user", roomUserByUserId.GetClient().Langue), session.Langue));
            return;
        }

        session.GetUser().ControlUserId = roomUserByUserId.GetClient().GetUser().Id;

    }
}
