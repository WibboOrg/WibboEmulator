namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomEffect : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        int.TryParse(parameters[1], out var number);

        if (number > 3)
        {
            number = 3;
        }
        else if (number < 0)
        {
            number = 0;
        }

        Room.SendPacket(new RoomEffectComposer(number));
    }
}