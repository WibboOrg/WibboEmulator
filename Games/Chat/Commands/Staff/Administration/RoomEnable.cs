namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomEnable : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (!int.TryParse(parameters[1], out var NumEnable))
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(NumEnable, session.GetUser().HasPermission("perm_god")))
        {
            return;
        }

        foreach (var User in Room.GetRoomUserManager().GetUserList().ToList())
        {
            if (!User.IsBot)
            {
                User.ApplyEffect(NumEnable);
            }
        }
    }
}
