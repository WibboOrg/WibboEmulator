namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ForceEnable : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 2)
        {
            return;
        }

        int.TryParse(Params[1], out var EnableNum);

        if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(EnableNum, session.GetUser().HasPermission("perm_god")))
        {
            return;
        }

        UserRoom.ApplyEffect(EnableNum);
    }
}
