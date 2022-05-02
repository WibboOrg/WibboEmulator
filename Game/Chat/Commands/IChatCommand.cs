using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands
{
    public interface IChatCommand
    {
        ///public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params);
    }
}
