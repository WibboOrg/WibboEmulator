namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ForceEnableUser : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 3)
        {
            return;
        }

        var Username = Params[1];

        var roomUserByUserId = session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByName(Username);
        if (roomUserByUserId == null || roomUserByUserId.GetClient() == null)
        {
            return;
        }

        if (session.Langue != roomUserByUserId.GetClient().Langue)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", roomUserByUserId.GetClient().Langue), session.Langue));
            return;
        }

        if (!int.TryParse(Params[2], out var NumEnable))
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(NumEnable, session.GetUser().HasPermission("perm_god")))
        {
            return;
        }

        roomUserByUserId.ApplyEffect(NumEnable);
    }
}
