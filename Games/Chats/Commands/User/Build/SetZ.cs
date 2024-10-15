namespace WibboEmulator.Games.Chats.Commands.User.Build;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class SetZ : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var heigth = parameters[1];
        if (!double.TryParse(heigth, out var result))
        {
            return;
        }

        if (result < -100)
        {
            result = 0;
        }

        if (result > 100)
        {
            result = 100;
        }

        userRoom.BuildToolStackHeight = true;
        userRoom.BuildToolHeight = result;

        Session.SendWhisper("SetZ: " + result);

        if (result >= 0)
        {
            Session.SendPacket(room.GameMap.Model.SetHeightMap(result > 63 ? 63 : result));
        }
    }
}
