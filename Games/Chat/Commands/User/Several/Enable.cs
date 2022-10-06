namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class Enable : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        if (!int.TryParse(parameters[1], out var NumEnable))
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(NumEnable, session.GetUser().HasPermission("perm_god")))
        {
            return;
        }

        if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
        {
            return;
        }

        var CurrentEnable = UserRoom.CurrentEffect;
        if (CurrentEnable is 28 or 29 or 30 or 184)
        {
            return;
        }

        UserRoom.ApplyEffect(NumEnable);
    }
}
