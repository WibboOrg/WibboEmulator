namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomEnable : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (!int.TryParse(parameters[1], out var effectId))
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(effectId, session.GetUser().HasPermission("perm_god")))
        {
            return;
        }

        foreach (var user in room.RoomUserManager.GetUserList().ToList())
        {
            if (!user.IsBot)
            {
                user.ApplyEffect(effectId);
            }
        }
    }
}
