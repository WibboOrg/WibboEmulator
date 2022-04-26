using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands
{
    public interface IChatCommand
    {
        void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params);
        /// void Execute(Client Session, Room Room, string[] Params);
    }
}
