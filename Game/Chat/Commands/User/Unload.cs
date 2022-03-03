using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Unload : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            ButterflyEnvironment.GetGame().GetRoomManager().UnloadRoom(Session.GetUser().CurrentRoom);
        }
    }
}
