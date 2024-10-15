namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.Effects;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ForceEnableUser : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 3)
        {
            return;
        }

        var username = parameters[1];

        var roomUserByUserId = session.User.Room.RoomUserManager.GetRoomUserByName(username);
        if (roomUserByUserId == null || roomUserByUserId.Client == null)
        {
            return;
        }

        if (session.Language != roomUserByUserId.Client.Language)
        {
            session.SendWhisper(string.Format(LanguageManager.TryGetValue("cmd.authorized.langue.user", session.Language), roomUserByUserId.Client.Language));
            return;
        }

        if (!int.TryParse(parameters[2], out var effectId))
        {
            return;
        }

        if (!EffectManager.HasEffect(effectId, session.User.HasPermission("god")))
        {
            return;
        }

        roomUserByUserId.ApplyEffect(effectId);
    }
}
