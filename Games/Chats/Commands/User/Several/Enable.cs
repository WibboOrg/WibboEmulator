namespace WibboEmulator.Games.Chats.Commands.User.Several;

using WibboEmulator.Games.Effects;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class Enable : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        if (!int.TryParse(parameters[1], out var numEnable))
        {
            return;
        }

        if (!EffectManager.HasEffect(numEnable, Session.User.HasPermission("god")))
        {
            return;
        }

        if (userRoom.Team != TeamType.None || userRoom.InGame || room.IsGameMode)
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
