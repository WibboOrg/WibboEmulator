using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands
{
    public interface IChatCommand
    {
        //public string Commands { get; set; }

        void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params);
    }
}
