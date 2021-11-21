using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.GameClients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class RoomEffect : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
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

            Room.SendPacket(new RoomEFfectComposer(number));

            /*ServerPacket RoomEffectComposer = new ServerPacket(4001);
            RoomEffectComposer.WriteInteger(number);
            Room.SendPacket(RoomEffectComposer);*/
        }
    }
}
