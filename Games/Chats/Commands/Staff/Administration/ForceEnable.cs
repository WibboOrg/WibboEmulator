namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ForceEnable : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        _ = int.TryParse(parameters[1], out var effectId);

        if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(effectId, session.User.HasPermission("god")))
        {
            return;
        }

        userRoom.ApplyEffect(effectId);
    }
}
