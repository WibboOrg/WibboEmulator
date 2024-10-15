namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;

using WibboEmulator.Games.Effects;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomEnable : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (!int.TryParse(parameters[1], out var effectId))
        {
            return;
        }

        if (!EffectManager.HasEffect(effectId, Session.User.HasPermission("god")))
        {
            return;
        }

        foreach (var user in room.RoomUserManager.UserList.ToList())
        {
            if (!user.IsBot)
            {
                user.ApplyEffect(effectId);
            }
        }
    }
}
