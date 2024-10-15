namespace WibboEmulator.Games.Chats.Commands.User.Build;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ForceRot : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        _ = int.TryParse(parameters[1], out var result);
        if (result is <= (-1) or >= 7)
        {
            session.User.ForceRot = 0;
        }
        else
        {
            session.User.ForceRot = result;
        }

        session.SendWhisper("Rot: " + result);
    }
}
