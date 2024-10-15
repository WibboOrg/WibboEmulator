namespace WibboEmulator.Games.Chats.Commands.User.Build;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Construit : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var heigth = parameters[1];
        if (double.TryParse(heigth, out var result))
        {
            if (result is >= 0.01 and <= 10)
            {
                userRoom.BuildToolEnable = true;
                userRoom.BuildToolHeight = result;
            }
        }

        Session.SendWhisper("Construit: " + result);
    }
}
