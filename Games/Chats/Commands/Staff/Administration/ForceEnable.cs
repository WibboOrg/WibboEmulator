namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;

using WibboEmulator.Games.Effects;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ForceEnable : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        _ = int.TryParse(parameters[1], out var effectId);

        if (!EffectManager.HasEffect(effectId, Session.User.HasPermission("god")))
        {
            return;
        }

        userRoom.ApplyEffect(effectId);
    }
}
