using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands
{
    public interface IChatCommand
    {
        //public string Commands { get; set; }

        void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params);
    }
}
