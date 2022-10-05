namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games;

internal class Enable : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 2)
        {
            return;
        }

        if (!int.TryParse(Params[1], out var NumEnable))
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
