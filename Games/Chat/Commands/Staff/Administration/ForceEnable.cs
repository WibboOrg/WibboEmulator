namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ForceEnable : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        _ = int.TryParse(parameters[1], out var EnableNum);

        if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(EnableNum, session.GetUser().HasPermission("perm_god")))
        {
            return;
        }

        UserRoom.ApplyEffect(EnableNum);
    }
}
