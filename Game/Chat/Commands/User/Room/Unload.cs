using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class Unload : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(Session.GetUser().CurrentRoom);
        }
    }
}
