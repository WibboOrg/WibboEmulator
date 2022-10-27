namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ForceEnableUser : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 3)
        {
            return;
        }

        var username = parameters[1];

        var roomUserByUserId = session.User.CurrentRoom.RoomUserManager.GetRoomUserByName(username);
        if (roomUserByUserId == null || roomUserByUserId.Client == null)
        {
            return;
        }

        if (session.Langue != roomUserByUserId.Client.Langue)
        {
            session.SendWhisper(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.langue.user", session.Langue), roomUserByUserId.Client.Langue));
            return;
        }

        if (!int.TryParse(parameters[2], out var effectId))
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(effectId, session.User.HasPermission("perm_god")))
        {
            return;
        }

        roomUserByUserId.ApplyEffect(effectId);
    }
}
