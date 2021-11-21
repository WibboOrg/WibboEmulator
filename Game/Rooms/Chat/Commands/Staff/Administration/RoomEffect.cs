using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
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

            ServerPacket RoomEffectComposer = new ServerPacket(4001);
            RoomEffectComposer.WriteInteger(number);
            Room.SendPacket(RoomEffectComposer);
        }
    }
}
