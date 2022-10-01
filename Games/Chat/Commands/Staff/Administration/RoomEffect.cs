using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class RoomEffect : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            int.TryParse(Params[1], out int number);

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
}