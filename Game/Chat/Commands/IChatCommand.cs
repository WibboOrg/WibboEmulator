using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands
{
    public interface IChatCommand
    {
        void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params);
    }
}
