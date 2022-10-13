namespace WibboEmulator.Games.Chat.Commands.User.Build;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class SetZ : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
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

        userRoom.ConstruitZMode = true;
        userRoom.ConstruitHeigth = result;

        session.SendWhisper("SetZ: " + result);

        if (result >= 0)
        {
            session.SendPacket(room.GameMap.Model.SetHeightMap(result > 63 ? 63 : result));
        }
    }
}
