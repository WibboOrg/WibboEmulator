using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class UnloadRoom : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(int.Parse(Params[1]));
            if (room == null)
            {
                return;
            }

            ButterflyEnvironment.GetGame().GetRoomManager().UnloadRoom(room);

        }
    }
}