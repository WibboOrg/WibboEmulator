namespace WibboEmulator.Games.Chats.Commands.User.Build;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ForceRot : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        _ = int.TryParse(parameters[1], out var result);
        if (result is <= (-1) or >= 7)
        {
            Session.User.ForceRot = 0;
        }
        else
        {
            Session.User.ForceRot = result;
        }

        Session.SendWhisper("Rot: " + result);
    }
}
