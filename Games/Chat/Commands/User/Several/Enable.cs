namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class Enable : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        if (!int.TryParse(parameters[1], out var numEnable))
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(numEnable, session.GetUser().HasPermission("perm_god")))
        {
            return;
        }

        if (userRoom.Team != TeamType.NONE || userRoom.InGame)
        {
            return;
        }

        var currentEnable = userRoom.CurrentEffect;
        if (currentEnable is 28 or 29 or 30 or 184)
        {
            return;
        }

        userRoom.ApplyEffect(numEnable);
    }
}
