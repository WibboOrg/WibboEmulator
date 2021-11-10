using Butterfly.Game.GameClients;

namespace Butterfly.Game.Rooms.Chat.Commands
{
    public interface IChatCommand
    {
        //public string Commands { get; set; }

        void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params);
    }
}
