using Butterfly.Game.Clients;

namespace Butterfly.Game.Rooms.Chat.Commands
{
    public interface IChatCommand
    {
        //public string Commands { get; set; }

        void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params);
    }
}
